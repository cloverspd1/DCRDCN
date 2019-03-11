namespace BEL.CommonDataContract
{
    /// <summary>
    /// IMaster item inteface
    /// </summary>
    public interface IMasterItem
    {
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        string Title { get; set; }
        
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        string Value { get; set; }
    }
}
