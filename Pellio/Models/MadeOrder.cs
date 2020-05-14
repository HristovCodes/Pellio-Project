namespace Pellio.Models
{
    /// <summary>
    /// Keeps track all succesful orders.
    /// </summary>
    public class MadeOrder
    {
        /// <summary>
        /// Gets or sets Id value.
        /// </summary>
        /// <value>Always unique.</value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets CustomerName value.
        /// </summary>
        /// <value>Always unique.</value>
        public string CustomerName { get; set; }

        /// <summary>
        /// Gets or sets CustomerAddress value.
        /// </summary>
        /// <value>Always unique.</value>
        public string CustomerAddress { get; set; }

        /// <summary>
        /// Gets or sets CustomerEmail value.
        /// </summary>
        /// <value>Always unique.</value>
        public string CustomerEmail { get; set; }

        /// <summary>
        /// Gets or sets CustomerPhoneNumber value.
        /// </summary>
        /// <value>Always unique.</value>
        public int CustomerPhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets UserId value.
        /// </summary>
        /// <value>Always unique.</value>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets TimeOfOrder value.
        /// </summary>
        /// <value>Always unique.</value>
        public string TimeOfOrder { get; set; }

        /// <summary>
        /// Gets or sets FinalPrice value.
        /// </summary>
        /// <value>Always unique.</value>
        public decimal FinalPrice { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the order is complete or not.
        /// </summary>
        /// <value>Always unique.</value>
        public bool Complete { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the order is canceled or not.
        /// </summary>
        /// <value>Always unique.</value>
        public bool Canceled { get; set; }

        /// <summary>
        /// Gets or sets Products_names value.
        /// </summary>
        /// <value>Always unique.</value>
        public string Products_names { get; set; }
    }
}
