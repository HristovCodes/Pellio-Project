using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pellio.Models
{
    public class PercentOffCode
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public decimal Percentage { get; set; }
        public bool Available { get; set; }
        public int OrdersListId { get; set; }
        public OrdersList OrdersList { get; set; }
    }
}
