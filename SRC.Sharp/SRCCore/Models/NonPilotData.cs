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
                //// イベントで愛称が変更されている？
                //if (Strings.InStr(NicknameRet, "主人公") == 1 || Strings.InStr(NicknameRet, "ヒロイン") == 1)
                //{
                //    NicknameRet = Expression.GetValueAsString(ref NicknameRet + "愛称");
                //}

                //Expression.ReplaceSubExpression(ref NicknameRet);

                //// 表情パターンの場合
                //idx = Strings.InStr(Name, "(");
                //if (idx > 1)
                //{
                //    // パイロット本来の名称or愛称を切り出し
                //    pname = Strings.Left(Name, idx - 1);

                //    // そのパイロットが作成されている？
                //    bool localIsDefined() { object argIndex1 = pname; var ret = SRC.PList.IsDefined(ref argIndex1); return ret; }

                //    if (!localIsDefined())
                //    {
                //        return default;
                //    }

                //    {
                //        var withBlock = SRC.PList.Item(ref pname);
                //        // パイロットがユニットに乗っている？
                //        if (withBlock.Unit is null)
                //        {
                //            return default;
                //        }

                //        {
                //            var withBlock1 = withBlock.Unit;
                //            // 念のため……
                //            if (withBlock1.CountPilot() == 0)
                //            {
                //                return default;
                //            }

                //            // パイロットはメインパイロット？
                //            if ((pname ?? "") != (withBlock1.MainPilot().Name ?? "") && (pname ?? "") != (withBlock1.MainPilot().Data.Nickname ?? ""))
                //            {
                //                return default;
                //            }

                //            // パイロット愛称変更能力を適用
                //            if (withBlock1.IsFeatureAvailable(ref "パイロット愛称"))
                //            {
                //                pname = withBlock1.FeatureData(ref "パイロット愛称");
                //                idx = Strings.InStr(pname, "$(愛称)");
                //                if (idx > 0)
                //                {
                //                    pname = Strings.Left(pname, idx - 1) + NicknameRet + Strings.Mid(pname, idx + 5);
                //                }

                //                NicknameRet = pname;
                //            }
                //        }
                //    }
                //}

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
