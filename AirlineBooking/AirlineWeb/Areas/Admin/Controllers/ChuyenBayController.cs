using AirlineWeb.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AirlineWeb.Areas.Admin.Controllers
{
    public class ChuyenBayController : Controller
    {
        private DataAirline db = new DataAirline();

        // GET: Admin/ChuyenBay
        public async Task<ActionResult> Index()
        {
            var chuyenbays = await db.ChuyenBay.Include(c => c.MayBay).Include(c => c.TuyenBay).ToListAsync();
            return View(chuyenbays);
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var chuyenbay = await db.ChuyenBay.Include(c => c.MayBay).Include(c => c.TuyenBay).FirstOrDefaultAsync(c => c.MaChuyenBay == id);
            if (chuyenbay == null) return HttpNotFound();
            return View(chuyenbay);
        }

        public ActionResult Create()
        {
            ViewBag.MaMayBay = new SelectList(db.MayBay.Select(mb => new { mb.MaMayBay, Display = mb.MaMayBay }), "MaMayBay", "Display");
            ViewBag.MaTuyenBay = new SelectList(db.TuyenBay.Select(tb => new { tb.MaTuyenBay, Display = tb.MaTuyenBay }), "MaTuyenBay", "Display");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "MaTuyenBay,MaMayBay,MaHangHangKhong,TrangThai,SoHieuChuyenBay")] ChuyenBay chuyenbay)
        {
            if (ModelState.IsValid)
            {
                db.ChuyenBay.Add(chuyenbay);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.MaMayBay = new SelectList(db.MayBay.Select(mb => new { mb.MaMayBay, Display = mb.MaMayBay }), "MaMayBay", "Display", chuyenbay.MaMayBay);
            ViewBag.MaTuyenBay = new SelectList(db.TuyenBay.Select(tb => new { tb.MaTuyenBay, Display = tb.MaTuyenBay }), "MaTuyenBay", "Display", chuyenbay.MaTuyenBay);
            return View(chuyenbay);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var chuyenbay = await db.ChuyenBay.FindAsync(id);
            if (chuyenbay == null) return HttpNotFound();
            ViewBag.MaMayBay = new SelectList(db.MayBay.Select(mb => new { mb.MaMayBay, Display = mb.MaMayBay }), "MaMayBay", "Display", chuyenbay.MaMayBay);
            ViewBag.MaTuyenBay = new SelectList(db.TuyenBay.Select(tb => new { tb.MaTuyenBay, Display = tb.MaTuyenBay }), "MaTuyenBay", "Display", chuyenbay.MaTuyenBay);
            return View(chuyenbay);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "MaChuyenBay,MaTuyenBay,MaMayBay,MaHangHangKhong,TrangThai,SoHieuChuyenBay")] ChuyenBay chuyenbay)
        {
            if (ModelState.IsValid)
            {
                db.Entry(chuyenbay).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.MaMayBay = new SelectList(db.MayBay.Select(mb => new { mb.MaMayBay, Display = mb.MaMayBay }), "MaMayBay", "Display", chuyenbay.MaMayBay);
            ViewBag.MaTuyenBay = new SelectList(db.TuyenBay.Select(tb => new { tb.MaTuyenBay, Display = tb.MaTuyenBay }), "MaTuyenBay", "Display", chuyenbay.MaTuyenBay);
            return View(chuyenbay);
        }

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var chuyenbay = await db.ChuyenBay.Include(c => c.MayBay).Include(c => c.TuyenBay).FirstOrDefaultAsync(c => c.MaChuyenBay == id);
            if (chuyenbay == null) return HttpNotFound();
            return View(chuyenbay);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var chuyenbay = await db.ChuyenBay.FindAsync(id);
            if (chuyenbay != null)
            {
                db.ChuyenBay.Remove(chuyenbay);
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