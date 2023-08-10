document.onreadystatechange = function () {

    if (document.readyState == "interactive" || document.readyState === "complete") {
        HashLinkTab();
    }
}

window.onload = function () {
    var checkboxes = document.getElementsByName('checkbox');

    for (let i = 0; i < checkboxes.length; i++) {
        checkboxes[i].addEventListener('change', enableDisableShowSelection);
    }

    document.getElementById('BtnShowSelection').addEventListener('click', showSelection);
}

function do_Selection() {
    var checkboxes = document.getElementsByName('checkbox');
    var button = document.getElementById('select_button');

    if (button.value == 'Select all') {
        for (var i in checkboxes) {
            // Note - 'FALSE' means that the box will be ticked.
            checkboxes[i].checked = 'FALSE';
        }
        document.getElementById("BtnShowSelection").disabled = false;
        button.value = 'Clear all'
    } else {
        for (var i in checkboxes) {
            // The means that the box won't be ticked.
            checkboxes[i].checked = '';
        }
        document.getElementById("BtnShowSelection").disabled = true;
        button.value = 'Select all';
    }
}

function showSelection() {
    const checkbox_warnings = Array.from(document.querySelectorAll(".checkbox_warning"));

    checkbox_warnings.map(function (checkbox_warning) {
        checkbox_warning.checked = false;
    })
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
let tabPane = document.querySelector('#rnwTabContent .tab-pane');

allWarningTabButton.addEventListener('click', function () {
    tabPane.id = "allwarnings";
    tabPane.setAttribute("aria-labelledby", "allwarnings-tab");
    rnwTableCaption.innerText = "All Warnings";
    for (let i = 0; i < tabButtons.length; i++) {
        tabButtons[i].setAttribute("aria-controls", "allwarnings");
    }
    selected_tab = undefined;
    const rows = getFilteredWarningRows();
    insertWarningRows(rows)
})
navarea1TabButton.addEventListener('click', function () {
    tabPane.id = "navarea1";
    tabPane.setAttribute("aria-labelledby", "NAVAREA1-tab");
    rnwTableCaption.innerText = "Navarea 1";
    for (let i = 0; i < tabButtons.length; i++) {
        tabButtons[i].setAttribute("aria-controls", "navarea1");
    }
    selected_tab = "N";
    const rows = getFilteredWarningRows("N");
    insertWarningRows(rows)
})
ukcostalTabButton.addEventListener('click', function () {
    tabPane.id = "ukcoastal";
    tabPane.setAttribute("aria-labelledby", "ukcoastal-tab");
    rnwTableCaption.innerText = "UK Coastal";
    for (let i = 0; i < tabButtons.length; i++) {
        tabButtons[i].setAttribute("aria-controls", "ukcoastal");
    }
    selected_tab = "U";
    const rows = getFilteredWarningRows("U");
    insertWarningRows(rows)
})

toggleDetailsButtons.map(function (toggleDetailsButton, index) {

    toggleDetailsButton.addEventListener('click', function (event) {
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
    if (rows.length > 0) {
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
    else {
        tableRefBody.innerHTML = 'No Record Found.';
    }
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
            if (this.checked) {
                if (!selectedIds.includes(id)) {
                    selectedIds.push(id);
                }
            }
            else {
                selectedIds = selectedIds.filter(e => e !== id);
            }

            document.getElementById('showSelectionId').value = selectedIds;
        })

    })
}

select_button.addEventListener("click", function (event) {
    do_Selection();

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
