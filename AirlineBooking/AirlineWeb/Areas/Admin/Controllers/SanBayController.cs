using AirlineWeb.Common;
using AirlineWeb.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AirlineWeb.Areas.Admin.Controllers
{
    public class SanBayController : Controller
    {
        private DataAirline db = new DataAirline();

        #region Danh Sách Sân Bay
        public async Task<ActionResult> Index()
        {
            try
            {
                var sanbays = await db.SanBay.ToListAsync();
                return View(sanbays);
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

        #region Xem chi tiết Sân Bay
        public async Task<ActionResult> Details(string id)
        {
            if (id == null) return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            var sanbay = await db.SanBay.FindAsync(id);
            if (sanbay == null) return HttpNotFound();
            return View(sanbay);
        }
        #endregion

        #region Thêm mới Sân Bay
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "MaSanBay,TenSanBay,ThanhPho,QuocGia")] SanBay sanbay)
        {
            if (ModelState.IsValid)
            {
                db.SanBay.Add(sanbay);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(sanbay);
        }
        #endregion

        #region Cập nhật Sân Bay
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null) return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            var sanbay = await db.SanBay.FindAsync(id);
            if (sanbay == null) return HttpNotFound();
            return View(sanbay);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "MaSanBay,TenSanBay,ThanhPho,QuocGia")] SanBay sanbay)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sanbay).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(sanbay);
        }
        #endregion

        #region Xóa Sân Bay
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null) return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            var sanbay = await db.SanBay.FindAsync(id);
            if (sanbay == null) return HttpNotFound();
            return View(sanbay);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            var sanbay = await db.SanBay.FindAsync(id);
            if (sanbay != null)
            {
                db.SanBay.Remove(sanbay);
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