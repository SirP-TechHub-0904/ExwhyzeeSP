using ExwhyzeeSP.Web.Areas.Data.IServices;
using ExwhyzeeSP.Web.Areas.Data.Services;
using ExwhyzeeSP.Core.Models;
using ExwhyzeeSP.Data.Contexts;
using ExwhyzeeSP.Core.Dtos;
using ExwhyzeeSP.Core.Entities;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
// // using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using X.PagedList.Extensions;
using X.PagedList;
using System.IO;
using Microsoft.AspNetCore.Identity;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace ExwhyzeeSP.Web.Areas.SuperUser.Controllers
{

    [Authorize(Roles = "SuperAdmin")]
    [Area("SuperUser")]
    public class ImportFromExcelController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private IStaffService _staffProfileService = new StaffService();
        private IClassLevelService _classLevelService = new ClassLevelService();
        private IResultService _resultService = new ResultService();
        private IEnrollmentService _enrollmentService = new EnrollmentService();
        private IStudentProfileService _studentProfileService = new StudentProfileService();
        private ISubjectService _subjectService = new SubjectService();
        private ISessionService _sessionService = new SessionService();
        private IEnrolledSubjectService _enrolledSubjectService = new EnrolledSubjectService();
        private ISettingService _settingService = new SettingService();
        private IPublishResultService _publishResultService = new PublishResultService();
        private IImageService _imageService = new ImageService();
        private IPostService _postService = new PostService();
        private IAssignmentService _assignmentService = new AssignmentService();
        private IPinService _pinService = new PinService();

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            set
            {
                _userManager = value;
            }
        }

        [Microsoft.Extensions.DependencyInjection.ActivatorUtilitiesConstructor]
        public ImportFromExcelController()
        {

        }
        public ImportFromExcelController(StaffService staffProfileService,
            ClassLevelService classLevelService,
            ResultService resultService,
            EnrollmentService enrollmentService,
            StudentProfileService studentProfileService,
            SubjectService subjectService,
            SessionService sessionService,
            EnrolledSubjectService enrolledSubjectService,
            SettingService settingService,
            PublishResultService publishResultService,
            ImageService imageService,
            PostService postService,
            PinService pinService,
            AssignmentService assignmentService, ApplicationUserManager userManager
            )
        {
            _imageService = imageService;
            _postService = postService;
            _staffProfileService = staffProfileService;
            _classLevelService = classLevelService;
            _resultService = resultService;
            _enrollmentService = enrollmentService;
            _studentProfileService = studentProfileService;
            _subjectService = subjectService;
            _sessionService = sessionService;
            _enrolledSubjectService = enrolledSubjectService;
            _settingService = settingService;
            _publishResultService = publishResultService;
            _assignmentService = assignmentService;
            UserManager = userManager;
            _pinService = pinService;
        }
        public ActionResult Index()
        {

            return View("Import");
        }

        //
        public async Task<ActionResult> UploadCard(string searchString, string currentFilter, int? page)
        {
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            var items = await _pinService.List(searchString, currentFilter, page);
            ViewBag.countAll = items.Count();
            int pageSize = 100;
            int pageNumber = (page ?? 1);
            ViewBag.Total = await _pinService.TotalPin();
            return View(items.ToPagedList(pageNumber, pageSize));

        }

        [HttpPost]
        public async Task<ActionResult> UploadCard(string batchnumber, IFormFile excelfile)
        {

            string path = "";
            if (excelfile == null || excelfile.Length == 0)
            {
                TempData["error"] = "Please select a excel file";
                return RedirectToAction("Offline");

            }
            else
            {
                if (excelfile.FileName.EndsWith("xls") || excelfile.FileName.EndsWith("xlsx"))
                {
                    System.Random randomInteger = new System.Random();
                    int genNumber = randomInteger.Next(100000);
                    path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot") + ("~/ExcelUpload/" + genNumber + excelfile.FileName);
                    //if (System.IO.File.Exists(path))
                    //    System.IO.File.Delete(path);


                    excelfile.SaveAs(path);

                    using (SpreadsheetDocument spreadSheetDocument = SpreadsheetDocument.Open(path, false))
                    {
                        WorkbookPart workbookPart = spreadSheetDocument.WorkbookPart;
                        WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
                        SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();

                        int rowCount = sheetData.Elements<Row>().Skip(2).Count();


                        foreach (Row r in sheetData.Descendants<Row>().Skip(2))
                        {

                            try
                            {
                                var pin = r.ChildElements[1].InnerText;
                                var serial = r.ChildElements[2].InnerText;



                                string pinNumber = pin;
                                string serialNumber = serial;
                                string batchnumber1 = batchnumber;

                                PinCodeModel pinmodel = new PinCodeModel();
                                pinmodel.BatchNumber = batchnumber1;
                                pinmodel.DateCreated = DateTime.UtcNow;
                                pinmodel.PinNumber = pinNumber;
                                pinmodel.SerialNumber = serialNumber;
                                pinmodel.Usage = 2;
                                db.PinCodeModels.Add(pinmodel);
                                await db.SaveChangesAsync();



                            }
                            catch (Exception e)
                            {

                            }




                        }
                    }
                    TempData["msg"] = "Upload successfull Scroll down to review.";

                }
                else
                {
                    TempData["error"] = "file type incorrect";
                }

                return RedirectToAction("UploadCard");

            }
        }


        public async Task<ActionResult> DeleteCard()
        {
            return View();

        }

        [HttpPost]
        public async Task<ActionResult> DeleteCard(string batchnumber, string confirmCode = "")
        {
            if (string.IsNullOrWhiteSpace(confirmCode) || !confirmCode.Trim().Equals("CONFIRM", StringComparison.OrdinalIgnoreCase))
            {
                TempData["error"] = "Card batch deletion cancelled: You must type 'CONFIRM' to approve deleting this scratch card batch.";
                return View();
            }

            var pins = await db.PinCodeModels.Where(x => x.BatchNumber == batchnumber).ToListAsync();
            if (batchnumber != null && pins.Any())
            {
                int count = pins.Count;
                foreach (var pin in pins)
                {
                    try
                    {
                        db.PinCodeModels.Remove(pin);
                    }
                    catch (Exception)
                    {
                    }
                }
                await db.SaveChangesAsync();
                TempData["msg"] = $"Batch '{batchnumber}' containing {count} scratch card PIN(s) was deleted successfully.";
            }
            else
            {
                TempData["error"] = "Unable to delete card: No PINs found matching the specified batch number.";
            }

            return RedirectToAction("UploadCard");
        }

        public static string GetCellValue(SpreadsheetDocument document, Cell cell)
        {
            SharedStringTablePart stringTablePart = document.WorkbookPart.SharedStringTablePart;
            string value = cell.CellValue.InnerXml;

            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                return stringTablePart.SharedStringTable.ChildElements[Int32.Parse(value)].InnerText;
            }
            else
            {
                return value;
            }
        }
        //choose class by subject class and session

    }
}

