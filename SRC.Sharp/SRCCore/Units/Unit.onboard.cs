// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Maps;
using SRCCore.Models;
using SRCCore.Pilots;
using SRCCore.VB;
using System.Collections.Generic;

namespace SRCCore.Units
{
    // === ユニット搭載関連処理 ===
    public partial class Unit
    {
        // ユニットを搭載
        public void LoadUnit(Unit u)
        {
            colUnitOnBoard.Add(u, u.ID);
        }

        // 搭載したユニットを削除
        public void UnloadUnit(object Index)
        {
            short i;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 896556


            Input:

                    On Error GoTo ErrorHandler

             */
            colUnitOnBoard.Remove(Index);
            return;
        ErrorHandler:
            ;
            var loopTo = (short)colUnitOnBoard.Count;
            for (i = 1; i <= loopTo; i++)
            {
                // UPGRADE_WARNING: オブジェクト colUnitOnBoard(i).Name の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(colUnitOnBoard[(int)i].Name, Index, false)))
                {
                    colUnitOnBoard.Remove(i);
                    return;
                }
            }
        }

        // 搭載したユニットの総数
        public short CountUnitOnBoard()
        {
            short CountUnitOnBoardRet = default;
            CountUnitOnBoardRet = (short)colUnitOnBoard.Count;
            return CountUnitOnBoardRet;
        }

        // 搭載したユニット
        public Unit UnitOnBoard(object Index)
        {
            Unit UnitOnBoardRet = default;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 897275


            Input:

                    On Error GoTo ErrorHandler

             */
            UnitOnBoardRet = (Unit)colUnitOnBoard[Index];
            return UnitOnBoardRet;
        ErrorHandler:
            ;
            foreach (Unit u in colUnitOnBoard)
            {
                // UPGRADE_WARNING: オブジェクト Index の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(u.Name, Index, false)))
                {
                    UnitOnBoardRet = u;
                    return UnitOnBoardRet;
                }
            }
        }

    }
}
