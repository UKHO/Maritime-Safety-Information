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
    var checkboxes = document.getElementsByName('checkbox');
    var selectedIds = [];

    for (var i in checkboxes) {
        if (checkboxes[i].checked && i < checkboxes.length) {
            selectedIds.push(((document.getElementById("Id_" + i)).innerHTML).trim());
        }
    }
    document.getElementById('showSelectionId').value = selectedIds
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
