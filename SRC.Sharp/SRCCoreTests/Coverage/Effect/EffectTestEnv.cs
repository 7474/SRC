using SRCCore.Config;
using SRCCore.Filesystem;
using SRCCore.TestLib;
using SRCCore.Units;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SRCCore.Coverage.Tests.Effects
{
    /// <summary>
    /// Effect のテスト用の環境。
    /// ・効果音: 一時フォルダの Sound に空の wav を置き、HeadlessGUI が記録した IPlaySound の呼び出しから
    ///   再生したファイル名を取り出す。
    /// ・戦闘アニメ: 指定した「戦闘アニメ_xxx」ラベルをイベントに定義し、呼ばれたアニメ名と引数を変数「記録」に残す。
    ///   未定義のラベルが呼ばれた場合（ShowAnimation のエラーメッセージ）は「未定義:xxx」として同じ記録に残す。
    /// </summary>
    internal sealed class EffectTestEnv : IDisposable
    {
        // Effect.cs が参照する効果音ファイル
        private static readonly string[] Waves =
        {
            "AntiShipMissile.wav", "Bazooka.wav", "Beam.wav", "BeamCoat.wav", "BeamSaber.wav", "Bomb.wav", "Bow.wav",
            "BreakOff.wav", "Breath.wav", "Bubble.wav", "Cannon.wav", "Charge.wav", "Combo.wav", "Crash.wav", "Drill.wav",
            "Explode(Nuclear).wav", "Explode(Small).wav", "Explode.wav", "FastGun.wav", "Fire.wav", "Gun.wav", "GunPod.wav",
            "Hide.wav", "Inori.wav", "LaserGun.wav", "MachineCannon.wav", "MachineGun.wav", "MicroMissile.wav", "Missile.wav",
            "MultipleRocketLauncher(Light).wav", "Punch.wav", "Rifle.wav", "Saber.wav", "Shock(Low).wav", "Shower.wav",
            "Slap.wav", "Slash.wav", "Splash.wav", "Stab.wav", "Storm.wav", "Swing.wav", "Sword.wav", "Thunder.wav",
            "Whip.wav", "Whiz.wav",
        };

        public string Dir { get; }
        public SRC Src { get; }
        public HeadlessGUI Gui { get; }

        public EffectTestEnv(bool battleAnimation, IEnumerable<string> animations = null, int messageWait = 0, string[] options = null, string[] keepAnimations = null)
        {
            Dir = Path.Combine(Path.GetTempPath(), "srceffect-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Path.Combine(Dir, "Sound"));
            foreach (var wave in Waves)
            {
                File.WriteAllBytes(Path.Combine(Dir, "Sound", wave), new byte[0]);
            }

            var lines = new List<string> { "Exit" };
            foreach (var name in (animations ?? Enumerable.Empty<string>()).Distinct())
            {
                lines.Add("戦闘アニメ_" + name + ":");
                lines.Add("Set 記録 (記録 & \"[" + name + "|\" & Args(1) & \"|\" & Args(2) & \"|\" & Args(3) & \"|\" & Args(4) & \"]\")");
                // keepAnimations に含まれるアニメは「keep」を返す（画像を消去させない指定）
                lines.Add((keepAnimations ?? new string[0]).Contains(name) ? "Return keep" : "Return");
            }
            var eve = Path.Combine(Dir, "test.eve");
            File.WriteAllText(eve, string.Join("\r\n", lines), new UTF8Encoding(false));

            Src = new SRC
            {
                SystemConfig = new LocalFileConfig
                {
                    AppPath = Dir,
                    BattleAnimation = battleAnimation,
                    WeaponAnimation = true,
                    ExtendedAnimation = false,
                },
            };
            var fs = new LocalFileSystem();
            fs.AddPath(Dir);
            Src.FileSystem = fs;
            Gui = new HeadlessGUI();
            Gui.Install(Src);
            Src.ScenarioPath = Dir;
            Src.Event.InitEventData();
            Src.Event.LoadEventData(eve);
            // 汎用戦闘アニメの include.eve が無いと読み込み時に戦闘アニメがオフにされるので、読み込み後に設定する
            Src.SystemConfig.BattleAnimation = battleAnimation;
            Src.Expression.DefineGlobalVariable("記録");
            Src.Expression.SetVariableAsString("記録", "");
            foreach (var opt in options ?? new string[0])
            {
                // Option コマンドと同じくグローバル変数 Option(名前) で判定される
                Src.Expression.DefineGlobalVariable("Option(" + opt + ")");
            }
            Src.GUI.MessageWait = messageWait;

            // 未定義の戦闘アニメが呼ばれたことも記録する
            Gui.Handle("ErrorMessage", args =>
            {
                var msg = (string)args[0];
                var m = Regex.Match(msg, "戦闘アニメ_([^`」]+)");
                var name = m.Success ? m.Groups[1].Value : msg;
                Src.Expression.SetVariableAsString("記録", Src.Expression.GetValueAsString("記録") + "[未定義:" + name + "]");
                return null;
            });
        }

        /// <summary>
        /// 再生された効果音のファイル名（再生順）
        /// </summary>
        public List<string> PlayedWaves()
        {
            return Gui.CallsOf("ResolveSoundType").Select(x => Path.GetFileName((string)x.Args[0])).ToList();
        }

        /// <summary>
        /// GUI.Sleep の待ち時間（呼び出し順）
        /// </summary>
        public List<int> Sleeps()
        {
            return Gui.CallsOf("Sleep").Select(x => (int)x.Args[0]).ToList();
        }

        /// <summary>
        /// 表示された戦闘アニメ（「名前 引数1 引数2 ...」の形、呼び出し順）
        /// </summary>
        public List<string> Animations()
        {
            var rec = Src.Expression.GetValueAsString("記録");
            return rec.Split(new[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => string.Join(" ", x.Split('|').Where(y => y != "")))
                .ToList();
        }

        /// <summary>
        /// 武器を 1 つだけ持つユニットを作成して武器を返す。
        /// </summary>
        public UnitWeapon CreateWeapon(string wname, string wclass, string unitClass = "汎用", string transportation = "陸", int maxRange = 1, string nickname = null, string feature = null)
        {
            var ud = Src.UDList.Add("テスト機" + (Src.UDList.Count() + 1));
            ud.Nickname = nickname ?? ud.Name;
            if (feature != null)
            {
                ud.AddFeature(feature);
            }
            ud.HP = 1000;
            ud.EN = 100;
            ud.Class = unitClass;
            ud.Transportation = transportation;
            var wd = ud.AddWeapon(wname);
            wd.Class = wclass;
            wd.MinRange = 1;
            wd.MaxRange = maxRange;
            wd.Power = 1000;
            ud.PilotNum = 1;
            var pd = Src.PDList.Add(ud.Name + "のパイロット");
            pd.SP = 100;
            var p = Src.PList.Add(pd.Name, 1, "味方");
            var u = Src.UList.Add(ud.Name, 0, "味方");
            p.Ride(u);
            u.Update();
            return u.Weapons[0];
        }

        public void Dispose()
        {
            try
            {
                Directory.Delete(Dir, true);
            }
            catch
            {
                // ignore
            }
        }
    }
}
