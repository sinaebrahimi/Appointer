using System;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Appointer.Models;
using System.Data.Entity;
using Appointer.Security;

namespace Appointer.Controllers
{
    public class AccountController : Controller
    {

        private AppointerEntities db = new AppointerEntities();
        private AccountModel um = new AccountModel();

        //[CustomAuthorize(Roles = "jc,ad,u")]//jobCorp and admin
        public ActionResult Index()
        {
            if (Session["userRole"] != null)
            {
                if (SessionPersister.UserRole.ToString() == "Admin")
                {
                    return View(db.Users.ToList());
                }
                else if (SessionPersister.UserRole.ToString() == "User")
                {
                    return RedirectToAction("Jobs", "Main");
                }
                else if (SessionPersister.UserRole.ToString() == "JobCorp" || SessionPersister.UserRole.ToString() == "JobOwner")
                {

                    return RedirectToAction("Index", "JCDashboard");

                    //show jobcorp's panel
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            
            User user = um.findById(SessionPersister.UserId);
            return View(user);
        }

        [HttpGet]
        public ActionResult SignUp()
        {
            if (SessionPersister.UserId < 0)
            {
                ViewBag.cities = um.GetCityList();
                
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }


        }

        [HttpPost]
        public ActionResult SignUp(User user)
        {



            if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password) || um.signup(user) == null)
            {
                ViewBag.Error = "Signup was not successful";
                return View();
            }
            SessionPersister.UserId = user.Id;
            SessionPersister.Email = user.Email;
            SessionPersister.FullName = user.FullName;
            var ur = user.RoleName;
            SessionPersister.UserRole = ur;


            if(ur == "JobOwner")
            {
                return RedirectToAction("Create", "Jobs", new { JobOwnerId = user.Id });
            }
            else if (ur == "JobCorp")
            {
                return RedirectToAction("EnrollJob", "JCDashboard");
            }
            else
            {
                return RedirectToAction("Jobs", "Main");
            }
        }

        [HttpGet]
        public ActionResult SignIn()
        {
            if (SessionPersister.UserId < 0)
            {

                return View();
            }
            else
            {

                if (SessionPersister.UserRole.ToString() == "Admin")
                {

                    return RedirectToAction("Index", "Admin");
                }
                else if (SessionPersister.UserRole.ToString() == "JobCorp" || SessionPersister.UserRole.ToString() == "JobOwner")
                {

                    DateTime dt = DateTime.Now.AddDays(2);

                    var hasAppointmentsToday = db.Appointments.Any(a => a.Service.JobCorp.UserId == SessionPersister.UserId && a.StartTime >= DateTime.Now && a.StartTime <= dt);

                    var hasAppointments = db.Appointments.Any(p => p.Service.JobCorp.UserId == SessionPersister.UserId);
                    var hasServices = db.Services.Any(p => p.JobCorp.UserId == SessionPersister.UserId);
                    var hasWorkingTimes = db.WorkingTimes.Any(p => p.JobCorp.UserId == SessionPersister.UserId);
                    if (!hasServices)
                    {
                        return RedirectToAction("Create", "Services");
                    }
                    if (!hasWorkingTimes)
                    {
                        return RedirectToAction("AddWorkingTime", "JCDashboard");
                    }
                    if (hasAppointments)
                    {
                        if (hasAppointmentsToday)
                        {
                            return RedirectToAction("AppointmentList", "JCDashboard");
                        }
                        return RedirectToAction("FutureAppointmentList", "JCDashboard");
                    }
                    return RedirectToAction("Index", "JCDashboard");
                }
                else//user
                {
                    DateTime dt = DateTime.Now.AddDays(2);

                    var hasReservationsToday = db.Appointments.Any(p => p.UserId == SessionPersister.UserId && p.StartTime >= DateTime.Now && p.StartTime <= dt);

                    var hasReservations = db.Appointments.Any(p => p.UserId == SessionPersister.UserId);//
                    if (hasReservations)
                    {
                        if (hasReservationsToday)
                        {
                            return RedirectToAction("Reservations", "Main");
                        }
                        return RedirectToAction("FutureReservations", "Main");
                    }
                    return RedirectToAction("Jobs", "Main");
                }

            }
        }



        [HttpPost]
        public ActionResult SignIn(User user)
        {
           
            user = um.signIn(user.Email, user.Email, user.Password);
            if (user == null)
            {
                ViewBag.Error = "ایمیل یا رمز عبور درست وارد کنید";
                return View();
            }

            SessionPersister.UserId = user.Id;
            SessionPersister.Email = user.Email;

            SessionPersister.FullName = user.FullName;
            SessionPersister.UserRole = user.UserRole.Name;


            if (SessionPersister.UserRole.ToString() == "Admin")
            {

                return RedirectToAction("Index", "Admin");
            }
            else if (SessionPersister.UserRole.ToString() == "JobCorp" || SessionPersister.UserRole.ToString()  == "JobOwner")
            {
                DateTime dt = DateTime.Now.AddDays(2);

                var hasAppointmentsToday = db.Appointments.Any(a => a.Service.JobCorp.UserId == SessionPersister.UserId && a.StartTime >= DateTime.Now && a.StartTime <= dt);

                var hasAppointments = db.Appointments.Any(p => p.Service.JobCorp.UserId == SessionPersister.UserId);
                var hasServices = db.Services.Any(p => p.JobCorp.UserId == SessionPersister.UserId);
                var hasWorkingTimes = db.WorkingTimes.Any(p => p.JobCorp.UserId == SessionPersister.UserId);
                if (!hasServices)
                {
                    return RedirectToAction("Create", "Services");
                }
                if (!hasWorkingTimes)
                {
                    return RedirectToAction("AddWorkingTime", "JCDashboard");
                }
                if (hasAppointments)
                {
                    if (hasAppointmentsToday)
                    {
                        return RedirectToAction("AppointmentList", "JCDashboard");
                    }
                    return RedirectToAction("FutureAppointmentList", "JCDashboard");
                }
                return RedirectToAction("Index", "JCDashboard");
            }
            else//user
            {

                DateTime dt = DateTime.Now.AddDays(2);

                var hasReservationsToday = db.Appointments.Any(p => p.UserId == SessionPersister.UserId && p.StartTime >= DateTime.Now && p.StartTime <= dt);
                var hasReservations = db.Appointments.Any(p => p.UserId == SessionPersister.UserId);//
                if (hasReservations)
                {
                    if (hasReservationsToday)
                    {
                        return RedirectToAction("Reservations", "Main");
                    }
                    return RedirectToAction("FutureReservations", "Main");
                }
                return RedirectToAction("Jobs", "Main");
            }


        }

        //[CustomAuthorize(Roles = "Dev,SuperAdmin,Admin,User")]
        public ActionResult SignOut()
        {
            SessionPersister.UserId = -1;
            SessionPersister.Email = null;
            SessionPersister.FullName = null;
            SessionPersister.UserRole = null;

            ViewBag.Message = "You've Signed out successfully";

            return RedirectToAction("SignIn", "Account");
        }

        [HttpGet]
        //[CustomAuthorize(Roles = "Dev,SuperAdmin,Admin,User")]
        public ActionResult Edit()
        {
            User user = um.findById(SessionPersister.UserId);
            ViewBag.cities = um.GetCityList();

            if (user == null)
            {
                ViewBag.Error = "user Not Found";
                return RedirectToAction("Index");
            }

            return View(user);
        }

        // POST: /User/Edit/5
        [HttpPost]
        //[CustomAuthorize(Roles = "Dev,SuperAdmin,Admin,User")]
        public ActionResult Edit(User user)
        {

            um.EditUser(user);
            SessionPersister.Email = user.Email;
            SessionPersister.FullName = user.FullName;
            var ur = user.RoleName;
            //SessionPersister.UserRole = ur;


            ////else if (ur == "JobCorp")
            ////{
            ////    return RedirectToAction("EnrollJob", "JCDashboard");
            ////}
            ////else
            ////{
            ////    return RedirectToAction("Index", "Main");
            ////}
            if (SessionPersister.UserRole.ToString() == "User")
            {
                if (ur == "JobCorp")
                {
                    SessionPersister.UserRole = ur;
                    return RedirectToAction("EnrollJob", "JCDashboard");
                }
                else if (ur == "JobOwner")
                {
                    SessionPersister.UserRole = ur;
                    return RedirectToAction("Create", "Jobs", new { JobOwnerId = user.Id });
                }
                else//user
                {
                    var hasReservations = db.Appointments.Any(p => p.UserId == SessionPersister.UserId);//
                    if (hasReservations)
                    {
                        return RedirectToAction("Reservations", "Main");
                    }
                    return RedirectToAction("Jobs", "Main");
                }
            }
            else if (SessionPersister.UserRole.ToString() == "JobOwner")
            {
                if (ur == "JobCorp")
                {
                    //first we need to delete the job!
                    //Job job = new Job();
                    //int u = Int32.Parse(Session["userId"].ToString());
                    //job = db.Jobs.Where(acc => acc.User.Id == u).FirstOrDefault();
                    //db.Jobs.Remove(job);
                    //db.SaveChanges();

                    SessionPersister.UserRole = ur;
                    return RedirectToAction("EnrollJob", "JCDashboard");
                }
                else if (ur == "User")
                {
                    var hasReservations = db.Appointments.Any(p => p.UserId == SessionPersister.UserId);//
                    if (hasReservations)
                    {
                        return RedirectToAction("Reservations", "Main");
                    }
                    return RedirectToAction("Jobs", "Main");
                }
                else//JobOwner
                {
                    var hasAppointments = db.Appointments.Any(p => p.Service.JobCorp.UserId == SessionPersister.UserId);
                    var hasServices = db.Services.Any(p => p.JobCorp.UserId == SessionPersister.UserId);
                    var hasWorkingTimes = db.WorkingTimes.Any(p => p.JobCorp.UserId == SessionPersister.UserId);
                    if (!hasServices)
                    {
                        return RedirectToAction("Create", "Services");
                    }
                    if (!hasWorkingTimes)
                    {
                        return RedirectToAction("AddWorkingTime", "JCDashboard");
                    }
                    if (hasAppointments)
                    {
                        return RedirectToAction("AppointmentList", "JCDashboard");
                    }
                    return RedirectToAction("Index", "JCDashboard");

                }

            }
            else if (SessionPersister.UserRole.ToString() == "JobCorp")
            {
                if (ur == "JobOwner")
                {

                    JobCorp jc = new JobCorp();
                    var u = Int32.Parse(Session["userId"].ToString());
                    jc = db.JobCorps.Where(acc => acc.UserId.Equals(u)).FirstOrDefault();
                    
                    //IQueryable<Service> ser = db.Services.Where(acc => acc.JobCorpId == jc.Id);
                    //List<Service> serlist = ser.ToList();
                    //IQueryable<Appointment> app = db.Appointments.Where(acc => acc.Service.JobCorpId == jc.Id);
                    //List<Appointment> applist = app.ToList();
                    //IQueryable<WorkingTime> wt = db.WorkingTimes.Where(acc => acc.JobCorpId == jc.Id);
                    //List<WorkingTime> wtlist = wt.ToList();
                    if (jc == null)
                    {
                        ViewBag.Error = "سیستم موفق به حذف شما از لیست همکاران نشد";
                        return View();
                    }
                    //db.Services.RemoveRange(ser);
                    //db.SaveChanges();
                    //db.Appointments.RemoveRange(applist);
                    //db.SaveChanges();
                    //db.WorkingTimes.RemoveRange(wtlist);
                    //db.SaveChanges();

                    //db.JobCorps.Remove(jc);
                    //db.SaveChanges();

                    SessionPersister.UserRole = ur;
                    return RedirectToAction("Create", "Jobs", new { JobOwnerId = user.Id });
                }
                else if (ur == "User")
                {
                    var hasReservations = db.Appointments.Any(p => p.UserId == SessionPersister.UserId);//
                    if (hasReservations)
                    {
                        return RedirectToAction("Reservations", "Main");
                    }
                    return RedirectToAction("Jobs", "Main");
                }
                else//JobOwner
                {
                    var hasAppointments = db.Appointments.Any(p => p.Service.JobCorp.UserId == SessionPersister.UserId);
                    var hasServices = db.Services.Any(p => p.JobCorp.UserId == SessionPersister.UserId);
                    var hasWorkingTimes = db.WorkingTimes.Any(p => p.JobCorp.UserId == SessionPersister.UserId);
                    if (!hasServices)
                    {
                        return RedirectToAction("Create", "Services");
                    }
                    if (!hasWorkingTimes)
                    {
                        return RedirectToAction("AddWorkingTime", "JCDashboard");
                    }
                    if (hasAppointments)
                    {
                        return RedirectToAction("AppointmentList", "JCDashboard");
                    }
                    return RedirectToAction("Index", "JCDashboard");

                }

            }

            
            return RedirectToAction("Jobs", "Main");
        }

        //[HttpGet]
        //[CustomAuthorize(Roles = "Dev,SuperAdmin")]
        //public ActionResult EditUser(int? id)
        //{
        //    if (id.HasValue)
        //    {
        //        User user = am.findById(id.Value);
        //        if (user == null)
        //        {
        //            ViewBag.Error = "user Not Found";
        //            return RedirectToAction("Index");
        //        }
        //        List<UserRole> UserRoles = new List<UserRole>();
        //        UserRoles = db.UserRoles.ToList();
        //        ViewBag.UserRoles = UserRoles;

        //        ViewBag.EDG = am.GetEduList();
        //        return View(user);
        //    }
        //    return RedirectToAction("Members");
        //}

        //// POST: /User/Edit/5
        //[HttpPost]
        //[CustomAuthorize(Roles = "Dev,SuperAdmin")]
        //public ActionResult EditUser(User user)
        //{
        //    am.EditUser(user);
        //    return RedirectToAction("Members");
        //}

        //[HttpGet]
        //[CustomAuthorize(Roles = "Dev,SuperAdmin,Admin,User")]
        //public ActionResult Delete()
        //{
        //    User user = am.findById(SessionPersister.UserId);
        //    user = am.DeleteUser(user);
        //    if (user == null)
        //    {
        //        ViewBag.Message = "user Not Deleted";
        //    }
        //    else
        //    {
        //        ViewBag.Message = user.Username + "Deleted";
        //    }

        //    return RedirectToAction("SignIn");
        //}

        //[CustomAuthorize(Roles = "Dev,SuperAdmin")]
        //public ActionResult Members()
        //{
        //    List<User> users = new List<User>();
        //    users = am.findAll();
        //    return View(users);
        //}




    }
}