using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Pellio.Models
{
    public class Comments
    {
        public int Id { get; set; }
        [Display(Name = "Username")]
        public string Name { get; set; }
        [Display(Name = "Review")]
        public string Comment { get; set; }

        public int ProductsId { get; set; }
        public Products Products { get; set; }
    }
}
