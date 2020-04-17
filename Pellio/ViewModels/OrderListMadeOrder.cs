using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pellio.Models;

namespace Pellio.ViewModels
{
    public class OrderListMadeOrder
    {
        public OrdersList OrdersList { get; set; }
        public List<MadeOrder> MadeOrder { get; set; }
    }
}
