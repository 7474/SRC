window.bulma = {
    blur: function (elementId) {
        const elem = document.getElementById(elementId) || document.activeElement;
        if (elem) {
            elem.blur();
        }
    }
}