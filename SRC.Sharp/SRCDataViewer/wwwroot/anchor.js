window.anchor = {
    scrollTo: function (elementId) {
        const elem = document.getElementById(elementId);
        if (elem) {
            elem.scrollIntoView();
        }
    }
}