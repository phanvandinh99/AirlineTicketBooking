using AirlineWeb.Common;
using AirlineWeb.Models;
using System;
using System.Collections.Generic;
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

        #region Search Chuyến Bay
        [HttpPost]
        public async Task<ActionResult> SearchChuyenBay(
            string fromAirport,
            string toAirport,
            string departureDate,
            string returnDate = null,
            string tripType = "oneway"
        )
        {
            try
            {
                // Validate input
                if (string.IsNullOrEmpty(fromAirport) ||
                    string.IsNullOrEmpty(toAirport) ||
                    string.IsNullOrEmpty(departureDate))
                {
                    TempData["ErrorMessage"] = "Vui lòng nhập đầy đủ thông tin tìm kiếm!";
                    return RedirectToAction("Index");
                }

                // Parse dates
                if (!DateTime.TryParse(departureDate, out DateTime ngayDi))
                {
                    TempData["ErrorMessage"] = "Ngày bay không hợp lệ!";
                    return RedirectToAction("Index");
                }

                DateTime? ngayVe = null;
                if (tripType == "roundtrip" && !string.IsNullOrEmpty(returnDate))
                {
                    if (!DateTime.TryParse(returnDate, out DateTime tempNgayVe))
                    {
                        TempData["ErrorMessage"] = "Ngày về không hợp lệ!";
                        return RedirectToAction("Index");
                    }
                    ngayVe = tempNgayVe;
                }

                // Tìm chuyến bay
                var query = db.ChuyenBay
                    .Include(cb => cb.HangHangKhong)
                    .Include(cb => cb.MayBay)
                    .Include(cb => cb.MayBay.LoaiMayBay)
                    .Include(cb => cb.TuyenBay)
                    .Include(cb => cb.TuyenBay.SanBay)
                    .Include(cb => cb.TuyenBay.SanBay1)
                    .Include(cb => cb.LichBay)
                    .Where(cb => cb.TrangThai == Const.TrangThai_HoatDong);

                // Filter theo sân bay
                query = query.Where(cb =>
                    cb.TuyenBay.SanBay.MaSanBay == fromAirport &&
                    cb.TuyenBay.SanBay1.MaSanBay == toAirport);

                // Filter theo ngày bay
                query = query.Where(cb =>
                    cb.LichBay.Any(lb => DbFunctions.TruncateTime(lb.NgayBay) == ngayDi.Date));

                var chuyenBayDi = await Task.Run(() => query
                    .Include(cb => cb.HangHangKhong)
                    .Include(cb => cb.MayBay)
                    .Include(cb => cb.MayBay.LoaiMayBay)
                    .Include(cb => cb.TuyenBay)
                    .Include(cb => cb.TuyenBay.SanBay)
                    .Include(cb => cb.TuyenBay.SanBay1)
                    .ToList());

                // Nếu là khứ hồi, tìm chuyến bay về
                List<ChuyenBay> chuyenBayVe = null;
                if (tripType == "roundtrip" && ngayVe.HasValue)
                {
                    var queryVe = db.ChuyenBay
                        .Include(cb => cb.HangHangKhong)
                        .Include(cb => cb.MayBay)
                        .Include(cb => cb.MayBay.LoaiMayBay)
                        .Include(cb => cb.TuyenBay)
                        .Include(cb => cb.TuyenBay.SanBay)
                        .Include(cb => cb.TuyenBay.SanBay1)
                        .Include(cb => cb.LichBay)
                        .Where(cb => cb.TrangThai == Const.TrangThai_HoatDong)
                        .Where(cb =>
                            cb.TuyenBay.SanBay.MaSanBay == toAirport &&
                            cb.TuyenBay.SanBay1.MaSanBay == fromAirport)
                        .Where(cb =>
                            cb.LichBay.Any(lb => DbFunctions.TruncateTime(lb.NgayBay) == ngayVe.Value.Date));

                    chuyenBayVe = await Task.Run(() => queryVe.ToList());
                }

                // Lưu thông tin search và kết quả
                ViewBag.SearchMessage = $"Đã tìm kiếm: {fromAirport} → {toAirport}, Ngày: {departureDate}";
                ViewBag.SearchCount = chuyenBayDi.Count;
                ViewBag.SearchResults = chuyenBayDi;

                // Log search
                await Logger.InfoAsync($"Search flight: {fromAirport} -> {toAirport}, Date: {departureDate}, Type: {tripType}");

                // Lấy danh sách chuyến bay để hiển thị
                var allChuyenBay = await db.ChuyenBay
                    .Include(cb => cb.HangHangKhong)
                    .Include(cb => cb.MayBay)
                    .Include(cb => cb.MayBay.LoaiMayBay)
                    .Include(cb => cb.TuyenBay)
                    .Include(cb => cb.TuyenBay.SanBay)
                    .Include(cb => cb.TuyenBay.SanBay1)
                    .Where(cb => cb.TrangThai == Const.TrangThai_HoatDong)
                    .ToListAsync();

                return View("Index", allChuyenBay);
            }
            catch (Exception ex)
            {
                await Logger.ErrorAsync(ex);
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tìm kiếm chuyến bay!";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<JsonResult> SearchChuyenBayAjax(
            string fromAirport,
            string toAirport,
            string departureDate,
            string returnDate = null,
            string tripType = "oneway"
        )
        {
            try
            {
                // Validate input
                if (string.IsNullOrEmpty(fromAirport) ||
                    string.IsNullOrEmpty(toAirport) ||
                    string.IsNullOrEmpty(departureDate))
                {
                    return Json(new { success = false, message = "Vui lòng nhập đầy đủ thông tin tìm kiếm!" }, JsonRequestBehavior.AllowGet);
                }

                // Parse dates
                if (!DateTime.TryParse(departureDate, out DateTime ngayDi))
                {
                    return Json(new { success = false, message = "Ngày bay không hợp lệ!" }, JsonRequestBehavior.AllowGet);
                }

                DateTime? ngayVe = null;
                if (tripType == "roundtrip" && !string.IsNullOrEmpty(returnDate))
                {
                    if (!DateTime.TryParse(returnDate, out DateTime tempNgayVe))
                    {
                        return Json(new { success = false, message = "Ngày về không hợp lệ!" }, JsonRequestBehavior.AllowGet);
                    }
                    ngayVe = tempNgayVe;
                }

                // Tìm chuyến bay
                var query = db.ChuyenBay
                    .Include(cb => cb.HangHangKhong)
                    .Include(cb => cb.MayBay)
                    .Include(cb => cb.MayBay.LoaiMayBay)
                    .Include(cb => cb.TuyenBay)
                    .Include(cb => cb.TuyenBay.SanBay)
                    .Include(cb => cb.TuyenBay.SanBay1)
                    .Include(cb => cb.LichBay)
                    .Where(cb => cb.TrangThai == Const.TrangThai_HoatDong);

                // Filter theo sân bay
                query = query.Where(cb =>
                    cb.TuyenBay.SanBay.MaSanBay == fromAirport &&
                    cb.TuyenBay.SanBay1.MaSanBay == toAirport);

                // Filter theo ngày bay
                query = query.Where(cb =>
                    cb.LichBay.Any(lb => DbFunctions.TruncateTime(lb.NgayBay) == ngayDi.Date));

                var chuyenBayDi = await Task.Run(() =>
                    query.Select(cb => new
                    {
                        MaChuyenBay = cb.MaChuyenBay,
                        SoHieuChuyenBay = cb.SoHieuChuyenBay,
                        HangHangKhong = cb.HangHangKhong.TenHang,
                        SanBayDi = cb.TuyenBay.SanBay.TenSanBay,
                        SanBayDen = cb.TuyenBay.SanBay1.TenSanBay,
                        MayBay = cb.MayBay.LoaiMayBay.TenLoaiMayBay,
                        TrangThai = cb.TrangThai
                    }).ToList()
                );

                // Nếu là khứ hồi, tìm chuyến bay về
                List<object> chuyenBayVe = null;
                if (tripType == "roundtrip" && ngayVe.HasValue)
                {
                    var queryVe = db.ChuyenBay
                        .Include(cb => cb.HangHangKhong)
                        .Include(cb => cb.MayBay)
                        .Include(cb => cb.MayBay.LoaiMayBay)
                        .Include(cb => cb.TuyenBay)
                        .Include(cb => cb.TuyenBay.SanBay)
                        .Include(cb => cb.TuyenBay.SanBay1)
                        .Include(cb => cb.LichBay)
                        .Where(cb => cb.TrangThai == Const.TrangThai_HoatDong)
                        .Where(cb =>
                            cb.TuyenBay.SanBay.MaSanBay == toAirport &&
                            cb.TuyenBay.SanBay1.MaSanBay == fromAirport)
                        .Where(cb =>
                            cb.LichBay.Any(lb => DbFunctions.TruncateTime(lb.NgayBay) == ngayVe.Value.Date));

                    chuyenBayVe = await Task.Run(() =>
                        queryVe.Select(cb => new
                        {
                            MaChuyenBay = cb.MaChuyenBay,
                            SoHieuChuyenBay = cb.SoHieuChuyenBay,
                            HangHangKhong = cb.HangHangKhong.TenHang,
                            SanBayDi = cb.TuyenBay.SanBay.TenSanBay,
                            SanBayDen = cb.TuyenBay.SanBay1.TenSanBay,
                            MayBay = cb.MayBay.LoaiMayBay.TenLoaiMayBay,
                            TrangThai = cb.TrangThai
                        }).ToList<object>()
                    );
                }

                return Json(new
                {
                    success = true,
                    chuyenBayDi = chuyenBayDi,
                    chuyenBayVe = chuyenBayVe,
                    tripType = tripType
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                await Logger.ErrorAsync(ex);
                return Json(new { success = false, message = "Có lỗi xảy ra khi tìm kiếm chuyến bay!" }, JsonRequestBehavior.AllowGet);
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