using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Appointer.Models;
using Appointer.Security;

namespace Appointer.Controllers
{
    public class ServicesController : Controller
    {
        private AppointerEntities db = new AppointerEntities();

        // GET: Services
        public ActionResult Index()
        {
            if (Session["userRole"].ToString() == "JobCorp" || Session["userRole"].ToString() == "JobOwner")
            {


            
            JobCorp jc = new JobCorp();
            var u = Int32.Parse(Session["userId"].ToString());
            jc = db.JobCorps.Where(acc => acc.UserId ==u).FirstOrDefault();
            //var services = db.Services.Include(s => s.JobCorp);
            var services = db.Services.Where(acc => acc.JobCorpId == jc.Id);
            return View(services.ToList());
            }

            else
            {

                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Services/Create
        public ActionResult Create()
        {


            //var services = db.Services.Include(s => s.JobCorp);
            //var services = db.Services.Where(acc => acc.JobCorpId.Equals(jc.Id));
            //ViewBag.JobCorpId = new SelectList(db.JobCorps, "Id", "RoleTitle");
            return View();
        }

        // POST: Services/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Service service)
        {
            JobCorp jc = new JobCorp();
            var u = Int32.Parse(Session["userId"].ToString());
            jc = db.JobCorps.Where(acc => acc.UserId.Equals(u)).FirstOrDefault();
            service.JobCorpId = jc.Id;
            if (ModelState.IsValid)
            {
                db.Services.Add(service);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            
            return View(service);
        }

        // GET: Services/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Service service = db.Services.Find(id);
            if (service == null)
            {
                return HttpNotFound();
            }
            
            return View(service);
        }

        // POST: Services/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Service service)
        {

            JobCorp jc = new JobCorp();
            var u = Int32.Parse(Session["userId"].ToString());
            jc = db.JobCorps.Where(acc => acc.UserId.Equals(u)).FirstOrDefault();
            service.JobCorpId = jc.Id;

            if (ModelState.IsValid)
            {
                db.Entry(service).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            return View(service);
        }

        // GET: Services/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Service service = db.Services.Find(id);
            if (service == null)
            {
                return HttpNotFound();
            }
            return View(service);
        }

        // POST: Services/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Service service = db.Services.Find(id);
            db.Services.Remove(service);
            db.SaveChanges();
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
