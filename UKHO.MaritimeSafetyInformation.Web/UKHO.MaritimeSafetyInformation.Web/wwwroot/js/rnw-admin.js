var warningTypeSelectedValue;
var yeareSelectedValue;

function WarningTypeValueChange(WarningType) {
    warningTypeSelectedValue = $(WarningType).val();
}

function YearValueChange(Year) {
    yeareSelectedValue = $(Year).val();
}

function RnwFilterList(pageSelectedIndex) {
    warningTypeSelectedValue = $("#WarningType").val();
    yeareSelectedValue = $("#Year").val();

    if (yeareSelectedValue == undefined) {
        yeareSelectedValue = '';
    }

    if (warningTypeSelectedValue == undefined) {
        warningTypeSelectedValue = 0;
    }
    var rnwUrl = "RadioNavigationalWarningsAdmin?pageIndex=ReplacepageSelectedIndex&warningType=ReplaceWarningTypeSelectedValue&year=ReplacYeareSelectedValue";
    rnwUrl = rnwUrl.replace("ReplacepageSelectedIndex", pageSelectedIndex).trim();
    rnwUrl = rnwUrl.replace("ReplaceWarningTypeSelectedValue", warningTypeSelectedValue).trim();
    rnwUrl = rnwUrl.replace("ReplacYeareSelectedValue", yeareSelectedValue).trim();
    rnwUrl = rnwUrl.replace("amp;", "");
    window.location.href = rnwUrl.replace("amp;", "");
}