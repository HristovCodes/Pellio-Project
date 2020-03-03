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
        public string Name { get; set; }
        public string Comment { get; set; }

        public int ProductId { get; set; }
        public Products Products { get; set; }
    }
}
