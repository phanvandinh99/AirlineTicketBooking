using AirlineWeb.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Collections.Generic; // Added for List

namespace AirlineWeb.Areas.Admin.Controllers
{
    public class LichBayController : Controller
    {
        private DataAirline db = new DataAirline();

        // GET: Admin/LichBay
        public async Task<ActionResult> Index()
        {
            var lichbays = await db.LichBay.Include(l => l.ChuyenBay).ToListAsync();
            return View(lichbays);
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var lichbay = await db.LichBay.Include(l => l.ChuyenBay).FirstOrDefaultAsync(l => l.MaLichBay == id);
            if (lichbay == null) return HttpNotFound();
            return View(lichbay);
        }

        public ActionResult Create()
        {
            ViewBag.MaChuyenBay = new SelectList(
                db.ChuyenBay.Select(cb => new {
                    cb.MaChuyenBay,
                    Display = cb.MaMayBay + " - " + cb.SoHieuChuyenBay
                }),
                "MaChuyenBay", "Display");
            ViewBag.HangVes = db.HangVe.ToList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "MaChuyenBay,NgayGioKhoiHanh,NgayGioHaCanh,NgayBay")] LichBay lichbay, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                db.LichBay.Add(lichbay);
                await db.SaveChangesAsync();
                // Sinh vé tự động
                var hangVes = db.HangVe.ToList();
                foreach (var hangVe in hangVes)
                {
                    int soLuong = 0;
                    decimal giaGoc = 0;
                    int.TryParse(form[$"SoLuong_{hangVe.MaHangVe}"], out soLuong);
                    decimal.TryParse(form[$"Gia_{hangVe.MaHangVe}"], out giaGoc);
                    if (soLuong > 0 && giaGoc > 0)
                    {
                        var veList = new List<VeChuyenBay>();
                        // Sinh ký hiệu ghế theo quy tắc từng hạng
                        if (hangVe.TenHangVe.ToLower().Contains("phổ thông") && !hangVe.TenHangVe.ToLower().Contains("cao cấp"))
                        {
                            // Phổ thông: 1A-F, 2A-F, ...
                            string[] cols = {"A","B","C","D","E","F"};
                            int row = 1, count = 0;
                            while (veList.Count < soLuong)
                            {
                                foreach (var col in cols)
                                {
                                    if (veList.Count < soLuong)
                                        veList.Add(new VeChuyenBay { MaLichBay = lichbay.MaLichBay, MaHangVe = hangVe.MaHangVe, SoGhe = $"{row}{col}", TrangThai = 0, GiaVND = giaGoc * hangVe.TyLeGia });
                                }
                                row++;
                            }
                        }
                        else if (hangVe.TenHangVe.ToLower().Contains("phổ thông cao cấp"))
                        {
                            // Phổ thông cao cấp: 1A-D, 2A-D, ...
                            string[] cols = {"A","B","C","D"};
                            int row = 1;
                            while (veList.Count < soLuong)
                            {
                                foreach (var col in cols)
                                {
                                    if (veList.Count < soLuong)
                                        veList.Add(new VeChuyenBay { MaLichBay = lichbay.MaLichBay, MaHangVe = hangVe.MaHangVe, SoGhe = $"{row}{col}", TrangThai = 0, GiaVND = giaGoc * hangVe.TyLeGia });
                                }
                                row++;
                            }
                        }
                        else if (hangVe.TenHangVe.ToLower().Contains("thương gia"))
                        {
                            // Thương gia: 1A, 1B, 2A, 2B, ...
                            string[] cols = {"A","B"};
                            int row = 1;
                            while (veList.Count < soLuong)
                            {
                                foreach (var col in cols)
                                {
                                    if (veList.Count < soLuong)
                                        veList.Add(new VeChuyenBay { MaLichBay = lichbay.MaLichBay, MaHangVe = hangVe.MaHangVe, SoGhe = $"{row}{col}", TrangThai = 0, GiaVND = giaGoc * hangVe.TyLeGia });
                                }
                                row++;
                            }
                        }
                        else if (hangVe.TenHangVe.ToLower().Contains("hạng nhất"))
                        {
                            // Hạng nhất: 1A, 2A, 3A, ...
                            int row = 1;
                            while (veList.Count < soLuong)
                            {
                                veList.Add(new VeChuyenBay { MaLichBay = lichbay.MaLichBay, MaHangVe = hangVe.MaHangVe, SoGhe = $"{row}A", TrangThai = 0, GiaVND = giaGoc * hangVe.TyLeGia });
                                row++;
                            }
                        }
                        // Lưu vé vào DB
                        db.VeChuyenBay.AddRange(veList);
                    }
                }
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.MaChuyenBay = new SelectList(
                db.ChuyenBay.Select(cb => new {
                    cb.MaChuyenBay,
                    Display = cb.MaMayBay + " - " + cb.SoHieuChuyenBay
                }),
                "MaChuyenBay", "Display", lichbay.MaChuyenBay);
            ViewBag.HangVes = db.HangVe.ToList();
            return View(lichbay);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            var lichbay = await db.LichBay.FindAsync(id);
            if (lichbay == null) return HttpNotFound();
            ViewBag.MaChuyenBay = new SelectList(
                db.ChuyenBay.Select(cb => new {
                    cb.MaChuyenBay,
                    Display = cb.MaMayBay + " - " + cb.SoHieuChuyenBay
                }),
                "MaChuyenBay", "Display", lichbay.MaChuyenBay);
            ViewBag.HangVes = db.HangVe.ToList();
            return View(lichbay);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "MaLichBay,MaChuyenBay,NgayGioKhoiHanh,NgayGioHaCanh,NgayBay")] LichBay lichbay, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                db.Entry(lichbay).State = EntityState.Modified;
                await db.SaveChangesAsync();
                // (Có thể bổ sung logic sinh/cập nhật vé ở đây nếu cần)
                return RedirectToAction("Index");
            }
            ViewBag.MaChuyenBay = new SelectList(
                db.ChuyenBay.Select(cb => new {
                    cb.MaChuyenBay,
                    Display = cb.MaMayBay + " - " + cb.SoHieuChuyenBay
                }),
                "MaChuyenBay", "Display", lichbay.MaChuyenBay);
            ViewBag.HangVes = db.HangVe.ToList();
            return View(lichbay);
        }

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var lichbay = await db.LichBay.Include(l => l.ChuyenBay).FirstOrDefaultAsync(l => l.MaLichBay == id);
            if (lichbay == null) return HttpNotFound();
            return View(lichbay);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var lichbay = await db.LichBay.FindAsync(id);
            if (lichbay != null)
            {
                db.LichBay.Remove(lichbay);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}