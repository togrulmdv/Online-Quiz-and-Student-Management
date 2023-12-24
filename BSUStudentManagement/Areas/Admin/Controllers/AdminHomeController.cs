using BSUStudentManagement.CommonCode;
using DAL.CommonModels;
using DAL.Entities;
using DAL.Helpers.Enums;
using DAL.Helpers.PageHelpers;
using DAL.Services;
using Microsoft.AspNetCore.Mvc;


namespace BSUStudentManagement.Areas.Admin.Controllers
{


    [ServiceFilter(typeof(AdminAuthorization))]
    [Area("Admin")]

    public class AdminHomeController : Controller
    {
        private readonly IConstants _constants;
        private readonly ISessionManager _SessionManag;
        private readonly IAdminServices _adminServices;

        public AdminHomeController( IConstants constants , ISessionManager sessionManag, IAdminServices adminServices)
        {
            this._constants = constants;
            _SessionManag = sessionManag;
            this._adminServices = adminServices;

        }

        // GET: Admin/AdminHome
        public IActionResult Dashboard()
        {
            BasicDataModels model = new BasicDataModels();
            model.ChartDataList = _adminServices.GetDashboardChartData(); //--Leads chart data
            return View(model);
        }

        [HttpGet]
        //[Route("courses/{pageId}")]
        public IActionResult Courses( string? title, int? status, int? course_category_id , int pageId=1)
        {
       
            status = status == null || status == 0 ? 0 : status;
            course_category_id = course_category_id == null || course_category_id == 0 ? 0 : course_category_id;

            BasicDataModels model = new BasicDataModels();
            model.courses_obj = new courses();


            model.courses_list = _adminServices.GetCoursesList(pageId, _constants.ITEMS_PER_PAGE(), title, status, course_category_id).ToList();

            model.pageHelperObj = new PagerHelper();
            int TotalRecords = model != null && model.courses_list != null && model.courses_list.Count() > 0 ? model.courses_list.FirstOrDefault().TotalRecordCount : 0;
            model.pageHelperObj = PagerHelper.Instance.MakePaginationObject(model.courses_list != null ? model.courses_list.Count() : 0, TotalRecords, _constants.ITEMS_PER_PAGE(), pageId);


            //---get status list
            ViewBag.StatusList = _adminServices.GetStatusDropdownList().ToList();

            //--get course categories for dropdwon GetCourseCategoriesDropDownList
            ViewBag.CourseCategories = _adminServices.GetCourseCategoriesDropDownList().ToList();

          

            return View(model);
        }

        [HttpGet]
        //[Route("courses/{pageId}")]
        public IActionResult CoursesPartial(string? title, int? status, int? course_category_id, int pageId = 1)
        {

            status = status == null || status == 0 ? 0 : status;
            course_category_id = course_category_id == null || course_category_id == 0 ? 0 : course_category_id;

            BasicDataModels model = new BasicDataModels();
            model.courses_obj = new courses();


            model.courses_list = _adminServices.GetCoursesList(pageId, _constants.ITEMS_PER_PAGE(), title, status, course_category_id).ToList();

            model.pageHelperObj = new PagerHelper();
            int TotalRecords = model != null && model.courses_list != null && model.courses_list.Count() > 0 ? model.courses_list.FirstOrDefault().TotalRecordCount : 0;
            model.pageHelperObj = PagerHelper.Instance.MakePaginationObject(model.courses_list != null ? model.courses_list.Count() : 0, TotalRecords, _constants.ITEMS_PER_PAGE(), pageId);


            //---get status list
            ViewBag.StatusList = _adminServices.GetStatusDropdownList().ToList();

            //--get course categories for dropdwon GetCourseCategoriesDropDownList
            ViewBag.CourseCategories = _adminServices.GetCourseCategoriesDropDownList().ToList();

            return PartialView("~/Areas/Admin/Views/AdminHome/_CoursesPartial.cshtml", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddNewCourse(courses FormData)
        {
            BasicDataModels model = new BasicDataModels();

            //--getting calling page URL
            FormData.ReturnURL = Request.Headers["Referer"].ToString();

            if (String.IsNullOrEmpty(FormData.title) || FormData.status == null || FormData.course_category_id == null || String.IsNullOrEmpty(FormData.description))
            {
                TempData["ErrorMsg"] = "Please fill all required fields!";
                return Redirect(FormData.ReturnURL);
            }

            //--Data Insert Part starts here
            FormData.created_by =_SessionManag.GetLoginAdminFromSession().user_id;
            FormData.guid = CommonHelper.Instance.GeneratNewGUID();
            FormData.resultMsg = _adminServices.InsertNewCourse(FormData);
            if (FormData.resultMsg == "Saved Successfully!")
            {
                TempData["SuccessMsg"] = FormData.resultMsg;
                return Redirect(FormData.ReturnURL);
            }
            else
            {
                TempData["ErrorMsg"] = FormData.resultMsg;
                return Redirect(FormData.ReturnURL);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditCourse(courses FormData)
        {
            BasicDataModels model = new BasicDataModels();

            //--getting calling page URL
            FormData.ReturnURL = Request.Headers["Referer"].ToString();


            if (FormData.course_id < 1)
            {
                TempData["ErrorMsg"] = "Course ID is null!";
                return Redirect(FormData.ReturnURL);
            }

            if (String.IsNullOrEmpty(FormData.title) || FormData.status == null || FormData.course_category_id == null || String.IsNullOrEmpty(FormData.description))
            {
                TempData["ErrorMsg"] = "Please fill all required fields!";
                return Redirect(FormData.ReturnURL);
            }

            //--Data update Part starts here
            FormData.modified_by = _SessionManag.GetLoginAdminFromSession().user_id;
            FormData.resultMsg = _adminServices.UpdateCourse(FormData);
            if (FormData.resultMsg == "Updated Successfully!")
            {
                TempData["SuccessMsg"] = FormData.resultMsg;
                return Redirect(FormData.ReturnURL);
            }
            else
            {
                TempData["ErrorMsg"] = FormData.resultMsg;
                return Redirect(FormData.ReturnURL);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCourse(int? course_id)
        {
            courses FormData = new courses();


            //--getting calling page URL
            FormData.ReturnURL = Request.Headers["Referer"].ToString();

            if (course_id == null)
            {
                TempData["ErrorMsg"] = "Some thing went wrong. Please try again!";

                return Redirect(FormData.ReturnURL);
            }

            //--Data Delete Part starts here
            FormData.resultMsg = _adminServices.DeleteCourse(course_id);

            if (FormData.resultMsg == "Deleted Successfully!")
            {
                TempData["SuccessMsg"] = FormData.resultMsg;
                return Redirect(FormData.ReturnURL);
            }
            else
            {
                TempData["ErrorMsg"] = FormData.resultMsg;
                return Redirect(FormData.ReturnURL);
            }

        }


        [HttpGet]
        //[Route("students-list/{pageId}")]
        public IActionResult StudentsList( string? user_name, string? email, int? student_id , int pageId=1)
        {
            pageId = pageId == null || pageId == 0 ? 1 : pageId;

            student_id = student_id == null || student_id == 0 ? 0 : student_id;

            BasicDataModels model = new BasicDataModels();
            model.student_obj = new users();


            model.student_list = _adminServices.GetStudentsListsForAdmin(pageId, _constants.ITEMS_PER_PAGE(), user_name, email, student_id).ToList();


            model.pageHelperObj = new PagerHelper();
            int TotalRecords = model != null && model.student_list != null && model.student_list.Count() > 0 ? model.student_list.FirstOrDefault().TotalRecordCount : 0;
            model.pageHelperObj = PagerHelper.Instance.MakePaginationObject(model.student_list != null ? model.student_list.Count() : 0, TotalRecords, _constants.ITEMS_PER_PAGE(), pageId);





            return View(model);
        }



        [HttpGet]
        //[Route("students-list/{pageId}")]
        public IActionResult StudentsListPartial(string? user_name, string? email, int? user_id, int pageId = 1)
        {
            pageId = pageId == null || pageId == 0 ? 1 : pageId;

            user_id = user_id == null || user_id == 0 ? 0 : user_id;

            BasicDataModels model = new BasicDataModels();
            model.student_obj = new users();


            model.student_list = _adminServices.GetStudentsListsForAdmin(pageId, _constants.ITEMS_PER_PAGE(), user_name, email, user_id).ToList();


            model.pageHelperObj = new PagerHelper();
            int TotalRecords = model != null && model.student_list != null && model.student_list.Count() > 0 ? model.student_list.FirstOrDefault().TotalRecordCount : 0;
            model.pageHelperObj = PagerHelper.Instance.MakePaginationObject(model.student_list != null ? model.student_list.Count() : 0, TotalRecords, _constants.ITEMS_PER_PAGE(), pageId);

            return PartialView("~/Areas/Admin/Views/AdminHome/_StudentsListPartial.cshtml", model);
        }


        [HttpPost]
        
        public IActionResult AddStudent(users FormData)
        {
            BasicDataModels model = new BasicDataModels();

            //--getting calling page URL
            FormData.ReturnURL = Request.Headers["Referer"].ToString();

            if (String.IsNullOrEmpty(FormData.first_name) || String.IsNullOrEmpty(FormData.last_name) || String.IsNullOrEmpty(FormData.user_name) || String.IsNullOrEmpty(FormData.email))
            {
                TempData["ErrorMsg"] = "Please fill all required fields!";
                return Redirect(FormData.ReturnURL);
            }

            //--Data Insert Part starts here
            FormData.created_by = _SessionManag.GetLoginAdminFromSession().user_id;
            FormData.guid = CommonHelper.Instance.GeneratNewGUID();
            FormData.password = CommonHelper.Instance.GeneratRandomNumber();
            FormData.user_type = (short)UserTypesEnum.Student;
            FormData.resultMsg = _adminServices.InsertNewStudent(FormData);
            if (FormData.resultMsg == "Saved Successfully!")
            {
                TempData["SuccessMsg"] = FormData.resultMsg;
                return Redirect(FormData.ReturnURL);
            }
            else
            {
                TempData["ErrorMsg"] = FormData.resultMsg;
                return Redirect(FormData.ReturnURL);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditStudent(users FormData)
        {
            BasicDataModels model = new BasicDataModels();

            //--getting calling page URL
            FormData.ReturnURL = Request.Headers["Referer"].ToString();


            if (FormData.user_id < 1)
            {
                TempData["ErrorMsg"] = "Student ID is null!";
                return Redirect(FormData.ReturnURL);
            }

            if (String.IsNullOrEmpty(FormData.first_name) || String.IsNullOrEmpty(FormData.last_name) || String.IsNullOrEmpty(FormData.user_name) || String.IsNullOrEmpty(FormData.email))
            {
                TempData["ErrorMsg"] = "Please fill all required fields!";
                return Redirect(FormData.ReturnURL);
            }

            //--Data update Part starts here
            FormData.modified_by = _SessionManag.GetLoginAdminFromSession().user_id;
            FormData.resultMsg = _adminServices.EditStudent(FormData);
            if (FormData.resultMsg == "Updated Successfully!")
            {
                TempData["SuccessMsg"] = FormData.resultMsg;
                return Redirect(FormData.ReturnURL);
            }
            else
            {
                TempData["ErrorMsg"] = FormData.resultMsg;
                return Redirect(FormData.ReturnURL);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteStudent(int? user_id)
        {
            courses FormData = new courses();


            //--getting calling page URL
            FormData.ReturnURL = Request.Headers["Referer"].ToString();

            if (user_id == null)
            {
                TempData["ErrorMsg"] = "Some thing went wrong. Please try again!";

                return Redirect(FormData.ReturnURL);
            }

            //--Data Delete Part starts here
            FormData.resultMsg = _adminServices.DeleteStudent(user_id);

            if (FormData.resultMsg == "Deleted Successfully!")
            {
                TempData["SuccessMsg"] = FormData.resultMsg;
                return Redirect(FormData.ReturnURL);
            }
            else
            {
                TempData["ErrorMsg"] = FormData.resultMsg;
                return Redirect(FormData.ReturnURL);
            }

        }

        [HttpGet]
        public IActionResult StudentRegisteredCourses( int student_id, string? title, int? course_id , int pageId=1)
        {
            pageId = pageId == null || pageId == 0 ? 1 : pageId;
            course_id = course_id == null || course_id == 0 ? 0 : course_id;

            ViewBag.student_id = student_id;
            BasicDataModels model = new BasicDataModels();
            model.courses_obj = new courses();


            model.courses_list = _adminServices.GetStudentRegisteredCourses(pageId, _constants.ITEMS_PER_PAGE(), student_id, title, course_id).ToList();


            model.pageHelperObj = new PagerHelper();
            int TotalRecords = model != null && model.courses_list != null && model.courses_list.Count() > 0 ? model.courses_list.FirstOrDefault().TotalRecordCount : 0;
            model.pageHelperObj = PagerHelper.Instance.MakePaginationObject(model.courses_list != null ? model.courses_list.Count() : 0, TotalRecords, _constants.ITEMS_PER_PAGE(), pageId);




            return View(model);
        }

        [HttpGet]
        public IActionResult StudentRegisteredCoursesPartial(int student_id, string? title, int? course_id, int pageId = 1)
        {
            pageId = pageId == null || pageId == 0 ? 1 : pageId;
            course_id = course_id == null || course_id == 0 ? 0 : course_id;

            ViewBag.student_id = student_id;
            BasicDataModels model = new BasicDataModels();
            model.courses_obj = new courses();


            model.courses_list = _adminServices.GetStudentRegisteredCourses(pageId, _constants.ITEMS_PER_PAGE(), student_id, title, course_id).ToList();


            model.pageHelperObj = new PagerHelper();
            int TotalRecords = model != null && model.courses_list != null && model.courses_list.Count() > 0 ? model.courses_list.FirstOrDefault().TotalRecordCount : 0;
            model.pageHelperObj = PagerHelper.Instance.MakePaginationObject(model.courses_list != null ? model.courses_list.Count() : 0, TotalRecords, _constants.ITEMS_PER_PAGE(), pageId);

            return PartialView("~/Areas/Admin/Views/AdminHome/_StudentRegisteredCoursesPartial.cshtml", model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AssignNewCourseToStudent(student_courses FormData)
        {
            BasicDataModels model = new BasicDataModels();

            //--getting calling page URL
            FormData.ReturnURL = Request.Headers["Referer"].ToString();


            if (FormData.student_id < 1)
            {
                TempData["ErrorMsg"] = "Student ID is null!";
                return Redirect(FormData.ReturnURL);
            }

            if (FormData.course_id < 1)
            {
                TempData["ErrorMsg"] = "Please fill all required fields!";
                return Redirect(FormData.ReturnURL);
            }

            //--Data update Part starts here
            FormData.created_by = _SessionManag.GetLoginAdminFromSession().user_id;
            FormData.resultMsg = _adminServices.AssignCourseToStudent(FormData);
            if (FormData.resultMsg == "Saved Successfully!")
            {
                TempData["SuccessMsg"] = FormData.resultMsg;
                return Redirect(FormData.ReturnURL);
            }
            else
            {
                TempData["ErrorMsg"] = FormData.resultMsg;
                return Redirect(FormData.ReturnURL);
            }

        }

        [HttpGet]
       
        public IActionResult GetStudentsUnassignCourseList(int student_id, string? course_title)

        {
            BasicDataModels model = new BasicDataModels();
            //---get status list
            model.courses_list = _adminServices.GetStudentsUnAssignCourseList(student_id, course_title).ToList();

            if (model.courses_list != null)
            {
                if (model.courses_list.Count() > 0)
                {
                    return Json(model.courses_list);
                }
                else
                {
                    return Json(new { message = "No data found" });
                }

            }
            else
            {
                return Json(new { message = "No data found" });
            }


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveStudentFromCourse(int student_course_id)
        {
            courses FormData = new courses();


            //--getting calling page URL
            FormData.ReturnURL = Request.Headers["Referer"].ToString();

            if (student_course_id < 1)
            {
                TempData["ErrorMsg"] = "Some thing went wrong. Please try again!";

                return Redirect(FormData.ReturnURL);
            }

            //--Data Delete Part starts here
            FormData.resultMsg = _adminServices.RemoveCourseFromStudentList(student_course_id);

            if (FormData.resultMsg == "Deleted Successfully!")
            {
                TempData["SuccessMsg"] = FormData.resultMsg;
                return Redirect(FormData.ReturnURL);
            }
            else
            {
                TempData["ErrorMsg"] = FormData.resultMsg;
                return Redirect(FormData.ReturnURL);
            }

        }

        //-------------------------------
        [HttpGet]
        public IActionResult TeachersList( string? user_name, string? email, int? user_id , int pageId=1)
        {
            pageId = pageId == null || pageId == 0 ? 1 : pageId;

            user_id = user_id == null || user_id == 0 ? 0 : user_id;
            int user_type = 2; //2 is teacher in user table

            BasicDataModels model = new BasicDataModels();
            model.user_obj = new users();


            model.user_list = _adminServices.GetTeachersListsForAdmin(pageId, _constants.ITEMS_PER_PAGE(), user_name, email, user_id, user_type).ToList();


            model.pageHelperObj = new PagerHelper();
            int TotalRecords = model != null && model.user_list != null && model.user_list.Count() > 0 ? model.user_list.FirstOrDefault().TotalRecordCount : 0;
            model.pageHelperObj = PagerHelper.Instance.MakePaginationObject(model.user_list != null ? model.user_list.Count() : 0, TotalRecords, _constants.ITEMS_PER_PAGE(), pageId);


            return View(model);
        }

        [HttpGet]
        public IActionResult TeachersListPartial(string? user_name, string? email, int? user_id, int pageId = 1)
        {
            pageId = pageId == null || pageId == 0 ? 1 : pageId;

            user_id = user_id == null || user_id == 0 ? 0 : user_id;
            int user_type = 2; //2 is teacher in user table

            BasicDataModels model = new BasicDataModels();
            model.user_obj = new users();


            model.user_list = _adminServices.GetTeachersListsForAdmin(pageId, _constants.ITEMS_PER_PAGE(), user_name, email, user_id, user_type).ToList();


            model.pageHelperObj = new PagerHelper();
            int TotalRecords = model != null && model.user_list != null && model.user_list.Count() > 0 ? model.user_list.FirstOrDefault().TotalRecordCount : 0;
            model.pageHelperObj = PagerHelper.Instance.MakePaginationObject(model.user_list != null ? model.user_list.Count() : 0, TotalRecords, _constants.ITEMS_PER_PAGE(), pageId);



            return PartialView("~/Areas/Admin/Views/AdminHome/_TeachersListPartial.cshtml", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddTeacher(users FormData)
        {
            BasicDataModels model = new BasicDataModels();

            //--getting calling page URL
            FormData.ReturnURL = Request.Headers["Referer"].ToString();

            if (String.IsNullOrEmpty(FormData.first_name) || String.IsNullOrEmpty(FormData.last_name) || String.IsNullOrEmpty(FormData.user_name) || String.IsNullOrEmpty(FormData.email))
            {
                TempData["ErrorMsg"] = "Please fill all required fields!";
                return Redirect(FormData.ReturnURL);
            }

            //--Data Insert Part starts here
            FormData.created_by = _SessionManag.GetLoginAdminFromSession().user_id;
            FormData.user_type = (short)UserTypesEnum.Teacher;
            FormData.password = CommonHelper.Instance.GeneratRandomNumber();
            FormData.resultMsg = _adminServices.InsertNewTeacher(FormData);
            if (FormData.resultMsg == "Saved Successfully!")
            {
                TempData["SuccessMsg"] = FormData.resultMsg;
                return Redirect(FormData.ReturnURL);
            }
            else
            {
                TempData["ErrorMsg"] = FormData.resultMsg;
                return Redirect(FormData.ReturnURL);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditTeacher(users FormData)
        {
            BasicDataModels model = new BasicDataModels();

            //--getting calling page URL
            FormData.ReturnURL = Request.Headers["Referer"].ToString();


            if (FormData.user_id < 1)
            {
                TempData["ErrorMsg"] = "Student ID is null!";
                return Redirect(FormData.ReturnURL);
            }

            if (String.IsNullOrEmpty(FormData.first_name) || String.IsNullOrEmpty(FormData.last_name) || String.IsNullOrEmpty(FormData.user_name) || String.IsNullOrEmpty(FormData.email))
            {
                TempData["ErrorMsg"] = "Please fill all required fields!";
                return Redirect(FormData.ReturnURL);
            }

            //--Data update Part starts here
            FormData.modified_by = _SessionManag.GetLoginAdminFromSession().user_id;
            FormData.resultMsg = _adminServices.EditTeacher(FormData);
            if (FormData.resultMsg == "Updated Successfully!")
            {
                TempData["SuccessMsg"] = FormData.resultMsg;
                return Redirect(FormData.ReturnURL);
            }
            else
            {
                TempData["ErrorMsg"] = FormData.resultMsg;
                return Redirect(FormData.ReturnURL);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteTeacher(int? user_id)
        {
            courses FormData = new courses();


            //--getting calling page URL
            FormData.ReturnURL = Request.Headers["Referer"].ToString();

            if (user_id == null)
            {
                TempData["ErrorMsg"] = "Some thing went wrong. Please try again!";

                return Redirect(FormData.ReturnURL);
            }

            //--Data Delete Part starts here
            FormData.resultMsg = _adminServices.DeleteTeacher(user_id);

            if (FormData.resultMsg == "Deleted Successfully!")
            {
                TempData["SuccessMsg"] = FormData.resultMsg;
                return Redirect(FormData.ReturnURL);
            }
            else
            {
                TempData["ErrorMsg"] = FormData.resultMsg;
                return Redirect(FormData.ReturnURL);
            }

        }

        [HttpGet]
        public IActionResult TeacherAssignCourses(int user_id, string? title, int? course_id , int pageId=1)
        {
          
            course_id = course_id == null || course_id == 0 ? 0 : course_id;

            ViewBag.user_id = user_id;
            BasicDataModels model = new BasicDataModels();
            model.courses_obj = new courses();


            model.courses_list = _adminServices.GetTeacherAssignCourses(pageId, _constants.ITEMS_PER_PAGE(), user_id, title, course_id).ToList();

            model.pageHelperObj = new PagerHelper();
            int TotalRecords = model != null && model.courses_list != null && model.courses_list.Count() > 0 ? model.courses_list.FirstOrDefault().TotalRecordCount : 0;
            model.pageHelperObj = PagerHelper.Instance.MakePaginationObject(model.courses_list != null ? model.courses_list.Count() : 0, TotalRecords, _constants.ITEMS_PER_PAGE(), pageId);



            return View(model);
        }

        [HttpGet]
        public IActionResult TeacherAssignCoursesPartial(int user_id, string? title, int? course_id, int pageId = 1)
        {

            course_id = course_id == null || course_id == 0 ? 0 : course_id;

            ViewBag.user_id = user_id;
            BasicDataModels model = new BasicDataModels();
            model.courses_obj = new courses();


            model.courses_list = _adminServices.GetTeacherAssignCourses(pageId, _constants.ITEMS_PER_PAGE(), user_id, title, course_id).ToList();

            model.pageHelperObj = new PagerHelper();
            int TotalRecords = model != null && model.courses_list != null && model.courses_list.Count() > 0 ? model.courses_list.FirstOrDefault().TotalRecordCount : 0;
            model.pageHelperObj = PagerHelper.Instance.MakePaginationObject(model.courses_list != null ? model.courses_list.Count() : 0, TotalRecords, _constants.ITEMS_PER_PAGE(), pageId);



           
                return PartialView("~/Areas/Admin/Views/AdminHome/_TeacherAssignCoursesPartial.cshtml", model);
           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AssignNewCourseToTeacher(teacher_assign_courses FormData)
        {
            BasicDataModels model = new BasicDataModels();

            //--getting calling page URL
            FormData.ReturnURL = Request.Headers["Referer"].ToString();


            if (FormData.user_id == null)
            {
                TempData["ErrorMsg"] = "user_id is null!";
                return Redirect(FormData.ReturnURL);
            }

            if (FormData.user_id < 1)
            {
                TempData["ErrorMsg"] = "user_id should not be in minus!";
                return Redirect(FormData.ReturnURL);
            }

            if (FormData.course_id < 1)
            {
                TempData["ErrorMsg"] = "Please fill all required fields!";
                return Redirect(FormData.ReturnURL);
            }

            //--Data update Part starts here
            FormData.created_by = _SessionManag.GetLoginAdminFromSession().user_id;
            FormData.resultMsg = _adminServices.AssignCourseToTeacher(FormData);
            if (FormData.resultMsg == "Saved Successfully!")
            {
                TempData["SuccessMsg"] = FormData.resultMsg;
                return Redirect(FormData.ReturnURL);
            }
            else
            {
                TempData["ErrorMsg"] = FormData.resultMsg;
                return Redirect(FormData.ReturnURL);
            }

        }

        [HttpGet]
        public IActionResult GetTeacherUnassignCourseList(int user_id, string? course_title)

        {
            BasicDataModels model = new BasicDataModels();
            //---get status list
            model.courses_list = _adminServices.GetTeacherUnAssignCourseList(user_id, course_title).ToList();

            if (model.courses_list != null)
            {
                if (model.courses_list.Count() > 0)
                {
                    return Json(model.courses_list);
                   
                }
                else
                {
                    return Json(new { message = "No data found" });
                }

            }
            else
            {
                return Json(new { message = "No data found" });
            }


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveCourseFromTeacherCourses(int teacher_assign_id)
        {
            courses FormData = new courses();


            //--getting calling page URL
            FormData.ReturnURL = Request.Headers["Referer"].ToString();

            if (teacher_assign_id < 1)
            {
                TempData["ErrorMsg"] = "Some thing went wrong. Please try again!";

                return Redirect(FormData.ReturnURL);
            }

            //--Data Delete Part starts here
            FormData.resultMsg = _adminServices.RemoveCourseFromTeacherCoursesList(teacher_assign_id);

            if (FormData.resultMsg == "Deleted Successfully!")
            {
                TempData["SuccessMsg"] = FormData.resultMsg;
                return Redirect(FormData.ReturnURL);
            }
            else
            {
                TempData["ErrorMsg"] = FormData.resultMsg;
                return Redirect(FormData.ReturnURL);
            }

        }

        public IActionResult GetAdminFooterTotalStudentsCourses()
        {
          
            FooterData model = new FooterData();

            model = _adminServices.GetAdminFooterTotalStudentsCourses();
            if (model != null)
            {
              
                return Json(new { Success = true, FooterData = model, Message = "Successfully!" });

            }
            else
            {

                return Json(new { Success = false, Message = "An error occured. Please try again!" });
            }

        }
    }
}
