using Appointer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Appointer.Security
{
    //public class CustomAuthorizeAttribute
    //{
    //}
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        //public override void OnAuthorization(AuthorizationContext filterContext)
        //{
        //    if (string.IsNullOrEmpty(SessionPersister.Email))
        //        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "SignIn" }));
        //    else
        //    {
        //        AccountModel am = new AccountModel();
        //        CustomPrincipal cp = new CustomPrincipal(am.find(SessionPersister.Email));
        //        if (!cp.IsInRole(Roles))
        //            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Error", action = "Index" }));
        //    }
        //}
    }


}