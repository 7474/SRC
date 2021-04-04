// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Models;
using SRCCore.Units;
using SRCCore.VB;
using System;

namespace SRCCore.Pilots
{
    public partial class Pilot
    {
        // 名称
        public string Name
        {
            get => Data.Name;
            set
            {
                Data = SRC.PDList.Item(value);
                Update();
            }
        }

        // 愛称
        public string Nickname0 => Data.Nickname;

        public string get_Nickname(bool dont_call_unit_nickname)
        {
            return Nickname0;
        }
        // TODO Impl
        //    string NicknameRet = default;
        //    short idx;
        //    Unit u;
        //    string uname = default, vname;
        //    NicknameRet = Nickname0;

        //    // 愛称変更
        //    if (Unit_Renamed is null)
        //    {
        //        Expression.ReplaceSubExpression(NicknameRet);
        //        return default;
        //    }

        //    {
        //        var withBlock = Unit_Renamed;
        //        if (withBlock.CountPilot() > 0)
        //        {
        //            if (!ReferenceEquals(withBlock.MainPilot(), this))
        //            {
        //                Expression.ReplaceSubExpression(NicknameRet);
        //                return default;
        //            }
        //        }

        //        u = Unit_Renamed;

        //        // パイロットステータスコマンド中の場合はユニットを検索する必要がある
        //        if (withBlock.Name == "ステータス表示用ダミーユニット")
        //        {
        //            // メインパイロットかどうかチェック
        //            vname = "搭乗順番[" + ID + "]";
        //            if (Expression.IsLocalVariableDefined(vname))
        //            {
        //                // UPGRADE_WARNING: オブジェクト LocalVariableList.Item(vname).NumericValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
        //                if (Conversions.ToBoolean(Operators.ConditionalCompareObjectNotEqual(Event_Renamed.LocalVariableList[vname].NumericValue, 1, false)))
        //                {
        //                    return default;
        //                }
        //            }
        //            else
        //            {
        //                return default;
        //            }

        //            vname = "搭乗ユニット[" + ID + "]";
        //            if (Expression.IsLocalVariableDefined(vname))
        //            {
        //                // UPGRADE_WARNING: オブジェクト LocalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
        //                uname = Conversions.ToString(Event_Renamed.LocalVariableList[vname].StringValue);
        //            }

        //            if (string.IsNullOrEmpty(uname))
        //            {
        //                return default;
        //            }

        //            u = SRC.UList.Item(uname);
        //        }

        //        {
        //            var withBlock1 = u;
        //            if (withBlock1.IsFeatureAvailable("パイロット愛称"))
        //            {
        //                NicknameRet = withBlock1.FeatureData("パイロット愛称");
        //                idx = (short)Strings.InStr(NicknameRet, "$(愛称)");
        //                if (idx > 0)
        //                {
        //                    NicknameRet = Strings.Left(NicknameRet, idx - 1) + Data.Nickname + Strings.Mid(NicknameRet, idx + 5);
        //                }
        //            }
        //        }
        //    }

        //    // PilotのNickname()とUnitのNickname()の呼び出しが無限に続かないように
        //    // Nickname()への呼び出しは無効化
        //    if (dont_call_unit_nickname)
        //    {
        //        GeneralLib.ReplaceString(NicknameRet, "Nickname()", "");
        //        GeneralLib.ReplaceString(NicknameRet, "nickname()", "");
        //    }

        //    // 愛称内の式置換のため、デフォルトユニットを一時的に変更する
        //    u = Event_Renamed.SelectedUnitForEvent;
        //    Event_Renamed.SelectedUnitForEvent = Unit_Renamed;
        //    Expression.ReplaceSubExpression(NicknameRet);
        //    Event_Renamed.SelectedUnitForEvent = u;
        //    return NicknameRet;
        //}

        // 読み仮名
        // TODO Impl
        public string KanaName
        {
            get { return Nickname0; }
        }
        //string KanaNameRet = default;
        //        short idx;
        //        Unit u;
        //        string uname = default, vname;
        //        KanaNameRet = Data.KanaName;

        //        // 愛称変更
        //        if (Unit_Renamed is null)
        //        {
        //            Expression.ReplaceSubExpression(KanaNameRet);
        //            return default;
        //        }

        //        {
        //            var withBlock = Unit_Renamed;
        //            if (withBlock.CountPilot() > 0)
        //            {
        //                if (!ReferenceEquals(withBlock.MainPilot(), this))
        //                {
        //                    Expression.ReplaceSubExpression(KanaNameRet);
        //                    return default;
        //                }
        //            }

        //            u = Unit_Renamed;

        //            // パイロットステータスコマンド中の場合はユニットを検索する必要がある
        //            if (withBlock.Name == "ステータス表示用ダミーユニット")
        //            {
        //                // メインパイロットかどうかチェック
        //                vname = "搭乗順番[" + ID + "]";
        //                if (Expression.IsLocalVariableDefined(vname))
        //                {
        //                    // UPGRADE_WARNING: オブジェクト LocalVariableList.Item(vname).NumericValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
        //                    if (Conversions.ToBoolean(Operators.ConditionalCompareObjectNotEqual(Event_Renamed.LocalVariableList[vname].NumericValue, 1, false)))
        //                    {
        //                        return default;
        //                    }
        //                }
        //                else
        //                {
        //                    return default;
        //                }

        //                vname = "搭乗ユニット[" + ID + "]";
        //                if (Expression.IsLocalVariableDefined(vname))
        //                {
        //                    // UPGRADE_WARNING: オブジェクト LocalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
        //                    uname = Conversions.ToString(Event_Renamed.LocalVariableList[vname].StringValue);
        //                }

        //                if (string.IsNullOrEmpty(uname))
        //                {
        //                    return default;
        //                }

        //                u = SRC.UList.Item(uname);
        //            }

        //            {
        //                var withBlock1 = u;
        //                if (withBlock1.IsFeatureAvailable("パイロット読み仮名"))
        //                {
        //                    KanaNameRet = withBlock1.FeatureData("パイロット読み仮名");
        //                    idx = (short)Strings.InStr(KanaNameRet, "$(読み仮名)");
        //                    if (idx > 0)
        //                    {
        //                        KanaNameRet = Strings.Left(KanaNameRet, idx - 1) + Data.KanaName + Strings.Mid(KanaNameRet, idx + 5);
        //                    }
        //                }
        //                else if (withBlock1.IsFeatureAvailable("パイロット愛称"))
        //                {
        //                    KanaNameRet = withBlock1.FeatureData("パイロット愛称");
        //                    idx = (short)Strings.InStr(KanaNameRet, "$(愛称)");
        //                    if (idx > 0)
        //                    {
        //                        KanaNameRet = Strings.Left(KanaNameRet, idx - 1) + Data.Nickname + Strings.Mid(KanaNameRet, idx + 5);
        //                    }

        //                    KanaNameRet = GeneralLib.StrToHiragana(KanaNameRet);
        //                }
        //            }
        //        }

        //        // 読み仮名内の式置換のため、デフォルトユニットを一時的に変更する
        //        u = Event_Renamed.SelectedUnitForEvent;
        //        Event_Renamed.SelectedUnitForEvent = Unit_Renamed;
        //        Expression.ReplaceSubExpression(KanaNameRet);
        //        Event_Renamed.SelectedUnitForEvent = u;
        //        return KanaNameRet;
        //    }
        //}

        //// 性別
        //public string Sex
        //{
        //    get
        //    {
        //        string SexRet = default;
        //        SexRet = Data.Sex;
        //        if (Unit_Renamed is object)
        //        {
        //            {
        //                var withBlock = Unit_Renamed;
        //                if (withBlock.IsFeatureAvailable("性別"))
        //                {
        //                    SexRet = withBlock.FeatureData("性別");
        //                }
        //            }
        //        }

        //        return SexRet;
        //    }
        //}

        //// 搭乗するユニットのクラス
        //// UPGRADE_NOTE: Class は Class_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        //public string Class_Renamed
        //{
        //    get
        //    {
        //        string Class_RenamedRet = default;
        //        Class_RenamedRet = Data.Class_Renamed;
        //        return Class_RenamedRet;
        //    }
        //}

        //// 倒したときに得られる経験値
        //public short ExpValue
        //{
        //    get
        //    {
        //        short ExpValueRet = default;
        //        ExpValueRet = Data.ExpValue;
        //        return ExpValueRet;
        //    }
        //}

        //// 性格
        //public string Personality
        //{
        //    get
        //    {
        //        string PersonalityRet = default;
        //        PersonalityRet = Data.Personality;

        //        // ユニットに乗っている？
        //        if (Unit_Renamed is null)
        //        {
        //            return default;
        //        }

        //        {
        //            var withBlock = Unit_Renamed;
        //            // アイテム用特殊能力「性格変更」
        //            if (withBlock.IsFeatureAvailable("性格変更"))
        //            {
        //                PersonalityRet = withBlock.FeatureData("性格変更");
        //                return default;
        //            }

        //            // 追加パイロットの性格を優先させる
        //            if (!IsAdditionalPilot)
        //            {
        //                if (withBlock.CountPilot() > 0)
        //                {
        //                    if (ReferenceEquals(withBlock.Pilot(1), this))
        //                    {
        //                        PersonalityRet = withBlock.MainPilot().Data.Personality;
        //                    }
        //                }
        //            }
        //        }

        //        return PersonalityRet;
        //    }
        //}

        //// ビットマップ
        //public string get_Bitmap(bool use_orig)
        //{
        //    string BitmapRet = default;
        //    Unit u;
        //    string uname = default, vname;
        //    if (use_orig)
        //    {
        //        BitmapRet = Data.Bitmap0;
        //    }
        //    else
        //    {
        //        BitmapRet = Data.Bitmap;
        //    }

        //    // パイロット画像変更
        //    if (Unit_Renamed is null)
        //    {
        //        return default;
        //    }

        //    {
        //        var withBlock = Unit_Renamed;
        //        if (withBlock.CountPilot() > 0)
        //        {
        //            if (!ReferenceEquals(withBlock.MainPilot(), this))
        //            {
        //                return default;
        //            }
        //        }

        //        u = Unit_Renamed;

        //        // パイロットステータスコマンド中の場合はユニットを検索する必要がある
        //        if (withBlock.Name == "ステータス表示用ダミーユニット")
        //        {
        //            // メインパイロットかどうかチェック
        //            vname = "搭乗順番[" + ID + "]";
        //            if (Expression.IsLocalVariableDefined(vname))
        //            {
        //                // UPGRADE_WARNING: オブジェクト LocalVariableList.Item(vname).NumericValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
        //                if (Conversions.ToBoolean(Operators.ConditionalCompareObjectNotEqual(Event_Renamed.LocalVariableList[vname].NumericValue, 1, false)))
        //                {
        //                    return default;
        //                }
        //            }
        //            else
        //            {
        //                return default;
        //            }

        //            vname = "搭乗ユニット[" + ID + "]";
        //            if (Expression.IsLocalVariableDefined(vname))
        //            {
        //                // UPGRADE_WARNING: オブジェクト LocalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
        //                uname = Conversions.ToString(Event_Renamed.LocalVariableList[vname].StringValue);
        //            }

        //            if (string.IsNullOrEmpty(uname))
        //            {
        //                return default;
        //            }

        //            u = SRC.UList.Item(uname);
        //        }

        //        if (u.IsConditionSatisfied("パイロット画像"))
        //        {
        //            BitmapRet = GeneralLib.LIndex(u.ConditionData(argIndex2), 2);
        //        }

        //        if (u.IsFeatureAvailable("パイロット画像"))
        //        {
        //            BitmapRet = u.FeatureData("パイロット画像");
        //        }
        //    }

        //    return BitmapRet;
        //}

        // ＢＧＭ
        public string BGM => Data.BGM;

        //// メッセージタイプ
        //public string MessageType
        //{
        //    get
        //    {
        //        string MessageTypeRet = default;
        //        MessageTypeRet = Name;

        //        // パイロット能力「メッセージ」
        //        if (IsSkillAvailable("メッセージ"))
        //        {
        //            MessageTypeRet = SkillData("メッセージ");
        //        }

        //        // 能力コピーで変身した場合はメッセージもコピー元パイロットのものを使う
        //        if (Unit_Renamed is object)
        //        {
        //            {
        //                var withBlock = Unit_Renamed;
        //                if (withBlock.IsConditionSatisfied("メッセージ"))
        //                {
        //                    MessageTypeRet = GeneralLib.LIndex(withBlock.ConditionData(argIndex2), 2);
        //                }
        //            }
        //        }

        //        return MessageTypeRet;
        //    }
        //}


        //// 防御力
        //public short Defense
        //{
        //    get
        //    {
        //        short DefenseRet = default;
        //        if (Expression.IsOptionDefined("防御力成長") | Expression.IsOptionDefined("防御力レベルアップ"))
        //        {
        //            DefenseRet = (short)(100d + 5d * SkillLevel("耐久", ref_mode: ""));
        //            if (Expression.IsOptionDefined("防御力低成長"))
        //            {
        //                DefenseRet = (short)(DefenseRet + (long)(Level * (1d + 2d * SkillLevel("防御成長", ref_mode: ""))) / 2L);
        //            }
        //            else
        //            {
        //                DefenseRet = (short)(DefenseRet + Conversion.Int(Level * (1d + SkillLevel("防御成長", ref_mode: ""))));
        //            }
        //        }
        //        else
        //        {
        //            DefenseRet = (short)(100d + 5d * SkillLevel("耐久", ref_mode: ""));
        //        }

        //        return DefenseRet;
        //    }
        //}
    }
}
