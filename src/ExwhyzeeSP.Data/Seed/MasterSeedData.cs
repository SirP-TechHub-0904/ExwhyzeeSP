using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ExwhyzeeSP.Core.Models;
using ExwhyzeeSP.Data.Contexts;

namespace ExwhyzeeSP.Data.Seed
{
    /// <summary>
    /// Seeds only system essentials: Database schema migrations, RBAC roles, default SuperAdmin user, and legal CMS pages.
    /// Does NOT seed any dummy schools, test invoices, fake telemetry, or mock CRM contacts.
    /// </summary>
    public static class MasterSeedData
    {
        // Master toggle - Seeding is permanently disabled since the database is already seeded/customized
        public static readonly bool IsSeedingEnabled = false;

        public static async Task InitializeAsync(MasterDbContext context, IServiceProvider? serviceProvider = null)
        {
            if (!IsSeedingEnabled)
            {
                return;
            }

            // 1. Ensure Master database tables and schema columns exist
            await context.Database.EnsureCreatedAsync();

            try
            {
                await context.Database.ExecuteSqlRawAsync(@"
                    IF COL_LENGTH('dbo.TenantSchools', 'IsDeleted') IS NULL
                    BEGIN
                        ALTER TABLE dbo.TenantSchools ADD [IsDeleted] bit NOT NULL CONSTRAINT DF_TenantSchools_IsDeleted DEFAULT(0);
                    END
                    IF COL_LENGTH('dbo.TenantSchools', 'DeletedAt') IS NULL
                    BEGIN
                        ALTER TABLE dbo.TenantSchools ADD [DeletedAt] datetime2 NULL;
                    END
                    IF COL_LENGTH('dbo.TenantSchools', 'DeletedBy') IS NULL
                    BEGIN
                        ALTER TABLE dbo.TenantSchools ADD [DeletedBy] nvarchar(200) NULL;
                    END
                    IF COL_LENGTH('dbo.TenantSchools', 'DeletionReason') IS NULL
                    BEGIN
                        ALTER TABLE dbo.TenantSchools ADD [DeletionReason] nvarchar(500) NULL;
                    END
                    IF COL_LENGTH('dbo.TenantSchools', 'LastBackupDate') IS NULL
                    BEGIN
                        ALTER TABLE dbo.TenantSchools ADD [LastBackupDate] datetime2 NULL;
                    END
                    IF COL_LENGTH('dbo.TenantSchools', 'LastBackupUrl') IS NULL
                    BEGIN
                        ALTER TABLE dbo.TenantSchools ADD [LastBackupUrl] nvarchar(500) NULL;
                    END
                    IF COL_LENGTH('dbo.TenantSchools', 'LastBackupSize') IS NULL
                    BEGIN
                        ALTER TABLE dbo.TenantSchools ADD [LastBackupSize] nvarchar(50) NULL;
                    END

                    IF OBJECT_ID('dbo.TenantBackupHistories', 'U') IS NULL
                    BEGIN
                        CREATE TABLE dbo.TenantBackupHistories (
                            [Id] bigint IDENTITY(1,1) NOT NULL PRIMARY KEY,
                            [TenantSchoolId] int NOT NULL,
                            [SchoolName] nvarchar(200) NOT NULL,
                            [Subdomain] nvarchar(100) NULL,
                            [BackupDate] datetime2 NOT NULL CONSTRAINT DF_TenantBackupHistories_BackupDate DEFAULT(SYSUTCDATETIME()),
                            [FileUrl] nvarchar(1000) NULL,
                            [FileName] nvarchar(255) NULL,
                            [FileSize] nvarchar(50) NULL,
                            [BackupType] nvarchar(100) NOT NULL,
                            [InitiatedBy] nvarchar(200) NULL,
                            [Status] nvarchar(50) NOT NULL CONSTRAINT DF_TenantBackupHistories_Status DEFAULT('Success'),
                            [Notes] nvarchar(500) NULL
                        );
                    END
                ");
            }
            catch
            {
                // Suppressed if table not yet created or permissions restricted
            }

            // 2. Seed RBAC Identity Roles and Default SuperAdmin Account
            if (serviceProvider != null)
            {
                await SeedIdentityRolesAndAdminAsync(context, serviceProvider);
            }

            // 3. Seed Essential Legal CMS Pages (Privacy Policy, Terms of Service, Cookie Policy) only if empty
            if (!await context.MasterCmsPages.AnyAsync())
            {
                var defaultPages = new List<MasterCmsPage>
                {
                    new MasterCmsPage
                    {
                        Slug = "privacy-policy",
                        Title = "Privacy Policy",
                        Summary = "Comprehensive data protection, student record privacy, multi-tenant database isolation, and NDPR/GDPR compliance guidelines for Exwhyzee SP.",
                        Content = @"
<h3 id=""introduction"">1. Introduction &amp; Commitment</h3>
<p>Exwhyzee Technologies (&quot;Exwhyzee SP&quot;, &quot;we&quot;, &quot;our&quot;, or &quot;us&quot;) operates a unified, multi-tenant cloud School Management and Academic Enterprise Resource Planning (ERP) platform. We are committed to protecting the privacy, integrity, and confidentiality of student records, educational assessment data, parent communications, and institutional financial transactions.</p>
<p>This Privacy Policy explains how personal data is collected, stored, encrypted, processed, and safeguarded when schools, administrators, teachers, parents, guardians, and students access and utilize our services.</p>

<h3 id=""information-we-collect"">2. Categories of Information We Collect</h3>
<p>To provide school management services, we process several categories of data entrusted to us by educational institutions:</p>
<ul>
    <li><strong>Student Profile &amp; Academic Data:</strong> Full legal names, admission/matriculation numbers, gender, date of birth, passport photographs, continuous assessment (CA) scores, examination marks, grade positions, psychomotor ratings, and terminal broadsheets.</li>
    <li><strong>Parent &amp; Guardian Information:</strong> Full names, primary phone numbers (for SMS broadcasts and notifications), email addresses, residential locations, and fee payment verification transaction records.</li>
    <li><strong>Staff &amp; Educator Credentials:</strong> Official email addresses, telephone numbers, role-based access rights, lesson plan submissions, attendance records, and administrative audit trails.</li>
    <li><strong>System &amp; Usage Telemetry:</strong> Log timestamps, IP addresses, browser types, session duration, and device identifiers collected to prevent brute-force attacks and ensure account safety.</li>
</ul>

<h3 id=""how-we-use-data"">3. Purpose and Legal Basis for Processing</h3>
<p>All data processed on Exwhyzee SP is governed by contractual necessity with registered educational institutions and used solely for lawful academic and administrative operations, including:</p>
<ul>
    <li>Processing terminal report cards, cumulative transcripts, and automated cognitive evaluations.</li>
    <li>Conducting Computer-Based Testing (CBT) examinations, online quizzes, and entrance assessment grading.</li>
    <li>Invoicing school fees, issuing digital receipts, generating bank tellers, and reconciling payments.</li>
    <li>Delivering automated SMS alerts, exam timetable broadcasts, fee reminder alerts, and emergency notifications.</li>
    <li>Facilitating prospective student admissions, entrance screening forms, and registration verification.</li>
</ul>

<h3 id=""data-isolation-security"">4. Multi-Tenant Data Isolation &amp; Security Architecture</h3>
<p>Security is embedded at every layer of our platform architecture:</p>
<ul>
    <li><strong>Database Isolation:</strong> Each institution operates with dedicated database connection strings or isolated schemas, preventing cross-tenant data leakage or unauthorized access.</li>
    <li><strong>Encryption in Transit &amp; at Rest:</strong> All web traffic is encrypted using 256-bit TLS/SSL encryption. Database backups and sensitive credentials are encrypted using industry-standard hashing algorithms.</li>
    <li><strong>Role-Based Access Control (RBAC):</strong> Strict permission matrices ensure teachers only access their assigned classes, students only view their own performance, and accountants only access financial ledgers.</li>
    <li><strong>Automated Disaster Recovery:</strong> Redundant cloud snapshots and point-in-time recovery mechanisms ensure business continuity and guard against catastrophic data loss.</li>
</ul>

<h3 id=""financial-data"">5. Financial &amp; Payment Data Security</h3>
<p>Exwhyzee SP does not store raw credit/debit card numbers or bank account PINs on our servers. All online fee transactions are processed through certified, PCI-DSS compliant payment gateways. We only retain transaction reference IDs, settlement timestamps, and bank payment confirmation logs for accounting and audit reconciliation.</p>

<h3 id=""data-retention"">6. Data Retention &amp; Institutional Ownership</h3>
<p>Educational institutions retain complete, exclusive ownership of their student and staff data. Exwhyzee SP acts as a Data Processor. We retain academic archives for the duration of the school's active subscription and in accordance with institutional regulatory standards. School administrators may request full database exports or account archiving at the conclusion of any academic session.</p>

<h3 id=""rights-of-users"">7. Data Subject Rights &amp; Regulatory Compliance</h3>
<p>We comply with the Nigeria Data Protection Regulation (NDPR) and international privacy frameworks (GDPR). Data subjects (students, parents, and teachers) have the right to request access, rectification, or updates to their personal information through their school administrator or our Data Protection Officer.</p>

<h3 id=""cookies-analytics"">8. Cookies and Storage Technologies</h3>
<p>We use essential cookies and browser storage tokens to authenticate users, protect forms from Cross-Site Request Forgery (CSRF), and maintain session state. Please review our <a href=""/Cookies"">Cookie Policy</a> for complete details.</p>

<h3 id=""contact-privacy"">9. Contact Our Data Protection Officer</h3>
<p>If you have questions, feedback, or compliance requests regarding this Privacy Policy, please contact our Data Protection Team:</p>
<p><strong>Exwhyzee Technologies Ltd</strong><br />
Email: <a href=""mailto:info@exwhyzee.ng"">info@exwhyzee.ng</a> | <a href=""mailto:privacy@exwhyzee.ng"">privacy@exwhyzee.ng</a><br />
Website: <a href=""https://exwhyzee.ng"" target=""_blank"">www.exwhyzee.ng</a><br />
Support Line: +234 800 EXWHYZEE (24/7 Academic Support)</p>
",
                        LastUpdated = DateTime.UtcNow,
                        IsPublished = true
                    },
                    new MasterCmsPage
                    {
                        Slug = "terms-and-conditions",
                        Title = "Terms of Service",
                        Summary = "Rules of engagement, institutional licensing, subscription terms, SLA commitments, and acceptable use policy for Exwhyzee SP.",
                        Content = @"
<h3 id=""agreement-to-terms"">1. Acceptance of Terms</h3>
<p>These Terms of Service (&quot;Agreement&quot;) constitute a legally binding contract between Exwhyzee Technologies Ltd (&quot;Exwhyzee&quot;, &quot;we&quot;, &quot;us&quot;) and the subscribing educational institution, school administrator, educator, parent, or student (&quot;Customer&quot;, &quot;User&quot;, &quot;You&quot;) accessing the Exwhyzee School Portal (Exwhyzee SP) software platform.</p>
<p>By registering a school tenant, logging into the portal, or purchasing a subscription, you confirm that you have read, understood, and agreed to be bound by these Terms.</p>

<h3 id=""provisioning-tenancy"">2. Tenant Provisioning &amp; Authorized Access</h3>
<p>Upon registration and verification, Exwhyzee provisions a dedicated sub-domain (e.g., <code>yourschool.exwhyzee.ng</code>) or custom domain mapping linked to a secure database instance. The subscribing institution is responsible for maintaining the confidentiality of administrative master passwords, staff PINs, and student access scratch cards.</p>

<h3 id=""sla-uptime"">3. Service Level Agreement (SLA) &amp; System Availability</h3>
<p>We commit to a <strong>99.9% application uptime</strong> throughout the active academic session, excluding scheduled maintenance windows. Scheduled maintenance is typically conducted during off-peak weekend hours with advance email and in-app dashboard notification.</p>

<h3 id=""billing-subscriptions"">4. Pricing, Subscriptions &amp; Billing Terms</h3>
<p>Exwhyzee SP offers flexible educational billing models designed to accommodate private and public institutions of all sizes:</p>
<ul>
    <li><strong>Per-Student Per-Term Model:</strong> Invoiced based on the number of active enrolled students at each term's census date.</li>
    <li><strong>Per-Classroom Flat Model:</strong> Predictable tier-based pricing per active academic classroom or branch.</li>
    <li><strong>Annual Institutional License:</strong> Discounted flat annual subscription covering unlimited students, CBT, and admissions.</li>
    <li><strong>Digital Result Scratch Cards:</strong> Pay-as-you-go PIN verification cards for end-of-term student result access and online verification.</li>
</ul>
<p>All subscription payments are due within the invoice grace period. Services may be placed in read-only mode if invoices remain delinquent past the agreed term deadline.</p>

<h3 id=""acceptable-use"">5. Acceptable Use Policy &amp; Platform Integrity</h3>
<p>Users agree not to:</p>
<ul>
    <li>Attempt unauthorized penetration testing, vulnerability scanning, or SQL injection attacks against the infrastructure.</li>
    <li>Upload malicious scripts, viruses, or unauthorized exam materials.</li>
    <li>Use portal SMS broadcast channels to send unsolicited spam, political advertising, or fraudulent notices.</li>
    <li>Share administrative master credentials outside of authorized institutional staff.</li>
</ul>

<h3 id=""intellectual-property"">6. Proprietary Software Rights</h3>
<p>All source code, software architecture, CBT algorithms, custom report card template engines, user interfaces, branding, and documentation are the exclusive intellectual property of Exwhyzee Technologies Ltd. Subscribing institutions are granted a non-exclusive, non-transferable license to access and use the software during the active subscription period.</p>

<h3 id=""data-ownership-backup"">7. Academic Data Ownership &amp; Backups</h3>
<p>The subscribing school maintains full legal ownership of all student bio-data, academic grades, examination questions, and financial records uploaded to their instance. Exwhyzee performs automated daily snapshots to cloud vaults. In the event of contract termination, schools may request complete data exports in SQL/CSV/Excel format.</p>

<h3 id=""limitation-liability"">8. Limitation of Liability</h3>
<p>To the maximum extent permitted by applicable law, Exwhyzee Technologies shall not be liable for indirect, punitive, or consequential damages resulting from internet service provider downtime, unauthorized user credential sharing, or delayed third-party SMS gateway delivery. Our aggregate liability is strictly limited to the subscription fees paid by the institution during the preceding term.</p>

<h3 id=""dispute-resolution"">9. Governing Law &amp; Dispute Resolution</h3>
<p>This Agreement is governed by the laws of the Federal Republic of Nigeria. Any disputes arising from or in connection with these Terms shall first be resolved through good-faith mutual consultation, and if unresolved, referred to binding arbitration in accordance with the Arbitration and Conciliation Act.</p>

<h3 id=""contact-legal"">10. Contact Information</h3>
<p>For questions regarding these Terms of Service or enterprise licensing agreements, please contact:</p>
<p><strong>Legal &amp; Contracts Department</strong><br />
Exwhyzee Technologies Ltd<br />
Email: <a href=""mailto:legal@exwhyzee.ng"">legal@exwhyzee.ng</a> | <a href=""mailto:info@exwhyzee.ng"">info@exwhyzee.ng</a><br />
Website: <a href=""https://exwhyzee.ng"" target=""_blank"">www.exwhyzee.ng</a></p>
",
                        LastUpdated = DateTime.UtcNow,
                        IsPublished = true
                    },
                    new MasterCmsPage
                    {
                        Slug = "cookie-policy",
                        Title = "Cookie Policy",
                        Summary = "How Exwhyzee SP uses cookies, local storage tokens, and browser cache to ensure security, high performance, and reliable session authentication.",
                        Content = @"
<h3 id=""what-are-cookies"">1. What Are Cookies and Local Storage?</h3>
<p>Cookies and browser storage mechanisms (such as <code>localStorage</code> and session tokens) are small data files stored on your computer or mobile device when you browse websites and use cloud applications. They allow our platform to remember your login session, prevent repeated logins, and safeguard your account against unauthorized requests.</p>

<h3 id=""cookies-we-use"">2. Categories of Cookies We Utilize</h3>
<p>Exwhyzee SP uses strictly necessary and functional cookies for portal operations:</p>
<ul>
    <li><strong>Essential Authentication Cookies:</strong> Cryptographically signed session identifiers that authenticate teachers, school administrators, students, and parents across pages without requiring re-login on every click.</li>
    <li><strong>Security &amp; Anti-Forgery Tokens (CSRF):</strong> Cryptographic tokens that verify form submissions (such as student score entry, CBT quiz submissions, and fee payments) originate genuinely from your browser, protecting against malicious cross-site attacks.</li>
    <li><strong>User Preference Tokens:</strong> Store interface preferences, such as selected academic sessions, active class filters, table pagination sizes, and your cookie consent status.</li>
    <li><strong>Performance &amp; Cache State:</strong> Ephemeral browser tokens used to cache report card assets, minimizing load times over mobile broadband networks.</li>
</ul>

<h3 id=""third-party-cookies"">3. Third-Party Cookies &amp; Integrations</h3>
<p>When processing tuition fees online, integrated payment gateways (such as Flutterwave, Paystack, or Interswitch) may set temporary security cookies to detect fraudulent transactions and verify two-factor authentication (2FA). Exwhyzee SP does not use intrusive third-party cross-site advertising tracking cookies.</p>

<h3 id=""managing-cookies"">4. How You Can Manage Cookies</h3>
<p>You can configure or adjust cookie settings directly within your web browser settings. Most modern browsers (Chrome, Edge, Firefox, Safari) allow you to block or delete cookies. However, please note that disabling essential authentication cookies will prevent you from logging into the portal, submitting CBT exams, or viewing academic report cards.</p>

<h3 id=""policy-updates"">5. Updates to This Cookie Policy</h3>
<p>We may update this policy periodically to reflect enhancements in technology, security standards, or regulatory guidelines. Updates take effect immediately upon publication on this page.</p>

<h3 id=""contact-cookies"">6. Questions &amp; Support</h3>
<p>For questions regarding our use of cookies and privacy safeguards, please reach out to our technical team at <a href=""mailto:support@exwhyzee.ng"">support@exwhyzee.ng</a> or <a href=""mailto:info@exwhyzee.ng"">info@exwhyzee.ng</a>.</p>
",
                        LastUpdated = DateTime.UtcNow,
                        IsPublished = true
                    }
                };

                await context.MasterCmsPages.AddRangeAsync(defaultPages);
                await context.SaveChangesAsync();
            }

            // 4. One-time clean-up of any orphaned demo/test invoices belonging to decommissioned/deleted schools
            try
            {
                var orphanedInvoices = await context.SchoolTransactions
                    .IgnoreQueryFilters()
                    .Where(t => t.TenantSchool == null || t.TenantSchool.IsDeleted)
                    .ToListAsync();

                if (orphanedInvoices.Any())
                {
                    context.SchoolTransactions.RemoveRange(orphanedInvoices);
                    await context.SaveChangesAsync();
                }
            }
            catch
            {
                // Suppressed if table empty
            }
        }

        private static async Task SeedIdentityRolesAndAdminAsync(MasterDbContext context, IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetService<UserManager<MasterAdminUser>>();

            if (roleManager != null && userManager != null)
            {
                // 1. Seed Roles
                foreach (var roleName in MasterRoles.All)
                {
                    if (!await roleManager.RoleExistsAsync(roleName))
                    {
                        await roleManager.CreateAsync(new IdentityRole(roleName));
                    }
                }

                // 2. Seed Default SuperAdmin from .env or secure defaults
                var adminEmail = Environment.GetEnvironmentVariable("MASTER_ADMIN_EMAIL");
                if (string.IsNullOrWhiteSpace(adminEmail))
                {
                    adminEmail = "superadmin@exwhyzee.ng";
                }

                var adminPassword = Environment.GetEnvironmentVariable("MASTER_ADMIN_DEFAULT_PASSWORD");
                if (string.IsNullOrWhiteSpace(adminPassword))
                {
                    adminPassword = "ExwhyzeeAdmin2026!";
                }

                var existingAdmin = await userManager.FindByEmailAsync(adminEmail) ?? await userManager.FindByNameAsync(adminEmail);
                if (existingAdmin == null)
                {
                    var superAdminUser = new MasterAdminUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        FullName = "Master Super Administrator",
                        RoleTitle = MasterRoles.SuperAdmin,
                        EmailConfirmed = true,
                        IsActive = true,
                        CreatedDate = DateTime.UtcNow
                    };

                    var result = await userManager.CreateAsync(superAdminUser, adminPassword);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(superAdminUser, MasterRoles.SuperAdmin);
                    }
                }
                else
                {
                    if (!await userManager.IsInRoleAsync(existingAdmin, MasterRoles.SuperAdmin))
                    {
                        await userManager.AddToRoleAsync(existingAdmin, MasterRoles.SuperAdmin);
                    }
                }
            }
        }
    }
}
