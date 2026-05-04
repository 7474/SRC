(function () {
    function applyDarkModeClass(html) {
        var theme = html.getAttribute('data-theme');
        var systemDark = window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches;
        var isDark = theme === 'dark' || (theme !== 'light' && systemDark);
        if (isDark) {
            html.classList.add('src-dark-active');
        } else {
            html.classList.remove('src-dark-active');
        }
    }

    // Listen for system preference changes
    if (window.matchMedia) {
        window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', function () {
            applyDarkModeClass(document.documentElement);
        });
    }

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
            applyDarkModeClass(html);
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
            applyDarkModeClass(html);
        } else {
            applyDarkModeClass(document.documentElement);
        }
    } catch (e) {
        applyDarkModeClass(document.documentElement);
    }
}());
