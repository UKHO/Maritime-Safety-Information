//Document ready 
$(function () {

    if ($('#hdnRequestType').val() === "Daily") {
        ShowDailyFilesAsync();
    }
    else {
        LoadYears();
        $("#ddlYears").change(function () {
            LoadWeeks($("#ddlYears").val());
        });
        $("#ddlWeeks").change(function () {
            ShowWeeklyFilesAsync();
        });
    }
});

function LoadYears() {
    $.ajax({
        url: '/NoticesToMariners/LoadYears',
        type: "GET",
        dataType: "json",
        success: function (data) {
            $('#ddlYears').empty();
            $.each(data, function (i, data) {
                var div_data = "<option value=" + data.value + ">" + data.key + "</option>";
                $(div_data).appendTo('#ddlYears');
                let curYear = new Date().getFullYear()
                $('#ddlYears').val(curYear);
                LoadWeeks(curYear);

            });
        },
        error: function (error) {
            console.log(`Error ${error}`);
        }
    });
}

function LoadWeeks(selectedYear) {
    if (selectedYear != "") {
        $.ajax({
            url: '/NoticesToMariners/LoadWeeks',
            type: "POST",
            data: {
                year: parseInt(selectedYear)
            },
            dataType: "json",
            success: function (data) {
                $('#ddlWeeks').empty();
                $.each(data, function (i, data) {
                    var div_data = "<option value=" + data.value + ">" + data.key + "</option>";
                    $(div_data).appendTo('#ddlWeeks');
                });
            },
            error: function (error) {
                console.log(`Error ${error}`);
            }
        });
    }
}

function ShowWeeklyFilesAsync() {
    let selectedYear = $('#ddlYears').val();
    let selectedWeek = $('#ddlWeeks').val();
    if (selectedYear != "" && selectedWeek != "") {
        $.ajax({
            url: '/NoticesToMariners/ShowWeeklyFiles',
            type: "POST",
            data: {
                year: parseInt(selectedYear),
                week: parseInt(selectedWeek)
            },
            success: function (data) {
                $('#divFilesList').html(data);
            },
            error: function (error) {
                console.log(`Error ${error}`);
            }
        });
    }
}

function ShowDailyFilesAsync() {

    $.ajax({
        url: '/NoticesToMariners/ShowDailyFiles',
        type: "GET",
        success: function (data) {
            $('#divFilesList').html(data);
        },
        error: function (error) {
            console.log(`Error ${error}`);
        }
    });
}

