namespace BEL.CommonDataContract
{
    /// <summary>
    /// Ajax Response Status Codes Class
    /// </summary>
    public static class AjaxResponseStatusCodes
    {
        /// <summary>
        /// The ok
        /// </summary>
        public const string Ok = "OK";

        /// <summary>
        /// The error
        /// </summary>
        public const string Error = "ERROR";

        /// <summary>
        /// The true
        /// </summary>
        public const string True = "TRUE";

        /// <summary>
        /// The false
        /// </summary>
        public const string False = "FALSE";

        /// <summary>
        /// The more information required
        /// </summary>
        public const string MoreInfoRequired = "MORE_INFO_REQUIRED";

        /// <summary>
        /// The already exist
        /// </summary>
        public const string AlreadyExist = "ALREADY_EXIST";

        /// <summary>
        /// The validation error
        /// </summary>
        public const string ValidationError = "VALIDATION_ERROR";

        /// <summary>
        /// The delete restrict
        /// </summary>
        public const string DeleteRestrict = "DELETE_RESTRICT";

        /// <summary>
        /// The resubmission
        /// </summary>
        public const string Resubmission = "RESUBMISSION";

        /// <summary>
        /// The email send error
        /// </summary>
        public const string EmailSendError = "EMAIL_SEND_ERROR";

        /// <summary>
        /// The bill period validation
        /// </summary>
        public const string BillPeriodValidation = "BILL_PERIOD_VALIDATION";
    }

    /// <summary>
    /// Ajax Response class
    /// </summary>
    public class AjaxResponse
    {
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public object Data { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the redirect to.
        /// </summary>
        /// <value>
        /// The redirect to.
        /// </value>
        public string RedirectTo { get; set; }

        /// <summary>
        /// Gets or sets the error information.
        /// </summary>
        /// <value>
        /// The error information.
        /// </value>
        public AjaxErrorInfo ErrorInfo { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }
    }

    /// <summary>
    /// Ajax Error Info class
    /// </summary>
    public class AjaxErrorInfo
    {
        /// <summary>
        /// Gets or sets the friendly error message.
        /// </summary>
        /// <value>
        /// The friendly error message.
        /// </value>
        public string FriendlyErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        /// <value>
        /// The error code.
        /// </value>
        public string ErrorCode { get; set; }
    }
}
