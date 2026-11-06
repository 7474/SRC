using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Extensions;
using SRCCore.Units;
using SRCCore.VB;
using System.Linq;

namespace SRCCore.CmdDatas.Commands
{
    public class RemoveItemCmd : CmdData
    {
        public RemoveItemCmd(SRC src, EventDataLine eventData) : base(src, CmdType.RemoveItemCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            switch (ArgNum)
            {
                case 1:
                    // 指定したユニットが装備しているアイテムすべてを外す
                    RemoveForUnit(Event.SelectedUnitForEvent);
                    break;

                case 2:
                    {
                        var pname = GetArgAsString(2);

                        if (SRC.UList.IsDefined(pname))
                        {
                            // 指定したユニットが装備しているアイテムすべてを外す
                            RemoveForUnit(SRC.UList.Item(pname).CurrentForm());
                        }
                        else if (SRC.PList.IsDefined(pname))
                        {
                            // 指定したパイロットが乗るユニットが装備しているアイテムすべてを外す
                            RemoveForUnit(SRC.PList.Item(pname)?.Unit);
                        }
                        else
                        {
                            // 指定されたアイテムを削除
                            var u = Event.SelectedUnitForEvent;
                            var iname = pname;
                            if (Information.IsNumeric(iname))
                            {
                                var inumber = Conversions.ToInteger(iname);
                                RemoveForUnitAndNumber(u, iname, inumber);
                                return EventData.NextID;
                            }

                            // アイテムＩＤが指定された場合はそのまま削除
                            if (SRC.IList.IsDefined(iname))
                            {
                                var itm = SRC.IList.Item(iname);

                                // IList.Item はＩＤで見つからなければアイテム名でも探すので、
                                // ＩＤで指定されたときだけここで削除する
                                if (itm.ID == iname)
                                {
                                    if (itm.Unit != null)
                                    {
                                        var iu = itm.Unit;
                                        var item_with_image = itm.IsFeatureAvailable("ユニット画像");
                                        iu.DeleteItem(itm);
                                        if (item_with_image)
                                        {
                                            RefreshUnitBitmap(iu);
                                        }
                                    }

                                    itm.Unit = null;
                                    itm.Exist = false;
                                    return EventData.NextID;
                                }
                            }

                            // 大文字・小文字、ひらがな・かたかなの違いを正しく判定できるように、
                            // 名前をデータのそれとあわせる
                            if (SRC.IDList.IsDefined(iname))
                            {
                                iname = SRC.IDList.Item(iname).Name;
                            }

                            {
                                // まずは装備されていないアイテムを探す
                                var itm = SRC.IList.List.FirstOrDefault(x => x.Name == iname && x.Exist && x.Unit == null);
                                if (itm != null)
                                {
                                    itm.Exist = false;
                                    return EventData.NextID;
                                }

                                // 見つからなかったら装備されたアイテムから
                                itm = SRC.IList.List.FirstOrDefault(x => x.Name == iname && x.Exist);
                                if (itm == null)
                                {
                                    // 見つからなければ何もしない
                                    return EventData.NextID;
                                }

                                var iu = itm.Unit;
                                if (iu != null)
                                {
                                    var item_with_image = itm.IsFeatureAvailable("ユニット画像");
                                    iu.DeleteItem(itm);
                                    if (item_with_image)
                                    {
                                        RefreshUnitBitmap(iu);
                                    }
                                }

                                itm.Unit = null;
                                itm.Exist = false;
                                return EventData.NextID;
                            }
                        }

                        break;
                    }

                case 3:
                    {
                        // 指定されたアイテムを削除
                        var pname = GetArgAsString(2);
                        Unit u;

                        if (SRC.UList.IsDefined(pname))
                        {
                            u = SRC.UList.Item(pname).CurrentForm();
                        }
                        else if (SRC.PList.IsDefined(pname))
                        {
                            u = SRC.PList.Item(pname)?.Unit;
                            if (u == null)
                            {
                                throw new EventErrorException(this, "「" + pname + "」はユニットに乗っていません");
                            }
                        }
                        else
                        {
                            throw new EventErrorException(this, "「" + pname + "」というパイロットが見つかりません");
                        }

                        var iname = GetArgAsString(3);
                        if (Information.IsNumeric(iname))
                        {
                            var inumber = Conversions.ToInteger(iname);
                            RemoveForUnitAndNumber(u, iname, inumber);
                            return EventData.NextID;
                        }

                        // 大文字・小文字、ひらがな・かたかなの違いを正しく判定できるように、
                        // 名前をデータのそれとあわせる
                        if (SRC.IDList.IsDefined(iname))
                        {
                            iname = SRC.IDList.Item(iname).Name;
                        }

                        var itm = u.ItemList.FirstOrDefault(x => (x.Name == iname || x.ID == iname) && x.Exist);
                        if (itm != null)
                        {
                            var item_with_image = itm.IsFeatureAvailable("ユニット画像");
                            u.DeleteItem(itm);
                            if (item_with_image)
                            {
                                RefreshUnitBitmap(u);
                            }
                            itm.Unit = null;
                            itm.Exist = false;
                        }

                        break;
                    }

                default:
                    throw new EventErrorException(this, "RemoveItemコマンドの引数の数が違います");
            }

            return EventData.NextID;
        }

        private void RemoveForUnitAndNumber(Unit u, string iname, int inumber)
        {
            if (inumber < 1)
            {
                throw new EventErrorException(this, "指定されたアイテム番号「" + iname + "」が不正です");
            }

            if (inumber > u.CountItem())
            {
                throw new EventErrorException(this, "指定されたユニットは" + SrcFormatter.Format(u.CountItem()) + "個のアイテムしか持っていません");
            }

            var itm = u.Item(inumber);
            var item_with_image = itm.IsFeatureAvailable("ユニット画像");
            u.DeleteItem(itm);
            if (item_with_image)
            {
                RefreshUnitBitmap(u);
            }
            itm.Unit = null;
            itm.Exist = false;
        }

        private void RemoveForUnit(Unit u)
        {
            if (u == null) { return; }

            // 元の SRC と同じく、先頭のアイテムを外すことを、アイテムが無くなるまで繰り返す
            // （ハードポイントの都合で一緒に外れたアイテムは、Exist を変えずにユニットから外れる）
            var item_with_image = false;
            while (u.CountItem() > 0)
            {
                var itm = u.Item(1);
                if (itm.IsFeatureAvailable("ユニット画像"))
                {
                    item_with_image = true;
                }

                if (u.Party0 != "味方")
                {
                    itm.Exist = false;
                }

                u.DeleteItem(itm);
            }

            if (item_with_image)
            {
                RefreshUnitBitmap(u);
            }
        }

        // ユニット画像を持つアイテムを外したときに、ユニットを描き直す。
        // 元の SRC の BitmapID の作り直しは、SRC# では描画のときに画像を引くので要らない。
        // 元の SRC はユニット・アイテムＩＤ・アイテム名で指定したときに SelectedUnitForEvent を描き直すが、
        // アイテムを外したユニットを描き直す（元の SRC の誤り）。
        private void RefreshUnitBitmap(Unit u)
        {
            if (u.Status == "出撃")
            {
                if (!GUI.IsPictureVisible && !Map.IsStatusView)
                {
                    GUI.PaintUnitBitmap(u);
                }
            }
        }
    }
}
