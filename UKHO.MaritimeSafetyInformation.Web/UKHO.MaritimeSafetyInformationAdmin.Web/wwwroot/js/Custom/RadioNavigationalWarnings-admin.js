var warningTypeSelectedValue;
var yearSelectedValue;

window.onload = function () {
    document.getElementById('WarningType').addEventListener('change', WarningTypeValueChange);
    document.getElementById('Year').addEventListener('change', YearValueChange);

    let firstPageValue = document.getElementById('firstPageValueId').value;
    let previousPageValue = document.getElementById('previousPageValueId').value;
    let nextPageValue = document.getElementById('nextPageValueId').value;
    let lastPageValue = document.getElementById('lastPageValueId').value;

    document.getElementById('BtnFilter').addEventListener('click', function () { RnwFilterList(firstPageValue); });
    document.getElementById('BtnFirst').addEventListener('click', function () { RnwFilterList(firstPageValue); });
    document.getElementById('BtnPrevious').addEventListener('click', function () { RnwFilterList(previousPageValue); });
    document.getElementById('BtnNext').addEventListener('click', function () { RnwFilterList(nextPageValue); });
    document.getElementById('BtnLast').addEventListener('click', function () { RnwFilterList(lastPageValue); });
}

function WarningTypeValueChange() {
    warningTypeSelectedValue = $("#WarningType").val();
}

function YearValueChange() {
    yearSelectedValue = $("#Year").val();
}

function RnwFilterList(pageSelectedIndex) {
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
