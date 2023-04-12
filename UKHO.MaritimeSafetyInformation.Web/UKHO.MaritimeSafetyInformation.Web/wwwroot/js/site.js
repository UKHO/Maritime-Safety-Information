// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

//Accessibility code for tabs start here//
let tabButtons = Object.assign([], document.querySelectorAll('#rnwTab li .nav-link')); //object.assign is used to convert NodeList to an Array
let rnwTableCaption = document.querySelector('#rnwTableCaption');
for (let i = 0; i < tabButtons.length; i++) {
    tabButtons[i].onkeydown = function (e) {
        tabButtonKeyDownFunction(e);
    }
    tabButtons[i].addEventListener('click', function (e) {
        removeTabIndex(e);
    });
}

function removeTabIndex(e) {
    for (var i = 0; i < tabButtons.length; i++) {
        tabButtons[i].setAttribute("tabindex", '-1');
    }
    event.target.removeAttribute("tabindex");
}

function tabButtonKeyDownFunction(e) {
    var keycode = (event.keyCode ? event.keyCode : event.which);
    if (keycode === 13) {
        setTimeout(function () {
            if (document.body.contains(rnwTableCaption)) {
                rnwTableCaption.focus();
            }
        }, 100);
    }
    if (keycode === 39 || keycode === 37) { // Keycode "39" is for rightkey and "37" is for leftkey
        event.preventDefault();
    }
    if (keycode === 39) { // Keycode "39" is right key
        let nextButton = event.target.parentNode.nextElementSibling.children[0];
        nextButton.focus();
    } else if (keycode === 37) {// Keycode "37" is left key
        let prevButton = event.target.parentNode.previousElementSibling.children[0];
        prevButton.focus();
    }
    if (keycode === 39) { // Keycode "39" is right key
        let nextButton = event.target.parentNode.nextElementSibling.children[0];
        nextButton.focus();
    }
}
//Accessibility code for tabs ends here//

//site header dropdown menu on mouse hover start here//
let dropdownParent = document.querySelector('.site-header .dropdown');
let dropdowToggleButton = document.querySelector('.site-header .dropdown-toggle');
if (document.getElementById('hdnLoggedIn').value === "Y") {
    dropdownParent.addEventListener('mouseenter', function () {
        dropdowToggleButton.click();
    })
    dropdownParent.addEventListener('mouseleave', function () {
        dropdowToggleButton.click();
    })
}
//site header dropdown menu on mouse hover ends here//

function HashLinkTab() {
    let url = location.href.replace(/\/$/, "");

    if (location.hash) {
        const hash = url.split("#");
        const hashValue = hash[1].toLowerCase();
        if (SetHashTitle(hashValue)) {

            let tabs = Array.from(document.querySelectorAll('.msi-tabs .nav-link'));
            let tabContents = Array.from(document.querySelectorAll('.tab-content .tab-pane'));

            for (let i = 0; i < tabs.length; i++) {
                tabs[i].classList.remove('active');
            }

            if (hashValue != "navarea1" && hashValue != "ukcoastal") {
                for (let i = 0; i < tabContents.length; i++) {
                    tabContents[i].classList.remove('active');
                    tabContents[i].classList.remove('show');
                }
            }

            document.querySelector('.msi-tabs a[href="#' + hashValue + '"]').classList.add('active');
            if (hashValue != "navarea1" && hashValue != "ukcoastal") {
                document.querySelector('.tab-content #' + hashValue + '').classList.add('active');
                document.querySelector('.tab-content #' + hashValue + '').classList.add('show');
            }

            url = location.href.replace(/\/#/, "#");
            history.replaceState(null, null, url);
            setTimeout(() => {
                window.scrollTo(0, 0)
            }, 100);

        }
    }
    else {
        SetTitle();
    }

    Array.prototype.slice.call(document.querySelectorAll('a[data-bs-toggle="tab"]'))
        .forEach(function (element) {
            element.onclick = function () {
                let newUrl;
                const hash = this.getAttribute('href');
                if (hash == "#") {
                    newUrl = url.split("#")[0];

                } else {
                    if (hash.toLowerCase() === '#allwarnings') newUrl = url.split('#')[0];
                    else newUrl = url.split("#")[0] + hash;
                    SetHashTitle(hash);
                }
                history.replaceState(null, null, newUrl);
            }
        });
}

function SetHashTitle(hash) {
    let newhash = hash.replace('#', '');
    let bool = true;
    if (newhash) {
        switch (newhash.toLowerCase()) {
            case 'allwarnings': document.title = 'Radio Navigation Warnings';
                break;
            case 'navarea1': document.title = 'Radio Navigation Warnings - NAVAREA I';
                document.querySelector('#NAVAREA1-tab').click();
                break;
            case 'ukcoastal': document.title = 'Radio Navigation Warnings - UK Coastal';
                document.querySelector('#ukcoastal-tab').click();
                break;
            default: bool = false;
                break;
        }
    }
    else { bool = false; }
    return bool;
}

function SetTitle() {
    let pathName = location.pathname;

    if (pathName) {
        switch (pathName.toLowerCase()) {
            case '/noticestomariners/weekly': document.title = 'Notices to Mariners - Weekly';
                break;
            case '/noticestomariners/daily': document.title = 'Notices to Mariners - Daily';
                break;
            case '/noticestomariners/cumulative': document.title = 'Notices to Mariners - Cumulative';
                break;
            case '/noticestomariners/annual': document.title = 'Notices to Mariners - Annual';
                break;
            case '/noticestomariners/resellers': document.title = 'Notices to Mariners - Value Added Resellers';
                break;
            case '/noticestomariners/about': document.title = 'About Notices to Mariners';
                break;
            case '/radionavigationalwarnings/about': document.title = 'About Radio Navigation Warnings';
                break;
            case '/radionavigationalwarnings/showselection': document.title = 'Radio Navigation Warnings';
                break;
            case '/radionavigationalwarnings': document.title = 'Radio Navigation Warnings';
                break;

        }
    }

}
