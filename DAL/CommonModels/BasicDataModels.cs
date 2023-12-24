using DAL.Entities;
using DAL.Helpers.PageHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.CommonModels
{
   public class BasicDataModels
    {
        #region Declare Data Class Objects
        public quiz_categories quiz_categories_obj { get; set; }
        public IEnumerable<quiz_categories> quiz_categories_list { get; set; }

        public quiz quiz_obj { get; set; }
        public IEnumerable<quiz> quiz_list { get; set; }
        public assignments assignment_obj { get; set; }
        public IEnumerable<assignments> assignment_list { get; set; }

        public quiz_questions quiz_ques_obj { get; set; }
        public IEnumerable<quiz_questions> quiz_ques_list { get; set; }
        public courses courses_obj { get; set; }
        public IEnumerable<courses> courses_list { get; set; }

        public student_result student_result_obj { get; set; }
        public IEnumerable<student_result> student_result_list { get; set; }

        public student_assignment_result std_assignment_result_obj { get; set; }
        public IEnumerable<student_assignment_result> std_assignment_result_list { get; set; }

        public course_categories course_categories_obj { get; set; }
        public IEnumerable<course_categories> course_categories_list { get; set; }

        public users student_obj { get; set; }
        public IEnumerable<users> student_list { get; set; }

        public users user_obj { get; set; }
        public GeneralData generalDataObj { get; set; }
        public IEnumerable<users> user_list { get; set; }
        public student_courses student_courses_obj { get; set; }
        public IEnumerable<ChartData> ChartDataList { get; set; }
        public IEnumerable<student_courses> student_courses_list { get; set; }
        public PagerHelper? pageHelperObj { get; set; }

        #endregion
    }
}
