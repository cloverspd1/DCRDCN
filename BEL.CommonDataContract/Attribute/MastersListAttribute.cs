namespace BEL.CommonDataContract
{
    using System;

    /// <summary>
    /// Masters List Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class MastersListAttribute : Attribute
    {
        /// <summary>
        /// The master names
        /// </summary>
        private string masterNames = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="MastersListAttribute" /> class.
        /// </summary>
        /// <param name="masterName">Name of the master.</param>
        public MastersListAttribute(string masterName)
        {
            this.masterNames = masterName;
        }

        /// <summary>
        /// Gets the master names.
        /// </summary>
        /// <value>
        /// The master names.
        /// </value>
        public string MasterNames
        {
            get
            {
                return this.masterNames;
            }
        }
    }
}
