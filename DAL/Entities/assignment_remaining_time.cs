using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class assignment_remaining_time
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int assign_rem_time_id { get; set; }
        public int assignment_id { get; set; }
        public int student_id { get; set; }
        public int remaining_minutes { get; set; }
    }
}
