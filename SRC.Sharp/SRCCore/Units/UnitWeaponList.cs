using SRCCore.Lib;
using SRCCore.VB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Units
{
    public class UnitWeaponListItem
    {
        public UnitWeapon Weapon { get; set; }
        public bool CanUse { get; set; }
    }

    public class UnitWeaponList
    {
        public UnitWeaponList(WeaponListMode mode, Unit unit, Unit targetUnit = null)
        {
            Mode = mode;
            Unit = unit;
            TargetUnit = targetUnit;
            Items = unit.Weapons.Where(x => x.IsDisplayFor(Mode))
                .Select(x => new UnitWeaponListItem
                {
                    Weapon = x,
                    CanUse = x.CanUseFor(mode, targetUnit),
                })
                .ToList();
        }

        public WeaponListMode Mode { get; set; }
        public IList<UnitWeaponListItem> Items { get; set; }

        // 武器を持つユニット
        public Unit Unit { get; }
        // 反撃時の相手のユニット（反撃の武器選択で命中率などを表示する相手）
        public Unit TargetUnit { get; }

        // 武器選択用リストボックスの見出し（GUI.bas の WeaponListBox）
        public string ListBoxInfo(SRC src)
        {
            var expression = src.Expression;
            if (Mode == WeaponListMode.Counter)
            {
                return "名称                         攻撃 命中 " + expression.Term("CT", Unit, 2) + "   弾  "
                    + expression.Term("EN", Unit, 2) + " 適応 分類";
            }
            return "名称                       攻撃 射程  命 弾  " + expression.Term("EN", Unit, 2) + "  "
                + expression.Term("CT", Unit, 2) + " 適応 分類";
        }

        // 武器選択用リストボックスの各武器の表示内容（GUI.bas の WeaponListBox）
        // 元の SRC と同じく攻撃力の順に並べる。ListItemObject が選んだ武器。
        public IList<ListBoxItem<UnitWeapon>> ToListBoxItems(SRC src)
        {
            var displayed = Items.ToDictionary(x => x.Weapon);
            return SortByPower(Unit.Weapons)
                .Where(x => displayed.ContainsKey(x))
                .Select(x => new ListBoxItem<UnitWeapon>
                {
                    Text = Mode == WeaponListMode.Counter
                        ? CounterListBoxText(src, x, TargetUnit)
                        : ListBoxText(x),
                    ListItemComment = "",
                    ListItemFlag = !displayed[x].CanUse,
                    ListItemID = "",
                    ListItemObject = x,
                })
                .ToList();
        }

        // 援護攻撃を使うかどうかを切り替える行を出すか（GUI.bas の WeaponListBox）
        // 攻撃の武器選択（移動前・移動後）で、援護攻撃をしてくれるユニットがいるときに出す。
        public bool HasSupportAttackItem =>
            (Mode == WeaponListMode.BeforeMove || Mode == WeaponListMode.AfterMove)
            && Unit.LookForSupportAttack(null) != null;

        // 援護攻撃を使うかどうかを切り替える行の表示内容
        public static string SupportAttackListBoxText(bool useSupportAttack)
        {
            return useSupportAttack ? "援護攻撃：使用する" : "援護攻撃：使用しない";
        }

        // 武器選択用リストボックスを出して武器を選ぶ（GUI.bas の WeaponListBox）。
        // 援護攻撃を切り替える行があれば最後に付け、それを選んだら Commands.UseSupportAttack を
        // 切り替えてリストボックスを出し直す。キャンセルしたときは null を返す。
        public UnitWeapon SelectWeapon(SRC src, string caption_msg, Func<ListBoxArgs, int> listBox)
        {
            var list = ToListBoxItems(src);
            var items = list.Cast<ListBoxItem>().ToList();
            ListBoxItem supportAttackItem = null;
            if (HasSupportAttackItem)
            {
                // 援護攻撃を使うかどうか選択
                src.Commands.UseSupportAttack = true;
                supportAttackItem = new ListBoxItem(SupportAttackListBoxText(true), "");
                items.Add(supportAttackItem);
            }

            var args = new ListBoxArgs
            {
                Items = items,
                HasFlag = true,
                lb_caption = caption_msg,
                lb_info = ListBoxInfo(src),
                // 反撃の武器選択は選んでも閉じず（呼び出し元が閉じる）、カーソルをリストボックスへ動かす
                lb_mode = Mode == WeaponListMode.Counter ? "連続表示,カーソル移動" : "",
            };
            while (true)
            {
                var ret = listBox(args);
                if (ret <= list.Count)
                {
                    return ret > 0 ? list[ret - 1].ListItemObject : null;
                }

                // 援護攻撃のオン/オフ切り替え
                src.Commands.UseSupportAttack = !src.Commands.UseSupportAttack;
                supportAttackItem.Text = SupportAttackListBoxText(src.Commands.UseSupportAttack);
            }
        }

        // 攻撃・武装一覧での武器の表示内容
        public static string ListBoxText(UnitWeapon w)
        {
            var wd = w.WeaponData;
            var power = w.WeaponPower("");
            string text;

            // 攻撃力
            if (power < 10000)
            {
                text = GeneralLib.RightPaddedString(wd.Nickname(), 27)
                    + GeneralLib.LeftPaddedString(SrcFormatter.Format(power), 4);
            }
            else
            {
                text = GeneralLib.RightPaddedString(wd.Nickname(), 26)
                    + GeneralLib.LeftPaddedString(SrcFormatter.Format(power), 5);
            }

            // 最大射程
            if (w.WeaponMaxRange() > 1)
            {
                text += GeneralLib.LeftPaddedString(SrcFormatter.Format(wd.MinRange) + "-" + SrcFormatter.Format(w.WeaponMaxRange()), 5);
            }
            else
            {
                text += "    1";
            }

            // 命中率修正
            text += GeneralLib.LeftPaddedString(SignedString(w.WeaponPrecision()), 4);

            // 残り弾数
            text += BulletString(w);

            // ＥＮ消費量
            text += ENConsumptionString(w);

            // クリティカル率修正
            text += GeneralLib.LeftPaddedString(SignedString(w.WeaponCritical()), 4);

            return text + AdaptionMoraleClassString(w);
        }

        // 反撃の武器選択での武器の表示内容
        public static string CounterListBoxText(SRC src, UnitWeapon w, Unit target)
        {
            var wd = w.WeaponData;
            // 攻撃力
            var text = GeneralLib.RightPaddedString(wd.Nickname(), 29)
                + GeneralLib.LeftPaddedString(SrcFormatter.Format(w.WeaponPower("")), 4);

            var showProbability = !src.Expression.IsOptionDefined("予測命中率非表示");

            // 命中率
            if (showProbability)
            {
                text += GeneralLib.LeftPaddedString(SrcFormatter.Format(GeneralLib.MinLng(w.HitProbability(target, true), 100)) + "%", 5);
            }
            else
            {
                text += GeneralLib.LeftPaddedString(SignedString(w.WeaponPrecision()), 5);
            }

            // クリティカル率
            if (showProbability)
            {
                text += GeneralLib.LeftPaddedString(SrcFormatter.Format(GeneralLib.MinLng(w.CriticalProbability(target), 100)) + "%", 5);
            }
            else
            {
                text += GeneralLib.LeftPaddedString(SignedString(w.WeaponCritical()), 5);
            }

            // 残り弾数
            text += BulletString(w);

            // ＥＮ消費量
            text += ENConsumptionString(w);

            return text + AdaptionMoraleClassString(w);
        }

        private static string SignedString(int value)
        {
            return value >= 0 ? "+" + SrcFormatter.Format(value) : SrcFormatter.Format(value);
        }

        private static string BulletString(UnitWeapon w)
        {
            return w.WeaponData.Bullet > 0
                ? GeneralLib.LeftPaddedString(SrcFormatter.Format(w.Bullet()), 3)
                : "  -";
        }

        private static string ENConsumptionString(UnitWeapon w)
        {
            return w.WeaponData.ENConsumption > 0
                ? GeneralLib.LeftPaddedString(SrcFormatter.Format(w.WeaponENConsumption()), 4)
                : "   -";
        }

        // 地形適応、必要気力、属性
        private static string AdaptionMoraleClassString(UnitWeapon w)
        {
            var wd = w.WeaponData;
            var text = " " + wd.Adaption;

            // 必要気力
            if (wd.NecessaryMorale > 0)
            {
                text += " 気" + SrcFormatter.Format(wd.NecessaryMorale);
            }

            // 属性
            var wclass = w.WeaponClass();
            if (GeneralLib.InStrNotNest(wclass, "|") > 0)
            {
                wclass = Strings.Left(wclass, GeneralLib.InStrNotNest(wclass, "|") - 1);
            }
            return text + " " + wclass;
        }

        // 武器を攻撃力の順に並べる（GUI.bas の WeaponListBox の「攻撃力でソート」）
        // 攻撃力が同じ場合の比べ方も元の SRC のとおりにする。
        public static IList<UnitWeapon> SortByPower(IList<UnitWeapon> weapons)
        {
            var n = weapons.Count;
            var wpower = new int[n + 1];
            var warray = new int[n + 1];
            for (var i = 1; i <= n; i++)
            {
                wpower[i] = weapons[i - 1].WeaponPower("");
            }

            for (var i = 1; i <= n; i++)
            {
                var wd = weapons[i - 1].WeaponData;
                int j;
                for (j = 1; j <= i - 1; j++)
                {
                    var other = weapons[warray[i - j] - 1].WeaponData;
                    if (wpower[i] > wpower[warray[i - j]])
                    {
                        break;
                    }
                    else if (wpower[i] == wpower[warray[i - j]])
                    {
                        if (wd.ENConsumption > 0)
                        {
                            if (wd.ENConsumption >= other.ENConsumption)
                            {
                                break;
                            }
                        }
                        else if (wd.Bullet > 0)
                        {
                            if (wd.Bullet <= other.Bullet)
                            {
                                break;
                            }
                        }
                        else
                        {
                            // 元の SRC は ENConsumption の方を warray(i - j) ではなく i - j 番目の武器で比べている
                            if (weapons[i - j - 1].WeaponData.ENConsumption == 0 && other.Bullet == 0)
                            {
                                break;
                            }
                        }
                    }
                }

                for (var k = 1; k <= j - 1; k++)
                {
                    warray[i - k + 1] = warray[i - k];
                }
                warray[i - j + 1] = i;
            }

            return warray.Skip(1).Select(x => weapons[x - 1]).ToList();
        }
    }
}
