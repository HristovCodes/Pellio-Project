namespace Pellio.Models
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Comments model that contains all comments.
    /// </summary>
    public class Comments
    {
        /// <summary>
        /// Gets or sets Id value.
        /// </summary>
        /// <value>Always unique.</value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets Name value.
        /// </summary>
        /// <value>Always unique.</value>
        [Display(Name = "Username")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets Comment value.
        /// </summary>
        /// <value>Always unique.</value>
        [Display(Name = "Review")]
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets Score value.
        /// </summary>
        /// <value>Always unique.</value>
        public double Score { get; set; }

        /// <summary>
        /// Gets or sets ProductsId value.
        /// </summary>
        /// <value>Always unique.</value>
        public int ProductsId { get; set; }

        /// <summary>
        /// Gets or sets Products value.
        /// </summary>
        /// <value>Always unique.</value>
        public Products Products { get; set; }
    }
}
