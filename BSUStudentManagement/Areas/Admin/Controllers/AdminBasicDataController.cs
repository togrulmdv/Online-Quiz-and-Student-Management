using BSUStudentManagement.CommonCode;
using DAL.CommonModels;
using DAL.Entities;
using DAL.Helpers.PageHelpers;
using DAL.Services;
using Microsoft.AspNetCore.Mvc;


namespace BSUStudentManagement.Areas.Admin.Controllers
{
    [ServiceFilter(typeof(AdminAuthorization))]
    [Area("Admin")]
    //[Route("admin")]
    public class AdminBasicDataController : Controller
    {

        private readonly IConstants _constants;
        private readonly ISessionManager _SessionManag;
        private readonly IHttpContextAccessor _contx;
        private readonly IAdminServices _adminServices;

        public AdminBasicDataController(IConstants constants, ISessionManager sessionManag, IHttpContextAccessor contx , IAdminServices adminServices)
        {
            this._constants = constants;
            _SessionManag = sessionManag;
            this._contx = contx;
            this._adminServices = adminServices;

        }


        [HttpGet]
        public ActionResult QuizCategories(string? category_name, string? is_active , int pageId = 1)
        {

            BasicDataModels model = new BasicDataModels();
            model.quiz_categories_obj = new quiz_categories();

            model.quiz_categories_list = _adminServices.GetQuizCategoriesList(pageId, _constants.ITEMS_PER_PAGE(), category_name, is_active).ToList();
           
            model.pageHelperObj = new PagerHelper();
            int TotalRecords = model!=null && model.quiz_categories_list != null && model.quiz_categories_list.Count() > 0  ? model.quiz_categories_list.FirstOrDefault().TotalRecordCount : 0;
            model.pageHelperObj = PagerHelper.Instance.MakePaginationObject(model.quiz_categories_list != null ? model.quiz_categories_list.Count() : 0, TotalRecords, _constants.ITEMS_PER_PAGE(), pageId);


            return View(model);
        }


        [HttpGet]
        public ActionResult QuizCategoriesPartial(string? category_name, string? is_active, int pageId = 1)
        {

            BasicDataModels model = new BasicDataModels();
            model.quiz_categories_obj = new quiz_categories();

            model.quiz_categories_list = _adminServices.GetQuizCategoriesList(pageId, _constants.ITEMS_PER_PAGE(), category_name, is_active).ToList();

            model.pageHelperObj = new PagerHelper();
            int TotalRecords = model != null && model.quiz_categories_list != null && model.quiz_categories_list.Count() > 0 ? model.quiz_categories_list.FirstOrDefault().TotalRecordCount : 0;
            model.pageHelperObj = PagerHelper.Instance.MakePaginationObject(model.quiz_categories_list != null ? model.quiz_categories_list.Count() : 0, TotalRecords, _constants.ITEMS_PER_PAGE(), pageId);

            return PartialView("~/Areas/Admin/Views/AdminBasicData/_QuizCategoriesPartial.cshtml", model);
        }




        [HttpPost]
        public ActionResult QuizCategories(quiz_categories FormData)
        {
            BasicDataModels model = new BasicDataModels();

            //--getting calling page URL
            FormData.ReturnURL = Request.Headers["Referer"].ToString();

            if (String.IsNullOrEmpty(FormData.category_name))
            {
                TempData["ErrorMsg"] = "Please fill all required fields!";
                FormData.ErrorMsg = "Please fill all required fields!";
                return Redirect(FormData.ReturnURL);
            }

            //--Data Insert Part starts here
            FormData.created_by = _SessionManag.GetLoginAdminFromSession().user_id;
            FormData.ErrorMsg = _adminServices.InsertQuizCategory(FormData);
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

            return Redirect(FormData.ReturnURL);
        }

        [HttpPost]
        public ActionResult QuizCategoriesEdit(quiz_categories FormData)
        {


            //--getting calling page URL
            FormData.ReturnURL = Request.Headers["Referer"].ToString();

            if (String.IsNullOrEmpty(FormData.category_name) || FormData.category_id < 1)
            {
                TempData["ErrorMsg"] = "Please fill all required fields!";

                return Redirect(FormData.ReturnURL);
            }

            //--Data Insert Part starts here
            FormData.modified_by = _SessionManag.GetLoginAdminFromSession().user_id;
            FormData.ErrorMsg = _adminServices.UpdateQuizCategory(FormData);
            if (FormData.ErrorMsg == "Updated Successfully!")
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


        [HttpPost]
        public ActionResult DeleteQuizCategories(quiz_categories FormData)
        {


            //--getting calling page URL
            FormData.ReturnURL = Request.Headers["Referer"].ToString();

            if (FormData.category_id < 1)
            {
                TempData["ErrorMsg"] = "Some thing went wrong. Please try again!";

                return Redirect(FormData.ReturnURL);
            }

            //--Data Delete Part starts here
            FormData.ErrorMsg = _adminServices.DeleteQuizCategory(FormData);

            if (FormData.ErrorMsg == "Deleted Successfully!")
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
       // [Route("course/categories/{pageId}")]
        public ActionResult CourseCategories( string? title , int pageId=1)
        {

            BasicDataModels model = new BasicDataModels();
            model.course_categories_obj = new course_categories();

            model.course_categories_list = _adminServices.GetCourseCategoriesList(pageId, _constants.ITEMS_PER_PAGE(), title).ToList();

            model.pageHelperObj = new PagerHelper();
            int TotalRecords = model != null && model.course_categories_list != null && model.course_categories_list.Count() > 0 ? model.course_categories_list.FirstOrDefault().TotalRecordCount : 0;
            model.pageHelperObj = PagerHelper.Instance.MakePaginationObject(model.course_categories_list != null ? model.course_categories_list.Count() : 0, TotalRecords, _constants.ITEMS_PER_PAGE(), pageId);

            return View(model);
        }


        [HttpGet]
        // [Route("course/categories/{pageId}")]
        public ActionResult CourseCategoriesPartial(string? title, int pageId = 1)
        {

            BasicDataModels model = new BasicDataModels();
            model.course_categories_obj = new course_categories();

            model.course_categories_list = _adminServices.GetCourseCategoriesList(pageId, _constants.ITEMS_PER_PAGE(), title).ToList();

            model.pageHelperObj = new PagerHelper();
            int TotalRecords = model != null && model.course_categories_list != null && model.course_categories_list.Count() > 0 ? model.course_categories_list.FirstOrDefault().TotalRecordCount : 0;
            model.pageHelperObj = PagerHelper.Instance.MakePaginationObject(model.course_categories_list != null ? model.course_categories_list.Count() : 0, TotalRecords, _constants.ITEMS_PER_PAGE(), pageId);

            return PartialView("~/Areas/Admin/Views/AdminBasicData/_CourseCategoriesPartial.cshtml", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CourseCategories(course_categories FormData)
        {
            BasicDataModels model = new BasicDataModels();

            //--getting calling page URL
            FormData.ReturnURL = Request.Headers["Referer"].ToString();

            if (String.IsNullOrEmpty(FormData.title))
            {
                TempData["ErrorMsg"] = "Please fill all required fields!";
                FormData.ErrorMsg = "Please fill all required fields!";

                model.course_categories_obj = FormData;
                // return View(model);
                return Redirect(FormData.ReturnURL);
            }

            //--Data Insert Part starts here
            FormData.created_by = _SessionManag.GetLoginAdminFromSession().user_id;
            FormData.guid = CommonHelper.Instance.GeneratNewGUID();
            FormData.ErrorMsg = _adminServices.InsertCourseCategory(FormData);
            if (FormData.ErrorMsg == "Saved Successfully!")
            {
                TempData["SuccessMsg"] = FormData.ErrorMsg;
            }
            else
            {
                TempData["ErrorMsg"] = FormData.ErrorMsg;
            }

            model.course_categories_obj = FormData;

            //return View(model);
            return Redirect(FormData.ReturnURL);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCourseCategory(course_categories FormData)
        {


            //--getting calling page URL
            FormData.ReturnURL = Request.Headers["Referer"].ToString();

            if (String.IsNullOrEmpty(FormData.title) || FormData.course_category_id < 1)
            {
                TempData["ErrorMsg"] = "Please fill all required fields!";

                return Redirect(FormData.ReturnURL);
            }

            //--Data Insert Part starts here
            FormData.modified_by = _SessionManag.GetLoginAdminFromSession().user_id;
            FormData.ErrorMsg = _adminServices.UpdateCourseCategory(FormData);
            if (FormData.ErrorMsg == "Updated Successfully!")
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


        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult DeleteCourseCategory(course_categories FormData)
        {


            //--getting calling page URL
            FormData.ReturnURL = Request.Headers["Referer"].ToString();

            if (FormData.course_category_id < 1)
            {
                TempData["ErrorMsg"] = "Some thing went wrong. Please try again!";

                return Redirect(FormData.ReturnURL);
            }

            //--Data Delete Part starts here
            FormData.ErrorMsg = _adminServices.DeleteCourseCategory(FormData);

            if (FormData.ErrorMsg == "Deleted Successfully!")
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





    }
}
