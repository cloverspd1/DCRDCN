var securityToken;
$(window).load(function () {
    if (ENVIRONMENTLIVE == 'true') {
        /* try {
             if (parent.location.href == top.location.href && window.location.href != BASEPATHURL + '/Master/NotAuthorize') {
                 window.location.href = BASEPATHURL + '/Master/NotAuthorize';
             }
         }
         catch (e1) { } */
    }
});
function BindValidation() {
    $(".numberonly").off("keypress").on("keypress", function (evt) {
        return isNumber(evt, this);
    });
    $(".integeronly").off("keypress").on("keypress", function (evt) {
        return isInteger(evt, this);
    });

    $(".fixedtwodecimal").off("change").on("change", function (evt) {
        if ($(this).val().indexOf('.') != -1) {
            $(this).val(parseFloat($(this).val()).toFixed(2));
        }
    });
    $(".fixedthreedecimal").off("change").on("change", function (evt) {
        if ($(this).val().indexOf('.') != -1) {
            $(this).val(parseFloat($(this).val()).toFixed(3));
        }
    });
    $(".fixedtwodecimal").off("keypress").on("keypress", function (evt) {
        return isNumber(evt, this);
    });
    $("textarea").off("keyup change load").on("keyup change load", function (evt) {
        return textAreaAdjust(this);
    });
    $("textarea").trigger('keyup change load');
    $("textarea").trigger('load');
}
$(document).ready(function (e) {

    BindDatePicker("");
    BindValidation();
    $(".grid-view").each(function (index, element) {
        var $chk = $(this).find($(".grpChkBox input:checkbox"));
        var $tbl = $(this).find($(".someTable"));
        var $tblhead = $(this).find($(".someTable th"));

        $chk.prop('checked', true);

        $chk.click(function () {
            var colToHide = $tblhead.filter("." + $(this).attr("name"));
            var index = $(colToHide).index();
            $tbl.find('tr :nth-child(' + (index + 1) + ')').toggle();
        });
    });
    setTimeout(function () {
        DisableFormItems();
    }, 1000);
    $("form[data-ajax='true']").each(function () {
        $(this).attr("data-ajax-old-success", $(this).attr("data-ajax-success"));
    });
    $("span.text-danger.text-default-light").each(function () {
        var nextLabel = $(this).next();
        if (nextLabel.is("label")) {
            var html = nextLabel.html();
            nextLabel.html("");
            $(this).appendTo(nextLabel);
            nextLabel.append(html);
        }
    });
    $(".amounttoword").on("change keyup", function () {
        var targetselector = $(this).attr("data-target");
        if (typeof (targetselector) !== "undefined" && targetselector != null) {
            $(targetselector).val(AmountToWords($(this).val(), $(targetselector).hasClass("lacs")));
        }
    });
    $(".amounttoword").change();
    $("form").attr("autocomplete", "off");
    $(".sel-first-radio").each(function () {
        if ($(this).find("input[type='radio']:checked").length == 0) {
            $(this).find("input[type='radio']:first").prop("checked", true);
        }
    });
    $(".sel-first-checkbox").each(function () {
        if ($(this).find("input[type='checkbox']:checked").length == 0) {
            $(this).find("input[type='checkbox']:first").prop("checked", true);
        }
    });


    ////Comment for - Change Request 185528650 : DCRDCN: CR:  DCR DCN UAT2 MOM Points
    //setTimeout(function () {
    //    var firstElement = $("input,select,textarea").not(":hidden").not(":disabled").not(".notvisible").first();

    //    if (firstElement.length > 0) {
    //        //console.log(firstElement.attr("id"));
    //        $("html,body").animate({ "scrollTop": firstElement.parents(".card:first").offset().top - 70 });
    //        firstElement.focus();
    //    }
    //}, 1100);
    var spHost = $("<input id='hdnSPHOST' type='hidden'/>");
    spHost.val(SPHOST);
    spHost.appendTo("body");
    var spHostUrl = $("<input id='hdnSPHOSTURL' type='hidden'/>");
    spHostUrl.val(SPHOSTURL);
    spHostUrl.appendTo("body");
    // $("input[name='PublishFrom']").on("change", function () {
    //setTimeout(function () {
    //    var tmsReqId = $("#subSection #TMSRequestID");
    //    if (tmsReqId.length > 0 && $.trim(tmsReqId.val()) != '' && $.trim(tmsReqId.val()) != '0') {
    //        AjaxCall({
    //            url: "/Master/GetTMSInfo?id=" + tmsReqId.val(),
    //            httpmethod: "GET",
    //            sucesscallbackfunction: function (result) {
    //                OnTMSSelected(result);
    //            }
    //        });
    //    }
    //}, 500);
    // });
    setTimeout(function () {
        $("i.fa[data-original-title]").each(function () {
            $(this).tooltip('hide').attr('data-original-title', $(this).val()).tooltip('fixTitle');
        });
        $("select").each(function () {
            var text = $(this).find("option:selected").text();
            $(this).tooltip('hide').attr('data-original-title', text).tooltip('fixTitle');
        });
    }, 500);
    $(document).on("keyup change", "select", function () {
        var $val = $(this).val();
        if ($(this).is("select")) {
            $val = $(this).find("option:selected").text();
        }
        if ($.trim($val) == '') {
            try {
                $(this).tooltip('destroy');
            } catch (e) { }
        }
        if (typeof ($(this).attr('data-original-title')) !== "undefined") {
            $(this).tooltip('hide').attr('data-original-title', $val).tooltip('fixTitle');
        } else {
            $(this).tooltip({ 'trigger': 'hover', 'title': $val });
        }
    });
    $(document).on("hover", "i.fa[data-toggle='tooltip']", function () {
        var $val = $(this).attr("title");
        $(this).tooltip({ 'trigger': 'hover', 'title': $val });
    });
    $(document).on("hover", "textarea[data-toggle='tooltip'], input[type='text'][data-toggle='tooltip']", function () {
        var $val = $(this).attr("tooltip");
        $(this).tooltip({ 'trigger': 'hover', 'title': $val });
    });
    $(document).on("hover", "li a", function () {
        var $val = $(this).attr("title");
        $(this).tooltip({ 'trigger': 'hover', 'title': $val });
    });
    $(document).ready(function () {
        $('i.fa[data-toggle="tooltip"]').tooltip({ 'trigger': 'hover', 'title': $(this).attr('title') });
    });
    HideWaitDialog();

    $(document).on('scroll', function () {
        if ($(window).scrollTop() > 100) {
            $('.scroll-top-wrapper').addClass('show');
        } else {
            $('.scroll-top-wrapper').removeClass('show');
        }
    });
    $('.scroll-top-wrapper').on('click', scrollToTop);

    console.log(BASEPATHURL + '/Master/KeepSessionAlive');
    SessionUpdater.Setup(BASEPATHURL + '/Master/KeepSessionAlive');
});


function scrollToTop() {
    verticalOffset = typeof (verticalOffset) != 'undefined' ? verticalOffset : 0;
    element = $('body');
    offset = element.offset();
    offsetTop = offset.top;
    $('html, body').animate({ scrollTop: offsetTop }, 500, 'linear');
}

function InputToUpper(obj) {
    if (obj.value != "") {
        obj.value = obj.value.toUpperCase();
    }
}

function DisableFormItems() {
    $(".disabled").find("form").addClass("disabled");
    $(".disabled").find("input,textarea").prop("disabled", true);
    $(".disabled").find("select").not('[multiple]').prop("disabled", true);
    $(".disabled .token-input-delete-token,.disabled .qq-upload-button,.disabled .qq-upload-status-text .fa-trash-o").remove();
    $(".disabled .btn").not(".collapse-btn").not(".multiselect").not(".view-task").addClass("hide");
    $("ul.token-input-list li.token-input-token:last").click();
    $(".disabled:not(:first)").find("div.card-body").addClass("collapse");
    $(".disabled:not(:first)").find("div.card-body").removeClass("in");
    //$(".disabled").find("div.card-body").addClass("collapse");
    //$(".disabled").find("div.card-body").removeClass("in");


}
function BindDatePicker(selector) {
    if ($.trim(selector) != "") {
        selector += selector + " ";
    }
    var todayDate = new Date();
    $(selector + '.datepicker').each(function () {
        var tempValue = $(this).find("input:first").val();
        $(this).datetimepicker({
            format: 'L', //for Date+++
            widgetParent: $(this).parent().is("td") ? "body" : null,
            //widgetPositioning: $(this).parent().is("td") ? { horizontal: "left", vertical: "bottom" } : { horizontal: "auto", vertical: "auto" },
            minDate: $(this).hasClass("pastDisabled") ? new Date(todayDate.getFullYear(), todayDate.getMonth(), todayDate.getDate(), 00, 00, 00) : undefined
        }).on("dp.change", function () {
            $(this).find("input").change();
        });
        $(this).find("input:first").val(tempValue);
    });
    $(selector + '.timepicker').each(function () {
        var tempValue = $(this).find("input:first").val();
        $(this).datetimepicker({
            format: 'LT' //for Date+++
            , widgetParent: $(this).parent().is("td") ? "body" : null
        }).on("dp.change", function () {
            $(this).find("input").change();
        });
        $(this).find("input:first").val(tempValue);
    });
}
function BindUserTags(selector) {
    if ($.trim(selector) != "") {
        selector += selector + " ";
    }
    $(selector + ".user-tags").each(function () {
        var ele = $(this);
        var valueList = ele.val().split(',');
        var url = BASEPATHURL + ele.attr("data-url");
        var hint = ele.attr("data-hint");
        var limit = ele.attr("data-limit");
        var onadd = ele.attr("data-onadd");
        var onremove = ele.attr("data-onremove");
        var displayColumn = ele.attr("data-displaycolumn");
        var takenValue = ele.attr("data-tokenvalue");
        if (typeof (displayColumn) === "undefined" || displayColumn == null || displayColumn == '') {
            displayColumn = 'id';
        }
        if (typeof (takenValue) === "undefined" || takenValue == null || takenValue == '') {
            takenValue = 'id';
        }
        if (typeof (limit) === "undefined") {
            limit = null;
        } else {
            limit = Number(limit);
        }
        if (typeof (hint) === "undefined") {
            hint = ResourceManager.Message.SearchHint;
        }
        ele.tokenInput(url, {
            hintText: hint,
            minChars: 2,
            tokenLimit: limit,
            preventDuplicates: true,
            animateDropdown: true,
            tokenFormatter: function (item) { return "<li><p>" + item[displayColumn] + "</p></li>" },
            tokenValue: takenValue,
            onAdd: function (item) {
                if (typeof (onadd) !== "undefined" && onadd != null && onadd != '') {
                    eval(onadd + "('#" + ele.attr("id") + "','" + item.id + "','" + encodeURIComponent(item.name) + "');");
                }
            },
            onDelete: function () {
                if (typeof (onremove) !== "undefined" && onremove != null && onremove != '') {
                    eval(onremove + "('#" + ele.attr("id") + "');");
                }
            }
        });
        $(valueList).each(function (i, item) {
            if ($.trim(item) != "") {
                var idElement = $(ele).attr("data-idelement");
                var id = item;
                if (typeof (idElement) !== "undefind" && idElement != null) {
                    id = $(idElement).val();
                }
                ele.tokenInput("add", { id: id, name: item });
            }
        });
    });
    $(".token-input-delete-token").text('x');
}
(function ($) {
    $.validator.unobtrusive.parsePopup = function (selector) {
        var $form = $(selector).find("form");
        $form.unbind();
        $form.data("validator", null);
        $.validator.unobtrusive.parse(document);
        $form.validate($form.data("unobtrusiveValidation").options);
    }
    $.validator.unobtrusive.parseDynamicContent = function (selector) {
        //use the normal unobstrusive.parse method
        $.validator.unobtrusive.parse(selector);

        //get the relevant form
        var form = $(selector).first().closest('form');
        if (form.length == 0) { return; }
        //get the collections of unobstrusive validators, and jquery validators
        //and compare the two
        var unobtrusiveValidation = form.data('unobtrusiveValidation');
        var validator = form.validate();

        $.each(unobtrusiveValidation.options.rules, function (elname, elrules) {
            if (validator.settings.rules[elname] == undefined) {
                var args = {};
                $.extend(args, elrules);
                args.messages = unobtrusiveValidation.options.messages[elname];
                //edit:use quoted strings for the name selector
                $("[name='" + elname + "']").rules("add", args);
            } else {
                $.each(elrules, function (rulename, data) {
                    if (validator.settings.rules[elname][rulename] == undefined) {
                        var args = {};
                        args[rulename] = data;
                        args.messages = unobtrusiveValidation.options.messages[elname][rulename];
                        //edit:use quoted strings for the name selector
                        $("[name='" + elname + "']").rules("add", args);
                    }
                });
            }
        });
    }
})($);

// Bind file upload control
function BindFileUploadControl(options) {
    /*
	{	ElementId: '<div> element id', 
		Url: <controller url 'controller/action'>, 
		Params: <{}>, param list pass to action, Eg. {Id=1, Data=""}
		AllowedExtensions: ['xls', 'xlsx', 'doc', 'docx', 'pdf'...], 
		CallBack: '<CallBack function name>', // optional 
		ButtonText: <upload button text.>, // optional - default 'Browse'
        DuplicateCheck: true
	*/

    var mutilple = false;
    if (typeof options.MultipleFiles !== "undefined" && options.MultipleFiles !== null) {
        mutilple = (options.MultipleFiles) ? options.MultipleFiles : false;
    }

    var maxfiles = 10;
    if (typeof options.maxFiles !== "undefined" && options.maxFiles !== null) {
        maxfiles = (options.maxFiles) ? options.maxFiles : 10;
    }

    var buttonText = "ATTACH";
    if (typeof options.ButtonText !== "undefined" && options.ButtonText !== null) {
        buttonText = options.ButtonText;
    }

    var hideFileSummary = false;
    if (typeof options.HideFileSummary !== "undefined" && options.HideFileSummary !== null) {
        hideFileSummary = options.HideFileSummary;
    }
    var DuplicateCheck = true;
    if (typeof options.DuplicateCheck !== "undefined" && options.DuplicateCheck !== null) {
        DuplicateCheck = options.DuplicateCheck;
    }
    var acrionURL = "";
    if (options.Url.toLowerCase() == 'uploadauditreportobservation')
        acrionURL = BASEPATHURL + "/AKI/" + options.Url;
    else if (options.Url.toLowerCase() == 'uploadbomunits')
        acrionURL = BASEPATHURL + "/PM/" + options.Url;
    else
        acrionURL = BASEPATHURL + "/Master/" + options.Url;
    var uploader = uploader = new qq.FileUploader(
    {
        element: document.getElementById(options.ElementId),
        params: options.Params,
        action: acrionURL,
        multiple: mutilple,
        debug: true,
        sizeLimit: 2 * 1024 * 1024,
        maxFiles: maxfiles, //10,
        uploadButtonText: buttonText,
        allowedExtensions: options.AllowedExtensions,
        duplicateCheck: DuplicateCheck,
        onProgress: function (id, fileName, loaded, total) {
            if (hideFileSummary) {
                jQuery(".qq-upload-list").hide()
            }
        },
        onComplete: function (id, fileName, responseJSON) {
            if (hideFileSummary) {
                jQuery(".qq-upload-list").hide()
            }
            if (responseJSON.IsSucceed || responseJSON.IsSucceed === undefined) {
                if (typeof (responseJSON.IndividualUpload) !== "undefined" && responseJSON.IndividualUpload) {
                    if (responseJSON.Status) {
                        if (typeof options.CallBack !== "undefined" && options.CallBack !== null) {
                            eval(options.CallBack + "(responseJSON)");
                        }
                    } else {
                        AlertModal(ResourceManager.Title.Error, responseJSON.Message);
                    }
                }
                else if (typeof (responseJSON.FileId) !== "undefined" && responseJSON.FileId != null && responseJSON.FileId != '') {
                    if (typeof options.CallBack !== "undefined" && options.CallBack !== null) {
                        eval(options.CallBack + "(responseJSON)");
                    }
                }
                else {
                    AlertModal(ResourceManager.Title.Error, responseJSON.Message);
                    if (typeof options.CallBack !== "undefined" && options.CallBack !== null) {
                        eval(options.CallBack + "(responseJSON)");
                    }
                }
            }
            else {
                AlertModal(ResourceManager.Title.Error, responseJSON.Message);
            }
        },
        onCancel: function (id, fileName) {
            if (hideFileSummary) {
                jQuery(".qq-upload-list").hide()
            }
        },

        maxConnections: 2,
        messages: {
            typeError: "{file} type of {extensions} not allowed.",
            sizeError: "You can not upload file of size more than {sizeLimit}.",
            minSizeError: "File size must be atleast {minSizeLimit}.",
            maxFilesError: "You cannot upload more than {maxFiles} files.",
            alreadyExistError: "{file} is already uploaded. You cannot upload file with same name.",
            emptyError: "file can not be empty.",
            onLeave: "",
            InvalidCharacter: "{file} filename is invalid, Please verify and try again.<ul> " +
                "<li>Filename should not contains (~ # % & * { } \ : < > ? / + | \" ; ..) character(s)</li>" +
                "<li>Filename should not start with period(.) character</li>" +
                "<li>Filename should not contain consecutive period(.) character</li>" +
                "<li>Filename should not end with period(.) character</li>" +
                "</ul>"
        },
        showMessage: function (message) {
            AlertModal(ResourceManager.Title.Error, message);
        }
    });
}
function CheckAndFillCommentAndDateField() {

}
var forwardButton = null;
function ForwardAndSubmit(ele) {
    forwardButton = ele;
    $("div[id='ForwardModal']").remove();
    var dlg = '<div class="modal fade" id="ForwardModal" tabindex="-1" role="dialog"><div class="modal-dialog" role="document"><div class="modal-content"><div class="modal-header"><button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button><h4 class="modal-title">Task Forward To</h4></div><div class="modal-body form-horizontal"><div class="row"><div class="col-sm-12"><div class="form-group"><label class="col-md-4 control-label">Forward To:</label><div class="col-md-8"><input id="PopupForwardToField" type="text" class="user-tags" data-url="/Master/GetUsers" data-limit="1" /></div></div></div></div></div><div class="modal-footer"><button type="button" onclick="TaskForward();" class="btn btn-primary">Forward</button><button type="button" class="btn btn-default" data-dismiss="modal">Close</button></div></div></div></div>';
    $(dlg).appendTo("body");
    $("#ForwardModal").modal().on('shown.bs.modal', function () {
        BindUserTags("#ForwardModal");
    });
}
function TaskForward() {
    var forwardId = $("#PopupForwardToField").val();
    if ($.trim(forwardId) == '') {
        AlertModal(ResourceManager.Title.Error, "Please enter 'forward to' person.");
    } else {
        $("#ForwardModal").modal('hide');
        $("input[id='ForwardTo']").val(forwardId);
        if ($("#Comment").val() == "") {
            $("#Comment").val("Task has been forwarded to " + forwardId);
        }
        if ($("#ActualStartDate").val() == "") {
            $("#ActualStartDate").val(formatDate(new Date()));
        }
        $("#Comment").val(forwardId);
        Submit(forwardButton);
    }
}

var reAssignButton = null;
function ReAssignAndSubmit(ele) {
    reAssignButton = ele;
    $("div[id='ReAssignModal']").remove();
    var dlg = '<div class="modal fade" id="ReAssignModal" tabindex="-1" role="dialog"><div class="modal-dialog" role="document"><div class="modal-content"><div class="modal-header"><button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button><h4 class="modal-title">Task ReAssign To</h4></div><div class="modal-body form-horizontal"><div class="row"><div class="col-sm-12"><div class="form-group"><label class="col-md-4 control-label">Assign To:</label><div class="col-md-8"><input id="PopupAssignToField" type="text" class="user-tags" data-url="/Master/GetUsers" data-limit="1" /></div></div></div></div><div class="row"><div class="col-sm-12"><div class="form-group"><label class="col-md-4 control-label">Comment:</label><div class="col-md-8"><textarea id="PopupCommentField" type="text" class="form-control"></textarea></div></div></div></div></div><div class="modal-footer"><button type="button" onclick="TaskReAssign();" class="btn btn-primary">ReAssign</button><button type="button" class="btn btn-default" data-dismiss="modal">Close</button></div></div></div></div>';
    $(dlg).appendTo("body");
    $("#PopupAssignToField").val($("input[id='ActionBy']").val());
    $("#ReAssignModal").modal().on('shown.bs.modal', function () {
        BindUserTags("#ReAssignModal");
    });
}
function TaskReAssign() {
    var reAssignId = $("#PopupAssignToField").val();
    var comment = $("#PopupCommentField").val();
    if ($.trim(reAssignId) == '') {
        AlertModal(ResourceManager.Title.Error, "Please enter 'ReAssign to' person.");
    }
    else if ($.trim(comment) == '' || comment.length < 15) {
        AlertModal(ResourceManager.Title.Error, "Please enter Comment(It should be minimum 15 characters of length).");
    }
    else {
        $("#ReAssignModal").modal('hide');
        $("input[id='ActionBy']").val(reAssignId);
        $("#Comment").val(comment);
        //if ($("#ActualStartDate").val() == "") {
        //    $("#ActualStartDate").val(formatDate(new Date()));
        //}
        Submit(reAssignButton);
    }
}

var multipleReAssignButton = null;
function MultipleReAssignAndSubmit(ele) {
    multipleReAssignButton = ele;
    $("div[id='MultipleReAssignModal']").remove();
    var dlg = '<div class="modal fade" id="MultipleReAssignModal" tabindex="-1" role="dialog"><div class="modal-dialog" role="document"><div class="modal-content"><div class="modal-header"><button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button><h4 class="modal-title">Task ReAssign To</h4></div><div class="modal-body form-horizontal"><div class="row"><div class="col-sm-12"><div class="form-group"><label class="col-md-4 control-label">Assign To:</label><div class="col-md-8"><input id="PopupMultipleAssignToField" type="text" class="user-tags" data-url="/Master/GetUsers" /></div></div></div></div><div class="row"><div class="col-sm-12"><div class="form-group"><label class="col-md-4 control-label">Comment:</label><div class="col-md-8"><textarea id="PopupMultiCommentField" class="form-control"></textarea></div></div></div></div></div><div class="modal-footer"><button type="button" onclick="TaskMultipleReAssign();" class="btn btn-primary">ReAssign</button><button type="button" class="btn btn-default" data-dismiss="modal">Close</button></div></div></div></div>';
    $(dlg).appendTo("body");
    $("#PopupMultipleAssignToField").val($("input[id='ActionBy']").val());
    $("#MultipleReAssignModal").modal().on('shown.bs.modal', function () {
        BindUserTags("#MultipleReAssignModal");
    });
}
function TaskMultipleReAssign() {
    var reAssignId = $("#PopupMultipleAssignToField").val();
    var comment = $("#PopupMultiCommentField").val();
    if ($.trim(reAssignId) == '') {
        AlertModal(ResourceManager.Title.Error, "Please enter 'ReAssign to' person.");
    }
    else if ($.trim(comment) == '' || comment.length < 15) {
        AlertModal(ResourceManager.Title.Error, "Please enter Comment(It should be minimum 15 characters of length).");
    }
    else {
        $("#MultipleReAssignModal").modal('hide');
        $("input[id='ActionBy']").val(reAssignId);
        $("#Comment").val(comment);
        //if ($("#ActualStartDate").val() == "") {
        //    $("#ActualStartDate").val(formatDate(new Date()));
        //}
        Submit(multipleReAssignButton);
    }
}

function AlertModal(title, msg, isExit, callback) {


    $("div[id='PopupDialog']").remove();
    var popupDlg = '<div class="modal fade bs-example-modal-sm" tabindex="-1" role="dialog" id="PopupDialog" aria-labelledby="mySmallModalLabel"><div class="modal-dialog modal-sm"><div class="modal-content"><div class="modal-header"><button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button><h4 class="modal-title" id="ModalTitle">Modal title</h4></div><div class="modal-body" id="ModalContent"></div><div class="modal-footer"><button type="button" id="ClosePopup" isdialogclose="false" class="btn btn-default" data-dismiss="modal">Close</button> </div></div></div></div>';
    $(popupDlg).appendTo("body");
    $("#PopupDialog #ModalTitle").text(title);
    $("#PopupDialog #ModalContent").html(msg);
    if (title == "Success") {
        $("#PopupDialog .modal-header").addClass("bg-success text-white");
    }
    else if (title == "Error") {
        $("#PopupDialog .modal-header").addClass("bg-danger text-white");
    }
    else if (title == "SessionTimeout") {
        $("#PopupDialog .modal-header").addClass("bg-warning text-white");
    }
    $("#PopupDialog").modal('show').on('hidden.bs.modal', function () {
        if (typeof (isExit) !== 'undefined' && isExit == true) {
            if (typeof (callback) !== 'undefined' && callback != null) {
                callback();
            }
            else {
                Exit();
            }
        }
    });
}


function ConfirmationDailog(options) {
    $("#ConfirmDialog").remove();
    var confirmDlg = "<div class='modal fade bs-example-modal-sm' tabindex='-1' role='dialog' id='ConfirmDialog' aria-labelledby='mySmallModalLabel'><div class='modal-dialog modal-sm'><div class='modal-content'><div class='modal-header'>" +
        "<button type='button' class='close' data-dismiss='modal' aria-label='Close'><span aria-hidden='true'>&times;</span></button><h4 class='modal-title' id='ModalTitle'>Modal title</h4></div><div class='modal-body' id='ModalContent'>" +
        "</div><div class='modal-footer'><button type='button' id='btnYesPopup' isdialogclose='false' class='btn btn-default' data-dismiss='modal'>" +
        "Yes</button><button type='button' id='btnNoPopup' isdialogclose='false' class='btn btn-default' data-dismiss='modal'>No</button> </div></div></div></div>";
    $(confirmDlg).appendTo("body");
    $("#ConfirmDialog #btnYesPopup").on("click", function () {
        if (typeof (options.okCallback) !== "undefined" && options.okCallback != null) {
            //options.okCallback();
            ConfirmPopupYes(options.url, options.id, options.okCallback);
        }
    });
    $("#ConfirmDialog #btnNoPopup").on("click", function () {
        if (typeof (options.cancelCallback) !== "undefined" && options.cancelCallback != null) {
            options.cancelCallback();
        }
    });
    $("#ConfirmDialog #ModalTitle").text(options.title);
    $("#ConfirmDialog #ModalContent").text(options.message);
    $("#ConfirmDialog").modal('show').on('hidden.bs.modal', function () {
        if (typeof (options.closeCallback) !== "undefined" && options.closeCallback != null) {
            options.closeCallback();
        }
    });
}

function ConfirmPopupYes(url, id, okCallback) {
    ShowWaitDialog();
    if (typeof (url) !== "undefined" && url != null) {
        url = BASEPATHURL + url;
        jQuery.post(url, {
            Id: id
        }, function (data) {
            if (typeof (okCallback) !== "undefined" && okCallback != null) {
                okCallback(id, data);
            }
            HideWaitDialog();
        }).fail(function (xhr) {
            onAjaxError(xhr);
            HideWaitDialog();
        });
    } else {
        if (typeof (okCallback) !== "undefined" && okCallback != null) {
            okCallback();
        }
        //HideWaitDialog();
    }
}

function CheckErrors(currentForm) {
    if (!jQuery(currentForm).valid()) {
        return false;
    }
    return true;
}

function AjaxCall(options) {
    var url = BASEPATHURL + options.url;
    var postData = options.postData;
    var httpmethod = options.httpmethod;
    var calldatatype = options.calldatatype;
    var sucesscallbackfunction = options.sucesscallbackfunction;
    var contentType = options.contentType == undefined ? "application/x-www-form-urlencoded;charset=UTF-8" : options.contentType;
    var showLoading = options.showLoading == undefined ? true : options.showLoading;
    var isAsync = options.isAsync == undefined ? true : options.isAsync;
    ShowWaitDialog();
    jQuery.ajax({
        type: httpmethod,
        url: url,
        data: postData,
        global: showLoading,
        dataType: calldatatype,
        contentType: contentType,
        async: isAsync,
        cache: false,
        success: function (data) {
            if (typeof (sucesscallbackfunction) !== 'undefined') {
                sucesscallbackfunction(data);
            }
            HideWaitDialog();
        },
        error: function (xhr, textStatus, errorThrown) {
            HideWaitDialog();
            onAjaxError(xhr);
        }
    });
}

function onAjaxError(xhr) {
    if (!UserAborted(xhr)) {
        if (xhr.status.toString().substr(0, 1) == "4" || xhr.status == 504) {
            AlertModal('SessionTimeout', ResourceManager.Message.SessionTimeOut);
        }
        else {
            //This shortcut is not recommended way to track unauthorized action.
            //if (xhr.responseText.indexOf("403.png") > 0) {
            //    window.location = UnAuthorizationUrl;
            //}
            //else {
            //    AlertModal("Error", "System error has occurred.", BootstrapDialog.TYPE_DANGER);
            //}
        }
    }
}

function ShowError(ModelStateErrors) {
    jQuery('input,select,textarea').removeClass("input-validation-error");
    var messages = "";
    jQuery(ModelStateErrors).each(function (i, e) {
        jQuery('[name="' + e.Key + '"]').addClass("input-validation-error");
        messages += "<li>" + e.Value[0] + "</li>";
    });
    messages = "<div><h5>" + "errorTitle" + "</h5><ul>" + messages + "</ul></div>";
}

function ShowWaitDialog() {
    try {
        jQuery("#loading").show();
    }
    catch (ex) {
        // blank catch to handle ie issue in case of CK editor
    }
}

function HideWaitDialog() {
    jQuery("#loading").hide();
}
function SubmitNoRedirect(ele) {
    ValidateCollapseForm();
    var formList = $("form[data-ajax='true']:visible").not(".disabled");
    var isValid = true;
    var dataAction = $(ele).attr("data-action");
    formList.each(function () {
        if (dataAction == "1" || dataAction == "33") {
            $(this).validate().settings.ignore = "*";
        }
        else if (dataAction == "22") {
            $(this).validate().settings.ignore = "*";
            $(".field-validation-error").addClass("field-validation-valid");
            $(".field-validation-valid").removeClass("field-validation-error");
            $(this).validate().settings.ignore = ":not(.requiredOnSendBack)";
        }
        else if (dataAction == "41") {
            $(this).validate().settings.ignore = "*";
            $(".field-validation-error").addClass("field-validation-valid");
            $(".field-validation-valid").removeClass("field-validation-error");
            $(this).validate().settings.ignore = ":not(.requiredOnDelegate)";
        }
        else {
            $(this).validate().settings.ignore = ":hidden";
            $(".field-validation-error").addClass("field-validation-valid");
            $(".field-validation-valid").removeClass("field-validation-error");
        }
        $(this).attr("data-ajax-success", "OnSuccessNoRedirect");
        $(this).find("input[id='ActionStatus']").val($(ele).attr("data-action"));
        $(this).find("input[id='SendBackTo']").val($(ele).attr("data-sendbackto"));
        $(this).find("input[id='SendToRole']").val($(ele).attr("data-sendtorole"));
        if ($(this).find("input[id='ButtonCaption']").length == 0) {
            var input = $("<input id='ButtonCaption' name='ButtonCaption' type='hidden'/>");
            input.val($(ele).text());
            $(this).append(input);
        } else {
            $(this).find("input[id='ButtonCaption']").val($(ele).text());
        }
        if (!$(this).valid()) {
            isValid = false;
            try {
                var validator = $(this).validate();
                $(validator.errorList).each(function (i, errorItem) {
                    console.log("{ '" + errorItem.element.id + "' : '" + errorItem.message + "'}");
                });
            }
            catch (e1) {
                console.log(e1.message);
            }
        }
    });
    if (isValid) {
        ShowWaitDialog();
        jQuery.ajax({
            type: "GET",
            url: BASEPATHURL + "/Master/GetTocken",
            global: true,
            contentType: "application/x-www-form-urlencoded;charset=UTF-8",
            async: true,
            cache: false,
            success: function (result) {
                securityToken = result;
                $(formList[0]).submit();

            },
            error: function (xhr, textStatus, errorThrown) {
                HideWaitDialog();
                onAjaxError(xhr);
            }

        });


    } else {
        if ($(".field-validation-error:first").length > 0) {
            //$("html,body").animate({ "scrollTop": $(".field-validation-error:first").offset().top - 100 });
            $("html,body").animate({ "scrollTop": $(".field-validation-error:first").parents(".card:first").offset().top - 100 });
            setTimeout(function () {
                $(".field-validation-error:first").parent().find("select,input,textarea").first().focus();
            }, 10);
        }
    }
}
function Submit(ele) {
    ValidateCollapseForm();
    var formList = $("form[data-ajax='true']:visible").not(".disabled");
    var isValid = true;
    var dataAction = $(ele).attr("data-action");
    formList.each(function () {
        if (dataAction == "1" || dataAction == "33") {
            $(this).validate().settings.ignore = "*";
            $(".field-validation-error").addClass("field-validation-valid");
            $(".field-validation-valid").removeClass("field-validation-error");

        }
        else if (dataAction == "22") {
            $(this).validate().settings.ignore = "*";
            $(".field-validation-error").addClass("field-validation-valid");
            $(".field-validation-valid").removeClass("field-validation-error");
            $(this).validate().settings.ignore = ":not(.requiredOnSendBack)";
        }
        else if (dataAction == "41") {
            $(this).validate().settings.ignore = "*";
            $(".field-validation-error").addClass("field-validation-valid");
            $(".field-validation-valid").removeClass("field-validation-error");
            $(this).validate().settings.ignore = ":not(.requiredOnDelegate)";
        }
        else {
            $(this).validate().settings.ignore = ":hidden";
        }

        $(this).attr("data-ajax-success", $(this).attr("data-ajax-old-success"));
        $(this).find("input[id='ActionStatus']").val($(ele).attr("data-action"));
        $(this).find("input[id='SendBackTo']").val($(ele).attr("data-sendbackto"));
        $(this).find("input[id='SendToRole']").val($(ele).attr("data-sendtorole"));
        if ($(this).find("input[id='ButtonCaption']").length == 0) {
            var input = $("<input id='ButtonCaption' name='ButtonCaption' type='hidden'/>");
            input.val($(ele).text());
            $(this).append(input);
        } else {
            $(this).find("input[id='ButtonCaption']").val($(ele).text());
        }
        if (!$(this).valid()) {
            isValid = false;
            try {
                var validator = $(this).validate();
                $(validator.errorList).each(function (i, errorItem) {
                    console.log("{ '" + errorItem.element.id + "' : '" + errorItem.message + "'}");
                });
            }
            catch (e1) { }
        }
    });
    //if (dataAction == "22") {
    //    $(".requiredonSendBack").rules("remove", "required");
    //    $(".requiredonSendBack").rules("add", {
    //        required: true,
    //        messages: {
    //            required: "The Permanent Account Number field is required."
    //        }
    //    });
    //    formList.each(function () {
    //    if (!$(this).valid()) {
    //        isValid = false;
    //        try {
    //            var validator = $(".requiredonSendBack").validate();
    //            $(validator.errorList).each(function (i, errorItem) {
    //                console.log("{ '" + errorItem.element.id + "' : '" + errorItem.message + "'}");
    //            });
    //        }
    //        catch (e1) { }
    //    }
    //    });
    //}
    if (isValid) {
        //$(formList).submit();
        ShowWaitDialog();
        jQuery.ajax({
            type: "GET",
            url: BASEPATHURL + "/Master/GetTocken",
            global: true,
            contentType: "application/x-www-form-urlencoded;charset=UTF-8",
            async: true,
            cache: false,
            success: function (result) {
                securityToken = result;
                $(formList[0]).submit();

            },
            error: function (xhr, textStatus, errorThrown) {
                HideWaitDialog();
                onAjaxError(xhr);
            }

        });

        //AjaxCall({
        //    url: BASEPATHURL +  "/Master/GetTocken",
        //    httpmethod: "GET",
        //    async: true,
        //    sucesscallbackfunction: function (result) {
        //        securityToken = result;
        //        $(formList[0]).submit();
        //    }
        //});

    } else {
        if ($(".field-validation-error:first").length > 0) {

            //$("html,body").animate({ "scrollTop": $(".field-validation-error:first").offset().top - 100 });
            $("html,body").animate({ "scrollTop": $(".field-validation-error:first").parents(".card:first").offset().top - 100 });
            setTimeout(function () {
                $(".field-validation-error:first").parent().find("select,input,textarea").first().focus();
            }, 10);
        }
    }
}

function ConfirmNewSubmit(ele) {
    ValidateCollapseForm();
    var formList = $("form[data-ajax='true']:visible").not(".disabled");
    var isValid = true;
    var dataAction = $(ele).attr("data-action");
    formList.each(function () {
        if (dataAction == "1" || dataAction == "33") {
            $(this).validate().settings.ignore = "*";
            $(".field-validation-error").addClass("field-validation-valid");
            $(".field-validation-valid").removeClass("field-validation-error");
        }
        else if (dataAction == "22") {
            $(this).validate().settings.ignore = "*";
            $(".field-validation-error").addClass("field-validation-valid");
            $(".field-validation-valid").removeClass("field-validation-error");
            $(this).validate().settings.ignore = ":not(.requiredOnSendBack)";
        }
        else if (dataAction == "41") {
            $(this).validate().settings.ignore = "*";
            $(".field-validation-error").addClass("field-validation-valid");
            $(".field-validation-valid").removeClass("field-validation-error");
            $(this).validate().settings.ignore = ":not(.requiredOnDelegate)";
        }
        else {
            $(this).validate().settings.ignore = ":hidden";
        }
        $(this).attr("data-ajax-success", $(this).attr("data-ajax-old-success"));
        $(this).find("input[id='ActionStatus']").val($(ele).attr("data-action"));
        $(this).find("input[id='SendBackTo']").val($(ele).attr("data-sendbackto"));
        $(this).find("input[id='SendToRole']").val($(ele).attr("data-sendtorole"));
        if ($(this).find("input[id='ButtonCaption']").length == 0) {
            var input = $("<input id='ButtonCaption' name='ButtonCaption' type='hidden'/>");
            input.val($(ele).text());
            $(this).append(input);
        } else {
            $(this).find("input[id='ButtonCaption']").val($(ele).text());
        }
        if (!$(this).valid()) {
            isValid = false;
            try {
                var validator = $(this).validate();
                $(validator.errorList).each(function (i, errorItem) {
                    console.log("{ '" + errorItem.element.id + "' : '" + errorItem.message + "'}");
                });
            }
            catch (e1) { }
        }
    });

    //confirm file Attachment need attach or not
    var attachmsg = "Are you sure for '" + $.trim($(ele).text()) + "' Form Data?";
    if ($(formList[0]).find("div[data-appname]").length != 0 && $(formList[0]).find("div[data-appname]").find("ul li").length == 0) {
        attachmsg = 'are you sure want to submit without attachment';
    }

    if (isValid) {
        ConfirmationDailog({
            //title: "Confirm", message: "Are you sure for '" + $.trim($(ele).text()) + "' Form Data?", okCallback: function (id, data) {
            title: "Confirm", message: attachmsg, okCallback: function (id, data) {
                //$(formList).submit();
                ShowWaitDialog();
                jQuery.ajax({
                    type: "GET",
                    url: BASEPATHURL + "/Master/GetTocken",
                    global: true,
                    contentType: "application/x-www-form-urlencoded;charset=UTF-8",
                    async: true,
                    cache: false,
                    success: function (result) {
                        securityToken = result;
                        $(formList[0]).submit();

                    },
                    error: function (xhr, textStatus, errorThrown) {
                        HideWaitDialog();
                        onAjaxError(xhr);
                    }

                });
            }
        });
    } else {
        if ($(".field-validation-error:first").length > 0) {
            // $("html,body").animate({ "scrollTop": $(".field-validation-error:first").offset().top - 100 });
            $("html,body").animate({ "scrollTop": $(".field-validation-error:first").parents(".card:first").offset().top - 100 });
            setTimeout(function () {
                $(".field-validation-error:first").parent().find("select,input,textarea").first().focus();
            }, 10);
        }
    }
}

function ConfirmSubmit(ele) {
    ValidateCollapseForm();
    var formList = $("form[data-ajax='true']:visible").not(".disabled");
    var isValid = true;
    var dataAction = $(ele).attr("data-action");
    formList.each(function () {
        if (dataAction == "1" || dataAction == "33") {
            $(this).validate().settings.ignore = "*";
            $(".field-validation-error").addClass("field-validation-valid");
            $(".field-validation-valid").removeClass("field-validation-error");
        }
        else if (dataAction == "22") {
            $(this).validate().settings.ignore = "*";
            $(".field-validation-error").addClass("field-validation-valid");
            $(".field-validation-valid").removeClass("field-validation-error");
            $(this).validate().settings.ignore = ":not(.requiredOnSendBack)";
        }
        else if (dataAction == "41") {
            $(this).validate().settings.ignore = "*";
            $(".field-validation-error").addClass("field-validation-valid");
            $(".field-validation-valid").removeClass("field-validation-error");
            $(this).validate().settings.ignore = ":not(.requiredOnDelegate)";
        }
        else {
            $(this).validate().settings.ignore = ":hidden";
        }
        $(this).attr("data-ajax-success", $(this).attr("data-ajax-old-success"));
        $(this).find("input[id='ActionStatus']").val($(ele).attr("data-action"));
        $(this).find("input[id='SendBackTo']").val($(ele).attr("data-sendbackto"));
        $(this).find("input[id='SendToRole']").val($(ele).attr("data-sendtorole"));
        if ($(this).find("input[id='ButtonCaption']").length == 0) {
            var input = $("<input id='ButtonCaption' name='ButtonCaption' type='hidden'/>");
            input.val($(ele).text());
            $(this).append(input);
        } else {
            $(this).find("input[id='ButtonCaption']").val($(ele).text());
        }
        if (!$(this).valid()) {
            isValid = false;
            try {
                var validator = $(this).validate();
                $(validator.errorList).each(function (i, errorItem) {
                    console.log("{ '" + errorItem.element.id + "' : '" + errorItem.message + "'}");
                });
            }
            catch (e1) { }
        }
    });

    //confirm file Attachment need attach or not
    var attachmsg = "Are you sure to '" + $.trim($(ele).text()) + "'?";
    if ($(formList[0]).find("div[data-appname]").length != 0 && $(formList[0]).find("div[data-appname]").find("ul li").length == 0 && dataAction == "10") {
        attachmsg = "Are you sure to '" + $.trim($(ele).text()) + "' without attachment?";
    }


    if (isValid) {
        ConfirmationDailog({
            title: "Confirm", message: attachmsg, okCallback: function (id, data) {
                //$(formList).submit();
                ShowWaitDialog();
                jQuery.ajax({
                    type: "GET",
                    url: BASEPATHURL + "/Master/GetTocken",
                    global: true,
                    contentType: "application/x-www-form-urlencoded;charset=UTF-8",
                    async: true,
                    cache: false,
                    success: function (result) {
                        securityToken = result;
                        $(formList[0]).submit();

                    },
                    error: function (xhr, textStatus, errorThrown) {
                        HideWaitDialog();
                        onAjaxError(xhr);
                    }

                });
            }
        });
    } else {
        if ($(".field-validation-error:first").length > 0) {
            //$("html,body").animate({ "scrollTop": $(".field-validation-error:first").offset().top - 100 });
            $("html,body").animate({ "scrollTop": $(".field-validation-error:first").parents(".card:first").offset().top - 100 });
            setTimeout(function () {
                $(".field-validation-error:first").parent().find("select,input,textarea").first().focus();
            }, 10);
        }
    }
}


function ValidateCollapseForm() {
    $(".card-body").each(function () {
        if ($(this).hasClass("collapse")) {
            var form = $(this).find("form");
            if (form.length == 0) {
                form = $(this).parents("form");
            }
            if (form.length > 0 && !form.hasClass("disabled")) {
                $(this).removeClass("collapse");
                $(this).addClass("in").css("height", "auto");
            }
        }
    });
}
function Exit(ele) {
    try {
        parent.postMessage(SPHOSTURL, SPHOST);
    }
    catch (e) {
        parent.postMessage($("#hdnSPHOSTURL").val(), $("#hdnSPHOST").val());
    }
}
function ParseMessage(msg) {
    if (msg.length == 1) {
        return msg[0];
    } else {
        var finalMSg = "<ul>";
        $(msg).each(function (i, item) {
            finalMSg += "<li>" + item + "</li>";
        });
        finalMSg += "</ul>";
        return finalMSg;
    }
}
function OnSuccessNoRedirect(data, status, xhr) {
    try {
        if (data.IsSucceed) {
            if (data.IsFile) {
                DownloadUploadedFile("<a data-url='" + data.ExtraData + "'/>", function () {
                    ShowWaitDialog();
                    setTimeout(function () {
                        window.location = window.location.href + (window.location.href.indexOf('?') >= 0 ? "&" : "?");
                    }, 2000)
                });
            } else {
                AlertModal('Success', ParseMessage(data.Messages), true, function () {
                    if (window.location.href.indexOf('&id=' + data.ItemID + "&") >= 0) {
                        ShowWaitDialog();
                        window.location = window.location.href;
                    } else {
                        ShowWaitDialog();
                        window.location = window.location.href.replace("&id={ItemId}&", "&id=" + data.ItemID + "&").replace("&id=", "&id=" + data.ItemID + "&");
                    }
                });
            }
        }
        else {
            AlertModal('Error', ParseMessage(data.Messages));
        }
    }
    catch (e) { window.location.reload(); }
}
function OnSuccess(data, status, xhr) {
    try {
        if (data.IsSucceed) {
            if (data.IsFile) {
                DownloadUploadedFile("<a data-url='" + data.ExtraData + "'/>", function () {
                    ShowWaitDialog();
                    setTimeout(function () {
                        window.location = window.location.href + (window.location.href.indexOf('?') >= 0 ? "&" : "?");
                    }, 2000)
                });
            } else {
                var msg = '';
                if (data.ExtraData != null) {
                    msg = "<b>" + data.ExtraData + "</b>" + "<br>" + ParseMessage(data.Messages);
                }
                else {
                    if ($("#ReferenceNo").length != 0) {
                        msg = $("#ReferenceNo").html() + "<br>" + ParseMessage(data.Messages);
                    }
                    else {
                        msg = ParseMessage(data.Messages);
                    }
                    ////msg = $("#ReferenceNo").html() + "<br>" + ParseMessage(data.Messages);
                }
                //AlertModal('Success', ParseMessage(data.Messages), true);
                AlertModal('Success', msg, true);
            }
        } else {
            AlertModal('Error', ParseMessage(data.Messages));
        }
    }
    catch (e) { window.location.reload(); }
}
function OnFailure(xhr, status, error) {
    try {
        if (xhr.status.toString().substr(0, 1) == "4" || xhr.status == 504) {
            AlertModal('SessionTimeout', ResourceManager.Message.SessionTimeOut);
        }
        else {
            AlertModal('Error', ResourceManager.Message.Error);
        }
    }
    catch (e) { window.location.reload(); }
}
function formatDateTime(date) {
    return date.getUTCDate() + "/" + (date.getUTCMonth() + 1) + "/" + date.getUTCFullYear() + " " + formatAMPM(date);
}
function formatDate(date) {
    return date.getUTCDate() + "/" + (date.getUTCMonth() + 1) + "/" + date.getUTCFullYear();
}
function formatAMPM(date) {
    var hours = date.getUTCHours();
    var minutes = date.getUTCMinutes();
    var ampm = hours >= 12 ? 'PM' : 'AM';
    hours = hours % 12;
    hours = hours ? hours : 12; // the hour '0' should be '12'
    minutes = minutes < 10 ? '0' + minutes : minutes;
    var strTime = hours + ':' + minutes + ' ' + ampm;
    return strTime;
}
function OnTagAdded(ele, id, text) {
    var url = $(ele).attr("data-loadurl");
    var idElement = $(ele).attr("data-idelement");
    if (typeof (idElement) !== "undefind" && idElement != null) {
        $(ele).val(decodeURIComponent(text));
        $("*[id='" + idElement.replace('#', '') + "']").val(id);
    }
    if (typeof (url) !== "undefined" && url != null && id != "undefined") {
        AjaxCall({
            url: url + id,
            httpmethod: "GET",
            sucesscallbackfunction: function (result) {
                $.each(result, function (key, value) {
                    var hasClassInSpan = $("span." + key).find("span").attr("class");
                    if (hasClassInSpan == "comma-space") { hasClassInSpan = ""; }
                    hasClassInSpan = typeof (hasClassInSpan) !== "undefined" && hasClassInSpan != null && hasClassInSpan != '';
                    if (value != null && $.trim(value) != '' && value.toString().indexOf("\Date(") >= 0) {
                        var date = eval('new ' + value.replace(/\//g, ''));
                        var dateString = "";
                        if ($("span." + key).parent().hasClass("datepicker")) {
                            dateString = formatDate(date);
                        } else if ($("span." + key).parent().hasClass("timepicker")) {
                            dateString = formatAMPM(date);
                        } else {
                            dateString = formatDateTime(date);
                        }
                        if (!hasClassInSpan) {
                            $("span." + key).find("span").text(dateString);
                        }
                        $("span." + key).find("input").val(dateString).attr("value", dateString).blur();
                    } else if (value != null && $.trim(value) != '') {
                        if ($("span." + key).find("input.user-tags").length > 0) {
                            $("span." + key).find("input.user-tags").val(value).attr("value", value).blur();
                            $(value.split(',')).each(function (i, email) {
                                $("span." + key).find("input.user-tags").tokenInput("add", { id: $.trim(email), name: $.trim(email) });
                            });
                        } else {
                            if (!hasClassInSpan) {
                                $("span." + key).find("span").text(value);
                            }
                            $("span." + key).find("input").val(value).attr("value", value).blur();
                            $("span." + key).find("select").val(value).attr("value", value).blur();
                            $("span." + key + " option:contains(" + value + ")").attr('selected', 'selected');
                            $("span." + key).find("textarea").val(value).attr("value", value).blur();
                        }
                    }
                });
                $(".comma-space").each(function () {
                    var text = $(this).text();
                    $(this).text(text.replace(/,/g, ', '));
                })
            }
        });
    }
    $(ele).blur();
}
function OnTagRemoved(ele) {
    var eleContainer = $(ele).attr("data-propertycontainer");
    var idElement = $(ele).attr("data-idelement");
    if (typeof (idElement) !== "undefind" && idElement != null) {
        $("*[id='" + idElement.replace('#', '') + "']").val('');
    }
    $(eleContainer).find('span').text('');
    $(eleContainer).find('input').val('').attr("value", "");
    try {
        $(eleContainer).find(".user-tags").tokenInput("clear");
    }
    catch (e) { }
}
function OnTMSSelected(result) {
    $("#SelectMeetingModal").modal('hide');
    var idElement = $("#subSection #TMSRequestID");
    var ele = $("#subSection #Title");
    $(ele).val(result.Title);
    $(idElement).val(result.ID);
    FormatSelectedResult(result);
}
function FormatSelectedResult(result) {
    $.each(result, function (key, value) {
        var hasClassInSpan = $("span." + key).find("span").attr("class");
        if (hasClassInSpan == "comma-space") { hasClassInSpan = ""; }
        hasClassInSpan = typeof (hasClassInSpan) !== "undefined" && hasClassInSpan != null && hasClassInSpan != '';
        if (value != null && $.trim(value) != '' && value.toString().indexOf("\Date(") >= 0) {
            //  if ($("#ListDetails_0__ItemId") == undefined || $("#ListDetails_0__ItemId").val() == undefined || $("#ListDetails_0__ItemId").val().trim() == '0') {
            var date = eval('new ' + value.replace(/\//g, ''));
            var dateString = "";
            if ($("span." + key).parent().hasClass("datepicker")) {
                dateString = formatDate(date);
            } else if ($("span." + key).parent().hasClass("timepicker")) {
                dateString = formatAMPM(date);
            } else {
                dateString = formatDateTime(date);
            }
            if (!hasClassInSpan) {
                $("span." + key).find("span").text(dateString);
            }
            $("span." + key).find("input").val(dateString).attr("value", dateString).blur();
            //}
        } else if (value != null && $.trim(value) != '') {
            // if ($("#ListDetails_0__ItemId") == undefined || $("#ListDetails_0__ItemId").val() == undefined || $("#ListDetails_0__ItemId").val().trim() == '0') { // Prevent get data from TMS in Edit page of AKI
            if ($("span." + key).find("input.user-tags").length > 0) {
                $("span." + key).find("input.user-tags").val(value).attr("value", value).blur();
                $(value.split(',')).each(function (i, email) {
                    $("span." + key).find("input.user-tags").tokenInput("add", { id: $.trim(email), name: $.trim(email) });
                });
            } else {
                if (!hasClassInSpan) {
                    $("span." + key).find("span").text(value);
                }
                $("span." + key).find("input").val(value).attr("value", value).blur();
            }
        }
        //}
    });
    $(".comma-space").each(function () {
        var text = $(this).text();
        $(this).text(text.replace(/,/g, ', '));
    });
}
function IsNullNumber(value, defaultValue) {
    var v = parseFloat(value);
    if (isNaN(v)) {
        return defaultValue;
    }
    return v;
}
function GuideLineDownload(appName, formName, fileName) {
    $("#download_form").remove();
    var download_form = document.createElement('FORM');
    download_form.setAttribute('style', 'display:none;visibility:hidden;');
    download_form.className = "hide";
    download_form.name = 'download_form';
    download_form.id = "download_form";
    download_form.method = 'POST';
    download_form.action = BASEPATHURL + '/Master/DownloadGuideLine';
    var appElement = document.createElement('INPUT');
    appElement.type = 'hidden';
    appElement.name = 'applicationName';
    appElement.id = 'applicationName';
    appElement.value = appName;
    download_form.appendChild(appElement);
    var formElement = document.createElement('INPUT');
    formElement.type = 'hidden';
    formElement.name = 'formName';
    formElement.id = 'formName';
    formElement.value = formName;
    download_form.appendChild(formElement);
    if (typeof (fileName) !== "undefined" && fileName != null) {
        var filenameElement = document.createElement('INPUT');
        filenameElement.type = 'hidden';
        filenameElement.name = 'fileName';
        filenameElement.id = 'fileName';
        filenameElement.value = fileName;
        download_form.appendChild(filenameElement);
    }
    var btnSubmit = document.createElement('INPUT');
    btnSubmit.id = "btnDownloadSubmitFileUploader";
    btnSubmit.type = 'submit';
    download_form.appendChild(btnSubmit);
    //download_form.submit();
    $(download_form).appendTo("body");
    $("#btnDownloadSubmitFileUploader").click();
    $("#download_form").remove();
}
function Print(ele) {
    //var contents = $("body").clone();
    var contents = $("#printContent").clone();
    contents.find("a,button,input[type='button'],input[type='submit']").remove();
    contents.find(".input-group-addon").remove(".input-group-addon");
    contents.find(".qq-upload-button").remove(".qq-upload-button");
    //contents.find(".table").removeClass("table");
    contents.find(".table-bordered").removeClass("table-bordered");
    contents.find(".table-responsive").removeClass("table-responsive");
    //contents.find(".card-head").not(':first').css("display", "block").css("page-break-before", "always");;

    contents.find("input[class='form-control']").each(function () {
        $(this).replaceWith("<span>" + this.value + "</span>");
    });

    var frame1 = $('<iframe />');
    frame1[0].id = "frame1";
    frame1[0].name = "frame1";
    frame1.css({ "position": "absolute", "top": "-1000000px" });
    $("body").append(frame1);
    var frameDoc = frame1[0].contentWindow ? frame1[0].contentWindow : frame1[0].contentDocument.document ? frame1[0].contentDocument.document : frame1[0].contentDocument;
    frameDoc.document.open();
    //Create a new HTML document.

    if ($('#hdnDCRNo').length > 0)
        $("head").find('title').html($('#hdnDCRNo').val())
    if ($('#hdnDCNNo').length > 0)
        $("head").find('title').html($('#hdnDCNNo').val())

    frameDoc.document.write("<html><head><style type='text/css'> html{height:0px !important; } .table{font-size:13px !important; padding:1px !important;}" +
        ".table tr td{padding:1px !important; text-align:center}" +
       // "@media print {div.card-head:not(first-of-type){ display: block; page-break-before: always; }}" +
       " .col-sm-6{display: inline-block; width:47%;}  </style>" + $("head").html());
    frameDoc.document.write('</head><body>');
    frameDoc.document.write(contents.html())
    //Append the DIV contents.
    frameDoc.document.write('</body></html>');
    frameDoc.document.close();
    setTimeout(function () {
        window.frames["frame1"].focus();
        window.frames["frame1"].print();
        frame1.remove();
    }, 500);
}

function PrintPreview(ele) {
    var msg = $("#printContent").clone();
    msg.find("a,button,input[type='button'],input[type='submit']").remove();
    msg.find(".input-group-addon").remove(".input-group-addon");
    msg.find(".qq-upload-button").remove(".qq-upload-button");
    //contents.find(".table").removeClass("table");
    msg.find(".table-bordered").removeClass("table-bordered");
    msg.find(".table-responsive").removeClass("table-responsive");
    //contents.find(".card-head").not(':first').css("display", "block").css("page-break-before", "always");;

    msg.find("input[class='form-control']").each(function () {
        $(this).replaceWith("<span>" + this.value + "</span>");
    });

    AlertModal("Print Preview", msg, false);
}


function PrintWithAttachment(ele) {
    //var contents = $("body").clone();
    $("#printContent").find("logo").attr("images/bajajLogo")
    var contents = $("#printContent").clone();
    contents.find("a,button,input[type='button'],input[type='submit']").remove();
    contents.find(".input-group-addon").remove(".input-group-addon");
    contents.find(".qq-upload-button").remove(".qq-upload-button");
    //contents.find(".table").removeClass("table");
    contents.find(".table-bordered").removeClass("table-bordered");
    contents.find(".table-responsive").removeClass("table-responsive");
    //contents.find(".card-head").not(':first').css("display", "block").css("page-break-before", "always");;

    contents.find("input[class='form-control']").each(function () {
        $(this).replaceWith("<span>" + this.value + "</span>");
    });
    contents.find("#logo").attr("src", BASEPATHURL + "/images/bajajLogo.png")
    var head = $("head").clone();
    head.find("title").remove();
    head.find("script").remove();

    var frame1 = $('<iframe />');
    frame1[0].id = "frame1";
    frame1[0].name = "frame1";
    frame1.css({ "position": "absolute", "top": "-1000000px" });
    $("body").append(frame1);
    var frameDoc = frame1[0].contentWindow ? frame1[0].contentWindow : frame1[0].contentDocument.document ? frame1[0].contentDocument.document : frame1[0].contentDocument;
    frameDoc.document.open();
    //Create a new HTML document.
    frameDoc.document.write("<html><head>" + head.html());
    frameDoc.document.write('</head><body>');
    frameDoc.document.write(contents.html())
    //Append the DIV contents.
    frameDoc.document.write('</body></html>');
    frameDoc.document.close();

    var dataToSend = { 'html': contents.html() };//JSON.stringify({ 'html': contents.html() }); //frameDoc.document.documentElement.innerHTML });
    $.ajax({
        url: BASEPATHURL + "DCR/GetPdf",
        type: "POST",
        //contentType: "application/json;",
        dataType: "json",
        data: dataToSend, // pass that text to the server as a correct JSON String
        success: function (msg) {
            alert(msg);
        },
        error: function (type) { alert("ERROR!!" + type.responseText); }

    });
    //setTimeout(function () {
    //    window.frames["frame1"].focus();
    //    window.frames["frame1"].print();
    //    frame1.remove();
    //}, 500);
}

function ExpandAll(ele) {
    $(".collapse-btn").each(function () {
        if ($(this).hasClass("collapsed")) {
            $(this).click();
        }
    });
}
function CollapseAll(ele) {
    $(".collapse-btn").each(function () {
        if (!$(this).hasClass("collapsed")) {
            $(this).click();
        }
    });
}

function Guideline(ele) {
    GuideLineDownload(AppName, FormName);
}

function AmountToWords(junkVal, inLacs) {
    if (typeof (inLacs) !== "undefined" && inLacs != null && inLacs) {
        junkVal = Number(junkVal) * 100000;
        if (isNaN(junkVal)) {
            junkVal = 0;
        }
    }
    junkVal = Math.floor(junkVal);
    var obStr = new String(junkVal);
    numReversed = obStr.split("");
    actnumber = numReversed.reverse();

    if (Number(junkVal) >= 0) {
        //do nothing
    }
    else {
        return "";
    }
    if (Number(junkVal) == 0) {
        return 'Rupees Zero Only';
    }
    if (actnumber.length > 9) {
        return "";
    }
    var iWords = ["Zero", " One", " Two", " Three", " Four", " Five", " Six", " Seven", " Eight", " Nine"];
    var ePlace = ['Ten', ' Eleven', ' Twelve', ' Thirteen', ' Fourteen', ' Fifteen', ' Sixteen', ' Seventeen', ' Eighteen', ' Nineteen'];
    var tensPlace = ['dummy', ' Ten', ' Twenty', ' Thirty', ' Forty', ' Fifty', ' Sixty', ' Seventy', ' Eighty', ' Ninety'];
    var iWordsLength = numReversed.length;
    var totalWords = "";
    var inWords = new Array();
    var finalWord = "";
    j = 0;
    for (i = 0; i < iWordsLength; i++) {
        switch (i) {
            case 0:
                if (actnumber[i] == 0 || actnumber[i + 1] == 1) {
                    inWords[j] = '';
                }
                else {
                    inWords[j] = iWords[actnumber[i]];
                }
                inWords[j] = inWords[j] + ' Only';
                break;
            case 1:
                tens_complication();
                break;
            case 2:
                if (actnumber[i] == 0) {
                    inWords[j] = '';
                }
                else if (actnumber[i - 1] != 0 && actnumber[i - 2] != 0) {
                    inWords[j] = iWords[actnumber[i]] + ' Hundred and';
                }
                else {
                    inWords[j] = iWords[actnumber[i]] + ' Hundred';
                }
                break;
            case 3:
                if (actnumber[i] == 0 || actnumber[i + 1] == 1) {
                    inWords[j] = '';
                }
                else {
                    inWords[j] = iWords[actnumber[i]];
                }
                if (actnumber[i + 1] != 0 || actnumber[i] > 0) {
                    inWords[j] = inWords[j] + " Thousand";
                }
                break;
            case 4:
                tens_complication();
                break;
            case 5:
                if (actnumber[i] == 0 || actnumber[i + 1] == 1) {
                    inWords[j] = '';
                }
                else {
                    inWords[j] = iWords[actnumber[i]];
                }
                if (actnumber[i + 1] != 0 || actnumber[i] > 0) {
                    inWords[j] = inWords[j] + " Lakh";
                }
                break;
            case 6:
                tens_complication();
                break;
            case 7:
                if (actnumber[i] == 0 || actnumber[i + 1] == 1) {
                    inWords[j] = '';
                }
                else {
                    inWords[j] = iWords[actnumber[i]];
                }
                inWords[j] = inWords[j] + " Crore";
                break;
            case 8:
                tens_complication();
                break;
            default:
                break;
        }
        j++;
    }

    function tens_complication() {
        if (actnumber[i] == 0) {
            inWords[j] = '';
        }
        else if (actnumber[i] == 1) {
            inWords[j] = ePlace[actnumber[i - 1]];
        }
        else {
            inWords[j] = tensPlace[actnumber[i]];
        }
    }
    inWords.reverse();
    for (i = 0; i < inWords.length; i++) {
        finalWord += inWords[i];
    }
    return finalWord;
}

function isNumber(evt, control) {
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode == 46 && control.value.indexOf('.') != -1) {
        return false;
    }
    if (charCode != 46 && charCode > 31 && (charCode < 48 || charCode > 57))
        return false;
    return true;
}
function isInteger(evt, control) {
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    //Char Code : 46 for "."
    //Allow only Numbers
    if (charCode == 46 || charCode > 31 && (charCode < 48 || charCode > 57))
        return false;
    return true;
}
function textAreaAdjust(o) {
    o.style.height = "1px";
    o.style.height = (15 + o.scrollHeight) + "px";
}

function UserAborted(xhr) {
    return !xhr.getAllResponseHeaders();
}
function ValidateUser(eleId, emailId, ItemName) {
    var valid = true;
    var trimdEmail = $.trim(emailId);
    if (ItemName != '') {
        var elements = [];
        if (eleId != "#SBUHeadHOD") {
            if ($("#Attendees").length > 0) {
                elements.push(["#Attendees", "Attendees"]);
            }
        }
        if ($("#Convener").length > 0) {
            elements.push(["#Convener", "Convener"]);
        }
        if ($("#MeetingConvener").length > 0) {
            elements.push(["#MeetingConvener", "Convener"]);
        }
        if (eleId != "#Attendees") {
            if ($("#SBUHeadHOD").length > 0) {
                elements.push(["#SBUHeadHOD", "HOD"]);
            }
        }
        if ($("#ShareWith").length > 0) {
            elements.push(["#ShareWith", "Shared With"]);
        }
        var currentFieldRole = null;
        for (var i = 0; i < elements.length; i++) {
            if (eleId == elements[i][0]) {
                currentFieldRole = elements[i][1];
                break;
            }
        }
        for (var i = 0; i < elements.length; i++) {
            ItemName = decodeURIComponent(ItemName);
            if (eleId != elements[i][0]) {
                if ($(elements[i][0]).val().indexOf(trimdEmail) != -1) {
                    $(eleId).tokenInput("remove", { id: emailId });
                    valid = false;
                    AlertModal(ResourceManager.Title.Error, ItemName.substring(0, ItemName.indexOf(" -")) + " is already entered as " + elements[i][1] + " role, so you can't make same user as " + currentFieldRole + " in this request.", true, function () {
                        setTimeout(function () {
                            $(eleId).parent().find("input:first").focus();
                        }, 100);
                    });
                    break;
                }
            }
        }
    }
    if (valid && eleId == "#Convener") {
        var requestBy = $.trim($("#RequestBy").val());
        var onBehalfOf = $.trim($("#OnBehalfOf").val());
        var convener = $.trim($("#Convener").val());
        if ((requestBy.indexOf(trimdEmail) != -1 && onBehalfOf != '') && convener != '') {
            $(eleId).tokenInput("remove", { id: emailId });
            AlertModal(ResourceManager.Title.Error, "Creator cannot be convener as On Behalf Of is selected.", true, function () {
                setTimeout(function () {
                    $(eleId).parent().find("input:first").focus();
                }, 100);
            });
        }
    }
}
//Added By Ashok(27/06/2016)
function RemoveValidationByName(name) {
    RemoveValidation("input[name='" + name + "']");
    console.log("input[name='" + name + "']");
}
function RemoveValidationById(id) {
    RemoveValidation("#" + id);
}
function RemoveValidation(ele) {
    $(ele).rules("remove", "required");
}
function AddValidationByName(name) {
    AddValidation("input[name='" + name + "']");
}
function AddValidationById(id) {
    AddValidation("#" + id);
}
function AddValidation(ele) {
    $(ele).rules("remove", "required");
    $(ele).rules("add", {
        required: true
    });
}
//Validation Methods End

// Below method for CSRF - Security Testing Start
function BeginClient(xhr) {
    xhr.setRequestHeader('UID', securityToken);
}

function OnModelSubmit(frmid) {
    if ($("#" + frmid).valid()) {
        ShowWaitDialog();
        jQuery.ajax({
            type: "GET",
            url: BASEPATHURL + "/Master/GetTocken",
            global: true,
            contentType: "application/x-www-form-urlencoded;charset=UTF-8",
            async: true,
            cache: false,
            success: function (result) {
                securityToken = result;
                $("#" + frmid).submit();

            },
            error: function (xhr, textStatus, errorThrown) {
                HideWaitDialog();
                onAjaxError(xhr);
            }

        });
    }
    //AjaxCall({
    //    url: BASEPATHURL + "/Master/GetTocken",
    //    httpmethod: "GET",
    //    async: true,
    //    sucesscallbackfunction: function (result) {
    //        securityToken = result;
    //        $("#" + frmid).submit();
    //    }
    //});
    //return false;
}
// Below method for CSRF - Security Testing End


function OpenPrintModel() {
    $('#printModel').modal('show');
}

$(window).load(function () {
    adjustFrameSize(500);
    //adjustFrameSize(document.body.scrollHeight);
    try {
        if ($("#ReferenceNo").length != 0) {
            window.parent.postMessage({
                'func': 'ChangePageTitle',
                'message': $("#ReferenceNo").text()
            }, "*");
        }


    } catch (e1) { }
});

"use strict";
function adjustFrameSize(contentHeight) {
    var senderId,
        resizeMessage = '<message senderId={Sender_ID}>resize({Width}, {Height}px)</message>';

    var args = document.URL.split("?");
    if (args.length < 2) return;
    var params = args[1].split("&");
    for (var i = 0; i < params.length; i = i + 1) {
        var param = params[i].split("=");
        if (param[0].toLowerCase() == "senderid") {
            senderId = decodeURIComponent(param[1]);
            senderId = senderId.split("#")[0]; //for chrome - strip out #/viewname if present
        }
    }

    var step = 30, finalHeight;
    finalHeight = (step - (contentHeight % step)) + contentHeight;

    resizeMessage = resizeMessage.replace("{Sender_ID}", senderId);
    resizeMessage = resizeMessage.replace("{Height}", finalHeight);
    resizeMessage = resizeMessage.replace("{Width}", "100%");
    console.log(resizeMessage);
    window.parent.postMessage(resizeMessage, "*");
}

//Get PerameterValue from current URL
function GetURLPerameterValue(name) {
    var results = new RegExp('[\?&]' + name + '=([^]*)').exec(window.location.href);
    if (results == null) {
        return null;
    }
    else {
        return results[1] || 0;
    }
}