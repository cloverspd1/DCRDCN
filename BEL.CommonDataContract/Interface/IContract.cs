namespace BEL.CommonDataContract
{
    using BEL.CommonDataContract;
    using System;
    
    /// <summary>
    /// IContract Interface
    /// </summary>
    public interface IContract : IDisposable
    {
        /// <summary>
        /// Gets or sets the user details.
        /// </summary>
        /// <value>
        /// The user details.
        /// </value>
        UserDetails UserDetails { get; set; }
    }
}
