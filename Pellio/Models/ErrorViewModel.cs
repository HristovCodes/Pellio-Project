namespace Pellio.Models
{
    /// <summary>
    /// View for when we get an error.
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// Gets or sets RequestId value.
        /// </summary>
        /// <value>Always unique.</value>
        public string RequestId { get; set; }

        /// <summary>
        /// No idea what this does.
        /// </summary>
        /// <value>Always unique.</value>
        public bool ShowRequestId => !string.IsNullOrEmpty(this.RequestId);
    }
}
