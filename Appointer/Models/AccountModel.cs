using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;

namespace Appointer.Models
{
    public class AccountModel
    {

        private AppointerEntities db = new AppointerEntities();
        private User _user = new User();

        public AccountModel()
        {
        }

        public User signIn(string email, string fullname, string password)
        {
            return db.Users.Where(acc => (acc.Email.Equals(email) || acc.FullName.Equals(fullname)) && acc.Password.Equals(password)).FirstOrDefault();
        }

        public User signup(User user)
        {
            //user.UserRoleId = db.UserRoles.Where(m => m.Name.Equals("User")).FirstOrDefault().Id;
            //db.UserInfoes.Add(user.UserInfo);

            db.Users.Add(user);

            //db.Jobs.Add(job);
            //-------------------------to add shit
            db.SaveChanges();
            return user;
        }

        public List<City> GetCityList()
        {
            return db.Cities.ToList();
        }



        public List<UserRole_Persian> GetUserRolesList()
        {

            return db.UserRoles.Select(p => p.Id).Cast<UserRole_Persian>().Where(ur => ur != UserRole_Persian.مدیر).ToList();

        }


        public User EditUser(User user)
        {
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
            return user;
        }

        public User DeleteUser(User user)
        {
            db.Entry(user).State = EntityState.Deleted;
            db.SaveChanges();
            return user;
        }

        public User find(string email)
        {
            return db.Users.Where(acc => acc.Email.Equals(email)).FirstOrDefault();
        }

        public User findById(int id)
        {
            return db.Users.Where(acc => acc.Id.Equals(id)).FirstOrDefault();
        }

        public List<User> findAll()
        {
            return db.Users.ToList();
        }




    }
}