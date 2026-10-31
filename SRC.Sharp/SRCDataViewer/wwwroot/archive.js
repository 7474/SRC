// アーカイブの頁（ArchivePage）から使う。合成した WAV をブラウザで鳴らせる URL にする
(function () {
    window.srcArchive = {
        audioUrl: function (bytes) {
            return URL.createObjectURL(new Blob([bytes], { type: 'audio/wav' }));
        },
        revoke: function (url) {
            URL.revokeObjectURL(url);
        }
    };
}());
