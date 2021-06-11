using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class EquipCmd : CmdData
    {
        public EquipCmd(SRC src, EventDataLine eventData) : base(src, CmdType.EquipCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
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
                    {
                        Event.EventErrorMessage = "Equipコマンドの引数の数が違います";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 223923


                        Input:
                                        Error(0)

                         */
                        break;
                    }
            }

            // 大文字・小文字、ひらがな・かたかなの違いを正しく判定できるように、
            // 名前をデータのそれとあわせる
            if (SRC.IDList.IsDefined(iname))
            {
                ItemData localItem() { object argIndex1 = iname; var ret = SRC.IDList.Item(argIndex1); return ret; }

                iname = localItem().Name;
            }

            // 装備するアイテムを検索 or 作成
            bool localIsDefined() { object argIndex1 = iname; var ret = SRC.IDList.IsDefined(argIndex1); return ret; }

            if (SRC.IList.IsDefined(iname))
            {
                Item localItem1() { object argIndex1 = (object)iname; var ret = SRC.IList.Item(argIndex1); return ret; }

                if ((iname ?? "") == (localItem1().Name ?? ""))
                {
                    // アイテム名で指定した場合
                    if (u.Party0 == "味方")
                    {
                        // まずは装備されてないものを探す
                        foreach (Item currentItm in SRC.IList)
                        {
                            itm = currentItm;
                            {
                                var withBlock = itm;
                                if ((withBlock.Name ?? "") == (iname ?? "") && withBlock.Unit is null && withBlock.Exist)
                                {
                                    goto EquipItem;
                                }
                            }
                        }
                        // なかったら装備されているものを…
                        foreach (Item currentItm1 in SRC.IList)
                        {
                            itm = currentItm1;
                            {
                                var withBlock1 = itm;
                                if ((withBlock1.Name ?? "") == (iname ?? "") && withBlock1.Unit is object && withBlock1.Exist)
                                {
                                    if (withBlock1.Unit.Party0 == "味方")
                                    {
                                        goto EquipItem;
                                    }
                                }
                            }
                        }
                        // それでもなければ新たに作成
                        itm = SRC.IList.Add(iname);
                    }
                    else
                    {
                        itm = SRC.IList.Add(iname);
                    }
                }
                else
                {
                    // アイテムＩＤで指定した場合
                    itm = SRC.IList.Item((object)iname);
                }
            }
            else if (localIsDefined())
            {
                itm = SRC.IList.Add(iname);
            }
            else
            {
                Event.EventErrorMessage = "「" + iname + "」というアイテムは存在しません";
                ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 225275


                Input:
                            Error(0)

                 */
            }

        EquipItem:
            ;

            // アイテムを装備
            string ubitmap;
            short rank_lv = default, cmd_lv = default, support_lv = default;
            if (itm is object)
            {
                {
                    var withBlock2 = itm;
                    if (withBlock2.Exist)
                    {
                        if (withBlock2.Unit is object)
                        {
                            withBlock2.Unit.DeleteItem(withBlock2.ID);
                        }

                        {
                            var withBlock3 = u;
                            ubitmap = withBlock3.get_Bitmap(false);
                            if (withBlock3.CountPilot() > 0)
                            {
                                {
                                    var withBlock4 = withBlock3.MainPilot();
                                    cmd_lv = withBlock4.SkillLevel("指揮", ref_mode: "");
                                    rank_lv = withBlock4.SkillLevel("階級", ref_mode: "");
                                    support_lv = withBlock4.SkillLevel("広域サポート", ref_mode: "");
                                }
                            }

                            withBlock3.AddItem(itm);

                            // ユニット画像が変化した？
                            if ((ubitmap ?? "") != (withBlock3.get_Bitmap(false) ?? ""))
                            {
                                withBlock3.BitmapID = GUI.MakeUnitBitmap(u);
                                var loopTo = withBlock3.CountOtherForm();
                                for (i = 1; i <= loopTo; i++)
                                {
                                    Unit localOtherForm() { object argIndex1 = i; var ret = withBlock3.OtherForm(argIndex1); return ret; }

                                    localOtherForm().BitmapID = 0;
                                }

                                if (withBlock3.Status == "出撃")
                                {
                                    if (!GUI.IsPictureVisible && !string.IsNullOrEmpty(Map.MapFileName))
                                    {
                                        GUI.PaintUnitBitmap(u);
                                    }
                                }
                            }

                            // 支援効果が変化した？
                            if (withBlock3.CountPilot() > 0)
                            {
                                {
                                    var withBlock5 = withBlock3.MainPilot();
                                    if (cmd_lv != withBlock5.SkillLevel("指揮", ref_mode: "") || rank_lv != withBlock5.SkillLevel("階級", ref_mode: "") || support_lv != withBlock5.SkillLevel("広域サポート", ref_mode: ""))
                                    {
                                        if (u.Status == "出撃")
                                        {
                                            SRC.PList.UpdateSupportMod(u);
                                        }
                                    }
                                }
                            }

                            // 最大弾数が変化した？
                            if (itm.IsFeatureAvailable("最大弾数増加"))
                            {
                                withBlock3.FullSupply();
                            }
                        }
                    }
                }
            }
            return EventData.NextID;
        }
    }
}
