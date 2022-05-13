// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

//Accessibility code for tabs start here//
let tabButtons = Object.assign([], document.querySelectorAll('.msi-tabs li button')); //object.assign is used to convert NodeList to an Array
for (let i = 0; i < tabButtons.length; i++) {
    tabButtons[i].onclick = function (e) {
        removeTabIndex(e);
    }
    tabButtons[i].onkeydown = function (e) {
        tabButtonKeyDownFunction(e)
    }
}

function removeTabIndex(e) {
    for (var i = 0; i < tabButtons.length; i++) {
        tabButtons[i].setAttribute("tabindex", '-1');
    }
    event.target.removeAttribute("tabindex");
}

function tabButtonKeyDownFunction(e) {
    var keycode = (event.keyCode ? event.keyCode : event.which);
    if (keycode === 13 || keycode === 32) { // Keycode "13" is for Enter and "32" is for Space bar
        removeTabIndex();
    }
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




