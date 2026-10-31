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

    // 表示の言語（ja・en・auto）。画面の文言は .NET 側（Resources/UIStrings*.resx）で切り替える。
    // ここでは html の lang と、.NET が動く前に出る静的な文言だけを合わせる
    try {
        var language = 'ja';
        var savedLanguage = saved ? JSON.parse(saved).Language : null;
        if (savedLanguage === 'en' || (savedLanguage === 'auto' && /^en\b/i.test(navigator.language || ''))) {
            language = 'en';
        }
        document.documentElement.setAttribute('lang', language);
        if (language === 'en') {
            var errorUi = document.getElementById('blazor-error-ui');
            if (errorUi) {
                errorUi.innerHTML =
                    'An error has occurred. ' +
                    'Please try to <a href="" class="reload">reload</a>.<br />' +
                    'If reloading does not help, please report the error (for example the console log of the <a href="https://support.google.com/displayvideo/answer/6188066">developer tools</a>) to the <a href="https://github.com/7474/SRC">repository</a>.' +
                    ' <a class="dismiss">&#x1F5D9;</a>';
            }
        }
    } catch (e) {
        // ignore
    }
}());
