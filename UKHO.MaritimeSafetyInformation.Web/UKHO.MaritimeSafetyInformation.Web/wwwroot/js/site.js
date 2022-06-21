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

let allWarningTabButton = document.querySelector('#allwarnings-tab');
let navarea1TabButton = document.querySelector('#NAVAREA1-tab');
let ukcostalTabButton = document.querySelector('#ukcoastal-tab');
let rnwTabContent = document.querySelector('#rnwTabContent');

allWarningTabButton.addEventListener('click', function () {
    const rows = getFilteredWarningRows();
    insertWarningRows(rows)
})
navarea1TabButton.addEventListener('click', function () {
    const rows = getFilteredWarningRows("N");
    insertWarningRows(rows)
})
ukcostalTabButton.addEventListener('click', function () {
    const rows = getFilteredWarningRows("U");
    insertWarningRows(rows)
})

function insertWarningRows(rows) {
    tableRefBody.innerHTML = '';
    for (var i = 0; i < rows.length; i++) {
        rows[i].classList.remove('show');
        rows[i].classList.remove('coll');
        ////if (rows[i] && rows[i].children[4].children[0].classList.contains("collapsed")) {
        ////    rows[i].children[4].children[0].click();
        ////}
        tableRef.tBodies[0].appendChild(rows[i]);
    }
}

function getFilteredWarningRows(warningType) {
    return allwarningrows.filter(
        function (warning) {
            return warningType ? warning.classList.contains(warningType) : warning;
        }
    );
}
