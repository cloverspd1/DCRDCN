namespace BEL.CommonDataContract
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Button Status
    /// </summary>
    public enum ButtonActionStatus
    {
        /// <summary>
        /// The none
        /// </summary>
        None = 0,

        /// <summary>
        /// The saveas draft
        /// </summary>
        SaveAsDraft = 1,

        /// <summary>
        /// The save
        /// </summary>
        Save = 2,

        /// <summary>
        /// The ready to publish
        /// </summary>
        ReadyToPublish = 3,

        /// <summary>
        /// The send mail notification
        /// </summary>
        SendMailNotification = 4,

        /// <summary>
        /// The exit
        /// </summary>
        Exit = 5,

        /// <summary>
        /// The print
        /// </summary>
        Print = 6,

        /// <summary>
        /// The reschedule
        /// </summary>
        Reschedule = 7,

        /// <summary>
        /// The cancel
        /// </summary>
        Cancel = 8,

        /// <summary>
        /// The replace
        /// </summary>
        Replace = 9,

        /// <summary>
        /// The next approval
        /// </summary>
        NextApproval = 10,

        /// <summary>
        /// The back to creator
        /// </summary>
        BackToCreator = 11,

        /// <summary>
        /// The guidelines
        /// </summary>
        Guidelines = 12,

        /// <summary>
        /// The re assign
        /// </summary>
        ReAssign = 13,

        /// <summary>
        /// The complete
        /// </summary>
        Complete = 14,

        /// <summary>
        /// The forward
        /// </summary>
        Forward = 15,

        /// <summary>
        /// The integrate
        /// </summary>
        Integrate = 16,

        /// <summary>
        /// The saveas draft and set permission
        /// </summary>
        SaveAsDraftAndSetPermission = 17,

        /// <summary>
        /// The save and Set Permission
        /// </summary>
        SaveAndSetPermission = 18,

        /// <summary>
        /// The next approval
        /// </summary>
        NextApprovalAndSetPermission = 19,

        /// <summary>
        /// The send oaap
        /// </summary>
        SendOAAP = 20,

        /// <summary>
        /// The meeting conducted
        /// </summary>
        MeetingConducted = 21,

        /// <summary>
        /// The send back
        /// </summary>
        SendBack = 22,

        /// <summary>
        /// The meeting not conducted
        /// </summary>
        MeetingNotConducted = 23,

        /// <summary>
        /// The copy schedule
        /// </summary>
        CopySchedule = 24,

        /// <summary>
        /// The send forward
        /// </summary>
        SendForward = 25,

        /// <summary>
        /// The submit
        /// </summary>
        Submit = 26,

        /// <summary>
        /// The counducted
        /// </summary>
        Counducted = 27,

        /// <summary>
        /// The update and republish
        /// </summary>
        UpdateAndRepublish = 28,

        /// <summary>
        /// The generate LSMW
        /// </summary>
        GenerateLSMW = 29,

        /// <summary>
        /// The update and reschedule
        /// </summary>
        UpdateAndReschedule = 30,

        /// <summary>
        /// The confirm submit
        /// </summary>
        ConfirmSave = 31,

        /// <summary>
        /// The save and status update
        /// </summary>
        SaveAndStatusUpdate = 32,

        /// <summary>
        /// The save and no status update
        /// </summary>
        SaveAndNoStatusUpdate = 33,

        /// <summary>
        /// The save and status update with email
        /// </summary>
        SaveAndStatusUpdateWithEmail = 34,

        /// <summary>
        /// The save and no status update with email
        /// </summary>
        SaveAndNoStatusUpdateWithEmail = 35,

        /// <summary>
        /// The send for sap
        /// </summary>
        SendForSAP = 36,

        /// <summary>
        /// The integrate for three month
        /// </summary>
        IntegrateForThreeMonth = 37,

        /// <summary>
        /// The revise date
        /// </summary>
        ReviseDate = 38,

        /// <summary>
        /// The removed task
        /// </summary>
        RemovedTask = 39,

        /// <summary>
        /// Rejected 
        /// </summary>
        Rejected = 40,

        /// <summary>
        /// Delegate
        /// </summary>
        Delegate =41
    }
}
