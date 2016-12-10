using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;

namespace Appointer.Models
{
    public class JobModel
    {

        private AppointerEntities db = new AppointerEntities();
        private Job _job = new Job();

        public JobModel()
        {
        }


        public Job EditJob(Job job)
        {
            //db.Entry(job).State = EntityState.Modified;
            db.SaveChanges();
            return job;
        }

        public Job Enroll(string ek)
        {
            return db.Jobs.Where(acc => acc.EnrollmentKey.Equals(ek)).FirstOrDefault();
        }


        //public User DeleteUser(User user)
        //{
        //    db.Entry(user).State = EntityState.Deleted;
        //    db.SaveChanges();
        //    return user;
        //}



        public Job findByUserId(int id)
        {
            return db.Jobs.Where(acc => acc.JobOwnerId.Equals(id)).FirstOrDefault();
        }

        public List<Job> findAll()
        {
            return db.Jobs.ToList();
        }




    }
}