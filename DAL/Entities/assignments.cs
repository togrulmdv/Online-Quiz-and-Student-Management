using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class assignments
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int assignment_id { get; set; }
        public string? title { get; set; }
        public string? body { get; set; }
        public string? start_date { get; set; }
        public string? end_date { get; set; }
        public int? allowed_time_minutes { get; set; }
        public int? status { get; set; } 
        public string? status_title { get; set; }


        public int course_id { get; set; }
      
        public string? description { get; set; }
        public int total_marks { get; set; }  //like total marks: 120
        public int passing_marks { get; set; }//like passing marks 55 out of 120
       
        public string? guid { get; set; }
        public DateTime? created_at { get; set; }

        public int? created_by { get; set; }

        public DateTime? modified_at { get; set; }


        public int? modified_by { get; set; }


        public Boolean? is_deleted { get; set; }

        //Not mapped fields
        [NotMapped]
        public string? course_name { get; set; }
        public string? resultMsg { get; set; }
        [NotMapped]
        public bool IsSuccess { get; set; }
        [NotMapped]

        public string? ReturnURL { get; set; }
        [NotMapped]

        public int TotalRecordCount { get; set; }
        [NotMapped]
        public int pageId { get; set; }
        [NotMapped]
        public int ItemsPerPage { get; set; }
        [NotMapped]
        public string? created_by_username { get; set; }

        [NotMapped]
        public string? submit_status { get; set; }  //whether student submit this assignment or not [NotMapped]
        [NotMapped]
        public int is_checked_by_teacher { get; set; }  //either the teacher mark the assignment for a specific student or not
        [NotMapped]
        public string? student_ans { get; set; }  //student answer from "assignment_answers" table to the required assignment

        [NotMapped]
        public int assign_answers_id { get; set; }  //assign_answers_id from "assignment_answers" table to the required assignment for a specific student. this row not exist in the actual assignments table

        [NotMapped]
        public int gain_marks { get; set; }  //a column from "assignment_answers" table
    }
}
