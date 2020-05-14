namespace Pellio.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Model containing all products.
    /// </summary>
    public class Products
    {
        /// <summary>
        /// Gets or sets Id value.
        /// </summary>
        /// <value>Always unique.</value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets ProductName value.
        /// </summary>
        /// <value>Always unique.</value>
        [Display(Name = "Product Name")]
        public string ProductName { get; set; }

        /// <summary>
        /// Gets or sets Ingredients value.
        /// </summary>
        /// <value>Always unique.</value>
        public string Ingredients { get; set; }

        /// <summary>
        /// Gets or sets Price value.
        /// </summary>
        /// <value>Always unique.</value>
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets ImageUrl value.
        /// </summary>
        /// <value>Always unique.</value>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets Tag value.
        /// </summary>
        /// <value>Always unique.</value>
        public string Tag { get; set; }

        /// <summary>
        /// Gets or sets Comments value.
        /// </summary>
        /// <value>Always unique.</value>
        public List<Comments> Comments { get; set; }

        /// <summary>
        /// Gets or sets ListOfIngredients value.
        /// </summary>
        /// <value>Always unique.</value>
        public List<Ingredient> ListOfIngredients { get; set; }
    }
}
