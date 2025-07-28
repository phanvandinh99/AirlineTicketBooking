using AirlineWeb.Common;
using AirlineWeb.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AirlineWeb.Areas.Admin.Controllers
{
    public class VeController : Controller
    {
        private DataAirline db = new DataAirline();

        #region Danh Sách Ve
        public async Task<ActionResult> Index()
        {
            try
            {
                var ves = await db.VeChuyenBay.ToListAsync();
                return View(ves);
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