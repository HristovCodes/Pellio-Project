using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pellio.Models
{
    public class EmailCredentials
    {//stored email and password for when the user orders
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
