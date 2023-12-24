using DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSUStudentManagement.CommonCode
{
    public class AdminAuthorization : ActionFilterAttribute
    {
        private readonly ISessionManager _SessionManag;
        public AdminAuthorization(ISessionManager sessionManag)
        {
            _SessionManag = sessionManag;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            users? usr = new users();

            usr = _SessionManag.GetLoginAdminFromSession();


            //--if teacher session null, then redirect to admin login page
            if (usr == null || usr.user_id < 1)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "AdminAccount",
                    action = "Login",
                    area = "Admin"
                }));

            }


            base.OnActionExecuting(filterContext);
        }

    }
}