using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Pellio.Models
{
    public class Products
    {
        public int Id { get; set; }
        [Display(Name = "Product Name")]
        public string ProductName { get; set; }
        public string Ingredients { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public List<Comments> Comments { get; set; }
        public List<Ingredient> ListOfIngredients { get; set; }

    }
}
