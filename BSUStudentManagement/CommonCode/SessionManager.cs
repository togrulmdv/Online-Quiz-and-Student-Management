using DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;



namespace BSUStudentManagement.CommonCode
{
    public  class SessionManager :  ControllerBase, ISessionManager
    {
        private readonly IHttpContextAccessor _contx;
    
      
        private readonly IConfiguration _configuration;
        public SessionManager(IHttpContextAccessor contx,  IConfiguration configuration)
        {
            this._contx = contx;
            this._configuration = configuration;
        }


        public users? GetLoginAdminFromSession()
        {

            users? user = new users();


            var Users = _contx != null && _contx.HttpContext != null && _contx.HttpContext.Session != null ? _contx.HttpContext.Session.GetString("Admin") : null;
            if (!String.IsNullOrEmpty(Users))
            {
                user = JsonConvert.DeserializeObject<users>(Users);

                return user;
            }
            else
            {

                return null;
            }

        }

        public users? GetLoginTeacherFromSession()
        {

            users? user = new users();

            var Users = _contx != null && _contx.HttpContext != null && _contx.HttpContext.Session != null ? _contx.HttpContext.Session.GetString("Teacher") : null;
            if (Users != null && !String.IsNullOrEmpty(Users))
            {
                user = JsonConvert.DeserializeObject<users>(Users);

                return user;
            }
            else
            {

                return null;
            }
        }

        public users? GetLoginStudentFromSession()
        {

            users? user = new users();

            var Users = _contx != null && _contx.HttpContext != null && _contx.HttpContext.Session != null ? _contx.HttpContext.Session.GetString("Student") : null;
            if (Users != null && !String.IsNullOrEmpty(Users))
            {
                user = JsonConvert.DeserializeObject<users>(Users);

                return user;
            }
            else
            {

                return null;
            }
        }


      

        public void SetMenusInSession(string? DashboardLangShortName)
        {
            string? lang = String.IsNullOrEmpty(DashboardLangShortName) ? "en" : DashboardLangShortName;

        }

        public void SetUserDataInSession(users model)
        {
            //set session
            var user = JsonConvert.SerializeObject(model);
        
        }

    }

    public interface ISessionManager
    {
        public users? GetLoginAdminFromSession();
        public users? GetLoginTeacherFromSession();
        public users? GetLoginStudentFromSession();
        public void SetUserDataInSession(users model);
        public void SetMenusInSession(string? DashboardLangShortName);
    }
}