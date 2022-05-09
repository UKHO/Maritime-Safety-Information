var warningTypeSelectedValue;
var yearSelectedValue;

function WarningTypeValueChange(WarningType) {
    warningTypeSelectedValue = $(WarningType).val();
}

function YearValueChange(Year) {
    yearSelectedValue = $(Year).val();
}

function RnwFilterList(pageSelectedIndex) {
    warningTypeSelectedValue = $("#WarningType").val();
    yearSelectedValue = $("#Year").val();

    if (yearSelectedValue == undefined) {
        yearSelectedValue = '';
    }

    if (warningTypeSelectedValue == undefined) {
        warningTypeSelectedValue = 0;
    }
    var rnwUrl = "RadioNavigationalWarningsAdmin?pageIndex=ReplacepageSelectedIndex&warningType=ReplaceWarningTypeSelectedValue&year=ReplacYeareSelectedValue&reLoadData=ReplacReLoadData";
    rnwUrl = rnwUrl.replace("ReplacepageSelectedIndex", pageSelectedIndex).trim();
    rnwUrl = rnwUrl.replace("ReplaceWarningTypeSelectedValue", warningTypeSelectedValue).trim();
    rnwUrl = rnwUrl.replace("ReplacYeareSelectedValue", yearSelectedValue).trim();
    rnwUrl = rnwUrl.replace("ReplacReLoadData", "false").trim();
    rnwUrl = rnwUrl.replace("amp;", "");
    window.location.href = rnwUrl.replace("amp;", "");
}
