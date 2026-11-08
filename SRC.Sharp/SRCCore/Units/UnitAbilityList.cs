using SRCCore.Lib;
using SRCCore.VB;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Units
{
    public class UnitAbilityListItem
    {
        public UnitAbility Ability { get; set; }
        public bool CanUse { get; set; }
    }

    public class UnitAbilityList
    {
        public UnitAbilityList(AbilityListMode mode, Unit unit, Unit targetUnit = null)
        {
            Mode = mode;
            Unit = unit;
            Items = unit.Abilities.Where(x => x.IdDisplayFor(Mode))
                .Select(x => new UnitAbilityListItem
                {
                    Ability = x,
                    CanUse = x.CanUseFor(mode, targetUnit),
                })
                .ToList();
        }

        public AbilityListMode Mode { get; set; }
        public IList<UnitAbilityListItem> Items { get; set; }

        // アビリティを持つユニット
        public Unit Unit { get; }

        // リストボックスを出さずに選ぶアビリティ（GUI.bas の AbilityListBox）。
        // ユニットコマンドのメニューにアビリティ名そのものを表示しているとき（アビリティが 1 つのとき）は、
        // 一覧を出さずに、アイテムでない修得済みの最初のアビリティを選ぶ。該当しなければ null。
        public UnitAbility AbilityWithoutListBox(SRC src, string lb_mode, bool is_item)
        {
            var caption = src.Commands.AbilityCommandCaption;
            if (lb_mode == "一覧" || is_item
                || caption == null
                || caption == src.Expression.Term("アビリティ", Unit))
            {
                return null;
            }
            return Unit.Abilities.FirstOrDefault(x => !x.Data.IsItem() && x.IsAbilityMastered());
        }

        // アビリティ選択用リストボックスの見出し（GUI.bas の AbilityListBox）
        public string ListBoxInfo(SRC src)
        {
            return "名称                 効果                            射程 数  "
                + src.Expression.Term("EN", Unit, 2) + " 分類";
        }

        // アビリティ選択用リストボックスの各アビリティの表示内容（GUI.bas の AbilityListBox）
        // 効果の解説が長いアビリティは 2 行にする。2 行目も ListItemObject は同じアビリティ。
        // is_item が真ならアイテムの使用効果だけを、偽ならアイテムでないアビリティだけを出す
        // （一覧では、ステータス画面で使えないアビリティはアイテムかどうかに関わらず出す。元の SRC と同じ）。
        public IList<ListBoxItem<UnitAbility>> ToListBoxItems(bool is_item = false)
        {
            var list = new List<ListBoxItem<UnitAbility>>();
            foreach (var item in Items)
            {
                if (item.Ability.Data.IsItem() != is_item
                    && (Mode != AbilityListMode.List || item.Ability.IsAbilityAvailable("ステータス")))
                {
                    continue;
                }
                foreach (var text in ListBoxTexts(item.Ability))
                {
                    list.Add(new ListBoxItem<UnitAbility>
                    {
                        Text = text,
                        ListItemComment = "",
                        ListItemFlag = !item.CanUse,
                        ListItemID = "",
                        ListItemObject = item.Ability,
                    });
                }
            }
            return list;
        }

        // アビリティの表示内容。効果の解説が長すぎる場合は残りを 2 行目にする。
        public static IList<string> ListBoxTexts(UnitAbility a)
        {
            var ad = a.Data;
            var text = GeneralLib.RightPaddedString(ad.Nickname(), 20);

            var msg = "";
            var rest_msg = "";
            foreach (var ae in ad.Effects)
            {
                var ename = ad.EffectName(ae);
                if (ae.EffectType == "解説")
                {
                    msg = ename;
                    break;
                }
                else if (Strings.InStr(ename, "ターン)") > 0)
                {
                    // 持続時間が同じ能力はターン数をまとめて表示
                    var k = Strings.InStr(msg, Strings.Mid(ename, Strings.InStr(ename, "(")));
                    if (k > 0)
                    {
                        msg = Strings.Left(msg, k - 1) + "、"
                            + Strings.Left(ename, Strings.InStr(ename, "(") - 1)
                            + Strings.Mid(msg, k);
                    }
                    else
                    {
                        msg = msg + " " + ename;
                    }
                }
                else if (!string.IsNullOrEmpty(ename))
                {
                    msg = msg + " " + ename;
                }
            }
            msg = Strings.Trim(msg);

            // 効果解説が長すぎる場合は改行
            if (GeneralLib.StrWidth(msg) > 32)
            {
                var buf = msg;
                do
                {
                    buf = Strings.Left(buf, Strings.Len(buf) - 1);
                }
                while (GeneralLib.StrWidth(buf) >= 32);
                rest_msg = Strings.Mid(msg, Strings.Len(buf) + 1);
                if (GeneralLib.StrWidth(buf) < 32)
                {
                    buf = buf + Strings.Space(32 - GeneralLib.StrWidth(buf));
                }
                msg = buf;
            }

            text = GeneralLib.RightPaddedString(text + " " + msg, 53);

            // 最大射程
            if (a.AbilityMaxRange() > 1)
            {
                text += GeneralLib.LeftPaddedString(SrcFormatter.Format(a.AbilityMinRange()) + "-" + SrcFormatter.Format(a.AbilityMaxRange()), 4);
            }
            else if (a.AbilityMaxRange() == 1)
            {
                text += "   1";
            }
            else
            {
                text += "   -";
            }

            // 残り使用回数
            if (ad.Stock > 0)
            {
                text += GeneralLib.LeftPaddedString(SrcFormatter.Format(a.Stock()), 3);
            }
            else
            {
                text += "  -";
            }

            // ＥＮ消費量
            if (ad.ENConsumption > 0)
            {
                text += GeneralLib.LeftPaddedString(SrcFormatter.Format(a.AbilityENConsumption()), 4);
            }
            else
            {
                text += "   -";
            }

            // 必要気力
            if (ad.NecessaryMorale > 0)
            {
                text += " 気" + SrcFormatter.Format(ad.NecessaryMorale);
            }

            // 属性
            if (GeneralLib.InStrNotNest(ad.Class, "|") > 0)
            {
                text += " " + Strings.Left(ad.Class, GeneralLib.InStrNotNest(ad.Class, "|") - 1);
            }
            else
            {
                text += " " + ad.Class;
            }

            var texts = new List<string> { text };
            if (!string.IsNullOrEmpty(rest_msg))
            {
                texts.Add(Strings.Space(21) + rest_msg);
            }
            return texts;
        }
    }
}
