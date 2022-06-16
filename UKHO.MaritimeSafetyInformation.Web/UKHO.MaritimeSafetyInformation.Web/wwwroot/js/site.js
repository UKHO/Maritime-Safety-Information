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

let allWarningTabButton = document.querySelector('#allwarnings-tab');
let navarea1TabButton = document.querySelector('#NAVAREA1-tab');
let ukcostalTabButton = document.querySelector('#ukcoastal-tab');
let rnwTabContent = document.querySelector('#rnwTabContent');
allWarningTabButton.addEventListener('click', function () {
    rnwTabContent.classList.add('all-warnings');
    rnwTabContent.classList.remove('navarea1', 'ukcostal');
})
navarea1TabButton.addEventListener('click', function () {
    rnwTabContent.classList.add('navarea1');
    rnwTabContent.classList.remove('all-warnings', 'ukcostal');
})
ukcostalTabButton.addEventListener('click', function () {
    rnwTabContent.classList.add('ukcostal');
    rnwTabContent.classList.remove('all-warnings', 'navarea1');
})



