////document.addEventListener("DOMContentLoaded", function () {
////    HashLinkTab();
////});

document.onreadystatechange = function () {
    if (document.readyState == "interactive" || document.readyState === "complete") {
        HashLinkTab();
    }
}

function HashLinkTab() {
    let url = location.href.replace(/\/$/, "");

    if (location.hash) {
        const hash = url.split("#");

        let tabs = Array.from(document.querySelectorAll('.msi-tabs .nav-link'));

        let tabContents = Array.from(document.querySelectorAll('.tab-content .tab-pane'));

        for (let i = 0; i < tabs.length; i++) {
            tabs[i].classList.remove('active');
            tabContents[i].classList.remove('active');
            tabContents[i].classList.remove('show');
        }

        document.querySelector('.msi-tabs a[href="#' + hash[1] + '"]').classList.add('active');

        document.querySelector('.tab-content #' + hash[1] + '').classList.add('active');
        document.querySelector('.tab-content #' + hash[1] + '').classList.add('show');

        //console.log($('.msi-tabs a[href="#' + hash[1] + '"]'));
        //$('.msi-tabs a[href="#' + hash[1] + '"]').tab("show");

        url = location.href.replace(/\/#/, "#");
        history.replaceState(null, null, url);
        setTimeout(() => {
            $(window).scrollTop(0);
        }, 400);
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
                }
                history.replaceState(null, null, newUrl);
            }
        });
}
