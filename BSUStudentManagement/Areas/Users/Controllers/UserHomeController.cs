using BSUStudentManagement.CommonCode;
using DAL.CommonModels;
using DAL.Entities;
using DAL.Helpers.PageHelpers;
using DAL.Services;
using Microsoft.AspNetCore.Mvc;


namespace BSUStudentManagement.Areas.Users.Controllers
{

    [ServiceFilter(typeof(StudentAuthorization))]
    [Area("Users")]

    public class UserHomeController : Controller
    {

        private readonly IConstants _constants;
        private readonly ISessionManager _SessionManag;
        private readonly IAdminServices _adminServices;
        private readonly IStudentServices _studentServices;



        public UserHomeController(IConstants constants, ISessionManager sessionManag , IAdminServices adminServices , IStudentServices studentServices)
        {
            this._constants = constants;
            _SessionManag = sessionManag;
            this._adminServices = adminServices;
            this._studentServices = studentServices;
        }


        [HttpGet]

        public IActionResult Index()
        {

            int? student_id = _SessionManag.GetLoginStudentFromSession() == null ? 0 : _SessionManag.GetLoginStudentFromSession().user_id;
            BasicDataModels model = new BasicDataModels();

            //--get course list by student id
            model.courses_list = _studentServices.GetCoursesForDropDownByStudentID(student_id);

            return View(model);
        }

        [HttpGet]
        public IActionResult CoursesList(string? title, int? status, int? course_category_id  , int pageId=1)
        {

            status = status == null || status == 0 ? 0 : status;
            course_category_id = course_category_id == null || course_category_id == 0 ? 0 : course_category_id;

            int? student_id = _SessionManag.GetLoginStudentFromSession() == null ? 0 : _SessionManag.GetLoginStudentFromSession().user_id;

            BasicDataModels model = new BasicDataModels();
            model.courses_obj = new courses();


            model.courses_list = _studentServices.GetStudentMainCoursesListByStudentID(pageId, _constants.ITEMS_PER_PAGE(), title, status, course_category_id, student_id).ToList();

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
        public IActionResult CoursesListPartial(string? title, int? status, int? course_category_id, int pageId = 1)
        {

            status = status == null || status == 0 ? 0 : status;
            course_category_id = course_category_id == null || course_category_id == 0 ? 0 : course_category_id;

            int? student_id = _SessionManag.GetLoginStudentFromSession() == null ? 0 : _SessionManag.GetLoginStudentFromSession().user_id;

            BasicDataModels model = new BasicDataModels();
            model.courses_obj = new courses();


            model.courses_list = _studentServices.GetStudentMainCoursesListByStudentID(pageId, _constants.ITEMS_PER_PAGE(), title, status, course_category_id, student_id).ToList();

            model.pageHelperObj = new PagerHelper();
            int TotalRecords = model != null && model.courses_list != null && model.courses_list.Count() > 0 ? model.courses_list.FirstOrDefault().TotalRecordCount : 0;
            model.pageHelperObj = PagerHelper.Instance.MakePaginationObject(model.courses_list != null ? model.courses_list.Count() : 0, TotalRecords, _constants.ITEMS_PER_PAGE(), pageId);


            //---get status list
            ViewBag.StatusList = _adminServices.GetStatusDropdownList().ToList();

            //--get course categories for dropdwon GetCourseCategoriesDropDownList
            ViewBag.CourseCategories = _adminServices.GetCourseCategoriesDropDownList().ToList();

           
                return PartialView("~/Areas/Users/Views/UserHome/_CoursesListPartial.cshtml", model);
          
        }



        [HttpGet]
        public IActionResult StudentQuizList(int? course_id,  string? title , int pageId=1)
        {
            course_id = course_id == null ? 0 : course_id;

            ViewBag.course_id = course_id;

            int? student_id = _SessionManag.GetLoginStudentFromSession() == null ? 0 : _SessionManag.GetLoginStudentFromSession().user_id;

            BasicDataModels model = new BasicDataModels();
            model.quiz_obj = new quiz();
            //--get quiz list by student id
            model.quiz_list = _studentServices.GetQuizListForStudentExamByCourseID(pageId, _constants.ITEMS_PER_PAGE(), student_id, course_id, title);

            model.pageHelperObj = new PagerHelper();
            int TotalRecords = model != null && model.quiz_list != null && model.quiz_list.Count() > 0 ? model.quiz_list.FirstOrDefault().TotalRecordCount : 0;
            model.pageHelperObj = PagerHelper.Instance.MakePaginationObject(model.quiz_list != null ? model.quiz_list.Count() : 0, TotalRecords, _constants.ITEMS_PER_PAGE(), pageId);


            return View(model);
        }

        [HttpGet]
        public IActionResult StudentQuizListPartial(int? course_id, string? title, int pageId = 1)
        {
            course_id = course_id == null ? 0 : course_id;

            ViewBag.course_id = course_id;

            int? student_id = _SessionManag.GetLoginStudentFromSession() == null ? 0 : _SessionManag.GetLoginStudentFromSession().user_id;

            BasicDataModels model = new BasicDataModels();
            model.quiz_obj = new quiz();
            //--get quiz list by student id
            model.quiz_list = _studentServices.GetQuizListForStudentExamByCourseID(pageId, _constants.ITEMS_PER_PAGE(), student_id, course_id, title);

            model.pageHelperObj = new PagerHelper();
            int TotalRecords = model != null && model.quiz_list != null && model.quiz_list.Count() > 0 ? model.quiz_list.FirstOrDefault().TotalRecordCount : 0;
            model.pageHelperObj = PagerHelper.Instance.MakePaginationObject(model.quiz_list != null ? model.quiz_list.Count() : 0, TotalRecords, _constants.ITEMS_PER_PAGE(), pageId);



          
                return PartialView("~/Areas/Users/Views/UserHome/_StudentQuizListPartial.cshtml", model);
            
        }

        [HttpGet]
        public IActionResult StartQuiz(int? quiz_ID)
        {

            quiz_ID = quiz_ID == null ? 2 : quiz_ID;

            BasicDataModels model = new BasicDataModels();
            model.quiz_categories_obj = new quiz_categories();

            //--get queiz detail
            model.quiz_obj = _adminServices.GetQuizDetailByID(quiz_ID);
            //--get question list by quiz_id
            model.quiz_ques_list = _studentServices.GetQuestionListByQuizId(quiz_ID).ToList();

            return View(model);
        }

        [HttpPost]

        public IActionResult SaveAnswer(int question_id, string? correct_option)
        {



            if (String.IsNullOrEmpty(correct_option))
            {

                return Json("an error occured. please try again" );
            }

            //--Data upate Part starts here
            int student_id = _SessionManag.GetLoginStudentFromSession().user_id;
            string? guid = CommonHelper.Instance.GeneratNewGUID();
            string? resultMsg = _studentServices.SaveQuizAnswer(student_id, question_id, correct_option, guid);

            if (resultMsg == "Updated Successfully!")
            {
                return Json("saved successfully" );
                //return Content("saved successfully");

            }
            else
            {
                return Json("an error occured. please try again" );

            }


        }


        [HttpPost]

        public IActionResult updateQuizRemainingTime(int quiz_ID, int remaining_minutes)
        {


            //--Data upate Part starts here
            int student_id = _SessionManag.GetLoginStudentFromSession().user_id;

            string? resultMsg = _studentServices.updateQuizRemainingTime(quiz_ID, student_id, remaining_minutes);

            if (resultMsg == "Updated Successfully!")
            {

                //return Content("saved successfully");
                return Json("saved successfully" );

            }
            else
            {

                return Json("an error occured. please try again" );
            }


        }

        [HttpGet]
        public JsonResult GetStudentsResult(int quiz_id)

        {
            int student_id = _SessionManag.GetLoginStudentFromSession().user_id;

            BasicDataModels model = new BasicDataModels();
            model.student_result_obj = new student_result();
            //---get status list
            model.student_result_obj = _studentServices.GetStudentsResultDetail(student_id, quiz_id);

            if (model.student_result_obj != null)
            {

                return Json(model.student_result_obj );


            }
            else
            {
                return Json(new { message = "No data found" } );
            }


        }

        [HttpGet]
        public IActionResult StudentAssignmentList(int? course_id,  string? title , int pageId=1)
        {
            course_id = course_id == null ? 0 : course_id;

            ViewBag.course_id = course_id;

            int? student_id = _SessionManag.GetLoginStudentFromSession() == null ? 0 : _SessionManag.GetLoginStudentFromSession().user_id;

            BasicDataModels model = new BasicDataModels();
            model.assignment_obj = new assignments();
            //--get quiz list by student id
            model.assignment_list = _studentServices.GetAssignmentListForStudentByCourseID(pageId, _constants.ITEMS_PER_PAGE(), student_id, course_id, title);

            model.pageHelperObj = new PagerHelper();
            int TotalRecords = model != null && model.assignment_list != null && model.assignment_list.Count() > 0 ? model.assignment_list.FirstOrDefault().TotalRecordCount : 0;
            model.pageHelperObj = PagerHelper.Instance.MakePaginationObject(model.assignment_list != null ? model.assignment_list.Count() : 0, TotalRecords, _constants.ITEMS_PER_PAGE(), pageId);




            return View(model);
        }

        [HttpGet]
        public IActionResult StudentAssignmentListPartial(int? course_id, string? title, int pageId = 1)
        {
            course_id = course_id == null ? 0 : course_id;

            ViewBag.course_id = course_id;

            int? student_id = _SessionManag.GetLoginStudentFromSession() == null ? 0 : _SessionManag.GetLoginStudentFromSession().user_id;

            BasicDataModels model = new BasicDataModels();
            model.assignment_obj = new assignments();
            //--get quiz list by student id
            model.assignment_list = _studentServices.GetAssignmentListForStudentByCourseID(pageId, _constants.ITEMS_PER_PAGE(), student_id, course_id, title);

            model.pageHelperObj = new PagerHelper();
            int TotalRecords = model != null && model.assignment_list != null && model.assignment_list.Count() > 0 ? model.assignment_list.FirstOrDefault().TotalRecordCount : 0;
            model.pageHelperObj = PagerHelper.Instance.MakePaginationObject(model.assignment_list != null ? model.assignment_list.Count() : 0, TotalRecords, _constants.ITEMS_PER_PAGE(), pageId);



            return PartialView("~/Areas/Users/Views/UserHome/_StudentAssignmentListPartial.cshtml", model);
        }

        [HttpGet]
        public JsonResult GetStudentsAssignmentResultByAssignmentID(int assignment_id)

        {
            int student_id = _SessionManag.GetLoginStudentFromSession().user_id;

            BasicDataModels model = new BasicDataModels();
            model.std_assignment_result_obj = new student_assignment_result();
            //---get status list
            model.std_assignment_result_obj = _studentServices.GetStudentAssignmentResultByAssignmentID(student_id, assignment_id);

            if (model.std_assignment_result_obj != null)
            {

                return Json(model.std_assignment_result_obj );


            }
            else
            {
                return Json(new { message = "No data found" } );
            }


        }


        [HttpGet]
        //[Route("home/start-assignment/{assignment_id}")]
        public IActionResult StartAssignment(int? assignment_id)
        {

            assignment_id = assignment_id == null ? -1 : assignment_id;
            int student_id = _SessionManag.GetLoginStudentFromSession().user_id;

            BasicDataModels model = new BasicDataModels();
            model.assignment_obj = new assignments();


            //--get queiz detail
            model.assignment_obj = _studentServices.GetAssignmentDetailForStudentByID(assignment_id, student_id);


            return View(model);
        }

        [HttpPost]

        public IActionResult updateAssignmentRemainingTime(int assignment_id, int remaining_minutes)
        {


            //--Data upate Part starts here
            int student_id = _SessionManag.GetLoginStudentFromSession().user_id;

            string? resultMsg = _studentServices.updateAssignmentRemainingTime(assignment_id, student_id, remaining_minutes);

            if (resultMsg == "Updated Successfully!")
            {


                return Json("saved successfully" );
            }
            else
            {

                return Json("an error occured. please try again" );
            }


        }

        //I have added here ValidateInput(false) bcz i am sending an html string? in assignment body
        [HttpPost]
        public IActionResult SaveStudentAssignmentAnswer(assignment_answers FormData)
        {

            FormData.ReturnURL = Request.Headers["Referer"].ToString();

            if (String.IsNullOrEmpty(FormData.student_ans) || FormData.assignment_id < 1)
            {
                TempData["ErrorMsg"] = "Please fill all required fields!";
                //--getting calling page URL
                return Redirect(FormData.ReturnURL);
            }

            //--Data Delete Part starts here
            FormData.created_by = _SessionManag.GetLoginStudentFromSession().user_id;
            FormData.guid = CommonHelper.Instance.GeneratNewGUID();
            FormData.resultMsg = _studentServices.SaveStudentAssignmentAnswer(FormData);

            if (FormData.resultMsg == "Saved Successfully!")
            {

                //--here return url coming from the startAssignment page
                TempData["SuccessMsg"] = FormData.resultMsg;
                return RedirectToAction("CoursesList", "UserHome", new { pageId = 1 });


            }
            else
            {

                TempData["ErrorMsg"] = FormData.resultMsg;
                return Redirect(FormData.ReturnURL);
            }

        }


    }
}
