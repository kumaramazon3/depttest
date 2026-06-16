using Maruwa_Emgmt.BAL;
using Maruwa_Emgmt.BAL.master;
using Microsoft.AspNetCore.Mvc;

namespace Maruwa_Emgmt.Controllers
{
    public class masterController : Controller
    {  
        private readonly bll_Designation _blldesig;
        private readonly bal_tbldropdownData _dropdownBal;
        public masterController(bll_Designation blldesig, bal_tbldropdownData dropdownBal)
        {
            _blldesig = blldesig;
            _dropdownBal = dropdownBal;  // Assign the new BAL to the field
        }

        public IActionResult DesignationList()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetDesignationList()
        {
            try
            {
                var data = await _blldesig.GetAllDesignationAsync();
                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

    }
}
