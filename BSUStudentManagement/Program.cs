using BSUStudentManagement.CommonCode;
using DAL.ConnectionManager;
using DAL.Helpers.PageHelpers;
using DAL.Services;
using DAL.DBContext;
using BSUStudentManagement.CommonCode;
using PetaPoco;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("OnlineQuizConn")));


//--register classes
builder.Services.AddSingleton<IDatabase, Database>();
builder.Services.AddSingleton<IDataContextHelper, DataContextHelper>();
builder.Services.AddSingleton<IConstants, Constants>();
builder.Services.AddSingleton<ISessionManager, SessionManager>();
builder.Services.AddSingleton<IAdminServices, AdminServices>();
builder.Services.AddSingleton<ITeacherServices, TeacherServices>();
builder.Services.AddSingleton<IStudentServices, StudentServices>();

//--For session and cookies purpose
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddScoped<AdminAuthorization>();
builder.Services.AddScoped<TeacherAuthorization>();
builder.Services.AddScoped<StudentAuthorization>();


// Add services to the container.
builder.Services.AddControllersWithViews();



//--Session and cookies settings
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(60);//You can set Time  in seconds, minutes 
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

//---For session purpose
app.UseSession();

app.UseRouting();

app.UseAuthorization();

#region routing
//app.MapAreaControllerRoute(
//    name: "MyAreaAdmin",
//    areaName: "Admin",
//    pattern: "Admin/{controller=AdminAccount}/{action=Login}");

app.MapAreaControllerRoute(
    name: "AdminLogin",
    areaName: "Admin",
    pattern: "Admin/Login",
     defaults: new { controller = "AdminAccount", action = "Login" });

app.MapAreaControllerRoute(
    name: "AdminDashboard",
    areaName: "Admin",
    pattern: "Admin/Home",
     defaults: new { controller = "AdminHome", action = "Dashboard" });


app.MapAreaControllerRoute(
    name: "AdminCourses",
    areaName: "Admin",
    pattern: "Admin/courses/{pageId}",
     defaults: new { controller = "AdminHome", action = "Courses" });

app.MapAreaControllerRoute(
    name: "AdminStudentsList",
    areaName: "Admin",
    pattern: "Admin/students-list/{pageId}",
     defaults: new { controller = "AdminHome", action = "StudentsList" });

app.MapAreaControllerRoute(
    name: "AdminTeachersList",
    areaName: "Admin",
    pattern: "Admin/teachers-list/{pageId}",
     defaults: new { controller = "AdminHome", action = "TeachersList" });



app.MapAreaControllerRoute(
    name: "AdminStudentRegisteredCourses",
    areaName: "Admin",
    pattern: "Admin/students-courses/{student_id}/{pageId}",
     defaults: new { controller = "AdminHome", action = "StudentRegisteredCourses" });

app.MapAreaControllerRoute(
    name: "AdminTeacherAssignCourses",
    areaName: "Admin",
    pattern: "Admin/teacher-courses/{user_id}/{pageId}",
    defaults: new { controller = "AdminHome", action = "TeacherAssignCourses" });

app.MapAreaControllerRoute(
    name: "AdminQuizCategories",
    areaName: "Admin",
    pattern: "Admin/quiz/categories/{pageId}",
    defaults: new { controller = "AdminBasicData", action = "QuizCategories" });

app.MapAreaControllerRoute(
    name: "AdminCourseCategories",
    areaName: "Admin",
    pattern: "Admin/course/categories/{pageId}",
    defaults: new { controller = "AdminBasicData", action = "CourseCategories" });



app.MapAreaControllerRoute(
    name: "Teacher_Quiz_list",
    areaName: "Teachers",
    pattern: "Teachers/quiz/list/{course_id}/{pageId}",
    defaults: new { controller = "TeacherHome", action = "QuizList" });

app.MapAreaControllerRoute(
    name: "Teacher_Assignment_list",
    areaName: "Teachers",
    pattern: "Teachers/assignment/list/{course_id}/{pageId}",
    defaults: new { controller = "TeacherHome", action = "AssignmentList" });

app.MapAreaControllerRoute(
    name: "Teacher_Add_Quiz",
    areaName: "Teachers",
    pattern: "Teachers/add-quiz",
    defaults: new { controller = "TeacherHome", action = "AddQuiz" });

app.MapAreaControllerRoute(
    name: "Teacher_Add_Assignment",
    areaName: "Teachers",
    pattern: "Teachers/add-assignment",
    defaults: new { controller = "TeacherHome", action = "AddAssignment" });


app.MapAreaControllerRoute(
    name: "Teacher_Students_List",
    areaName: "Teachers",
    pattern: "Teachers/students-list/{course_id}/{pageId}",
    defaults: new { controller = "TeacherHome", action = "StudentListInCourses" });

app.MapAreaControllerRoute(
    name: "Teacher_Quiz_Detail",
    areaName: "Teachers",
    pattern: "Teachers/quiz-detail/{quiz_id}",
    defaults: new { controller = "TeacherHome", action = "QuizDetail" });


app.MapAreaControllerRoute(
    name: "Teacher_Assignment_Detail",
    areaName: "Teachers",
    pattern: "Teachers/assignment-detail/{assignment_id}",
    defaults: new { controller = "TeacherHome", action = "AssignmentDetail" });


app.MapAreaControllerRoute(
    name: "Teacher_Courses_List",
    areaName: "Teachers",
    pattern: "Teachers/courses/{pageId}",
    defaults: new { controller = "TeacherHome", action = "Courses" });


app.MapAreaControllerRoute(
    name: "Teacher_Student_Quiz_Result_Detail",
    areaName: "Teachers",
    pattern: "Teachers/student-result-detail/{quiz_id}/{student_id}",
    defaults: new { controller = "TeacherHome", action = "StudentResultDetail" });

app.MapAreaControllerRoute(
    name: "Teacher_Student_Assignment_Result_Detail",
    areaName: "Teachers",
    pattern: "Teachers/assignment-result-detail/{assign_answers_id}/{student_id}",
    defaults: new { controller = "TeacherHome", action = "StudentAssignmentResultDetail" });

app.MapAreaControllerRoute(
    name: "Teacher_Home_Dashboard",
    areaName: "Teachers",
    pattern: "Teachers/home",
    defaults: new { controller = "TeacherHome", action = "Index" });

app.MapAreaControllerRoute(
    name: "Users_Login_Login",
    areaName: "Users",
    pattern: "Users/UserAccount/StudentLogin",
    defaults: new { controller = "UserAccount", action = "StudentLogin" });

app.MapAreaControllerRoute(
    name: "UsersQuizList",
    areaName: "Users",
    pattern: "Users/home/quiz-list/{course_id}/{pageId}",
    defaults: new { controller = "UserHome", action = "StudentQuizList" });

app.MapAreaControllerRoute(
    name: "UsersAssignment_List",
    areaName: "Users",
    pattern: "Users/home/assignment-list/{course_id}/{pageId}",
    defaults: new { controller = "UserHome", action = "StudentAssignmentList" });

app.MapAreaControllerRoute(
    name: "UsersHome_Page",
    areaName: "Users",
    pattern: "Users/home",
    defaults: new { controller = "UserHome", action = "Index" });

app.MapAreaControllerRoute(
    name: "UsersCourses_List",
    areaName: "Users",
    pattern: "Users/courses/{pageId}",
    defaults: new { controller = "UserHome", action = "CoursesList" });

app.MapAreaControllerRoute(
    name: "UsersCourses_List",
    areaName: "Users",
    pattern: "Users/home/start-quiz/{quiz_ID}",
    defaults: new { controller = "UserHome", action = "StartQuiz" });

app.MapAreaControllerRoute(
    name: "UsersCourses_List",
    areaName: "Users",
    pattern: "Users/home/start-quiz/{quiz_ID}",
    defaults: new { controller = "UserHome", action = "StartQuiz" });

app.MapAreaControllerRoute(
    name: "UsersStart_Assignment",
    areaName: "Users",
    pattern: "Users/home/start-assignment/{assignment_id}",
    defaults: new { controller = "UserHome", action = "StartAssignment" });




app.MapAreaControllerRoute(
    name: "MyAreaAdmin",
    areaName: "Admin",
    pattern: "Admin/{controller=AdminAccount}/{action=Login}");


app.MapAreaControllerRoute(
    name: "MyAreaTeacher",
    areaName: "Teachers",
    pattern: "Teachers/{controller=TeacherHome}/{action=Index}");

app.MapAreaControllerRoute(
    name: "MyAreaUsers",
    areaName: "Users",
    pattern: "Users/{controller=UserHome}/{action=Index}");


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
#endregion

app.Run();
