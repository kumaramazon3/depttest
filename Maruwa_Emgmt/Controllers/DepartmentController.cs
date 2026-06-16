using Maruwa_Emgmt.BAL.master;
using Maruwa_Emgmt.Models;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text;

namespace Maruwa_Emgmt.Controllers
{
    public class DepartmentController : BaseController
    {
        private readonly bll_Department _bllDepartment;
        private readonly ILogger<DepartmentController> _logger;

        public DepartmentController(bll_Department bllDepartment, ILogger<DepartmentController> logger)
        {
            _bllDepartment = bllDepartment ?? throw new ArgumentNullException(nameof(bllDepartment));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Department Index");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDepartments()
        {
            try
            {
                var departments = await _bllDepartment.GetAllDepartmentsAsync();
                return Json(new { success = true, data = departments });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving departments");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddDepartment([FromBody] master_Department department)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(department?.departmentCode) ||
                    string.IsNullOrWhiteSpace(department?.departmentName) ||
                    string.IsNullOrWhiteSpace(department?.departmentPrefix))
                {
                    return Json(new { success = false, message = "All fields are mandatory" });
                }

                // Check for duplicates
                bool exists = await _bllDepartment.DepartmentExistsAsync(department.departmentCode);
                if (exists)
                {
                    return Json(new { success = false, message = "Department Already Exists" });
                }

                var userId = HttpContext.Session.GetString("EmpCode") ?? "System";
                department.createdBy = userId;
                department.subDepartment = department.subDepartment ?? "";
                department.departmentID = department.departmentID ?? "";

                var id = await _bllDepartment.AddDepartmentAsync(department);
                var newDepartment = await _bllDepartment.GetDepartmentByIdAsync(id);

                return Json(new { success = true, message = "Department added successfully", data = newDepartment });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding department");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDepartment([FromBody] master_Department department)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(department?.id))
                {
                    return Json(new { success = false, message = "Department ID is required" });
                }

                if (string.IsNullOrWhiteSpace(department?.departmentCode) ||
                    string.IsNullOrWhiteSpace(department?.departmentName) ||
                    string.IsNullOrWhiteSpace(department?.departmentPrefix))
                {
                    return Json(new { success = false, message = "All fields are mandatory" });
                }

                // Check for duplicates
                bool exists = await _bllDepartment.DepartmentExistsAsync(department.departmentCode, department.id);
                if (exists)
                {
                    return Json(new { success = false, message = "Department Already Exists" });
                }

                var userId = HttpContext.Session.GetString("EmpCode") ?? "System";
                department.modifiedBy = userId;
                department.subDepartment = department.subDepartment ?? "";
                department.departmentID = department.departmentID ?? "";

                bool result = await _bllDepartment.UpdateDepartmentAsync(department);

                if (result)
                {
                    var updatedDepartment = await _bllDepartment.GetDepartmentByIdAsync(department.id);
                    return Json(new { success = true, message = "Department updated successfully", data = updatedDepartment });
                }

                return Json(new { success = false, message = "Failed to update department" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating department");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteDepartment(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return Json(new { success = false, message = "Department ID is required" });
                }

                bool result = await _bllDepartment.DeleteDepartmentAsync(id);

                if (result)
                {
                    return Json(new { success = true, message = "Department deleted successfully" });
                }

                return Json(new { success = false, message = "Failed to delete department" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting department");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchDepartments(string searchTerm)
        {
            try
            {
                var departments = await _bllDepartment.SearchDepartmentsAsync(searchTerm);
                return Json(new { success = true, data = departments });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching departments");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ExportToExcel()
        {
            try
            {
                var departments = await _bllDepartment.GetAllDepartmentsAsync();

                var csv = new StringBuilder();
                csv.AppendLine("Department Code,Department Name,Prefix,Sub Department,Created By,Created Date,Modified By,Modified Date,Active Status");

                foreach (var dept in departments)
                {
                    csv.AppendLine($"\"{dept.departmentCode}\",\"{dept.departmentName}\",\"{dept.departmentPrefix}\",\"{dept.subDepartment ?? ""}\",\"{dept.createdBy}\",\"{dept.createdDate:yyyy-MM-dd HH:mm:ss}\",\"{dept.modifiedBy ?? ""}\",\"{dept.modifiedDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? ""}\",\"{(dept.idDeptActive == "Y" ? "Active" : "Inactive")}\"");
                }

                var bytes = Encoding.UTF8.GetBytes(csv.ToString());
                return File(bytes, "text/csv", $"Department_Master_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting to Excel");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ExportToCSV()
        {
            try
            {
                var departments = await _bllDepartment.GetAllDepartmentsAsync();

                var csv = new StringBuilder();
                csv.AppendLine("Department Code,Department Name,Prefix,Sub Department,Created By,Created Date,Modified By,Modified Date,Active Status");

                foreach (var dept in departments)
                {
                    csv.AppendLine($"\"{dept.departmentCode}\",\"{dept.departmentName}\",\"{dept.departmentPrefix}\",\"{dept.subDepartment ?? ""}\",\"{dept.createdBy}\",\"{dept.createdDate:yyyy-MM-dd HH:mm:ss}\",\"{dept.modifiedBy ?? ""}\",\"{dept.modifiedDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? ""}\",\"{(dept.idDeptActive == "Y" ? "Active" : "Inactive")}\"");
                }

                var bytes = Encoding.UTF8.GetBytes(csv.ToString());
                return File(bytes, "text/csv", $"Department_Master_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting to CSV");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ExportToPDF()
        {
            try
            {
                var departments = await _bllDepartment.GetAllDepartmentsAsync();

                // Generate HTML for PDF
                var html = new StringBuilder();
                html.AppendLine("<html><head><style>");
                html.AppendLine("table { border-collapse: collapse; width: 100%; }");
                html.AppendLine("th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }");
                html.AppendLine("th { background-color: #4CAF50; color: white; }");
                html.AppendLine("</style></head><body>");
                html.AppendLine("<h2>Department Master Report</h2>");
                html.AppendLine("<p>Generated on: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "</p>");
                html.AppendLine("<table>");
                html.AppendLine("<tr><th>Department Code</th><th>Department Name</th><th>Prefix</th><th>Sub Department</th><th>Created By</th><th>Created Date</th><th>Modified By</th><th>Modified Date</th><th>Active Status</th></tr>");

                foreach (var dept in departments)
                {
                    html.AppendLine("<tr>");
                    html.AppendLine($"<td>{dept.departmentCode}</td>");
                    html.AppendLine($"<td>{dept.departmentName}</td>");
                    html.AppendLine($"<td>{dept.departmentPrefix}</td>");
                    html.AppendLine($"<td>{dept.subDepartment ?? ""}</td>");
                    html.AppendLine($"<td>{dept.createdBy}</td>");
                    html.AppendLine($"<td>{dept.createdDate:yyyy-MM-dd HH:mm:ss}</td>");
                    html.AppendLine($"<td>{dept.modifiedBy ?? ""}</td>");
                    html.AppendLine($"<td>{dept.modifiedDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? ""}</td>");
                    html.AppendLine($"<td>{(dept.idDeptActive == "Y" ? "Active" : "Inactive")}</td>");
                    html.AppendLine("</tr>");
                }

                html.AppendLine("</table>");
                html.AppendLine("</body></html>");

                var bytes = Encoding.UTF8.GetBytes(html.ToString());
                return File(bytes, "application/pdf", $"Department_Master_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting to PDF");
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
