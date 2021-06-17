using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class RemoveItemCmd : CmdData
    {
        public RemoveItemCmd(SRC src, EventDataLine eventData) : base(src, CmdType.RemoveItemCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //            string pname;
            //            Unit u;
            //            string iname;
            //            short inumber;
            //            var itm = default(Item);
            //            short i, j;
            //            var item_with_image = default(bool);
            //            switch (ArgNum)
            //            {
            //                case 1:
            //                    {
            //                        // 指定したユニットが装備しているアイテムすべてを外す
            //                        {
            //                            var withBlock = Event.SelectedUnitForEvent;
            //                            while (withBlock.CountItem() > 0)
            //                            {
            //                                if (withBlock.Item((object)1).IsFeatureAvailable("ユニット画像"))
            //                                {
            //                                    item_with_image = true;
            //                                }

            //                                if (withBlock.Party0 != "味方")
            //                                {
            //                                    withBlock.Item((object)1).Exist = false;
            //                                }

            //                                withBlock.DeleteItem((object)1);
            //                            }

            //                            if (item_with_image)
            //                            {
            //                                withBlock.BitmapID = GUI.MakeUnitBitmap(Event.SelectedUnitForEvent);
            //                                var loopTo = withBlock.CountOtherForm();
            //                                for (i = 1; i <= loopTo; i++)
            //                                {
            //                                    Unit localOtherForm() { object argIndex1 = (object)i; var ret = withBlock.OtherForm(argIndex1); return ret; }

            //                                    localOtherForm().BitmapID = 0;
            //                                }

            //                                if (withBlock.Status == "出撃")
            //                                {
            //                                    if (!GUI.IsPictureVisible && !string.IsNullOrEmpty(Map.MapFileName))
            //                                    {
            //                                        GUI.PaintUnitBitmap(Event.SelectedUnitForEvent);
            //                                    }
            //                                }
            //                            }
            //                        }

            //                        break;
            //                    }

            //                case 2:
            //                    {
            //                        pname = GetArgAsString(2);
            //                        bool localIsDefined() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

            //                        if (SRC.UList.IsDefined((object)pname))
            //                        {
            //                            // 指定したユニットが装備しているアイテムすべてを外す
            //                            Unit localItem() { object argIndex1 = (object)pname; var ret = SRC.UList.Item(argIndex1); return ret; }

            //                            u = localItem().CurrentForm();
            //                            {
            //                                var withBlock1 = u;
            //                                while (withBlock1.CountItem() > 0)
            //                                {
            //                                    if (withBlock1.Item((object)1).IsFeatureAvailable("ユニット画像"))
            //                                    {
            //                                        item_with_image = true;
            //                                    }

            //                                    if (withBlock1.Party0 != "味方")
            //                                    {
            //                                        withBlock1.Item((object)1).Exist = false;
            //                                    }

            //                                    withBlock1.DeleteItem((object)1);
            //                                }

            //                                if (item_with_image)
            //                                {
            //                                    withBlock1.BitmapID = GUI.MakeUnitBitmap(Event.SelectedUnitForEvent);
            //                                    var loopTo1 = withBlock1.CountOtherForm();
            //                                    for (i = 1; i <= loopTo1; i++)
            //                                    {
            //                                        Unit localOtherForm1() { object argIndex1 = (object)i; var ret = withBlock1.OtherForm(argIndex1); return ret; }

            //                                        localOtherForm1().BitmapID = 0;
            //                                    }

            //                                    if (withBlock1.Status == "出撃")
            //                                    {
            //                                        if (!GUI.IsPictureVisible && !string.IsNullOrEmpty(Map.MapFileName))
            //                                        {
            //                                            GUI.PaintUnitBitmap(Event.SelectedUnitForEvent);
            //                                        }
            //                                    }
            //                                }
            //                            }
            //                        }
            //                        else if (localIsDefined())
            //                        {
            //                            // 指定したパイロットが乗るユニットが装備しているアイテムすべてを外す
            //                            Pilot localItem3() { object argIndex1 = (object)pname; var ret = SRC.PList.Item(argIndex1); return ret; }

            //                            u = localItem3().Unit;
            //                            if (u is object)
            //                            {
            //                                {
            //                                    var withBlock8 = u;
            //                                    while (withBlock8.CountItem() > 0)
            //                                    {
            //                                        if (withBlock8.Item((object)1).IsFeatureAvailable("ユニット画像"))
            //                                        {
            //                                            item_with_image = true;
            //                                        }

            //                                        if (withBlock8.Party0 != "味方")
            //                                        {
            //                                            withBlock8.Item((object)1).Exist = false;
            //                                        }

            //                                        withBlock8.DeleteItem((object)1);
            //                                    }

            //                                    if (item_with_image)
            //                                    {
            //                                        withBlock8.BitmapID = GUI.MakeUnitBitmap(u);
            //                                        var loopTo5 = withBlock8.CountOtherForm();
            //                                        for (i = 1; i <= loopTo5; i++)
            //                                        {
            //                                            Unit localOtherForm5() { object argIndex1 = (object)i; var ret = withBlock8.OtherForm(argIndex1); return ret; }

            //                                            localOtherForm5().BitmapID = 0;
            //                                        }

            //                                        if (withBlock8.Status == "出撃")
            //                                        {
            //                                            if (!GUI.IsPictureVisible && !string.IsNullOrEmpty(Map.MapFileName))
            //                                            {
            //                                                GUI.PaintUnitBitmap(u);
            //                                            }
            //                                        }
            //                                    }
            //                                }
            //                            }
            //                        }
            //                        else
            //                        {
            //                            // 指定されたアイテムを削除
            //                            iname = pname;
            //                            if (Information.IsNumeric(iname))
            //                            {
            //                                {
            //                                    var withBlock2 = Event.SelectedUnitForEvent;
            //                                    inumber = Conversions.ToShort(iname);
            //                                    if (inumber < 1)
            //                                    {
            //                                        Event.EventErrorMessage = "指定されたアイテム番号「" + iname + "」が不正です";
            //                                        ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 416293


            //                                        Input:
            //                                                                        Error(0)

            //                                         */
            //                                    }

            //                                    if (inumber > withBlock2.CountItem())
            //                                    {
            //                                        Event.EventErrorMessage = "指定されたユニットは" + SrcFormatter.Format((object)withBlock2.CountItem()) + "個のアイテムしか持っていません";
            //                                        ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 416523


            //                                        Input:
            //                                                                        Error(0)

            //                                         */
            //                                    }

            //                                    {
            //                                        var withBlock3 = withBlock2.Item((object)inumber);
            //                                        if (withBlock3.IsFeatureAvailable("ユニット画像"))
            //                                        {
            //                                            item_with_image = true;
            //                                        }

            //                                        Event.SelectedUnitForEvent.DeleteItem((object)withBlock3.ID);
            //                                        if (item_with_image)
            //                                        {
            //                                            {
            //                                                var withBlock4 = Event.SelectedUnitForEvent;
            //                                                withBlock4.BitmapID = GUI.MakeUnitBitmap(Event.SelectedUnitForEvent);
            //                                                var loopTo2 = withBlock4.CountOtherForm();
            //                                                for (i = 1; i <= loopTo2; i++)
            //                                                {
            //                                                    Unit localOtherForm2() { object argIndex1 = (object)i; var ret = withBlock4.OtherForm(argIndex1); return ret; }

            //                                                    localOtherForm2().BitmapID = 0;
            //                                                }

            //                                                if (withBlock4.Status == "出撃")
            //                                                {
            //                                                    if (!GUI.IsPictureVisible && !string.IsNullOrEmpty(Map.MapFileName))
            //                                                    {
            //                                                        GUI.PaintUnitBitmap(Event.SelectedUnitForEvent);
            //                                                    }
            //                                                }
            //                                            }
            //                                        }

            //                                        // UPGRADE_NOTE: オブジェクト SelectedUnitForEvent.Item().Unit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //                                        withBlock3.Unit = null;
            //                                        withBlock3.Exist = false;
            //                                        ExecRemoveItemCmdRet = LineNum + 1;
            //                                        return ExecRemoveItemCmdRet;
            //                                    }
            //                                }
            //                            }

            //                            // アイテムＩＤが指定された場合はそのまま削除
            //                            if (SRC.IList.IsDefined((object)iname))
            //                            {
            //                                Item localItem1() { object argIndex1 = (object)iname; var ret = SRC.IList.Item(argIndex1); return ret; }

            //                                if ((localItem1().ID ?? "") == (iname ?? ""))
            //                                {
            //                                    {
            //                                        var withBlock5 = SRC.IList.Item((object)iname);
            //                                        if (withBlock5.Unit is object)
            //                                        {
            //                                            if (withBlock5.IsFeatureAvailable("ユニット画像"))
            //                                            {
            //                                                item_with_image = true;
            //                                            }

            //                                            withBlock5.Unit.DeleteItem((object)withBlock5.ID);
            //                                            if (item_with_image)
            //                                            {
            //                                                withBlock5.Unit.BitmapID = GUI.MakeUnitBitmap(withBlock5.Unit);
            //                                                {
            //                                                    var withBlock6 = withBlock5.Unit;
            //                                                    var loopTo3 = withBlock6.CountOtherForm();
            //                                                    for (i = 1; i <= loopTo3; i++)
            //                                                    {
            //                                                        Unit localOtherForm3() { object argIndex1 = (object)i; var ret = withBlock6.OtherForm(argIndex1); return ret; }

            //                                                        localOtherForm3().BitmapID = 0;
            //                                                    }

            //                                                    if (withBlock6.Status == "出撃")
            //                                                    {
            //                                                        if (!GUI.IsPictureVisible && !string.IsNullOrEmpty(Map.MapFileName))
            //                                                        {
            //                                                            GUI.PaintUnitBitmap(Event.SelectedUnitForEvent);
            //                                                        }
            //                                                    }
            //                                                }
            //                                            }
            //                                        }

            //                                        // UPGRADE_NOTE: オブジェクト IList.Item().Unit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //                                        withBlock5.Unit = null;
            //                                        withBlock5.Exist = false;
            //                                        ExecRemoveItemCmdRet = LineNum + 1;
            //                                        return ExecRemoveItemCmdRet;
            //                                    }
            //                                }
            //                            }

            //                            // 大文字・小文字、ひらがな・かたかなの違いを正しく判定できるように、
            //                            // 名前をデータのそれとあわせる
            //                            if (SRC.IDList.IsDefined((object)iname))
            //                            {
            //                                ItemData localItem2() { object argIndex1 = (object)iname; var ret = SRC.IDList.Item(argIndex1); return ret; }

            //                                iname = localItem2().Name;
            //                            }

            //                            // まずは装備されていないアイテムを探す
            //                            foreach (Item currentItm in SRC.IList)
            //                            {
            //                                itm = currentItm;
            //                                if ((itm.Name ?? "") == (iname ?? "") && itm.Exist && itm.Unit is null)
            //                                {
            //                                    // 見つかった
            //                                    itm.Exist = false;
            //                                    break;
            //                                }
            //                            }
            //                            // 見つからなかったら装備されたアイテムから
            //                            if (itm is null)
            //                            {
            //                                foreach (Item currentItm1 in SRC.IList)
            //                                {
            //                                    itm = currentItm1;
            //                                    if ((itm.Name ?? "") == (iname ?? "") && itm.Exist)
            //                                    {
            //                                        if (itm.IsFeatureAvailable("ユニット画像"))
            //                                        {
            //                                            item_with_image = true;
            //                                        }

            //                                        u = itm.Unit;
            //                                        u.DeleteItem((object)itm.ID);
            //                                        if (item_with_image)
            //                                        {
            //                                            u.BitmapID = GUI.MakeUnitBitmap(u);
            //                                            {
            //                                                var withBlock7 = u;
            //                                                var loopTo4 = withBlock7.CountOtherForm();
            //                                                for (i = 1; i <= loopTo4; i++)
            //                                                {
            //                                                    Unit localOtherForm4() { object argIndex1 = (object)i; var ret = withBlock7.OtherForm(argIndex1); return ret; }

            //                                                    localOtherForm4().BitmapID = 0;
            //                                                }

            //                                                if (withBlock7.Status == "出撃")
            //                                                {
            //                                                    if (!GUI.IsPictureVisible && !string.IsNullOrEmpty(Map.MapFileName))
            //                                                    {
            //                                                        GUI.PaintUnitBitmap(Event.SelectedUnitForEvent);
            //                                                    }
            //                                                }
            //                                            }
            //                                        }

            //                                        // UPGRADE_NOTE: オブジェクト itm.Unit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //                                        itm.Unit = null;
            //                                        itm.Exist = false;
            //                                        break;
            //                                    }
            //                                }
            //                            }
            //                        }

            //                        break;
            //                    }

            //                case 3:
            //                    {
            //                        // 指定されたアイテムを削除
            //                        pname = GetArgAsString(2);
            //                        bool localIsDefined1() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

            //                        if (SRC.UList.IsDefined((object)pname))
            //                        {
            //                            Unit localItem4() { object argIndex1 = (object)pname; var ret = SRC.UList.Item(argIndex1); return ret; }

            //                            u = localItem4().CurrentForm();
            //                        }
            //                        else if (localIsDefined1())
            //                        {
            //                            Pilot localItem5() { object argIndex1 = (object)pname; var ret = SRC.PList.Item(argIndex1); return ret; }

            //                            u = localItem5().Unit;
            //                            if (u is null)
            //                            {
            //                                Event.EventErrorMessage = "「" + pname + "」はユニットに乗っていません";
            //                                ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 421350


            //                                Input:
            //                                                        Error(0)

            //                                 */
            //                            }
            //                        }
            //                        else
            //                        {
            //                            Event.EventErrorMessage = "「" + pname + "」というパイロットが見つかりません";
            //                            ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                            /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 421478


            //                            Input:
            //                                                Error(0)

            //                             */
            //                        }

            //                        iname = GetArgAsString(3);
            //                        {
            //                            var withBlock9 = u;
            //                            if (Information.IsNumeric(iname))
            //                            {
            //                                inumber = Conversions.ToShort(iname);
            //                                if (inumber < 1)
            //                                {
            //                                    Event.EventErrorMessage = "指定されたアイテム番号「" + iname + "」が不正です";
            //                                    ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 421787


            //                                    Input:
            //                                                                Error(0)

            //                                     */
            //                                }

            //                                if (inumber > withBlock9.CountItem())
            //                                {
            //                                    Event.EventErrorMessage = "指定されたユニットは" + SrcFormatter.Format((object)withBlock9.CountItem()) + "個のアイテムしか持っていません";
            //                                    ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 422013


            //                                    Input:
            //                                                                Error(0)

            //                                     */
            //                                }

            //                                {
            //                                    var withBlock10 = withBlock9.Item((object)inumber);
            //                                    if (withBlock10.IsFeatureAvailable("ユニット画像"))
            //                                    {
            //                                        item_with_image = true;
            //                                    }

            //                                    u.DeleteItem((object)withBlock10.ID);
            //                                    if (item_with_image)
            //                                    {
            //                                        {
            //                                            var withBlock11 = u;
            //                                            withBlock11.BitmapID = GUI.MakeUnitBitmap(u);
            //                                            var loopTo6 = withBlock11.CountOtherForm();
            //                                            for (j = 1; j <= loopTo6; j++)
            //                                            {
            //                                                Unit localOtherForm6() { object argIndex1 = (object)j; var ret = withBlock11.OtherForm(argIndex1); return ret; }

            //                                                localOtherForm6().BitmapID = 0;
            //                                            }

            //                                            if (withBlock11.Status == "出撃")
            //                                            {
            //                                                if (!GUI.IsPictureVisible && !string.IsNullOrEmpty(Map.MapFileName))
            //                                                {
            //                                                    GUI.PaintUnitBitmap(u);
            //                                                }
            //                                            }
            //                                        }
            //                                    }

            //                                    // UPGRADE_NOTE: オブジェクト u.Item().Unit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //                                    withBlock10.Unit = null;
            //                                    withBlock10.Exist = false;
            //                                    ExecRemoveItemCmdRet = LineNum + 1;
            //                                    return ExecRemoveItemCmdRet;
            //                                }
            //                            }

            //                            // 大文字・小文字、ひらがな・かたかなの違いを正しく判定できるように、
            //                            // 名前をデータのそれとあわせる
            //                            if (SRC.IDList.IsDefined((object)iname))
            //                            {
            //                                ItemData localItem6() { object argIndex1 = (object)iname; var ret = SRC.IDList.Item(argIndex1); return ret; }

            //                                iname = localItem6().Name;
            //                            }

            //                            var loopTo7 = withBlock9.CountItem();
            //                            for (i = 1; i <= loopTo7; i++)
            //                            {
            //                                {
            //                                    var withBlock12 = withBlock9.Item((object)i);
            //                                    if (((withBlock12.Name ?? "") == (iname ?? "") || (withBlock12.ID ?? "") == (iname ?? "")) && withBlock12.Exist)
            //                                    {
            //                                        if (withBlock12.IsFeatureAvailable("ユニット画像"))
            //                                        {
            //                                            item_with_image = true;
            //                                        }

            //                                        u.DeleteItem((object)withBlock12.ID);
            //                                        if (item_with_image)
            //                                        {
            //                                            {
            //                                                var withBlock13 = u;
            //                                                withBlock13.BitmapID = GUI.MakeUnitBitmap(u);
            //                                                var loopTo8 = withBlock13.CountOtherForm();
            //                                                for (j = 1; j <= loopTo8; j++)
            //                                                {
            //                                                    Unit localOtherForm7() { object argIndex1 = (object)j; var ret = withBlock13.OtherForm(argIndex1); return ret; }

            //                                                    localOtherForm7().BitmapID = 0;
            //                                                }

            //                                                if (withBlock13.Status == "出撃")
            //                                                {
            //                                                    if (!GUI.IsPictureVisible && !string.IsNullOrEmpty(Map.MapFileName))
            //                                                    {
            //                                                        GUI.PaintUnitBitmap(u);
            //                                                    }
            //                                                }
            //                                            }
            //                                        }

            //                                        // UPGRADE_NOTE: オブジェクト u.Item().Unit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //                                        withBlock12.Unit = null;
            //                                        withBlock12.Exist = false;
            //                                        break;
            //                                    }
            //                                }
            //                            }
            //                        }

            //                        break;
            //                    }

            //                default:
            //                    {
            //                        Event.EventErrorMessage = "RemoveItemコマンドの引数の数が違います";
            //                        ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 424422


            //                        Input:
            //                                        Error(0)

            //                         */
            //                        break;
            //                    }
            //            }

            //return EventData.NextID;
        }
    }
}
