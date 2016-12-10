using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Appointer.Security
{
    public static class SessionPersister
    {
        public static string Email
        {
            get
            {
                if (HttpContext.Current == null)
                    return string.Empty;
                var sessionvar = HttpContext.Current.Session["email"];
                if (sessionvar != null)
                    return sessionvar as string;
                return null;
            }
            set
            {
                HttpContext.Current.Session["email"] = value;
            }
        }


        public static int UserId
        {
            get
            {
                if (HttpContext.Current == null)
                    return -1;
                var sessionvar = HttpContext.Current.Session["userId"];
                if (sessionvar != null)
                    return Int32.Parse(sessionvar.ToString());
                return -1;
            }
            set
            {
                HttpContext.Current.Session["userId"] = value;
            }
        }

        public static int JobId
        {
            get
            {
                if (HttpContext.Current == null)
                    return -1;
                var sessionvar = HttpContext.Current.Session["jobId"];
                if (sessionvar != null)
                    return Int32.Parse(sessionvar.ToString());
                return -1;
            }
            set
            {
                HttpContext.Current.Session["jobId"] = value;
            }
        }

        public static string UserRole
        {
            get
            {
                if (HttpContext.Current == null)
                    return string.Empty;
                var sessionvar = HttpContext.Current.Session["userRole"];
                if (sessionvar != null)
                    return sessionvar as string;
                return null;
            }
            set
            {
                HttpContext.Current.Session["userRole"] = value;
            }
        }

        public static string FullName
        {
            get
            {
                if (HttpContext.Current == null)
                    return string.Empty;
                var sessionvar = HttpContext.Current.Session["fullName"];
                if (sessionvar != null)
                    return sessionvar as string;
                return null;
            }
            set
            {
                HttpContext.Current.Session["fullName"] = value;
            }
        }



    }
}