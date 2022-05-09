$(document).ready(function () {

    $("#btnCreate").click(function () {
        
        var dt = $("#dateTimeGroup").val();

        var d = new Date(dt);
        var firstDate = new Date("1975-01-01");
        var secondDate = new Date("2099-01-01");

        if (d >= firstDate && d <= secondDate) {
            //alert("Success");
        }
        else {
            alert("Please provide date/time range between 01/01/1975 to 01/01/2099.");
            return false;
        }
    });

});
