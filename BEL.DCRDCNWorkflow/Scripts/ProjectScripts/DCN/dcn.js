
$(document).ready(function () {
    $(".sectionDetailType").change();
    // $("#vendortable").hide();
    BindUserTags("");

    //185477515 The user should also be able to view the DCR's which needs to be converted in to DCNs start
    var url = window.location.href;
    if (url.indexOf('DCRID=') != -1) {
        var dCRId = GetURLPerameterValue('DCRID')
        if (dCRId != null && parseInt(dCRId) > 0) {
            SelectProductSKU(dCRId);
        }
    }
    //185477515 The user should also be able to view the DCR's which needs to be converted in to DCNs End
});

function OnSpecificationaddSuccess(data, status, xhr) {
    if (data.IsSucceed) {
        $("#addeditSpecificationModel").modal('hide');
        AlertModal('Success', ParseMessage(data.Messages));
        AjaxCall({
            url: "/DCN/GetSpecification",
            httpmethod: "GET",
            sucesscallbackfunction: function (result) {
                $("#divSpecificationGrid").html(result);
            }
        });
    } else {
        AlertModal('Error', ParseMessage(data.Messages));
    }
}

function SpecificationDelete(index) {
    ConfirmationDailog({
        title: "Delete Specification Detail",
        message: "Are you sure want to delete?",
        url: "/DCN/DeleteSpecification?index=" + index,
        okCallback: function (id, data) {
            OnSpecificationaddSuccess(data);
        }
    });
}

function OnAdminSpecificationaddSuccess(data, status, xhr) {
    if (data.IsSucceed) {
        $("#addeditSpecificationModel").modal('hide');
        AlertModal('Success', ParseMessage(data.Messages));
        AjaxCall({
            url: "/DCNAdmin/GetSpecification",
            httpmethod: "GET",
            sucesscallbackfunction: function (result) {
                $("#divSpecificationGrid").html(result);
            }
        });
    } else {
        AlertModal('Error', ParseMessage(data.Messages));
    }
}

function AdminSpecificationDelete(index) {
    ConfirmationDailog({
        title: "Delete Specification Detail",
        message: "Are you sure want to delete?",
        url: "/DCNAdmin/DeleteSpecification?index=" + index,
        okCallback: function (id, data) {
            OnAdminSpecificationaddSuccess(data);
        }
    });
}


function OnRevisedAppDocaddSuccess(data, status, xhr) {
    if (data.IsSucceed) {
        $("#addeditRevisedAppDocModel").modal('hide');
        AlertModal('Success', ParseMessage(data.Messages));
        AjaxCall({
            url: "/DCN/GetRevisedAppDoc",
            httpmethod: "GET",
            sucesscallbackfunction: function (result) {
                $("#divRevisedAppDocGrid").html(result);
            }
        });
    } else {
        AlertModal('Error', ParseMessage(data.Messages));
    }
}

function DeleteRevisedAppDoc(index) {
    ConfirmationDailog({
        title: "Delete Document Detail",
        message: "Are you sure want to delete?",
        url: "/DCN/DeleteRevisedAppDoc?index=" + index,
        okCallback: function (id, data) {
            OnRevisedAppDocaddSuccess(data);
        }
    });
}

function OnAdminRevisedAppDocaddSuccess(data, status, xhr) {
    if (data.IsSucceed) {
        $("#addeditRevisedAppDocModel").modal('hide');
        AlertModal('Success', ParseMessage(data.Messages));
        AjaxCall({
            url: "/DCNAdmin/GetRevisedAppDoc",
            httpmethod: "GET",
            sucesscallbackfunction: function (result) {
                $("#divRevisedAppDocGrid").html(result);
            }
        });
    } else {
        AlertModal('Error', ParseMessage(data.Messages));
    }
}

function AdminDeleteRevisedAppDoc(index) {
    ConfirmationDailog({
        title: "Delete Document Detail",
        message: "Are you sure want to delete?",
        url: "/DCNAdmin/DeleteRevisedAppDoc?index=" + index,
        okCallback: function (id, data) {
            OnAdminRevisedAppDocaddSuccess(data);
        }
    });
}

function OnVendorEditSuccess(data, status, xhr) {
    if (data.IsSucceed) {
        $("#addeditVendorDCNModel").modal('hide');
        AlertModal('Success', ParseMessage(data.Messages));
        AjaxCall({
            url: "/DCN/GetVendorDCN",
            httpmethod: "GET",
            sucesscallbackfunction: function (result) {
                $("#divqaVendorGrid").html(result);
            }
        });
    } else {
        AlertModal('Error', ParseMessage(data.Messages));
    }
}

//GetDCRDetails Show Print popup - 
function GetDCRDetails() {
    if ($('#DCRID').length > 0 && $('#DCRID').val()!="") {
        $.ajax({
            type: "GET",
            url: BASEPATHURL + '/DCN/GetDCRDetails?itemID='+$('#DCRID').val(),
            beforeSend: function () {
                $("#loading").show();
            },
            success: function (data) {
                if (data != null && data != '') {
                    $("#DCRDetails").html(data);
                    $("#DCRDetails").removeClass('hidden');
                    $('#DCRDetails #printModel').modal('show');
                }
                else {
                    AlertModal("Error", "There is some error occured.");
                }
                $("#loading").hide();
            },
            error: function (data) {
                $("#loading").hide();
            }
    })
    }

}

function SelectProductSKU(itemID) {

    $.ajax({
        type: "GET",
        url: BASEPATHURL + '/DCN/RetrieveDCRNoDetails?itemID=' + itemID,
        //data: { itemID: itemID },
        beforeSend: function () {
            $("#loading").show();
        },
        success: function (data) {
            if (data) {
                if (data.DCNNo != null) {
                    $('#RequestDepartment').val(data.RequestDepartment);
                    $('#ProductName').val(data.ProductName);
                    $('#DCRNo').val(data.DCRNo);
                    $('#DCNNo').val(data.DCNNo);
                    $('#DCRID').val(data.DCRID);
                    $('#QAIncharge').val(data.QAIncharge);
                    $('#QAInchargeName').val(data.QAInchargeName);
                    $('#SCMIncharge').val(data.SCMIncharge);            //185531313
                    $('#SCMInchargeName').val(data.SCMInchargeName);
                    $('#CCIncharge').val(data.CCIncharge);
                    $('#CCInchargeName').val(data.CCInchargeName);
                    $('#DesignEngineerIncharge').val(data.DesignEngineerIncharge);
                    $('#DesignEngineerInchargeName').val(data.DesignEngineerInchargeName);
                    $('#DCRProcessIncharge').val(data.DCRProcessIncharge);
                    $('#DCRProcessInchargeName').val(data.DCRProcessInchargeName);
                    $('#DesignEngineer').val(data.DesignEngineer);
                    $('#DesignEngineerName').val(data.DesignEngineerName);
                    $('#DCRCreator').val(data.ProposedBy);
                    $('#DCRCreatorName').val(data.ProposedByName);
                    $('span.DCRCreatorName').text(data.ProposedByName);
                    $('#DAPMarketingIncharge').val(data.DAPMarketingIncharge);
                    $('#DAPMarketingInchargeName').val(data.DAPMarketingInchargeName);
                    $('#KAPMarketingIncharge').val(data.KAPMarketingIncharge);
                    $('#KAPMarketingInchargeName').val(data.KAPMarketingInchargeName);
                    $('#FANSMarketingIncharge').val(data.FANSMarketingIncharge);
                    $('#FANSMarketingInchargeName').val(data.FANSMarketingInchargeName);
                    $('#LightingMarketingIncharge').val(data.LightingMarketingIncharge);
                    $('#LightingMarketingInchargeName').val(data.LightingMarketingInchargeName);
                    $('#MRMarketingIncharge').val(data.MRMarketingIncharge);
                    $('#MRMarketingInchargeName').val(data.MRMarketingInchargeName);
                    $('#LUMMarketingIncharge').val(data.LUMMarketingIncharge);
                    $('#LUMMarketingInchargeName').val(data.LUMMarketingInchargeName);

                    $('span.RequestDepartment').html(data.RequestDepartment);
                    $("span.ProductName").text(data.ProductName);
                    $("span.DCRNo").text(data.DCRNo);
                    $("span.DCNNo").html("<b>" + data.DCNNo + "</b>");;
                    $("span.QAInchargeName").text(data.QAInchargeName);
                    $("span.SCMInchargeName").text(data.SCMInchargeName);
                    $("span.CCInchargeName").text(data.CCInchargeName);
                    $("span.DesignEngineerInchargeName").text(data.DesignEngineerInchargeName);

                    if (data.DAPMarketingInchargeName != null) { $("span.DAPMarketingInchargeName").text(data.DAPMarketingInchargeName); }
                    if (data.KAPMarketingInchargeName != null) { $("span.KAPMarketingInchargeName").text(data.KAPMarketingInchargeName); }
                    if (data.FANSMarketingInchargeName != null) { $("span.FANSMarketingInchargeName").text(data.FANSMarketingInchargeName); }
                    if (data.LightingMarketingInchargeName != null) { $("span.LightingMarketingInchargeName").text(data.LightingMarketingInchargeName); }
                    if (data.MRMarketingInchargeName != null) { $("span.MRMarketingInchargeName").text(data.MRMarketingInchargeName); }
                    if (data.LUMMarketingInchargeName != null) { $("span.LUMMarketingInchargeName").text(data.LUMMarketingInchargeName); }

                    var dataArray = new Array();
                    for (var o in data.VendorDCRList) {
                        dataArray.push(data.VendorDCRList[o]);
                    }

                    $("#vendortablefrommodel").hide();
                    $("#vendortable").html("")
                    var table = document.createElement("table");
                    var row = table.insertRow(-1);
                    var headerCell = document.createElement("TH");
                    headerCell.innerHTML = "Vendor";
                    row.appendChild(headerCell);
                    //var headerCell = document.createElement("TH");
                    //headerCell.innerHTML = "Quantity";
                    //row.appendChild(headerCell);
                    var headerCell = document.createElement("TH");
                    headerCell.innerHTML = "FG Stock";
                    row.appendChild(headerCell);
                    var headerCell = document.createElement("TH");
                    headerCell.innerHTML = "Existing Component stock";
                    row.appendChild(headerCell);
                    var headerCell = document.createElement("TH");
                    headerCell.innerHTML = "Implementation Date";
                    row.appendChild(headerCell);
                    var tbody = document.createElement("tbody");
                    if (dataArray.length != 0) {
                        for (var i = 0; i < dataArray.length; i++) {
                            row = tbody.insertRow(-1);
                            var cell = row.insertCell(-1);
                            cell.innerHTML = dataArray[i].VendorName;
                            //var cell = row.insertCell(-1);                       
                            //cell.innerHTML = dataArray[i].Quantity; 
                            var cell = row.insertCell(-1);
                            cell.innerHTML = dataArray[i].FGStock;
                            var cell = row.insertCell(-1);
                            cell.innerHTML = dataArray[i].ExistingComponentStock;
                            var date = " ";
                            var cell = row.insertCell(-1);
                            if (dataArray[i].DateOfImplementation != null) {
                                var parsedDate = new Date(parseInt(dataArray[i].DateOfImplementation.substr(6)));
                                var month = parsedDate.getMonth() + 1
                                var day = parsedDate.getDate()
                                var year = parsedDate.getFullYear()
                                date = day + "/" + month + "/" + year
                            }
                            cell.innerHTML = date;

                        }
                    }
                    else {
                        row = tbody.insertRow(-1);
                        var cell = row.insertCell(-1);
                        cell.colSpan = 4;
                        cell.innerHTML = "<span class='text-danger'>No Vendor Data Selected</span>";
                    }
                    table.appendChild(tbody);

                    //$("#vendortable").append(table);
                    $("#vendortable").html(table);
                    $('table').addClass('table table-hover table-bordered someTable');
                    // dvTable.appendChild(tbody);
                    //  $("#nodata").hide();

                    $("#divPrintlink").removeClass('hidden');

                }
                else {
                    AlertModal("Error", "Error while fetching data. kindly contact your DCRDCN Administrator");
                    $('#RequestDepartment').val('');
                    $('#ProductName').val('');
                    $('#DCRNo').val('');
                    $('#DCNNo').val('');
                    $('#DCRID').val('');
                    $('#QAIncharge').val('');
                    $('#QAInchargeName').val('');
                    $('#SCMIncharge').val('');
                    $('#SCMInchargeName').val('');
                    $('#CCIncharge').val('');
                    $('#CCInchargeName').val('');
                    $('#DesignEngineerIncharge').val('');
                    $('#DesignEngineerInchargeName').val('');
                    $('#DCRProcessIncharge').val('');
                    $('#DCRProcessInchargeName').val('');
                    $('#DesignEngineer').val('');
                    $('#DesignEngineerName').val('');

                    $('#DAPMarketingIncharge').val('');
                    $('#DAPMarketingInchargeName').val('');
                    $('#KAPMarketingIncharge').val('');
                    $('#KAPMarketingInchargeName').val('');
                    $('#FANSMarketingIncharge').val('');
                    $('#FANSMarketingInchargeName')
                    $('#LightingMarketingIncharge').val('');
                    $('#LightingMarketingInchargeName').val('');
                    $('#MRMarketingIncharge').val('');
                    $('#MRMarketingInchargeName').val('');
                    $('#LUMMarketingIncharge').val('');
                    $('#LUMMarketingInchargeName').val('');

                    $('span.RequestDepartment').html('');
                    $("span.ProductName").text('');
                    $("span.DCRNo").text('');
                    $("span.DCNNo").text('');
                    $("span.QAInchargeName").text('');
                    $("span.SCMInchargeName").text('');
                    $("span.CCInchargeName").text('');
                    $("span.DesignEngineerInchargeName").text('');

                    $("span.DAPMarketingInchargeName").text('');
                    $("span.KAPMarketingInchargeName").text('');
                    $("span.FANSMarketingInchargeName").text('');
                    $("span.LightingMarketingInchargeName").text('');
                    $("span.MRMarketingInchargeName").text('');
                    $("span.LUMMarketingInchargeName").text('');

                    $("#vendortable").html("");
                }
                $("#loading").hide();
                $("#SelectDCRModal").modal('hide');

            }
            else {
                AlertModal("Error", "There is some error occured.");
            }
            $("#loading").hide();
        }
    })

}


var uploadedFiles = [];
function OnFileUploaded(result) {
    var FileName = $('.qq-upload-file')[0].innerText;
    $('#DocumentNo').val(FileName);
    uploadedFiles.push(result);
    $("#FileNameList").val(JSON.stringify(uploadedFiles)).blur();
}

function AttachFileRemoveImage(ele) {
    var Id = $(ele).attr("data-id");
    var li = $(ele).parents("li.qq-upload-success");
    var itemIdx = li.index();
    ConfirmationDailog({
        title: "Remove", message: "Are you sure to remove file?", id: Id, url: "/DCN/RemoveUploadFile", okCallback: function (id, data) {
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
                    $("#FileNameList").val("").blur();
                } else {
                    $("#FileNameList").val(JSON.stringify(uploadedFiles)).blur();

                }
                $('#DocumentNo').val('');
            }
        }
    });
}


var uploadedPSFiles = [];
function OnPresentSpecificationFileUploaded(result) {
    var FileName = $('.qq-upload-file')[0].innerText;
    uploadedPSFiles.push(result);
    $("#FNLPresentSpecification").val(JSON.stringify(uploadedPSFiles)).blur();
}

function CTRLPresentSpecificationRemoveImage(ele) {
    var Id = $(ele).attr("data-id");
    var li = $(ele).parents("li.qq-upload-success");
    var itemIdx = li.index();
    ConfirmationDailog({
        title: "Remove", message: "Are you sure to remove file?", id: Id, url: "/DCN/RemoveUploadFile", okCallback: function (id, data) {
            li.find(".qq-upload-status-text").remove();
            $('<span class="qq-upload-spinner"></span>').appendTo(li);
            li.removeClass("qq-upload-success");
            var idx = -1;
            var tmpList = [];
            $(uploadedPSFiles).each(function (i, item) {
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
                uploadedPSFiles = tmpList;
                li.remove();
                if (uploadedPSFiles.length == 0) {
                    $("#FNLPresentSpecification").val("").blur();
                } else {
                    $("#FNLPresentSpecification").val(JSON.stringify(uploadedPSFiles)).blur();

                }

            }
        }
    });
}

var uploadedRSFiles = [];
function OnRevisedSpecificationFileUploaded(result) {
    var FileName = $('.qq-upload-file')[0].innerText;
    uploadedRSFiles.push(result);
    $("#FNLRevisedSpecification").val(JSON.stringify(uploadedRSFiles)).blur();
}

function CTRLRevisedSpecificationRemoveImage(ele) {
    var Id = $(ele).attr("data-id");
    var li = $(ele).parents("li.qq-upload-success");
    var itemIdx = li.index();
    ConfirmationDailog({
        title: "Remove", message: "Are you sure to remove file?", id: Id, url: "/DCN/RemoveUploadFile", okCallback: function (id, data) {
            li.find(".qq-upload-status-text").remove();
            $('<span class="qq-upload-spinner"></span>').appendTo(li);
            li.removeClass("qq-upload-success");
            var idx = -1;
            var tmpList = [];
            $(uploadedRSFiles).each(function (i, item) {
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
                uploadedRSFiles = tmpList;
                li.remove();
                if (uploadedRSFiles.length == 0) {
                    $("#FNLRevisedSpecification").val("").blur();
                } else {
                    $("#FNLRevisedSpecification").val(JSON.stringify(uploadedRSFiles)).blur();

                }

            }
        }
    });
}