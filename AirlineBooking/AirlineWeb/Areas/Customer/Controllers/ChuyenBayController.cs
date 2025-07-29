using AirlineWeb.Common;
using AirlineWeb.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Collections.Generic; // Added for List

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

                // Validate độ dài dữ liệu
                if (HoTenHanhKhach.Length > 50)
                {
                    TempData["ErrorMessage"] = "Họ tên hành khách không được vượt quá 50 ký tự!";
                    return RedirectToAction("DatVe", new { maVe = maVe });
                }

                if (CanCuoc.Length > 20)
                {
                    TempData["ErrorMessage"] = "Số căn cước không được vượt quá 20 ký tự!";
                    return RedirectToAction("DatVe", new { maVe = maVe });
                }

                if (!string.IsNullOrEmpty(Email) && Email.Length > 50)
                {
                    TempData["ErrorMessage"] = "Email không được vượt quá 50 ký tự!";
                    return RedirectToAction("DatVe", new { maVe = maVe });
                }

                if (!string.IsNullOrEmpty(SoDienThoai) && SoDienThoai.Length > 15)
                {
                    TempData["ErrorMessage"] = "Số điện thoại không được vượt quá 15 ký tự!";
                    return RedirectToAction("DatVe", new { maVe = maVe });
                }

                // Validate ngày sinh
                DateTime ngaySinhDate;
                if (!DateTime.TryParse(NgaySinh, out ngaySinhDate))
                {
                    TempData["ErrorMessage"] = "Ngày sinh không hợp lệ!";
                    return RedirectToAction("DatVe", new { maVe = maVe });
                }

                // Kiểm tra tuổi (phải từ 1 tuổi trở lên và không quá 120 tuổi)
                var tuoi = DateTime.Now.Year - ngaySinhDate.Year;
                if (ngaySinhDate > DateTime.Now.AddYears(-tuoi)) tuoi--;
                
                if (tuoi < 1 || tuoi > 120)
                {
                    TempData["ErrorMessage"] = "Tuổi không hợp lệ!";
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

                    // Validate khách hàng trước khi thêm
                    var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(khachHang, null, null);
                    var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                    
                    if (!System.ComponentModel.DataAnnotations.Validator.TryValidateObject(khachHang, validationContext, validationResults, true))
                    {
                        var errorMessages = string.Join("; ", validationResults.Select(v => v.ErrorMessage));
                        await Logger.ErrorAsync($"Validation failed for KhachHang: {errorMessages}");
                        TempData["ErrorMessage"] = $"Dữ liệu khách hàng không hợp lệ: {errorMessages}";
                        return RedirectToAction("DatVe", new { maVe = maVe });
                    }

                    db.KhachHang.Add(khachHang);
                    
                    try
                    {
                        await db.SaveChangesAsync();
                        await Logger.InfoAsync($"Created new customer: {CanCuoc}");
                        TempData["SuccessMessage"] = "Tài khoản đã được tạo tự động với mật khẩu là số căn cước của bạn!";
                    }
                    catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                    {
                        var errorMessages = new List<string>();
                        foreach (var eve in ex.EntityValidationErrors)
                        {
                            foreach (var ve_error in eve.ValidationErrors)
                            {
                                errorMessages.Add($"Property: {ve_error.PropertyName} Error: {ve_error.ErrorMessage}");
                                await Logger.ValidationErrorAsync("KhachHang", ve_error.PropertyName, ve_error.ErrorMessage);
                            }
                        }
                        await Logger.ErrorAsync($"DbEntityValidationException: {string.Join("; ", errorMessages)}");
                        TempData["ErrorMessage"] = $"Lỗi validation khi tạo khách hàng: {string.Join("; ", errorMessages)}";
                        return RedirectToAction("DatVe", new { maVe = maVe });
                    }
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

                // Validate phiếu đặt vé trước khi thêm
                var phieuValidationContext = new System.ComponentModel.DataAnnotations.ValidationContext(phieuDatVe, null, null);
                var phieuValidationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                
                if (!System.ComponentModel.DataAnnotations.Validator.TryValidateObject(phieuDatVe, phieuValidationContext, phieuValidationResults, true))
                {
                    var errorMessages = string.Join("; ", phieuValidationResults.Select(v => v.ErrorMessage));
                    await Logger.ErrorAsync($"Validation failed for PhieuDatVe: {errorMessages}");
                    TempData["ErrorMessage"] = $"Dữ liệu phiếu đặt vé không hợp lệ: {errorMessages}";
                    return RedirectToAction("DatVe", new { maVe = maVe });
                }

                // Cập nhật trạng thái vé
                ve.TrangThai = 1; // Đang giữ chỗ

                db.PhieuDatVe.Add(phieuDatVe);
                
                try
                {
                    await db.SaveChangesAsync();
                    await Logger.InfoAsync($"Ticket booked successfully: Ve={maVe}, KhachHang={CanCuoc}");
                    TempData["SuccessMessage"] = "Đặt vé thành công! Vui lòng thanh toán trong vòng 24 giờ.";
                    return RedirectToAction("ChiTiet", new { id = ve.LichBay.ChuyenBay.MaChuyenBay });
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    var errorMessages = new List<string>();
                    foreach (var eve in ex.EntityValidationErrors)
                    {
                        foreach (var ve_error in eve.ValidationErrors)
                        {
                            errorMessages.Add($"Property: {ve_error.PropertyName} Error: {ve_error.ErrorMessage}");
                            await Logger.ValidationErrorAsync("PhieuDatVe", ve_error.PropertyName, ve_error.ErrorMessage);
                        }
                    }
                    await Logger.ErrorAsync($"DbEntityValidationException: {string.Join("; ", errorMessages)}");
                    TempData["ErrorMessage"] = $"Lỗi validation khi đặt vé: {string.Join("; ", errorMessages)}";
                    return RedirectToAction("DatVe", new { maVe = maVe });
                }
            }
            catch (Exception ex)
            {
                await Logger.ErrorAsync(ex);
                TempData["ErrorMessage"] = $"Có lỗi xảy ra khi đặt vé: {ex.Message}";
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