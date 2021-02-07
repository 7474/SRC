using Microsoft.VisualBasic;

namespace Project1
{
    internal class NonPilotData
    {

        // Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
        // 本プログラムはフリーソフトであり、無保証です。
        // 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
        // 再頒布または改変することができます。

        // ノンパイロットデータのクラス

        // 名称
        public string Name;

        // 愛称
        private string strNickname;

        // ビットマップ名
        private string proBitmap;
        // ビットマップが存在するか
        public bool IsBitmapMissing;


        // 愛称
        public string Nickname0
        {
            get
            {
                string Nickname0Ret = default;
                Nickname0Ret = strNickname;
                return Nickname0Ret;
            }
        }

        public string Nickname
        {
            get
            {
                string NicknameRet = default;
                string pname;
                short idx;
                NicknameRet = strNickname;

                // イベントで愛称が変更されている？
                if (Strings.InStr(NicknameRet, "主人公") == 1 | Strings.InStr(NicknameRet, "ヒロイン") == 1)
                {
                    string argexpr = NicknameRet + "愛称";
                    NicknameRet = Expression.GetValueAsString(ref argexpr);
                }

                Expression.ReplaceSubExpression(ref NicknameRet);

                // 表情パターンの場合
                idx = (short)Strings.InStr(Name, "(");
                if (idx > 1)
                {
                    // パイロット本来の名称or愛称を切り出し
                    pname = Strings.Left(Name, idx - 1);

                    // そのパイロットが作成されている？
                    bool localIsDefined() { object argIndex1 = pname; var ret = SRC.PList.IsDefined(ref argIndex1); return ret; }

                    if (!localIsDefined())
                    {
                        return default;
                    }

                    object argIndex2 = pname;
                    {
                        var withBlock = SRC.PList.Item(ref argIndex2);
                        // パイロットがユニットに乗っている？
                        if (withBlock.Unit_Renamed is null)
                        {
                            return default;
                        }

                        {
                            var withBlock1 = withBlock.Unit_Renamed;
                            // 念のため……
                            if (withBlock1.CountPilot() == 0)
                            {
                                return default;
                            }

                            // パイロットはメインパイロット？
                            if ((pname ?? "") != (withBlock1.MainPilot().Name ?? "") & (pname ?? "") != (withBlock1.MainPilot().Data.Nickname ?? ""))
                            {
                                return default;
                            }

                            // パイロット愛称変更能力を適用
                            string argfname = "パイロット愛称";
                            if (withBlock1.IsFeatureAvailable(ref argfname))
                            {
                                object argIndex1 = "パイロット愛称";
                                pname = withBlock1.FeatureData(ref argIndex1);
                                idx = (short)Strings.InStr(pname, "$(愛称)");
                                if (idx > 0)
                                {
                                    pname = Strings.Left(pname, idx - 1) + NicknameRet + Strings.Mid(pname, idx + 5);
                                }

                                NicknameRet = pname;
                            }
                        }
                    }
                }

                return NicknameRet;
            }

            set
            {
                strNickname = value;
            }
        }

        // ビットマップ
        public string Bitmap0
        {
            get
            {
                string Bitmap0Ret = default;
                Bitmap0Ret = proBitmap;
                return Bitmap0Ret;
            }
        }

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