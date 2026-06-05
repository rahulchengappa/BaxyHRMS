using HRMS.Application.DTOs.Employee;
using HRMS.Application.Interfaces;
using HRMS.Application.Interfaces.Security;
using HRMS.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ClosedXML.Excel;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using iText.Layout.Properties;

namespace HRMS.Web.Controllers
{
    public class EmployeeController : BaseController
    {
        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentService _departmentService;
        private readonly IDesignationService _designationService;
        private readonly IRoleService _roleService;
        private readonly IIdProtector _idProtector;

        public EmployeeController(
            IEmployeeService employeeService,
            IDepartmentService departmentService,
            IDesignationService designationService,
            IRoleService roleService,
            IIdProtector idProtector)
        {
            _employeeService = employeeService;
            _departmentService = departmentService;
            _designationService = designationService;
            _roleService = roleService;
            _idProtector = idProtector;
        }

        // 🔧 NEW METHOD (dropdown fix)
        private void LoadDropdowns(EmployeeDto? emp = null)
        {
            ViewBag.Departments = new SelectList(
                _departmentService.GetAll(),
                "DepartmentId",
                "DepartmentName",
                emp?.DepartmentId
            );

            ViewBag.Designations = new SelectList(
                _designationService.GetAll(),
                "DesignationId",
                "DesignationName",
                emp?.DesignationId
            );

            ViewBag.Roles = new SelectList(
                _roleService.GetAll(),
                "RoleId",
                "RoleName",
                emp?.RoleId
            );

            ViewBag.Managers = _employeeService.GetAll()
                .Select(x => new SelectListItem
                {
                    Value = x.EmployeeID.ToString(),
                    Text = x.EmployeeName
                }).ToList();
        }

        // LIST 
        public IActionResult Index(string filterBy, string filterValue)
        {
            if (!IsUserLoggedIn())
                return RedirectToLogin();

            var auth = RequireMenuAccess("/Employee/Index");
            if (auth != null) return auth;

            var employees = string.IsNullOrEmpty(filterValue)
                ? _employeeService.GetAll()
                : _employeeService.Filter(filterBy, filterValue);

            return View(employees);
        }

        // DETAILS
        public IActionResult Details(string id)
        {
            if (!IsUserLoggedIn())
                return RedirectToLogin();

            var auth = RequireMenuAccess("/Employee/Details");
            if (auth != null) return auth;

            int employeeId;
            try
            {
                employeeId = int.Parse(_idProtector.Unprotect(id));
            }
            catch
            {
                return BadRequest("Invalid employee id");
            }

            var emp = _employeeService.GetById(employeeId);
            if (emp == null) return NotFound();

            emp.Documents = _employeeService.GetEmployeeDocuments(employeeId);

            return View(emp);
        }

        // CREATE (GET)
        [HttpGet]
        public IActionResult Create()
        {
            if (!IsUserLoggedIn())
                return RedirectToLogin();

            var auth = RequireMenuAccess("/Employee/Create");
            if (auth != null) return auth;

            LoadDropdowns();
            return View(new EmployeeDto());
        }

        // CREATE (POST)
        [HttpPost]
        public IActionResult Create(EmployeeDto emp)
        {
            if (!IsUserLoggedIn())
                return RedirectToLogin();

            var auth = RequireMenuAccess("/Employee/Create");
            if (auth != null) return auth;

            if (!ModelState.IsValid)
            {
                LoadDropdowns(emp);   // ✅ FIX
                return View(emp);
            }

            var request = new EmployeeCreateRequestDto
            {
                EmployeeCode = emp.EmployeeCode ?? "",
                FirstName = emp.FirstName ?? "",
                LastName = emp.LastName ?? "",
                Email = emp.Email ?? "",
                MobileNumber = emp.MobileNumber ?? "",
                DepartmentId = emp.DepartmentId,
                DesignationId = emp.DesignationId,
                RoleId = emp.RoleId,
                JoiningDate = emp.JoiningDate,
                Status = emp.Status,
                Username = emp.Username ?? "",
                Password = emp.Password ?? "",
                ReportingManagerId = emp.ReportingManagerId
            };

            try
            {
                _employeeService.Create(request, "MVC");

                TempData["EmployeeSuccess"] = "Employee created successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                LoadDropdowns(emp);   // ✅ FIX
                return View(emp);
            }
        }

        // EDIT (GET)
        public IActionResult Edit(string id)
        {
            if (!IsUserLoggedIn())
                return RedirectToLogin();

            var auth = RequireMenuAccess("/Employee/Edit");
            if (auth != null) return auth;

            int employeeId;
            try
            {
                employeeId = int.Parse(_idProtector.Unprotect(id));
            }
            catch
            {
                return BadRequest("Invalid employee id");
            }

            var emp = _employeeService.GetById(employeeId);
            if (emp == null) return NotFound();

            LoadDropdowns(emp);  // ✅ FIX

            ViewBag.EmployeeError = TempData["EmployeeError"];

            return View(emp);
        }

        // EDIT (POST)
        [HttpPost]
        public IActionResult Edit(EmployeeDto emp, string EncryptedEmployeeID)
        {
            if (!IsUserLoggedIn())
                return RedirectToLogin();

            var auth = RequireMenuAccess("/Employee/Edit");
            if (auth != null) return auth;

            if (!ModelState.IsValid)
            {
                LoadDropdowns(emp);   // ✅ FIX
                return View(emp);
            }

            int employeeId;
            try
            {
                employeeId = int.Parse(_idProtector.Unprotect(EncryptedEmployeeID));
            }
            catch
            {
                return BadRequest("Invalid Employee ID");
            }

            emp.EmployeeID = employeeId;
            emp.UpdatedBy = "MVC";

            try
            {
                _employeeService.UpdateEmployee(emp);

                ModelState.Clear();
                TempData["EmployeeSuccess"] = "Employee updated successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["EmployeeError"] = ex.Message;
                TempData.Remove("EmployeeSuccess"); // 🔥 important

                LoadDropdowns(emp);

                return View(emp);   // 🔥 THIS IS THE FIX
            }
        }

        // DELETE
        public IActionResult Delete(string id)
        {
            if (!IsUserLoggedIn())
                return RedirectToLogin();

            var auth = RequireMenuAccess("/Employee/Delete");
            if (auth != null) return auth;

            int employeeId;
            try
            {
                employeeId = int.Parse(_idProtector.Unprotect(id));
            }
            catch
            {
                return BadRequest("Invalid employee id");
            }

            var emp = _employeeService.GetById(employeeId);
            if (emp == null) return NotFound();

            return View(emp);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(string id)
        {
            if (!IsUserLoggedIn())
                return RedirectToLogin();

            var auth = RequireMenuAccess("/Employee/Delete");
            if (auth != null) return auth;

            int employeeId;
            try
            {
                employeeId = int.Parse(_idProtector.Unprotect(id));
            }
            catch
            {
                return BadRequest("Invalid employee id");
            }

            _employeeService.Delete(employeeId, "MVC");

            TempData["EmployeeSuccess"] = "Employee deleted successfully";
            return RedirectToAction(nameof(Index));
        }

        // UPDATE STATUS
        [HttpPost]
        public IActionResult UpdateStatus(string employeeId, bool status)
        {
            if (!IsUserLoggedIn())
                return Unauthorized();

            var auth = RequireMenuAccess("/Employee/UpdateStatus");
            if (auth != null) return auth;

            int id;
            try
            {
                id = int.Parse(_idProtector.Unprotect(employeeId));
            }
            catch
            {
                return BadRequest("Invalid employeeId");
            }

            _employeeService.UpdateStatus(id, status);
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> UploadDocument(string employeeId, string documentType, IFormFile file)
        {
            if (!IsUserLoggedIn())
                return RedirectToLogin();

            if (file == null || file.Length == 0)
                return RedirectToAction("Details", new { id = employeeId });

            if (file.Length > 5 * 1024 * 1024)
            {
                TempData["EmployeeError"] = "File size must be less than 5 MB.";
                return RedirectToAction("Details", new { id = employeeId });
            }

            if (string.IsNullOrWhiteSpace(documentType))
            {
                TempData["EmployeeError"] = "Please select a document type.";
                return RedirectToAction("Details", new { id = employeeId });
            }

            int id;
            try
            {
                id = int.Parse(_idProtector.Unprotect(employeeId));
            }
            catch
            {
                return BadRequest("Invalid employee id");
            }

            var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/documents");

            if (!Directory.Exists(uploads))
                Directory.CreateDirectory(uploads);

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploads, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            _employeeService.UploadEmployeeDocument(
                id,
                documentType,
                file.FileName,
                "/documents/" + fileName
            );

            return RedirectToAction("Details", new { id = employeeId });
        }

        [HttpPost]
        public IActionResult DeleteDocument(string documentId, string employeeId)
        {
            if (!IsUserLoggedIn())
                return RedirectToLogin();

            int docId;
            int empId;

            try
            {
                docId = int.Parse(_idProtector.Unprotect(documentId));
                empId = int.Parse(_idProtector.Unprotect(employeeId));
            }
            catch
            {
                return BadRequest("Invalid id");
            }

            _employeeService.DeleteEmployeeDocument(docId, empId);

            return RedirectToAction("Details", new
            {
                id = _idProtector.Protect(empId.ToString())
            });
        }

        [HttpGet]
        public IActionResult ExportToExcel()
        {
            var employees = _employeeService.GetAll();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Employees");

                worksheet.Cell(1, 1).Value = "Employee Code";
                worksheet.Cell(1, 2).Value = "Name";
                worksheet.Cell(1, 3).Value = "Email";
                worksheet.Cell(1, 4).Value = "Mobile";
                worksheet.Cell(1, 5).Value = "Department";
                worksheet.Cell(1, 6).Value = "Designation";
                worksheet.Cell(1, 7).Value = "Role";
                worksheet.Cell(1, 8).Value = "Joining Date";
                worksheet.Cell(1, 9).Value = "Status";

                int row = 2;

                foreach (var emp in employees)
                {
                    worksheet.Cell(row, 1).Value = emp.EmployeeCode;
                    worksheet.Cell(row, 2).Value = emp.EmployeeName;
                    worksheet.Cell(row, 3).Value = emp.Email;
                    worksheet.Cell(row, 4).Value = emp.MobileNumber;
                    worksheet.Cell(row, 5).Value = emp.DepartmentName;
                    worksheet.Cell(row, 6).Value = emp.DesignationName;
                    worksheet.Cell(row, 7).Value = emp.RoleName;
                    worksheet.Cell(row, 8).Value = emp.JoiningDate.ToString("dd-MM-yyyy");
                    worksheet.Cell(row, 9).Value = emp.Status ? "Active" : "Inactive";

                    row++;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Employees.xlsx");
                }
            }
        }

        [HttpGet]
        public IActionResult DownloadEmployeePdf(int id)   // ✅ FIXED
        {
            var emp = _employeeService.GetById(id);
            if (emp == null) return NotFound();

            using (var stream = new MemoryStream())
            {
                var writer = new PdfWriter(stream);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);

                var boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                var normalFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

                document.Add(new Paragraph("BAXY LIMITED")
                    .SetFont(boldFont).SetFontSize(18).SetTextAlignment(TextAlignment.CENTER));

                document.Add(new Paragraph(" "));
                document.Add(new Paragraph("Employee Details")
                    .SetFont(boldFont).SetFontSize(14).SetTextAlignment(TextAlignment.CENTER));

                document.Add(new Paragraph(" "));

                var table = new Table(2).UseAllAvailableWidth();

                void AddRow(string label, string value)
                {
                    table.AddCell(new Cell().Add(new Paragraph(label).SetFont(boldFont)));
                    table.AddCell(new Cell().Add(new Paragraph(value).SetFont(normalFont)));
                }

                AddRow("Employee Code", emp.EmployeeCode ?? "");
                AddRow("Name", $"{emp.FirstName} {emp.LastName}");
                AddRow("Email", emp.Email ?? "");
                AddRow("Mobile", emp.MobileNumber ?? "");
                AddRow("Department", emp.DepartmentName ?? "");
                AddRow("Designation", emp.DesignationName ?? "");
                AddRow("Role", emp.RoleName ?? "");
                AddRow("Joining Date", emp.JoiningDate.ToString("dd-MM-yyyy"));
                AddRow("Status", emp.Status ? "Active" : "Inactive");

                document.Add(table);

                document.Add(new Paragraph($"Generated on: {DateTime.Now:dd-MM-yyyy HH:mm}")
                    .SetFontSize(10).SetTextAlignment(TextAlignment.RIGHT));

                document.Close();

                return File(stream.ToArray(),
                    "application/pdf",
                    $"Employee_{emp.EmployeeCode}.pdf");
            }
        }

        [HttpGet]
        public IActionResult OrgChart(int id)
        {
            var data = _employeeService.GetOrgChart(id);
            return Json(data);
        }
    }
}