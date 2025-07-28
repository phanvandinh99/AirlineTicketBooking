using AirlineWeb.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

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
            ViewBag.MaChuyenBay = new SelectList(db.ChuyenBay, "MaChuyenBay", "MaChuyenBay");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "MaChuyenBay,NgayGioKhoiHanh,NgayGioHaCanh,NgayBay")] LichBay lichbay)
        {
            if (ModelState.IsValid)
            {
                db.LichBay.Add(lichbay);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.MaChuyenBay = new SelectList(db.ChuyenBay, "MaChuyenBay", "MaChuyenBay", lichbay.MaChuyenBay);
            return View(lichbay);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var lichbay = await db.LichBay.FindAsync(id);
            if (lichbay == null) return HttpNotFound();
            ViewBag.MaChuyenBay = new SelectList(db.ChuyenBay, "MaChuyenBay", "MaChuyenBay", lichbay.MaChuyenBay);
            return View(lichbay);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "MaLichBay,MaChuyenBay,NgayGioKhoiHanh,NgayGioHaCanh,NgayBay")] LichBay lichbay)
        {
            if (ModelState.IsValid)
            {
                db.Entry(lichbay).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.MaChuyenBay = new SelectList(db.ChuyenBay, "MaChuyenBay", "MaChuyenBay", lichbay.MaChuyenBay);
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