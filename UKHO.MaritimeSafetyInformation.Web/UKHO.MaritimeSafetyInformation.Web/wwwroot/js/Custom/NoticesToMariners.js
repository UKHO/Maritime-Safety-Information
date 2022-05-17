var yearweekdata;
let onload = false;
var ddlselectedyear;
var ddlselectedweek;

$(function () {
    if (document.getElementById('hdnYear').value != undefined && document.getElementById('hdnYear').value != null && document.getElementById('hdnYear').value != '') {
        ddlselectedyear = document.getElementById('hdnYear').value;
    }

    if (document.getElementById('hdnWeek').value != undefined && document.getElementById('hdnWeek').value != null && document.getElementById('hdnWeek').value != '') {
        ddlselectedweek = document.getElementById('hdnWeek').value;
    }

    if ((ddlselectedyear != undefined && ddlselectedyear != '') && (ddlselectedweek != undefined && ddlselectedweek != '')) {
        LoadData(yearweekdata)
    }
    $("#ddlYears").change(function () {
        GetCorrespondingWeeks($("#ddlYears").val(), yearweekdata);
    });

    $("#ddlWeeks").change(function () {
        if (document.getElementById('ddlYears').selectedIndex != 0)
            this.form.submit();
        else
            return false;
    });

});

function LoadData(data) {
    yearweekdata = data;
    onload = true;
    var selectedyear;

    if (yearweekdata != undefined && yearweekdata.length > 0) {
        $('#ddlYears').empty();
        var yeardata = getUniqueYearandWeeks(data, "Year", "Y").sort()
        var defaultYear = '<option value="0" selected> --Please select year-- </option>'
        $(defaultYear).appendTo('#ddlYears');
        for (i = 0; i < yeardata.length; i++) {
            var year = '<option>' + yeardata[i] + '</option>'
            $(year).appendTo('#ddlYears');
        }
        if (onload) {
            if (ddlselectedyear != undefined && ddlselectedyear != '') {
                selectedyear = ddlselectedyear;
            }
            else {
                selectedyear = yeardata[yeardata.length - 1];
            }
            $('#ddlYears').val(selectedyear);
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

        var defaultweek = '<option value="0" selected> --Please select week-- </option>'
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
