using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using SRCPlay;

namespace SRCRender.Tests
{
    /// <summary>
    /// 拡張データフォルダ（srcs.json の ExtDataPath）だけに SRC 本体の素材がある場合。
    /// SRC# を SRC 本体と別のフォルダに置き、拡張データフォルダで SRC 本体を指す使い方。
    /// </summary>
    [TestClass]
    public class ExtDataPathTests
    {
        /// <summary>再生を頼まれた効果音の中身を記録する</summary>
        private sealed class RecordingPlayer : SRCRender.NullSoundPlayer, SRCCore.IPlaySound
        {
            public readonly System.Collections.Generic.List<string> Played = new System.Collections.Generic.List<string>();

            void SRCCore.IPlaySound.Play(int channel, Stream stream, SRCCore.SoundType soundType, SRCCore.PlaySoundMode mode)
            {
                using (var reader = new StreamReader(stream))
                {
                    Played.Add(reader.ReadToEnd());
                }
            }
        }

        [TestMethod]
        public void WaveSounds_AreFoundInExtDataPathAndAssetPacks()
        {
            using (var app = new ScenarioWorkspace(withAssets: true))
            using (var ext = new ScenarioWorkspace(withAssets: false))
            {
                // 効果音は拡張データフォルダの Sound と、素材パック（Packs の zip）の Sound にだけある
                ext.Write(Path.Combine("Sound", "Beam.wav"), "ext-beam");
                Directory.CreateDirectory(Path.Combine(app.AppPath, "Packs"));
                using (var zip = System.IO.Compression.ZipFile.Open(Path.Combine(app.AppPath, "Packs", "sound.zip"), System.IO.Compression.ZipArchiveMode.Create))
                using (var writer = new StreamWriter(zip.CreateEntry("効果音集/Sound/Sword.wav").Open()))
                {
                    writer.Write("pack-sword");
                }
                app.Write("srcs.json", "{ \"ExtDataPath\": " + Newtonsoft.Json.JsonConvert.SerializeObject(ext.AppPath) + " }");
                app.WriteMap("test.map", 5, 5, (x, y) => 0);
                app.Write("test.eve", "スタート:", "ChangeMap test.map", "Talk システム", "待機", "End", "Quit", "");

                var player = new RecordingPlayer();
                using (var runner = new ScreenScenarioRunner(app.AppPath, soundPlayer: player))
                {
                    runner.Start(Path.Combine(app.AppPath, "test.eve"));
                    runner.WaitForMessage();
                    var src = runner.Session.SRC;
                    src.Sound.PlayWave("Beam.wav");
                    src.Sound.PlayWave("Sword.wav");
                    src.Sound.PlayWave("Missing.wav");
                    CollectionAssert.AreEqual(new[] { "ext-beam", "pack-sword" }, player.Played);
                }
            }
        }

        [TestMethod]
        public void MidiFiles_AreFoundInSubFoldersOfExtDataPathAndAssetPacks()
        {
            using (var app = new ScenarioWorkspace(withAssets: true))
            using (var ext = new ScenarioWorkspace(withAssets: false))
            {
                // 「作品名_曲名.mid」は Midi\作品名\ から探す（7474/SRC#1480）。素材パック（Packs の zip）の中も探す
                ext.Write(Path.Combine("Midi", "ext", "ext_battle.mid"), "ext-battle");
                Directory.CreateDirectory(Path.Combine(app.AppPath, "Packs"));
                using (var zip = System.IO.Compression.ZipFile.Open(Path.Combine(app.AppPath, "Packs", "midi.zip"), System.IO.Compression.ZipArchiveMode.Create))
                using (var writer = new StreamWriter(zip.CreateEntry("曲集/Midi/pack/pack_map.mp3").Open()))
                {
                    writer.Write("pack-map");
                }
                app.Write("srcs.json", "{ \"ExtDataPath\": " + Newtonsoft.Json.JsonConvert.SerializeObject(ext.AppPath) + " }");
                app.WriteMap("test.map", 5, 5, (x, y) => 0);
                app.Write("test.eve", "スタート:", "ChangeMap test.map", "Talk システム", "待機", "End", "Quit", "");

                using (var runner = new ScreenScenarioRunner(app.AppPath))
                {
                    runner.Start(Path.Combine(app.AppPath, "test.eve"));
                    runner.WaitForMessage();
                    var src = runner.Session.SRC;
                    StringAssert.EndsWith(src.Sound.SearchMidiFile("ext_battle.mid").Replace('\\', '/'), "Midi/ext/ext_battle.mid");
                    StringAssert.EndsWith(src.Sound.SearchMidiFile("pack_map.mid").Replace('\\', '/'), "Midi/pack/pack_map.mp3");
                    Assert.IsNull(src.Sound.SearchMidiFile("none_map.mid"));
                }
            }
        }

        [TestMethod]
        public void SystemDataAndAssets_AreReadFromExtDataPath()
        {
            using (var app = new ScenarioWorkspace(withAssets: false))
            using (var ext = new ScenarioWorkspace(withAssets: true))
            {
                // 地形データ（Data\System\terrain.txt）・マップの画像・BGM は拡張データフォルダにだけある
                ext.Write(Path.Combine("Midi", "Map1.mid"), "x");
                app.Write("srcs.json", "{ \"ExtDataPath\": " + Newtonsoft.Json.JsonConvert.SerializeObject(ext.AppPath) + " }");
                Directory.CreateDirectory(Path.Combine(app.AppPath, "scenario"));
                app.WriteMap(Path.Combine("scenario", "test.map"), 5, 5, (x, y) => 0);
                app.Write(Path.Combine("scenario", "test.eve"), "スタート:", "ChangeMap test.map", "Talk システム", "待機", "End", "Quit", "");

                using (var runner = new ScreenScenarioRunner(app.AppPath))
                {
                    runner.Start(Path.Combine(app.AppPath, "scenario", "test.eve"));
                    runner.WaitForMessage();
                    var src = runner.Session.SRC;

                    Assert.AreEqual(ext.AppPath, src.ExtDataPath);
                    // 仮の地形（地形0・画像なし）ではなく、拡張データフォルダの地形データを使う
                    Assert.AreEqual("平地", src.TDList.Item(0).Name);
                    Assert.AreEqual("plain", src.TDList.Item(0).Bitmap);
                    StringAssert.EndsWith(src.Map.SearchTerrainImageFile(src.Map.MapData[1, 1]).Replace('\\', '/'), "Bitmap/Map/plain0000.bmp");
                    StringAssert.EndsWith(src.Sound.SearchMidiFile("Map1.mid").Replace('\\', '/'), "Midi/Map1.mid");
                }
            }
        }
    }
}
