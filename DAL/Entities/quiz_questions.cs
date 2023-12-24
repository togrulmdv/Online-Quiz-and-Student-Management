using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
  public  class quiz_questions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int question_id { get; set; }
        public int quiz_id { get; set; }
		public string? question_title { get; set; }
		public string? option_a { get; set; }
		public string? option_b { get; set; }
		public string? option_c { get; set; }
		public string? option_d { get; set; }
		public string? correct_option { get; set; }

		[NotMapped]
		public string? student_ans { get; set; }
		public string? guid { get; set; }

		public DateTime? created_at { get; set; }

		public int? created_by { get; set; }

		public DateTime? modified_at { get; set; }
		public int? modified_by { get; set; }
		public Boolean? is_deleted { get; set; }
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
