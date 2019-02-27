using Appointer.DAL;
using Appointer.Models;
using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Appointer.Controllers
{
    public class ReserveController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public async Task<ActionResult> Index()
        {
            var list = await db.Reserves.ToListAsync();
            return View(list);
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var reserve = await db.Reserves.FindAsync(id);
            if (reserve == null)
            {
                return HttpNotFound();
            }
            return View(reserve);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Reserve reserve)
        {
            if (ModelState.IsValid)
            {
                db.Reserves.Add(reserve);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(reserve);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var reserve = await db.Reserves.FindAsync(id);
            if (reserve == null)
            {
                return HttpNotFound();
            }
            return View(reserve);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Reserve reserve)
        {
            if (ModelState.IsValid)
            {
                db.Entry(reserve).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(reserve);
        }


        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var reserve = await db.Reserves.FindAsync(id);
            if (reserve == null)
            {
                return HttpNotFound();
            }
            return View(reserve);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Developer,Admin")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Reserve reserve = await db.Reserves.FindAsync(id);
            db.Reserves.Remove(reserve);
            await db.SaveChangesAsync();
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
