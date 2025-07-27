using System.Web.Mvc;
using System;
using System.Threading.Tasks;
using AirlineWeb.Common;

namespace AirlineWeb.Areas.Customer.Controllers
{
    public class HomeController : Controller
    {
        #region Home
        public async Task<ActionResult> Index()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                // Log lỗi
                await Logger.ErrorAsync(ex);

                // Trang trả về nếu xảy ra lỗi
                return RedirectToAction("404", "Error", new { area = "Customer" });
            }
        }
        #endregion
    }
}