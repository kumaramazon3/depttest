using Maruwa_Emgmt.Models;

namespace Maruwa_Emgmt.InterFace.master
{
    public interface i_Department
    {
        Task<List<master_Department>> GetAllDepartmentsAsync();
        Task<master_Department> GetDepartmentByIdAsync(string id);
        Task<bool> DepartmentExistsAsync(string departmentCode, string? excludeId = null);
        Task<string> AddDepartmentAsync(master_Department department);
        Task<bool> UpdateDepartmentAsync(master_Department department);
        Task<bool> DeleteDepartmentAsync(string id);
        Task<List<master_Department>> SearchDepartmentsAsync(string searchTerm);
        Task<int> GetTotalDepartmentCountAsync();
    }
}
