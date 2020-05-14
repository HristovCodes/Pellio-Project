namespace Pellio.ViewModels
{
    using Pellio.Models;
    
    /// <summary>
    /// ViewModel for displaying comments and products on a single view.
    /// </summary>
    public class ProductComment
    {
        /// <summary>
        /// Gets or sets Products value.
        /// </summary>
        /// <value>Always unique.</value>
        public Products Products { get; set; }

        /// <summary>
        /// Gets or sets Comments value.
        /// </summary>
        /// <value>Always unique.</value>
        public Comments Comments { get; set; }
    }
}
