using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pellio.Models
{
    public class OrdersList
    {
        public int Id { get; set; }
        public decimal Total { get; set; }
        public string UserId { get; set; }
        public string TimeMade { get; set; }

        public List<Products> Products { get; set; }
    }
}
