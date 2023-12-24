using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
	public class teacher_assign_courses
	{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int teacher_assign_id { get; set; }
		public int? user_id { get; set; }
		public int? course_id { get; set; }

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
