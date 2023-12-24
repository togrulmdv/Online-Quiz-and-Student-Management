using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.CommonModels
{
    [NotMapped]
    public class student_result
    {
        [NotMapped]
        public int student_id { get; set; }
        [NotMapped]
        public string? first_name { get; set; }
        [NotMapped]
        public string? last_name { get; set; }
        [NotMapped]
        public string? user_name { get; set; }
        [NotMapped]
        public string? email { get; set; }

        [NotMapped]
        public int quiz_id { get; set; }


        [NotMapped]
        public int passing_marks { get; set; }

        [NotMapped]
        public int total_questions { get; set; }

        [NotMapped]
        public int total_correct_ans { get; set; }

        [NotMapped]
        public int std_percentage { get; set; }


        //--extra fields
        [NotMapped]
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




    }
}
