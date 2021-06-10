using SRCCore.Units;
using SRCCore.VB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRCCore.Items
{
    public class ItemSlot
    {
        public string SlotName { get; set; }
        public Item Item { get; set; }
        /// <summary>
        /// 他のパーツで埋まったスロットかどうか
        /// </summary>
        public bool IsOccupied { get; set; }

        public bool IsMatch(string partName)
        {
            return SlotName == partName
                   || (partName == "片手" || partName == "両手" || partName == "盾")
                       && (SlotName == "右手" || SlotName == "左手")
                   || (partName == "肩" || partName == "両肩")
                       && (SlotName == "右肩" || SlotName == "左肩")
                   || (partName == "アイテム" || partName == "強化パーツ")
                       && (SlotName == "アイテム" || SlotName == "強化パーツ");
        }
    }

    public class ItemSlots
    {
        public IList<ItemSlot> Slots { get; set; } = new List<ItemSlot>();

        public ItemSlots(Unit u)
        {
            Slots = GetPartList(u).Select(x => new ItemSlot
            {
                SlotName = x,
            }).ToList();
        }

        public void FillSlot(Unit u)
        {

        }

        public static IList<string> GetPartList(Unit u)
        {
            // アイテムの装備個所一覧を作成
            var part_list = new List<string>();
            if (u.IsFeatureAvailable("装備個所"))
            {
                var buf = u.FeatureData("装備個所");
                if (Strings.InStr(buf, "腕") > 0)
                {
                    part_list.Add("右手");
                    part_list.Add("左手");
                }

                if (Strings.InStr(buf, "肩") > 0)
                {
                    part_list.Add("右肩");
                    part_list.Add("左肩");
                }

                if (Strings.InStr(buf, "体") > 0)
                {
                    part_list.Add("体");
                }

                if (Strings.InStr(buf, "頭") > 0)
                {
                    part_list.Add("頭");
                }
            }

            foreach (var fd in u.Features.Where(x => x.Name == "ハードポイント"))
            {
                var ipart = fd.Data;
                switch (ipart)
                {
                    // 表示しない
                    case "強化パーツ":
                    case "アイテム":
                    case "非表示":
                        {
                            break;
                        }

                    default:
                        {
                            if (!part_list.Contains(ipart))
                            {
                                for (var i = 0; i < u.ItemSlotSize(ipart); i++)
                                {
                                    part_list.Add(ipart);
                                }
                            }
                            break;
                        }
                }
            }
            for (var i = 0; i < u.MaxItemNum(); i++)
            {
                part_list.Add(u.IsHero() ? "アイテム" : "強化パーツ");
            }
            return part_list;
        }
    }
}
