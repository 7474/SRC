// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

namespace SRC.Core
{
    // ユーザーインターフェースと画面描画の処理を行うためのインタフェース
    public interface IGUI
    {
        void DisplayLoadingProgress();
        void ErrorMessage(string str);
    }
}
