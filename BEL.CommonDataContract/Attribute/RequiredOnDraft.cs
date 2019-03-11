namespace BEL.CommonDataContract
{
    using System;

    /// <summary>
    /// Required On Draft
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class RequiredOnDraft : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredOnDraft"/> class.
        /// </summary>
        public RequiredOnDraft()
        {
        }
    }

    /// <summary>
    /// Required On Draft
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class RequiredOnSendBack : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredOnSendBack"/> class.
        /// </summary>
        public RequiredOnSendBack()
        {
        }
    }

    /// <summary>
    /// Required On Delegate
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class RequiredOnDelegate : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredOnDelegate"/> class.
        /// </summary>
        public RequiredOnDelegate()
        {
        }
    }
}
