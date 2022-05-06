//Document ready 
$(function () {
  ///////  ShowYearWeekData();
   var getdata= LoadYears();    
    $("#ddlYears").change(function () {
        //////LoadWeeks($("#ddlYears").val());
        GetCorrespondingWeeks($("#ddlYears").val(), getdata);
    });
    $("#ddlWeeks").change(function () {
        ShowWeeklyFilesAsync();
    });
});

function LoadYears() {
    $.ajax({       
        url: '/NoticesToMariners/YearWeek',
        type: "POST",
        dataType: "json",
        success: function (data) {
            $('#ddlYears').empty();
            var div_data = ('<option value="' + data.data.batchAttributes[2].key + '">' + data.data.batchAttributes[2].values + '</option>');
            $(div_data).appendTo('#ddlYears');
            let curYear = new Date().getFullYear()
            var year;
            for (var i = 0; i < 5; i++) {
                if (curYear == data.data.batchAttributes[2].values) {
                     year = curYear;
                    $('#ddlYears').val(year);
                }
                curYear --;
            }
            ////////    $('#ddlYears').val(curYear);
            ////// LoadWeeks(curYear);
            
            GetCorrespondingWeeks(year, data)                       
            return data;
          
        },
        error: function (error) {
            console.log(`Error ${error}`);
        }
    });
}

function GetCorrespondingWeeks(id, data) {
    if (id != "") {
        $('#ddlWeeks').empty();
      //////  var div_data = ('<option value="' + data.data.batchAttributes[0].values + '">' + data.data.batchAttributes[0].key + '</option>');
        let currYearWeek = data.data.batchAttributes[0].values
        var propId = currYearWeek.toString().replace(/ /g, '').split('/');
        if (propId[0] == id){
            for (i = 1; i < propId.length; i++) {
                var weekdata = '<option>' + propId[i].toString().split(',')[0] + '</option>'
                $(weekdata).appendTo('#ddlWeeks');
            }
        }        
    }
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
                ////////
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

function ShowYearWeekData() {
    $.ajax({
        url: '/NoticesToMariners/Yearweek',
        dataType: "json",
        type: "GET",
        data: {
            success: function (data) {
                Console.log(data);
            }
        }

    })
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

