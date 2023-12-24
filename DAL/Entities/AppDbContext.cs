using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
     : base(options)
        {
        }

        public DbSet<assignment_answers>? assignment_answers { get; set; }
        public DbSet<assignments>? assignments { get; set; }
        public DbSet<course_categories>? course_categories { get; set; }
        public DbSet<courses>? courses { get; set; }
        public DbSet<quiz>? quiz { get; set; }
        public DbSet<quiz_categories>? quiz_categories { get; set; }
        public DbSet<quiz_questions>? quiz_questions { get; set; }
        public DbSet<status>? status { get; set; }
        public DbSet<student_answers>? student_answers { get; set; }
        public DbSet<student_courses>? student_courses { get; set; }
       
        public DbSet<teacher_assign_courses>? teacher_assign_courses { get; set; }
        public DbSet<users>? users { get; set; }

        public DbSet<error_log>? error_log { get; set; }
        public DbSet<QueryResponse>? QueryResponse { get; set; }
        public DbSet<assignment_remaining_time>? assignment_remaining_time { get; set; }
        public DbSet<quiz_remaining_time>? quiz_remaining_time { get; set; }
        public DbSet<user_types>? user_types { get; set; }
    }
}
