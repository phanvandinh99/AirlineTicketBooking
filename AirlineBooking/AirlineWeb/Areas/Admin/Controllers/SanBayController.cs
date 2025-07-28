using AirlineWeb.Common;
using AirlineWeb.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AirlineWeb.Areas.Admin.Controllers
{
    public class SanBayController : Controller
    {
        private DataAirline db = new DataAirline();

        #region Danh Sách Sân Bay
        public async Task<ActionResult> Index()
        {
            try
            {
                var sanbays = await db.SanBay.ToListAsync();
                return View(sanbays);
            }
            catch (Exception ex)
            {
                // Log lỗi
                await Logger.ErrorAsync(ex);

                // Trả về trang 404
                return RedirectToAction("NotFound", "Notification", new { area = "Customer" });
            }
        }
        #endregion

        #region Giải phóng tài nguyên
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}