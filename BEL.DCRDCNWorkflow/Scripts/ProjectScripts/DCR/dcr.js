
$(document).ready(function () {
    $('span#negativeSign').hide();
    $(".sectionDetailType").change();

    BindUserTags("");
    Calculation();

    //if (jQuery("#DDHODDISPOSAL").not(":select"))
    if (jQuery("input:hidden[id='DDHODDISPOSAL']").length > 0)
    {

        var value = $("#DDHODDISPOSAL").val();
       debugger;
        if (value == "Consider") {
            $("#IfNotConsidered").rules("remove", "required");
            $("#ReworkComments").rules("remove", "required");

            $(".consider").show();
            $(".notConsider").hide();
            $(".rework").hide();
            $('ul.header-nav > li > a:contains("Reject")').html('<i class="fa fa-save"></i> Confirm');
            $('ul.header-nav > li > a:contains("Rework")').html('<i class="fa fa-stop"></i> Confirm');
            $('ul.header-nav > li > a:contains("Confirm")').attr('data-original-title', 'Request will send back to Creator for rework.')
        }
        else if (value == "Rework") {
            //$("#DesignEngineerIncharge").rules("remove", "required");
            $("#DesignEngineer").rules("remove", "required");
            $("#TargetDateOfImplementation").rules("remove", "required");
            $("#IfNotConsidered").rules("remove", "required");
            $(".consider").hide();
            $(".notConsider").hide();
            $(".rework").show();
            $('ul.header-nav > li > a:contains("Confirm")').html('<i class="fa fa-share"></i> Rework');
            $('ul.header-nav > li > a:contains("Reject")').html('<i class="fa fa-share"></i> Rework');
            $('ul.header-nav > li > a:contains("Rework")').attr('data-original-title', 'Request will send back to Creator for rework.')
        }
        else {
            //$("#DesignEngineerIncharge").rules("remove", "required");
            $("#DesignEngineer").rules("remove", "required");
            $("#TargetDateOfImplementation").rules("remove", "required");
            $("#ReworkComments").rules("remove", "required");
            $(".notConsider").show();
            $(".consider").hide();
            $(".rework").hide();
            $('ul.header-nav > li > a:contains("Confirm")').html('<i class="fa fa-stop"></i> Reject');
            $('ul.header-nav > li > a:contains("Rework")').html('<i class="fa fa-stop"></i> Reject');
            $('ul.header-nav > li > a:contains("Reject")').attr('data-original-title', 'Request will be rejected.');
        }
    }
  
    if (jQuery("input:hidden[id='ConsiderRework']").length > 0) {

        var value = $("#ConsiderRework").val();
        
    
        if (value == "Consider") {
            
            $("#DEIReworkComments").rules("remove", "required");
            $(".consider1").show();         
            $(".rework1").hide();
            $('ul.header-nav > li > a:contains("Rework")').html('<i class="fa fa-save"></i> Submit');
            $('ul.header-nav > li > a:contains("Reject")').html('<i class="fa fa-save"></i> Submit')
            $('ul.header-nav > li > a:contains("Submit")').attr('data-original-title', 'Request will send to Next Approver.')
        }
        else if (value == "Rework") {
            $("#DCRDESIGNENGINEERUser").rules("remove", "required");
            $("#DesignEngineer").rules("remove", "required");
            $(".consider1").hide();
            $(".rework1").show();
            $('ul.header-nav > li > a:contains("Submit")').html('<i class="fa fa-share"></i> Rework');
            $('ul.header-nav > li > a:contains("Reject")').html('<i class="fa fa-share"></i> Rework');
            $('ul.header-nav > li > a:contains("Rework")').attr('data-original-title', 'Request will send back to Creator for rework.')
        }
        else if (value == "Not Consider") {
            $("#DCRDESIGNENGINEERUser").rules("remove", "required");
            $("#DesignEngineer").rules("remove", "required");
            $(".consider1").hide();
            $(".rework1").hide();
            $(".reject").show();
            $('ul.header-nav > li > a:contains("Submit")').html('<i class="fa fa-stop"></i> Reject');
            $('ul.header-nav > li > a:contains("Rework")').html('<i class="fa fa-stop"></i> Reject');
            $('ul.header-nav > li > a:contains("Reject")').attr('data-original-title', 'Request will be rejected.')
        }
    }

    $("#CostReducedIncreasedByRs, #TotalExpectedQuantityInCurrentYe, #TotalExpectedQuantityInNextYear").on("change", function () {
        Calculation();

    });
    //if ($("#AttachFile").length > 0) {
    if ($("#FNLDesignChangeAttachment").length != 0) {
        BindFileUploadControl({
            ElementId: 'AttachmentDesignChange', Params: {}, Url: "UploadFile",
            AllowedExtensions: [],
            MultipleFiles: true,
            CallBack: "OnFileUploadedDesignChange"
        });
        uploadedFiles = BindFileList("FNLDesignChangeAttachment", "AttachmentDesignChange");
    }
    if ($("#FNLExpectedResultsAttachment").length != 0) {
        BindFileUploadControl({
            ElementId: 'AttachmentExpectedResult', Params: {}, Url: "UploadFile",
            AllowedExtensions: [],
            MultipleFiles: true,
            CallBack: "OnFileUploadedExpectedResults"
        });
        uploadedFiles1 = BindFileList("FNLExpectedResultsAttachment", "AttachmentExpectedResult");
    }
    if ($("#FNLQATestReport").length != 0) {
        BindFileUploadControl({
            ElementId: 'AttachmentQATestReport', Params: {}, Url: "UploadFile",
            AllowedExtensions: [],
            MultipleFiles: true,
            CallBack: "OnFileUploadedQATestReport"
        });
        uploadedFiles2 = BindFileList("FNLQATestReport", "AttachmentQATestReport");
    }
    if ($("#FNLDEAttachment").length != 0) {
        BindFileUploadControl({
            ElementId: 'AttachmentDE', Params: {}, Url: "UploadFileWithStamp",
            AllowedExtensions: [],
            MultipleFiles: true,
            CallBack: "OnFileUploadedDE"
        });
        uploadedFiles3 = BindFileList("FNLDEAttachment", "AttachmentDE");
    }

    if ($("#FNLDCRAttachment").length != 0) {
        BindFileUploadControl({
            ElementId: 'AttachmentDCR', Params: {}, Url: "UploadFile",
            AllowedExtensions: [],
            MultipleFiles: false,
            maxFiles: 1,
            CallBack: "OnFileUploadedDCR"
        });
        try{
            uploadedFiles4 = BindFileList("FNLDCRAttachment", "AttachmentDCR");
        }
        catch(E){}
    }
    //}
    //$("#DivisionCode").off("change").on("change", function () {
    //    var value = $("#DivisionCode option:selected").text();
    //    $("#Division").val(value);
    //    $("#tbodyid .token-input-delete-token").click();
    //    if (value != 'Select') {
    //        GetDivisionApproverforDR(value);
    //    }
    //});
    //$("#Division").change();


    $("#DivisionCode").off("change").on("change", function () {
        if ($("#DivisionCode").val() != '') {
            $("#Division").val($.trim($("#DivisionCode option:selected").text().split(' - ')[1]));
        } else {
            $("#Division").val('');
        }
        var value = $("#Division").val();
        $("#SalesGroupCode").html("<option value=''>Select</option>");
        if ($.trim(value) != "") {
            $(GroupList).each(function (i, item) {
                if (item.RelatedTo == value) {
                    var opt = $("<option/>");
                    opt.text(item.Value + ' - ' + item.Title);
                    opt.attr("value", item.Value);
                    opt.appendTo("#SalesGroupCode");
                }
            });
        }
        var selectedSection = $("#SalesGroupCode").attr("data-selected");

        if ($("#SalesGroupCode option[value='" + selectedSection + "']").length > 0) {
            $("#SalesGroupCode").val(selectedSection).change();
        } else {
            $("#SalesGroupCode").val('').change();
        }
    }).change();
    $("#SalesGroupCode").on("change", function () {
        if ($("#SalesGroupCode").val() != '') {
            var divisioncode = $("#DivisionCode option:selected").val();
            var salesgroupcode = $("#SalesGroupCode option:selected").val();
            var email = $("#RequestBy").val();
            $("#SalesGroup").val($.trim($("#SalesGroupCode option:selected").text()));
            GetDivisionApproverforDR(divisioncode, salesgroupcode, email);
        } else {
            $("#SalesGroup").val('');
        }
    }).change();

    $("#divproposedduetoOther").hide();
    $("#Designchangeproposeddueto").off("change").on("change", function () {
        if ($("#Designchangeproposeddueto").val() == 'Others') {
            $("#divproposedduetoOther").show();
        } else {
            $("#divproposedduetoOther").hide();
        }

    }).change();

    $("select#BusinessUnit").off("change").on("change", function () {
        var value = $("#BusinessUnit option:selected").text();
        $("select#DCRDivision").html("");
        $("input[type='hidden'][name='Division']").val("");
        $("select#DCRProductCategory").html("");
        $("input[type='hidden'][name='ProductCategory']").val("");
        if ($.trim(value) != "") {
            $(DivisionList).each(function (i, item) {
                if (item.BusinessUnit == value) {
                    var opt = $("<option/>");
                    opt.text(item.Title);
                    opt.attr("value", item.Value);
                    opt.appendTo("select#DCRDivision");
                }
            });
        }

        $('#DCRDivision').multiselect('destroy');
        $('#DCRDivision').multiselect({
            includeSelectAllOption: true
        });
        //$('#DCRProductCategory').multiselect('rebuild');
        $('#DCRProductCategory').multiselect('destroy');
        $('#DCRProductCategory').multiselect({
            includeSelectAllOption: true
        });

        var selectedSection = $("#DCRDivision").attr("data-selected");
        if ($.trim(selectedSection) != "") {
            $('#DCRDivision').multiselect('select', selectedSection.split(","));
        }
        if ($("#ListDetails_0__ItemId").val() == 0) {
            $("td[data-dept]").each(function () {
                $(this).find("span").text("");
                $(this).find("input.hiddenuser").val("");
            });
        }

        //var selectedSection = $("#DCRDivision").attr("data-selected");

        //if ($("#DCRDivision option[value='" + selectedSection + "']").length > 0) {
        //    $("#DCRDivision").val(selectedSection).change();
        //} else {
        //    $("#DCRDivision").val('').change();
        //}
    }).change();


    $("select#DCRDivision").off("change").on("change", function () {
        var value = $("#DCRDivision option:selected");
        var BusinessUnitvalue = $("#BusinessUnit option:selected").text();
        var selectedDivision = '';
        $("select#DCRProductCategory").html("");
        $(value).each(function () {
            var selctedval = $(this).val();
            selectedDivision = selectedDivision + selctedval + ",";
            if ($.trim(selctedval) != "") {
                $(ProductCategoryList).each(function (i, item) {
                    if (item.Division == selctedval) {
                        var opt = $("<option/>");
                        opt.text(item.Title);
                        opt.attr("value", item.Value);
                        opt.appendTo("select#DCRProductCategory");
                    }
                });
            }

        });
        $("input[type='hidden'][name='Division']").val(selectedDivision.trim(','));

        //if ($("#DCRProductCategory option[value='" + selectedSection + "']").length > 0) {
        //    $("#DCRProductCategory").val(selectedSection).change();
        //} else {
        //    $("#DCRProductCategory").val('').change();
        //}
        $('#DCRProductCategory').multiselect('destroy');
        $('#DCRProductCategory').multiselect({
            includeSelectAllOption: true
        });


        var selectedSection = $("#ProductCategory").val();
        if ($.trim(selectedSection) != "") {
            $('#DCRProductCategory').multiselect('select', selectedSection.split(","));
        }
        selectedSection = $("#DCRProductCategory").attr("data-selected");
        if ($.trim(selectedSection) != "") {
            $('#DCRProductCategory').multiselect('select', selectedSection.split(","));
        }



        //$(".Marketing").each(function () {
        //    $(this).find("span").text("");
        //    $(this).find("input.hiddenuser").val("");
        //});
        if ($("#ListDetails_0__ItemId").val() >= 0) { //updated by kk
            $("td[data-dept]").each(function () {
                $(this).find("span").text("");
                $(this).find("input.hiddenuser").val("");
            });



            if (BusinessUnitvalue.indexOf("LUM") !== -1) {
                $(Approverlist).each(function (i, item) {
                    if (item.Role == "LUM Marketing Incharge") {
                        $(".LUM").find("span").text(item.UserName);
                        $(".LUM").find("input.hiddenuser").val(item.UserID);
                    }
                    else if (item.BusinessUnit == BusinessUnitvalue) {
                        //alert(item.Role + "#" + BusinessUnitvalue + "#" + item.UserName);
                        $("td[data-dept$='" + item.Role + "']").find("span").text(item.UserName);
                        $("td[data-dept$='" + item.Role + "']").find("input.hiddenuser").val(item.UserID);
                    }
                });
            }
            else {
                $(value).each(function () {
                    var div = $(this).val();
                    $(Approverlist).each(function (i, item) {
                        if (item.Role == div + " Marketing Incharge" && item.BusinessUnit == BusinessUnitvalue) {
                            div = div.replace(/ /g, '');
                            $("." + div).find("span").text(item.UserName);
                            $("." + div).find("input.hiddenuser").val(item.UserID);
                        }
                        else if (item.BusinessUnit == BusinessUnitvalue) {
                            // alert(item.Role + "#" + BusinessUnitvalue + "#" + item.UserName);
                            $("td[data-dept$='" + item.Role + "']").find("span").text(item.UserName);
                            $("td[data-dept$='" + item.Role + "']").find("input.hiddenuser").val(item.UserID);
                        }
                    });
                });


            }
        }
   
        if ($("#DCRProductCategory").val() == null) {
            $('#ProductCategory').val('');
           
        }
 
    }).change();

    $("select#DCRProductCategory").off("change").on("change", function () {
        //var value = $("#DCRProductCategory option:selected")
        //var selectedproductCategory = '';
        //$(value).each(function () {
        //    selectedproductCategory = selectedproductCategory + $(this).val() + ",";
        //});
      
        $("input[type='hidden'][name='ProductCategory']").val($('select#DCRProductCategory').val());
        if ($("#DCRProductCategory").val() == null) {
            $('#ProductCategory').val('');

        }
    }).change();


    //Page Load Multiselect Dropdown bind Start
    $('#DCRDivision').multiselect({
        includeSelectAllOption: true
    });
    $('#DCRProductCategory').multiselect({
        includeSelectAllOption: true
    });

    //For QA
    $('#DCRQAUser').multiselect({
        onChange: function (option, checked) {
            // Get selected options.

            var selectedOptions = $('#DCRQAUser option:selected');

            $("input[type='hidden'][name='QAUser']").val($('select#DCRQAUser').val());
            $("input[type='hidden'][name='QAUserName']").val(GetMultiselectValue(selectedOptions));

            if (selectedOptions.length >= 3) {
                // Disable all other checkboxes.
                var nonSelectedOptions = $('#DCRQAUser option').filter(function () {
                    return !$(this).is(':selected');
                });

                nonSelectedOptions.each(function () {
                    var input = $('input[value="' + $(this).val() + '"]');
                    input.prop('disabled', true);
                    input.parent('li').addClass('disabled');
                });
            }
            else {
                // Enable all checkboxes.
                $('#DCRQAUser option').each(function () {
                    var input = $('input[value="' + $(this).val() + '"]');
                    input.prop('disabled', false);
                    input.parent('li').addClass('disabled');
                });
            }

            ShowHideDelegateButton(selectedOptions);
        }
    });

    if ($.trim($("#DCRQAUser").attr("data-selected")) != "") {
        $('#DCRQAUser').multiselect('select', $("#DCRQAUser").attr("data-selected").split(","));
    }

    //Design Engineer
    $('#DCRDESIGNENGINEERUser').multiselect({
        onChange: function (option, checked) {
            // Get selected options.

            var selectedOptions = $('#DCRDESIGNENGINEERUser option:selected');

            $("input[type='hidden'][name='DesignEngineer']").val($('select#DCRDESIGNENGINEERUser').val());
            $("input[type='hidden'][name='DesignEngineerName']").val(GetMultiselectValue(selectedOptions));

            if (selectedOptions.length >= 3) {
                // Disable all other checkboxes.
                var nonSelectedOptions = $('#DCRDESIGNENGINEERUser option').filter(function () {
                    return !$(this).is(':selected');
                });

                nonSelectedOptions.each(function () {
                    var input = $('input[value="' + $(this).val() + '"]');
                    input.prop('disabled', true);
                    input.parent('li').addClass('disabled');
                });
            }
            else {
                // Enable all checkboxes.
                $('#DCRDESIGNENGINEERUser option').each(function () {
                    var input = $('input[value="' + $(this).val() + '"]');
                    input.prop('disabled', false);
                    input.parent('li').addClass('disabled');
                });
            }
            //185490391 - as there is no delegate button for design engineer incharge
            //ShowHideDelegateButton(selectedOptions);
        }
    });

    if ($.trim($("#DCRDESIGNENGINEERUser").attr("data-selected")) != "") {
        $('#DCRDESIGNENGINEERUser').multiselect('select', $("#DCRDESIGNENGINEERUser").attr("data-selected").split(","));
    }


    //For SCM
    $('#DCRSCMUser').multiselect({
        onChange: function (option, checked) {
            // Get selected options.

            var selectedOptions = $('#DCRSCMUser option:selected');

            $("input[type='hidden'][name='SCMUser']").val($('select#DCRSCMUser').val());
            //$("input[type='hidden'][name='SCMUserName']").val($('select#DCRSCMUser option:selected').text());
            $("input[type='hidden'][name='SCMUserName']").val(GetMultiselectValue(selectedOptions));

            if (selectedOptions.length >= 3) {
                // Disable all other checkboxes.
                var nonSelectedOptions = $('#DCRSCMUser option').filter(function () {
                    return !$(this).is(':selected');
                });

                nonSelectedOptions.each(function () {
                    var input = $('input[value="' + $(this).val() + '"]');
                    input.prop('disabled', true);
                    input.parent('li').addClass('disabled');
                });
            }
            else {
                // Enable all checkboxes.
                $('#DCRSCMUser option').each(function () {
                    var input = $('input[value="' + $(this).val() + '"]');
                    input.prop('disabled', false);
                    input.parent('li').addClass('disabled');
                });
            }
            ShowHideDelegateButton(selectedOptions);
            
        }
    });

    if ($.trim($("#DCRSCMUser").attr("data-selected")) != "") {
        $('#DCRSCMUser').multiselect('select', $("#DCRSCMUser").attr("data-selected").split(","));
    }

    

    //For DAP MKT
    $('#DCRDAPMarketingUser').multiselect({
        onChange: function (option, checked) {
            // Get selected options.

            var selectedOptions = $('#DCRDAPMarketingUser option:selected');

            $("input[type='hidden'][name='DAPMarketingUser']").val($('select#DCRDAPMarketingUser').val());
            $("input[type='hidden'][name='DAPMarketingUserName']").val(GetMultiselectValue(selectedOptions));

            if (selectedOptions.length >= 3) {
                // Disable all other checkboxes.
                var nonSelectedOptions = $('#DCRDAPMarketingUser option').filter(function () {
                    return !$(this).is(':selected');
                });

                nonSelectedOptions.each(function () {
                    var input = $('input[value="' + $(this).val() + '"]');
                    input.prop('disabled', true);
                    input.parent('li').addClass('disabled');
                });
            }
            else {
                // Enable all checkboxes.
                $('#DCRDAPMarketingUser option').each(function () {
                    var input = $('input[value="' + $(this).val() + '"]');
                    input.prop('disabled', false);
                    input.parent('li').addClass('disabled');
                });
            }
            ShowHideDelegateButton(selectedOptions);
        }
    });
    if ($.trim($("#DCRDAPMarketingUser").attr("data-selected")) != "") {
        $('#DCRDAPMarketingUser').multiselect('select', $("#DCRDAPMarketingUser").attr("data-selected").split(","));
    }

    //For KAP MKT
    $('#DCRKAPMarketingUser').multiselect({
        onChange: function (option, checked) {
            // Get selected options.

            var selectedOptions = $('#DCRKAPMarketingUser option:selected');

            $("input[type='hidden'][name='KAPMarketingUser']").val($('select#DCRKAPMarketingUser').val());
            $("input[type='hidden'][name='KAPMarketingUserName']").val(GetMultiselectValue(selectedOptions));

            if (selectedOptions.length >= 3) {
                // Disable all other checkboxes.
                var nonSelectedOptions = $('#DCRKAPMarketingUser option').filter(function () {
                    return !$(this).is(':selected');
                });

                nonSelectedOptions.each(function () {
                    var input = $('input[value="' + $(this).val() + '"]');
                    input.prop('disabled', true);
                    input.parent('li').addClass('disabled');
                });
            }
            else {
                // Enable all checkboxes.
                $('#DCRKAPMarketingUser option').each(function () {
                    var input = $('input[value="' + $(this).val() + '"]');
                    input.prop('disabled', false);
                    input.parent('li').addClass('disabled');
                });
            }
            ShowHideDelegateButton(selectedOptions);
        }
    });
    if ($.trim($("#DCRKAPMarketingUser").attr("data-selected")) != "") {
        $('#DCRKAPMarketingUser').multiselect('select', $("#DCRKAPMarketingUser").attr("data-selected").split(","));
    }


    //For FANS MKT
    $('#DCRFANSMarketingUser').multiselect({
        onChange: function (option, checked) {
            // Get selected options.

            var selectedOptions = $('#DCRFANSMarketingUser option:selected');

            $("input[type='hidden'][name='FANSMarketingUser']").val($('select#DCRFANSMarketingUser').val());
            $("input[type='hidden'][name='FANSMarketingUserName']").val(GetMultiselectValue(selectedOptions));

            if (selectedOptions.length >= 3) {
                // Disable all other checkboxes.
                var nonSelectedOptions = $('#DCRFANSMarketingUser option').filter(function () {
                    return !$(this).is(':selected');
                });

                nonSelectedOptions.each(function () {
                    var input = $('input[value="' + $(this).val() + '"]');
                    input.prop('disabled', true);
                    input.parent('li').addClass('disabled');
                });
            }
            else {
                // Enable all checkboxes.
                $('#DCRFANSMarketingUser option').each(function () {
                    var input = $('input[value="' + $(this).val() + '"]');
                    input.prop('disabled', false);
                    input.parent('li').addClass('disabled');
                });
            }
            ShowHideDelegateButton(selectedOptions);
        }
    });

    if ($.trim($("#DCRFANSMarketingUser").attr("data-selected")) != "") {
        $('#DCRFANSMarketingUser').multiselect('select', $("#DCRFANSMarketingUser").attr("data-selected").split(","));
    }

    //For Lighting MKT
    $('#DCRLightingMarketingUser').multiselect({
        onChange: function (option, checked) {
            // Get selected options.

            var selectedOptions = $('#DCRLightingMarketingUser option:selected');

            $("input[type='hidden'][name='LightingMarketingUser']").val($('select#DCRLightingMarketingUser').val());
            $("input[type='hidden'][name='LightingMarketingUserName']").val(GetMultiselectValue(selectedOptions));

            if (selectedOptions.length >= 3) {
                // Disable all other checkboxes.
                var nonSelectedOptions = $('#DCRLightingMarketingUser option').filter(function () {
                    return !$(this).is(':selected');
                });

                nonSelectedOptions.each(function () {
                    var input = $('input[value="' + $(this).val() + '"]');
                    input.prop('disabled', true);
                    input.parent('li').addClass('disabled');
                });
            }
            else {
                // Enable all checkboxes.
                $('#DCRLightingMarketingUser option').each(function () {
                    var input = $('input[value="' + $(this).val() + '"]');
                    input.prop('disabled', false);
                    input.parent('li').addClass('disabled');
                });
            }
            ShowHideDelegateButton(selectedOptions);
        }
    });

    if ($.trim($("#DCRLightingMarketingUser").attr("data-selected")) != "") {
        $('#DCRLightingMarketingUser').multiselect('select', $("#DCRLightingMarketingUser").attr("data-selected").split(","));
    }

    //For LUM MKT
    $('#DCRLUMMarketingUser').multiselect({
        onChange: function (option, checked) {
            // Get selected options.

            var selectedOptions = $('#DCRLUMMarketingUser option:selected');

            $("input[type='hidden'][name='LUMMarketingUser']").val($('select#DCRLUMMarketingUser').val());
            $("input[type='hidden'][name='LUMMarketingUserName']").val(GetMultiselectValue(selectedOptions));

            if (selectedOptions.length >= 3) {
                // Disable all other checkboxes.
                var nonSelectedOptions = $('#DCRLUMMarketingUser option').filter(function () {
                    return !$(this).is(':selected');
                });

                nonSelectedOptions.each(function () {
                    var input = $('input[value="' + $(this).val() + '"]');
                    input.prop('disabled', true);
                    input.parent('li').addClass('disabled');
                });
            }
            else {
                // Enable all checkboxes.
                $('#DCRLUMMarketingUser option').each(function () {
                    var input = $('input[value="' + $(this).val() + '"]');
                    input.prop('disabled', false);
                    input.parent('li').addClass('disabled');
                });
            }
            ShowHideDelegateButton(selectedOptions);
        }
    });
    if ($.trim($("#DCRLUMMarketingUser").attr("data-selected")) != "") {
        $('#DCRLUMMarketingUser').multiselect('select', $("#DCRLUMMarketingUser").attr("data-selected").split(","));
    }

    //For Morphy MKT
    $('#DCRMRMarketingUser').multiselect({
        onChange: function (option, checked) {
            // Get selected options.

            var selectedOptions = $('#DCRMRMarketingUser option:selected');

            $("input[type='hidden'][name='MRMarketingUser']").val($('select#DCRMRMarketingUser').val());
            $("input[type='hidden'][name='MRMarketingUserName']").val(GetMultiselectValue(selectedOptions));

            if (selectedOptions.length >= 3) {
                // Disable all other checkboxes.
                var nonSelectedOptions = $('#DCRMRMarketingUser option').filter(function () {
                    return !$(this).is(':selected');
                });

                nonSelectedOptions.each(function () {
                    var input = $('input[value="' + $(this).val() + '"]');
                    input.prop('disabled', true);
                    input.parent('li').addClass('disabled');
                });
            }
            else {
                // Enable all checkboxes.
                $('#DCRMRMarketingUser option').each(function () {
                    var input = $('input[value="' + $(this).val() + '"]');
                    input.prop('disabled', false);
                    input.parent('li').addClass('disabled');
                });
            }
            ShowHideDelegateButton(selectedOptions);
        }
    });

    if ($.trim($("#DCRMRMarketingUser").attr("data-selected")) != "") {
        $('#DCRMRMarketingUser').multiselect('select', $("#DCRMRMarketingUser").attr("data-selected").split(","));
    }


    //Page Load Multiselect Dropdown bind End

    //185477502 - Hide delegate button if SCM,QA, Design Incharge section is active start
    
    var isSCMInchargeSectionActive = $('#hdnIsSCMInchargeSectionActive').val();

    
    var isQAInchargeSectionActive = $('#hdnIsQAInchargeSectionActive').val();

    
    var isdesignInchargeSectionActive = $('#hdnIsDesignInchargeSectionActive').val();

    var isDAPMarketingInchargeSectionActive = $('#hdnIsDAPMarketingInchargeSectionActive').val();
    var isFANSMarketingInchargeSectionActive = $('#hdnIsFANSMarketingInchargeSectionActive').val();
    var isKAPSMarketingInchargeSectionActive = $('#hdnIsKAPSMarketingInchargeSectionActive').val();
    var isLightingMarketingInchargeSectionActive = $('#hdnIsLightingMarketingInchargeSectionActive').val();
    var isLUMMarketingInchargeSectionActive = $('#hdnIsLUMMarketingInchargeSectionActive').val();
    var isMRMarketingInchargeSectionActive = $('#hdnIsMRMarketingInchargeSectionActive').val();

    if (isSCMInchargeSectionActive!=undefined && isSCMInchargeSectionActive != "disabled") {
        if ($('#DCRSCMUser').length > 0) {
            var selectedOptions = $('#DCRSCMUser option:selected')
            ShowHideDelegateButton(selectedOptions)
        }
        else {
            $('ul.header-nav > li > a:contains("Delegate")').parent().hide();
            $('ul.header-nav > li > a:contains("Submit")').parent().show();
        }
    }

    if (isQAInchargeSectionActive != undefined && isQAInchargeSectionActive != "disabled") {
        if ($('#DCRQAUser').length > 0) {
            var selectedOptions = $('#DCRQAUser option:selected')
            ShowHideDelegateButton(selectedOptions)
        }
        else {
            $('ul.header-nav > li > a:contains("Delegate")').parent().hide();
            $('ul.header-nav > li > a:contains("Submit")').parent().show();
        }
    }
    // 185490391 - no delegate button for design engineer incharge
    //if (isdesignInchargeSectionActive != undefined && isdesignInchargeSectionActive != "disabled") {
    //    if ($('#DCRDESIGNENGINEERUser').length > 0) {
    //        var selectedOptions = $('#DCRDESIGNENGINEERUser option:selected')
    //        ShowHideDelegateButton(selectedOptions)
    //    }
    //    else {
    //        $('ul.header-nav > li > a:contains("Delegate")').parent().hide();
    //        $('ul.header-nav > li > a:contains("Submit")').parent().show();
    //    }
    //}

    if (isDAPMarketingInchargeSectionActive != undefined && isDAPMarketingInchargeSectionActive != "disabled") {
        if ($('#DCRDAPMarketingUser').length > 0) {
            var selectedOptions = $('#DCRDAPMarketingUser option:selected')
            ShowHideDelegateButton(selectedOptions)
        }
        else {
            $('ul.header-nav > li > a:contains("Delegate")').parent().hide();
            $('ul.header-nav > li > a:contains("Submit")').parent().show();
        }
    }


    if (isFANSMarketingInchargeSectionActive != undefined && isFANSMarketingInchargeSectionActive != "disabled") {
        if ($('#DCRFANSMarketingUser').length > 0) {
            var selectedOptions = $('#DCRFANSMarketingUser option:selected')
            ShowHideDelegateButton(selectedOptions)
        }
        else {
            $('ul.header-nav > li > a:contains("Delegate")').parent().hide();
            $('ul.header-nav > li > a:contains("Submit")').parent().show();
        }
    }

    if (isKAPSMarketingInchargeSectionActive != undefined && isKAPSMarketingInchargeSectionActive != "disabled") {
        if ($('#DCRKAPMarketingUser').length > 0) {
            var selectedOptions = $('#DCRKAPMarketingUser option:selected')
            ShowHideDelegateButton(selectedOptions)
        }
        else {
            $('ul.header-nav > li > a:contains("Delegate")').parent().hide();
            $('ul.header-nav > li > a:contains("Submit")').parent().show();
        }
    }

    if (isLightingMarketingInchargeSectionActive != undefined && isLightingMarketingInchargeSectionActive != "disabled") {
        if ($('#DCRLightingMarketingUser').length > 0) {
            var selectedOptions = $('#DCRLightingMarketingUser option:selected')
            ShowHideDelegateButton(selectedOptions)
        }
        else {
            $('ul.header-nav > li > a:contains("Delegate")').parent().hide();
            $('ul.header-nav > li > a:contains("Submit")').parent().show();
        }
    }

    if (isLUMMarketingInchargeSectionActive != undefined && isLUMMarketingInchargeSectionActive != "disabled") {
        if ($('#DCRLUMMarketingUser').length > 0) {
            var selectedOptions = $('#DCRLUMMarketingUser option:selected')
            ShowHideDelegateButton(selectedOptions)
        }
        else {
            $('ul.header-nav > li > a:contains("Delegate")').parent().hide();
            $('ul.header-nav > li > a:contains("Submit")').parent().show();
        }
    }
    
    if (isMRMarketingInchargeSectionActive != undefined && isMRMarketingInchargeSectionActive != "disabled") {
        if ($('#DCRMRMarketingUser').length > 0) {
            var selectedOptions = $('#DCRMRMarketingUser option:selected')
            ShowHideDelegateButton(selectedOptions)
        }
        else {
            $('ul.header-nav > li > a:contains("Delegate")').parent().hide();
            $('ul.header-nav > li > a:contains("Submit")').parent().show();
        }
    }

    function ShowHideDelegateButton(selectedOptions) {
        if (selectedOptions.length > 0) {
            $('ul.header-nav > li > a:contains("Submit")').parent().hide();
            $('ul.header-nav > li > a:contains("Delegate")').parent().show();

        } else {
            $('ul.header-nav > li > a:contains("Delegate")').parent().hide();
            $('ul.header-nav > li > a:contains("Submit")').parent().show();
        }
    }
    //185477502 - delegate button if SCM section is active end


    $(".ddhoddisposal").off("change").on("change", function () {

        var value = $(".ddhoddisposal option:selected").text();
        if (value == '') {
            value = $(".ddhoddisposal").val()
        }

        if (value == "Select") {
            $(".consider").hide();
            $(".notConsider").hide();
            $(".rework").hide();
            $('ul.header-nav > li > a:contains("Rework")').html('<i class="fa fa-save"></i> Confirm')
            $('ul.header-nav > li > a:contains("Reject")').html('<i class="fa fa-save"></i> Confirm')
            $('ul.header-nav > li > a:contains("Confirm")').attr('data-original-title', 'Request will move to the next approval level.')
        }
        else if (value == "Consider") {
            $("#IfNotConsidered").rules("remove", "required");
            $("#ReworkComments").rules("remove", "required");
            //$("#DesignEngineerIncharge").rules("add", {
            //    required: true,
            //    messages: {
            //        required: "Design Engineer Incharge is required."
            //    }
            //});
            $("#DesignEngineer").rules("add", {
                required: true,
                messages: {
                    required: "Design Engineer is required."
                }
            });
            $(".consider").show();
            $(".notConsider").hide();
            $(".rework").hide();
            $('ul.header-nav > li > a:contains("Reject")').html('<i class="fa fa-save"></i> Confirm');
            $('ul.header-nav > li > a:contains("Rework")').html('<i class="fa fa-save"></i> Confirm');
            $('ul.header-nav > li > a:contains("Confirm")').attr('data-original-title', 'Request will move to the next approval level.')
        }
        else if (value == "Rework") {
            //$("#DesignEngineerIncharge").rules("remove", "required");
            $("#DesignEngineer").rules("remove", "required");
            $("#TargetDateOfImplementation").rules("remove", "required");
            $("#IfNotConsidered").rules("remove", "required");
            $("#ReworkComments").rules("add", {
                required: true,
                messages: {
                    required: "Rework reason is required."
                }
            });
            $(".consider").hide();
            $(".notConsider").hide();
            $(".rework").show();
            $('ul.header-nav > li > a:contains("Confirm")').html('<i class="fa fa-share"></i> Rework');
            $('ul.header-nav > li > a:contains("Reject")').html('<i class="fa fa-share"></i> Rework');
            $('ul.header-nav > li > a:contains("Rework")').attr('data-original-title', 'Request will send back to Creator for rework.')
        }
        else {
            //$("#DesignEngineerIncharge").rules("remove", "required");
            $("#DesignEngineer").rules("remove", "required");
            $("#TargetDateOfImplementation").rules("remove", "required");
            $("#ReworkComments").rules("remove", "required");
            $("#IfNotConsidered").rules("add", {
                required: true,
                messages: {
                    required: "Not considered reason is required."
                }
            });
            $(".notConsider").show();
            $(".consider").hide();
            $(".rework").hide();
            $('ul.header-nav > li > a:contains("Confirm")').html('<i class="fa fa-stop"></i> Reject');
            $('ul.header-nav > li > a:contains("Rework")').html('<i class="fa fa-stop"></i> Reject')
            $('ul.header-nav > li > a:contains("Reject")').attr('data-original-title', 'Request will be rejected.');

        }
    }).change();
    
    var valueStatus = $("#Status").val();
   

    $("#ConsiderRework").off("change").on("change", function () {

        var value = $("#ConsiderRework option:selected").text();
        if (value == '') {
            value = $("#ConsiderRework").val();
        }

        if (value == "Select") {
            $(".consider1").hide();
            $(".rework1").hide();
            $('ul.header-nav > li > a:contains("Rework")').html('<i class="fa fa-save"></i> Submit');
            $('ul.header-nav > li > a:contains("Reject")').html('<i class="fa fa-save"></i> Submit')
            $('ul.header-nav > li > a:contains("Submit")').attr('data-original-title', 'Request will move to the next approval level.')
        }
        else if (value == "Consider") {
            $("#DEIReworkComments").rules("remove", "required");
            $("#DCRDESIGNENGINEERUser").rules("add", {
                required: true,
                messages: {
                    required: "Design Engineer is required."
                }
            });
            $(".consider1").show();        
            $(".rework1").hide();
            $('ul.header-nav > li > a:contains("Reject")').html('<i class="fa fa-save"></i> Submit');
            $('ul.header-nav > li > a:contains("Rework")').html('<i class="fa fa-save"></i> Submit');
            $('ul.header-nav > li > a:contains("Submit")').attr('data-original-title', 'Request will move to the next approval level.');
        }

        else if (value == "Rework" ) {
            $("#DesignEngineer").rules("remove", "required");
            $("#DCRDESIGNENGINEERUser").rules("remove", "required");
            $("#DEIReworkComments").rules("add", {
                required: true,
                messages: {
                    required: "Rework reason is required."
                }
            });
            $(".consider1").hide();
            $(".rework1").show();
            if (valueStatus != "Sent Back") {
                $('ul.header-nav > li > a:contains("Submit")').html('<i class="fa fa-share"></i> Rework');
                $('ul.header-nav > li > a:contains("Reject")').html('<i class="fa fa-share"></i> Rework');
                $('ul.header-nav > li > a:contains("Rework")').attr('data-original-title', 'Request will send back to Creator for rework.')
            }
           else
            {
                $('ul.header-nav > li > a:contains("Rework")').html('<i class="fa fa-save"></i> Submit');
                $('ul.header-nav > li > a:contains("Reject")').html('<i class="fa fa-save"></i> Submit');
                $('ul.header-nav > li > a:contains("Submit")').attr('data-original-title', 'Request will move to the next approval level.')
            }
          
        }
        else if (value == "Not Consider") {
            $("#DCRDESIGNENGINEERUser").rules("remove", "required");
            $("#DesignEngineer").rules("remove", "required");
            $(".consider1").hide();
            $(".rework1").hide();
            $(".reject").show();
            $('ul.header-nav > li > a:contains("Submit")').html('<i class="fa fa-stop"></i> Reject');
            $('ul.header-nav > li > a:contains("Rework")').html('<i class="fa fa-stop"></i> Reject');
            $('ul.header-nav > li > a:contains("Reject")').attr('data-original-title', 'Request will be rejected.')
        }
    }).change();

    $("#Feasibility").off("change").on("change", function () {
        var value = $("#Feasibility option:selected").text();
        if (value == '') {
            value = $("#Feasibility").val()
        }
        if (value == "Feasible") {
            $("#CCIfNotFeasibleGiveReason").rules("remove", "required");
            $("#Feasibility").rules("add", {
                required: true,
                messages: {
                    required: "Feasibility is required."
                }
            });
        }
        else {
            $("#Feasibility").rules("remove", "required");
            $("#CCIfNotFeasibleGiveReason").rules("add", {
                required: true,
                messages: {
                    required: "Not Feasible reason is required."
                }
            });
        }
    }).change();

    $("select#IsApproved").off("change").on("change", function () {
        var value = $("#IsApproved option:selected").text();
        if (value == "Approved") {
            $("#IsApproved").rules("remove", "required");
            $("#FinalDesignEngineer").rules("add", {
                required: true,
                messages: {
                    required: "Design Document Engineer is required."
                }
            });
            $(".fnlconsider").show();

        }
        else {
            $("#FinalDesignEngineer").rules("remove", "required");
            $("#IsApproved").rules("add", {
                required: true,
                messages: {
                    required: "Approval is required."
                }
            });
            $(".fnlconsider").hide();

        }
    }).change();

    $("select#EffectOnCostOfPartAndProduct").off("change").on("change", function () {
        var value = $("#EffectOnCostOfPartAndProduct option:selected").text();
        if (value == "Cost Remains Same") {
            $("input#CostReducedIncreasedByRs").val('0');
            $("#CostReducedIncreasedByRs").rules("remove", "required");
            $("input#CostReducedIncreasedByRs").prop("disabled", true);
        }

        else {
            $("input#CostReducedIncreasedByRs").val($("input#CostReduceValue").val());
            $("input#CostReducedIncreasedByRs").prop("disabled", false);
            $("#CostReducedIncreasedByRs").rules("add", {
                required: true,
                messages: {
                    required: "Cost Reduced/Increased By Rs is required."
                }
            });
        }

        Calculation();
        if (value == "Cost Reduction") {
            $("span#negativeSign").show();

        }
        else {
            $('span#negativeSign').hide();

        }
    }).change();

    $("select#DesignEngineer").off("change").on("change", function () {
        $("input[type='hidden'][name='DesignEngineerName']").val($('select#DesignEngineer option:selected').text());
    });
    //$("select#DesignEngineerIncharge").off("change").on("change", function () {
    //    $("input[type='hidden'][name='DesignEngineerInchargeName']").val($('select#DesignEngineerIncharge option:selected').text());
    //});

    $("select#DCRIncharge").off("change").on("change", function () {
        $("input[type='hidden'][name='DCRInchargeName']").val($('select#DCRIncharge option:selected').text());
    });
    $("select#FinalDesignEngineer").off("change").on("change", function () {
        $("input[type='hidden'][name='FinalDesignEngineerName']").val($('select#FinalDesignEngineer option:selected').text());
    });

    setTimeout(function () {
        if ($("#BusinessUnit").val().indexOf("CP") > 0) {
            $(".forLUMGrid").addClass("hide");
        }
    }, 1000);
   
});

var uploadedFiles = [], uploadedFiles1 = [], uploadedFiles2 = [], uploadedFiles3 = [], uploadedFiles4 = [];
function OnFileUploadedDesignChange(result) {
    uploadedFiles.push(result);
    $("#FNLDesignChangeAttachment").val(JSON.stringify(uploadedFiles)).blur();
}
function OnFileUploadedExpectedResults(result) {
    uploadedFiles1.push(result);
    $("#FNLExpectedResultsAttachment").val(JSON.stringify(uploadedFiles1)).blur();
}
function OnFileUploadedQATestReport(result) {
    uploadedFiles2.push(result);
    $("#FNLQATestReport").val(JSON.stringify(uploadedFiles2)).blur();
}
function OnFileUploadedDE(result) {
    uploadedFiles3.push(result);
    $("#FNLDEAttachment").val(JSON.stringify(uploadedFiles3)).blur();
}
function OnFileUploadedDCR(result) {
    uploadedFiles4.push(result);
    $("#FNLDCRAttachment").val(JSON.stringify(uploadedFiles4)).blur();
}

function AttachmentDesignChangeRemoveImage(ele) {
    var Id = $(ele).attr("data-id");
    var li = $(ele).parents("li.qq-upload-success");
    var itemIdx = li.index();
    ConfirmationDailog({
        title: "Remove", message: "Are you sure to remove file?", id: Id, url: "/DCR/RemoveUploadFile", okCallback: function (id, data) {
            li.find(".qq-upload-status-text").remove();
            $('<span class="qq-upload-spinner"></span>').appendTo(li);
            li.removeClass("qq-upload-success");
            var idx = -1;
            var tmpList = [];
            $(uploadedFiles).each(function (i, item) {
                if (idx == -1 && item.FileId == id) {
                    idx = i;
                    if (item.Status == 0) {
                        item.Status = 2;
                        tmpList.push(item);
                    }
                } else {
                    tmpList.push(item);
                }
            });
            if (idx >= 0) {
                uploadedFiles = tmpList;
                li.remove();
                if (uploadedFiles.length == 0) {
                    $("#FNLDesignChangeAttachment").val("").blur();
                } else {
                    $("#FNLDesignChangeAttachment").val(JSON.stringify(uploadedFiles)).blur();
                }
            }
        }
    });
}

function AttachmentExpectedResultRemoveImage(ele) {
    var Id = $(ele).attr("data-id");
    var li = $(ele).parents("li.qq-upload-success");
    var itemIdx = li.index();
    ConfirmationDailog({
        title: "Remove", message: "Are you sure to remove file?", id: Id, url: "/DCR/RemoveUploadFile", okCallback: function (id, data) {
            li.find(".qq-upload-status-text").remove();
            $('<span class="qq-upload-spinner"></span>').appendTo(li);
            li.removeClass("qq-upload-success");
            var idx = -1;
            var tmpList = [];
            $(uploadedFiles1).each(function (i, item) {
                if (idx == -1 && item.FileId == id) {
                    idx = i;
                    if (item.Status == 0) {
                        item.Status = 2;
                        tmpList.push(item);
                    }
                } else {
                    tmpList.push(item);
                }
            });
            if (idx >= 0) {
                uploadedFiles1 = tmpList;
                li.remove();
                if (uploadedFiles1.length == 0) {
                    $("#FNLExpectedResultsAttachment").val("").blur();
                } else {
                    $("#FNLExpectedResultsAttachment").val(JSON.stringify(uploadedFiles1)).blur();
                }
            }
        }
    });
}

function AttachmentQATestReportRemoveImage(ele) {
    var Id = $(ele).attr("data-id");
    var li = $(ele).parents("li.qq-upload-success");
    var itemIdx = li.index();

    ConfirmationDailog({
        title: "Remove", message: "Are you sure to remove file?", id: Id, url: "/DCR/RemoveUploadFile", okCallback: function (id, data) {
            li.find(".qq-upload-status-text").remove();
            $('<span class="qq-upload-spinner"></span>').appendTo(li);
            li.removeClass("qq-upload-success");
            var idx = -1;
            var tmpList = [];
            $(uploadedFiles2).each(function (i, item) {
                if (idx == -1 && item.FileId == id) {
                    idx = i;
                    if (item.Status == 0) {
                        item.Status = 2;
                        tmpList.push(item);
                    }
                } else {
                    tmpList.push(item);
                }
            });
            if (idx >= 0) {
                uploadedFiles2 = tmpList;
                li.remove();
                if (uploadedFiles2.length == 0) {
                    $("#FNLQATestReport").val("").blur();
                } else {
                    $("#FNLQATestReport").val(JSON.stringify(uploadedFiles2)).blur();
                }
            }
        }
    });
}

function AttachmentDERemoveImage(ele) {
    var Id = $(ele).attr("data-id");
    var li = $(ele).parents("li.qq-upload-success");
    var itemIdx = li.index();

    ConfirmationDailog({
        title: "Remove", message: "Are you sure to remove file?", id: Id, url: "/DCR/RemoveUploadFile", okCallback: function (id, data) {
            li.find(".qq-upload-status-text").remove();
            $('<span class="qq-upload-spinner"></span>').appendTo(li);
            li.removeClass("qq-upload-success");
            var idx = -1;
            var tmpList = [];
            $(uploadedFiles3).each(function (i, item) {
                if (idx == -1 && item.FileId == id) {
                    idx = i;
                    if (item.Status == 0) {
                        item.Status = 2;
                        tmpList.push(item);
                    }
                } else {
                    tmpList.push(item);
                }
            });
            if (idx >= 0) {
                uploadedFiles3 = tmpList;
                li.remove();
                if (uploadedFiles3.length == 0) {
                    $("#FNLDEAttachment").val("").blur();
                } else {
                    $("#FNLDEAttachment").val(JSON.stringify(uploadedFiles3)).blur();
                }
            }
        }
    });
}

function AttachmentDCRRemoveImage(ele) {
    var Id = $(ele).attr("data-id");
    var li = $(ele).parents("li.qq-upload-success");
    var itemIdx = li.index();

    ConfirmationDailog({
        title: "Remove", message: "Are you sure to remove file?", id: Id, url: "/DCR/RemoveUploadFile", okCallback: function (id, data) {
            li.find(".qq-upload-status-text").remove();
            $('<span class="qq-upload-spinner"></span>').appendTo(li);
            li.removeClass("qq-upload-success");
            var idx = -1;
            var tmpList = [];
            $(uploadedFiles4).each(function (i, item) {
                if (idx == -1 && item.FileId == id) {
                    idx = i;
                    if (item.Status == 0) {
                        item.Status = 2;
                        tmpList.push(item);
                    }
                } else {
                    tmpList.push(item);
                }
            });
            if (idx >= 0) {
                uploadedFiles4 = tmpList;
                li.remove();
                if (uploadedFiles4.length == 0) {
                    $("#FNLDCRAttachment").val("").blur();
                } else {
                    $("#FNLDCRAttachment").val(JSON.stringify(uploadedFiles4)).blur();
                }
            }
        }
    });
}


function OnVendoraddSuccess(data, status, xhr) {
    if (data.IsSucceed) {
        $("#addeditVendorModel").modal('hide');
        AlertModal('Success', ParseMessage(data.Messages));
        AjaxCall({
            url: "/DCR/GetVendorGrid",
            httpmethod: "GET",
            sucesscallbackfunction: function (result) {
                $("#divVendorGrid").html(result);
            }
        });
    } else {
        AlertModal('Error', ParseMessage(data.Messages));
    }
}

function VendorDelete(index) {
    ConfirmationDailog({
        title: "Delete Vendor",
        message: "Are you sure want to delete?",
        url: "/DCR/DeleteVendor?index=" + index,
        okCallback: function (id, data) {
            OnVendoraddSuccess(data);
        }
    });
}

function OnAdminVendoraddSuccess(data, status, xhr) {
    if (data.IsSucceed) {
        $("#addeditVendorModel").modal('hide');
        AlertModal('Success', ParseMessage(data.Messages));
        AjaxCall({
            url: "/DCRAdmin/GetVendorGrid",
            httpmethod: "GET",
            sucesscallbackfunction: function (result) {
                $("#divVendorGrid").html(result);
            }
        });
    } else {
        AlertModal('Error', ParseMessage(data.Messages));
    }
}

function AdminVendorDelete(index) {
    ConfirmationDailog({
        title: "Delete Vendor",
        message: "Are you sure want to delete?",
        url: "/DCRAdmin/VendorDelete?index=" + index,
        okCallback: function (id, data) {
            OnAdminVendoraddSuccess(data);
        }
    });
}

function printwithAttchment() {
    var dataToSend = JSON.stringify({ 'num': HTML });
    $.ajax({
        url: "EditingTextarea.aspx/GetValue",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: dataToSend, // pass that text to the server as a correct JSON String
        success: function (msg) { alert(msg.d); },
        error: function (type) { alert("ERROR!!" + type.responseText); }

    });
}
////Use this method in DCR WIP Report
function ViewWIPReport(url) {
    url = SPHOSTURL + url;
    parent.postMessage(url, SPHOST);
    // window.location.href = url;
}

$(document).ready(function () {
    $("#wipDCRTable").DataTable({
        "paging": false,
        "searching": true,
        "info": false,
        "order": [[1, "asc"]],
        "fixedHeader": true
    });
});

function GetMultiselectValue(options) {
    var selected = '';
    options.each(function () {
        var label = ($(this).attr('label') !== undefined) ? $(this).attr('label') : $(this).text();
        selected += label + ",";
    });
    return selected.substr(0, selected.length - 1);
}

function Calculation() {

    var costVal = IsNullNumber($("#CostReducedIncreasedByRs").val(), 0);
    var currentQty = IsNullNumber($("#TotalExpectedQuantityInCurrentYe").val(), 0);
    var nextQty = IsNullNumber($("#TotalExpectedQuantityInNextYear").val(), 0);
    var total = parseFloat(costVal * (currentQty + nextQty)).toFixed(2)

    $("#TotalBenefitLossInRupeesLakhs").val(total);
    $("#TotalBenefitLoss").val(total);
    $("input#TotalBenefitLoss").prop("disabled", true);


}
