// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

//Accessibility code for tabs start here//
let tabButtons = Object.assign([], document.querySelectorAll('.msi-tabs li .nav-link')); //object.assign is used to convert NodeList to an Array
for (let i = 0; i < tabButtons.length; i++) {
    tabButtons[i].onkeydown = function (e) {
        tabButtonKeyDownFunction(e)
    }
}

function tabButtonKeyDownFunction(e) {
    var keycode = (event.keyCode ? event.keyCode : event.which);
    if (keycode === 40 || keycode === 38) { // Keycode "40" is for downkey and "38" is for upkey
        event.preventDefault();
    }
    if (keycode === 40) { // Keycode "40" is down key
        let nextButton = event.target.parentNode.nextElementSibling.children[0];
        nextButton.focus();
    } else if (keycode === 38) {// Keycode "38" is up key
        let prevButton = event.target.parentNode.previousElementSibling.children[0];
        prevButton.focus();
    }
}
//Accessibility code for tabs ends here//

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
let toggleDetailsButtons = Array.from(document.querySelectorAll('.view_details'));


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
        if (event.target.classList.contains("collapsed")) {
            event.target.innerHTML = "View details"
        }
        else {
            event.target.innerHTML = "Hide details"
        }
    })
 })

function insertWarningRows(rows) {
    tableRefBody.innerHTML = '';
    
    for (var i = 0; i < rows.length; i++) {
        //rows[i].classList.remove('show');
        //rows[i].classList.remove('coll');
        tableRef.tBodies[0].appendChild(rows[i]);
    }
    setTimeout(function () {
        if (document.querySelector(".view_details:not(.collapsed)")) {
            console.log(document.querySelector(".view_details:not(.collapsed)"));
            document.querySelector(".view_details:not(.collapsed)").click();
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
            console.log(event.target.getAttribute("warning-id"));

            document.getElementById('showSelectionId').value = selectedIds;
        })

    })
}

select_button.addEventListener("click", function (event) {
    console.log(event.target.value)
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
