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
