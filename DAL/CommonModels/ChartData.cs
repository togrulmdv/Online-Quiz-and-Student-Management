using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.CommonModels
{
    [NotMapped]
    public class ChartData
    {
        public string? Label { get; set; }
        public string? Data { get; set; }
    }

    [NotMapped]
    public class FooterData
    {
        public string? TotalStudents { get; set; }
        public string? TotalCourses { get; set; }
    }

}
