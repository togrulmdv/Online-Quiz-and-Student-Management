using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class users
    {
        [Key]         
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int user_id { get; set; }
		public string? first_name { get; set; }
		public string? last_name { get; set; }
		public string? user_name { get; set; }

		public string? email { get; set; }
		public string? nationality { get; set; }
		public string? mobile_no { get; set; }
		public string? residence_no { get; set; }
		public string? address { get; set; }

		public string? password { get; set; }
		public int user_type { get; set; }
		public int is_active { get; set; }
		public string? guid { get; set; }
		public DateTime? created_at { get; set; }
		public int? created_by { get; set; }
		public DateTime? modified_at { get; set; }
		public int? modified_by { get; set; }
		public Boolean? is_deleted { get; set; }

		//Not mapped fields
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

		[NotMapped]
		public string? submit_status { get; set; }

	}
}
