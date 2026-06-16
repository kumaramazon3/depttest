using Maruwa_Emgmt.DBcontex;
using Maruwa_Emgmt.InterFace.master;
using Maruwa_Emgmt.Models;
using Microsoft.EntityFrameworkCore;

namespace Maruwa_Emgmt.DAL.master
{
    public class da_Department : i_Department
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<da_Department> _logger;

        public da_Department(ApplicationDbContext context, ILogger<da_Department> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<master_Department>> GetAllDepartmentsAsync()
        {
            try
            {
                var departments = await _context.master_department
                    .Where(x => x.idDeptActive == "Y")
                    .OrderByDescending(x => x.createdDate)
                    .ToListAsync();
                return departments;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database error in GetAllDepartmentsAsync");
                throw;
            }
        }

        public async Task<master_Department> GetDepartmentByIdAsync(string id)
        {
            try
            {
                var department = await _context.master_department
                    .FirstOrDefaultAsync(x => x.id == id && x.idDeptActive == "Y");
                return department;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database error in GetDepartmentByIdAsync");
                throw;
            }
        }

        public async Task<bool> DepartmentExistsAsync(string departmentCode, string? excludeId = null)
        {
            try
            {
                var query = _context.master_department
                    .Where(x => x.departmentCode == departmentCode && x.idDeptActive == "Y");

                if (!string.IsNullOrEmpty(excludeId))
                {
                    query = query.Where(x => x.id != excludeId);
                }

                return await query.AnyAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database error in DepartmentExistsAsync");
                throw;
            }
        }

        public async Task<string> AddDepartmentAsync(master_Department department)
        {
            try
            {
                if (department == null)
                    throw new ArgumentNullException(nameof(department));

                department.id = Guid.NewGuid().ToString();
                department.idDeptActive = "Y";
                department.createdDate = DateTime.Now;

                _context.master_department.Add(department);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Department '{department.departmentName}' added successfully.");
                return department.id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding department");
                throw;
            }
        }

        public async Task<bool> UpdateDepartmentAsync(master_Department department)
        {
            try
            {
                if (department == null)
                    throw new ArgumentNullException(nameof(department));

                var existingDept = await _context.master_department
                    .FirstOrDefaultAsync(x => x.id == department.id);

                if (existingDept == null)
                    throw new InvalidOperationException("Department not found");

                existingDept.departmentCode = department.departmentCode;
                existingDept.departmentName = department.departmentName;
                existingDept.departmentPrefix = department.departmentPrefix;
                existingDept.subDepartment = department.subDepartment;
                existingDept.modifiedBy = department.modifiedBy;
                existingDept.modifiedDate = DateTime.Now;

                _context.master_department.Update(existingDept);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Department '{department.departmentName}' updated successfully.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating department");
                throw;
            }
        }

        public async Task<bool> DeleteDepartmentAsync(string id)
        {
            try
            {
                var department = await _context.master_department
                    .FirstOrDefaultAsync(x => x.id == id);

                if (department == null)
                    throw new InvalidOperationException("Department not found");

                // Soft delete
                department.idDeptActive = "N";
                department.modifiedDate = DateTime.Now;

                _context.master_department.Update(department);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Department '{department.departmentName}' deleted successfully.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting department");
                throw;
            }
        }

        public async Task<List<master_Department>> SearchDepartmentsAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return await GetAllDepartmentsAsync();

                var searchLower = searchTerm.ToLower();
                var departments = await _context.master_department
                    .Where(x => x.idDeptActive == "Y" &&
                        (x.departmentCode.ToLower().Contains(searchLower) ||
                         x.departmentName.ToLower().Contains(searchLower) ||
                         x.departmentPrefix.ToLower().Contains(searchLower) ||
                         x.subDepartment.ToLower().Contains(searchLower) ||
                         (x.createdBy != null && x.createdBy.ToLower().Contains(searchLower))))
                    .OrderByDescending(x => x.createdDate)
                    .ToListAsync();

                return departments;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database error in SearchDepartmentsAsync");
                throw;
            }
        }

        public async Task<int> GetTotalDepartmentCountAsync()
        {
            try
            {
                return await _context.master_department
                    .Where(x => x.idDeptActive == "Y")
                    .CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database error in GetTotalDepartmentCountAsync");
                throw;
            }
        }
    }
}
