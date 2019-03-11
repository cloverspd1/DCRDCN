namespace BEL.CommonDataContract
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Button Class
    /// </summary>
    [DataContract, Serializable]
    public class Button
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the javascript function.
        /// </summary>
        /// <value>
        /// The javascript function.
        /// </value>
        [DataMember]
        public string JsFunction { get; set; }

        /// <summary>
        /// Gets or sets the button status.
        /// </summary>
        /// <value>
        /// The button status.
        /// </value>
        [DataMember]
        public ButtonActionStatus ButtonStatus { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is visible.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is visible; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool IsVisible { get; set; }

        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        /// <value>
        /// The icon.
        /// </value>
        [DataMember]
        public string Icon { get; set; }

        /// <summary>
        /// Gets or sets the send back to.
        /// </summary>
        /// <value>
        /// The send back to.
        /// </value>
        [DataMember]
        public string SendBackTo { get; set; }

        /// <summary>
        /// Gets or sets the send to role.
        /// </summary>
        /// <value>
        /// The send to role.
        /// </value>
        [DataMember]
        public string SendToRole { get; set; }

        /// <summary>
        /// Gets or sets the Levels.
        /// </summary>
        /// <value>
        /// The Levels.
        /// </value>
        [DataMember]
        public string Levels { get; set; }

        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        /// <value>
        /// The role.
        /// </value>
        [DataMember]
        public string Role { get; set; }

        /// <summary>
        /// Gets or sets the name of the application.
        /// </summary>
        /// <value>
        /// The name of the application.
        /// </value>
        [DataMember]
        public string ApplicationName { get; set; }

        /// <summary>
        /// Gets or sets the name of the form.
        /// </summary>
        /// <value>
        /// The name of the form.
        /// </value>
        [DataMember]
        public string FormName { get; set; }

        /// <summary>
        /// Gets or sets the form status.
        /// </summary>
        /// <value>
        /// The form status.
        /// </value>
        [DataMember]
        public string FormStatus { get; set; }

        /// <summary>
        /// Gets or sets the type of the form.
        /// </summary>
        /// <value>
        /// The type of the form.
        /// </value>
        [DataMember]
        public string FormType { get; set; }

        /// <summary>
        /// Gets or sets the type of the form.
        /// </summary>
        /// <value>
        /// The type of the form.
        /// </value>
        [DataMember]
        public int Sequence { get; set; }

        /// <summary>
        /// Gets or sets the ToolTip of button.
        /// </summary>
        /// <value>
        /// The ToolTip button.
        /// </value>
        [DataMember]
        public string ToolTip { get; set; }
    }
}
