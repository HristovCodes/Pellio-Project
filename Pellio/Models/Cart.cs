using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Pellio.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public decimal Total { get; set; }
        public string UserId { get; set; }

        public List<Products> Products { get; set; }
    }
}
