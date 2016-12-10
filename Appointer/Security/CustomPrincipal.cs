using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using Appointer.Models;

namespace Appointer.Security
{
    //public class CustomPrincipal
    //{
    //}
    public class CustomPrincipal : IPrincipal
    {
        private User user;

        public CustomPrincipal(User account)
        {
            this.user = account;
            this.Identity = new GenericIdentity(account.Email);
        }

        public IIdentity Identity
        {
            get;
            set;
        }

        //public bool IsInRole(string role)
        //{
        //    throw new NotImplementedException();
        //}

        public bool IsInRole(string role)
        {
            var roles = role.Split(new char[] { ',' });
            return roles.Any(r => this.user.UserRole.Name.Contains(r));
            //Roles: ad-- > admin / jo = JobOwner / u = normal user
        }
    }

}