using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class error_log
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int erro_log_id { get; set; }
        public string? log_message { get; set; }
        public string? log_type { get; set; }
        public DateTime? created_at { get; set; }
    }
}
