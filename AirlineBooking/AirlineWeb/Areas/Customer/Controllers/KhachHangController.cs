using AirlineWeb.Common;
using AirlineWeb.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AirlineWeb.Areas.Customer.Controllers
{
    public class KhachHangController : Controller
    {
        private DataAirline db = new DataAirline();

        #region Quản lý vé
        [HttpGet]
        public async Task<ActionResult> QuanLyVe()
        {
            try
            {
                // Kiểm tra đăng nhập
                var maKhachHang = Session["MaKhachHang"] as string;
                if (string.IsNullOrEmpty(maKhachHang))
                {
                    TempData["ErrorMessage"] = "Vui lòng đăng nhập để xem vé đã đặt!";
                    return RedirectToAction("Index", "DangNhap", new { area = "Customer" });
                }

                // Lấy danh sách phiếu đặt vé của khách hàng
                var danhSachPhieuDatVe = await Task.Run(() =>
                    db.PhieuDatVe
                        .Include(pdv => pdv.VeChuyenBay)
                        .Include(pdv => pdv.VeChuyenBay.LichBay)
                        .Include(pdv => pdv.VeChuyenBay.LichBay.ChuyenBay)
                        .Include(pdv => pdv.VeChuyenBay.LichBay.ChuyenBay.HangHangKhong)
                        .Include(pdv => pdv.VeChuyenBay.LichBay.ChuyenBay.TuyenBay)
                        .Include(pdv => pdv.VeChuyenBay.LichBay.ChuyenBay.TuyenBay.SanBay)
                        .Include(pdv => pdv.VeChuyenBay.LichBay.ChuyenBay.TuyenBay.SanBay1)
                        .Include(pdv => pdv.VeChuyenBay.HangVe)
                        .Where(pdv => pdv.MaKhachHang == maKhachHang)
                        .OrderByDescending(pdv => pdv.NgayDat)
                        .ToList()
                );

                ViewBag.TenKhachHang = Session["TenKhachHang"] as string;
                return View(danhSachPhieuDatVe);
            }
            catch (Exception ex)
            {
                await Logger.ErrorAsync(ex);
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải danh sách vé!";
                return RedirectToAction("Index", "Home", new { area = "Customer" });
            }
        }

        [HttpGet]
        public async Task<ActionResult> ChiTietVe(int maPhieuDatVe)
        {
            try
            {
                // Kiểm tra đăng nhập
                var maKhachHang = Session["MaKhachHang"] as string;
                if (string.IsNullOrEmpty(maKhachHang))
                {
                    TempData["ErrorMessage"] = "Vui lòng đăng nhập để xem chi tiết vé!";
                    return RedirectToAction("Index", "DangNhap", new { area = "Customer" });
                }

                // Lấy chi tiết phiếu đặt vé
                var phieuDatVe = await Task.Run(() =>
                    db.PhieuDatVe
                        .Include(pdv => pdv.VeChuyenBay)
                        .Include(pdv => pdv.VeChuyenBay.LichBay)
                        .Include(pdv => pdv.VeChuyenBay.LichBay.ChuyenBay)
                        .Include(pdv => pdv.VeChuyenBay.LichBay.ChuyenBay.HangHangKhong)
                        .Include(pdv => pdv.VeChuyenBay.LichBay.ChuyenBay.TuyenBay)
                        .Include(pdv => pdv.VeChuyenBay.LichBay.ChuyenBay.TuyenBay.SanBay)
                        .Include(pdv => pdv.VeChuyenBay.LichBay.ChuyenBay.TuyenBay.SanBay1)
                        .Include(pdv => pdv.VeChuyenBay.HangVe)
                        .Include(pdv => pdv.HoaDon)
                        .FirstOrDefault(pdv => pdv.MaPhieuDatVe == maPhieuDatVe && pdv.MaKhachHang == maKhachHang)
                );

                if (phieuDatVe == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy thông tin vé!";
                    return RedirectToAction("QuanLyVe");
                }

                return View(phieuDatVe);
            }
            catch (Exception ex)
            {
                await Logger.ErrorAsync(ex);
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải chi tiết vé!";
                return RedirectToAction("QuanLyVe");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> HuyVe(int maPhieuDatVe)
        {
            try
            {
                // Kiểm tra đăng nhập
                var maKhachHang = Session["MaKhachHang"] as string;
                if (string.IsNullOrEmpty(maKhachHang))
                {
                    TempData["ErrorMessage"] = "Vui lòng đăng nhập để thực hiện thao tác này!";
                    return RedirectToAction("Index", "DangNhap", new { area = "Customer" });
                }

                // Lấy phiếu đặt vé
                var phieuDatVe = await Task.Run(() =>
                    db.PhieuDatVe
                        .Include(pdv => pdv.VeChuyenBay)
                        .FirstOrDefault(pdv => pdv.MaPhieuDatVe == maPhieuDatVe && pdv.MaKhachHang == maKhachHang)
                );

                if (phieuDatVe == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy thông tin vé!";
                    return RedirectToAction("QuanLyVe");
                }

                // Kiểm tra trạng thái vé
                if (phieuDatVe.TrangThai == 0) // Đã thanh toán
                {
                    TempData["ErrorMessage"] = "Không thể hủy vé đã thanh toán!";
                    return RedirectToAction("QuanLyVe");
                }

                // Kiểm tra thời gian hủy vé (trước 24h giờ bay)
                var thoiGianBay = phieuDatVe.VeChuyenBay.LichBay.NgayGioKhoiHanh;
                var thoiGianHienTai = DateTime.Now;
                var khoangCach = thoiGianBay - thoiGianHienTai;

                if (khoangCach.TotalHours < 24)
                {
                    TempData["ErrorMessage"] = "Không thể hủy vé trong vòng 24 giờ trước chuyến bay!";
                    return RedirectToAction("QuanLyVe");
                }

                // Cập nhật trạng thái vé về còn trống
                phieuDatVe.VeChuyenBay.TrangThai = 0;

                // Xóa phiếu đặt vé
                db.PhieuDatVe.Remove(phieuDatVe);
                await db.SaveChangesAsync();

                TempData["SuccessMessage"] = "Hủy vé thành công!";
                return RedirectToAction("QuanLyVe");
            }
            catch (Exception ex)
            {
                await Logger.ErrorAsync(ex);
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi hủy vé. Vui lòng thử lại!";
                return RedirectToAction("QuanLyVe");
            }
        }

        [HttpGet]
        public ActionResult ThongTinCaNhan()
        {
            try
            {
                // Kiểm tra đăng nhập
                var maKhachHang = Session["MaKhachHang"] as string;
                if (string.IsNullOrEmpty(maKhachHang))
                {
                    TempData["ErrorMessage"] = "Vui lòng đăng nhập để xem thông tin cá nhân!";
                    return RedirectToAction("Index", "DangNhap", new { area = "Customer" });
                }

                // Lấy thông tin khách hàng
                var khachHang = db.KhachHang.FirstOrDefault(kh => kh.MaKhachHang == maKhachHang);
                if (khachHang == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy thông tin khách hàng!";
                    return RedirectToAction("QuanLyVe");
                }

                // Lấy thống kê vé
                var danhSachPhieuDatVe = db.PhieuDatVe.Where(pdv => pdv.MaKhachHang == maKhachHang).ToList();
                ViewBag.TongVe = danhSachPhieuDatVe.Count;
                ViewBag.VeDaThanhToan = danhSachPhieuDatVe.Count(pdv => pdv.TrangThai == 0);
                ViewBag.VeChuaThanhToan = danhSachPhieuDatVe.Count(pdv => pdv.TrangThai != 0);

                return View(khachHang);
            }
            catch (Exception ex)
            {
                Logger.ErrorAsync(ex);
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải thông tin cá nhân!";
                return RedirectToAction("QuanLyVe");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CapNhatThongTin(string TenKhachHang, string Email, string SoDienThoai, string DiaChi)
        {
            try
            {
                // Kiểm tra đăng nhập
                var maKhachHang = Session["MaKhachHang"] as string;
                if (string.IsNullOrEmpty(maKhachHang))
                {
                    TempData["ErrorMessage"] = "Vui lòng đăng nhập để cập nhật thông tin!";
                    return RedirectToAction("Index", "DangNhap", new { area = "Customer" });
                }

                // Lấy thông tin khách hàng
                var khachHang = await Task.Run(() =>
                    db.KhachHang.FirstOrDefault(kh => kh.MaKhachHang == maKhachHang)
                );

                if (khachHang == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy thông tin khách hàng!";
                    return RedirectToAction("QuanLyVe");
                }

                // Cập nhật thông tin
                khachHang.TenKhachHang = TenKhachHang;
                khachHang.Email = Email;
                khachHang.SoDienThoai = SoDienThoai;
                khachHang.DiaChi = DiaChi;

                // Cập nhật session
                Session["TenKhachHang"] = TenKhachHang;
                Session["Email"] = Email;

                await db.SaveChangesAsync();

                TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
                return RedirectToAction("ThongTinCaNhan");
            }
            catch (Exception ex)
            {
                await Logger.ErrorAsync(ex);
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật thông tin!";
                return RedirectToAction("ThongTinCaNhan");
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