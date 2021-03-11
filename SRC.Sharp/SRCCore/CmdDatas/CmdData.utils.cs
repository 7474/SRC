// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Units;
using SRCCore.VB;

namespace SRCCore.CmdDatas
{
    // イベントコマンドのクラス
    public abstract partial class CmdData
    {
        // 話者の中央表示用サブルーチン
        protected void CenterUnit(string pname, bool without_cursor, int X = 0, int Y = 0)
        {
            // 座標が指定されている場合
            if (X != 0 && Y != 0)
            {
                if (!Map.IsInside(X, Y))
                {
                    // マップ外
                    return;
                }

                CenterUnitInternal(without_cursor, X, Y);
                return;
            }

            if (pname == "母艦")
            {
                // 母艦を中央表示
                foreach (Unit u in SRC.UList.Items)
                {
                    if (u.Party0 == "味方" & u.Status == "出撃")
                    {
                        if (u.IsFeatureAvailable("母艦"))
                        {
                            CenterUnitInternal(without_cursor, u.x, u.y);
                            return;
                        }
                    }
                }
                return;
            }

            // 表情パターン名での指定はパイロット名に変換しておく
            if (!SRC.PList.IsDefined(pname) && Strings.InStr(pname, "(") > 0)
            {
                var pnameEdited = Strings.Left(pname, Strings.InStr(pname, "(") - 1);
                if (SRC.PList.IsDefined(pnameEdited))
                {
                    pname = pnameEdited;
                }
            }

            if (!SRC.PList.IsDefined(pname) && SRC.NPDList.IsDefined(pname))
            {
                pname = SRC.NPDList.Item(pname).Nickname;
            }

            // 話者はパイロット？
            if (!SRC.PList.IsDefined(pname))
            {
                return;
            }

            var pilot = SRC.PList.Item(pname);
            if (pilot.Unit is object)
            {
                // パイロットが乗っているユニットを中央表示
                if (pilot.Unit.Status == "出撃" || pilot.Unit.Status == "格納")
                {
                    CenterUnitInternal(without_cursor, pilot.Unit.x, pilot.Unit.y);
                }
            }

            // 話者が味方でかつ出撃中でない場合は母艦を中央表示
            if (pilot.Party == "味方")
            {
                CenterUnit("母艦", without_cursor);
            }
        }

        private void CenterUnitInternal(bool without_cursor, int xx, int yy)
        {
            if (GUI.MapX != xx || GUI.MapY != yy)
            {
                GUI.Center(xx, yy);
                GUI.RefreshScreen(false, true);
            }

            if (!GUI.IsCursorVisible && !without_cursor)
            {
                // Impl
                bool tmp = GUI.IsPictureVisible;
                GUI.DrawPicture(@"Event\cursor.bmp",
                    Constants.DEFAULT_LEVEL, Constants.DEFAULT_LEVEL, Constants.DEFAULT_LEVEL, Constants.DEFAULT_LEVEL,
                    0, 0, 0, 0,
                    "透過");
                GUI.IsPictureVisible = tmp;
                GUI.IsCursorVisible = true;
            }

            GUI.UpdateScreen();
        }
    }
}
