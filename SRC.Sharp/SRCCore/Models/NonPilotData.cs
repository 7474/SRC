// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRCCore.VB;

namespace SRCCore.Models
{
    // ノンパイロットデータのクラス
    public class NonPilotData
    {
        // 名称
        public string Name = "";

        private SRC SRC;

        public NonPilotData()
        {
        }

        public NonPilotData(SRC src)
        {
            SRC = src;
        }

        // 愛称
        private string strNickname = "";

        // ビットマップ名
        private string proBitmap = "";
        // ビットマップが存在するか
        public bool IsBitmapMissing;

        // 愛称
        public string Nickname0 => strNickname;

        public string Nickname
        {
            get
            {
                var nickname = strNickname;
                // SRC を持たない（テストなどで直接作った）データは、置き換えをせずに返す
                if (SRC == null)
                {
                    return nickname;
                }

                // イベントで愛称が変更されている？
                if (Strings.InStr(nickname, "主人公") == 1 || Strings.InStr(nickname, "ヒロイン") == 1)
                {
                    nickname = SRC.Expression.GetValueAsString(nickname + "愛称");
                }

                SRC.Expression.ReplaceSubExpression(ref nickname);

                // 表情パターンの場合
                var idx = Strings.InStr(Name, "(");
                if (idx > 1)
                {
                    // パイロット本来の名称or愛称を切り出し
                    var pname = Strings.Left(Name, idx - 1);

                    // そのパイロットが作成されている？
                    if (!SRC.PList.IsDefined(pname))
                    {
                        return nickname;
                    }

                    // パイロットがユニットに乗っている？
                    var u = SRC.PList.Item(pname).Unit;
                    if (u is null)
                    {
                        return nickname;
                    }

                    // 念のため……
                    if (u.CountPilot() == 0)
                    {
                        return nickname;
                    }

                    // パイロットはメインパイロット？
                    var mp = u.MainPilot();
                    if (pname != mp.Name && pname != mp.Data?.Nickname)
                    {
                        return nickname;
                    }

                    // パイロット愛称変更能力を適用
                    if (u.IsFeatureAvailable("パイロット愛称"))
                    {
                        pname = u.FeatureData("パイロット愛称");
                        idx = Strings.InStr(pname, "$(愛称)");
                        if (idx > 0)
                        {
                            pname = Strings.Left(pname, idx - 1) + nickname + Strings.Mid(pname, idx + 5);
                        }
                        nickname = pname;
                    }
                }

                return nickname;
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
