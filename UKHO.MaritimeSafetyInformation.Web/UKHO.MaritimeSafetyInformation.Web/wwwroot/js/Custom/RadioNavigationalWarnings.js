function do_Selection() {
    var checkboxes = document.getElementsByName('checkbox');
    var button = document.getElementById('select_button');

    if (button.value == 'Select all') {
        for (var i in checkboxes) {
            checkboxes[i].checked = 'FALSE';
        }
        document.getElementById("BtnShowSelection").disabled = false;
        button.value = 'Clear all'
    } else {
        for (var i in checkboxes) {
            checkboxes[i].checked = '';
        }
        document.getElementById("BtnShowSelection").disabled = true;
        button.value = 'Select all';
    }
}

function showSelection() {
    //var checkboxes = Array.from(document.querySelectorAll('input[type=checkbox]:checked'));

    //var selectedIds = [];

    ////for (var checkbox in checkboxes) {
    ////    console.log(checkbox.getAttribute("warning-id"));
    ////    //if (checkboxes[i].checked && i < checkboxes.length) {

    ////       // selectedIds.push(((document.getElementById("Id_" + i)).innerHTML).trim());
    ////   // }
    ////}
    //console.log(checkboxes);

    //for (var i = 0; i < checkboxes.length; i++) {
    //    selectedIds.push(checkboxes[i].getAttribute("warning-id"));
    //}
    //document.getElementById('showSelectionId').value = selectedIds
}

function enableDisableShowSelection() {
    var checkboxes = document.getElementsByName('checkbox');
    document.getElementById("BtnShowSelection").disabled = true;

    for (var i in checkboxes) {
        if (checkboxes[i].checked && i < checkboxes.length) {
            document.getElementById("BtnShowSelection").disabled = false;
        }
    }
}
