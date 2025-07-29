using AirlineWeb.Common;
using AirlineWeb.Models;
using System;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AirlineWeb.Areas.Admin.Controllers
{

    public class TuyenBayController : Controller
    {
        private DataAirline db = new DataAirline();

        #region Danh Sách Tuyến Bay
        public async Task<ActionResult> Index()
        {
            try
            {
                var tuyenbays = await db.TuyenBay.ToListAsync();
                return View(tuyenbays);
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

        #region Xem chi tiết Tuyến Bay
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            var tuyenbay = await db.TuyenBay.Include(tb => tb.SanBay).Include(tb => tb.SanBay1).FirstOrDefaultAsync(tb => tb.MaTuyenBay == id);
            if (tuyenbay == null) return HttpNotFound();
            return View(tuyenbay);
        }
        #endregion

        #region Thêm mới Tuyến Bay
        public ActionResult Create()
        {
            ViewBag.MaSanBayCatCanh = new SelectList(db.SanBay, "MaSanBay", "TenSanBay");
            ViewBag.MaSanBayHaCanh = new SelectList(db.SanBay, "MaSanBay", "TenSanBay");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "MaSanBayCatCanh,MaSanBayHaCanh,KhoangCach")] TuyenBay tuyenbay)
        {
            if (ModelState.IsValid)
            {
                db.TuyenBay.Add(tuyenbay);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.MaSanBayCatCanh = new SelectList(db.SanBay, "MaSanBay", "TenSanBay", tuyenbay.MaSanBayCatCanh);
            ViewBag.MaSanBayHaCanh = new SelectList(db.SanBay, "MaSanBay", "TenSanBay", tuyenbay.MaSanBayHaCanh);
            return View(tuyenbay);
        }
        #endregion

        #region Cập nhật Tuyến Bay
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            var tuyenbay = await db.TuyenBay.FindAsync(id);
            if (tuyenbay == null) return HttpNotFound();
            ViewBag.MaSanBayCatCanh = new SelectList(db.SanBay, "MaSanBay", "TenSanBay", tuyenbay.MaSanBayCatCanh);
            ViewBag.MaSanBayHaCanh = new SelectList(db.SanBay, "MaSanBay", "TenSanBay", tuyenbay.MaSanBayHaCanh);
            return View(tuyenbay);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "MaTuyenBay,MaSanBayCatCanh,MaSanBayHaCanh,KhoangCach")] TuyenBay tuyenbay)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tuyenbay).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.MaSanBayCatCanh = new SelectList(db.SanBay, "MaSanBay", "TenSanBay", tuyenbay.MaSanBayCatCanh);
            ViewBag.MaSanBayHaCanh = new SelectList(db.SanBay, "MaSanBay", "TenSanBay", tuyenbay.MaSanBayHaCanh);
            return View(tuyenbay);
        }
        #endregion

        #region Xóa Tuyến Bay
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            var tuyenbay = await db.TuyenBay.Include(tb => tb.SanBay).Include(tb => tb.SanBay1).FirstOrDefaultAsync(tb => tb.MaTuyenBay == id);
            if (tuyenbay == null) return HttpNotFound();
            return View(tuyenbay);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var tuyenbay = await db.TuyenBay.FindAsync(id);
            if (tuyenbay != null)
            {
                db.TuyenBay.Remove(tuyenbay);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
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