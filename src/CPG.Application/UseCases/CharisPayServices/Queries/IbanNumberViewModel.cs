using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.CharisPayServices.Queries
{
    public class IbanViewModel
    {
        [Required]
        public string Iban { get; set; }
    }
}
