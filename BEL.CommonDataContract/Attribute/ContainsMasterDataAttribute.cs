namespace BEL.CommonDataContract
{
    using System;
    
    /// <summary>
    /// Contains Master Data Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class ContainsMasterDataAttribute : Attribute
    {
        /// <summary>
        /// The contains master data
        /// </summary>
        private bool containsMasterData = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainsMasterDataAttribute"/> class.
        /// </summary>
        /// <param name="containsMasterData">if set to <c>true</c> [contains master data].</param>
        public ContainsMasterDataAttribute(bool containsMasterData)
        {
            this.containsMasterData = containsMasterData;
        }

        /// <summary>
        /// Gets a value indicating whether contains master data.
        /// </summary>
        /// <value>
        /// <c>true</c> if [contains master data]; otherwise, <c>false</c>.
        /// </value>
        public bool ContainsMasterData
        {
            get
            {
                return this.containsMasterData;
            }
        }
    }
}
