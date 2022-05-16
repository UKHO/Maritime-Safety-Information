//////Document ready 

var yearweekdata;
let onload = false;
$(function () {
    LoadYears();

    $("#ddlYears").change(function () {
        GetCorrespondingWeeks($("#ddlYears").val(), yearweekdata);

    });
    $("#ddlWeeks").change(function () {
        ShowWeeklyFilesAsync();
    });
});

function LoadYears() {
    $.ajax({
        url: '/NoticesToMariners/GetAllYearandWeeks',
        type: "POST",
        dataType: "json",
        success: function (data) {
            yearweekdata = data;
            onload = !onload;
            var selectedyear;

            if (yearweekdata.length > 0) {
                $('#ddlYears').empty();
                var yeardata = getUniqueYearandWeeks(data, "year", "Y").sort()
                var defaultYear = '<option selected> --Please select year-- </option>'
                $(defaultYear).appendTo('#ddlYears');
                for (i = 0; i < yeardata.length; i++) {
                    var year = '<option>' + yeardata[i] + '</option>'
                    $(year).appendTo('#ddlYears');
                }
                if (onload) {
                    selectedyear = yeardata[yeardata.length - 1];
                    $('#ddlYears').val(selectedyear);
                }
                else {
                    $('#ddlYears').val('--Please select year--');
                }
                GetCorrespondingWeeks(selectedyear, data);
            }
        },
        error: function (error) {
            console.log(`Error ${error}`);
        }
    });
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
            if (d.year == prop) { a.push(d.week); }
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
            var selectedweek = weekdata[weekdata.length - 1];
            $('#ddlWeeks').val(selectedweek);
            ShowWeeklyFilesAsync();
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
