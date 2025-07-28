using AirlineWeb.Common;
using AirlineWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AirlineWeb.Areas.Customer.Controllers
{
    public class DangNhapController : Controller
    {
        private DataAirline db = new DataAirline();

        #region Đăng nhập
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            try
            {
                // Kiểm tra nếu đã đăng nhập thì chuyển đến trang quản lý vé
                if (Session["MaKhachHang"] != null)
                {
                    return RedirectToAction("QuanLyVe", "KhachHang", new { area = "Customer" });
                }
                return View();
            }
            catch (Exception ex)
            {
                // Log lỗi
                await Logger.ErrorAsync(ex);

                // Trả về trang 404
                return RedirectToAction("NotFound", "Notification", new { area = "Customer" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(string MaKhachHang, string MatKhau)
        {
            try
            {
                if (string.IsNullOrEmpty(MaKhachHang) || string.IsNullOrEmpty(MatKhau))
                {
                    TempData["ErrorMessage"] = "Vui lòng nhập đầy đủ thông tin đăng nhập!";
                    return View();
                }

                // Tìm khách hàng theo mã khách hàng (căn cước)
                var khachHang = await Task.Run(() =>
                    db.KhachHang.FirstOrDefault(kh => kh.MaKhachHang == MaKhachHang)
                );

                if (khachHang == null)
                {
                    TempData["ErrorMessage"] = "Tài khoản không tồn tại! Vui lòng kiểm tra lại số căn cước.";
                    return View();
                }

                // Kiểm tra mật khẩu
                if (khachHang.MatKhau != MatKhau)
                {
                    TempData["ErrorMessage"] = "Mật khẩu không đúng! Vui lòng thử lại.";
                    return View();
                }

                // Đăng nhập thành công - lưu thông tin vào Session
                Session["MaKhachHang"] = khachHang.MaKhachHang;
                Session["TenKhachHang"] = khachHang.TenKhachHang;
                Session["Email"] = khachHang.Email;

                TempData["SuccessMessage"] = "Đăng nhập thành công! Chào mừng " + khachHang.TenKhachHang;
                
                // Chuyển đến trang quản lý vé
                return RedirectToAction("QuanLyVe", "KhachHang", new { area = "Customer" });
            }
            catch (Exception ex)
            {
                await Logger.ErrorAsync(ex);
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi đăng nhập. Vui lòng thử lại!";
                return View();
            }
        }

        [HttpGet]
        public ActionResult DangXuat()
        {
            // Xóa session
            Session.Clear();
            Session.Abandon();
            
            TempData["SuccessMessage"] = "Đăng xuất thành công!";
            return RedirectToAction("Index", "Home", new { area = "Customer" });
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