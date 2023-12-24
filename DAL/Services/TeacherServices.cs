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

namespace DAL.Services
{
    public class TeacherServices : ITeacherServices
    {
        private readonly IConfiguration _configuration;
        private readonly IDatabase _db;
        private readonly IDataContextHelper _contextHelper;


       

        public TeacherServices(IConfiguration configuration, IDatabase db, IDataContextHelper contextHelper)
        {
            _configuration = configuration;
            _db = db;
            _contextHelper = contextHelper;
        }


        public void LogErrorTeacherInDatabase(string? error_message, string? log_type)
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


        //--get quiz list
        public List<quiz> GetQuizList(int? pageId, int pageSize, int? course_id, int? category_id, string? title, int user_id)
        {
            List<quiz> result = new List<quiz>();


            using (var repo = _contextHelper.GetPPContextHelper())
            {
                try
                {


                    #region Create Search Parameters
                    var SearchParameters = PetaPoco.Sql.Builder.Append(" ");

                    if (course_id != null && course_id > 0)
                    {
                        SearchParameters.Append("and qz.course_id = @0 ", course_id);
                    }

                    if (category_id != null && category_id > 0)
                    {
                        SearchParameters.Append("and cat.category_id = @0 ", category_id);
                    }
                    if (!String.IsNullOrEmpty(title))
                    {
                        SearchParameters.Append("and qz.title LIKE @0", "%" + title + "%");
                    }


                    #endregion

                    var ppSql = PetaPoco.Sql.Builder.Select(@"distinct COUNT(*) OVER () as TotalRecordCount,   qz.quiz_id, qz.title, qz.start_date, qz.end_date, qz.allowed_time_minutes, cr.title AS course_title,
 cat.category_name,  st.title AS status_title")
                            .From(" quiz AS qz")
                            .LeftJoin("courses AS cr").On("qz.course_id = cr.course_id ")
                                .LeftJoin("quiz_categories AS cat").On("qz.category_id = cat.category_id")
                                    .LeftJoin("status AS st").On("qz.status = st.status_id")

                                       .LeftJoin("teacher_assign_courses tac").On("cr.course_id = tac.course_id")



                            .Where("qz.is_deleted!= 1 and tac.user_id=@0", user_id)
                            .Append(SearchParameters)
                            .OrderBy("qz.quiz_id DESC")
                           .Append(@"OFFSET (@0-1)*@1 ROWS
	                        FETCH NEXT @1 ROWS ONLY", pageId, pageSize);


                    result = repo.Query<quiz>(ppSql).ToList();


                }
                catch (Exception ex)
                {
                    LogErrorTeacherInDatabase(ex.Message, "DAL Teacher Service error");
                    return (result);
                }


            }
            return (result);

        }

        public List<courses> GetCoursesListForTeacherDropDowns(int user_id)
        {
            List<courses> result = new List<courses>();


            using (var repo = _contextHelper.GetPPContextHelper())
            {
                try
                {




                    var ppSql = PetaPoco.Sql.Builder.Select(@" cr.course_id, cr.title")
                        .From(" courses cr")
                         .InnerJoin("teacher_assign_courses tac").On("cr.course_id = tac.course_id")
                        .Where("cr.is_deleted != 1 and tac.user_id=@0", user_id)

                        .OrderBy("course_id asc");



                    result = repo.Query<courses>(ppSql).ToList();


                }
                catch (Exception ex)
                {
                    LogErrorTeacherInDatabase(ex.Message, "DAL Teacher Service error");
                    return (result);
                }



            }
            return (result);

        }

        public List<quiz_questions> GetQuestionListByQuizId(int? quiz_id)
        {
            List<quiz_questions> result = new List<quiz_questions>();


            using (var repo = _contextHelper.GetPPContextHelper())
            {
                try
                {




                    var ppSql = PetaPoco.Sql.Builder.Select(@" question_id, isnull(quiz_id,0) as quiz_id, question_title, option_a, option_b, option_c, option_d, correct_option")
                        .From("quiz_questions")

                        .Where("quiz_id=@0 and is_deleted!=1", quiz_id)
                        .OrderBy("question_id asc");



                    result = repo.Query<quiz_questions>(ppSql).ToList();

                }
                catch (Exception ex)
                {
                    LogErrorTeacherInDatabase(ex.Message, "DAL Teacher Service error");
                    return result;
                }


            }

            return result; ;

        }

        public List<student_result> GetStudentsResultList(int? pageId, int pageSize, int? quiz_id)
        {
            List<student_result> result = new List<student_result>();


            using (var repo = _contextHelper.GetPPContextHelper())
            {
                try
                {




                    var ppSql = PetaPoco.Sql.Builder.Select(@"COUNT(*) OVER () as TotalRecordCount, quiz_q.quiz_id , std.user_id as student_id, std.first_name,   std.last_name, std.email,isnull(quiz.passing_marks,50) as passing_marks,
                         count(quiz_q.question_id) as total_questions, 
                        count(quiz_q_correct.correct_option) as total_correct_ans,
                         (select count(quiz_q_correct.correct_option)*100/count(quiz_q.question_id) ) as std_percentage")
                        .From("student_answers AS sa")
                        .InnerJoin("quiz_questions AS quiz_q").On("sa.question_id = quiz_q.question_id ")
                         .InnerJoin("users AS std").On("sa.student_id = std.user_id ")
                          .LeftJoin("quiz_questions AS quiz_q_correct").On("sa.question_id = quiz_q_correct.question_id and sa.student_ans = quiz_q_correct.correct_option ")
                          .InnerJoin("quiz as quiz").On("quiz_q.quiz_id=quiz.quiz_id ")
                          .Where("quiz_q.quiz_id=@0", quiz_id)
                          .GroupBy("quiz_q.quiz_id, std.user_id, std.first_name,   std.last_name, std.email, quiz.passing_marks")
                        .OrderBy("std.user_id")
                      .Append(@"OFFSET (@0-1)*@1 ROWS
	                        FETCH NEXT @1 ROWS ONLY", pageId, pageSize);


                    result = repo.Query<student_result>(ppSql).ToList();

                }
                catch (Exception ex)
                {
                    LogErrorTeacherInDatabase(ex.Message, "DAL Teacher Service error");
                    return result;
                }


            }

            return result; ;

        }


        //--method for updating quiz
        public string? UpdateQuiz(quiz FormData)
        {
            string? result = "";
            using (var repo = _contextHelper.GetPPContextHelper())
            {

                try
                {
                    if (FormData != null)
                    {


                        var response = repo.Execute(@"update quiz set title=@0, start_date=@1, end_date=@2, allowed_time_minutes=@3, status=@4, category_id=@5, course_id=@6,
                        description =@7, passing_marks =@8, modified_at =getdate(), modified_by =@9 where quiz_id=@10", FormData.title, FormData.start_date, FormData.end_date, FormData.allowed_time_minutes, FormData.status,
                        FormData.category_id, FormData.course_id, FormData.description, FormData.passing_marks, FormData.modified_by, FormData.quiz_id);

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
                    LogErrorTeacherInDatabase(ex.Message, "DAL Teacher Service error");
                    result = "Error occured in processing your request!";
                    return result;
                }


            }
            return result;


        }

        public string? InsertNewQuiz(quiz FormData)
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


                        var response = repo.Execute("INSERT INTO quiz ( title, start_date, end_date, allowed_time_minutes, status, category_id, course_id, description," +
                            " guid, created_at, created_by,passing_marks ,is_deleted) VALUES (@0, @1, @2, @3,@4,@5 ,@6 ,@7 ,@8 ,@9 ,@10 ,@11, @12)",
                            FormData.title, FormData.start_date, FormData.end_date, FormData.allowed_time_minutes, FormData.status, FormData.category_id,
                            FormData.course_id, FormData.description, FormData.guid, DateTime.Now, FormData.created_by, FormData.passing_marks, "0");

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
                    LogErrorTeacherInDatabase(ex.Message, "DAL Teacher Service error");
                    result = "Error occured in processing your request!";
                    return result;
                }


            }
            return result;


        }


        public student_result GetStudentsResultByQuizID_StudentID(int? quiz_id, int? student_id)
        {

            student_result result = new student_result();


            using (var repo = _contextHelper.GetPPContextHelper())
            {
                try
                {




                    var ppSql = PetaPoco.Sql.Builder.Select(@"top 1  quiz_q.quiz_id , std.user_id, std.first_name,   std.last_name, std.email,isnull(quiz.passing_marks,50) as passing_marks,
                       count(quiz_q.question_id) as total_questions,
                         (select count(quiz_q_correct.correct_option)*100/count(quiz_q.question_id) ) as std_percentage")
                        .From("student_answers AS sa")
                        .InnerJoin("quiz_questions AS quiz_q").On("sa.question_id = quiz_q.question_id ")
                         .InnerJoin("users AS std").On("sa.student_id = std.user_id ")
                          .LeftJoin("quiz_questions AS quiz_q_correct").On("sa.question_id = quiz_q_correct.question_id and sa.student_ans = quiz_q_correct.correct_option ")
                          .InnerJoin("quiz as quiz").On("quiz_q.quiz_id=quiz.quiz_id ")
                          .Where("quiz_q.quiz_id=@0 and std.user_id=@1", quiz_id, student_id)
                          .GroupBy("quiz_q.quiz_id, std.user_id, std.first_name,   std.last_name, std.email, quiz.passing_marks");



                    result = repo.Query<student_result>(ppSql).FirstOrDefault();

                }
                catch (Exception ex)
                {
                    LogErrorTeacherInDatabase(ex.Message, "DAL Teacher Service error");
                    return result;
                }


            }

            return result; ;

        }


        public List<quiz_questions> GetQuestionListByQuizId_StudentID(int? quiz_id, int? student_id)
        {
            List<quiz_questions> result = new List<quiz_questions>();


            using (var repo = _contextHelper.GetPPContextHelper())
            {
                try
                {




                    var ppSql = PetaPoco.Sql.Builder.Select(@" quiz_q.question_id,quiz_q.question_title, quiz_q.option_a, quiz_q.option_b, quiz_q.option_c, quiz_q.option_d, quiz_q.correct_option, std_ans.student_ans")
                        .From("student_answers AS std_ans")
                        .InnerJoin("quiz_questions AS quiz_q").On("std_ans.question_id = quiz_q.question_id")

                        .Where("quiz_q.quiz_id=@0 and std_ans.student_id=@1", quiz_id, student_id)
                        .OrderBy("quiz_q.question_id asc");



                    result = repo.Query<quiz_questions>(ppSql).ToList();

                }
                catch (Exception ex)
                {
                    LogErrorTeacherInDatabase(ex.Message, "DAL Teacher Service error");
                    return result;
                }


            }

            return result; ;

        }



        //--insert new quiz question
        public string? InsertQuestion(quiz_questions FormData)
        {
            string? result = "";
            using (var repo = _contextHelper.GetPPContextHelper())
            {

                try
                {
                    if (FormData != null)
                    {

                        var response = repo.Execute("INSERT INTO quiz_questions ( quiz_id, question_title, option_a, option_b, option_c, option_d, correct_option," +
                            " guid, created_at, created_by, is_deleted) VALUES (@0, @1, @2, @3,@4,@5 ,@6 ,@7 ,@8 ,@9 ,@10 )",
                            FormData.quiz_id, FormData.question_title, FormData.option_a, FormData.option_b, FormData.option_c, FormData.option_d,
                            FormData.correct_option, FormData.guid, DateTime.Now, FormData.created_by, "0");

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
                    LogErrorTeacherInDatabase(ex.Message, "DAL Teacher Service error");
                    result = "Error occured in processing your request!";
                    return result;
                }


            }
            return result;


        }

        //--Update new quiz question
        public string? UdpateQuestion(quiz_questions FormData)
        {
            string? result = "";
            using (var repo = _contextHelper.GetPPContextHelper())
            {

                try
                {
                    if (FormData != null)
                    {

                        var response = repo.Execute(@"update quiz_questions
                            set question_title =@0, option_a =@1, option_b =@2, option_c =@3, option_d =@4, correct_option =@5,  modified_at =@6, modified_by =@7
                            where question_id =@8", FormData.question_title, FormData.option_a, FormData.option_b, FormData.option_c, FormData.option_d, FormData.correct_option,
                            DateTime.Now, FormData.modified_by, FormData.question_id);

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
                    LogErrorTeacherInDatabase(ex.Message, "DAL Teacher Service error");
                    result = "Error occured in processing your request!";
                    return result;
                }


            }
            return result;


        }

        //--delete new quiz question
        public string? DeleteQuestion(int question_id)
        {
            string? result = "";
            using (var repo = _contextHelper.GetPPContextHelper())
            {

                try
                {
                    if (question_id > 0)
                    {

                        var response = repo.Execute(@"delete from quiz_questions where question_id =@0", question_id);

                        if (response > 0)
                        {
                            result = "Deleted Successfully!";
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
                    LogErrorTeacherInDatabase(ex.Message, "DAL Teacher Service error");
                    result = "Error occured in processing your request!";
                    return result;
                }


            }
            return result;


        }

        public List<courses> GetCoursesList(int? pageId, int pageSize, string? title, int? status, int? course_category_id, int user_id)
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
                            .InnerJoin("teacher_assign_courses tac").On("cr.course_id = tac.course_id")
                        .Where("cr.is_deleted = 0 and tac.user_id=@0", user_id)
                        .Append(SearchParameters)
                        .OrderBy("cr.course_id desc")
                       .Append(@"OFFSET (@0-1)*@1 ROWS
	                        FETCH NEXT @1 ROWS ONLY", pageId, pageSize);


                    result = repo.Query<courses>(ppSql).ToList();


                }
                catch (Exception ex)
                {
                    LogErrorTeacherInDatabase(ex.Message, "DAL Teacher Service error");
                    return (result);
                }



            }
            return (result);

        }

        public List<users> GetStudentsListsForTeachers(int? pageId, int pageSize, string? user_name, string? email, int? student_id)
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

                    if (student_id != null && student_id != 0)
                    {
                        SearchParameters.Append("and std.user_id = @0 ", student_id);
                    }

                    #endregion

                    var ppSql = PetaPoco.Sql.Builder.Select(@"COUNT(*) OVER () as TotalRecordCount,  std.user_id, std.first_name, std.last_name,  std.user_name, std.email, std.is_active")
                        .From(" users AS std")

                        .Where("std.is_deleted is not null ")
                        .Append(SearchParameters)
                        .OrderBy("std.user_id desc")
                       .Append(@"OFFSET (@0-1)*@1 ROWS
	                        FETCH NEXT @1 ROWS ONLY", pageId, pageSize);


                    result = repo.Query<users>(ppSql).ToList();


                }
                catch (Exception ex)
                {
                    LogErrorTeacherInDatabase(ex.Message, "DAL Teacher Service error");
                    return (result);
                }



            }
            return (result);

        }

        public List<assignments> GetAssignmentListForTeachers(int? pageId, int pageSize, int? course_id, string? title, int user_id)
        {
            List<assignments> result = new List<assignments>();


            using (var repo = _contextHelper.GetPPContextHelper())
            {
                try
                {


                    #region Create Search Parameters
                    var SearchParameters = PetaPoco.Sql.Builder.Append(" ");

                    if (course_id != null && course_id > 0)
                    {
                        SearchParameters.Append("and ast.course_id = @0 ", course_id);
                    }


                    if (!String.IsNullOrEmpty(title))
                    {
                        SearchParameters.Append("and ast.title LIKE @0", "%" + title + "%");
                    }


                    #endregion

                    var ppSql = PetaPoco.Sql.Builder.Select(@" distinct COUNT(*) OVER () as TotalRecordCount, ast.*, st.title AS status_title, cr.title as course_name")
                            .From(" assignments ast")
                            .LeftJoin("courses AS cr").On("ast.course_id = cr.course_id ")

                                    .LeftJoin("status AS st").On("ast.status = st.status_id")
                                    .InnerJoin("teacher_assign_courses tac").On("cr.course_id = tac.course_id")
                            .Where("ast.is_deleted!= 1 AND tac.user_id=@0", user_id)
                            .Append(SearchParameters)
                            .OrderBy("ast.assignment_id DESC")
                           .Append(@"OFFSET (@0-1)*@1 ROWS
	                        FETCH NEXT @1 ROWS ONLY", pageId, pageSize);


                    result = repo.Query<assignments>(ppSql).ToList();


                }
                catch (Exception ex)
                {
                    LogErrorTeacherInDatabase(ex.Message, "DAL Teacher Service error");
                    return (result);
                }


            }
            return (result);

        }

        public string? DeleteAssignment(int assignment_id)
        {
            string? result = "";
            using (var repo = _contextHelper.GetPPContextHelper())
            {

                try
                {
                    if (assignment_id > 0)
                    {

                        var response = repo.Execute(@"delete from assignments where assignment_id =@0", assignment_id);

                        if (response > 0)
                        {
                            result = "Deleted Successfully!";
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
                    LogErrorTeacherInDatabase(ex.Message, "DAL Teacher Service error");
                    result = "Error occured in processing your request!";
                    return result;
                }


            }
            return result;


        }

        public string? DeleteQuizDAL(int quiz_id)
        {
            string? result = "";
            using (var repo = _contextHelper.GetPPContextHelper())
            {

                try
                {
                    if (quiz_id > 0)
                    {

                        var response = repo.Execute(@"delete from quiz where quiz_id =@0", quiz_id);

                        if (response > 0)
                        {
                            result = "Deleted Successfully!";
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
                    LogErrorTeacherInDatabase(ex.Message, "DAL Teacher Service error");
                    result = "Error occured in processing your request!";
                    return result;
                }


            }
            return result;


        }

        public string? InsertNewAssignment(assignments FormData)
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


                        var response = repo.Execute("INSERT INTO assignments ( title, body, start_date, end_date, allowed_time_minutes, status, course_id, " +
                            " guid, created_at, created_by,passing_marks ,  total_marks ,is_deleted) VALUES (@0, @1, @2, @3,@4,@5 ,@6 ,@7,getdate() ,@8 ,@9 ,@10 , 0)",
                            FormData.title, FormData.body, FormData.start_date, FormData.end_date, FormData.allowed_time_minutes, FormData.status,
                            FormData.course_id, FormData.guid, FormData.created_by, FormData.passing_marks, FormData.total_marks);

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
                    LogErrorTeacherInDatabase(ex.Message, "DAL Teacher Service error");
                    result = "Error occured in processing your request!";
                    return result;
                }


            }
            return result;


        }

        public int GetAssignmentIdByGuid(string? guid)
        {
            assignments result = new assignments();
            int assignment_id = 0;

            using (var repo = _contextHelper.GetPPContextHelper())
            {
                try
                {




                    var ppSql = PetaPoco.Sql.Builder.Select(@" top 1  assignment_id")
                        .From("assignments")

                        .Where("guid=@0 and is_deleted!=1", guid)
                        .OrderBy("assignment_id desc");



                    result = repo.Query<assignments>(ppSql).FirstOrDefault();
                    assignment_id = result != null ? result.assignment_id : 0;
                }
                catch (Exception ex)
                {
                    LogErrorTeacherInDatabase(ex.Message, "DAL Teacher Service error");
                    return assignment_id;
                }


            }

            return assignment_id;

        }

        public assignments GetAssignmentDetailByID(int? assignment_id)
        {
            assignments result = new assignments();


            using (var repo = _contextHelper.GetPPContextHelper())
            {
                try
                {




                    var ppSql = PetaPoco.Sql.Builder.Select(@"top 1   asg.assignment_id, asg.title,asg.body, convert(varchar(30),asg.start_date,22) as start_date,  convert(varchar(30),asg.end_date,22) as end_date, IIF(rt.remaining_minutes>-1, rt.remaining_minutes, asg.allowed_time_minutes) as allowed_time_minutes, asg.status,  asg.course_id, asg.description, isnull(asg.passing_marks,0) as passing_marks, isnull(asg.total_marks,0) as total_marks")
                        .From("assignments asg")
                        .LeftJoin("assignment_remaining_time rt").On("asg.assignment_id = rt.assignment_id")
                        .Where("asg.assignment_id=@0 and asg.is_deleted!=1", assignment_id);




                    result = repo.Query<assignments>(ppSql).FirstOrDefault();

                }
                catch (Exception ex)
                {
                    LogErrorTeacherInDatabase(ex.Message, "DAL Teacher Service error");
                    return result;
                }


            }

            return result; ;

        }


        //--method for updating quiz
        public string? UpdateAssignment(assignments FormData)
        {
            string? result = "";
            using (var repo = _contextHelper.GetPPContextHelper())
            {

                try
                {
                    if (FormData != null)
                    {


                        var response = repo.Execute(@"update assignments set title=@0, start_date=@1, end_date=@2, allowed_time_minutes=@3, status=@4,  course_id=@5,
                    passing_marks =@6, modified_at =getdate(), modified_by =@7,  body=@8 where assignment_id=@9 ", FormData.title, FormData.start_date, FormData.end_date, FormData.allowed_time_minutes, FormData.status,
                        FormData.course_id, FormData.passing_marks, FormData.modified_by, FormData.body, FormData.assignment_id);

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
                    LogErrorTeacherInDatabase(ex.Message, "DAL Teacher Service error");
                    result = "Error occured in processing your request!";
                    return result;
                }


            }
            return result;


        }

        public List<student_assignment_result> GetStudentsAssignmentResultList(int? pageId, int pageSize, int? assignment_id)
        {
            List<student_assignment_result> result = new List<student_assignment_result>();


            using (var repo = _contextHelper.GetPPContextHelper())
            {
                try
                {




                    var ppSql = PetaPoco.Sql.Builder.Select(@" COUNT(*) OVER () as TotalRecordCount, ans.assign_answers_id, an.title as assignmentTitle,  isnull(ans.is_checked_by_teacher,0) as  is_checked_by_teacher  , an.total_marks, an.passing_marks,std.user_id as student_id, std.user_name, 
                        ans.gain_marks , IIF(ans.gain_marks>=an.passing_marks, 1, 0) as passStatus")

                        .From("assignment_answers ans")
                        .InnerJoin(" assignments an").On(" ans.assignment_id=an.assignment_id ")
                         .LeftJoin("users std").On(" ans.student_id=std.user_id ")

                          .Where("an.assignment_id=@0", assignment_id)

                        .OrderBy("ans.assign_answers_id asc")
                      .Append(@"OFFSET (@0-1)*@1 ROWS
	                        FETCH NEXT @1 ROWS ONLY", pageId, pageSize);


                    result = repo.Query<student_assignment_result>(ppSql).ToList();

                }
                catch (Exception ex)
                {
                    LogErrorTeacherInDatabase(ex.Message, "DAL Teacher Service error");
                    return result;
                }


            }

            return result; ;

        }



        public assignments GetAssignmentDetailForStudentForTeacher(int? assign_answers_id, int? student_id)
        {
            assignments result = new assignments();


            using (var repo = _contextHelper.GetPPContextHelper())
            {
                try
                {




                    var ppSql = PetaPoco.Sql.Builder.Select(@"top 1   qz.assignment_id , qz.title, qz.body , outAns.student_ans , IIF(rt.remaining_minutes>-1, rt.remaining_minutes, qz.allowed_time_minutes) as allowed_time_minutes, outAns.assign_answers_id,   outAns.gain_marks")
                        .From("assignments qz")
                        .LeftJoin("assignment_remaining_time rt").On("qz.assignment_id = rt.assignment_id")
                           .InnerJoin("assignment_answers outAns").On("qz.assignment_id=outAns.assignment_id")
                        .Where("outAns.assign_answers_id=@0 and qz.is_deleted!=1 and outAns.student_id=@1", assign_answers_id, student_id);




                    result = repo.Query<assignments>(ppSql).FirstOrDefault();

                }
                catch (Exception ex)
                {
                    LogErrorTeacherInDatabase(ex.Message, "DAL Teacher Service error");
                    return result;
                }


            }

            return result; ;

        }



        public string? GiveAssignmentMarksToStudent(assignment_answers FormData)
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

                        var response = repo.Execute("update assignment_answers set gain_marks=@0, is_checked_by_teacher=1 where assign_answers_id=@1", FormData.gain_marks, FormData.assign_answers_id);
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
                    LogErrorTeacherInDatabase(ex.Message, "DAL Teacher Service error");
                    result = "Error occured in processing your request!";
                    return result;
                }


            }
            return result;


        }

        public List<student_courses> GetStudentsListForCourse(int? pageId, int pageSize, int? course_id, string? user_name, int? student_id)
        {
            List<student_courses> result = new List<student_courses>();


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

                    if (student_id != null && student_id != 0)
                    {
                        SearchParameters.Append("and std.user_id = @0 ", student_id);
                    }



                    #endregion

                    var ppSql = PetaPoco.Sql.Builder.Select(@" distinct COUNT(*) OVER () as TotalRecordCount,   std.user_id as student_id, std.user_name, std.email, cr.course_id, cr.title")
                        .From(" student_courses scr")
                        .InnerJoin("users std").On("scr.student_id=std.user_id")
                        .InnerJoin("courses cr").On("scr.course_id=cr.course_id")

                        .Where("scr.course_id=@0", course_id)
                        .Append(SearchParameters)
                        .OrderBy("std.user_id asc")
                       .Append(@"OFFSET (@0-1)*@1 ROWS
	                        FETCH NEXT @1 ROWS ONLY", pageId, pageSize);


                    result = repo.Query<student_courses>(ppSql).ToList();


                }
                catch (Exception ex)
                {
                    LogErrorTeacherInDatabase(ex.Message, "DAL Teacher Service error");
                    return (result);
                }



            }
            return (result);

        }


    }


    public interface ITeacherServices
    {
        public void LogErrorTeacherInDatabase(string? error_message, string? log_type);
        public List<quiz> GetQuizList(int? pageId, int pageSize, int? course_id, int? category_id, string? title, int user_id);
        public List<courses> GetCoursesListForTeacherDropDowns(int user_id);
        public List<quiz_questions> GetQuestionListByQuizId(int? quiz_id);
        public List<student_result> GetStudentsResultList(int? pageId, int pageSize, int? quiz_id);
        public string? UpdateQuiz(quiz FormData);
        public string? InsertNewQuiz(quiz FormData);
        public student_result GetStudentsResultByQuizID_StudentID(int? quiz_id, int? student_id);
        public List<quiz_questions> GetQuestionListByQuizId_StudentID(int? quiz_id, int? student_id);
        public string? InsertQuestion(quiz_questions FormData);
        public string? UdpateQuestion(quiz_questions FormData);
        public string? DeleteQuestion(int question_id);
        public List<courses> GetCoursesList(int? pageId, int pageSize, string? title, int? status, int? course_category_id, int user_id);
        public List<users> GetStudentsListsForTeachers(int? pageId, int pageSize, string? user_name, string? email, int? student_id);
        public List<assignments> GetAssignmentListForTeachers(int? pageId, int pageSize, int? course_id, string? title, int user_id);
        public string? DeleteAssignment(int assignment_id);
        public string? DeleteQuizDAL(int quiz_id);
        public string? InsertNewAssignment(assignments FormData);
        public int GetAssignmentIdByGuid(string? guid);
        public assignments GetAssignmentDetailByID(int? assignment_id);
        public string? UpdateAssignment(assignments FormData);
        public List<student_assignment_result> GetStudentsAssignmentResultList(int? pageId, int pageSize, int? assignment_id);
        public assignments GetAssignmentDetailForStudentForTeacher(int? assign_answers_id, int? student_id);
        public string? GiveAssignmentMarksToStudent(assignment_answers FormData);
        public List<student_courses> GetStudentsListForCourse(int? pageId, int pageSize, int? course_id, string? user_name, int? student_id);

    }
}
