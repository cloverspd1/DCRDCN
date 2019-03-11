namespace BEL.CommonDataContract
{
    using System;

    /// <summary>
    /// Is Person Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class IsPersonAttribute : Attribute
    {
        /// <summary>
        /// The is person
        /// </summary>
        private bool isPerson = true;

        /// <summary>
        /// The is multiple
        /// </summary>
        private bool isMultiple = false;


        /// The is multiple
        /// </summary>
        private bool returnName = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="IsPersonAttribute" /> class.
        /// </summary>
        /// <param name="isPerson">if set to <c>true</c> [is person].</param>
        /// <param name="isMultiple">if set to <c>true</c> [is multiple].</param>
        public IsPersonAttribute(bool isPerson, bool isMultiple, bool returnName = false)
        {
            this.isPerson = isPerson;
            this.isMultiple = isMultiple;
            this.returnName = returnName;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is person.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is person; otherwise, <c>false</c>.
        /// </value>
        public bool IsPerson
        {
            get
            {
                return this.isPerson;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is multiple.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is multiple; otherwise, <c>false</c>.
        /// </value>
        public bool IsMultiple
        {
            get
            {
                return this.isMultiple;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is multiple.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is multiple; otherwise, <c>false</c>.
        /// </value>
        public bool ReturnName
        {
            get
            {
                return this.returnName;
            }
        }
    }
}
