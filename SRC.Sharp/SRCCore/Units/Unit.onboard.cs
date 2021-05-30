// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Extensions;
using System.Linq;

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
        public void UnloadUnit(string Index)
        {
            if (colUnitOnBoard.Remove(Index)) { return; }

            // XXX 一致のケース考慮してなかったかも？
            //if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(colUnitOnBoard[i].Name, Index, false)))
            foreach (var u in UnitOnBoards.Where(x => x.Name == Index))
            {
                colUnitOnBoard.Remove(u);
                return;
            }
        }

        public void UnloadAllUnitForEscape()
        {
            foreach (Unit u in colUnitOnBoard.List.CloneList())
            {
                u.Status = "待機";
                colUnitOnBoard.Remove(u.ID);
            }
        }

        // 搭載したユニットの総数
        public int CountUnitOnBoard()
        {
            return colUnitOnBoard.Count;
        }

        // 搭載したユニット
        public Unit UnitOnBoard(string Index)
        {
            var u = colUnitOnBoard[Index];
            if (u != null)
            {
                return u;
            }
            else
            {
                return UnitOnBoards.FirstOrDefault(x => x.Name == Index);
            }
        }
    }
}
