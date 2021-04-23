using SRCCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRCCore.Units
{
    // === スペシャルパワー関連処理 ===
    public partial class Unit
    {
        // 影響下にあるスペシャルパワー一覧
        public string SpecialPowerInEffect()
        {
            string SpecialPowerInEffectRet = "";
            foreach (Condition cnd in colSpecialPowerInEffect.List)
            {
                var spd = SRC.SPDList.Item(cnd.Name);
                if (spd.ShortName == "非表示")
                {
                    // イベント専用
                    continue;
                }

                if (spd.Duration == "みがわり")
                {
                    // みがわりは別表示
                    continue;
                }

                SpecialPowerInEffectRet = SpecialPowerInEffectRet + spd.ShortName;
            }

            // みがわりはかばってくれるユニットを表示する
            foreach (Condition cnd in colSpecialPowerInEffect)
            {
                var spd = SRC.SPDList.Item(cnd.Name);
                if (spd.Duration == "みがわり")
                {
                    if (SRC.PList.IsDefined(cnd.StrData))
                    {
                        SpecialPowerInEffectRet = SpecialPowerInEffectRet + spd.ShortName + "(" + SRC.PList.Item(cnd.StrData).get_Nickname(false) + ")";
                    }

                    return SpecialPowerInEffectRet;
                }
            }

            return SpecialPowerInEffectRet;
        }

        // ユニットがスペシャルパワー sname の影響下にあるかどうか
        public bool IsSpecialPowerInEffect(string sname)
        {
            return colSpecialPowerInEffect.List.Any(x => x.Name == sname);
        }

        // ユニットがスペシャルパワー効果 sptype の影響下にあるかどうか
        public bool IsUnderSpecialPowerEffect(string sptype)
        {
            foreach (Condition cnd in colSpecialPowerInEffect.List)
            {
                var spd = SRC.SPDList.Item(cnd.Name);
                if (spd.Effects.Any(x => x.strEffectType == sptype))
                {
                    return true;
                }
            }
            return false;
        }

        // 影響下にあるスペシャルパワーの総数
        public int CountSpecialPower()
        {
            return colSpecialPowerInEffect.Count;
        }

        // 影響下にあるスペシャルパワー
        public SpecialPowerData SpecialPower(int Index)
        {
            try
            {
                var cnd = colSpecialPowerInEffect[Index];
                return SRC.SPDList.Item(cnd.Name);
            }
            catch
            {
                return null;
            }
        }

        // スペシャルパワー mname の効果レベル
        public double SpecialPowerEffectLevel(string sname)
        {
            double SpecialPowerEffectLevelRet = 0d;
            double lv = Constants.DEFAULT_LEVEL;
            foreach (Condition cnd in colSpecialPowerInEffect)
            {
                var spd = SRC.SPDList.Item(cnd.Name);
                foreach (var effect in spd.Effects)
                {
                    if (effect.strEffectType == sname && effect.dblEffectLevel > lv)
                    {
                        lv = effect.dblEffectLevel;
                    }
                }
            }

            if (lv != Constants.DEFAULT_LEVEL)
            {
                SpecialPowerEffectLevelRet = lv;
            }

            return SpecialPowerEffectLevelRet;
        }

        // スペシャルパワーのデータ
        public string SpecialPowerData(int Index)
        {
            try
            {
                return colSpecialPowerInEffect[Index].StrData;
            }
            catch
            {
                return "";
            }
        }

        // スペシャルパワー sname の効果を適用
        public void MakeSpecialPowerInEffect(string sname, string sdata = "")
        {
            var cnd = new Condition();

            // すでに使用されていればなにもしない
            if (IsSpecialPowerInEffect(sname))
            {
                return;
            }

            cnd.Name = sname;
            cnd.StrData = sdata;
            colSpecialPowerInEffect.Add(cnd, sname);
        }

        // 持続時間が stype であるスペシャルパワーの効果を発動後、取り除く
        public void RemoveSpecialPowerInEffect(string stype)
        {
            // メッセージウィンドウが表示されているか記録
            var is_message_form_visible = GUI.MessageFormVisible;
            var i = 1;
            while (i <= CurrentForm().CountSpecialPower())
            {
                var sd = SpecialPower(i);

                // スペシャルパワーの持続期間が指定したものと一致しているかチェック
                if ((stype ?? "") != (sd.Duration ?? ""))
                {
                    i = (i + 1);
                    continue;
                }

                // 持続期間が敵ターンの場合、スペシャルパワーをかけてきた敵のフェイズ
                // が来るまで効果を削除しない
                if (stype == "敵ターン")
                {
                    string spd = SpecialPowerData(i);
                    if (SRC.PList.IsDefined(spd))
                    {
                        var p = SRC.PList.Item(spd);
                        if (p.Unit != null)
                        {
                            if ((p.Unit.CurrentForm().Party ?? "") != (SRC.Stage ?? ""))
                            {
                                i = (i + 1);
                                continue;
                            }
                        }
                    }
                }

                // 消去するスペシャルパワーの効果を発動
                if (CurrentForm().Status == "出撃")
                {
                    sd.Apply(CurrentForm().MainPilot(), CurrentForm(), false, true);
                }

                // スペシャルパワーの効果を削除
                CurrentForm().RemoveSpecialPowerInEffect2(i);
            }

            // メッセージウィンドウが元から表示されていなければ閉じておく
            if (!is_message_form_visible && GUI.MessageFormVisible)
            {
                GUI.CloseMessageForm();
            }
        }

        // スペシャルパワー sname の効果を取り除く
        public void RemoveSpecialPowerInEffect2(int Index)
        {
            colSpecialPowerInEffect.Remove(colSpecialPowerInEffect[Index]);
        }

        // 全てのスペシャルパワーの効果を取り除く
        public void RemoveAllSpecialPowerInEffect()
        {
            colSpecialPowerInEffect.Clear();
        }

        // スペシャルパワーの効果をユニット u にコピーする
        public void CopySpecialPowerInEffect(Unit u)
        {
            foreach (Condition cnd in colSpecialPowerInEffect.List)
            {
                u.MakeSpecialPowerInEffect(cnd.Name, cnd.StrData);
            }
        }
    }
}
