using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AirlineWeb.Models;
using AirlineWeb.Common;

namespace AirlineWeb.Areas.Admin.Controllers
{
    public class ThongKeController : Controller
    {
        private DataAirline db = new DataAirline();

        // GET: Admin/ThongKe
        public ActionResult Index(string type = "ngay")
        {
            try
            {
                // Lấy ngày hiện tại
                DateTime now = DateTime.Now;
                // Thống kê số vé bán theo ngày/tuần/tháng/quý/năm
                var veQuery = db.VeChuyenBay.Where(v => v.TrangThai == 1 && v.LichBay != null);
                var doanhThuQuery = db.VeChuyenBay.Where(v => v.TrangThai == 1 && v.LichBay != null && v.GiaVND != null);
                List<string> labels = new List<string>();
                List<int> soVe = new List<int>();
                List<decimal> doanhThu = new List<decimal>();
                if (type == "ngay")
                {
                    // 7 ngày gần nhất
                    for (int i = 6; i >= 0; i--)
                    {
                        var day = now.Date.AddDays(-i);
                        labels.Add(day.ToString("dd/MM"));
                        soVe.Add(veQuery.Count(v => v.LichBay.NgayBay == day));
                        doanhThu.Add(doanhThuQuery.Where(v => v.LichBay.NgayBay == day).Sum(v => v.GiaVND) ?? 0);
                    }
                }
                else if (type == "tuan")
                {
                    // 4 tuần gần nhất
                    for (int i = 3; i >= 0; i--)
                    {
                        var start = now.Date.AddDays(-7 * i);
                        var end = start.AddDays(6);
                        labels.Add($"{start:dd/MM}-{end:dd/MM}");
                        soVe.Add(veQuery.Count(v => v.LichBay.NgayBay >= start && v.LichBay.NgayBay <= end));
                        doanhThu.Add(doanhThuQuery.Where(v => v.LichBay.NgayBay >= start && v.LichBay.NgayBay <= end).Sum(v => v.GiaVND) ?? 0);
                    }
                }
                else if (type == "thang")
                {
                    // 6 tháng gần nhất
                    for (int i = 5; i >= 0; i--)
                    {
                        var month = now.AddMonths(-i);
                        labels.Add($"{month:MM/yyyy}");
                        soVe.Add(veQuery.Count(v => v.LichBay.NgayBay.Month == month.Month && v.LichBay.NgayBay.Year == month.Year));
                        doanhThu.Add(doanhThuQuery.Where(v => v.LichBay.NgayBay.Month == month.Month && v.LichBay.NgayBay.Year == month.Year).Sum(v => v.GiaVND) ?? 0);
                    }
                }
                else if (type == "quy")
                {
                    // 4 quý gần nhất
                    int year = now.Year;
                    int quyHienTai = (now.Month - 1) / 3 + 1;
                    for (int i = 3; i >= 0; i--)
                    {
                        int quy = quyHienTai - i;
                        int y = year;
                        while (quy <= 0) { quy += 4; y--; }
                        int startMonth = (quy - 1) * 3 + 1;
                        int endMonth = startMonth + 2;
                        labels.Add($"Q{quy}/{y}");
                        soVe.Add(veQuery.Count(v => v.LichBay.NgayBay.Year == y && v.LichBay.NgayBay.Month >= startMonth && v.LichBay.NgayBay.Month <= endMonth));
                        doanhThu.Add(doanhThuQuery.Where(v => v.LichBay.NgayBay.Year == y && v.LichBay.NgayBay.Month >= startMonth && v.LichBay.NgayBay.Month <= endMonth).Sum(v => v.GiaVND) ?? 0);
                    }
                }
                else if (type == "nam")
                {
                    // 5 năm gần nhất
                    for (int i = 4; i >= 0; i--)
                    {
                        int y = now.Year - i;
                        labels.Add(y.ToString());
                        soVe.Add(veQuery.Count(v => v.LichBay.NgayBay.Year == y));
                        doanhThu.Add(doanhThuQuery.Where(v => v.LichBay.NgayBay.Year == y).Sum(v => v.GiaVND) ?? 0);
                    }
                }
                ViewBag.Labels = labels;
                ViewBag.SoVe = soVe;
                ViewBag.DoanhThu = doanhThu;
                ViewBag.Type = type;
                // Tổng quan
                ViewBag.TongChuyenBay = db.ChuyenBay.Count();
                ViewBag.TongLichBay = db.LichBay.Count();
                ViewBag.TongVeBan = db.VeChuyenBay.Count(v => v.TrangThai == 1);
                ViewBag.TongDoanhThu = db.VeChuyenBay.Where(v => v.TrangThai == 1 && v.GiaVND != null).Sum(v => v.GiaVND) ?? 0;
                // Top tuyến bay doanh thu cao nhất
                ViewBag.TopTuyenBay = db.TuyenBay.Select(tb => new {
                    Tuyen = tb.SanBay.TenSanBay + " - " + tb.SanBay1.TenSanBay,
                    DoanhThu = tb.ChuyenBay.SelectMany(cb => cb.LichBay.SelectMany(lb => lb.VeChuyenBay)).Where(v => v.TrangThai == 1 && v.GiaVND != null).Sum(v => v.GiaVND) ?? 0
                }).OrderByDescending(x => x.DoanhThu).Take(5).ToList();
                // Số vé theo hạng vé
                ViewBag.VeTheoHangVe = db.HangVe.Select(hv => new {
                    hv.TenHangVe,
                    SoLuong = hv.VeChuyenBay.Count(v => v.TrangThai == 1)
                }).ToList();
                // Số chuyến bay theo máy bay
                ViewBag.ChuyenBayTheoMayBay = db.MayBay.Select(mb => new {
                    mb.MaMayBay,
                    SoLuong = mb.ChuyenBay.Count()
                }).ToList();
                return View();
            }
            catch (Exception ex)
            {
                Logger.ErrorAsync(ex);
                return RedirectToAction("NotFound", "Notification", new { area = "Customer" });
            }
        }
    }
}