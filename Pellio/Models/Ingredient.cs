namespace Pellio.Models
{
    /// <summary>
    /// Contains all ingredient so we know what we have avaliable.
    /// </summary>
    public class Ingredient
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
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the ingredient is avaliable or not.
        /// </summary>
        /// <value>Always unique.</value>
        public bool Available { get; set; }
    }
}
