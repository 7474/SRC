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
        public bool IsEmpty => Item == null;

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
            foreach (var itm in u.ItemList.Where(x => x.Class() != "固定" && !x.IsFeatureAvailable("非表示")))
            {

                switch (itm.Part() ?? "")
                {
                    case "両手":
                        {
                            var slots = Slots.Where(x => x.IsMatch("両手")).ToList();
                            if (slots.Count == 2)
                            {
                                slots[0].Item = itm;
                                slots[0].IsOccupied = false;
                                slots[1].Item = itm;
                                slots[1].IsOccupied = true;
                            }
                            break;
                        }

                    case "片手":
                        {
                            var slot = Slots.Where(x => x.IsMatch("片手") && x.IsEmpty).FirstOrDefault();
                            if (slot != null)
                            {
                                slot.Item = itm;
                                slot.IsOccupied = false;
                            }
                            break;
                        }

                    case "盾":
                        {
                            var slot = Slots.Where(x => x.IsMatch("盾") && x.IsEmpty).FirstOrDefault();
                            if (slot != null)
                            {
                                slot.Item = itm;
                                slot.IsOccupied = false;
                            }
                            break;
                        }

                    case "両肩":
                        {
                            var slots = Slots.Where(x => x.IsMatch("両肩")).ToList();
                            if (slots.Count == 2)
                            {
                                slots[0].Item = itm;
                                slots[0].IsOccupied = false;
                                slots[1].Item = itm;
                                slots[1].IsOccupied = true;
                            }
                            break;
                        }

                    case "肩":
                        {
                            var slot = Slots.Where(x => x.IsMatch("肩") && x.IsEmpty).FirstOrDefault();
                            if (slot != null)
                            {
                                slot.Item = itm;
                                slot.IsOccupied = false;
                            }
                            break;
                        }
                    // 無視
                    case "非表示":
                        {
                            break;
                        }

                    default:
                        {
                            var slots = Slots.Where(x => x.IsMatch(itm.Part()) && x.IsEmpty).ToList();
                            if (slots.Count >= itm.Size())
                            {
                                slots[0].Item = itm;
                                slots[0].IsOccupied = false;
                                slots.Skip(1).Take(itm.Size() - 1).ToList().ForEach(slot =>
                                {
                                    slot.Item = itm;
                                    slot.IsOccupied = true;
                                });
                            }
                            break;
                        }
                }
            }
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
