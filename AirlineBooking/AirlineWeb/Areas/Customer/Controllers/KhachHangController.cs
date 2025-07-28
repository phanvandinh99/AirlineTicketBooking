using AirlineWeb.Common;
using AirlineWeb.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
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
                        .Include(pdv => pdv.VeChuyenBay.LichBay)
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
                    TempData["ErrorMessage"] = $"Không thể hủy vé trong vòng 24 giờ trước chuyến bay! (Còn {khoangCach.TotalHours:F1} giờ)";
                    return RedirectToAction("QuanLyVe");
                }

                // Xóa hóa đơn liên quan nếu có
                var hoaDon = db.HoaDon.FirstOrDefault(hd => hd.MaPhieuDatVe == maPhieuDatVe);
                if (hoaDon != null)
                {
                    db.HoaDon.Remove(hoaDon);
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
                TempData["ErrorMessage"] = $"Có lỗi xảy ra khi hủy vé: {ex.Message}";
                return RedirectToAction("QuanLyVe");
            }
        }



        #endregion

        #region Thanh toán VNPAY
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThanhToanVNPAY(int maPhieuDatVe)
        {
            try
            {
                // Kiểm tra đăng nhập
                var maKhachHang = Session["MaKhachHang"] as string;
                if (string.IsNullOrEmpty(maKhachHang))
                {
                    TempData["ErrorMessage"] = "Vui lòng đăng nhập để thực hiện thanh toán!";
                    return RedirectToAction("Index", "DangNhap", new { area = "Customer" });
                }

                // Lấy thông tin phiếu đặt vé
                var phieuDatVe = db.PhieuDatVe
                    .Include(pdv => pdv.VeChuyenBay)
                    .Include(pdv => pdv.VeChuyenBay.LichBay)
                    .Include(pdv => pdv.VeChuyenBay.LichBay.ChuyenBay)
                    .FirstOrDefault(pdv => pdv.MaPhieuDatVe == maPhieuDatVe && pdv.MaKhachHang == maKhachHang);

                if (phieuDatVe == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy thông tin vé!";
                    return RedirectToAction("QuanLyVe");
                }

                if (phieuDatVe.TrangThai == 0)
                {
                    TempData["ErrorMessage"] = "Vé này đã được thanh toán!";
                    return RedirectToAction("ChiTietVe", new { maPhieuDatVe = maPhieuDatVe });
                }

                // Thông tin VNPAY
                string returnUrl = $"{Request.Url.Scheme}://{Request.Url.Authority}/Customer/KhachHang/VNPAYReturn";
                string vnp_Url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
                string vnp_TmnCode = "QV4AJ3NO";
                string vnp_HashSecret = "3CP0V5HCDJ6VFE1YPVYL85YUHK1SGLLP";

                // Tạo các thông tin cần thiết để gửi sang VNPAY
                SortedList<string, string> vnp_Params = new SortedList<string, string>();
                vnp_Params.Add("vnp_Version", "2.0.0");
                vnp_Params.Add("vnp_Command", "pay");
                vnp_Params.Add("vnp_TmnCode", vnp_TmnCode);
                vnp_Params.Add("vnp_Locale", "vn");
                vnp_Params.Add("vnp_CurrCode", "VND");
                vnp_Params.Add("vnp_TxnRef", phieuDatVe.MaPhieuDatVe.ToString());
                vnp_Params.Add("vnp_OrderInfo", $"Thanh toan ve may bay - {phieuDatVe.VeChuyenBay.LichBay.ChuyenBay.MaChuyenBay}");
                vnp_Params.Add("vnp_Amount", ((long)(phieuDatVe.VeChuyenBay.GiaVND * 100)).ToString());
                vnp_Params.Add("vnp_ReturnUrl", returnUrl);
                vnp_Params.Add("vnp_IpAddr", Request.UserHostAddress);
                vnp_Params.Add("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));

                string vnp_SecureHashType = "SHA512";
                string vnp_SecureHash = HmacSHA512(vnp_HashSecret, string.Join("", vnp_Params.Select(kvp => kvp.Key + "=" + kvp.Value + "&").ToArray()).Trim('&'));

                vnp_Params.Add("vnp_SecureHashType", vnp_SecureHashType);
                vnp_Params.Add("vnp_SecureHash", vnp_SecureHash);

                string vnp_UrlEncode = vnp_Url + "?" + string.Join("&", vnp_Params.Select(kvp => kvp.Key + "=" + HttpUtility.UrlEncode(kvp.Value)).ToArray());
                return Redirect(vnp_UrlEncode);
            }
            catch (Exception ex)
            {
                Logger.ErrorAsync(ex);
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi khởi tạo thanh toán!";
                return RedirectToAction("QuanLyVe");
            }
        }

        [HttpGet]
        public ActionResult VNPAYReturn(string vnp_ResponseCode, string vnp_TransactionNo, string vnp_TxnRef, string vnp_Amount, string vnp_OrderInfo)
        {
            try
            {
                int maPhieuDatVe = int.Parse(vnp_TxnRef);
                var phieuDatVe = db.PhieuDatVe
                    .Include(pdv => pdv.VeChuyenBay)
                    .Include(pdv => pdv.VeChuyenBay.LichBay)
                    .Include(pdv => pdv.VeChuyenBay.LichBay.ChuyenBay)
                    .FirstOrDefault(pdv => pdv.MaPhieuDatVe == maPhieuDatVe);

                if (phieuDatVe == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy thông tin vé!";
                    return RedirectToAction("QuanLyVe");
                }

                // Xử lý kết quả thanh toán
                if (vnp_ResponseCode == "00")
                {
                    // Thanh toán thành công
                    phieuDatVe.TrangThai = 0; // Đã thanh toán
                    phieuDatVe.VeChuyenBay.TrangThai = 2; // Đã bán

                    // Tạo hóa đơn
                    var hoaDon = new HoaDon
                    {
                        MaPhieuDatVe = phieuDatVe.MaPhieuDatVe,
                        NgayLapHoaDon = DateTime.Now,
                        TongTien = phieuDatVe.VeChuyenBay.GiaVND,
                        TrangThaiThanhToan = 1 // Đã thanh toán
                    };

                    db.HoaDon.Add(hoaDon);
                    db.SaveChanges();

                    TempData["SuccessMessage"] = "Thanh toán thành công! Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi.";
                    return RedirectToAction("ThanhToanThanhCong", new { maPhieuDatVe = maPhieuDatVe });
                }
                else
                {
                    // Thanh toán thất bại
                    string errorMessage = GetVNPAYErrorMessage(vnp_ResponseCode);
                    TempData["ErrorMessage"] = $"Thanh toán thất bại: {errorMessage}";
                    return RedirectToAction("ChiTietVe", new { maPhieuDatVe = maPhieuDatVe });
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorAsync(ex);
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xử lý kết quả thanh toán!";
                return RedirectToAction("QuanLyVe");
            }
        }

        [HttpGet]
        public ActionResult ThanhToanThanhCong(int maPhieuDatVe)
        {
            try
            {
                var phieuDatVe = db.PhieuDatVe
                    .Include(pdv => pdv.VeChuyenBay)
                    .Include(pdv => pdv.VeChuyenBay.LichBay)
                    .Include(pdv => pdv.VeChuyenBay.LichBay.ChuyenBay)
                    .Include(pdv => pdv.VeChuyenBay.LichBay.ChuyenBay.HangHangKhong)
                    .Include(pdv => pdv.VeChuyenBay.LichBay.ChuyenBay.TuyenBay)
                    .Include(pdv => pdv.VeChuyenBay.LichBay.ChuyenBay.TuyenBay.SanBay)
                    .Include(pdv => pdv.VeChuyenBay.LichBay.ChuyenBay.TuyenBay.SanBay1)
                    .Include(pdv => pdv.VeChuyenBay.HangVe)
                    .Include(pdv => pdv.HoaDon)
                    .FirstOrDefault(pdv => pdv.MaPhieuDatVe == maPhieuDatVe);

                if (phieuDatVe == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy thông tin vé!";
                    return RedirectToAction("QuanLyVe");
                }

                return View(phieuDatVe);
            }
            catch (Exception ex)
            {
                Logger.ErrorAsync(ex);
                TempData["ErrorMessage"] = "Có lỗi xảy ra!";
                return RedirectToAction("QuanLyVe");
            }
        }

        private string HmacSHA512(string key, string data)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);

            using (var hmac = new HMACSHA512(keyBytes))
            {
                byte[] hashBytes = hmac.ComputeHash(dataBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        private string GetVNPAYErrorMessage(string responseCode)
        {
            switch (responseCode)
            {
                case "01": return "Giao dịch chưa hoàn tất";
                case "02": return "Giao dịch lỗi";
                case "04": return "Giao dịch đảo (Khách hàng đã bị trừ tiền tại Ngân hàng nhưng GD chưa thành công ở VNPAY)";
                case "05": return "VNPAY đang xử lý";
                case "06": return "VNPAY đã gửi yêu cầu hoàn tiền sang Ngân hàng";
                case "07": return "Giao dịch bị nghi ngờ gian lận";
                case "09": return "Giao dịch không thành công do: Thẻ/Tài khoản bị khóa";
                case "13": return "Giao dịch không thành công do Quý khách nhập sai mật khẩu xác thực giao dịch (OTP)";
                case "65": return "Giao dịch không thành công do tài khoản của Quý khách không đủ số dư";
                case "75": return "Ngân hàng thanh toán đang bảo trì";
                case "79": return "Giao dịch không thành công do Quý khách nhập sai mật khẩu thanh toán quốc tế";
                case "99": return "Các lỗi khác (lỗi còn lại, không có trong danh sách mã lỗi đã liệt kê)";
                default: return "Lỗi không xác định";
            }
        }
        #endregion

        #region Thông tin cá nhân
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