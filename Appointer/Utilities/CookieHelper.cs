using System;
using System.Web;

namespace Appointer.Utility
{
    public static class CookieHelper
    {
        public static bool IsCookie(string cookieName, string CookieValue)
        {
            try
            {
                var cookie = HttpContext.Current.Request.Cookies.Get(cookieName);
                var value = cookie.Value.Split(',');
                foreach (var item in value)
                {
                    if (CookieValue == item)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static HttpCookie GetCookie(string CookieName)
        {
            if (HttpContext.Current.Request.Cookies[CookieName] != null)
            {
                return HttpContext.Current.Request.Cookies[CookieName];
            }
            else
            {
                return new HttpCookie(CookieName);
            }
        }

        public static bool SetCookie(string CookieName, string CookieValue, DateTime ExpireDate, bool HttpOnly = false)
        {
            try
            {
                if (HttpContext.Current.Request.Cookies[CookieName] == null)
                {
                    var cookie = new HttpCookie(CookieName, CookieValue);
                    cookie.Expires = ExpireDate;
                    cookie.HttpOnly = (HttpOnly) ? true : false;
                    HttpContext.Current.Response.Cookies.Add(cookie);
                }
                else
                {
                    var cookie = HttpContext.Current.Request.Cookies.Get(CookieName);
                    cookie.Value = CookieValue;
                    HttpContext.Current.Response.Cookies.Add(cookie);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}