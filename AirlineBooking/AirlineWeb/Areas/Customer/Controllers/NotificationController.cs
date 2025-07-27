using AirlineWeb.Common;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AirlineWeb.Areas.Customer.Controllers
{
    public class NotificationController : Controller
    {
        #region 404 error
        public async Task<ActionResult> NotFound()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                // Log lỗi
                await Logger.ErrorAsync(ex);

                return View();
            }
            finally
            {
                // Đảm bảo không có lỗi xảy ra trong quá trình xử lý
                Response.StatusCode = 404;
            }
        }
        #endregion
    }
}