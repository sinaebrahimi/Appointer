using Appointer.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Globalization;
using Appointer.Security;
using System.Net;

namespace Appointer.Controllers
{
    public class JCDashboardController : Controller
    {


        private AppointerEntities db = new AppointerEntities();

        private JobModel jm = new JobModel();
        // GET: JCDashboard
        public ActionResult Index()
        {

            if (SessionPersister.UserRole == "JobCorp" || SessionPersister.UserRole == "JobOwner")
            {
                User user = db.Users.Find(SessionPersister.UserId);
                DateTime dt = DateTime.Now.AddDays(2);
                var Appointments = db.Appointments.Where(a => a.Service.JobCorp.UserId == user.Id && a.StartTime >= DateTime.Now  && a.StartTime <= dt).ToList();
                return View(Appointments.ToList());
            }

            else
            {
                if (SessionPersister.UserId.ToString() == null)
                {

                    return RedirectToAction("SignIn", "Account");
                }
                return RedirectToAction("Index", "Main");
            }

            //return View();
        }




        public JsonResult JsonCheck(string ekey)
        {

            Job job = db.Jobs.FirstOrDefault(acc => acc.EnrollmentKey == ekey);
            if (job == null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                LittleJob lj = new LittleJob();
                lj.JobOwner = job.User.FullName;
                lj.JobPhone = job.JobPhone;
                lj.Title = job.Title;
                lj.Address = job.Address;
                lj.City = job.City.Name;

                return Json(lj, JsonRequestBehavior.AllowGet);

            }


        }

        [HttpGet]
        public ActionResult EnrollJob()
        {

            if (Session["userRole"].ToString() == "Admin")
            {

                return RedirectToAction("Index", "Admin");
            }
            else if (Session["userRole"].ToString() == "JobOwner")
            {

                return RedirectToAction("Index", "JCDashboard");
            }
            else if (Session["userRole"].ToString() == "JobCorp")
            {

                return View();
            }

            else
            {

                return RedirectToAction("Index", "Home");
            }

        }

        [HttpPost]
        public ActionResult EnrollJob(string EnrollmentKeyValue)
        {
            ////Job job = new Job();
            ////job = jm.Enroll(job.EnrollmentKey);
            Job job = db.Jobs.FirstOrDefault(acc => acc.EnrollmentKey == EnrollmentKeyValue);
            if (job == null)
            {
                ViewBag.Error = "شغلی با کد ورودی شما یافت نشد";
                return View();
            }
            else
            {
                JobCorp jc = new JobCorp();
                jc.RoleTitle = "همکار";//we can define a textbox too
                jc.UserId = Int32.Parse(Session["userId"].ToString());
                jc.JobId = job.Id;

                if (ModelState.IsValid)
                {

                    db.JobCorps.Add(jc);
                    db.SaveChanges();

                    return RedirectToAction("AddWorkingTime", "JCDashboard");
                }
                ViewBag.Error = "سیستم قادر به ذخیره تغییرات نشد";
                return View();
            }
        }



        [HttpGet]
        public ActionResult AddWorkingDate()
        {

            if (Session["userRole"].ToString() == "Admin")
            {

                return RedirectToAction("Index", "Admin");
            }

            else if (Session["userRole"].ToString() == "JobCorp" || Session["userRole"].ToString() == "JobOwner")
            {

                JobCorp jc = new JobCorp();
                int u = SessionPersister.UserId;
                jc = db.JobCorps.Where(acc => acc.UserId.Equals(u)).FirstOrDefault();
                ViewBag.WorkingTimes = db.WorkingTimes.Where(acc => (acc.JobCorpId == jc.Id) && (acc.StartTime > DateTime.Now)).OrderBy(p => p.StartTime).ToList();
                return View();
                //check this user's job id and redirect that user to that edit
            }

            else
            {

                return RedirectToAction("Index", "Main");
            }

        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult AddWorkingDate(WorkingTime wd)
        {
            JobCorp jc = new JobCorp();
            var u = Int32.Parse(Session["userId"].ToString());
            jc = db.JobCorps.Where(acc => acc.UserId.Equals(u)).FirstOrDefault();
            wd.JobCorpId = jc.Id;

            TimeSpan ts = TimeSpan.Parse(wd.start);
            wd.StartTime = wd.myDate.Date + ts;

            TimeSpan es = TimeSpan.Parse(wd.end);

            wd.EndTime = wd.myDate.Date + es;

            if (ModelState.IsValid)

            {

                //var isInOtherWorkingTimes = db.WorkingTimes.Any(m =>  (m.StartTime > wd.EndTime) && (m.EndTime < wd.StartTime) &&  (wd.StartTime >= DateTime.Now) && (m.JobCorpId == jc.Id));
                //var isInOtherWorkingTimes = db.WorkingTimes.Any(m => (wd.StartTime >= DateTime.Now) && (m.JobCorpId == jc.Id) && (m.StartTime < wd.EndTime) && (m.EndTime > wd.StartTime));
                //var notInOtherWorkingTimes = db.WorkingTimes.Any(m => (wd.StartTime >= DateTime.Now) && (m.JobCorpId == jc.Id) && (m.StartTime < wd.EndTime) && (m.EndTime > wd.StartTime));
                var isInWorkingTimes = db.WorkingTimes.Any(p => (p.StartTime <= wd.StartTime) && (p.EndTime >= wd.EndTime) && (wd.StartTime >= DateTime.Now) && (p.JobCorpId == wd.JobCorpId));
                //still have bugs
                //the bug is it saves any time but the times who are compeletely in some working time... if just a part of it is in some other working time it will save that anyway
                if (isInWorkingTimes)
                //if (isInOtherWorkingTimes)
                {

                    ViewBag.WorkingTimes = db.WorkingTimes.Where(acc => (acc.JobCorpId == jc.Id) && (acc.StartTime > DateTime.Now)).OrderBy(p => p.StartTime).ToList();
                    ViewBag.Error = "زمان انتخابی با بقیه روزهای کاری شما تداخل دارد";
                    return View();
                }

                db.WorkingTimes.Add(wd);
                db.SaveChanges();

                var hasAppointments = db.Appointments.Any(p => p.Service.JobCorp.UserId == SessionPersister.UserId);
                var hasServices = db.Services.Any(p => p.JobCorp.UserId == SessionPersister.UserId);
                if (!hasServices)
                {
                    return RedirectToAction("Create", "Services");
                }

                if (hasAppointments)
                {
                    return RedirectToAction("AppointmentList", "JCDashboard");
                }
                return RedirectToAction("Index", "JCDashboard");
            }

            ViewBag.WorkingTimes = db.WorkingTimes.Where(acc => (acc.JobCorpId == jc.Id) && (acc.StartTime > DateTime.Now)).OrderBy(p => p.StartTime).ToList();
            ViewBag.Error = "سیستم موفق به ذخیره زمان های کاری شما نشد";
            return View();
        }





        [HttpGet]
        public ActionResult AddWorkingTime()
        {

            if (Session["userRole"].ToString() == "Admin")
            {

                return RedirectToAction("Index", "Admin");
            }

            else if (Session["userRole"].ToString() == "JobCorp" || Session["userRole"].ToString() == "JobOwner")
            {

                JobCorp jc = new JobCorp();
                int u = SessionPersister.UserId;
                jc = db.JobCorps.Where(acc => acc.UserId.Equals(u)).FirstOrDefault();
                ViewBag.WorkingTimes = db.WorkingTimes.Where(acc => (acc.JobCorpId == jc.Id) && (acc.StartTime > DateTime.Now)).OrderBy(a => a.StartTime).ToList();
                return View();
                //check this user's job id and redirect that user to that edit
            }

            else
            {

                return RedirectToAction("Index", "Home");
            }

        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult AddWorkingTime(WorkingTime wd, int[] WeekDayId)
        {

            JobCorp jc = new JobCorp();
            int u = SessionPersister.UserId;
            jc = db.JobCorps.Where(acc => acc.UserId.Equals(u)).FirstOrDefault();
            wd.JobCorpId = jc.Id;

            //TimeSpan ts = TimeSpan.Parse(wd.start);

            //TimeSpan es = TimeSpan.Parse(wd.end);

            var stimelist = new List<HourMinute>();
            var etimelist = new List<HourMinute>();
            var t = new DateTime();
            var list = new List<WorkingTime>();
            //TimeSpan ts = TimeSpan.Parse(wd.Range[0].StartHour);
            int DoW;
            TimeSpan ts = TimeSpan.Parse(wd.Range[0].StartHour);
            for (int i = 0; i < 31; i++) //saves the dates until 1 month later, I think it works but it needs to be verified
            {
                //need to verify the time that is in the list , if it's already in working times we should remove it from the list
                //var isInWorkingTimes = db.WorkingTimes.Any(p => (p.StartTime <= wd.StartTime) && (p.EndTime >= wd.EndTime) && (wd.StartTime >= DateTime.Now) && (p.JobCorpId == wd.JobCorpId));
                t = DateTime.Now;
                t = t.AddDays(i);
                DoW = (int)t.DayOfWeek;
                if (WeekDayId.Contains(DoW))
                {
                    if (i == 0)
                    {
                        if (ts <= DateTime.Now.TimeOfDay)
                        {

                            t = t.AddDays(7);

                            for (int j = 0; j < wd.Range.Count; j++)
                            {
                                stimelist.Add(new HourMinute
                                {
                                    hm = TimeSpan.Parse(wd.Range[j].StartHour)
                                });
                                etimelist.Add(new HourMinute
                                {
                                    hm = TimeSpan.Parse(wd.Range[j].EndHour)
                                });
                                //stimelist[j] = TimeSpan.Parse(wd.Range[j].StartHour);
                                //etimelist[j] = TimeSpan.Parse(wd.Range[j].EndHour);
                                list.Add(new WorkingTime
                                {
                                    JobCorpId = jc.Id,
                                    StartTime = t.Date + stimelist[j].hm,
                                    EndTime = t.Date + etimelist[j].hm,
                                });

                            }
                            continue;
                        }
                        else //when the start hour is valid
                        {

                            for (int j = 0; j < wd.Range.Count; j++)
                            {

                                stimelist.Add(new HourMinute
                                {
                                    hm = TimeSpan.Parse(wd.Range[j].StartHour)
                                });
                                etimelist.Add(new HourMinute
                                {
                                    hm = TimeSpan.Parse(wd.Range[j].EndHour)
                                });
                                list.Add(new WorkingTime
                                {
                                    JobCorpId = jc.Id,
                                    StartTime = t.Date + stimelist[j].hm,
                                    EndTime = t.Date + etimelist[j].hm
                                });

                            }

                        }
                        
                    }
                    if ((i == 7) && (ts <= DateTime.Now.TimeOfDay))
                    {
                        continue;
                    }
                    for(int j=0; j<wd.Range.Count; j++)
                    {

                        stimelist.Add(new HourMinute
                        {
                            hm = TimeSpan.Parse(wd.Range[j].StartHour)
                        });
                        etimelist.Add(new HourMinute
                        {
                            hm = TimeSpan.Parse(wd.Range[j].EndHour)
                        });
                        list.Add(new WorkingTime
                        {
                            JobCorpId = jc.Id,
                            StartTime = t.Date + stimelist[j].hm,
                            EndTime = t.Date + etimelist[j].hm
                        });

                    }

                }
            }

            if (ModelState.IsValid)
            {
                //var isInOtherWorkingTimes = db.WorkingTimes.Any(m => (wd.StartTime >= DateTime.Now) && (m.JobCorpId == jc.Id) && (m.StartTime > wd.EndTime) && (m.EndTime < wd.StartTime));
                //if (!isInOtherWorkingTimes)
                //{


                //}
                db.WorkingTimes.AddRange(list);
                db.SaveChanges();
                var hasAppointments = db.Appointments.Any(p => p.Service.JobCorp.UserId == SessionPersister.UserId);
                var hasServices = db.Services.Any(p => p.JobCorp.UserId == SessionPersister.UserId);
                if (!hasServices)
                {
                    return RedirectToAction("Create", "Services");
                }

                if (hasAppointments)
                {
                    return RedirectToAction("AppointmentList", "JCDashboard");
                }
                return RedirectToAction("Index", "JCDashboard");
            }
            ViewBag.WorkingTimes = db.WorkingTimes.Where(acc => (acc.JobCorpId == jc.Id) && (acc.StartTime > DateTime.Now)).OrderBy(a=>a.StartTime).ToList();
            ViewBag.Error = "سیستم موفق به ذخیره زمان های کاری شما نشد";
            return View();
            //}
        }



        public ActionResult AppointmentList()
        {
            if (SessionPersister.UserRole == "JobCorp" || SessionPersister.UserRole == "JobOwner")
            {
                User user = db.Users.Find(SessionPersister.UserId);
                DateTime dt = DateTime.Now.AddDays(2);
                var Appointments = db.Appointments.Where(a => a.Service.JobCorp.UserId == user.Id && a.StartTime >= DateTime.Now && a.StartTime <= dt).OrderBy(a=>a.StartTime).ToList();
                return View(Appointments.ToList());
            }

            else
            {
                if (SessionPersister.UserId.ToString() == null)
                {

                    return RedirectToAction("SignIn", "Account");
                }
                return RedirectToAction("Index", "Main");
            }

        }

        public ActionResult ExpiredAppointmentList()
        {
            if (SessionPersister.UserRole == "JobCorp" || SessionPersister.UserRole == "JobOwner")
            {
                User user = db.Users.Find(SessionPersister.UserId);
                var Appointments = db.Appointments.Where(a => a.Service.JobCorp.UserId == user.Id && a.StartTime <= DateTime.Now).OrderByDescending(a => a.StartTime).ToList();
                return View(Appointments.ToList());
            }

            else
            {
                if (SessionPersister.UserId.ToString() == null)
                {

                    return RedirectToAction("SignIn", "Account");
                }
                return RedirectToAction("Index", "Main");
            }

        }

        public ActionResult FutureAppointmentList()
        {
            if (SessionPersister.UserRole == "JobCorp" || SessionPersister.UserRole == "JobOwner")
            {
                User user = db.Users.Find(SessionPersister.UserId);
                var Appointments = db.Appointments.Where(a => a.Service.JobCorp.UserId == user.Id && a.StartTime >= DateTime.Now).OrderBy(a => a.StartTime).ToList();
                return View(Appointments.ToList());
            }

            else
            {
                if (SessionPersister.UserId.ToString() == null)
                {

                    return RedirectToAction("SignIn", "Account");
                }
                return RedirectToAction("Index", "Main");
            }

        }



        public ActionResult AppointmentDetails(int? id)
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
        public ActionResult AppointmentCancellation(int? id)
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

        [HttpPost, ActionName("AppointmentCancellation")]
        [ValidateAntiForgeryToken]
        public ActionResult AppointmentCancellationConfirmed(int id)
        {
            Appointment ap = db.Appointments.Find(id);
            ap.isReserved = false;
            if (ModelState.IsValid)
            {
                //db.Appointments.Remove(ap);
                db.Entry(ap).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("AppointmentList", "JCDashboard");
            }
            ViewBag.Error = "سیستم موفق به کنسل کردن قرار شما نشد";
            return RedirectToAction("AppointmentList", "JCDashboard");
        }



        public ActionResult ModifyWorkingDate()
        {
            if (Session["userRole"].ToString() == "User" || Session["userRole"].ToString() == "Admin")
            {
                return RedirectToAction("Index", "Main");
            }

            User user = db.Users.Find(SessionPersister.UserId);
            var WorkingTimes = db.WorkingTimes.Where(a => (a.JobCorp.UserId == user.Id) && (a.StartTime >= DateTime.Now)).ToList();
            return View(WorkingTimes.ToList());
            
        }

        [HttpPost]
        public ActionResult ModifyWorkingDate(string wtid)
        {
            WorkingTime wt = new WorkingTime();
            wt = db.WorkingTimes.Find(int.Parse(wtid));

            User user = db.Users.Find(SessionPersister.UserId);
            var WorkingTimes = db.WorkingTimes.Where(a => (a.JobCorp.UserId == user.Id) && (a.StartTime >= DateTime.Now)).ToList();

            if (wt == null)
            {

                ViewBag.Info = "سیستم موفق به حذف زمان کاری شما نشد";
                return View(WorkingTimes.ToList());
            }
            db.WorkingTimes.Remove(wt);
            db.SaveChanges();
            ViewBag.Info = wt.StartTime.ToPersianDateTime().ToString("yyyy/MM/dd") + " از " + wt.StartTime.ToString("HH:mm") + " تا " + wt.EndTime.ToString("HH:mm") + " با موفقیت حذف شد";
            //return RedirectToAction("ModifyWorkingDate");
            
            WorkingTimes = db.WorkingTimes.Where(a => (a.JobCorp.UserId == user.Id) && (a.StartTime >= DateTime.Now)).ToList();
            return View(WorkingTimes.ToList());
        }

        }
    }