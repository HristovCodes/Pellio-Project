using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pellio.Models
{
    public class ProductsOrderList
    {
        public int ProductsId { get; set; }
        public int OrderListId { get; set; }
        public Products Products { get; set; }
        public OrdersList OrdersList { get; set; }
    }
}
