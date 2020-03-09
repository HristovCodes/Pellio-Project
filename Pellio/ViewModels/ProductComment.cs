using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pellio.Models;

namespace Pellio.ViewModels
{
    public class ProductComment
    {
        public Products Products { get; set; }
        public Comments Comments { get; set; }
    }
}
