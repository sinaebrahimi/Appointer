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
    public class JobsController : Controller
    {
        private AppointerEntities db = new AppointerEntities();

       // private JobModel jm = new JobModel();

        // GET: Jobs
        public ActionResult Index()
        {
            //should just be available for admin
            var jobs = db.Jobs.Include(j => j.User).Include(j => j.JobType);
            return View(jobs.ToList());
        }

        // GET: Jobs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Job job = db.Jobs.Find(id);
            if (job == null)
            {
                return HttpNotFound();
            }
            return View(job);
        }

        // GET: Jobs/Create

        [HttpGet]
        public ActionResult Create()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("SignIn", "Account");
            }
            if (SessionPersister.UserRole.ToString() == "User")
            {
                return RedirectToAction("Index", "Home");
            }
            if (SessionPersister.UserRole.ToString() == "JobCorp")
            {
                return RedirectToAction("EnrollJob", "JCDashboard");
            }

            //return View(ViewBag.FormTypeId);

            ViewBag.cities = db.Cities.ToList();
            ViewBag.jt = db.JobTypes.ToList();
            return View();
        }

        // POST: Jobs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,JobPhone,Address,About,EnrollmentKey,CityId,JobTypeId,JobOwnerId")] Job job)
        //public ActionResult Create(Job job)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("SignIn", "Account");
            }
            else if(SessionPersister.UserRole.ToString() == "User")
            {
                return RedirectToAction("Index", "Home");
            }
            else if (SessionPersister.UserRole.ToString() == "JobCorp")
            {
                return RedirectToAction("EnrollJob", "JCDashboard");
            }

            job.JobOwnerId = Int32.Parse(SessionPersister.UserId.ToString());
            job.EnrollmentKey = null;

            

            if (ModelState.IsValid)
            {
                db.Jobs.Add(job);
                JobCorp jc = new JobCorp();
                jc.JobId = job.Id;
                jc.UserId = Int32.Parse(Session["userId"].ToString());
                jc.RoleTitle = "صاحب کار";
                db.JobCorps.Add(jc);
                db.SaveChanges();
                return RedirectToAction("AddWorkingTime", "JCDashboard");
            }
            
            //ViewBag.jt = db.JobTypes.ToList();
            ViewBag.jt = new SelectList(db.JobTypes, "Id", "Title", job.JobTypeId);
            ViewBag.cities = new SelectList(db.Cities, "Id", "Name", job.CityId);
            return View(job);
        }


        public ActionResult Edit()
        {


            if (SessionPersister.UserRole.ToString() == "User")
            {

                return RedirectToAction("Index", "Home");
            }
            else if (SessionPersister.UserRole.ToString() == "Admin")
            {

                return RedirectToAction("Index", "Admin");
            }
            else if (SessionPersister.UserRole.ToString() == "JobCorp")
            {
                return RedirectToAction("Index", "JCDashboard");
            }

            else
            {
                int uid = Int32.Parse(SessionPersister.UserId.ToString());
                Job job = db.Jobs.FirstOrDefault(acc => acc.JobOwnerId == uid);
               // Job job = jm.findByUserId(Int32.Parse(Session["userId"].ToString()));

                //return db.Jobs.Where(acc => acc.JobOwnerId.Equals(id)).FirstOrDefault();
                if (job == null)
                {
                    ViewBag.Error = "job Not Found";
                    return RedirectToAction("Index" , "JCDashboard");
                }


                ViewBag.cities = db.Cities.ToList();
                ViewBag.jt = db.JobTypes.ToList();
                return View(job);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Job job)
        {
            
            job.JobOwnerId = Int32.Parse(SessionPersister.UserId.ToString());
            //job.EnrollmentKey = null;
            bool EnrollKeyExists = db.Jobs.Any(o => o.EnrollmentKey.Equals(job.EnrollmentKey));
            
            if (!EnrollKeyExists || job.EnrollmentKey ==null)
            {

                if (ModelState.IsValid)
                {
                    db.Entry(job).State = EntityState.Modified;

                    db.SaveChanges();
                    return RedirectToAction("Index", "JCDashboard");
                }

                ViewBag.jt = new SelectList(db.JobTypes, "Id", "Title", job.JobTypeId);
                ViewBag.cities = new SelectList(db.Cities, "Id", "Name", job.CityId);
                //return View(job);

                return RedirectToAction("Index", "JCDashboard");


            }
            else
            {

                ViewBag.Error = "کد همکاران قبلا توسط بقیه وارد شده است. کد دیگری وارد کتید";

                ViewBag.cities = db.Cities.ToList();
                ViewBag.jt = db.JobTypes.ToList();
                return View();
            }
        }


        public ActionResult JobCorpsList()
        {


            if (SessionPersister.UserRole.ToString() == "User")
            {

                return RedirectToAction("Index", "Home");
            }
            else if (SessionPersister.UserRole.ToString() == "Admin")
            {

                return RedirectToAction("Index", "Admin");
            }
            else if (SessionPersister.UserRole.ToString() == "JobCorp")
            {
                return RedirectToAction("Index", "JCDashboard");
            }

            else
            {

                //User user = um.findById(SessionPersister.UserId);

                //Job job = jm.findByUserId(Int32.Parse(Session["userId"].ToString()));

                //ViewBag.JobCorps = db.JobCorps.ToList();

                int uid = Int32.Parse(SessionPersister.UserId.ToString());
                Job job = db.Jobs.FirstOrDefault(acc => acc.JobOwnerId == uid);
                //Job job = jm.findByUserId(Int32.Parse(Session["userId"].ToString()));
                ViewBag.JobCorps = db.JobCorps.Where(model => model.JobId.Equals(job.Id)).ToList();
                //var jobs = db.Jobs.Where(model => model.JobOwnerId.Equals(SessionPersister.UserId)).FirstOrDefault();

                //if (job == null)
                //{
                //    ViewBag.Error = "job Not Found";
                //    return RedirectToAction("Index", "JCDashboard");
                //}


                return View();
                //check this user's job id and redirect that user to that edit
            }

        }

        // POST: Jobs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult JobCorpsList(int? id)
        {

            if (id.HasValue)
            {
                return View("DeleteJobCorp", id);
            }
            else
            {
                return RedirectToAction("Index","JCDashboard");
            }
        }
        public ActionResult DeleteJobCorp(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JobCorp jc = db.JobCorps.Find(id);
            if (jc == null)
            {
                return HttpNotFound();
            }
            return View(jc);
        }

        [HttpPost, ActionName("DeleteJobCorp")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteJobCorpConfirmed(int id)
        {
            JobCorp jc = db.JobCorps.Find(id);
            jc.User.UserRoleId = 1;
            db.JobCorps.Remove(jc);
            db.SaveChanges();
            return RedirectToAction("Index","JCDashboard");
        }


        // GET: Jobs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (SessionPersister.UserRole.ToString() == "User" || SessionPersister.UserRole.ToString() == "JobCorp")
            {
                return RedirectToAction("Index", "Home");
            }
            JobCorp jc = db.JobCorps.Find(id);
            if (jc == null)
            {
                return HttpNotFound();
            }
            return View(jc);
        }

        // POST: Jobs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Job job = db.Jobs.Find(id);
            db.Jobs.Remove(job);
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
