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

function enableDisableShowSelection() {
    var checkboxes = document.getElementsByName('checkbox');
    document.getElementById("BtnShowSelection").disabled = true;

    for (var i in checkboxes) {
        if (checkboxes[i].checked && i < checkboxes.length) {
            document.getElementById("BtnShowSelection").disabled = false;
        }
    }
}

let dropdownParent = document.querySelector('.site-header .dropdown');
let dropdowToggleButton = document.querySelector('.site-header .dropdown-toggle');
if (document.getElementById('hdnLoggedIn').value === "Y") {
    dropdownParent.addEventListener('mouseenter', function () {
        dropdowToggleButton.click();
    })
    dropdownParent.addEventListener('mouseleave', function () {
        dropdowToggleButton.click();
    })
    //site header dropdown menu on mouse hover ends here//
}
const allwarningrows = Array.from(document.querySelectorAll(".rnw-allwarnings-table tbody tr"));
const tableRef = document.querySelector(".rnw-allwarnings-table");
const tableRefBody = document.querySelector(".rnw-allwarnings-table tbody");
const select_button = document.querySelector("#select_button");
let selected_tab;
let selectedIds = [];

let allWarningTabButton = document.querySelector('#allwarnings-tab');
let navarea1TabButton = document.querySelector('#NAVAREA1-tab');
let ukcostalTabButton = document.querySelector('#ukcoastal-tab');
let rnwTabContent = document.querySelector('#rnwTabContent');
let toggleDetailsButtons = Array.from(document.querySelectorAll('.rnw-allwarnings-table .accordion-button'));

allWarningTabButton.addEventListener('click', function () {
    selected_tab = undefined;
    const rows = getFilteredWarningRows();
    insertWarningRows(rows)
})
navarea1TabButton.addEventListener('click', function () {
    selected_tab = "N";
    const rows = getFilteredWarningRows("N");
    insertWarningRows(rows)
})
ukcostalTabButton.addEventListener('click', function () {
    selected_tab = "U";
    const rows = getFilteredWarningRows("U");
    insertWarningRows(rows)
})

toggleDetailsButtons.map(function (toggleDetailsButton, index) {

    toggleDetailsButton.addEventListener('click', function (event) {

        console.log(event.target, event.target.children);
        resetViewdetailButtonText();
        if (toggleDetailsButton.classList.contains("collapsed")) {
            toggleDetailsButton.children[1].innerText = "View details";
        }
        else {
            toggleDetailsButton.children[1].innerText = "Hide details";
        }
    })
})

function resetViewdetailButtonText() {
    toggleDetailsButtons.map(function (toggleDetailsButton, index) {
        if (toggleDetailsButton.classList.contains("collapsed")) {
            toggleDetailsButton.children[1].innerText = "View details";
        }
        else {
            toggleDetailsButton.children[1].innerText = "Hide details";
        }
    })

}

function insertWarningRows(rows) {
    tableRefBody.innerHTML = '';

    for (let value of rows) {
        tableRef.tBodies[0].appendChild(value);
    }

    setTimeout(function () {
        if (document.querySelector(".rnw-allwarnings-table .accordion-button:not(.collapsed)")) {
            document.querySelector(".rnw-allwarnings-table .accordion-button:not(.collapsed)").click();
        }
    }, 100);
    initEvents();
}

function getFilteredWarningRows(warningType) {
    return allwarningrows.filter(
        function (warning) {
            return warningType ? warning.classList.contains(warningType) : warning;
        }
    );
}

function initEvents() {
    const checkbox_warnings = Array.from(document.querySelectorAll(".checkbox_warning"));

    checkbox_warnings.map(function (checkbox_warning) {
        checkbox_warning.addEventListener("change", function (event) {
            const id = event.target.getAttribute("warning-id");
            if (selectedIds.indexOf(id) === -1) {
                selectedIds.push(id);
                event.target.getAttribute("warning-id")
            }
            else {
                selectedIds.splice(id, 1);
            }

            document.getElementById('showSelectionId').value = selectedIds;
        })

    })
}

select_button.addEventListener("click", function (event) {
    if (event.target.value === "Clear all") {
        selectedIds = Array.from(document.querySelectorAll(".checkbox_warning")).map(function (element) {
            return element.getAttribute("warning-id");
        });
    }
    else {
        selectedIds = [];
    }
    document.getElementById('showSelectionId').value = selectedIds;
})

initEvents();
