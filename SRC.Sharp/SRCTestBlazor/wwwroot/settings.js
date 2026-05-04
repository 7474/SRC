window.dataViewerSettings = {
    applyTheme: function (theme) {
        const html = document.documentElement;
        if (theme === 'Dark') {
            html.setAttribute('data-theme', 'dark');
        } else if (theme === 'Light') {
            html.setAttribute('data-theme', 'light');
        } else {
            html.removeAttribute('data-theme');
        }
    }
};
