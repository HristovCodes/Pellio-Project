namespace Pellio.ViewModels
{
    using System.Collections.Generic;
    using Pellio.Models;
    
    /// <summary>
    /// ViewModel allowing orderlist and madeorder to display on a single view.
    /// </summary>
    public class OrderListMadeOrder
    {
        /// <summary>
        /// Gets or sets OrdersList dbset.
        /// </summary>
        /// <value>Always unique.</value>
        public OrdersList OrdersList { get; set; }

        /// <summary>
        /// Gets or sets MadeOrder dbset.
        /// </summary>
        /// <value>Always unique.</value>
        public List<MadeOrder> MadeOrder { get; set; }
    }
}
