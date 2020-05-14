namespace Pellio.Models
{
    /// <summary>
    /// Stored email and password for when the user orders.
    /// </summary>
    public class EmailCredentials
    {
        /// <summary>
        /// Gets or sets Id value.
        /// </summary>
        /// <value>Always unique.</value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets Email value.
        /// </summary>
        /// <value>Always unique.</value>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets Password value.
        /// </summary>
        /// <value>Always unique.</value>
        public string Password { get; set; }
    }
}
