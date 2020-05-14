namespace Pellio.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Keeps track of ordered items (cart).
    /// </summary>
    public class OrdersList
    {
        /// <summary>
        /// Gets or sets Id value.
        /// </summary>
        /// <value>Always unique.</value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets Total value.
        /// </summary>
        /// <value>Always unique.</value>
        public decimal Total { get; set; }

        /// <summary>
        /// Gets or sets UserId value.
        /// </summary>
        /// <value>Always unique.</value>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets TimeMade value.
        /// </summary>
        /// <value>Always unique.</value>
        public string TimeMade { get; set; }

        /// <summary>
        /// Gets or sets Products value.
        /// </summary>
        /// <value>Always unique.</value>
        public List<Products> Products { get; set; }

        /// <summary>
        /// Gets or sets PercentOffCodeId value.
        /// </summary>
        /// <value>Always unique.</value>
        public int PercentOffCodeId { get; set; }

        /// <summary>
        /// Gets or sets PercentOffCode value.
        /// </summary>
        /// <value>Always unique.</value>
        public PercentOffCode PercentOffCode { get; set; }
    }
}
