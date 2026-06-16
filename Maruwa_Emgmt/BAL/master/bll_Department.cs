using Maruwa_Emgmt.DAL.master;
using Maruwa_Emgmt.InterFace.master;
using Maruwa_Emgmt.Models;

namespace Maruwa_Emgmt.BAL.master
{
    public class bll_Department
    {
        private readonly i_Department _da;
        private readonly ILogger<bll_Department> _logger;

        public bll_Department(i_Department da, ILogger<bll_Department> logger)
        {
            _da = da ?? throw new ArgumentNullException(nameof(da));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<master_Department>> GetAllDepartmentsAsync()
        {
            try
            {
                return await _da.GetAllDepartmentsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in BAL GetAllDepartmentsAsync");
                throw;
            }
        }

        public async Task<master_Department> GetDepartmentByIdAsync(string id)
        {
            try
            {
                return await _da.GetDepartmentByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in BAL GetDepartmentByIdAsync");
                throw;
            }
        }

        public async Task<bool> DepartmentExistsAsync(string departmentCode, string? excludeId = null)
        {
            try
            {
                return await _da.DepartmentExistsAsync(departmentCode, excludeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in BAL DepartmentExistsAsync");
                throw;
            }
        }

        public async Task<string> AddDepartmentAsync(master_Department department)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(department.departmentCode))
                    throw new ArgumentException("Department Code is required");

                if (string.IsNullOrWhiteSpace(department.departmentName))
                    throw new ArgumentException("Department Name is required");

                if (string.IsNullOrWhiteSpace(department.departmentPrefix))
                    throw new ArgumentException("Department Prefix is required");

                // Check for duplicates
                if (await _da.DepartmentExistsAsync(department.departmentCode))
                    throw new InvalidOperationException($"Department with code '{department.departmentCode}' already exists");

                return await _da.AddDepartmentAsync(department);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in BAL AddDepartmentAsync");
                throw;
            }
        }

        public async Task<bool> UpdateDepartmentAsync(master_Department department)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(department.id))
                    throw new ArgumentException("Department ID is required");

                if (string.IsNullOrWhiteSpace(department.departmentCode))
                    throw new ArgumentException("Department Code is required");

                if (string.IsNullOrWhiteSpace(department.departmentName))
                    throw new ArgumentException("Department Name is required");

                // Check for duplicates (excluding current record)
                if (await _da.DepartmentExistsAsync(department.departmentCode, department.id))
                    throw new InvalidOperationException($"Department with code '{department.departmentCode}' already exists");

                return await _da.UpdateDepartmentAsync(department);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in BAL UpdateDepartmentAsync");
                throw;
            }
        }

        public async Task<bool> DeleteDepartmentAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    throw new ArgumentException("Department ID is required");

                return await _da.DeleteDepartmentAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in BAL DeleteDepartmentAsync");
                throw;
            }
        }

        public async Task<List<master_Department>> SearchDepartmentsAsync(string searchTerm)
        {
            try
            {
                return await _da.SearchDepartmentsAsync(searchTerm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in BAL SearchDepartmentsAsync");
                throw;
            }
        }

        public async Task<int> GetTotalDepartmentCountAsync()
        {
            try
            {
                return await _da.GetTotalDepartmentCountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in BAL GetTotalDepartmentCountAsync");
                throw;
            }
        }
    }
}
