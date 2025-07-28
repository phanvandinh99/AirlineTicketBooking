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

        #region Đặt vé
        [HttpGet]
        public async Task<ActionResult> DatVe(int maVe)
        {
            try
            {
                // Lấy thông tin vé
                var ve = await Task.Run(() =>
                    db.VeChuyenBay
                        .Include(v => v.LichBay)
                        .Include(v => v.LichBay.ChuyenBay)
                        .Include(v => v.LichBay.ChuyenBay.HangHangKhong)
                        .Include(v => v.LichBay.ChuyenBay.TuyenBay)
                        .Include(v => v.LichBay.ChuyenBay.TuyenBay.SanBay)
                        .Include(v => v.LichBay.ChuyenBay.TuyenBay.SanBay1)
                        .Include(v => v.HangVe)
                        .FirstOrDefault(v => v.MaVe == maVe)
                );

                if (ve == null)
                    return HttpNotFound();

                // Kiểm tra vé đã được đặt chưa
                if (ve.TrangThai != 0) // 0: Còn trống
                {
                    TempData["ErrorMessage"] = "Vé này đã được đặt hoặc đang được giữ chỗ!";
                    return RedirectToAction("ChiTiet", new { id = ve.LichBay.ChuyenBay.MaChuyenBay });
                }

                return View(ve);
            }
            catch (Exception ex)
            {
                await Logger.ErrorAsync(ex);
                return RedirectToAction("NotFound", "Notification", new { area = "Customer" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DatVe(int maVe, string HoTenHanhKhach, string NgaySinh, string CanCuoc, string Email, string SoDienThoai)
        {
            try
            {
                // Validate dữ liệu đầu vào
                if (string.IsNullOrEmpty(HoTenHanhKhach) || string.IsNullOrEmpty(NgaySinh) || string.IsNullOrEmpty(CanCuoc))
                {
                    TempData["ErrorMessage"] = "Vui lòng nhập đầy đủ thông tin bắt buộc!";
                    return RedirectToAction("DatVe", new { maVe = maVe });
                }

                // Lấy thông tin vé
                var ve = await Task.Run(() =>
                    db.VeChuyenBay
                        .Include(v => v.LichBay)
                        .Include(v => v.LichBay.ChuyenBay)
                        .FirstOrDefault(v => v.MaVe == maVe)
                );

                if (ve == null)
                    return HttpNotFound();

                // Kiểm tra vé còn trống không
                if (ve.TrangThai != 0)
                {
                    TempData["ErrorMessage"] = "Vé này đã được đặt hoặc đang được giữ chỗ!";
                    return RedirectToAction("ChiTiet", new { id = ve.LichBay.ChuyenBay.MaChuyenBay });
                }

                // Kiểm tra và tạo khách hàng (sử dụng căn cước làm MaKhachHang)
                var khachHang = await Task.Run(() =>
                    db.KhachHang.FirstOrDefault(kh => kh.MaKhachHang == CanCuoc)
                );

                if (khachHang == null)
                {
                    // Tạo khách hàng mới
                    khachHang = new KhachHang
                    {
                        MaKhachHang = CanCuoc, // Sử dụng căn cước làm mã khách hàng
                        MatKhau = CanCuoc, // Mật khẩu mặc định là căn cước
                        TenKhachHang = HoTenHanhKhach,
                        Email = Email,
                        SoDienThoai = SoDienThoai
                    };

                    db.KhachHang.Add(khachHang);
                    await db.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Tài khoản đã được tạo tự động với mật khẩu là số căn cước của bạn!";
                }
                else
                {
                    // Cập nhật thông tin khách hàng nếu cần
                    if (!string.IsNullOrEmpty(Email) && khachHang.Email != Email)
                        khachHang.Email = Email;
                    if (!string.IsNullOrEmpty(SoDienThoai) && khachHang.SoDienThoai != SoDienThoai)
                        khachHang.SoDienThoai = SoDienThoai;
                    if (khachHang.TenKhachHang != HoTenHanhKhach)
                        khachHang.TenKhachHang = HoTenHanhKhach;

                    await db.SaveChangesAsync();
                }

                // Tạo phiếu đặt vé
                var phieuDatVe = new PhieuDatVe
                {
                    MaVe = maVe,
                    MaKhachHang = khachHang.MaKhachHang,
                    HoTenHanhKhach = HoTenHanhKhach,
                    NgaySinh = NgaySinh,
                    CanCuoc = CanCuoc,
                    NgayDat = DateTime.Now,
                    TrangThai = 1 // Chưa thanh toán
                };

                // Cập nhật trạng thái vé
                ve.TrangThai = 1; // Đang giữ chỗ

                db.PhieuDatVe.Add(phieuDatVe);
                await db.SaveChangesAsync();

                TempData["SuccessMessage"] = "Đặt vé thành công! Vui lòng thanh toán trong vòng 24 giờ.";
                return RedirectToAction("ChiTiet", new { id = ve.LichBay.ChuyenBay.MaChuyenBay });
            }
            catch (Exception ex)
            {
                await Logger.ErrorAsync(ex);
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi đặt vé. Vui lòng thử lại!";
                return RedirectToAction("DatVe", new { maVe = maVe });
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