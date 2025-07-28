using AirlineWeb.Common;
using AirlineWeb.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AirlineWeb.Areas.Customer.Controllers
{
    public class ChuyenBayController : Controller
    {
        private DataAirline db = new DataAirline();

        #region Danh sách chuyến bay
        public async Task<ActionResult> Index()
        {
            try
            {
                // Lấy danh sách chuyến bay từ database
                var danhSachChuyenBay = await Task.Run(() =>
                    db.ChuyenBay
                        .Include(cb => cb.HangHangKhong)
                        .Include(cb => cb.MayBay)
                        .Include(cb => cb.MayBay.LoaiMayBay)
                        .Include(cb => cb.TuyenBay)
                        .Include(cb => cb.TuyenBay.SanBay) // Sân bay cất cánh
                        .Include(cb => cb.TuyenBay.SanBay1) // Sân bay hạ cánh
                        .Include(cb => cb.LichBay) // Lấy lịch bay
                        .Where(cb => cb.TrangThai == Const.TrangThai_HoatDong)
                        .OrderBy(cb => cb.SoHieuChuyenBay)
                        .ToList()
                );


                // Truyền danh sách chuyến bay sang view
                return View(danhSachChuyenBay);
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

        #region Chi tiết chuyến bay
        public async Task<ActionResult> ChiTiet(int id)
        {
            try
            {
                var chuyenBay = await Task.Run(() =>
                    db.ChuyenBay
                        .Include(cb => cb.HangHangKhong)
                        .Include(cb => cb.MayBay)
                        .Include(cb => cb.MayBay.LoaiMayBay)
                        .Include(cb => cb.TuyenBay)
                        .Include(cb => cb.TuyenBay.SanBay)
                        .Include(cb => cb.TuyenBay.SanBay1)
                        .Include(cb => cb.LichBay)
                        .FirstOrDefault(cb => cb.MaChuyenBay == id)
                );
                if (chuyenBay == null)
                    return HttpNotFound();
                return View(chuyenBay);
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