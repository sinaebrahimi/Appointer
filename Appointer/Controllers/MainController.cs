using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Appointer.Models;
using System.Data;
using System.Data.Entity;
using System.Net;
using Appointer.Security;
using Newtonsoft.Json;

namespace Appointer.Controllers
{
    public class MainController : Controller
    {

        private AppointerEntities db = new AppointerEntities();

        // GET: Main
        public ActionResult Index()
        {
            //search through jobs

            ViewBag.cities = db.Cities.ToList();
            ViewBag.jt = db.JobTypes.ToList();

            var jobs = db.Jobs.ToList();
            return View(jobs.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string title, string city, string jobtype)
        {

            IEnumerable<Job> query = db.Jobs.ToList();

            query = db.Jobs.Where(m => (string.IsNullOrEmpty(title) ? true : m.Title.Contains(title)) && (string.IsNullOrEmpty(city) ? true : m.City.Name == city) && (string.IsNullOrEmpty(jobtype) ? true : m.JobType.Title == jobtype));
            if (query == null)
            {

                ViewBag.cities = db.Cities.ToList();
                ViewBag.jt = db.JobTypes.ToList();
                var jobs = db.Jobs.ToList();
                return View(jobs.ToList());
            }
            ViewBag.cities = db.Cities.ToList();
            ViewBag.jt = db.JobTypes.ToList();
            var jb = query.ToList();
            return View(jb.ToList());

        }

        //public ActionResult Jobs()
        //{
        //    //search through jobs

        //    ViewBag.cities = db.Cities.ToList();
        //    ViewBag.jt = db.JobTypes.ToList();

        //    var jobs = db.Jobs.ToList();
        //    return View(jobs.ToList());
        //}
        [HttpGet]
        public ActionResult Jobs(string jobtype)
        {

            IEnumerable<Job> query = db.Jobs.ToList();

            query = db.Jobs.Where(m => (string.IsNullOrEmpty(jobtype) ? true : m.JobType.Title == jobtype));
            if (query == null)
            {

                ViewBag.cities = db.Cities.ToList();
                ViewBag.jt = db.JobTypes.ToList();
                var jobs = db.Jobs.ToList();
                return View(jobs.ToList());
            }
            ViewBag.cities = db.Cities.ToList();
            ViewBag.jt = db.JobTypes.ToList();
            var jb = query.ToList();
            return View(jb.ToList());

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Jobs(string title, string city, string jobtype)
        {

            IEnumerable<Job> query = db.Jobs.ToList();

            query = db.Jobs.Where(m => (string.IsNullOrEmpty(title) ? true : m.Title.Contains(title)) && (string.IsNullOrEmpty(city) ? true : m.City.Name == city) && (string.IsNullOrEmpty(jobtype) ? true : m.JobType.Title == jobtype));
            if (query == null)
            {

                ViewBag.cities = db.Cities.ToList();
                ViewBag.jt = db.JobTypes.ToList();
                var jobs = db.Jobs.ToList();
                return View(jobs.ToList());
            }
            ViewBag.cities = db.Cities.ToList();
            ViewBag.jt = db.JobTypes.ToList();
            var jb = query.ToList();
            return View(jb.ToList());

        }


        public ActionResult JobDetails(int? id)
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

            SessionPersister.JobId = job.Id;

            return View(job);
        }

        public ActionResult Reservations()
        {
            User user = db.Users.Find(SessionPersister.UserId);

            if (user == null)
            {
                return RedirectToAction("SignIn", "Account");
            }
            DateTime dt = DateTime.Now.AddDays(2);
            var Appointments = db.Appointments.Where(a => a.UserId == user.Id && a.StartTime >= DateTime.Now && a.StartTime <= dt).ToList();
            return View(Appointments.ToList());

        }

        public ActionResult ExpiredReservations()
        {
            User user = db.Users.Find(SessionPersister.UserId);

            if (user == null)
            {
                return RedirectToAction("SignIn", "Account");
            }
            //DateTime dt = DateTime.Now.AddDays(2);
            //&& a.StartTime >= DateTime.Now  && a.StartTime <= dt
            //var jobs = db.Jobs.Include(j => j.User).Include(j => j.JobType);
            var Appointments = db.Appointments.Where(a => a.UserId == user.Id && a.StartTime <= DateTime.Now).ToList();
            return View(Appointments.ToList());

        }

        public ActionResult FutureReservations()
        {
            User user = db.Users.Find(SessionPersister.UserId);

            if (user == null)
            {
                return RedirectToAction("SignIn", "Account");
            }
            //DateTime dt = DateTime.Now.AddDays(2);
            //&& a.StartTime >= DateTime.Now  && a.StartTime <= dt
            //var jobs = db.Jobs.Include(j => j.User).Include(j => j.JobType);
            var Appointments = db.Appointments.Where(a => a.UserId == user.Id && a.StartTime >= DateTime.Now).ToList();
            return View(Appointments.ToList());

        }
        public ActionResult ReserveDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment ap = db.Appointments.Find(id);
            if (ap == null)
            {
                return HttpNotFound();
            }

            return View(ap);
        }


        public ActionResult ReserveEdit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment ap = db.Appointments.Find(id);
            if (ap == null)
            {
                return HttpNotFound();
            }

            Service s = db.Services.Find(ap.ServiceId);
            ViewBag.WorkingTimes = db.WorkingTimes.Where(acc => (acc.JobCorpId == s.JobCorpId) && (acc.StartTime > DateTime.Now)).ToList();
            ViewBag.Services = db.Services.Where(acc => acc.JobCorpId == s.JobCorpId).ToList();
            return View();



        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReserveEdit(Appointment ap)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("SignIn", "Account");
            }
            User u = db.Users.Find(SessionPersister.UserId);
            ap.UserId = u.Id;
            ap.BookDate = DateTime.Now;
            ap.isReserved = true;
            TimeSpan ts = TimeSpan.Parse(ap.start);
            ap.StartTime = ap.myDate.Date + ts;
            Service s = db.Services.Find(ap.ServiceId);
            ap.EndTime = ap.StartTime;
            ap.EndTime = ap.EndTime.AddMinutes(s.Duration);


            if (ModelState.IsValid)
            {
                var isInWorkingTimes = db.WorkingTimes.Any(p => (p.StartTime <= ap.StartTime) && (p.EndTime >= ap.EndTime) && (ap.StartTime >= DateTime.Now) && (p.JobCorpId == s.JobCorpId));//&& (ap.Service.JobCorpId == s.JobCorpId)
                                                                                                                                                                                              //to check isInOtherAppointments


                //var isInOtherAppointments = db.Appointments.Any(m=> (ap.StartTime >= DateTime.Now) && (m.Service.JobCorpId == s.JobCorpId) && (m.StartTime >= ap.EndTime) && (m.EndTime <= ap.StartTime));
                var isInOtherAppointments = db.Appointments.Any(m => (ap.StartTime >= DateTime.Now) && (m.Service.JobCorpId == s.JobCorpId) && (m.StartTime < ap.EndTime) && (m.EndTime > ap.StartTime));

                if (!isInWorkingTimes)
                {

                    ViewBag.Error = "زمان انتخابی در لیست زمان های همکار انتخابی نمی باشد";
                    ViewBag.WorkingTimes = db.WorkingTimes.Where(acc => (acc.JobCorpId == s.JobCorpId) && (acc.StartTime >= DateTime.Now)).ToList();
                    ViewBag.Services = db.Services.Where(acc => acc.JobCorpId == s.JobCorpId).ToList();
                    return View(ap);
                }

                if (isInOtherAppointments)
                {
                    ViewBag.Error = "زمان انتخابی با رزروهای بقیه کاربران تداخل دارد";
                    ViewBag.WorkingTimes = db.WorkingTimes.Where(acc => (acc.JobCorpId == s.JobCorpId) && (acc.StartTime >= DateTime.Now)).ToList();
                    ViewBag.Services = db.Services.Where(acc => acc.JobCorpId == s.JobCorpId).ToList();
                    return View(ap);
                }
                db.Entry(ap).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Main");
            }


            ViewBag.Error = "لطفا تمام مقادیر را وارد کنید";
            ViewBag.WorkingTimes = db.WorkingTimes.Where(acc => (acc.JobCorpId == s.JobCorpId) && (acc.StartTime > DateTime.Now)).ToList();
            ViewBag.Services = db.Services.Where(acc => acc.JobCorpId == ap.Service.JobCorpId).ToList(); ;
            return View(ap);


        }

        public ActionResult ReserveCancellation(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment ap = db.Appointments.Find(id);
            if (ap == null)
            {
                return HttpNotFound();
            }
            return View(ap);
        }

        [HttpPost, ActionName("ReserveCancellation")]
        [ValidateAntiForgeryToken]
        public ActionResult ReserveCancellationConfirmed(int id)
        {
            Appointment ap = db.Appointments.Find(id);
            ap.isReserved = false;
            if (ModelState.IsValid)
            {
                //db.Appointments.Remove(ap);
                db.Entry(ap).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Main");
            }

            return RedirectToAction("Index", "Main");
        }



        public ActionResult JobCorpsList(int? id)//JobCorp Id
        {
            ViewBag.JobCorps = db.JobCorps.Where(a => a.JobId == id).ToList();
            return View();

        }

        [HttpGet]
        public ActionResult Reserve(int? id)//JobCorp Id
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("SignIn", "Account");
            }

            ViewBag.WorkingTimes = db.WorkingTimes.Where(acc => (acc.JobCorpId == id) && (acc.StartTime > DateTime.Now)).OrderBy(a => a.StartTime).ToList();
            ViewBag.Services = db.Services.Where(acc => acc.JobCorpId == id).ToList();
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reserve(Appointment ap)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("SignIn", "Account");
            }
            
            User u = db.Users.Find(SessionPersister.UserId);
            ap.UserId = u.Id;
            ap.BookDate = DateTime.Now;
            ap.isReserved = true;
            TimeSpan ts = TimeSpan.Parse(ap.start);
            ap.StartTime = ap.myDate.Date + ts;
            Service s = db.Services.Find(ap.ServiceId);
            ap.EndTime = ap.StartTime;
            ap.EndTime = ap.EndTime.AddMinutes(s.Duration);
            
            if (ModelState.IsValid)
            {

                var isInWorkingTimes = db.WorkingTimes.Any(p => (p.StartTime <= ap.StartTime) && (p.EndTime >= ap.EndTime) && (ap.StartTime >= DateTime.Now) && (p.JobCorpId == s.JobCorpId));
                var isInOtherAppointments = db.Appointments.Any(m => (ap.StartTime >= DateTime.Now) && (m.Service.JobCorpId == s.JobCorpId) && (m.StartTime < ap.EndTime) && (m.EndTime > ap.StartTime));

                if (!isInWorkingTimes)
                {
                    ViewBag.Error = "زمان انتخابی در لیست زمان های همکار انتخابی نمی باشد";
                    ViewBag.WorkingTimes = db.WorkingTimes.Where(acc => (acc.JobCorpId == s.JobCorpId) && (acc.StartTime >= DateTime.Now)).ToList();
                    ViewBag.Services = db.Services.Where(acc => acc.JobCorpId == s.JobCorpId).ToList();
                    return View(ap);
                }

                if (isInOtherAppointments)
                {
                    ViewBag.Error = "زمان انتخابی با رزروهای بقیه کاربران تداخل دارد";
                    ViewBag.WorkingTimes = db.WorkingTimes.Where(acc => (acc.JobCorpId == s.JobCorpId) && (acc.StartTime >= DateTime.Now)).ToList();
                    ViewBag.Services = db.Services.Where(acc => acc.JobCorpId == s.JobCorpId).ToList();
                    return View(ap);
                }
                db.Appointments.Add(ap);
                db.SaveChanges();
                return RedirectToAction("Reservations", "Main");
            }
            //not sure if it works

            //ViewBag.WorkingTimes = db.WorkingTimes.Where(acc => (acc.JobCorpId.Equals(id)) && (acc.StartTime > DateTime.Now)).ToList();

            ViewBag.Error = "زمان انتخابی در لیست زمان های همکار انتخابی نمی باشد";

            ViewBag.WorkingTimes = db.WorkingTimes.Where(acc => (acc.JobCorpId == s.JobCorpId) && (acc.StartTime > DateTime.Now)).OrderBy(a => a.StartTime).ToList();
            ViewBag.Services = db.Services.Where(acc => acc.JobCorpId == s.JobCorpId).ToList();
            return View(ap);

            //return View(ap);
        }


        public JsonResult ReserveJsonInfo(DateTime wtdate)
        {
            JobCorp jc = new JobCorp();
            var u = Int32.Parse(SessionPersister.UserId.ToString());
            jc = db.JobCorps.Where(acc => acc.UserId.Equals(u)).FirstOrDefault();
            //DateTime wtimedate = wtdate.
            List<WorkingTime> wt;
            wt = db.WorkingTimes.Where(a => a.StartTime.Date == wtdate.Date && a.JobCorpId == jc.Id).ToList();
            //var isInWorkingTimes = db.WorkingTimes.Where(p => (p.StartTime <= ap.StartTime) && (p.EndTime >= ap.EndTime) && (ap.StartTime >= DateTime.Now) && (p.JobCorpId == s.JobCorpId));
            //ViewBag.Appointments = db.Appointments.Where(acc => (acc.Service.JobCorpId == jc.Id) && (acc.StartTime > DateTime.Now) && (acc.StartTime >= wt.StartTime) && (acc.EndTime <= wt.EndTime)).ToList();
            if (wt == null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                LittleWorkingTime lwt = new LittleWorkingTime();

                //lwt.wt = db.WorkingTimes.Where(a => a.StartTime.Date == wtdate.Date && a.JobCorpId == jc.Id).ToList();

                lwt.wt = (from a in db.Appointments
                          from w in db.WorkingTimes
                          where w.StartTime.Date == wtdate.Date && w.JobCorpId == jc.Id
                          select new WTItem { StartTime = a.StartTime, EndTime = a.EndTime }).ToList();

                lwt.StartDate = lwt.wt[0].StartTime.ToPersianDateTime().ToString("yyyy/MM/dd");
                //lwt.StartTime = wt.StartTime.ToString("HH:mm");
                //lwt.EndTime = wt.EndTime.ToString("HH:mm");
                lwt.JobTitle = wt[0].JobCorp.Job.Title;
                lwt.JobCorp = wt[0].JobCorp.User.FullName;

                lwt.ap = (from s in db.Services
                          from a in db.Appointments
                          from w in db.WorkingTimes
                          where w.StartTime <= a.StartTime && w.EndTime >= a.EndTime && w.JobCorpId == s.JobCorpId && s.Id == a.ServiceId
                          select new Item { StartTime = a.StartTime, EndTime = a.EndTime }).ToList();

                string myday = wt[0].StartTime.DayOfWeek.ToString();
                for (int i = 0; i < lwt.wt.Count; i++)
                {
                    lwt.wt[i].StartHour = lwt.wt[i].StartTime.ToString("HH:mm");
                    lwt.wt[i].EndHour = lwt.wt[i].EndTime.ToString("HH:mm");
                }

                for (int i = 0; i < lwt.ap.Count; i++)
                {
                    lwt.ap[i].StartHour = lwt.ap[i].StartTime.ToString("HH:mm");
                    lwt.ap[i].EndHour = lwt.ap[i].EndTime.ToString("HH:mm");
                }

                switch (myday)
                {
                    case "Sunday":
                        lwt.dow = "یکشنبه";
                        break;

                    case "Monday":
                        lwt.dow = "دوشنبه";
                        break;

                    case "Tuesday":
                        lwt.dow = "سه‌شنبه";
                        break;

                    case "Wednesday":
                        lwt.dow = "چهارشنبه";
                        break;

                    case "Thursday":
                        lwt.dow = "پنج‌شنبه";
                        break;

                    case "Friday":
                        lwt.dow = "جمعه";
                        break;

                    case "Saturday":
                        lwt.dow = "شنبه";
                        break;

                    default:
                        lwt.dow = "روز هفته";
                        break;
                }

                return Json(lwt, JsonRequestBehavior.AllowGet);

            }
        }

        [HttpGet]
        public ActionResult ChooseReserve(int? id)//JobCorp Id
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("SignIn", "Account");
            }

            //ViewBag.WTChoose = db.WorkingTimes.Where(acc => (acc.JobCorpId == id) && (acc.StartTime > DateTime.Now)).ToList();
            ViewBag.WorkingTimes = db.WorkingTimes.Where(acc => (acc.JobCorpId == id) && (acc.StartTime > DateTime.Now)).ToList();
            ViewBag.Services = db.Services.Where(acc => acc.JobCorpId == id).ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChooseReserve(Appointment ap, string selDate)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("SignIn", "Account");
            }

            User u = db.Users.Find(SessionPersister.UserId);
            ap.UserId = u.Id;

            Service s = db.Services.Find(ap.ServiceId);
            ap.BookDate = DateTime.Now;
            ap.isReserved = true;
            if(selDate == "notset")
            {
                ViewBag.Error = "لططفا یک تاریخ را انتخاب کنید";
                ViewBag.WorkingTimes = db.WorkingTimes.Where(acc => (acc.JobCorpId == s.JobCorpId) && (acc.StartTime >= DateTime.Now)).ToList();
                ViewBag.Services = db.Services.Where(acc => acc.JobCorpId == s.JobCorpId).ToList();
                return View(ap);
            }
            int sdid = Int32.Parse(selDate.ToString());
            WorkingTime dt = db.WorkingTimes.Find(sdid);
            DateTime d = dt.StartTime.Date;
            TimeSpan ts = TimeSpan.Parse(ap.start);
            ap.StartTime = d.Date + ts;
            ap.EndTime = ap.StartTime;
            ap.EndTime = ap.EndTime.AddMinutes(s.Duration);

            if (ModelState.IsValid)
            {

                var isInWorkingTimes = db.WorkingTimes.Any(p => (p.StartTime <= ap.StartTime) && (p.EndTime >= ap.EndTime) && (ap.StartTime >= DateTime.Now) && (p.JobCorpId == s.JobCorpId));
                var isInOtherAppointments = db.Appointments.Any(m => (ap.StartTime >= DateTime.Now) && (m.Service.JobCorpId == s.JobCorpId) && (m.StartTime < ap.EndTime) && (m.EndTime > ap.StartTime));

                if (!isInWorkingTimes)
                {
                    ViewBag.Error = "زمان انتخابی در لیست زمان های همکار انتخابی نمی باشد";
                    ViewBag.WorkingTimes = db.WorkingTimes.Where(acc => (acc.JobCorpId == s.JobCorpId) && (acc.StartTime >= DateTime.Now)).ToList();
                    ViewBag.Services = db.Services.Where(acc => acc.JobCorpId == s.JobCorpId).ToList();
                    return View(ap);
                }

                if (isInOtherAppointments)
                {
                    ViewBag.Error = "زمان انتخابی با رزروهای بقیه کاربران تداخل دارد";
                    ViewBag.WorkingTimes = db.WorkingTimes.Where(acc => (acc.JobCorpId == s.JobCorpId) && (acc.StartTime >= DateTime.Now)).ToList();
                    ViewBag.Services = db.Services.Where(acc => acc.JobCorpId == s.JobCorpId).ToList();
                    return View(ap);
                }
                db.Appointments.Add(ap);
                db.SaveChanges();
                return RedirectToAction("Reservations", "Main");
            }
            //not sure if it works

            //ViewBag.WorkingTimes = db.WorkingTimes.Where(acc => (acc.JobCorpId.Equals(id)) && (acc.StartTime > DateTime.Now)).ToList();

            ViewBag.Error = "زمان انتخابی در لیست زمان های همکار انتخابی نمی باشد";

            ViewBag.WorkingTimes = db.WorkingTimes.Where(acc => (acc.JobCorpId == s.JobCorpId) && (acc.StartTime > DateTime.Now)).ToList();
            ViewBag.Services = db.Services.Where(acc => acc.JobCorpId == s.JobCorpId).ToList();
            return View(ap);

            //return View(ap);
        }

        public JsonResult JsonInfo(string wtid)
        {

            //i want to add a list of reservations on that day that i can show to user with this json
            int WorkingTimeId = Int32.Parse(wtid);
            JobCorp jc = new JobCorp();
            var u = Int32.Parse(SessionPersister.UserId.ToString());
            jc = db.JobCorps.Where(acc => acc.UserId.Equals(u)).FirstOrDefault();
            WorkingTime wt = db.WorkingTimes.FirstOrDefault(acc => acc.Id == WorkingTimeId);

            //var isInWorkingTimes = db.WorkingTimes.Where(p => (p.StartTime <= ap.StartTime) && (p.EndTime >= ap.EndTime) && (ap.StartTime >= DateTime.Now) && (p.JobCorpId == s.JobCorpId));
            //ViewBag.Appointments = db.Appointments.Where(acc => (acc.Service.JobCorpId == jc.Id) && (acc.StartTime > DateTime.Now) && (acc.StartTime >= wt.StartTime) && (acc.EndTime <= wt.EndTime)).ToList();
            if (wt == null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                LittleWorkingTime lwt = new LittleWorkingTime();
                lwt.StartDate = wt.StartTime.ToPersianDateTime().ToString("yyyy/MM/dd");
                lwt.StartTime = wt.StartTime.ToString("HH:mm");
                lwt.EndTime = wt.EndTime.ToString("HH:mm");
                lwt.JobTitle = wt.JobCorp.Job.Title;
                lwt.JobCorp = wt.JobCorp.User.FullName;
                
                lwt.ap = (from s in db.Services
                          from a in db.Appointments
                          from w in db.WorkingTimes
                          where w.Id == wt.Id && w.StartTime <= a.StartTime && w.EndTime >= a.EndTime && w.JobCorpId == s.JobCorpId && s.Id == a.ServiceId
                          select new Item{ StartTime = a.StartTime, EndTime = a.EndTime }).ToList();
                
                string myday = wt.StartTime.DayOfWeek.ToString();
                for(int i=0; i<lwt.ap.Count; i++)
                {
                    lwt.ap[i].StartHour = lwt.ap[i].StartTime.ToString("HH:mm");
                    lwt.ap[i].EndHour = lwt.ap[i].EndTime.ToString("HH:mm");
                }

                switch (myday)
                {
                    case "Sunday":
                        lwt.dow = "یکشنبه";
                        break;

                    case "Monday":
                        lwt.dow = "دوشنبه";
                        break;

                    case "Tuesday":
                        lwt.dow = "سه‌شنبه";
                        break;

                    case "Wednesday":
                        lwt.dow = "چهارشنبه";
                        break;

                    case "Thursday":
                        lwt.dow = "پنج‌شنبه";
                        break;

                    case "Friday":
                        lwt.dow = "جمعه";
                        break;

                    case "Saturday":
                        lwt.dow = "شنبه";
                        break;

                    default:
                        lwt.dow = "روز هفته";
                        break;
                }

                return Json(lwt, JsonRequestBehavior.AllowGet);

            }


        }


    }
}