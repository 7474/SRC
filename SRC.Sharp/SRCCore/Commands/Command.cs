// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

namespace SRCCore.Commands
{
    // ユニット＆マップコマンドの実行を行うモジュール
    public partial class Command
    {
        private SRC SRC;
        private Events.Event Event => SRC.Event;
        private IGUI GUI => SRC.GUI;
        private IGUIStatus Status => SRC.GUIStatus;
        private Maps.Map Map => SRC.Map;
        private Expressions.Expression Expression => SRC.Expression;

        public Command(SRC src)
        {
            SRC = src;
        }
    }
}