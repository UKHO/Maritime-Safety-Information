var yearweekdata;
let onload = false;
var ddlselectedyear;
var ddlselectedweek;

$(function () {
    if (document.getElementById('hdnYear').value != undefined || document.getElementById('hdnYear').value != null || document.getElementById('hdnYear').value != '') {
        ddlselectedyear = document.getElementById('hdnYear').value;
    }

    if (document.getElementById('hdnWeek').value != undefined || document.getElementById('hdnWeek').value != null || document.getElementById('hdnWeek').value != '') {
        ddlselectedweek = document.getElementById('hdnWeek').value;
    }

    if ((ddlselectedyear != undefined && ddlselectedyear != '') && (ddlselectedweek != undefined && ddlselectedweek != '')) {
        LoadData(yearweekdata)
    }
    $("#ddlYears").change(function () {
        GetCorrespondingWeeks($("#ddlYears").val(), yearweekdata);
    });

});

function LoadData(data) {
    console.log("inside loaddata")
    yearweekdata = data;
    onload = !onload;
    var selectedyear;

    if (yearweekdata.length > 0) {
        $('#ddlYears').empty();
        var yeardata = getUniqueYearandWeeks(data, "Year", "Y").sort()
        var defaultYear = '<option selected> --Please select year-- </option>'
        $(defaultYear).appendTo('#ddlYears');
        for (i = 0; i < yeardata.length; i++) {
            var year = '<option>' + yeardata[i] + '</option>'
            $(year).appendTo('#ddlYears');
        }
        if (onload) {
            if (ddlselectedyear != undefined && ddlselectedyear != '') {
                selectedyear = ddlselectedyear;
                $('#ddlYears').val(selectedyear);
            }
            else {
                selectedyear = yeardata[yeardata.length - 1];
                $('#ddlYears').val(selectedyear);
            }
        }
        else {
            $('#ddlYears').val('--Please select year--');
        }
        GetCorrespondingWeeks(selectedyear, data);
    }
}

function getUniqueYearandWeeks(arr, prop, type) {
    if (type == 'Y') {
        return arr.reduce((a, d) => {
            if (!a.includes(d[prop])) { a.push(d[prop]); }
            return a;
        }, []);
    }
    else if (type == 'W') {
        return arr.reduce((a, d) => {
            if (d.Year == prop) { a.push(d.Week); }
            return a;
        }, []);
    }
}

function GetCorrespondingWeeks(id, data) {
    if (id != "") {
        $('#ddlWeeks').empty();
        var weekdata = getUniqueYearandWeeks(data, id, "W").sort(function (a, b) { return a - b });

        var defaultweek = '<option selected> --Please select week-- </option>'
        $(defaultweek).appendTo('#ddlWeeks');

        for (i = 0; i < weekdata.length; i++) {
            var week = '<option>' + weekdata[i] + '</option>'
            $(week).appendTo('#ddlWeeks');
        }

        if (onload) {
            if (ddlselectedweek != undefined && ddlselectedweek != '') {
                $('#ddlWeeks').val(ddlselectedweek);
            }
            else {
                var selectedweek = weekdata[weekdata.length - 1];
                $('#ddlWeeks').val(selectedweek);
            }

            onload = false;
        }
        else {
            $('#ddlWeeks').val('--Please select week--');
        }
    }
}

function YearValueChange(year) {
    yearSelectedValue = $(year).val();
    document.getElementById('week').selectedIndex = 0;
}
