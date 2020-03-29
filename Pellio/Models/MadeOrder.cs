using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pellio.Models
{
    public class MadeOrder
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } 
        public string CustomerAddress { get; set; }
        public string CustomerEmail { get; set; }
        public int CustomerPhoneNumber { get; set; }
        public List<Products> Products { get; set; }
    }
}
