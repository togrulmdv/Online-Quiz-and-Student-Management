using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.CommonModels
{
    [NotMapped]
    public class GeneralData
    {
        public string? PageTitle { get; set; }
    }
}
