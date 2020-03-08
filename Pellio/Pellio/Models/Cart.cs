using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pellio.Models
{
    public class Cart
    {
        public int Id { get; set; }
        //[Display(Name = "Product Name")]
        //public string ProductName { get; set; }
        //public string Ingredients { get; set; }
        //public decimal Price { get; set; }
        public List<Products> Products { get; set; }
    }
}
