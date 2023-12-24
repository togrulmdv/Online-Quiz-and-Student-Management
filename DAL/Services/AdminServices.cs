using DAL.ConnectionManager;
using DAL.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PetaPoco;
using System.Data;
using DAL.CommonModels;
using DAL.Helpers.Enums;

namespace DAL.Services
{


    public class AdminServices : IAdminServices
    {
        private readonly IConfiguration _configuration;
        private readonly IDatabase _db;
        private readonly IDataContextHelper _contextHelper;

        public AdminServices(IConfiguration configuration, IDatabase db, IDataContextHelper contextHelper)
        {
            _configuration = configuration;
            _db = db;
            _contextHelper = contextHelper;
        }

        public void LogErrorInDatabase(string error_message, string? log_type)
        {
            string? result = "";

            using (var repo = _contextHelper.GetPPContextHelper())
            {

                try
                {
                    if (!String.IsNullOrEmpty(error_message))
                    {
                        //--First parameter is table name. 2nd par is table primary key. 3rd par is object that contains data.
                        //repo.Insert("quiz_categories", "category_id", FormData);

                        var response = repo.Execute(" insert into error_log(log_message, log_type, created_at)values(@0, @1, getdate() );", error_message, log_type);

                    }

                }
                catch (Exception ex)
                {
                    string? errorMessage = ex.Message;

                    result = errorMessage;
                }


            }



        }


        public string? InsertQuizCategory(quiz_categories FormData)
        {
            string? result = "";
            using (var repo = _contextHelper.GetPPContextHelper())
            {

                try
                {
                    if (FormData != null)
                    {
                        //--First parameter is table name. 2nd par is table primary key. 3rd par is object that contains data.
                        //repo.Insert("quiz_categories", "category_id", FormData);

                        var response = repo.Execute("INSERT INTO quiz_categories (category_name,is_active, created_at, created_by, is_deleted) VALUES (@0, @1, getdate(), @2,0)", FormData.category_name, FormData.is_active, FormData.created_by);
                        if (response > 0)
                        {
                            result = "Saved Successfully!";
                        }
                        else
                        {
                            result = "Error occured in processing your request!";
                        }

                    }
                    else
                    {
                        result = "Please fill all required fields!";
                    }
                }
                catch (Exception ex)
                {
                    LogErrorInDatabase(ex.Message, "DAL Operation error");

                    result = "Error occured in processing your request!";


                    return result;
                }


            }
            return result;


        }

        public string? InsertCourseCategory(course_categories FormData)
        {
            string? result = "";
            using (var repo = _contextHelper.GetPPContextHelper())
            {

                try
                {
                    if (FormData != null)
                    {
                        //--First parameter is table name. 2nd par is table primary key. 3rd par is object that contains data.
                        //repo.Insert("quiz_categories", "category_id", FormData);

                        var response = repo.Execute(" insert into course_categories(title, status, guid, created_at, created_by, is_deleted)values(@0, @1, @2, getdate(), @3, 0 );", FormData.title, FormData.status, FormData.guid, FormData.created_by);
                        if (response > 0)
                        {
                            result = "Saved Successfully!";
                        }
                        else
                        {
                            result = "Error occured in processing your request!";
                        }

                    }
                    else
                    {
                        result = "Please fill all required fields!";
                    }
                }
                catch (Exception ex)
                {
                    LogErrorInDatabase(ex.Message, "DAL Operation error");
                    result = "Error occured in processing your request!";
                    return result;
                }


            }
            return result;


        }


        public string? UpdateQuizCategory(quiz_categories FormData)
        {
            string? result = "";
            using (var repo = _contextHelper.GetPPContextHelper())
            {

                try
                {
                    if (FormData != null)
                    {
                        //--First parameter is table name. 2nd par is table primary key. 3rd par is object that contains data.
                        //repo.Insert("quiz_categories", "category_id", FormData);

                        var response = repo.Execute(@"update quiz_categories set category_name=@0, is_active=@1, modified_at=getdate(), modified_by=@2 where category_id=@3", FormData.category_name, FormData.is_active, FormData.modified_by, FormData.category_id);
                        if (response > 0)
                        {
                            result = "Updated Successfully!";
                        }
                        else
                        {
                            result = "Error occured in processing your request!";
                        }

                    }
                    else
                    {
                        result = "Please fill all required fields!";
                    }
                }
                catch (Exception ex)
                {
                    LogErrorInDatabase(ex.Message, "DAL Operation error");
                    result = "Error occured in processing your request!";
                    return result;
                }


            }
            return result;


        }


        public string? UpdateCourseCategory(course_categories FormData)
        {
            string? result = "";
            using (var repo = _contextHelper.GetPPContextHelper())
            {

                try
                {
                    if (FormData != null)
                    {
                        //--First parameter is table name. 2nd par is table primary key. 3rd par is object that contains data.
                        //repo.Insert("quiz_categories", "category_id", FormData);

                        var response = repo.Execute(@"update course_categories set title=@0, status=@1, modified_at=getdate(), modified_by=@2 where course_category_id=@3", FormData.title, FormData.status, FormData.modified_by, FormData.course_category_id);
                        if (response > 0)
                        {
                            result = "Updated Successfully!";
                        }
                        else
                        {
                            result = "Error occured in processing your request!";
                        }

                    }
                    else
                    {
                        result = "Please fill all required fields!";
                    }
                }
                catch (Exception ex)
                {
                    LogErrorInDatabase(ex.Message, "DAL Operation error");
                    result = "Error occured in processing your request!";
                    return result;
                }


            }
            return result;


        }


        public string? DeleteQuizCategory(quiz_categories FormData)
        {
            string? result = "";
            using (var repo = _contextHelper.GetPPContextHelper())
            {

                try
                {
                    if (FormData != null)
                    {
                        //--First parameter is table name. 2nd par is table primary key. 3rd par is object that contains data.
                        //repo.Insert("quiz_categories", "category_id", FormData);

                        var response = repo.Execute(@"Delete from quiz_categories   where  category_id=@0", FormData.category_id);
                        if (response > 0)
                        {
                            result = "Deleted Successfully!";
                        }
                        else
                        {
                            result = "Error occured in deleting the record. Please try again!";
                        }

                    }
                    else
                    {
                        result = "Error occured in deleting the record. Please try again!";
                    }
                }
                catch (Exception ex)
                {
                    LogErrorInDatabase(ex.Message, "DAL Operation error");
                    result = "Error occured in deleting the record. Please try again!";
                    return result;
                }


            }
            return result;


        }

        public string? DeleteCourseCategory(course_categories FormData)
        {
            string? result = "";
            using (var repo = _contextHelper.GetPPContextHelper())
            {

                try
                {
                    if (FormData != null)
                    {

                        var response = repo.Execute(@"Delete from course_categories   where  course_category_id=@0", FormData.course_category_id);
                        if (response > 0)
                        {
                            result = "Deleted Successfully!";
                        }
                        else
                        {
                            result = "Error occured in deleting the record. Please try again!";
                        }

                    }
                    else
                    {
                        result = "Error occured in deleting the record. Please try again!";
                    }
                }
                catch (Exception ex)
                {
                    LogErrorInDatabase(ex.Message, "DAL Operation error");
                    result = "Error occured in deleting the record. Please try again!";
                    return result;
                }


            }
            return result;


        }


        public List<quiz_categories> GetQuizCategoriesList(int? pageId, int pageSize, string? category_name, string? is_active)
        {
            List<quiz_categories> result = new List<quiz_categories>();

            if (result == null || result.Count < 1)
            {
                using (var repo = _contextHelper.GetPPContextHelper())
                {
                    try
                    {


                        #region Create Search Parameters
                        var SearchParameters = PetaPoco.Sql.Builder.Append(" ");

                        if (!String.IsNullOrEmpty(category_name))
                        {
                            SearchParameters.Append("and a.category_name LIKE  @0 ", "%" + category_name + "%");
                        }

                        if (!String.IsNullOrEmpty(is_active))
                        {
                            SearchParameters.Append("and a.is_active = @0 ", is_active);
                        }


                        #endregion

                        var ppSql = PetaPoco.Sql.Builder.Select(@"distinct COUNT(*) OVER () as TotalRecordCount,  a.category_id, a.category_name, a.is_active, a.created_at, a.created_by, t.user_name as created_by_username")
                            .From("quiz_categories a")
                            .LeftJoin("users t").On("a.created_by = t.user_id")

                            .Where("a.is_deleted = 0 ")
                            .Append(SearchParameters)
                            .OrderBy("a.category_id DESC")
                           .Append(@"OFFSET (@0-1)*@1 ROWS
	                        FETCH NEXT @1 ROWS ONLY", pageId, pageSize);


                        result = repo.Query<quiz_categories>(ppSql).ToList();


                    }
                    catch (Exception ex)
                    {
                        LogErrorInDatabase(ex.Message, "DAL Operation error");
                        return (result);
                    }


                }
            }
            return (result);

        }

        public List<course_categories> GetCourseCategoriesList(int? pageId, int pageSize, string? title)
        {
            List<course_categories> result = new List<course_categories>();

            if (result == null || result.Count < 1)
            {
                using (var repo = _contextHelper.GetPPContextHelper())
                {
                    try
                    {


                        #region Create Search Parameters
                        var SearchParameters = PetaPoco.Sql.Builder.Append(" ");

                        if (!String.IsNullOrEmpty(title))
                        {
                            SearchParameters.Append("and a.title LIKE  @0 ", "%" + title + "%");
                        }


                        #endregion

                        var ppSql = PetaPoco.Sql.Builder.Select(@"distinct COUNT(*) OVER () as TotalRecordCount,  a.course_category_id, a.title, a.status, a.created_at, a.created_by, t.user_name as created_by_username")
                            .From("course_categories a")
                            .LeftJoin("users t").On("a.created_by = t.user_id")

                            .Where("a.is_deleted = 0 ")
                            .Append(SearchParameters)
                            .OrderBy("a.course_category_id DESC")
                           .Append(@"OFFSET (@0-1)*@1 ROWS
	                        FETCH NEXT @1 ROWS ONLY", pageId, pageSize);


                        result = repo.Query<course_categories>(ppSql).ToList();


                    }
                    catch (Exception ex)
                    {
                        LogErrorInDatabase(ex.Message, "DAL Operation error");
                        return (result);
                    }


                }
            }
            return (result);

        }


        public int GetQuizIdByGuid(string guid)
        {
            quiz result = new quiz();
            int quiz_id = 0;

            using (var repo = _contextHelper.GetPPContextHelper())
            {
                try
                {




                    var ppSql = PetaPoco.Sql.Builder.Select(@" top 1  quiz_id")
                        .From("quiz")

                        .Where("guid=@0 and is_deleted!=1", guid)
                        .OrderBy("quiz_id desc");



                    result = repo.Query<quiz>(ppSql).FirstOrDefault();
                    quiz_id = result != null ? result.quiz_id : 0;
                }
                catch (Exception ex)
                {
                    LogErrorInDatabase(ex.Message, "DAL Operation error");
                    return quiz_id;
                }


            }

            return quiz_id;

        }

        public quiz GetQuizDetailByID(int? quiz_id)
        {
            quiz result = new quiz();


            using (var repo = _contextHelper.GetPPContextHelper())
            {
                try
                {




                    var ppSql = PetaPoco.Sql.Builder.Select(@"top 1   qz.quiz_id, qz.title,convert(varchar(30),qz.start_date,22) as start_date,convert(varchar(30),qz.end_date,22) as end_date, IIF(rt.remaining_minutes>-1, rt.remaining_minutes, qz.allowed_time_minutes) as allowed_time_minutes, qz.status, qz.category_id, qz.course_id, qz.description, isnull(qz.passing_marks,0) as passing_marks")
                        .From("quiz qz")
                        .LeftJoin("quiz_remaining_time rt").On("qz.quiz_id = rt.quiz_id")
                        .Where("qz.quiz_id=@0 and qz.is_deleted!=1", quiz_id);




                    result = repo.Query<quiz>(ppSql).FirstOrDefault();

                }
                catch (Exception ex)
                {
                    LogErrorInDatabase(ex.Message, "DAL Operation error");
                    return result;
                }


            }

            return result; ;

        }


        public List<courses> GetCoursesList(int? pageId, int pageSize, string? title, int? status, int? course_category_id)
        {
            List<courses> result = new List<courses>();


            using (var repo = _contextHelper.GetPPContextHelper())
            {
                try
                {


                    #region Create Search Parameters
                    var SearchParameters = PetaPoco.Sql.Builder.Append(" ");

                    if (!String.IsNullOrEmpty(title))
                    {
                        SearchParameters.Append("and cr.title LIKE @0 ", "%" + title + "%");
                    }

                    if (status != null && status != 0)
                    {
                        SearchParameters.Append("and cr.status = @0 ", status);
                    }

                    if (course_category_id != null && course_category_id != 0)
                    {
                        SearchParameters.Append("and cr.course_category_id = @0 ", course_category_id);
                    }

                    #endregion

                    var ppSql = PetaPoco.Sql.Builder.Select(@"COUNT(*) OVER () as TotalRecordCount, cr.course_id, cr.title, isnull(ct.course_category_id,0) as course_category_id, ct.title AS cousre_category_name, isnull(st.status_id,0) AS status, st.title AS status_name, description")
                        .From(" courses AS cr")
                        .LeftJoin(" course_categories AS ct").On("cr.course_category_id = ct.course_category_id")
                        .LeftJoin("status AS st").On("cr.status = st.status_id")
                        .Where("cr.is_deleted = 0 ")
                        .Append(SearchParameters)
                        .OrderBy("cr.course_id desc")
                       .Append(@"OFFSET (@0-1)*@1 ROWS
	                        FETCH NEXT @1 ROWS ONLY", pageId, pageSize);


                    result = repo.Query<courses>(ppSql).ToList();


                }
                catch (Exception ex)
                {
                    LogErrorInDatabase(ex.Message, "DAL Operation error");
                    return (result);
                }



            }
            return (result);

        }


        //--get status list
        public List<status> GetStatusDropdownList()
        {
            List<status> result = new List<status>();


            using (var repo = _contextHelper.GetPPContextHelper())
            {
                try
                {




                    var ppSql = PetaPoco.Sql.Builder.Select(@"status_id, title")
                        .From(" status")

                        .Where("is_deleted != 1 ")

                        .OrderBy("status_id asc");



                    result = repo.Query<status>(ppSql).ToList();


                }
                catch (Exception ex)
                {
                    LogErrorInDatabase(ex.Message, "DAL Operation error");
                    return (result);
                }



            }
            return (result);

        }

        //--get course categories for drop down in forms
        public List<course_categories> GetCourseCategoriesDropDownList()
        {
            List<course_categories> result = new List<course_categories>();


            using (var repo = _contextHelper.GetPPContextHelper())
            {
                try
                {




                    var ppSql = PetaPoco.Sql.Builder.Select(@" course_category_id, title")
                        .From(" course_categories")

                        .Where("is_deleted != 1 ")

                        .OrderBy("course_category_id asc");



                    result = repo.Query<course_categories>(ppSql).ToList();


                }
                catch (Exception ex)
                {
                    LogErrorInDatabase(ex.Message, "DAL Operation error");
                    return (result);
                }



            }
            return (result);

        }

        //--get course categories for drop down in forms
        public List<courses> GetCoursesListForDropDown(int created_by)
        {
            List<courses> result = new List<courses>();


            using (var repo = _contextHelper.GetPPContextHelper())
            {
                try
                {




                    var ppSql = PetaPoco.Sql.Builder.Select(@" course_id, title")
                        .From(" courses")

                        .Where("is_deleted != 1 and created_by=@0", created_by)

                        .OrderBy("course_id asc");



                    result = repo.Query<courses>(ppSql).ToList();


                }
                catch (Exception ex)
                {
                    LogErrorInDatabase(ex.Message, "DAL Operation error");
                    return (result);
                }



            }
            return (result);

        }

        //--get course categories for drop down in forms
        public List<quiz_categories> GetQuizCategoriesForDropDown()
        {
            List<quiz_categories> result = new List<quiz_categories>();


            using (var repo = _contextHelper.GetPPContextHelper())
            {
                try
                {




                    var ppSql = PetaPoco.Sql.Builder.Select(@"category_id, category_name")
                        .From(" quiz_categories")

                        .Where("is_deleted != 1 and is_active=1 ")

                        .OrderBy("category_id asc");



                    result = repo.Query<quiz_categories>(ppSql).ToList();


                }
                catch (Exception ex)
                {
                    LogErrorInDatabase(ex.Message, "DAL Operation error");
                    return (result);
                }



            }
            return (result);

        }

        //---insert new course
        public string? InsertNewCourse(courses FormData)
        {
            string? result = "";
            using (var repo = _contextHelper.GetPPContextHelper())
            {

                try
                {
                    if (FormData != null)
                    {

                        var response = repo.Execute("INSERT INTO courses ( title, status, course_category_id, description, guid, created_at, created_by, is_deleted)" +
                            " VALUES (@0, @1, @2, @3,@4,getdate(), @5, 0 )", FormData.title, FormData.status, FormData.course_category_id, FormData.description, FormData.guid, FormData.created_by);
                        if (response > 0)
                        {
                            result = "Saved Successfully!";
                        }
                        else
                        {
                            result = "Error occured in processing your request!";
                        }

                    }
                    else
                    {
                        result = "Please fill all required fields!";
                    }
                }
                catch (Exception ex)
                {
                    LogErrorInDatabase(ex.Message, "DAL Operation error");
                    result = "Error occured in processing your request!";
                    return result;
                }


            }
            return result;


        }

        //---update  course
        public string? UpdateCourse(courses FormData)
        {
            string? result = "";
            using (var repo = _contextHelper.GetPPContextHelper())
            {

                try
                {
                    if (FormData != null)
                    {

                        var response = repo.Execute("update courses set title=@0, status=@1, course_category_id=@2, description=@3, modified_at=getdate(), modified_by=@4 where course_id=@5",
                            FormData.title, FormData.status, FormData.course_category_id, FormData.description, FormData.modified_by, FormData.course_id);
                        if (response > 0)
                        {
                            result = "Updated Successfully!";
                        }
                        else
                        {
                            result = "Error occured in processing your request!";
                        }

                    }
                    else
                    {
                        result = "Please fill all required fields!";
                    }
                }
                catch (Exception ex)
                {
                    LogErrorInDatabase(ex.Message, "DAL Operation error");
                    result = "Error occured in processing your request!";
                    return result;
                }


            }
            return result;


        }

        //--delete course
        public string? DeleteCourse(int? course_id)
        {
            string? result = "";
            using (var repo = _contextHelper.GetPPContextHelper())
            {

                try
                {
                    if (course_id != null)
                    {
                        var response = repo.Execute(@"Delete from courses   where  course_id=@0", course_id);
                        if (response > 0)
                        {
                            result = "Deleted Successfully!";
                        }
                        else
                        {
                            result = "Error occured in deleting the record. Please try again!";
                        }

                    }
                    else
                    {
                        result = "Error occured in deleting the record. Please try again!";
                    }
                }
                catch (Exception ex)
                {
                    LogErrorInDatabase(ex.Message, "DAL Operation error");
                    result = "Error occured in deleting the record. Please try again!";
                    return result;
                }


            }
            return result;


        }




        public users UserLogin(string email, string? password)
        {
            users result = new users();


            using (var repo = _contextHelper.GetPPContextHelper())
            {
                try
                {


                    var ppSql = PetaPoco.Sql.Builder.Select(@" * ")
                   .From("users")
                   .Where("email=@0 and password=@1 and is_active=1", email, password);
                    result = repo.Query<users>(ppSql).FirstOrDefault();



                }
                catch (Exception ex)
                {
                    LogErrorInDatabase(ex.Message, "DAL Operation error");
                    return result;
                }


            }

            return result; ;

        }



        public users StudentLogin(string email, string? password)
        {
            users result = new users();


            using (var repo = _contextHelper.GetPPContextHelper())
            {
                try
                {




                    var ppSql = PetaPoco.Sql.Builder.Select(@" * ")
                        .From("users")

                        .Where("email=@0 and password=@1", email, password);




                    result = repo.Query<users>(ppSql).FirstOrDefault();

                }
                catch (Exception ex)
                {
                    LogErrorInDatabase(ex.Message, "DAL Operation error");
                    return result;
                }


            }

            return result; ;

        }


        public List<users> GetStudentsListsForAdmin(int? pageId, int pageSize, string? user_name, string? email, int? user_id)
        {
            List<users> result = new List<users>();


            using (var repo = _contextHelper.GetPPContextHelper())
            {
                try
                {


                    #region Create Search Parameters
                    var SearchParameters = PetaPoco.Sql.Builder.Append(" ");

                    if (!String.IsNullOrEmpty(user_name))
                    {
                        SearchParameters.Append("and std.user_name LIKE @0 ", "%" + user_name + "%");
                    }

                    if (!String.IsNullOrEmpty(email))
                    {
                        SearchParameters.Append("and std.email = @0 ", email);
                    }

                    if (user_id != null && user_id != 0)
                    {
                        SearchParameters.Append("and std.user_id = @0 ", user_id);
                    }

                    #endregion

                    var ppSql = PetaPoco.Sql.Builder.Select(@"COUNT(*) OVER () as TotalRecordCount,  std.user_id, std.first_name, std.last_name,  std.user_name, std.email, std.is_active, std.password")
                        .From(" users AS std")

                        .Where("std.is_deleted = 0 or std.is_deleted is null AND std.user_type = @0", (short)UserTypesEnum.Student)
                        .Append(SearchParameters)
                        .OrderBy("std.user_id desc")
                       .Append(@"OFFSET (@0-1)*@1 ROWS
	                        FETCH NEXT @1 ROWS ONLY", pageId, pageSize);


                    result = repo.Query<users>(ppSql).ToList();


                }
                catch (Exception ex)
                {
                    LogErrorInDatabase(ex.Message, "DAL Operation error");
                    return (result);
                }



            }
            return (result);

        }


        public string? DeleteStudent(int? user_id)
        {
            string? result = "";
            using (var repo = _contextHelper.GetPPContextHelper())
            {

                try
                {
                    if (user_id != null)
                    {
                        var response = repo.Execute(@"Update users set is_deleted=1  where  user_id=@0", user_id);
                        if (response > 0)
                        {
                            result = "Deleted Successfully!";
                        }
                        else
                        {
                            result = "Error occured in deleting the record. Please try again!";
                        }

                    }
                    else
                    {
                        result = "Error occured in deleting the record. Please try again!";
                    }
                }
                catch (Exception ex)
                {
                    LogErrorInDatabase(ex.Message, "DAL Operation error");
                    result = "Error occured in deleting the record. Please try again!";
                    return result;
                }


            }
            return result;


        }


        public string? InsertNewStudent(users FormData)
        {
            string? result = "";
            using (var repo = _contextHelper.GetPPContextHelper())
            {

                try
                {
                    if (FormData != null)
                    {




                        bool isNameExists = checkIfStudentUserNameExists(FormData.user_name);

                        if (isNameExists == true)
                        {
                            result = "user name already exists";
                            return result;
                        }

                        bool isEmailExists = checkIfStudentEmailExists(FormData.email);
                        if (isEmailExists == true)
                        {
                            result = "email already exists";
                            return result;
                        }

                        var response = repo.Execute(@"INSERT INTO users(first_name, last_name, user_name, email, is_active,  user_type , created_at, created_by, is_deleted, password)
                         VALUES(@0, @1, @2,@3, @4, @5, getdate(), @6, 0 , @7)", FormData.first_name, FormData.last_name, FormData.user_name, FormData.email, FormData.is_active, FormData.user_type  ,FormData.created_by, FormData.password);
                        if (response > 0)
                        {
                            result = "Saved Successfully!";
                        }
                        else
                        {
                            result = "Error occured in processing your request!";
                        }

                    }
                    else
                    {
                        result = "Please fill all required fields!";
                    }
                }
                catch (Exception ex)
                {
                    LogErrorInDatabase(ex.Message, "DAL Operation error");
                    result = "Error occured in processing your request!";
                    return result;
                }


            }
            return result;


        }


        public string? EditStudent(users FormData)
        {
            string? result = "";
            using (var repo = _contextHelper.GetPPContextHelper())
            {

                try
                {
                    if (FormData != null)
                    {


                        bool isNameExists = checkStudentUserNameBeforeUpdateStudent(FormData.user_name, FormData.user_id);

                        if (isNameExists == true)
                        {
                            result = "user name already exists";
                            return result;
                        }

                        bool isEmailExists = checkStudentEmailBeforeUpdateStudent(FormData.email, FormData.user_id);
                        if (isEmailExists == true)
                        {
                            result = "email already exists";
                            return result;
                        }

                        var response = repo.Execute(@"update users
set first_name=@0, last_name=@1, user_name=@2, email=@3, is_active=@4, modified_at=getdate(), modified_by=@5 where user_id=@6",
FormData.first_name, FormData.last_name, FormData.user_name, FormData.email, FormData.is_active, FormData.created_by, FormData.user_id);
                        if (response > 0)
                        {
                            result = "Updated Successfully!";
                        }
                        else
                        {
                            result = "Error occured in processing your request!";
                        }

                    }
                    else
                    {
                        result = "Please fill all required fields!";
                    }
                }
                catch (Exception ex)
                {
                    LogErrorInDatabase(ex.Message, "DAL Operation error");
                    result = "Error occured in processing your request!";
                    return result;
                }


            }
            return result;


        }


        public bool checkIfStudentUserNameExists(string user_name)
        {
            bool result = false;
            using (var repo = _contextHelper.GetPPContextHelper())
            {

                try
                {
                    if (!String.IsNullOrEmpty(user_name))
                    {


                        #region Check if user name already exists


                        var ppSql = PetaPoco.Sql.Builder.Append(@"select count(*) as ResponseMsg from users where  user_name =@0", user_name);

                        var ifExists = repo.Query<QueryResponse>(ppSql).ToList();

                        if (ifExists.Any(m => m.ResponseMsg != "0"))
                        {
                            result = true;
                        }
                        else
                        {
                            result = false;
                        }


                        #endregion


                    }
                    else
                    {
                        result = true;
                    }
                }
                catch (Exception ex)
                {
                    LogErrorInDatabase(ex.Message, "DAL Operation error");
                    result = true;
                    return result;
                }


            }
            return result;
        }

        public bool checkIfStudentEmailExists(string email)
        {
            bool result = false;
            using (var repo = _contextHelper.GetPPContextHelper())
            {

                try
                {
                    if (!String.IsNullOrEmpty(email))
                    {


                        #region Check if user name already exists

                        var ppSql = PetaPoco.Sql.Builder.Append(@"select count(*) as ResponseMsg from users where  email =@0", email);

                        var ifExists = repo.Query<QueryResponse>(ppSql).ToList();

                        if (ifExists.Any(m => m.ResponseMsg != "0"))
                        {
                            result = true;
                        }
                        else
                        {
                            result = false;
                        }

                        #endregion


                    }
                    else
                    {
                        result = true;
                    }
                }
                catch (Exception ex)
                {
                    LogErrorInDatabase(ex.Message, "DAL Operation error");
                    result = true;
                    return result;
                }


            }
            return result;
        }

        public bool checkStudentUserNameBeforeUpdateStudent(string user_name, int student_id)
        {
            bool result = false;
            using (var repo = _contextHelper.GetPPContextHelper())
            {

                try
                {
                    if (!String.IsNullOrEmpty(user_name) || student_id < 1)
                    {


                        #region Check if user name already exists


                        var ppSql = PetaPoco.Sql.Builder.Append(@"select count(*) as ResponseMsg from users where  user_name =@0 AND user_id!=@1", user_name, student_id);

                        var ifExists = repo.Query<QueryResponse>(ppSql).ToList();

                        if (ifExists.Any(m => m.ResponseMsg != "0"))
                        {
                            result = true;
                        }
                        else
                        {
                            result = false;
                        }



                        #endregion


                    }
                    else
                    {
                        result = true;
                    }
                }
                catch (Exception ex)
                {
                    LogErrorInDatabase(ex.Message, "DAL Operation error");
                    result = true;
                    return result;
                }


            }
            return result;
        }
        public bool checkUserNameBeforeUpdateUser(string user_name, int user_id)
        {
            bool result = false;
            using (var repo = _contextHelper.GetPPContextHelper())
            {

                try
                {
                    if (!String.IsNullOrEmpty(user_name) || user_id < 1)
                    {


                        #region Check if user name already exists


                        var ppSql = PetaPoco.Sql.Builder.Append(@"select count(*) as ResponseMsg from users where  user_name =@0 AND user_id!=@1", user_name, user_id);

                        var ifExists = repo.Query<QueryResponse>(ppSql).ToList();

                        if (ifExists.Any(m => m.ResponseMsg != "0"))
                        {
                            result = true;
                        }
                        else
                        {
                            result = false;
                        }



                        #endregion


                    }
                    else
                    {
                        result = true;
                    }
                }
                catch (Exception ex)
                {
                    LogErrorInDatabase(ex.Message, "DAL Operation error");
                    result = true;
                    return result;
                }


            }
            return result;
        }
        public bool checkUserEmailBeforeUpdateUser(string email, int user_id)
        {
            bool result = false;
            using (var repo = _contextHelper.GetPPContextHelper())
            {

                try
                {
                    if (!String.IsNullOrEmpty(email) || user_id < 1)
                    {


                        #region Check if user name already exists


                        var ppSql = PetaPoco.Sql.Builder.Append(@"select count(*) as ResponseMsg from users where  email =@0 AND user_id!=@1", email, user_id);

                        var ifExists = repo.Query<QueryResponse>(ppSql).ToList();

                        if (ifExists.Any(m => m.ResponseMsg != "0"))
                        {
                            result = true;
                        }
                        else
                        {
                            result = false;
                        }

                        #endregion


                    }
                    else
                    {
                        result = true;
                    }
                }
                catch (Exception ex)
                {
                    LogErrorInDatabase(ex.Message, "DAL Operation error");
                    result = true;
                    return result;
                }


            }
            return result;
        }
        public bool checkStudentEmailBeforeUpdateStudent(string email, int student_id)
        {
            bool result = false;
            using (var repo = _contextHelper.GetPPContextHelper())
            {

                try
                {
                    if (!String.IsNullOrEmpty(email) || student_id < 1)
                    {


                        #region Check if user name already exists


                        var ppSql = PetaPoco.Sql.Builder.Append(@"select count(*) as ResponseMsg from users where  email =@0 AND user_id!=@1", email, student_id);

                        var ifExists = repo.Query<QueryResponse>(ppSql).ToList();

                        if (ifExists.Any(m => m.ResponseMsg != "0"))
                        {
                            result = true;
                        }
                        else
                        {
                            result = false;
                        }

                        #endregion


                    }
                    else
                    {
                        result = true;
                    }
                }
                catch (Exception ex)
                {
                    LogErrorInDatabase(ex.Message, "DAL Operation error");
                    result = true;
                    return result;
                }


            }
            return result;
        }

        public List<courses> GetStudentRegisteredCourses(int? pageId, int pageSize, int student_id, string? title, int? course_id)
        {
            List<courses> result = new List<courses>();


            using (var repo = _contextHelper.GetPPContextHelper())
            {
                try
                {


                    #region Create Search Parameters
                    var SearchParameters = PetaPoco.Sql.Builder.Append(" ");

                    if (!String.IsNullOrEmpty(title))
                    {
                        SearchParameters.Append("and cr.title LIKE @0 ", "%" + title + "%");
                    }

                    if (course_id != null && course_id != 0)
                    {
                        SearchParameters.Append("and StdCour.course_id = @0 ", course_id);
                    }



                    #endregion

                    var ppSql = PetaPoco.Sql.Builder.Select(@"COUNT(*) OVER () as TotalRecordCount, cr.course_id,StdCour.student_course_id ,cr.title, isnull(ct.course_category_id,0) as course_category_id, ct.title AS cousre_category_name, isnull(st.status_id,0) AS status, st.title AS status_name, description")
                        .From(" student_courses AS StdCour")
                        .InnerJoin("courses as cr").On("StdCour.course_id = cr.course_id")
                        .LeftJoin(" course_categories AS ct").On("cr.course_category_id = ct.course_category_id")
                        .LeftJoin("status AS st").On("cr.status = st.status_id")
                        .Where("StdCour.student_id = @0 ", student_id)
                        .Append(SearchParameters)
                        .OrderBy("cr.course_id desc")
                       .Append(@"OFFSET (@0-1)*@1 ROWS
	                        FETCH NEXT @1 ROWS ONLY", pageId, pageSize);


                    result = repo.Query<courses>(ppSql).ToList();


                }
                catch (Exception ex)
                {
                    LogErrorInDatabase(ex.Message, "DAL Operation error");
                    return (result);
                }



            }
            return (result);

        }

        public List<courses> GetStudentsUnAssignCourseList(int student_id, string? course_title)
        {
            List<courses> result = new List<courses>();


            using (var repo = _contextHelper.GetPPContextHelper())
            {
                try
                {

                    var ppSql = PetaPoco.Sql.Builder.Select(@"top 6 cr.course_id, cr.title")
                        .From(" courses AS cr")

                        .Append("where cr.course_id NOT IN(")
                        .Append("SELECT course_id FROM student_courses")
                        .Where("student_id = @0", student_id)
                        .Append(")")
                        .Append("and  cr.title LIKE @0", "%" + course_title + "%")
                        .OrderBy("cr.course_id desc");

                    result = repo.Query<courses>(ppSql).ToList();


                }
                catch (Exception ex)
                {
                    LogErrorInDatabase(ex.Message, "DAL Operation error");
                    return (result);
                }



            }
            return (result);

        }


        public List<courses> GetTeacherUnAssignCourseList(int user_id, string? course_title)
        {
            List<courses> result = new List<courses>();


            using (var repo = _contextHelper.GetPPContextHelper())
            {
                try
                {

                    var ppSql = PetaPoco.Sql.Builder.Select(@"top 6 cr.course_id, cr.title")
                        .From(" courses AS cr")

                        .Append("where cr.course_id NOT IN(")
                        .Append("SELECT course_id FROM teacher_assign_courses")
                        .Where("user_id = @0", user_id)
                        .Append(")")
                        .Append("and  cr.title LIKE @0", "%" + course_title + "%")
                        .OrderBy("cr.course_id desc");

                    result = repo.Query<courses>(ppSql).ToList();


                }
                catch (Exception ex)
                {
                    LogErrorInDatabase(ex.Message, "DAL Operation error");
                    return (result);
                }



            }
            return (result);

        }


        public string? AssignCourseToStudent(student_courses FormData)
        {
            string? result = "";
            using (var repo = _contextHelper.GetPPContextHelper())
            {

                try
                {
                    if (FormData != null)
                    {



                        var response = repo.Execute(@"insert into student_courses(student_id, course_id, guid, created_at, created_by, is_deleted)values(@0, @1, '', getdate(), @2, 0)",
                            FormData.student_id, FormData.course_id, FormData.created_by);
                        if (response > 0)
                        {
                            result = "Saved Successfully!";
                        }
                        else
                        {
                            result = "Error occured in processing your request!";
                        }

                    }
                    else
                    {
                        result = "Please fill all required fields!";
                    }
                }
                catch (Exception ex)
                {
                    LogErrorInDatabase(ex.Message, "DAL Operation error");
                    result = "Error occured in processing your request!";
                    return result;
                }


            }
            return result;


        }

        public string? AssignCourseToTeacher(teacher_assign_courses FormData)
        {
            string? result = "";
            using (var repo = _contextHelper.GetPPContextHelper())
            {

                try
                {
                    if (FormData != null)
                    {



                        var response = repo.Execute(@"insert into teacher_assign_courses(user_id, course_id, guid, created_at, created_by, is_deleted)values(@0, @1, '', getdate(), @2, 0)",
                            FormData.user_id, FormData.course_id, FormData.created_by);
                        if (response > 0)
                        {
                            result = "Saved Successfully!";
                        }
                        else
                        {
                            result = "Error occured in processing your request!";
                        }

                    }
                    else
                    {
                        result = "Please fill all required fields!";
                    }
                }
                catch (Exception ex)
                {
                    LogErrorInDatabase(ex.Message, "DAL Operation error");
                    result = "Error occured in processing your request!";
                    return result;
                }


            }
            return result;


        }

        public string? RemoveCourseFromStudentList(int student_course_id)
        {
            string? result = "";
            using (var repo = _contextHelper.GetPPContextHelper())
            {

                try
                {
                    if (student_course_id > 0)
                    {
                        var response = repo.Execute(@"Delete from student_courses  where  student_course_id=@0", student_course_id);
                        if (response > 0)
                        {
                            result = "Deleted Successfully!";
                        }
                        else
                        {
                            result = "Error occured in deleting the record. Please try again!";
                        }

                    }
                    else
                    {
                        result = "Error occured in deleting the record. Please try again!";
                    }
                }
                catch (Exception ex)
                {
                    LogErrorInDatabase(ex.Message, "DAL Operation error");
                    result = "Error occured in deleting the record. Please try again!";
                    return result;
                }


            }
            return result;


        }

        public string? RemoveCourseFromTeacherCoursesList(int teacher_assign_id)
        {
            string? result = "";
            using (var repo = _contextHelper.GetPPContextHelper())
            {

                try
                {
                    if (teacher_assign_id > 0)
                    {
                        var response = repo.Execute(@"Delete from teacher_assign_courses  where  teacher_assign_id=@0", teacher_assign_id);
                        if (response > 0)
                        {
                            result = "Deleted Successfully!";
                        }
                        else
                        {
                            result = "Error occured in deleting the record. Please try again!";
                        }

                    }
                    else
                    {
                        result = "Error occured in deleting the record. Please try again!";
                    }
                }
                catch (Exception ex)
                {
                    LogErrorInDatabase(ex.Message, "DAL Operation error");
                    result = "Error occured in deleting the record. Please try again!";
                    return result;
                }


            }
            return result;


        }


        public List<users> GetTeachersListsForAdmin(int? pageId, int pageSize, string? user_name, string? email, int? user_id, int user_type)
        {
            List<users> result = new List<users>();


            using (var repo = _contextHelper.GetPPContextHelper())
            {
                try
                {


                    #region Create Search Parameters
                    var SearchParameters = PetaPoco.Sql.Builder.Append(" ");

                    if (!String.IsNullOrEmpty(user_name))
                    {
                        SearchParameters.Append("and usr.user_name LIKE @0 ", "%" + user_name + "%");
                    }

                    if (!String.IsNullOrEmpty(email))
                    {
                        SearchParameters.Append("and usr.email = @0 ", email);
                    }

                    if (user_id != null && user_id != 0)
                    {
                        SearchParameters.Append("and usr.user_id = @0 ", user_id);
                    }

                    #endregion

                    var ppSql = PetaPoco.Sql.Builder.Select(@"COUNT(*) OVER () as TotalRecordCount,  usr.user_id, usr.first_name, usr.last_name,  usr.user_name, usr.email, usr.is_active, usr.password")
                        .From(" users AS usr")

                        .Where(" (usr.is_deleted = 0 OR usr.is_deleted is null) and  usr.user_type=@0", user_type)
                        .Append(SearchParameters)
                        .OrderBy("usr.user_id desc")
                       .Append(@"OFFSET (@0-1)*@1 ROWS
	                        FETCH NEXT @1 ROWS ONLY", pageId, pageSize);


                    result = repo.Query<users>(ppSql).ToList();


                }
                catch (Exception ex)
                {
                    LogErrorInDatabase(ex.Message, "DAL Operation error");
                    return (result);
                }



            }
            return (result);

        }


        public string? InsertNewTeacher(users FormData)
        {
            string? result = "";
            using (var repo = _contextHelper.GetPPContextHelper())
            {

                try
                {
                    if (FormData != null)
                    {




                        bool isNameExists = checkIfUserNameExists(FormData.user_name);

                        if (isNameExists == true)
                        {
                            result = "user name already exists";
                            return result;
                        }

                        bool isEmailExists = checkIfUserEmailExists(FormData.email);
                        if (isEmailExists == true)
                        {
                            result = "email already exists";
                            return result;
                        }

                        var response = repo.Execute(@"INSERT INTO users(first_name, last_name, user_name, email, is_active, created_at, created_by, is_deleted, password, user_type)
                         VALUES(@0, @1, @2,@3, @4, getdate(), @5, 0 , @6, @7)", FormData.first_name, FormData.last_name, FormData.user_name, FormData.email, FormData.is_active, FormData.created_by, FormData.password, FormData.user_type);
                        if (response > 0)
                        {
                            result = "Saved Successfully!";
                        }
                        else
                        {
                            result = "Error occured in processing your request!";
                        }

                    }
                    else
                    {
                        result = "Please fill all required fields!";
                    }
                }
                catch (Exception ex)
                {
                    LogErrorInDatabase(ex.Message, "DAL Operation error");
                    result = "Error occured in processing your request!";
                    return result;
                }


            }
            return result;


        }

        public string? EditTeacher(users FormData)
        {
            string? result = "";
            using (var repo = _contextHelper.GetPPContextHelper())
            {

                try
                {
                    if (FormData != null)
                    {


                        bool isNameExists = checkUserNameBeforeUpdateUser(FormData.user_name, FormData.user_id);

                        if (isNameExists == true)
                        {
                            result = "user name already exists";
                            return result;
                        }

                        bool isEmailExists = checkUserEmailBeforeUpdateUser(FormData.email, FormData.user_id);
                        if (isEmailExists == true)
                        {
                            result = "email already exists";
                            return result;
                        }

                        var response = repo.Execute(@"update users
                    set first_name=@0, last_name=@1, user_name=@2, email=@3, is_active=@4, modified_at=getdate(), modified_by=@5 where user_id=@6",
                    FormData.first_name, FormData.last_name, FormData.user_name, FormData.email, FormData.is_active, FormData.created_by, FormData.user_id);
                        if (response > 0)
                        {
                            result = "Updated Successfully!";
                        }
                        else
                        {
                            result = "Error occured in processing your request!";
                        }

                    }
                    else
                    {
                        result = "Please fill all required fields!";
                    }
                }
                catch (Exception ex)
                {
                    LogErrorInDatabase(ex.Message, "DAL Operation error");
                    result = "Error occured in processing your request!";
                    return result;
                }


            }
            return result;


        }


        public bool checkIfUserNameExists(string user_name)
        {
            bool result = false;
            using (var repo = _contextHelper.GetPPContextHelper())
            {

                try
                {
                    if (!String.IsNullOrEmpty(user_name))
                    {


                        #region Check if user name already exists


                        var ppSql = PetaPoco.Sql.Builder.Append(@"select count(*) as ResponseMsg from users where  user_name =@0", user_name);

                        var ifExists = repo.Query<QueryResponse>(ppSql).ToList();

                        if (ifExists.Any(m => m.ResponseMsg != "0"))
                        {
                            result = true;
                        }
                        else
                        {
                            result = false;
                        }


                        #endregion


                    }
                    else
                    {
                        result = true;
                    }
                }
                catch (Exception ex)
                {
                    LogErrorInDatabase(ex.Message, "DAL Operation error");
                    result = true;
                    return result;
                }


            }
            return result;
        }

        public bool checkIfUserEmailExists(string email)
        {
            bool result = false;
            using (var repo = _contextHelper.GetPPContextHelper())
            {

                try
                {
                    if (!String.IsNullOrEmpty(email))
                    {


                        #region Check if user name already exists

                        var ppSql = PetaPoco.Sql.Builder.Append(@"select count(*) as ResponseMsg from users where  email =@0", email);

                        var ifExists = repo.Query<QueryResponse>(ppSql).ToList();

                        if (ifExists.Any(m => m.ResponseMsg != "0"))
                        {
                            result = true;
                        }
                        else
                        {
                            result = false;
                        }

                        #endregion


                    }
                    else
                    {
                        result = true;
                    }
                }
                catch (Exception ex)
                {
                    LogErrorInDatabase(ex.Message, "DAL Operation error");
                    result = true;
                    return result;
                }


            }
            return result;
        }

        public string? DeleteTeacher(int? user_id)
        {
            string? result = "";
            using (var repo = _contextHelper.GetPPContextHelper())
            {

                try
                {
                    if (user_id != null)
                    {
                        var response = repo.Execute(@"Update users set is_deleted=1  where  user_id=@0", user_id);
                        if (response > 0)
                        {
                            result = "Deleted Successfully!";
                        }
                        else
                        {
                            result = "Error occured in deleting the record. Please try again!";
                        }

                    }
                    else
                    {
                        result = "Error occured in deleting the record. Please try again!";
                    }
                }
                catch (Exception ex)
                {
                    LogErrorInDatabase(ex.Message, "DAL Operation error");
                    result = "Error occured in deleting the record. Please try again!";
                    return result;
                }


            }
            return result;


        }

        public List<courses> GetTeacherAssignCourses(int? pageId, int pageSize, int user_id, string? title, int? course_id)
        {
            List<courses> result = new List<courses>();


            using (var repo = _contextHelper.GetPPContextHelper())
            {
                try
                {


                    #region Create Search Parameters
                    var SearchParameters = PetaPoco.Sql.Builder.Append(" ");

                    if (!String.IsNullOrEmpty(title))
                    {
                        SearchParameters.Append("and cr.title LIKE @0 ", "%" + title + "%");
                    }

                    if (course_id != null && course_id != 0)
                    {
                        SearchParameters.Append("and tc.course_id = @0 ", course_id);
                    }



                    #endregion

                    var ppSql = PetaPoco.Sql.Builder.Select(@"COUNT(*) OVER () as TotalRecordCount, cr.course_id,tc.teacher_assign_id ,cr.title, isnull(ct.course_category_id,0) as course_category_id, ct.title AS cousre_category_name, isnull(st.status_id,0) AS status, st.title AS status_name, description")
                        .From(" teacher_assign_courses AS tc")
                        .InnerJoin("courses as cr").On("tc.course_id = cr.course_id")
                        .LeftJoin(" course_categories AS ct").On("cr.course_category_id = ct.course_category_id")
                        .LeftJoin("status AS st").On("cr.status = st.status_id")
                        .Where("tc.user_id = @0 ", user_id)
                        .Append(SearchParameters)
                        .OrderBy("cr.course_id desc")
                       .Append(@"OFFSET (@0-1)*@1 ROWS
	                        FETCH NEXT @1 ROWS ONLY", pageId, pageSize);


                    result = repo.Query<courses>(ppSql).ToList();


                }
                catch (Exception ex)
                {
                    LogErrorInDatabase(ex.Message, "DAL Operation error");
                    return (result);
                }



            }
            return (result);

        }

        public List<ChartData> GetDashboardChartData()
        {
            List<ChartData> result = new List<ChartData>();


            using (var repo = _contextHelper.GetPPContextHelper())
            {
                try
                {
                    var ppSql = PetaPoco.Sql.Builder.Select(@"DISTINCT year(S.created_at) as Label, COUNT(S.student_id) OVER(partition by year(S.created_at)) as Data")
                        .From(" users AS S");



                    result = repo.Query<ChartData>(ppSql).ToList();


                }
                catch (Exception ex)
                {
                    LogErrorInDatabase(ex.Message, "DAL Operation error");
                    return (result);
                }



            }
            return (result);

        }
        public FooterData GetAdminFooterTotalStudentsCourses()
        {
            FooterData result = new FooterData();


            using (var repo = _contextHelper.GetPPContextHelper())
            {
                try
                {
                    var ppSql = PetaPoco.Sql.Builder.Select(@"TOP 1 COUNT(*) AS TotalStudents, (SELECT COUNT(*) FROM courses) TotalCourses")
                        .From(" users");

                    result = repo.Query<FooterData>(ppSql).FirstOrDefault();


                }
                catch (Exception ex)
                {
                    LogErrorInDatabase(ex.Message, "DAL Operation error");
                    return (result);
                }



            }
            return (result);

        }


    }

    public interface IAdminServices
    {
        public void LogErrorInDatabase(string error_message, string? log_type);
        public string? InsertQuizCategory(quiz_categories FormData);
        public string? InsertCourseCategory(course_categories FormData);
        public string? UpdateQuizCategory(quiz_categories FormData);
        public string? UpdateCourseCategory(course_categories FormData);
        public string? DeleteQuizCategory(quiz_categories FormData);
        public string? DeleteCourseCategory(course_categories FormData);
        public List<quiz_categories> GetQuizCategoriesList(int? pageId, int pageSize, string? category_name, string? is_active);
        public List<course_categories> GetCourseCategoriesList(int? pageId, int pageSize, string? title);
        public int GetQuizIdByGuid(string guid);
        public quiz GetQuizDetailByID(int? quiz_id);
        public List<courses> GetCoursesList(int? pageId, int pageSize, string? title, int? status, int? course_category_id);
        public List<status> GetStatusDropdownList();
        public List<course_categories> GetCourseCategoriesDropDownList();
        public List<courses> GetCoursesListForDropDown(int created_by);
        public List<quiz_categories> GetQuizCategoriesForDropDown();
        public string? InsertNewCourse(courses FormData);
        public string? UpdateCourse(courses FormData);
        public string? DeleteCourse(int? course_id);
        public users UserLogin(string email, string? password);
        public users StudentLogin(string email, string? password);
        public List<users> GetStudentsListsForAdmin(int? pageId, int pageSize, string? user_name, string? email, int? user_id);
        public string? DeleteStudent(int? user_id);
        public string? InsertNewStudent(users FormData);
        public string? EditStudent(users FormData);
        public bool checkIfStudentUserNameExists(string user_name);
        public bool checkIfStudentEmailExists(string email);
        public bool checkStudentUserNameBeforeUpdateStudent(string user_name, int student_id);
        public bool checkUserNameBeforeUpdateUser(string user_name, int user_id);
        public bool checkUserEmailBeforeUpdateUser(string email, int user_id);
        public bool checkStudentEmailBeforeUpdateStudent(string email, int student_id);
        public List<courses> GetStudentRegisteredCourses(int? pageId, int pageSize, int student_id, string? title, int? course_id);
        public List<courses> GetStudentsUnAssignCourseList(int student_id, string? course_title);
        public List<courses> GetTeacherUnAssignCourseList(int user_id, string? course_title);
        public string? AssignCourseToStudent(student_courses FormData);
        public string? AssignCourseToTeacher(teacher_assign_courses FormData);
        public string? RemoveCourseFromStudentList(int student_course_id);
        public string? RemoveCourseFromTeacherCoursesList(int teacher_assign_id);
        public List<users> GetTeachersListsForAdmin(int? pageId, int pageSize, string? user_name, string? email, int? user_id, int user_type);
        public string? InsertNewTeacher(users FormData);
        public string? EditTeacher(users FormData);
        public bool checkIfUserNameExists(string user_name);
        public bool checkIfUserEmailExists(string email);
        public string? DeleteTeacher(int? user_id);
        public List<courses> GetTeacherAssignCourses(int? pageId, int pageSize, int user_id, string? title, int? course_id);
        public List<ChartData> GetDashboardChartData();
        public FooterData GetAdminFooterTotalStudentsCourses();

    }
}
