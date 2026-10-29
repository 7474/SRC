using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SRCTestBlazor.Models
{
    /// <summary>
    /// BGM の再生（画面を移っても鳴り続ける。MainLayout の BgmBar が鳴らし、止める）。
    /// データ（パイロットの BGM・ユニットの特殊能力「ＢＧＭ」）の名前を、開いているアーカイブのファイルから SRC と同じ規則で探す。
    /// MIDI は MidiRenderer で WAV に合成してから鳴らす（音タブの MIDI もここで鳴らす）
    /// </summary>
    public class BgmPlayer
    {
        private readonly HttpClient http;
        private readonly MidiRenderer renderer;
        private readonly IJSRuntime js;
        // 合成した WAV の URL（ファイルの URL ごと。同じ曲は合成し直さない）
        private readonly Dictionary<string, string> rendered = new Dictionary<string, string>();
        // 開いているアーカイブの、SRC のフォルダから見た道（小文字）→ ファイル。後から足したもの（優先するもの）で上書きする
        private Dictionary<string, ArchiveFile> files = new Dictionary<string, ArchiveFile>();
        private string soundFontUri;
        private int playId;

        public BgmPlayer(HttpClient http, MidiRenderer renderer, IJSRuntime js)
        {
            this.http = http;
            this.renderer = renderer;
            this.js = js;
        }

        /// <summary>鳴らしている（合成している）曲。無ければ null</summary>
        public ArchiveFile Current { get; private set; }
        /// <summary>鳴らす URL（合成が済むまで null）</summary>
        public string Url { get; private set; }
        /// <summary>合成の進み（0〜1）</summary>
        public double Progress { get; private set; }
        public string Error { get; private set; }

        public event Action OnChange;

        /// <summary>
        /// BGM を探すファイル（ArchivePage が開いたアーカイブと重ねるもの。後ろほど優先）と SoundFont を決める。
        /// 鳴らしている曲は止めない
        /// </summary>
        public void SetSource(IEnumerable<ArchiveFile> layeredFiles, string soundFont)
        {
            files = new Dictionary<string, ArchiveFile>();
            foreach (var file in layeredFiles.Where(x => x.Kind == ArchiveFileKind.Midi || x.Kind == ArchiveFileKind.Audio))
            {
                files[file.RelativePath.ToLowerInvariant()] = file;
            }
            soundFontUri = soundFont;
            OnChange?.Invoke();
        }

        public void ClearSource()
        {
            files = new Dictionary<string, ArchiveFile>();
            soundFontUri = null;
            OnChange?.Invoke();
        }

        /// <summary>
        /// BGM の指定（例: "battle.mid"、"(a.mid) b.mid"）から鳴らすファイルを探す（SRCCore の Sound.SearchMidiFile と同じ順）。
        /// 左の候補から順に、Midi フォルダ → SRC のフォルダの順に、同じ名前の .mp3 を .mid より先に探す。見つからなければ null
        /// </summary>
        public ArchiveFile Resolve(string bgm)
        {
            foreach (var name in Candidates(bgm))
            {
                var mp3 = name.Substring(0, name.Length - 4) + ".mp3";
                foreach (var path in new[] { "midi/" + mp3, "midi/" + name, mp3, name })
                {
                    if (files.TryGetValue(path.ToLowerInvariant(), out var file) && (file.Kind != ArchiveFileKind.Midi || soundFontUri != null))
                    {
                        return file;
                    }
                }
            }
            return null;
        }

        /// <summary>BGM の指定を候補の名前に分ける（空白を含む名前は .mid で終わるまでつなぐ。全体が () で囲まれていれば外す）</summary>
        public static IEnumerable<string> Candidates(string bgm)
        {
            if (string.IsNullOrWhiteSpace(bgm) || bgm.Trim().Length < 5)
            {
                yield break;
            }
            var buf = "";
            foreach (var raw in bgm.Split(new[] { ' ', '\t', '　' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var part = raw.StartsWith("(") && raw.EndsWith(")") ? raw.Substring(1, raw.Length - 2) : raw;
                buf = (buf + " " + part).Trim();
                if (buf.EndsWith(".mid", StringComparison.OrdinalIgnoreCase))
                {
                    yield return buf.Replace('\\', '/');
                    buf = "";
                }
            }
            if (buf.Length >= 5)
            {
                yield return buf.Replace('\\', '/');
            }
        }

        public bool IsPlaying(ArchiveFile file) => Current != null && file != null && Current.Url == file.Url;

        /// <summary>鳴らす。MIDI は合成してから（同じ曲は合成し直さない）</summary>
        public async Task PlayAsync(ArchiveFile file)
        {
            var id = ++playId;
            Current = file;
            Url = null;
            Error = null;
            Progress = 0;
            OnChange?.Invoke();
            try
            {
                string url;
                if (file.Kind != ArchiveFileKind.Midi)
                {
                    url = file.Url;
                }
                else if (!rendered.TryGetValue(file.Url, out url))
                {
                    var lastShown = 0.0;
                    var wav = await renderer.RenderAsync(http, file.Url, soundFontUri, p =>
                    {
                        Progress = p;
                        if (id == playId && p - lastShown >= 0.05)
                        {
                            lastShown = p;
                            OnChange?.Invoke();
                        }
                    });
                    url = await js.InvokeAsync<string>("srcArchive.audioUrl", wav);
                    rendered[file.Url] = url;
                }
                if (id == playId)
                {
                    Url = url;
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error playing BGM [{file.Path}]: {ex}");
                if (id == playId)
                {
                    Error = ex.Message;
                }
            }
            if (id == playId)
            {
                OnChange?.Invoke();
            }
        }

        public void Stop()
        {
            playId++;
            Current = null;
            Url = null;
            Error = null;
            OnChange?.Invoke();
        }
    }
}
