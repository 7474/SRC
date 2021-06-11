using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Items;
using SRCCore.Units;
using System.Linq;

namespace SRCCore.CmdDatas.Commands
{
    public class EquipCmd : CmdData
    {
        public EquipCmd(SRC src, EventDataLine eventData) : base(src, CmdType.EquipCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            Unit u;
            string iname;
            switch (ArgNum)
            {
                case 3:
                    {
                        u = GetArgAsUnit(2);
                        iname = GetArgAsString(3);
                        break;
                    }

                case 2:
                    {
                        u = Event.SelectedUnitForEvent;
                        iname = GetArgAsString(2);
                        break;
                    }

                default:
                    throw new EventErrorException(this, "Equipコマンドの引数の数が違います");
            }

            // 大文字・小文字、ひらがな・かたかなの違いを正しく判定できるように、
            // 名前をデータのそれとあわせる
            if (SRC.IDList.IsDefined(iname))
            {
                iname = SRC.IDList.Item(iname).Name;
            }

            // 装備するアイテムを検索 or 作成
            Item itm;
            if (SRC.IList.IsDefined(iname))
            {
                if (iname == SRC.IList.Item(iname).Name)
                {
                    // アイテム名で指定した場合
                    if (u.Party0 == "味方")
                    {
                        // まずは装備されてないものを探す
                        itm = SRC.IList.List
                           .Where(x => x.Exist)
                           .Where(x => x.Name == iname)
                           .FirstOrDefault(x => x.Unit == null);
                        if (itm == null)
                        {
                            // なかったら装備されているものを…
                            itm = SRC.IList.List
                               .Where(x => x.Exist)
                               .Where(x => x.Name == iname)
                               .FirstOrDefault(x => x.Unit != null && x.Unit.Party0 == "味方");

                        }
                        if (itm == null)
                        {
                            // それでもなければ新たに作成
                            itm = SRC.IList.Add(iname);
                        }
                    }
                    else
                    {
                        itm = SRC.IList.Add(iname);
                    }
                }
                else
                {
                    // アイテムＩＤで指定した場合
                    itm = SRC.IList.Item(iname);
                }
            }
            else if (SRC.IDList.IsDefined(iname))
            {
                itm = SRC.IList.Add(iname);
            }
            else
            {
                throw new EventErrorException(this, "「" + iname + "」というアイテムは存在しません");
            }

            // アイテムを装備
            if (itm != null)
            {
                if (itm.Exist)
                {
                    if (itm.Unit != null)
                    {
                        itm.Unit.DeleteItem(itm);
                    }

                    u.AddItem(itm);

                    // ユニット画像が変化した？
                    if (u.Status == "出撃")
                    {
                        if (!GUI.IsPictureVisible && Map.IsStatusView)
                        {
                            GUI.PaintUnitBitmap(u);
                        }
                    }

                    // 支援効果が変化した？
                    if (u.Status == "出撃")
                    {
                        SRC.PList.UpdateSupportMod(u);
                    }

                    // 最大弾数が変化した？
                    if (itm.IsFeatureAvailable("最大弾数増加"))
                    {
                        u.FullSupply();
                    }
                }
            }
            return EventData.NextID;
        }
    }
}
