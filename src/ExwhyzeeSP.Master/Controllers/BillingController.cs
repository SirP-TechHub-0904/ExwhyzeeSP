using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExwhyzeeSP.Core.Models;
using ExwhyzeeSP.Data.Contexts;
using ExwhyzeeSP.Services;

namespace ExwhyzeeSP.Master.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{MasterRoles.SuperAdmin},{MasterRoles.Accountant},{MasterRoles.Admin}")]
    public class BillingController : Controller
    {
        private readonly MasterDbContext _context;
        private readonly IEmailLoggingService _emailLoggingService;
        private readonly IAuditLogService _auditService;

        public BillingController(
            MasterDbContext context,
            IEmailLoggingService emailLoggingService,
            IAuditLogService auditService)
        {
            _context = context;
            _emailLoggingService = emailLoggingService;
            _auditService = auditService;
        }

        // GET: /Billing
        public async Task<IActionResult> Index(
            int? schoolId,
            SchoolPaymentStatus? status,
            SchoolBillingType? billingType,
            string? search,
            DateTime? startDate,
            DateTime? endDate,
            bool includeArchived = false,
            int page = 1,
            int pageSize = 15)
        {
            if (page < 1) page = 1;
            if (pageSize < 5) pageSize = 15;
            if (pageSize > 100) pageSize = 100;

            IQueryable<MasterSchoolTransaction> query;
            if (includeArchived)
            {
                query = _context.SchoolTransactions
                    .IgnoreQueryFilters()
                    .Include(t => t.TenantSchool)
                    .AsNoTracking()
                    .AsQueryable();
            }
            else
            {
                query = _context.SchoolTransactions
                    .Include(t => t.TenantSchool)
                    .Where(t => t.TenantSchool != null && !t.TenantSchool.IsDeleted)
                    .AsNoTracking()
                    .AsQueryable();
            }

            if (schoolId.HasValue && schoolId.Value > 0)
            {
                query = query.Where(t => t.TenantSchoolId == schoolId.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(t => t.Status == status.Value);
            }

            if (billingType.HasValue)
            {
                query = query.Where(t => t.BillingType == billingType.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(t =>
                    t.InvoiceNumber.ToLower().Contains(search) ||
                    t.Title.ToLower().Contains(search) ||
                    (t.ReceiptNumber != null && t.ReceiptNumber.ToLower().Contains(search)) ||
                    (t.TenantSchool != null && t.TenantSchool.SchoolName.ToLower().Contains(search)));
            }

            if (startDate.HasValue)
            {
                query = query.Where(t => t.CreatedAt >= startDate.Value.Date);
            }

            if (endDate.HasValue)
            {
                query = query.Where(t => t.CreatedAt <= endDate.Value.Date.AddDays(1).AddTicks(-1));
            }

            var totalItems = await query.CountAsync();

            var transactions = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Financial KPIs scoped to active/filtered scope
            var baseKpiQuery = includeArchived
                ? _context.SchoolTransactions.IgnoreQueryFilters().AsQueryable()
                : _context.SchoolTransactions.Include(t => t.TenantSchool).Where(t => t.TenantSchool != null && !t.TenantSchool.IsDeleted).AsQueryable();

            ViewBag.TotalCollected = await baseKpiQuery.SumAsync(t => (decimal?)t.AmountPaid) ?? 0m;
            ViewBag.TotalPending = await baseKpiQuery
                .Where(t => t.Status == SchoolPaymentStatus.Pending || t.Status == SchoolPaymentStatus.PartiallyPaid)
                .SumAsync(t => (decimal?)(t.Amount - t.AmountPaid)) ?? 0m;
            ViewBag.TotalOverdue = await baseKpiQuery
                .Where(t => t.Status == SchoolPaymentStatus.Overdue)
                .SumAsync(t => (decimal?)(t.Amount - t.AmountPaid)) ?? 0m;
            ViewBag.TotalInvoicesCount = await baseKpiQuery.CountAsync();
            ViewBag.PaidInvoicesCount = await baseKpiQuery.CountAsync(t => t.Status == SchoolPaymentStatus.Paid);

            // Filter Dropdowns
            var schoolsList = includeArchived
                ? await _context.TenantSchools.IgnoreQueryFilters().AsNoTracking().OrderBy(s => s.SchoolName).ToListAsync()
                : await _context.TenantSchools.AsNoTracking().OrderBy(s => s.SchoolName).ToListAsync();

            ViewBag.Schools = new SelectList(schoolsList, "Id", "SchoolName", schoolId);
            ViewBag.SelectedSchoolId = schoolId;
            ViewBag.SelectedStatus = status;
            ViewBag.SelectedBillingType = billingType;
            ViewBag.Search = search;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");
            ViewBag.IncludeArchived = includeArchived;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            return View(transactions);
        }

        // POST: /Billing/DeleteInvoice/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{MasterRoles.SuperAdmin},{MasterRoles.Admin}")]
        public async Task<IActionResult> DeleteInvoice(long id)
        {
            var tx = await _context.SchoolTransactions.IgnoreQueryFilters().FirstOrDefaultAsync(t => t.Id == id);
            if (tx != null)
            {
                _context.SchoolTransactions.Remove(tx);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Invoice #{tx.InvoiceNumber} has been permanently deleted.";
            }
            else
            {
                TempData["ErrorMessage"] = "Invoice record not found.";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: /Billing/PurgeDecommissionedInvoices
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{MasterRoles.SuperAdmin},{MasterRoles.Admin}")]
        public async Task<IActionResult> PurgeDecommissionedInvoices()
        {
            var orphanedTx = await _context.SchoolTransactions
                .IgnoreQueryFilters()
                .Where(t => t.TenantSchool == null || t.TenantSchool.IsDeleted)
                .ToListAsync();

            if (orphanedTx.Any())
            {
                var count = orphanedTx.Count;
                _context.SchoolTransactions.RemoveRange(orphanedTx);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Successfully purged {count} transaction(s) belonging to decommissioned schools.";
            }
            else
            {
                TempData["InfoMessage"] = "No decommissioned school transactions found to purge.";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /Billing/Create
        public async Task<IActionResult> Create(int? schoolId)
        {
            var schools = await _context.TenantSchools.OrderBy(s => s.SchoolName).ToListAsync();
            ViewBag.Schools = new SelectList(schools, "Id", "SchoolName", schoolId);

            var nextInvoiceNum = $"INV-{DateTime.UtcNow.Year}-{(_context.SchoolTransactions.Count() + 101):D5}";
            var model = new MasterSchoolTransaction
            {
                InvoiceNumber = nextInvoiceNum,
                DueDate = DateTime.UtcNow.AddDays(14),
                Status = SchoolPaymentStatus.Pending,
                BillingType = SchoolBillingType.TermlySubscription,
                PaymentChannel = SchoolPaymentChannel.BankTransfer
            };

            if (schoolId.HasValue && schoolId.Value > 0)
            {
                model.TenantSchoolId = schoolId.Value;
                var school = schools.FirstOrDefault(s => s.Id == schoolId.Value);
                if (school != null)
                {
                    model.Title = $"{school.SchoolName} - Termly Portal Subscription & Support";
                }
            }

            return View(model);
        }

        // POST: /Billing/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MasterSchoolTransaction model, bool sendEmailImmediately = false, string? recipientEmail = null)
        {
            if (ModelState.IsValid)
            {
                if (model.Status == SchoolPaymentStatus.Paid && model.AmountPaid == 0)
                {
                    model.AmountPaid = model.Amount;
                }

                if (model.Status == SchoolPaymentStatus.Paid && !model.PaidDate.HasValue)
                {
                    model.PaidDate = DateTime.UtcNow;
                }

                if (model.Status == SchoolPaymentStatus.Paid && string.IsNullOrWhiteSpace(model.ReceiptNumber))
                {
                    model.ReceiptNumber = $"REC-{DateTime.UtcNow.Year}-{(_context.SchoolTransactions.Count() + 101):D5}";
                }

                model.RecordedBy = User.Identity?.Name ?? "Admin";
                model.CreatedAt = DateTime.UtcNow;

                _context.SchoolTransactions.Add(model);
                await _context.SaveChangesAsync();

                // Audit Log
                await _auditService.LogAsync(
                    action: "InvoiceCreated",
                    details: $"Created invoice {model.InvoiceNumber} for amount ₦{model.Amount:N0} ({model.BillingType})",
                    userName: User.Identity?.Name,
                    area: "Billing",
                    tenantSchoolId: model.TenantSchoolId,
                    httpContext: HttpContext);

                // Send Email Notification if requested
                if (sendEmailImmediately && !string.IsNullOrWhiteSpace(recipientEmail))
                {
                    await DispatchInvoiceEmailAsync(model.Id, recipientEmail.Trim(), "Invoice generated upon registration.");
                }

                TempData["SuccessMessage"] = $"Invoice '{model.InvoiceNumber}' created successfully." + (sendEmailImmediately && !string.IsNullOrWhiteSpace(recipientEmail) ? " Email notification sent to " + recipientEmail : "");
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Schools = new SelectList(await _context.TenantSchools.OrderBy(s => s.SchoolName).ToListAsync(), "Id", "SchoolName", model.TenantSchoolId);
            return View(model);
        }

        // GET: /Billing/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            var transaction = await _context.SchoolTransactions
                .Include(t => t.TenantSchool)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (transaction == null) return NotFound();

            ViewBag.Schools = new SelectList(await _context.TenantSchools.OrderBy(s => s.SchoolName).ToListAsync(), "Id", "SchoolName", transaction.TenantSchoolId);
            return View(transaction);
        }

        // POST: /Billing/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MasterSchoolTransaction model)
        {
            if (ModelState.IsValid)
            {
                var existing = await _context.SchoolTransactions.FindAsync(model.Id);
                if (existing == null) return NotFound();

                existing.TenantSchoolId = model.TenantSchoolId;
                existing.InvoiceNumber = model.InvoiceNumber;
                existing.Title = model.Title;
                existing.BillingType = model.BillingType;
                existing.Amount = model.Amount;
                existing.AmountPaid = model.AmountPaid;
                existing.Status = model.Status;
                existing.PaymentChannel = model.PaymentChannel;
                existing.BankReference = model.BankReference;
                existing.DueDate = model.DueDate;
                existing.PaidDate = model.PaidDate;
                existing.ProofDocumentUrl = model.ProofDocumentUrl;
                existing.ReceiptNumber = model.ReceiptNumber;
                existing.Description = model.Description;
                existing.UpdatedAt = DateTime.UtcNow;

                if (existing.Status == SchoolPaymentStatus.Paid)
                {
                    if (existing.AmountPaid == 0) existing.AmountPaid = existing.Amount;
                    if (!existing.PaidDate.HasValue) existing.PaidDate = DateTime.UtcNow;
                    if (string.IsNullOrWhiteSpace(existing.ReceiptNumber))
                    {
                        existing.ReceiptNumber = $"REC-{DateTime.UtcNow.Year}-{existing.Id:D5}";
                    }
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Transaction '{existing.InvoiceNumber}' updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Schools = new SelectList(await _context.TenantSchools.OrderBy(s => s.SchoolName).ToListAsync(), "Id", "SchoolName", model.TenantSchoolId);
            return View(model);
        }

        // POST: /Billing/MarkAsPaid/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsPaid(long id, SchoolPaymentChannel channel, string? reference, bool sendReceiptEmail = false, string? recipientEmail = null)
        {
            var item = await _context.SchoolTransactions.FindAsync(id);
            if (item == null) return NotFound();

            item.Status = SchoolPaymentStatus.Paid;
            item.AmountPaid = item.Amount;
            item.PaidDate = DateTime.UtcNow;
            item.PaymentChannel = channel;
            if (!string.IsNullOrWhiteSpace(reference)) item.BankReference = reference;
            if (string.IsNullOrWhiteSpace(item.ReceiptNumber))
            {
                item.ReceiptNumber = $"REC-{DateTime.UtcNow.Year}-{item.Id:D5}";
            }
            item.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Audit Log
            await _auditService.LogAsync(
                action: "PaymentConfirmed",
                details: $"Marked invoice {item.InvoiceNumber} as fully PAID. Amount: ₦{item.AmountPaid:N0}. Receipt: {item.ReceiptNumber}",
                userName: User.Identity?.Name,
                area: "Billing",
                tenantSchoolId: item.TenantSchoolId,
                httpContext: HttpContext);

            if (sendReceiptEmail && !string.IsNullOrWhiteSpace(recipientEmail))
            {
                await DispatchReceiptEmailAsync(item.Id, recipientEmail.Trim(), "Official payment confirmation receipt.");
            }

            TempData["SuccessMessage"] = $"Invoice '{item.InvoiceNumber}' marked as fully PAID. Receipt '{item.ReceiptNumber}' issued." + (sendReceiptEmail && !string.IsNullOrWhiteSpace(recipientEmail) ? " Receipt emailed to " + recipientEmail : "");
            return RedirectToAction(nameof(Index));
        }

        // GET: /Billing/Receipt/5
        public async Task<IActionResult> Receipt(long id)
        {
            var transaction = await _context.SchoolTransactions
                .Include(t => t.TenantSchool)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (transaction == null) return NotFound();

            return View(transaction);
        }

        // GET: /Billing/Invoice/5
        public async Task<IActionResult> Invoice(long id)
        {
            var transaction = await _context.SchoolTransactions
                .Include(t => t.TenantSchool)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (transaction == null) return NotFound();

            return View("Receipt", transaction);
        }

        // POST: /Billing/SendInvoiceEmail
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendInvoiceEmail(long id, string recipientEmail, string? customNote)
        {
            if (string.IsNullOrWhiteSpace(recipientEmail))
            {
                TempData["ErrorMessage"] = "Please provide a valid recipient email address.";
                return RedirectToAction(nameof(Index));
            }

            var success = await DispatchInvoiceEmailAsync(id, recipientEmail.Trim(), customNote);
            if (success)
            {
                TempData["SuccessMessage"] = $"Invoice was dispatched successfully to {recipientEmail} and logged in the Email Outbox.";
            }
            else
            {
                TempData["ErrorMessage"] = "Could not find invoice to send.";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /Billing/SendReceiptEmail
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendReceiptEmail(long id, string recipientEmail, string? customNote)
        {
            if (string.IsNullOrWhiteSpace(recipientEmail))
            {
                TempData["ErrorMessage"] = "Please provide a valid recipient email address.";
                return RedirectToAction(nameof(Index));
            }

            var success = await DispatchReceiptEmailAsync(id, recipientEmail.Trim(), customNote);
            if (success)
            {
                TempData["SuccessMessage"] = $"Payment Receipt was dispatched successfully to {recipientEmail} and logged in the Email Outbox.";
            }
            else
            {
                TempData["ErrorMessage"] = "Could not find receipt to send.";
            }

            return RedirectToAction(nameof(Index));
        }

        // ================= PRIVATE EMAIL BUILDERS & DISPATCHERS =================
        private async Task<bool> DispatchInvoiceEmailAsync(long transactionId, string recipientEmail, string? customNote)
        {
            var tx = await _context.SchoolTransactions
                .Include(t => t.TenantSchool)
                .FirstOrDefaultAsync(t => t.Id == transactionId);

            if (tx == null) return false;

            var schoolName = tx.TenantSchool?.SchoolName ?? "School Management";
            var subDomain = tx.TenantSchool?.SubDomain ?? "portal";
            var subject = $"Invoice #{tx.InvoiceNumber}: {tx.Title} - Exwhyzee SP";

            var htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'/>
    <style>
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f8fafc; margin: 0; padding: 24px; color: #1e293b; }}
        .invoice-box {{ max-width: 650px; margin: auto; background: #ffffff; border-radius: 12px; border: 1px solid #e2e8f0; overflow: hidden; box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.05); }}
        .header {{ background: #059669; color: #ffffff; padding: 28px 32px; }}
        .header h1 {{ margin: 0 0 6px 0; font-size: 22px; font-weight: 700; }}
        .header p {{ margin: 0; font-size: 13px; opacity: 0.9; }}
        .body-content {{ padding: 32px; }}
        .info-grid {{ width: 100%; margin-bottom: 24px; border-collapse: collapse; }}
        .info-grid td {{ padding: 6px 0; font-size: 13px; vertical-align: top; }}
        .table-items {{ width: 100%; border-collapse: collapse; margin: 20px 0; }}
        .table-items th {{ background: #f1f5f9; padding: 10px 14px; text-align: left; font-size: 12px; font-weight: 600; text-transform: uppercase; color: #475569; border-bottom: 2px solid #e2e8f0; }}
        .table-items td {{ padding: 12px 14px; border-bottom: 1px solid #e2e8f0; font-size: 13.5px; }}
        .total-row td {{ font-weight: 700; font-size: 15px; background: #f8fafc; }}
        .bank-details {{ background: #f0fdf4; border: 1px solid #bbf7d0; border-radius: 8px; padding: 16px; margin: 24px 0; }}
        .bank-details h4 {{ margin: 0 0 8px 0; font-size: 13.5px; color: #166534; }}
        .bank-details p {{ margin: 3px 0; font-size: 12.5px; color: #15803d; }}
        .footer {{ text-align: center; padding: 20px; font-size: 11.5px; color: #94a3b8; background: #f8fafc; border-top: 1px solid #e2e8f0; }}
    </style>
</head>
<body>
    <div class='invoice-box'>
        <div class='header'>
            <h1>Exwhyzee SP Enterprise Billing</h1>
            <p>Official Billing Notification &amp; Service Invoice</p>
        </div>
        <div class='body-content'>
            <p style='font-size: 14px; margin-top: 0;'>Dear Management of <strong>{schoolName}</strong>,</p>
            <p style='font-size: 13px; color: #475569; line-height: 1.5;'>
                This is an official invoice notification regarding your school portal services on Exwhyzee SP. Please find the transaction invoice breakdown below:
            </p>

            <table class='info-grid'>
                <tr>
                    <td style='width: 50%;'><strong>Invoice Number:</strong> {tx.InvoiceNumber}</td>
                    <td style='text-align: right;'><strong>Issue Date:</strong> {tx.CreatedAt:MMMM dd, yyyy}</td>
                </tr>
                <tr>
                    <td><strong>Billing Type:</strong> {tx.BillingType}</td>
                    <td style='text-align: right;'><strong>Due Date:</strong> <span style='color: #dc2626; font-weight: 600;'>{tx.DueDate:MMMM dd, yyyy}</span></td>
                </tr>
            </table>

            <table class='table-items'>
                <thead>
                    <tr>
                        <th>Description</th>
                        <th style='text-align: right;'>Amount Due</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>
                            <strong>{tx.Title}</strong>
                            {(string.IsNullOrWhiteSpace(tx.Description) ? "" : $"<br/><small style='color: #64748b;'>{tx.Description}</small>")}
                        </td>
                        <td style='text-align: right; font-weight: 600;'>₦{tx.Amount:N2}</td>
                    </tr>
                    <tr class='total-row'>
                        <td>Total Balance Due:</td>
                        <td style='text-align: right; color: #059669;'>₦{tx.BalanceDue:N2}</td>
                    </tr>
                </tbody>
            </table>

            {(string.IsNullOrWhiteSpace(customNote) ? "" : $@"
            <div style='background: #f8fafc; border-left: 4px solid #059669; padding: 12px 16px; margin: 16px 0; font-size: 12.5px;'>
                <strong>Special Note from Accounts:</strong> {customNote}
            </div>")}

            <div class='bank-details'>
                <h4>Bank Payment Instructions:</h4>
                <p><strong>Bank:</strong> Zenith Bank PLC</p>
                <p><strong>Account Name:</strong> Exwhyzee Technologies Ltd</p>
                <p><strong>Account Number:</strong> 1014589230</p>
                <p><strong>Payment Reference:</strong> {tx.InvoiceNumber}</p>
            </div>

            <p style='font-size: 12.5px; color: #64748b;'>
                After payment, please send proof of transfer or reference code to accounts@exwhyzee.ng for automated confirmation.
            </p>
        </div>
        <div class='footer'>
            &copy; {DateTime.UtcNow.Year} Exwhyzee SP. All rights reserved. &bull; support@exwhyzee.ng
        </div>
    </div>
</body>
</html>";

            await _emailLoggingService.LogAndDumpEmailAsync(
                recipientEmail,
                subject,
                htmlBody,
                schoolName,
                $"Invoice #{tx.InvoiceNumber} for ₦{tx.Amount:N0} sent to {schoolName}",
                "Invoice",
                User.Identity?.Name ?? "BillingModule",
                subDomain);

            return true;
        }

        private async Task<bool> DispatchReceiptEmailAsync(long transactionId, string recipientEmail, string? customNote)
        {
            var tx = await _context.SchoolTransactions
                .Include(t => t.TenantSchool)
                .FirstOrDefaultAsync(t => t.Id == transactionId);

            if (tx == null) return false;

            var schoolName = tx.TenantSchool?.SchoolName ?? "School Management";
            var subDomain = tx.TenantSchool?.SubDomain ?? "portal";
            var subject = $"Payment Receipt #{tx.ReceiptNumber ?? tx.InvoiceNumber}: Payment Confirmed - Exwhyzee SP";

            var htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'/>
    <style>
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f8fafc; margin: 0; padding: 24px; color: #1e293b; }}
        .receipt-box {{ max-width: 650px; margin: auto; background: #ffffff; border-radius: 12px; border: 1px solid #e2e8f0; overflow: hidden; box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.05); }}
        .header {{ background: #059669; color: #ffffff; padding: 28px 32px; }}
        .header h1 {{ margin: 0 0 6px 0; font-size: 22px; font-weight: 700; }}
        .body-content {{ padding: 32px; }}
        .badge-paid {{ display: inline-block; background: #dcfce7; color: #15803d; font-weight: 700; padding: 6px 14px; border-radius: 9999px; font-size: 12px; text-transform: uppercase; margin-bottom: 16px; }}
        .info-grid {{ width: 100%; margin-bottom: 24px; border-collapse: collapse; }}
        .info-grid td {{ padding: 6px 0; font-size: 13px; vertical-align: top; }}
        .table-items {{ width: 100%; border-collapse: collapse; margin: 20px 0; }}
        .table-items th {{ background: #f1f5f9; padding: 10px 14px; text-align: left; font-size: 12px; font-weight: 600; text-transform: uppercase; color: #475569; border-bottom: 2px solid #e2e8f0; }}
        .table-items td {{ padding: 12px 14px; border-bottom: 1px solid #e2e8f0; font-size: 13.5px; }}
        .footer {{ text-align: center; padding: 20px; font-size: 11.5px; color: #94a3b8; background: #f8fafc; border-top: 1px solid #e2e8f0; }}
    </style>
</head>
<body>
    <div class='receipt-box'>
        <div class='header'>
            <h1>Official Payment Receipt</h1>
            <p>Exwhyzee SP School Management Platform</p>
        </div>
        <div class='body-content'>
            <div class='badge-paid'>✓ Payment Confirmed &amp; Verified</div>
            <p style='font-size: 14px; margin-top: 0;'>Dear Management of <strong>{schoolName}</strong>,</p>
            <p style='font-size: 13px; color: #475569; line-height: 1.5;'>
                Thank you for your payment. We have successfully processed and credited your portal account. Please find your official receipt details below:
            </p>

            <table class='info-grid'>
                <tr>
                    <td style='width: 50%;'><strong>Receipt Number:</strong> {tx.ReceiptNumber}</td>
                    <td style='text-align: right;'><strong>Invoice Number:</strong> {tx.InvoiceNumber}</td>
                </tr>
                <tr>
                    <td><strong>Payment Date:</strong> {(tx.PaidDate.HasValue ? tx.PaidDate.Value.ToString("MMMM dd, yyyy") : DateTime.UtcNow.ToString("MMMM dd, yyyy"))}</td>
                    <td style='text-align: right;'><strong>Payment Method:</strong> {tx.PaymentChannel}</td>
                </tr>
            </table>

            <table class='table-items'>
                <thead>
                    <tr>
                        <th>Item Description</th>
                        <th style='text-align: right;'>Amount Paid</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td><strong>{tx.Title}</strong></td>
                        <td style='text-align: right; font-weight: 700; color: #059669;'>₦{tx.AmountPaid:N2}</td>
                    </tr>
                    <tr style='background: #f8fafc; font-weight: 700;'>
                        <td>Outstanding Balance:</td>
                        <td style='text-align: right;'>₦0.00</td>
                    </tr>
                </tbody>
            </table>

            {(string.IsNullOrWhiteSpace(customNote) ? "" : $@"
            <div style='background: #f8fafc; border-left: 4px solid #059669; padding: 12px 16px; margin: 16px 0; font-size: 12.5px;'>
                <strong>Note:</strong> {customNote}
            </div>")}

            <p style='font-size: 13px; color: #166534; font-weight: 600;'>
                Your school portal status remains fully Active and in good standing.
            </p>
        </div>
        <div class='footer'>
            &copy; {DateTime.UtcNow.Year} Exwhyzee SP. All rights reserved. &bull; support@exwhyzee.ng
        </div>
    </div>
</body>
</html>";

            await _emailLoggingService.LogAndDumpEmailAsync(
                recipientEmail,
                subject,
                htmlBody,
                schoolName,
                $"Receipt #{tx.ReceiptNumber} for ₦{tx.AmountPaid:N0} issued to {schoolName}",
                "Receipt",
                User.Identity?.Name ?? "BillingModule",
                subDomain);

            return true;
        }
    }
}
