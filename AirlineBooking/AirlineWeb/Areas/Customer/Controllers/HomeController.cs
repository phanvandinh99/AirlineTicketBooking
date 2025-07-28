using AirlineWeb.Common;
using AirlineWeb.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AirlineWeb.Areas.Customer.Controllers
{
    public class HomeController : Controller
    {
        private DataAirline db = new DataAirline();

        #region Home
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

        #region API - Lấy danh sách sân bay
        [HttpGet]
        public async Task<JsonResult> GetDanhSachSanBay()
        {
            try
            {
                var danhSachSanBay = await Task.Run(() => 
                    db.SanBay
                        .Where(sb => sb.QuocGia == "Việt Nam" || sb.QuocGia == "Vietnam")
                        .OrderBy(sb => sb.TenSanBay)
                        .Select(sb => new
                        {
                            MaSanBay = sb.MaSanBay,
                            TenSanBay = sb.TenSanBay,
                            ThanhPho = sb.ThanhPho,
                            QuocGia = sb.QuocGia
                        })
                        .ToList()
                );

                return Json(new { success = true, data = danhSachSanBay }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                await Logger.ErrorAsync(ex);
                return Json(new { success = false, message = "Có lỗi xảy ra khi lấy danh sách sân bay" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetSanBayDen(string maSanBayDi)
        {
            try
            {
                var danhSachSanBayDen = await Task.Run(() => 
                    db.SanBay
                        .Where(sb => sb.QuocGia == "Việt Nam" || sb.QuocGia == "Vietnam")
                        .Where(sb => sb.MaSanBay != maSanBayDi) // Loại bỏ sân bay đi
                        .OrderBy(sb => sb.TenSanBay)
                        .Select(sb => new
                        {
                            MaSanBay = sb.MaSanBay,
                            TenSanBay = sb.TenSanBay,
                            ThanhPho = sb.ThanhPho,
                            QuocGia = sb.QuocGia
                        })
                        .ToList()
                );

                return Json(new { success = true, data = danhSachSanBayDen }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                await Logger.ErrorAsync(ex);
                return Json(new { success = false, message = "Có lỗi xảy ra khi lấy danh sách sân bay đến" }, JsonRequestBehavior.AllowGet);
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