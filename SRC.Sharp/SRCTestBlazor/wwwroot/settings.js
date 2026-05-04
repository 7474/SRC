(function () {
    window.dataViewerSettings = {
        applyTheme: function (theme) {
            var html = document.documentElement;
            if (theme === 'Dark') {
                html.setAttribute('data-theme', 'dark');
            } else if (theme === 'Light') {
                html.setAttribute('data-theme', 'light');
            } else {
                html.removeAttribute('data-theme');
            }
        }
    };

    // Apply saved theme immediately on load to avoid flash
    try {
        var saved = localStorage.getItem('dataViewerSettings');
        if (saved) {
            var settings = JSON.parse(saved);
            var themeMode = settings.ThemeMode;
            var html = document.documentElement;
            if (themeMode === 2) { // Dark
                html.setAttribute('data-theme', 'dark');
            } else if (themeMode === 1) { // Light
                html.setAttribute('data-theme', 'light');
            }
        }
    } catch (e) {
        // ignore
    }
}());
