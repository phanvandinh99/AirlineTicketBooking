using AirlineWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AirlineWeb.Areas.Admin.Controllers
{
    public class HangVeController : Controller
    {
        private DataAirline db = new DataAirline();

        #region Danh Sách Hạng Vé
        public ActionResult Index()
        {
            var hangves = db.HangVe.ToList();
            return View(hangves);
        }
        #endregion

        #region Xem chi tiết Hạng Vé
        public ActionResult Details(string id)
        {
            if (id == null) return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            var hangve = db.HangVe.Find(id);
            if (hangve == null) return HttpNotFound();
            return View(hangve);
        }
        #endregion

        #region Thêm mới Hạng Vé
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaHangVe,TenHangVe,TyLeGia")] HangVe hangve)
        {
            if (ModelState.IsValid)
            {
                db.HangVe.Add(hangve);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(hangve);
        }
        #endregion

        #region Cập nhật Hạng Vé
        public ActionResult Edit(string id)
        {
            if (id == null) return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            var hangve = db.HangVe.Find(id);
            if (hangve == null) return HttpNotFound();
            return View(hangve);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaHangVe,TenHangVe,TyLeGia")] HangVe hangve)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hangve).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(hangve);
        }
        #endregion

        #region Xóa Hạng Vé
        public ActionResult Delete(string id)
        {
            if (id == null) return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            var hangve = db.HangVe.Find(id);
            if (hangve == null) return HttpNotFound();
            return View(hangve);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            var hangve = db.HangVe.Find(id);
            if (hangve != null)
            {
                db.HangVe.Remove(hangve);
                db.SaveChanges();
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