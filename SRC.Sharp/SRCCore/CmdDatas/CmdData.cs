// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRC.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace SRC.Core.CmdDatas
{
    // イベントコマンドのクラス
    public partial class CmdData
    {
        // コマンドの種類
        private CmdType CmdName;
        // 引数の数
        public short ArgNum;
        // コマンドのEventDataにおける位置
        public int LineNum;

        // 引数の値
        private int[] lngArgs;
        private double[] dblArgs;
        private string[] strArgs;

        // 引数の型
        private Expressions.ValueType[] ArgsType;

        // コマンドの種類
        public CmdType Name
        {
            get
            {
                // XXX これ要るの？
                //object NameRet = default;
                //if (CmdName == CmdType.NullCmd)
                //{
                //    Parse(ref Event_Renamed.EventData[LineNum]);
                //}

                //NameRet = CmdName;
                //return NameRet;
                return CmdName;
            }

            set
            {
                CmdName = value;
            }
        }

    }
}
