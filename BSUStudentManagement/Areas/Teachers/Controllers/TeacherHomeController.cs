using BSUStudentManagement.CommonCode;
using DAL.CommonModels;
using DAL.Entities;
using DAL.Helpers.PageHelpers;
using DAL.Services;
using Microsoft.AspNetCore.Mvc;


namespace BSUStudentManagement.Areas.Teachers.Controllers
{
    [ServiceFilter(typeof(TeacherAuthorization))]
    [Area("Teachers")]
    //[Route("teacher/")]
    public class TeacherHomeController : Controller
    {
        private readonly IConstants _constants;
        private readonly ISessionManager _SessionManag;
        private readonly IAdminServices _adminServices;
        private readonly ITeacherServices _teacherServices;



        public TeacherHomeController(IConstants constants, ISessionManager sessionManag , IAdminServices adminServices, ITeacherServices teacherServices)
        {
            this._constants = constants;
            _SessionManag = sessionManag;
            this._adminServices = adminServices;
            this._teacherServices = teacherServices;
        }


        // GET: Teachers/TeacherHome
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult QuizList( int? course_id, int? category_id, string? title , int pageId=1)
        {
          
            int user_id = _SessionManag.GetLoginTeacherFromSession().user_id;


            BasicDataModels model = new BasicDataModels();
            model.quiz_obj = new quiz();

            //--get quiz list
            model.quiz_list = _teacherServices.GetQuizList(pageId, _constants.ITEMS_PER_PAGE(), course_id, category_id, title, user_id).ToList();

            model.pageHelperObj = new PagerHelper();
            int TotalRecords = model != null && model.quiz_list != null && model.quiz_list.Count() > 0 ? model.quiz_list.FirstOrDefault().TotalRecordCount : 0;
            model.pageHelperObj = PagerHelper.Instance.MakePaginationObject(model.quiz_list != null ? model.quiz_list.Count() : 0, TotalRecords, _constants.ITEMS_PER_PAGE(), pageId);


           

            //---get status list
            ViewBag.StatusList = _adminServices.GetStatusDropdownList().ToList();
            //--get course list for dropdwon GetCourseCategoriesDropDownList
            ViewBag.CoursesList = _teacherServices.GetCoursesListForTeacherDropDowns(user_id).ToList();
            //--get course list for dropdwon GetCourseCategoriesDropDownList
            ViewBag.QuizCategories = _adminServices.GetQuizCategoriesForDropDown().ToList();


            return View(model);
        }


        [HttpGet]
        public IActionResult QuizListPartial(int? course_id, int? category_id, string? title, int pageId = 1)
        {

            int user_id = _SessionManag.GetLoginTeacherFromSession().user_id;


            BasicDataModels model = new BasicDataModels();
            model.quiz_obj = new quiz();

            //--get quiz list
            model.quiz_list = _teacherServices.GetQuizList(pageId, _constants.ITEMS_PER_PAGE(), course_id, category_id, title, user_id).ToList();

            model.pageHelperObj = new PagerHelper();
            int TotalRecords = model != null && model.quiz_list != null && model.quiz_list.Count() > 0 ? model.quiz_list.FirstOrDefault().TotalRecordCount : 0;
            model.pageHelperObj = PagerHelper.Instance.MakePaginationObject(model.quiz_list != null ? model.quiz_list.Count() : 0, TotalRecords, _constants.ITEMS_PER_PAGE(), pageId);


            //---get status list
            ViewBag.StatusList = _adminServices.GetStatusDropdownList().ToList();
            //--get course list for dropdwon GetCourseCategoriesDropDownList
            ViewBag.CoursesList = _teacherServices.GetCoursesListForTeacherDropDowns(user_id).ToList();
            //--get course list for dropdwon GetCourseCategoriesDropDownList
            ViewBag.QuizCategories = _adminServices.GetQuizCategoriesForDropDown().ToList();


            return PartialView("~/Areas/Teachers/Views/TeacherHome/_QuizListPartial.cshtml", model);
        }



        [HttpGet]
       // [Route("quiz-detail/{quiz_id}")]
        public IActionResult QuizDetail(int? quiz_id)
        {
            quiz_id = quiz_id == null ? 2 : quiz_id;

            int created_by = _SessionManag.GetLoginTeacherFromSession().user_id;


            BasicDataModels model = new BasicDataModels();
            model.quiz_categories_obj = new quiz_categories();

            //--get queiz detail
            model.quiz_obj = _adminServices.GetQuizDetailByID(quiz_id);
            //--get question list by quiz_id
            model.quiz_ques_list = _teacherServices.GetQuestionListByQuizId(quiz_id).ToList();


            //---get status list
            ViewBag.StatusList = _adminServices.GetStatusDropdownList().ToList();
            //--get course list for dropdwon GetCourseCategoriesDropDownList
            ViewBag.CoursesList = _teacherServices.GetCoursesListForTeacherDropDowns(created_by).ToList();
            //--get course list for dropdwon GetCourseCategoriesDropDownList
            ViewBag.QuizCategories = _adminServices.GetQuizCategoriesForDropDown().ToList();




            return View(model);
        }


        [HttpGet]
        public IActionResult StudentResultList( int? quiz_id , int pageId=1)
        {
          
            quiz_id = quiz_id == null ? 0 : quiz_id;


            BasicDataModels model = new BasicDataModels();
            model.student_result_obj = new student_result();

            model.student_result_list = _teacherServices.GetStudentsResultList(pageId, _constants.ITEMS_PER_PAGE(), quiz_id).ToList();

            model.pageHelperObj = new PagerHelper();
            int TotalRecords = model != null && model.student_result_list != null && model.student_result_list.Count() > 0 ? model.student_result_list.FirstOrDefault().TotalRecordCount : 0;
            model.pageHelperObj = PagerHelper.Instance.MakePaginationObject(model.student_result_list != null ? model.student_result_list.Count() : 0, TotalRecords, _constants.ITEMS_PER_PAGE(), pageId);



            return PartialView(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditQuizDetail(quiz FormData)
        {
            //--getting calling page URL
            FormData.ReturnURL = Request.Headers["Referer"].ToString();

            if (String.IsNullOrEmpty(FormData.title) || FormData.status == null || String.IsNullOrEmpty(FormData.start_date) || String.IsNullOrEmpty(FormData.end_date) || FormData.allowed_time_minutes < 1 || FormData.category_id == null || FormData.course_id < 1 || String.IsNullOrEmpty(FormData.description) || FormData.passing_marks < 1)
            {
                TempData["ErrorMsg"] = "Please fill all required fields!";

                return Redirect(FormData.ReturnURL);
            }

            //--Data update Part starts here
            FormData.modified_by = _SessionManag.GetLoginTeacherFromSession().user_id;

            FormData.guid = CommonHelper.Instance.GeneratNewGUID();
            FormData.resultMsg = _teacherServices.UpdateQuiz(FormData);

            if (FormData.resultMsg == "Updated Successfully!")
            {
                TempData["SuccessMsg"] = "Updated Successfully!";
                return Redirect(FormData.ReturnURL);

            }
            else
            {
                TempData["ErrorMsg"] = FormData.resultMsg;
                return Redirect(FormData.ReturnURL);
            }

        }


        [HttpGet]
      //  [Route("add-quiz")]
        public IActionResult AddQuiz()
        {
            int created_by = _SessionManag.GetLoginTeacherFromSession().user_id;
            //---get status list
            ViewBag.StatusList = _adminServices.GetStatusDropdownList().ToList();

            //--get course list for dropdwon GetCourseCategoriesDropDownList
            ViewBag.CoursesList = _teacherServices.GetCoursesListForTeacherDropDowns(created_by).ToList();

            //--get course list for dropdwon GetCourseCategoriesDropDownList
            ViewBag.QuizCategories = _adminServices.GetQuizCategoriesForDropDown().ToList();

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddQuiz(quiz FormData)
        {

            //--getting calling page URL
            FormData.ReturnURL = Request.Headers["Referer"].ToString();

            if (String.IsNullOrEmpty(FormData.title) || FormData.status == null || String.IsNullOrEmpty(FormData.start_date) || String.IsNullOrEmpty(FormData.end_date) || FormData.allowed_time_minutes < 1 || FormData.category_id == null || FormData.course_id < 1 || String.IsNullOrEmpty(FormData.description) || FormData.passing_marks < 1)
            {
                TempData["ErrorMsg"] = "Please fill all required fields!";

                return Redirect(FormData.ReturnURL);
            }

            //--Data Delete Part starts here
            FormData.created_by = _SessionManag.GetLoginTeacherFromSession().user_id;
            FormData.guid = CommonHelper.Instance.GeneratNewGUID();
            FormData.resultMsg = _teacherServices.InsertNewQuiz(FormData);

            if (FormData.resultMsg == "Saved Successfully!")
            {
                FormData.quiz_id = _adminServices.GetQuizIdByGuid(FormData.guid);
                if (FormData.quiz_id < 1)
                {
                    TempData["ErrorMsg"] = "An error occured. Please try again";
                    return Redirect(FormData.ReturnURL);
                }
                else
                {
                    TempData["SuccessMsg"] = FormData.resultMsg;
                    return RedirectToAction("QuizDetail", "TeacherHome", new { quiz_id = FormData.quiz_id });
                }

            }
            else
            {
                TempData["ErrorMsg"] = FormData.resultMsg;
                return Redirect(FormData.ReturnURL);
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteQuiz(quiz FormData)
        {

            //--getting calling page URL
            FormData.ReturnURL = Request.Headers["Referer"].ToString();


            if (FormData.quiz_id < 1)
            {
                TempData["ErrorMsg"] = "Some thing went wrong. Please try again!";

                return Redirect(FormData.ReturnURL);
            }

            //--Data delete Part starts here
            FormData.resultMsg = _teacherServices.DeleteQuizDAL(FormData.quiz_id);

            if (FormData.resultMsg == "Deleted Successfully!")
            {

                TempData["SuccessMsg"] = "Deleted Successfully!";
                return Redirect(FormData.ReturnURL);


            }
            else
            {
                TempData["ErrorMsg"] = "Question not deleted. Please try again!";
                return Redirect(FormData.ReturnURL);
            }

        }

        [HttpGet]
       // [Route("student-result-detail/{quiz_id}/{student_id}")]
        public IActionResult StudentResultDetail(int? quiz_id, int? student_id)
        {
            quiz_id = quiz_id == null ? 0 : quiz_id;
            student_id = student_id == null ? 0 : student_id;

            BasicDataModels model = new BasicDataModels();
            model.quiz_categories_obj = new quiz_categories();

            //--get queiz detail
            model.student_result_obj = _teacherServices.GetStudentsResultByQuizID_StudentID(quiz_id, student_id);
            //--get question list by quiz_id
            model.quiz_ques_list = _teacherServices.GetQuestionListByQuizId_StudentID(quiz_id, student_id).ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddQuestion(quiz_questions FormData)
        {

            //--getting calling page URL
            FormData.ReturnURL = Request.Headers["Referer"].ToString();

            if (String.IsNullOrEmpty(FormData.question_title) || String.IsNullOrEmpty(FormData.option_a) || String.IsNullOrEmpty(FormData.option_b) || String.IsNullOrEmpty(FormData.option_c) || String.IsNullOrEmpty(FormData.option_d))
            {
                TempData["ErrorMsg"] = "Please fill all required fields!";

                return Redirect(FormData.ReturnURL);
            }

            if (FormData.quiz_id < 1)
            {
                TempData["ErrorMsg"] = "Quiz data is null. Please try again!";

                return Redirect(FormData.ReturnURL);
            }

            //--Data insert Part starts here
            FormData.created_by = _SessionManag.GetLoginTeacherFromSession().user_id;
            FormData.guid = CommonHelper.Instance.GeneratNewGUID();
            FormData.resultMsg = _teacherServices.InsertQuestion(FormData);

            if (FormData.resultMsg == "Saved Successfully!")
            {

                TempData["SuccessMsg"] = FormData.resultMsg;
                return Redirect(FormData.ReturnURL);


            }
            else
            {
                TempData["ErrorMsg"] = "Question not saved. Please try again!";
                return Redirect(FormData.ReturnURL);
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditQuestion(quiz_questions FormData)
        {

            //--getting calling page URL
            FormData.ReturnURL = Request.Headers["Referer"].ToString();

            if (String.IsNullOrEmpty(FormData.question_title) || String.IsNullOrEmpty(FormData.option_a) || String.IsNullOrEmpty(FormData.option_b) || String.IsNullOrEmpty(FormData.option_c) || String.IsNullOrEmpty(FormData.option_d))
            {
                TempData["ErrorMsg"] = "Please fill all required fields!";

                return Redirect(FormData.ReturnURL);
            }

            if (FormData.question_id < 1)
            {
                TempData["ErrorMsg"] = "Some thing went wrong. Please try again!";

                return Redirect(FormData.ReturnURL);
            }

            //--Data upate Part starts here
            FormData.modified_by = _SessionManag.GetLoginTeacherFromSession().user_id;
            FormData.resultMsg = _teacherServices.UdpateQuestion(FormData);

            if (FormData.resultMsg == "Updated Successfully!")
            {

                TempData["SuccessMsg"] = FormData.resultMsg;
                return Redirect(FormData.ReturnURL);


            }
            else
            {
                TempData["ErrorMsg"] = "Question not updated. Please try again!";
                return Redirect(FormData.ReturnURL);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteQuestion(quiz_questions FormData)
        {

            //--getting calling page URL
            FormData.ReturnURL = Request.Headers["Referer"].ToString();


            if (FormData.question_id < 1)
            {
                TempData["ErrorMsg"] = "Some thing went wrong. Please try again!";

                return Redirect(FormData.ReturnURL);
            }

            //--Data delete Part starts here
            FormData.resultMsg = _teacherServices.DeleteQuestion(FormData.question_id);

            if (FormData.resultMsg == "Deleted Successfully!")
            {

                TempData["SuccessMsg"] = "Deleted Successfully!";
                return Redirect(FormData.ReturnURL);


            }
            else
            {
                TempData["ErrorMsg"] = "Question not deleted. Please try again!";
                return Redirect(FormData.ReturnURL);
            }

        }

        [HttpGet]
        public IActionResult Courses( string? title, int? status, int? course_category_id , int pageId)
        {

            status = status == null || status == 0 ? 0 : status;
            course_category_id = course_category_id == null || course_category_id == 0 ? 0 : course_category_id;
            int user_id = _SessionManag.GetLoginTeacherFromSession().user_id;


            BasicDataModels model = new BasicDataModels();
            model.courses_obj = new courses();


            model.courses_list = _teacherServices.GetCoursesList(pageId, _constants.ITEMS_PER_PAGE(), title, status, course_category_id, user_id).ToList();

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
        public IActionResult CoursesPartial(string? title, int? status, int? course_category_id, int pageId)
        {

            status = status == null || status == 0 ? 0 : status;
            course_category_id = course_category_id == null || course_category_id == 0 ? 0 : course_category_id;
            int user_id = _SessionManag.GetLoginTeacherFromSession().user_id;


            BasicDataModels model = new BasicDataModels();
            model.courses_obj = new courses();


            model.courses_list = _teacherServices.GetCoursesList(pageId, _constants.ITEMS_PER_PAGE(), title, status, course_category_id, user_id).ToList();

            model.pageHelperObj = new PagerHelper();
            int TotalRecords = model != null && model.courses_list != null && model.courses_list.Count() > 0 ? model.courses_list.FirstOrDefault().TotalRecordCount : 0;
            model.pageHelperObj = PagerHelper.Instance.MakePaginationObject(model.courses_list != null ? model.courses_list.Count() : 0, TotalRecords, _constants.ITEMS_PER_PAGE(), pageId);



            //---get status list
            ViewBag.StatusList = _adminServices.GetStatusDropdownList().ToList();

            //--get course categories for dropdwon GetCourseCategoriesDropDownList
            ViewBag.CourseCategories = _adminServices.GetCourseCategoriesDropDownList().ToList();

            return PartialView("~/Areas/Teachers/Views/TeacherHome/_CoursesPartial.cshtml", model);
        }


        [HttpGet]
       
        public IActionResult StudentsList(string? user_name, string? email, int? student_id , int pageId=1)
        {
           
            student_id = student_id == null || student_id == 0 ? 0 : student_id;

            BasicDataModels model = new BasicDataModels();
            model.student_obj = new users();


            model.student_list = _teacherServices.GetStudentsListsForTeachers(pageId, _constants.ITEMS_PER_PAGE(), user_name, email, student_id).ToList();

            model.pageHelperObj = new PagerHelper();
            int TotalRecords = model != null && model.student_list != null && model.student_list.Count() > 0 ? model.student_list.FirstOrDefault().TotalRecordCount : 0;
            model.pageHelperObj = PagerHelper.Instance.MakePaginationObject(model.student_list != null ? model.student_list.Count() : 0, TotalRecords, _constants.ITEMS_PER_PAGE(), pageId);



            //---get status list
            ViewBag.StatusList = _adminServices.GetStatusDropdownList().ToList();

            //--get course categories for dropdwon GetCourseCategoriesDropDownList
            ViewBag.CourseCategories = _adminServices.GetCourseCategoriesDropDownList().ToList();


            return View(model);
        }


        [HttpGet]

        public IActionResult StudentsListPartial(string? user_name, string? email, int? student_id, int pageId = 1)
        {

            student_id = student_id == null || student_id == 0 ? 0 : student_id;

            BasicDataModels model = new BasicDataModels();
            model.student_obj = new users();


            model.student_list = _teacherServices.GetStudentsListsForTeachers(pageId, _constants.ITEMS_PER_PAGE(), user_name, email, student_id).ToList();

            model.pageHelperObj = new PagerHelper();
            int TotalRecords = model != null && model.student_list != null && model.student_list.Count() > 0 ? model.student_list.FirstOrDefault().TotalRecordCount : 0;
            model.pageHelperObj = PagerHelper.Instance.MakePaginationObject(model.student_list != null ? model.student_list.Count() : 0, TotalRecords, _constants.ITEMS_PER_PAGE(), pageId);



            //---get status list
            ViewBag.StatusList = _adminServices.GetStatusDropdownList().ToList();

            //--get course categories for dropdwon GetCourseCategoriesDropDownList
            ViewBag.CourseCategories = _adminServices.GetCourseCategoriesDropDownList().ToList();

            return PartialView("~/Areas/Teachers/Views/TeacherHome/_StudentsListPartial.cshtml", model);
        }



        [HttpGet]
        public IActionResult AssignmentList(int? course_id, string? title , int pageId=1)
        {

            int user_id = _SessionManag.GetLoginTeacherFromSession().user_id;


            BasicDataModels model = new BasicDataModels();
            model.assignment_obj = new assignments();

            //--get quiz list
            model.assignment_list = _teacherServices.GetAssignmentListForTeachers(pageId, _constants.ITEMS_PER_PAGE(), course_id, title, user_id).ToList();

            model.pageHelperObj = new PagerHelper();
            int TotalRecords = model != null && model.assignment_list != null && model.assignment_list.Count() > 0 ? model.assignment_list.FirstOrDefault().TotalRecordCount : 0;
            model.pageHelperObj = PagerHelper.Instance.MakePaginationObject(model.assignment_list != null ? model.assignment_list.Count() : 0, TotalRecords, _constants.ITEMS_PER_PAGE(), pageId);




            //---get status list
            ViewBag.StatusList = _adminServices.GetStatusDropdownList().ToList();
            //--get course list for dropdwon GetCourseCategoriesDropDownList
            ViewBag.CoursesList = _teacherServices.GetCoursesListForTeacherDropDowns(user_id).ToList();


            return View(model);
        }


        [HttpGet]
        public IActionResult AssignmentListPartial(int? course_id, string? title, int pageId = 1)
        {

            int user_id = _SessionManag.GetLoginTeacherFromSession().user_id;


            BasicDataModels model = new BasicDataModels();
            model.assignment_obj = new assignments();

            //--get quiz list
            model.assignment_list = _teacherServices.GetAssignmentListForTeachers(pageId, _constants.ITEMS_PER_PAGE(), course_id, title, user_id).ToList();

            model.pageHelperObj = new PagerHelper();
            int TotalRecords = model != null && model.assignment_list != null && model.assignment_list.Count() > 0 ? model.assignment_list.FirstOrDefault().TotalRecordCount : 0;
            model.pageHelperObj = PagerHelper.Instance.MakePaginationObject(model.assignment_list != null ? model.assignment_list.Count() : 0, TotalRecords, _constants.ITEMS_PER_PAGE(), pageId);




            //---get status list
            ViewBag.StatusList = _adminServices.GetStatusDropdownList().ToList();
            //--get course list for dropdwon GetCourseCategoriesDropDownList
            ViewBag.CoursesList = _teacherServices.GetCoursesListForTeacherDropDowns(user_id).ToList();


            return PartialView("~/Areas/Teachers/Views/TeacherHome/_AssignmentListPartial.cshtml", model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteAssignment(assignments FormData)
        {

            //--getting calling page URL
            FormData.ReturnURL = Request.Headers["Referer"].ToString();


            if (FormData.assignment_id < 1)
            {
                TempData["ErrorMsg"] = "Some thing went wrong. Please try again!";

                return Redirect(FormData.ReturnURL);
            }

            //--Data delete Part starts here
            FormData.resultMsg = _teacherServices.DeleteAssignment(FormData.assignment_id);

            if (FormData.resultMsg == "Deleted Successfully!")
            {

                TempData["SuccessMsg"] = "Deleted Successfully!";
                return Redirect(FormData.ReturnURL);


            }
            else
            {
                TempData["ErrorMsg"] = "Question not deleted. Please try again!";
                return Redirect(FormData.ReturnURL);
            }

        }


        [HttpGet]
       // [Route("add-assignment")]
        public IActionResult AddAssignment()
        {
            int created_by = _SessionManag.GetLoginTeacherFromSession().user_id;
            //---get status list
            ViewBag.StatusList = _adminServices.GetStatusDropdownList().ToList();

            //--get course list for dropdwon GetCourseCategoriesDropDownList
            ViewBag.CoursesList = _teacherServices.GetCoursesListForTeacherDropDowns(created_by).ToList();



            return View();
        }


        //I have added here ValidateInput(false) bcz i am sending an html string? in assignment body
        [HttpPost]
        public IActionResult AddAssignment(assignments FormData)
        {

            //--getting calling page URL
            FormData.ReturnURL = Request.Headers["Referer"].ToString();

            if (String.IsNullOrEmpty(FormData.title) || FormData.status == null || String.IsNullOrEmpty(FormData.start_date) || String.IsNullOrEmpty(FormData.end_date) || FormData.allowed_time_minutes < 1 || FormData.course_id < 1 || FormData.passing_marks < 1 || FormData.total_marks < 1 || String.IsNullOrEmpty(FormData.body))
            {
                TempData["ErrorMsg"] = "Please fill all required fields!";

                return Redirect(FormData.ReturnURL);
            }

            //--Data Delete Part starts here
            FormData.created_by = _SessionManag.GetLoginTeacherFromSession().user_id;
            FormData.guid = CommonHelper.Instance.GeneratNewGUID();
            FormData.resultMsg = _teacherServices.InsertNewAssignment(FormData);

            if (FormData.resultMsg == "Saved Successfully!")
            {
                FormData.assignment_id = _teacherServices.GetAssignmentIdByGuid(FormData.guid);
                if (FormData.assignment_id < 1)
                {
                    TempData["ErrorMsg"] = "An error occured. Please try again";
                    return Redirect(FormData.ReturnURL);
                }
                else
                {
                    TempData["SuccessMsg"] = FormData.resultMsg;
                    return RedirectToAction("AssignmentDetail", "TeacherHome", new { assignment_id = FormData.assignment_id });
                }

            }
            else
            {
                TempData["ErrorMsg"] = FormData.resultMsg;
                return Redirect(FormData.ReturnURL);
            }

        }


        [HttpGet]
      //  [Route("assignment-detail/{assignment_id}")]
        public IActionResult AssignmentDetail(int? assignment_id)
        {
            assignment_id = assignment_id == null ? 2 : assignment_id;

            int user_id = _SessionManag.GetLoginTeacherFromSession().user_id;


            BasicDataModels model = new BasicDataModels();
            model.assignment_obj = new assignments();

            //--get queiz detail
            model.assignment_obj = _teacherServices.GetAssignmentDetailByID(assignment_id);

            //---get status list
            ViewBag.StatusList = _adminServices.GetStatusDropdownList().ToList();
            //--get course list for dropdwon GetCourseCategoriesDropDownList
            ViewBag.CoursesList = _teacherServices.GetCoursesListForTeacherDropDowns(user_id).ToList();

            return View(model);
        }

        //I have added here ValidateInput(false) bcz i am sending an html string? in assignment body
        [HttpPost]
        public IActionResult EditAssignmentDetail(assignments FormData)
        {
            //--getting calling page URL
            FormData.ReturnURL = Request.Headers["Referer"].ToString();

            if (String.IsNullOrEmpty(FormData.title) || FormData.status == null || String.IsNullOrEmpty(FormData.start_date) || String.IsNullOrEmpty(FormData.end_date) || FormData.allowed_time_minutes < 1 || String.IsNullOrEmpty(FormData.body) || FormData.course_id < 1 || FormData.passing_marks < 1)
            {
                TempData["ErrorMsg"] = "Please fill all required fields!";

                return Redirect(FormData.ReturnURL);
            }

            //--Data update Part starts here
            FormData.modified_by = _SessionManag.GetLoginTeacherFromSession().user_id;

            FormData.guid = CommonHelper.Instance.GeneratNewGUID();
            FormData.resultMsg = _teacherServices.UpdateAssignment(FormData);

            if (FormData.resultMsg == "Updated Successfully!")
            {
                TempData["SuccessMsg"] = "Updated Successfully!";
                return Redirect(FormData.ReturnURL);

            }
            else
            {
                TempData["ErrorMsg"] = FormData.resultMsg;
                return Redirect(FormData.ReturnURL);
            }

        }


        [HttpGet]
        public IActionResult StudentAssignmentResultList(int? assignment_id , int pageId=1)
        {

            assignment_id = assignment_id == null ? 0 : assignment_id;


            BasicDataModels model = new BasicDataModels();
            model.std_assignment_result_obj = new student_assignment_result();

            model.std_assignment_result_list = _teacherServices.GetStudentsAssignmentResultList(pageId, 100, assignment_id).ToList();
           
            model.pageHelperObj = new PagerHelper();
            int TotalRecords = model != null && model.std_assignment_result_list != null && model.std_assignment_result_list.Count() > 0 ? model.std_assignment_result_list.FirstOrDefault().TotalRecordCount : 0;
            model.pageHelperObj = PagerHelper.Instance.MakePaginationObject(model.std_assignment_result_list != null ? model.std_assignment_result_list.Count() : 0, TotalRecords, 100, pageId);



            return PartialView(model);
        }

        [HttpGet]
      //  [Route("assignment-result-detail/{assign_answers_id}/{student_id")]
        public IActionResult StudentAssignmentResultDetail(int? assign_answers_id, int? student_id)
        {
            assign_answers_id = assign_answers_id == null ? -1 : assign_answers_id;
            student_id = student_id == null ? -1 : student_id;


            BasicDataModels model = new BasicDataModels();
            model.assignment_obj = new assignments();


            //--get queiz detail
            model.assignment_obj = _teacherServices.GetAssignmentDetailForStudentForTeacher(assign_answers_id, student_id);


            return View(model);
        }

        [HttpPost]
        public IActionResult GiveAssignmentMarkToStudent(assignment_answers FormData)
        {
            BasicDataModels model = new BasicDataModels();

            //--getting calling page URL
            FormData.ReturnURL = Request.Headers["Referer"].ToString();

            if (FormData.assign_answers_id < 1 || FormData.gain_marks < 1)
            {
                TempData["ErrorMsg"] = "Please fill all required fields!";
                FormData.ErrorMsg = "Please fill all required fields!";
                return Redirect(FormData.ReturnURL);
            }

            //--Data Insert Part starts here
            FormData.created_by = _SessionManag.GetLoginTeacherFromSession().user_id;
            FormData.ErrorMsg = _teacherServices.GiveAssignmentMarksToStudent(FormData);
            if (FormData.ErrorMsg == "Saved Successfully!")
            {
                TempData["SuccessMsg"] = FormData.ErrorMsg;
                return Redirect(FormData.ReturnURL);
            }
            else
            {
                TempData["ErrorMsg"] = FormData.ErrorMsg;
                return Redirect(FormData.ReturnURL);
            }


        }

        [HttpGet]
        public IActionResult StudentListInCourses(int? course_id, string? user_name, int? student_id , int pageId=1)
        {
            pageId = pageId == null || pageId == 0 ? 1 : pageId;
            student_id = student_id == null || student_id == 0 ? 0 : student_id;
            course_id = course_id == null || course_id == 0 ? 0 : course_id;
            ViewBag.course_id = course_id;

            int user_id = _SessionManag.GetLoginTeacherFromSession().user_id;


            BasicDataModels model = new BasicDataModels();
            model.student_courses_obj = new student_courses();


            model.student_courses_list = _teacherServices.GetStudentsListForCourse(pageId, _constants.ITEMS_PER_PAGE(), course_id, user_name, student_id).ToList();

            model.pageHelperObj = new PagerHelper();
            int TotalRecords = model != null && model.student_courses_list != null && model.student_courses_list.Count() > 0 ? model.student_courses_list.FirstOrDefault().TotalRecordCount : 0;
            model.pageHelperObj = PagerHelper.Instance.MakePaginationObject(model.student_courses_list != null ? model.student_courses_list.Count() : 0, TotalRecords, _constants.ITEMS_PER_PAGE(), pageId);




            return View(model);
        }

        [HttpGet]
        public IActionResult StudentListInCoursesPartial(int? course_id, string? user_name, int? student_id, int pageId = 1)
        {
            pageId = pageId == null || pageId == 0 ? 1 : pageId;
            student_id = student_id == null || student_id == 0 ? 0 : student_id;
            course_id = course_id == null || course_id == 0 ? 0 : course_id;
            ViewBag.course_id = course_id;

            int user_id = _SessionManag.GetLoginTeacherFromSession().user_id;


            BasicDataModels model = new BasicDataModels();
            model.student_courses_obj = new student_courses();


            model.student_courses_list = _teacherServices.GetStudentsListForCourse(pageId, _constants.ITEMS_PER_PAGE(), course_id, user_name, student_id).ToList();

            model.pageHelperObj = new PagerHelper();
            int TotalRecords = model != null && model.student_courses_list != null && model.student_courses_list.Count() > 0 ? model.student_courses_list.FirstOrDefault().TotalRecordCount : 0;
            model.pageHelperObj = PagerHelper.Instance.MakePaginationObject(model.student_courses_list != null ? model.student_courses_list.Count() : 0, TotalRecords, _constants.ITEMS_PER_PAGE(), pageId);



           
                return PartialView("~/Areas/Teachers/Views/TeacherHome/_StudentListInCoursesPartial.cshtml", model);
          
        }
    }
}
