using BSUStudentManagement.CommonCode;
using DAL.Entities;
using DAL.Helpers.Enums;
using DAL.Helpers.PageHelpers;
using DAL.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace BSUStudentManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminAccountController : Controller
    {


        private readonly IConstants _constants;
        private readonly ISessionManager _SessionManag;
        private readonly IHttpContextAccessor _contx;
        private readonly IAdminServices _adminServices;
 

        public AdminAccountController(IConstants constants, ISessionManager sessionManag , IHttpContextAccessor contx , IAdminServices adminServices)
        {
            this._constants = constants;
            _SessionManag = sessionManag;
            this._contx = contx;
            this._adminServices = adminServices;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string? email, string? password)
        {

            if (String.IsNullOrEmpty(email) || String.IsNullOrEmpty(password))
            {
                TempData["ErrorMsg"] = "Incorrect email or password!";
                return RedirectToAction("Login", "AdminAccount");
            }

            var user = _adminServices.UserLogin(email, password);

            if (user?.user_type == (short)UserTypesEnum.Admin)
            {
                _contx.HttpContext.Session.SetString("Admin", JsonConvert.SerializeObject(user));

                return RedirectToAction("Dashboard", "AdminHome");
            }
            else if (user?.user_type == (short)UserTypesEnum.Teacher)
            {
                _contx.HttpContext.Session.SetString("Teacher", JsonConvert.SerializeObject(user));


                return RedirectToAction("Index", "TeacherHome", new { area = "Teachers" });
            }
            else if(user?.user_type == (short)UserTypesEnum.Student)
            {
            
                _contx.HttpContext.Session.SetString("Student", JsonConvert.SerializeObject(user));

                return RedirectToAction("Index", "UserHome", new { area = "Users" });
            }
            else
            {
                TempData["ErrorMsg"] = "Incorrect email or password!";
                return RedirectToAction("Login", "AdminAccount");
            }

           
        }

        [HttpGet]
        public IActionResult Logout()
        {

          
            string? msg = RemoveSessionAndCookies();
            if (msg != "Removed Successfully!")
            {
                return RedirectToAction("Index", "Login");
            }

            return RedirectToAction("Login", "AdminAccount");


        }

        public string? RemoveSessionAndCookies()
        {
            string? result = "Removed Successfully!";
            HttpContext.Session.Clear();
            if (Request.Cookies != null)
            {
                try
                {
                    foreach (var cookie in Request.Cookies.Keys)
                    {
                        Response.Cookies.Delete(cookie);
                    }
                }
                catch (Exception ex)
                {
                    string? errorMsg = ex.Message;
                    result = "An error occured.";
                    return result;
                }

            }

            return result;

        }
    }
}
