namespace Pellio.Models
{
    /// <summary>
    /// Model for percent off codes.
    /// </summary>
    public class PercentOffCode
    {
        /// <summary>
        /// Gets or sets Id value.
        /// </summary>
        /// <value>Always unique.</value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets Code value.
        /// </summary>
        /// <value>Always unique.</value>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets Percentage value.
        /// </summary>
        /// <value>Always unique.</value>
        public decimal Percentage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the code is avaliable or not.
        /// </summary>
        /// <value>Always unique.</value>
        public bool Available { get; set; }

        /// <summary>
        /// Gets or sets OrdersListId value.
        /// </summary>
        /// <value>Always unique.</value>
        public int OrdersListId { get; set; }

        /// <summary>
        /// Gets or sets OrdersList value.
        /// </summary>
        /// <value>Always unique.</value>
        public OrdersList OrdersList { get; set; }
    }
}
