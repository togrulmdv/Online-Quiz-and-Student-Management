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
    public class StudentServices : IStudentServices
    {

        private readonly IConfiguration _configuration;
        private readonly IDatabase _db;
        private readonly IDataContextHelper _contextHelper;


  

        public StudentServices(IConfiguration configuration, IDatabase db, IDataContextHelper contextHelper)
        {
            _configuration = configuration;
            _db = db;
            _contextHelper = contextHelper;
        }


        public void LogErrorStudentInDatabase(string? error_message, string? log_type)
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
                    LogErrorStudentInDatabase(ex.Message, "DAL Student Service error");
                    return result;
                }


            }

            return result; ;

        }

        //--get course categories for drop down in forms
        public List<courses> GetCoursesForDropDownByStudentID(int? student_id)
        {
            List<courses> result = new List<courses>();


            using (var repo = _contextHelper.GetPPContextHelper())
            {
                try
                {




                    var ppSql = PetaPoco.Sql.Builder.Select(@" cr.course_id, cr.title, std_cr.student_id")
                        .From(" courses AS cr")
                        .InnerJoin("student_courses AS std_cr").On("cr.course_id = std_cr.course_id")
                        .Where("std_cr.student_id = @0", student_id)
                        .OrderBy("cr.course_id desc");



                    result = repo.Query<courses>(ppSql).ToList();


                }
                catch (Exception ex)
                {
                    LogErrorStudentInDatabase(ex.Message, "DAL Student Service error");
                    return (result);
                }



            }
            return (result);

        }


        public List<quiz> GetQuizListForStudentExamByCourseID(int? pageId, int pageSize, int? student_id, int? course_id, string? title)
        {
            List<quiz> result = new List<quiz>();


            using (var repo = _contextHelper.GetPPContextHelper())
            {
                try
                {

                    #region Create Search Parameters
                    var SearchParameters = PetaPoco.Sql.Builder.Append(" ");

                    if (!String.IsNullOrEmpty(title))
                    {
                        SearchParameters.Append("and qz.title LIKE @0 ", "%" + title + "%");
                    }

                    #endregion


                    var ppSql = PetaPoco.Sql.Builder.Select(@"distinct COUNT(*) OVER () as TotalRecordCount, qz.quiz_id ,sbSts.submit_status   ,qz.title, CASE WHEN getdate() BETWEEN qz.start_date AND qz.end_date THEN '1' ELSE '0' END AS status  ")
                       .From("quiz qz")
                       .InnerJoin("student_courses stdC").On("qz.course_id=stdC.course_id")
                       //.Append("outer apply(")
                         //.Append(" select top 1 qq.quiz_id, qq.question_id from quiz_questions qq where qz.quiz_id=qq.quiz_id ")
                          // .Append(" ) as kk ")
                            .Append("outer apply(")
                              .Append("select top 1 count(*) as submit_status from student_answers qm  ")
                                .Append(" where qm.question_id in (select  qq.question_id from quiz_questions qq where qz.quiz_id=qq.quiz_id) and qm.student_id=@0 ", student_id)
                                  .Append(" ) as sbSts ")
                                  .Where("qz.course_id = @0 and stdC.student_id=@1", course_id, student_id)
                          .Append(SearchParameters)
                        .OrderBy("  status desc")
                          .Append(@"OFFSET (@0-1)*@1 ROWS
	                        FETCH NEXT @1 ROWS ONLY", pageId, pageSize);


                    result = repo.Query<quiz>(ppSql).ToList();


                }
                catch (Exception ex)
                {
                    LogErrorStudentInDatabase(ex.Message, "DAL Student Service error");
                    return (result);
                }



            }
            return (result);

        }


        //--save answer of question
        public string? SaveQuizAnswer(int student_id, int question_id, string? correct_option, string? guid)
        {
            string? result = "";
            using (var repo = _contextHelper.GetPPContextHelper())
            {

                try
                {
                    if (student_id > 0 || question_id > 0 || !String.IsNullOrEmpty(correct_option))
                    {

                        var response = repo.Execute(@"if exists(select * from student_answers where student_id=@0 AND question_id=@1 )
                                                begin
                                                update student_answers
                                                set student_ans=@2, modified_at=getdate(), modified_by=@0 where question_id=@1 AND student_id=@0;
                                                end
                                                else
                                                begin
                                                INSERT into student_answers(student_id, question_id, student_ans, guid, created_at, created_by)
                                                VALUES(@0, @1, @2, @3, getdate(), @0);
                            end", student_id, question_id, correct_option, guid);

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
                    LogErrorStudentInDatabase(ex.Message, "DAL Student Service error");
                    result = "Error occured in processing your request!";
                    return result;
                }


            }
            return result;


        }

        //--update quiz remaining time in database
        public string? updateQuizRemainingTime(int quiz_ID, int student_id, int remaining_minutes)
        {
            string? result = "";
            using (var repo = _contextHelper.GetPPContextHelper())
            {

                try
                {
                    if (quiz_ID > 0)
                    {

                        var response = repo.Execute(@"if exists(select * from quiz_remaining_time where quiz_id=@1 AND student_id=@0)
                        begin
                        update quiz_remaining_time
                        set remaining_minutes=@2 where quiz_id=@1 AND student_id=@0;
                        end
                        else
                        begin
                        INSERT into quiz_remaining_time(quiz_id, student_id, remaining_minutes)
		                        VALUES(@1, @0, @2);
                      end", student_id, quiz_ID, remaining_minutes);

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
                    LogErrorStudentInDatabase(ex.Message, "DAL Student Service error");
                    result = "Error occured in processing your request!";
                    return result;
                }


            }
            return result;


        }


        public List<courses> GetStudentMainCoursesListByStudentID(int? pageId, int pageSize, string? title, int? status, int? course_category_id, int? student_id)
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
                         .InnerJoin(" student_courses AS sc").On("cr.course_id = sc.course_id")
                        .LeftJoin(" course_categories AS ct").On("cr.course_category_id = ct.course_category_id")
                        .LeftJoin("status AS st").On("cr.status = st.status_id")
                        .Where("cr.is_deleted = 0 AND sc.student_id=@0", student_id)
                        .Append(SearchParameters)
                        .OrderBy("cr.course_id desc")
                       .Append(@"OFFSET (@0-1)*@1 ROWS
	                        FETCH NEXT @1 ROWS ONLY", pageId, pageSize);


                    result = repo.Query<courses>(ppSql).ToList();


                }
                catch (Exception ex)
                {
                    LogErrorStudentInDatabase(ex.Message, "DAL Student Service error");
                    return (result);
                }



            }
            return (result);

        }


        public student_result GetStudentsResultDetail(int student_id,int quiz_id)
        {
            student_result result = new student_result();


            using (var repo = _contextHelper.GetPPContextHelper())
            {
                try
                {




                    var ppSql = PetaPoco.Sql.Builder.Select(@"top 1 quiz_q.quiz_id , std.user_id,isnull(quiz.passing_marks,50) as passing_marks,
                         count(quiz_q.question_id) as total_questions, 
                        count(quiz_q_correct.correct_option) as total_correct_ans,
                         (select count(quiz_q_correct.correct_option)*100/count(quiz_q.question_id) ) as std_percentage")
                        .From("student_answers AS sa")
                        .InnerJoin("quiz_questions AS quiz_q").On("sa.question_id = quiz_q.question_id ")
                         .InnerJoin("users AS std").On("sa.student_id = std.user_id ")
                          .LeftJoin("quiz_questions AS quiz_q_correct").On("sa.question_id = quiz_q_correct.question_id and sa.student_ans = quiz_q_correct.correct_option ")
                          .InnerJoin("quiz as quiz").On("quiz_q.quiz_id=quiz.quiz_id ")
                          .Where("quiz_q.quiz_id=@0 AND sa.student_id=@1", quiz_id, student_id)
                          .GroupBy("quiz_q.quiz_id, std.user_id,  quiz.passing_marks")
                        .OrderBy("std.user_id");
                   


                    result = repo.Query<student_result>(ppSql).FirstOrDefault();

                }
                catch (Exception ex)
                {
                    LogErrorStudentInDatabase(ex.Message, "DAL Student Service error");
                    return result;
                }


            }

            return result; ;

        }


        public List<assignments> GetAssignmentListForStudentByCourseID(int? pageId, int pageSize, int? student_id, int? course_id, string? title)
        {
            List<assignments> result = new List<assignments>();


            using (var repo = _contextHelper.GetPPContextHelper())
            {
                try
                {

                    #region Create Search Parameters
                    var SearchParameters = PetaPoco.Sql.Builder.Append(" ");

                    if (!String.IsNullOrEmpty(title))
                    {
                        SearchParameters.Append("and asgn.title LIKE @0 ", "%" + title + "%");
                    }

                    #endregion


                    var ppSql = PetaPoco.Sql.Builder.Select(@" distinct COUNT(*) OVER () as TotalRecordCount,   asgn.assignment_id, asgn.title, CASE WHEN getdate() BETWEEN start_date AND end_date THEN '1' ELSE '0' END AS status,  isnull(outAns.is_checked_by_teacher,0) as is_checked_by_teacher,")
                        .Append("(")
                        .Select(@"   top 1 count(*)")
                        .From(" assignment_answers sa")
                        .InnerJoin(" assignments asg").On("sa.assignment_id=asg.assignment_id")
                        .Where(" sa.student_id=@0 and  asg.course_id=@1", student_id, course_id)
                        .Append(" )  as submit_status")
                        .From(" assignments asgn")
                        .InnerJoin("student_courses sc").On("asgn.course_id=sc.course_id")
                        .LeftJoin("assignment_answers outAns").On("asgn.assignment_id=outAns.assignment_id")
                        .Where(" asgn.course_id = @0 and sc.student_id=@1", course_id, student_id)
                          .Append(SearchParameters)
                        .OrderBy(" asgn.assignment_id desc")
                          .Append(@"OFFSET (@0-1)*@1 ROWS
	                        FETCH NEXT @1 ROWS ONLY", pageId, pageSize);


                    result = repo.Query<assignments>(ppSql).ToList();


                }
                catch (Exception ex)
                {
                    LogErrorStudentInDatabase(ex.Message, "DAL Student Service error");
                    return (result);
                }



            }
            return (result);

        }

        public student_assignment_result GetStudentAssignmentResultByAssignmentID(int student_id, int assignment_id)
        {
            student_assignment_result result = new student_assignment_result();


            using (var repo = _contextHelper.GetPPContextHelper())
            {
                try
                {




                    var ppSql = PetaPoco.Sql.Builder.Select(@"top 1 an.total_marks,ans.student_id, an.passing_marks, ans.gain_marks, IIF(ans.gain_marks>=passing_marks, 1, 0) as passStatus")
                        .From(" assignment_answers ans")

                         .InnerJoin("assignments an").On("ans.assignment_id=an.assignment_id ")
                        
                          .Where("an.assignment_id=@0 and ans.student_id=@1", assignment_id, student_id);
   
                   



                    result = repo.Query<student_assignment_result>(ppSql).FirstOrDefault();

                }
                catch (Exception ex)
                {
                    LogErrorStudentInDatabase(ex.Message, "DAL Student Service error");
                    return result;
                }


            }

            return result; ;

        }



        public assignments GetAssignmentDetailForStudentByID(int? assignment_id,int student_id)
        {
            assignments result = new assignments();


            using (var repo = _contextHelper.GetPPContextHelper())
            {
                try
                {




                    var ppSql = PetaPoco.Sql.Builder.Select(@"top 1   qz.assignment_id, qz.title, qz.body , outAns.student_ans , qz.start_date, qz.end_date, IIF(rt.remaining_minutes>-1, rt.remaining_minutes, qz.allowed_time_minutes) as allowed_time_minutes")
                        .From("assignments qz")
                        .LeftJoin("assignment_remaining_time rt").On("qz.assignment_id = rt.assignment_id")
                           .LeftJoin("assignment_answers outAns").On("qz.assignment_id=outAns.assignment_id and outAns.student_id=@0", student_id)
                        .Where("qz.assignment_id=@0 and qz.is_deleted!=1", assignment_id);




                    result = repo.Query<assignments>(ppSql).FirstOrDefault();

                }
                catch (Exception ex)
                {
                    LogErrorStudentInDatabase(ex.Message, "DAL Student Service error");
                    return result;
                }


            }

            return result; ;

        }

        //--update quiz remaining time in database
        public string? updateAssignmentRemainingTime(int assignment_id, int student_id, int remaining_minutes)
        {
            string? result = "";
            using (var repo = _contextHelper.GetPPContextHelper())
            {

                try
                {
                    if (assignment_id > 0)
                    {

                        var response = repo.Execute(@"if exists(select * from assignment_remaining_time where assignment_id=@1 AND student_id=@0)
                        begin
                        update assignment_remaining_time
                        set remaining_minutes=@2 where assignment_id=@1 AND student_id=@0;
                        end
                        else
                        begin
                        INSERT into assignment_remaining_time(assignment_id, student_id, remaining_minutes)
		                        VALUES(@1, @0, @2);
                      end", student_id, assignment_id, remaining_minutes);

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
                    LogErrorStudentInDatabase(ex.Message, "DAL Student Service error");
                    result = "Error occured in processing your request!";
                    return result;
                }


            }
            return result;


        }


        public string? SaveStudentAssignmentAnswer(assignment_answers FormData)
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


                        var response = repo.Execute("INSERT INTO assignment_answers ( assignment_id, student_id, student_ans, guid, created_at, created_by, is_deleted )" +
                            "  VALUES (@0, @1, @2, @3,getdate() ,@4 , 0)",
                            FormData.assignment_id, FormData.created_by, FormData.student_ans, FormData.guid, FormData.created_by);

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
                    LogErrorStudentInDatabase(ex.Message, "DAL Student Service error");
                    result = "Error occured in processing your request!";
                    return result;
                }


            }
            return result;


        }
    }

    public interface IStudentServices
    {
     
        public void LogErrorStudentInDatabase(string? error_message, string? log_type);
        public List<quiz_questions> GetQuestionListByQuizId(int? quiz_id);
        public List<courses> GetCoursesForDropDownByStudentID(int? student_id);
        public List<quiz> GetQuizListForStudentExamByCourseID(int? pageId, int pageSize, int? student_id, int? course_id, string? title);
        public string? SaveQuizAnswer(int student_id, int question_id, string? correct_option, string? guid);
        public string? updateQuizRemainingTime(int quiz_ID, int student_id, int remaining_minutes);
        public List<courses> GetStudentMainCoursesListByStudentID(int? pageId, int pageSize, string? title, int? status, int? course_category_id, int? student_id);
        public student_result GetStudentsResultDetail(int student_id, int quiz_id);
        public List<assignments> GetAssignmentListForStudentByCourseID(int? pageId, int pageSize, int? student_id, int? course_id, string? title);
        public student_assignment_result GetStudentAssignmentResultByAssignmentID(int student_id, int assignment_id);
        public assignments GetAssignmentDetailForStudentByID(int? assignment_id, int student_id);
        public string? updateAssignmentRemainingTime(int assignment_id, int student_id, int remaining_minutes);
        public string? SaveStudentAssignmentAnswer(assignment_answers FormData);

    }
}
