var yearweekdata;
let onload = false;
var ddlselectedyear;
var ddlselectedweek;

(function () {

    if (document.getElementById('hdnRequestType').value != undefined && document.getElementById('hdnRequestType').value === "Weekly") {

        if (document.getElementById('hdnYear').value != undefined && document.getElementById('hdnYear').value != null && document.getElementById('hdnYear').value != '') {
            ddlselectedyear = document.getElementById('hdnYear').value;
        }

        if (document.getElementById('hdnWeek').value != undefined && document.getElementById('hdnWeek').value != null && document.getElementById('hdnWeek').value != '') {
            ddlselectedweek = document.getElementById('hdnWeek').value;
        }

        if ((ddlselectedyear != undefined && ddlselectedyear != '') && (ddlselectedweek != undefined && ddlselectedweek != '')) {
            LoadData(yearweekdata)
        }

        document.getElementById('ddlYears').onchange = function () {
            GetCorrespondingWeeks(this.value, yearweekdata);
        }

        document.getElementById('ddlWeeks').onchange = function () {
            if (document.getElementById('ddlYears').selectedIndex != 0 && document.getElementById('ddlWeeks').selectedIndex != 0)
                this.form.submit();
            else
                return false;
        }
    }
})();

function LoadData(data) {
    yearweekdata = data;
    onload = true;
    var selectedyear;

    if (yearweekdata != undefined && yearweekdata.length > 0) {
        document.getElementById('ddlYears').length = 0;
        var yeardata = getUniqueYearandWeeks(data, 'Year', 'Y').sort()
        var defaultYear = '<option value="0" selected>Select year</option>'
        document.getElementById('ddlYears').innerHTML = defaultYear;
        for (i = 0; i < yeardata.length; i++) {
            var year = '<option>' + yeardata[i] + '</option>'
            document.getElementById('ddlYears').innerHTML += year;
        }
        if (onload) {
            if (ddlselectedyear != undefined && ddlselectedyear != '') {
                selectedyear = ddlselectedyear;
            }
            else {
                selectedyear = yeardata[yeardata.length - 1];
            }
            document.getElementById('ddlYears').value = selectedyear;
        }
        else {
            document.getElementById('ddlYears').value = 'Select year';
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
    document.getElementById('ddlWeeks').length = 0;
    var weekdata = getUniqueYearandWeeks(data, id, 'W').sort(function (a, b) { return a - b });

    var defaultweek = '<option value="0" selected>Select week</option>'
    document.getElementById('ddlWeeks').innerHTML = defaultweek;

    for (i = 0; i < weekdata.length; i++) {
        var week = '<option>' + weekdata[i] + '</option>'
        document.getElementById('ddlWeeks').innerHTML += week;
    }

    if (onload) {
        if (ddlselectedweek != undefined && ddlselectedweek != '') {
            document.getElementById('ddlWeeks').value = ddlselectedweek;
        }
        else {
            var selectedweek = weekdata[weekdata.length - 1];
            document.getElementById('ddlWeeks').value = selectedweek;
        }

        onload = false;
    }
    else {
        document.getElementById('ddlWeeks').value = '0';
    }
}
