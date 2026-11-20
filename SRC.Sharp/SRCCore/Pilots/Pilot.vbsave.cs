// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Lib;

namespace SRCCore.Pilots
{
    // 元の SRC（VB5）のセーブデータのパイロットを読む（Pilot.cls の Restore・RestoreLinkInfo・RestoreParameter。#1577）
    public partial class Pilot
    {
        // 一時中断用データをファイルからロードする（VB: Restore。New Pilot に読む）
        internal static Pilot Restore(SRC src, VbSaveDataReader reader)
        {
            var p = new Pilot { SRC = src };

            // Name, ID, Party
            p.Name = reader.Input();
            p.ID = reader.Input();
            p.Party = reader.Input();

            // Leve, Exp
            p.Level = reader.InputInt();
            p.Exp = reader.InputInt();

            // SP, Morale, Plana
            p.SP = reader.InputInt();
            p.Morale = reader.InputInt();
            p.Plana = reader.InputInt();

            // Alive, Away, SupportIndex
            p.Alive = reader.InputBool();
            p.Away = reader.InputBool();
            p.SupportIndex = reader.InputInt();

            // Unit
            reader.LineInput();
            return p;
        }

        // 一時中断用データのリンク情報をファイルからロードする（VB: RestoreLinkInfo）
        public void RestoreLinkInfo(VbSaveDataReader reader)
        {
            // Name, ID, Party
            reader.LineInput();
            // Leve, Exp
            reader.LineInput();
            // SP, Morale, Plana
            reader.LineInput();
            // Alive, Away, SupportIndex
            reader.LineInput();

            // Unit
            Unit = SRC.UList.Item(reader.Input());
        }

        // 一時中断用データのパラメータ情報をファイルからロードする（VB: RestoreParameter）
        public void RestoreParameter(VbSaveDataReader reader)
        {
            // Name, ID, Party
            reader.LineInput();
            // Leve, Exp
            reader.LineInput();

            // SP, Morale, Plana
            SP = reader.InputInt();
            Morale = reader.InputInt();
            Plana = reader.InputInt();

            // Alive, Away, SupportIndex
            reader.LineInput();
            // Unit
            reader.LineInput();
        }
    }
}
