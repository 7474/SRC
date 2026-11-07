// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

namespace SRCCore.Models
{
    // ノンパイロットデータのクラス
    public class NonPilotData
    {
        // 名称
        public string Name;

        // 愛称
        private string strNickname;

        // ビットマップ名
        private string proBitmap;
        // ビットマップが存在するか
        public bool IsBitmapMissing;

        // 愛称
        public string Nickname0 => strNickname;

        public string Nickname
        {
            get
            {
                string NicknameRet = default;
                string pname;
                int idx;
                NicknameRet = strNickname;

                // Impl

                return NicknameRet;
            }

            set
            {
                strNickname = value;
            }
        }

        // ビットマップ
        public string Bitmap0 => proBitmap;

        public string Bitmap
        {
            get
            {
                string BitmapRet = default;
                if (IsBitmapMissing)
                {
                    BitmapRet = "-.bmp";
                }
                else
                {
                    BitmapRet = proBitmap;
                }

                return BitmapRet;
            }

            set
            {
                proBitmap = value;
            }
        }
    }
}
