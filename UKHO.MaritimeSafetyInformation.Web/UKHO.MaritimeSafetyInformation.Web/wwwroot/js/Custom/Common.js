document.addEventListener("DOMContentLoaded", function () {
    HashLinkTab();
});

function HashLinkTab() {
    let url = location.href.replace(/\/$/, "");

    if (location.hash) {
        const hash = url.split("#");
        const hashValue = hash[1].toLowerCase();

        let tabs = Array.from(document.querySelectorAll('.msi-tabs .nav-link'));
        //let tabContents = Array.from(document.querySelectorAll('.tab-content .tab-pane'));

        for (let i = 0; i < tabs.length; i++) {
            tabs[i].classList.remove('active');
        }

        //for (let i = 0; i < tabContents.length; i++) {
        //    tabContents[i].classList.remove('active');
        //    tabContents[i].classList.remove('show');
        //}

        document.querySelector('.msi-tabs a[href="#' + hashValue + '"]').classList.add('active');
        //document.querySelector('.tab-content #' + hashValue + '').classList.add('active');
        //document.querySelector('.tab-content #' + hashValue + '').classList.add('show');

        SetTitle(hashValue);

        url = location.href.replace(/\/#/, "#");
        history.replaceState(null, null, url);
        setTimeout(() => {
            window.scrollTo(0, 0)
        }, 100);
    }

    Array.prototype.slice.call(document.querySelectorAll('a[data-bs-toggle="tab"]'))
        .forEach(function (element) {
            element.onclick = function () {
                let newUrl;
                const hash = this.getAttribute('href');
                if (hash == "#") {
                    newUrl = url.split("#")[0];

                } else {
                    newUrl = url.split("#")[0] + hash;
                    SetTitle(hash);
                }
                history.replaceState(null, null, newUrl);
            }
        });
}

function SetTitle(hash) {
    let newhash = hash.replace('#', '');
    if (newhash) {
        switch (newhash.toLowerCase()) {
            case 'cumulative': document.title = 'Notices to Mariners - Cumulative';
                break;
            case 'annual': document.title = 'Notices to Mariners - Annual';
                break;
            case 'allwarnings': document.title = 'Radio Navigational Warnings';
                break;
            case 'navarea1': document.title = 'Radio Navigational Warnings - NAVAREA I';
                document.querySelector('#NAVAREA1-tab').click();
                break;
            case 'ukcoastal': document.title = 'Radio Navigational Warnings - UK Coastal';
                document.querySelector('#ukcoastal-tab').click();
                break;

        }
    }
}
