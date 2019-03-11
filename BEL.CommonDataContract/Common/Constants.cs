namespace BEL.CommonDataContract
{
    /// <summary>
    /// The Constants
    /// </summary>
    public static class Constants
    {   
        /// <summary>
        /// The DCRNO
        /// </summary>
        public const string DCRNO = "DCRNo";

        /// <summary>
        /// The CP
        /// </summary>
        public const string CP = "CP";

        /// <summary>
        /// The LUM
        /// </summary>
        public const string LUM = "LUM";

        /// <summary>
        /// The date time string
        /// </summary>
        public const string DateTimeString = "DateTime";

        /// <summary>
        /// The time zone name
        /// </summary>
        public const string TimeZoneName = "India Standard Time";
    }

    /// <summary>
    /// The Task Common Status
    /// </summary>
    public static class TaskStatus
    {
        /// <summary>
        /// The notstarted
        /// </summary>
        public const string NOTSTARTED = "Not Started";

        /// <summary>
        /// The notassigned
        /// </summary>
        public const string NOTASSIGNED = "Not Assigned";

        /// <summary>
        /// The inprogress
        /// </summary>
        public const string INPROGRESS = "In Progress";

        /// <summary>
        /// The completed
        /// </summary>
        public const string COMPLETED = "Completed";

        /// <summary>
        /// The deleted
        /// </summary>
        public const string DELETED = "Deleted";

        /// <summary>
        /// The removed
        /// </summary>
        public const string REMOVED = "Removed";
    }

    /// <summary>
    /// Item Status
    /// </summary>
    public static class ItemStatus
    {
        /// <summary>
        /// The notstarted
        /// </summary>
        public const string NOTSTARTED = "Not Started";

        /// <summary>
        /// The notassigned
        /// </summary>
        public const string NOTASSIGNED = "Not Assigned";

        /// <summary>
        /// The inprogress
        /// </summary>
        public const string INPROGRESS = "In Progress";

        /// <summary>
        /// The completed
        /// </summary>
        public const string COMPLETED = "Completed";

        /// <summary>
        /// The deleted
        /// </summary>
        public const string DELETED = "Deleted";
    }

    /// <summary>
    /// Application Constants
    /// </summary>
    public static class ApplicationConstants
    {
        /// <summary>
        /// The exceptionpolicyclass
        /// </summary>
        public const string EXCEPTIONPOLICYCLASS = "BusinessExceptionPolicy";

        /// <summary>
        /// The successmessage
        /// </summary>
        public const string SUCCESSMESSAGE = "Text_Success";

        /// <summary>
        /// The errormessage
        /// </summary>
        public const string ERRORMESSAGE = "Text_Error";

        /// <summary>
        /// The conflict error message
        /// </summary>
        public const string CONFLICTERRORMESSAGE = "Text_Conflict";

        /// <summary>
        /// The formnotfondmsg
        /// </summary>
        public const string FORMNOTFONDMSG = "Text_FormNotFound";
    }

    /// <summary>
    /// Application Names
    /// </summary>
    public static class ApplicationNameConstants
    {
        /// <summary>
        /// The rmapp
        /// </summary>
        public const string DCRAPP = "DCRDCN";
    }

    /// <summary>
    /// Form Names
    /// </summary>
    public static class FormNameConstants
    {
        /// <summary>
        /// The dcrform
        /// </summary>
        public const string DCRFORM = "DCR Form";

        /// <summary>
        /// The dcrform
        /// </summary>
        public const string DCNFORM = "DCN Form";       
    }

    /// <summary>
    /// List Scopes
    /// </summary>
    public static class SectionNameConstant
    {
        /// <summary>
        /// The local
        /// </summary>
        public const string ACTIVITYLOG = "Activity Log";

        /// <summary>
        /// The global
        /// </summary>
        public const string APPLICATIONSTATUS = "Application Status";

        /// <summary>
        /// The global
        /// </summary>
        public const string ISATTACHMENT = "ISATTACHMENT";
    }

    /// <summary>
    /// List Scopes
    /// </summary>
    public static class ListScope
    {
        /// <summary>
        /// The local
        /// </summary>
        public const string LOCAL = "Local";

        /// <summary>
        /// The global
        /// </summary>
        public const string GLOBAL = "Global";
    }

    /// <summary>
    /// User Roles
    /// </summary>
    public static class UserRoles
    {
        /// <summary>
        /// The administrator
        /// </summary>
        public const string ADMINISTRATOR = "Admin";

        /// <summary>
        /// The creator
        /// </summary>
        public const string CREATOR = "Creator";

        /// <summary>
        /// The viewer
        /// </summary>
        public const string VIEWER = "Viewer";

        /// <summary>
        /// The viewer
        /// </summary>
        public const string CPVIEWER = "CPViewer";

        /// <summary>
        /// The viewer
        /// </summary>
        public const string LUMVIEWER = "LUMViewer";

        /// <summary>
        /// The admin
        /// </summary>
        public const string ADMIN = "Admin";

        /// <summary>
        /// The contributor
        /// </summary>
        public const string CONTRIBUTOR = "Contributor";

        /// <summary>
        /// The division administrator
        /// </summary>
        public const string DIVISIONADMINISTRATOR = "Division Administrator";

        /// <summary>
        /// The publisher
        /// </summary>
        public const string PUBLISHER = "Publisher";

        /// <summary>
        /// All Users
        /// </summary>
        public const string ALL = "All";


    }

    /// <summary>
    /// User Roles
    /// </summary>
    public static class Parameter
    {
        /// <summary>
        /// The approvermatrixlistname
        /// </summary>
        public const string APPROVERMATRIXLISTNAME = "ApproverMatrixListName";

        /// <summary>
        /// The is asynchronous
        /// </summary>
        public const string ISASYNC = "IsAsync";

        /// <summary>
        /// The creator
        /// </summary>
        public const string ACTIVITYLOG = "ActivityLog";

        /// <summary>
        /// The viewer
        /// </summary>
        public const string USEREMAIL = "UserEmail";

        /// <summary>
        /// The viewer
        /// </summary>
        public const string USEREID = "UserID";

        /// <summary>
        /// The admin
        /// </summary>
        public const string ACTIONPER = "ActionPer";

        /// <summary>
        /// The contributor
        /// </summary>
        public const string CONTRIBUTOR = "Contributor";

        /// <summary>
        /// The section
        /// </summary>
        public const string SECTION = "Section";

        /// <summary>
        /// The sendbacklevel
        /// </summary>
        public const string SENDTOLEVEL = "SendToLevel";

        /// <summary>
        /// The applicationname
        /// </summary>
        public const string APPLICATIONNAME = "ApplicationName";

        /// <summary>
        /// The fromname
        /// </summary>
        public const string FROMNAME = "FormName";

        /// <summary>
        /// The isnewitem
        /// </summary>
        public const string ISNEWITEM = "IsNewItem";

        /// <summary>
        /// The current approver name
        /// </summary>
        public const string CURRENTAPPROVERNAME = "CurrentApproverName";

        /// <summary>
        /// The next approver name
        /// </summary>
        public const string NEXTAPPROVERNAME = "NextApproverName";

        /// <summary>
        /// The currentfromlevel
        /// </summary>
        public const string CURRENTFROMLEVEL = "CurrentFromLevel";

        /// <summary>
        /// The approver matrix user
        /// </summary>
        public const string APPROVERMATRIXUSER = "ApproverMatrixUser";

        /// <summary>
        /// The set permission
        /// </summary>
        public const string SETPERMISSION = "SetPermission";

        /// <summary>
        /// The meeting main list
        /// </summary>
        public const string MEETINGMAINLIST = "MainList";

        /// <summary>
        /// The copy meeting list
        /// </summary>
        public const string COPYMEETINGLIST = "CopyList";

        /// <summary>
        /// The form type
        /// </summary>
        public const string FORMTYPE = "FormType";

        /// <summary>
        /// The itemid
        /// </summary>
        public const string ITEMID = "ItemId";

        /// <summary>
        /// The email template
        /// </summary>
        public const string EMAILTEMPLATE = "EmailTemplate";

        /// <summary>
        /// The send task notification
        /// </summary>
        public const string SENDTASKNOTIFICATION = "SendTaskNotification";

        /// <summary>
        /// The task list name
        /// </summary>
        public const string TASKLISTNAME = "TaskListName";

        /// <summary>
        /// The sendtorole
        /// </summary>
        public const string SENDTOROLE = "SendToRole";

        /// <summary>
        /// The email field type
        /// </summary>
        public const string EMAILFIELDTYPE = "EmailFieldType";

        /// <summary>
        /// The location
        /// </summary>
        public const string LOCATION = "Location";

        /// <summary>
        /// The asset type
        /// </summary>
        public const string ASSETTYPE = "AssetType";

        /// <summary>
        /// The report name
        /// </summary>
        public const string REPORTNAME = "ReportName";
    }

    /// <summary>
    /// Sharepoint Permission
    /// </summary>
    public static class SharePointPermission
    {
        /// <summary>
        /// The reader
        /// </summary>
        public const string READER = "Read";

        /// <summary>
        /// The contributor
        /// </summary>
        public const string CONTRIBUTOR = "Contribute";
    }

    /// <summary>
    /// Form Type
    /// </summary>
    public static class FormType
    {
        /// <summary>
        /// The task
        /// </summary>
        public const string TASK = "Task";

        /// <summary>
        /// The main
        /// </summary>
        public const string MAIN = "Main";
    }

    /// <summary>
    /// The Form Status
    /// </summary>
    public static class FormStatus
    {
        /// <summary>
        /// The new
        /// </summary>
        public const string NEW = "New";

        /// <summary>
        /// The saveasdraft
        /// </summary>
        public const string SAVEASDRAFT = "Draft";

        /// <summary>
        /// The submitted
        /// </summary>
        public const string SUBMITTED = "Submitted";

        /// <summary>
        /// The completed
        /// </summary>
        public const string COMPLETED = "Completed";

        /// <summary>
        /// The cancelled
        /// </summary>
        public const string CANCELLED = "Cancelled";

        /// <summary>
        /// The deleted
        /// </summary>
        public const string DELETED = "Deleted";

        /// <summary>
        /// The sentback
        /// </summary>
        public const string SENTBACK = "Sent Back";

        /// <summary>
        /// The readytopublish
        /// </summary>
        public const string READYTOPUBLISH = "Ready to Publish";

        /// <summary>
        /// The update and republish
        /// </summary>
        public const string UPDATEANDREPUBLISH = "Update & Republish";

        /// <summary>
        /// The published
        /// </summary>
        public const string PUBLISHED = "Published";

        /// <summary>
        /// The scheduled
        /// </summary>
        public const string SCHEDULED = "Scheduled";

        /// <summary>
        /// The Re-scheduled
        /// </summary>
        public const string RESCHEDULED = "Re-Scheduled";

        /// <summary>
        /// The conducted
        /// </summary>
        public const string CONDUCTED = "Conducted";

        /// <summary>
        /// The notconducted
        /// </summary>
        public const string NOTCONDUCTED = "Not Conducted";

        /// <summary>
        /// The save
        /// </summary>
        public const string SAVE = "Save";

        /// <summary>
        /// The Schedule post date
        /// </summary>
        public const string SCHEDULEPOSTDATE = "SchedulePostDate";

        /// <summary>
        /// The publish post date
        /// </summary>
        public const string SENDTOHO = "SendToHO";

        /// <summary>
        /// Rejected Status
        /// </summary>
        public const string REJECTED = "Rejected";
    }

    /// <summary>
    /// The Form Approval Status
    /// </summary>
    public static class FormApprovalStatus
    {
        /// <summary>
        /// The notstarted
        /// </summary>
        public const string NOTSTARTED = "Not Started";

        /// <summary>
        /// The inprogress
        /// </summary>
        public const string INPROGRESS = "In Progress";

        /// <summary>
        /// The completed
        /// </summary>
        public const string COMPLETED = "Completed";

        /// <summary>
        /// The sentback
        /// </summary>
        public const string SENTBACK = "Sent Back";
    }

    /// <summary>
    /// The Approver Status
    /// </summary>
    public static class ApproverStatus
    {
        /// <summary>
        /// The notassigned
        /// </summary>
        public const string NOTASSIGNED = "Not Assigned";
               
        /// <summary>
        /// The pending
        /// </summary>
        public const string PENDING = "Pending";

        /// <summary>
        /// The approved
        /// </summary>
        public const string APPROVED = "Approved";

        /// <summary>
        /// The completed
        /// </summary>
        public const string COMPLETED = "Completed";

        /// <summary>
        /// The sendback
        /// </summary>
        public const string SENDBACK = "Send Back";

        /// <summary>
        /// The sendforward
        /// </summary>
        public const string SENDFORWARD = "Send Forward";

        /// <summary>
        /// The notassigned
        /// </summary>
        public const string NOTREQUIRED = "Not Required";
    }

    /// <summary>
    /// Email Template Name
    /// </summary>
    public static class EmailTemplateName
    {
        /// <summary>
        /// The design engg to creator
        /// </summary>
        public const string DESIGNENGGTOCREATOR = "DesignEnggToCreator";

        /// <summary>
        /// The forward task mail
        /// </summary>
        public const string FORWARDTASKMAIL = "Forward Task Mail";

        /// <summary>
        /// The approval mail
        /// </summary>
        public const string APPROVALMAIL = "ApprovalMail";

        /// <summary>
        /// The automatic approval mail
        /// </summary>
        public const string AUTOAPPROVALMAIL = "AutoApprovalMail";

        /// <summary>
        /// The sendback mail
        /// </summary>
        public const string SENDBACKMAIL = "SendBackMail";

        /// <summary>
        /// The request closer mail
        /// </summary>
        public const string REQUESTCLOSERMAIL = "RequestCloserMail";

        /// <summary>
        /// The task assigned mail
        /// </summary>
        public const string TASKASSIGNEDMAIL = "TaskAssignedMail";

        /// <summary>
        /// The taskreassignedmail
        /// </summary>
        public const string TASKREASSIGNEDMAIL = "Task ReAssigned Mail";

        /// <summary>
        /// The new request mail
        /// </summary>
        public const string NEWREQUESTMAIL = "NewRequestMail";

        /// <summary>
        /// The ready to publish
        /// </summary>
        public const string READYTOPUBLISH = "Ready To Publish";

        /// <summary>
        /// The meetingconducted
        /// </summary>
        public const string MEETINGCONDUCTED = "Meeting Conducted";

        /// <summary>
        /// The meetingnotconducted
        /// </summary>
        public const string MEETINGNOTCONDUCTED = "Meeting Not Conducted";

        /// <summary>
        /// The requestcanceled
        /// </summary>
        public const string REQUESTCANCELED = "RequestCancelled";

        /// <summary>
        /// The request reschedule
        /// </summary>
        public const string REQUESTRESCHEDULE = "Request Reschedule";

        /// <summary>
        /// The meetingpublished
        /// </summary>
        public const string MEETINGPUBLISHED = "Meeting Published";

        /// <summary>
        /// The task complete
        /// </summary>
        public const string TASKCOMPLETE = "Task Complete Mail";

        /// <summary>
        /// The task complete
        /// </summary>
        public const string SHAREWITH = "NewRequestMailShareWith";

        /// <summary>
        /// The task complete
        /// </summary>
        public const string ATTENDENCES = "NewRequestMailAttendees";

        /// <summary>
        /// The task complete
        /// </summary>
        public const string HOD = "NewRequestMailHOD";

        /// <summary>
        /// The esclation mail
        /// </summary>
        public const string ESCLATIONMAILTEMPLATE = "EscalationEmailTemplate";

        /// <summary>
        /// The reminder mail
        /// </summary>
        public const string REMINDEREMAILTEMPLATE = "ReminderEmailTemplate";

        /// <summary>
        /// The sendmailtosafetydept
        /// </summary>
        public const string SENDMAILTOSAFETYDEPT = "SendMailToSafetyDepartment";

        /// <summary>
        /// The premeetingpreperationemail
        /// </summary>
        public const string PREMEETINGPREPERATIONEMAIL = "Pre-Meeting Preperation Email";

        /// <summary>
        /// The forwardmail
        /// </summary>
        public const string FORWARDMAIL = "ForwardMail";

        /// <summary>
        /// The atrchallanmail
        /// </summary>
        public const string ATRCHALLANMAIL = "Challan Modification";

        /// <summary>
        /// The atrreceiptmail
        /// </summary>
        public const string ATRRECEIPTMAIL = "Receipt Modification";

        /// <summary>
        /// The productionplanning
        /// </summary>
        public const string PRODUCTIONPLANNING = "Production Planning";

        /// <summary>
        /// The taskremoved
        /// </summary>
        public const string TASKREMOVED = "TaskRemoved";

        /// <summary>
        /// The sendmailtosafetydept
        /// </summary>
        public const string SENDEMAILTOQASHAREWITH = "SendMailToQAShareWith";
    }

    /// <summary>
    /// Task Mode
    /// </summary>
    public static class TaskMode
    {
        /// <summary>
        /// The add
        /// </summary>
        public const string ADDEDIT = "AddEdit";

        /// <summary>
        /// The view
        /// </summary>
        public const string VIEW = "View";
    }

    /// <summary>
    /// The Button Caption
    /// </summary>
    public static class ButtonCaption
    {
        /// <summary>
        /// The send ooap
        /// </summary>
        public const string SendOOAP = "Send OAAP";

        /// <summary>
        /// The Level 1 Completed
        /// </summary>
        public const string Level1completed = "Level 1 Task Completed";

        /// <summary>
        /// The Level 2 Completed
        /// </summary>
        public const string Level2completed = "Level 2 Task Completed";

        /// <summary>
        /// The Meeting Conducted
        /// </summary>
        public const string MeetingConducted = "Meeting Conducted";

        /// <summary>
        /// The save as draft
        /// </summary>
        public const string SaveAsDraft = "Save As Draft";
    }

    /// <summary>
    /// Site URLS
    /// </summary>
    public class SiteURLs
    {
        /// <summary>
        /// The rootsiteurl
        /// </summary>
        public const string ROOTURL = "RootURL";

        /// <summary>
        /// The rootsiteurl
        /// </summary>
        public const string ROOTSITEURL = "RootSiteURL";

        /// <summary>
        /// The ndpobstaclesite
        /// </summary>
        public const string DCRDCNSITEURL = "DCRDCNSiteURL";

    }

    /// <summary>
    /// AKI Report Type
    /// </summary>
    public class AKIReportType
    {
        /// <summary>
        /// The irmdrmscorecard
        /// </summary>
        public const string IRMDRMSCORECARD = "IRM DRM Score Card";

        /// <summary>
        /// The departmentscorecard
        /// </summary>
        public const string DEPARTMENTSCORECARD = "Department Score Card";

        /// <summary>
        /// The departmentscorecard main
        /// </summary>
        public const string DEPARTMENTSCORECARDMAIN = "Department Score Card Main";

        /// <summary>
        /// The departmentscorecarddetail
        /// </summary>
        public const string DEPARTMENTSCORECARDDETAIL = "Department Score Card Detail";

        /// <summary>
        /// The individualcorecard
        /// </summary>
        public const string INDIVIDUALSCORECARD = "Individual Score Card";
    }

    /// <summary>
    /// Task Action Status
    /// </summary>
    public enum TaskActionStatus
    {
        /// <summary>
        /// The new
        /// </summary>
        NEW = 0,

        /// <summary>
        /// The updated
        /// </summary>
        UPDATED = 1,

        /// <summary>
        /// The deleted
        /// </summary>
        DELETED = 2,

        /// <summary>
        /// The nochange
        /// </summary>
        NOCHANGE = 3
    }

    /// <summary>
    /// Task Action Status
    /// </summary>
    public enum ItemActionStatus
    {
        /// <summary>
        /// The new
        /// </summary>
        NEW = 0,

        /// <summary>
        /// The updated
        /// </summary>
        UPDATED = 1,

        /// <summary>
        /// The deleted
        /// </summary>
        DELETED = 2,

        /// <summary>
        /// The nochange
        /// </summary>
        NOCHANGE = 3
    }

    /// <summary>
    /// Export To Excel Application
    /// </summary>
    public class ExportToExcelApplication
    {
        /// <summary>
        /// The irmdrmscorecard
        /// </summary>
        public const string MVR = "MVR";

        /// <summary>
        /// The departmentscorecard
        /// </summary>
        public const string IRM = "IRM";

        /// <summary>
        /// The departmentscorecard main
        /// </summary>
        public const string DRM = "DRM";
    }
}