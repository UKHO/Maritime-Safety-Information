var warningTypeSelectedValue;
var yearSelectedValue;

function WarningTypeValueChange(WarningType) {
    warningTypeSelectedValue = $(WarningType).val();
}

function YearValueChange(Year) {
    yearSelectedValue = $(Year).val();
}

function RnwFilterList(pageSelectedIndex) {
    debugger;
    warningTypeSelectedValue = $("#WarningType").val();
    yearSelectedValue = $("#Year").val();

    if (yearSelectedValue == undefined) {
        yearSelectedValue = null;
    }

    if (warningTypeSelectedValue == undefined) {
        warningTypeSelectedValue = null;
    }
    var rnwUrl = "RadioNavigationalWarningsAdmin?pageIndex=ReplacePageSelectedIndex"
                +"&warningType=ReplaceWarningTypeSelectedValue"
                +"&year=ReplaceYearSelectedValue";

    rnwUrl = rnwUrl.replace("ReplacePageSelectedIndex", pageSelectedIndex).trim();
    rnwUrl = rnwUrl.replace("ReplaceWarningTypeSelectedValue", warningTypeSelectedValue).trim();
    rnwUrl = rnwUrl.replace("ReplaceYearSelectedValue", yearSelectedValue).trim();
    rnwUrl = rnwUrl.replace("amp;", "");
    window.location.href = rnwUrl;
}

function do_Selection() {
    var checkboxes = document.getElementsByName('checkbox');
    var button = document.getElementById('select_button');

    if (button.value == 'Select all') {
        for (var i in checkboxes) {
            checkboxes[i].checked = 'FALSE';
            document.getElementById("BtnShowSelection").disabled = false;
        }
        button.value = 'Clear all'
    } else {
        for (var i in checkboxes) {
            checkboxes[i].checked = '';
            document.getElementById("BtnShowSelection").disabled = true;
        }
        button.value = 'Select all';
    }
}

function showSelection_() {
    var checkboxes = document.getElementsByName('checkbox');
    var len_ = checkboxes.length;
    var arr = [];

    for (var i in checkboxes) {
        if (checkboxes[i].checked && i < len_) {
            arr.push(((document.getElementById("Id_" + i)).innerHTML).trim());
        }
    }
    document.getElementById('showSelectionId').value = arr
}

function terms_changed() {
    var checkboxes = document.getElementsByName('checkbox');
    var len_ = checkboxes.length;
    document.getElementById("BtnShowSelection").disabled = true;

    for (var i in checkboxes) {
        if (checkboxes[i].checked && i < len_) {
            document.getElementById("BtnShowSelection").disabled = false;
        }
    }
}

function printDiv() {
    //Get the HTML of div
    var divElements = document.getElementById("printDiv").innerHTML;
    //Get the HTML of whole page
    //var oldPage = document.body.innerHTML;

    ////Reset the page's HTML with div's HTML only
    //document.body.innerHTML =
    //    "<html><head><title></title></head><body>" +
    //    divElements + "</body>";

    //Print Page
    window.print();

    ////Restore orignal HTML
    //document.body.innerHTML = oldPage;


}

function display() {
    window.print();
}
