window.anchor = {
    scrollTo: function (elementId) {
        const elem = document.getElementById(elementId);
        if (elem) {
            elem.scrollIntoView();
        }
    }
};

window.srcClipboard = {
    copyText: async function (text) {
        try {
            if (navigator.clipboard && window.isSecureContext) {
                await navigator.clipboard.writeText(text);
                return true;
            }
        } catch {
            // クリップボード API が拒否された場合は下のフォールバックを使う
        }
        try {
            const textarea = document.createElement('textarea');
            textarea.value = text;
            textarea.style.position = 'fixed';
            textarea.style.left = '-9999px';
            textarea.style.top = '0';
            document.body.appendChild(textarea);
            textarea.focus();
            textarea.select();
            const ok = document.execCommand('copy');
            document.body.removeChild(textarea);
            return ok;
        } catch {
            return false;
        }
    },
    openUri: function (url) {
        window.location.href = url;
    }
};