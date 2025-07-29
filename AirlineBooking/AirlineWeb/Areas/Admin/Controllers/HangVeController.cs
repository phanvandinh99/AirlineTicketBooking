using AirlineWeb.Common;
using AirlineWeb.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace AirlineWeb.Areas.Admin.Controllers
{
    public class HangVeController : Controller
    {
        private DataAirline db = new DataAirline();

        #region Danh Sách Hạng Vé
        public ActionResult Index()
        {
            try
            {
                var hangves = db.HangVe.ToList();
                return View(hangves);
            }
            catch (Exception ex)
            {
                Logger.ErrorAsync(ex);
                return RedirectToAction("NotFound", "Notification", new { area = "Customer" });
            }
        }
        #endregion

        #region Xem chi tiết Hạng Vé
        public ActionResult Details(string id)
        {
            try
            {
                if (id == null) return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
                var hangve = db.HangVe.Find(id);
                if (hangve == null) return HttpNotFound();
                return View(hangve);
            }
            catch (Exception ex)
            {
                Logger.ErrorAsync(ex);
                return RedirectToAction("NotFound", "Notification", new { area = "Customer" });
            }
        }
        #endregion

        #region Thêm mới Hạng Vé
        public ActionResult Create()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                Logger.ErrorAsync(ex);
                return RedirectToAction("NotFound", "Notification", new { area = "Customer" });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaHangVe,TenHangVe,TyLeGia")] HangVe hangve)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.HangVe.Add(hangve);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(hangve);
            }
            catch (Exception ex)
            {
                Logger.ErrorAsync(ex);
                return RedirectToAction("NotFound", "Notification", new { area = "Customer" });
            }
        }
        #endregion

        #region Cập nhật Hạng Vé
        public ActionResult Edit(string id)
        {
            try
            {
                if (id == null) return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
                var hangve = db.HangVe.Find(id);
                if (hangve == null) return HttpNotFound();
                return View(hangve);
            }
            catch (Exception ex)
            {
                Logger.ErrorAsync(ex);
                return RedirectToAction("NotFound", "Notification", new { area = "Customer" });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaHangVe,TenHangVe,TyLeGia")] HangVe hangve)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(hangve).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(hangve);
            }
            catch (Exception ex)
            {
                Logger.ErrorAsync(ex);
                return RedirectToAction("NotFound", "Notification", new { area = "Customer" });
            }
        }
        #endregion

        #region Xóa Hạng Vé
        public ActionResult Delete(string id)
        {
            try
            {
                if (id == null) return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
                var hangve = db.HangVe.Find(id);
                if (hangve == null) return HttpNotFound();
                return View(hangve);
            }
            catch (Exception ex)
            {
                Logger.ErrorAsync(ex);
                return RedirectToAction("NotFound", "Notification", new { area = "Customer" });
            }
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            try
            {
                var hangve = db.HangVe.Find(id);
                if (hangve != null)
                {
                    db.HangVe.Remove(hangve);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Logger.ErrorAsync(ex);
                return RedirectToAction("NotFound", "Notification", new { area = "Customer" });
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