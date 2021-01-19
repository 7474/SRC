using System;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Project1
{
    internal class Pilot
    {

        // Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
        // 本プログラムはフリーソフトであり、無保証です。
        // 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
        // 再頒布または改変することができます。

        // 作成されたパイロットのクラス

        // パイロットデータへのポインタ
        public PilotData Data;

        // 識別用ＩＤ
        public string ID;
        // 所属陣営
        public string Party;
        // 搭乗しているユニット
        // 未搭乗時は Nothing
        // UPGRADE_NOTE: Unit は Unit_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        public Unit Unit_Renamed;

        // 生きているかどうか
        public bool Alive;

        // Leaveしているかどうか
        public bool Away;

        // 追加パイロットかどうか
        public bool IsAdditionalPilot;

        // 追加サポートかどうか
        public bool IsAdditionalSupport;

        // サポートパイロットとして乗り込んだ時の順番
        public short SupportIndex;

        // レベル
        private short proLevel;

        // 経験値
        private short proEXP;
        // ＳＰ
        private short proSP;
        // 気力
        private short proMorale;
        // 霊力
        private short proPlana;

        // 能力値
        public short Infight;
        public short Shooting;
        public short Hit;
        public short Dodge;
        public short Technique;
        public short Intuition;
        public string Adaption;

        // 能力値の基本値
        public short InfightBase;
        public short ShootingBase;
        public short HitBase;
        public short DodgeBase;
        public short TechniqueBase;
        public short IntuitionBase;

        // 能力値の修正値

        // 特殊能力＆自ユニットによる修正
        public short InfightMod;
        public short ShootingMod;
        public short HitMod;
        public short DodgeMod;
        public short TechniqueMod;
        public short IntuitionMod;

        // 他ユニットによる支援修正
        public short InfightMod2;
        public short ShootingMod2;
        public short HitMod2;
        public short DodgeMod2;
        public short TechniqueMod2;
        public short IntuitionMod2;

        // 気力修正値
        public short MoraleMod;

        // 特殊能力
        private Collection colSkill = new Collection();


        // UPGRADE_NOTE: Class_Terminate は Class_Terminate_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        private void Class_Terminate_Renamed()
        {
            short i;

            // UPGRADE_NOTE: オブジェクト Data をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            Data = null;
            // UPGRADE_NOTE: オブジェクト Unit_Renamed をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            Unit_Renamed = null;
            {
                var withBlock = colSkill;
                var loopTo = (short)withBlock.Count;
                for (i = 1; i <= loopTo; i++)
                    withBlock.Remove(1);
            }
            // UPGRADE_NOTE: オブジェクト colSkill をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            colSkill = null;
        }

        ~Pilot()
        {
            Class_Terminate_Renamed();
        }

        // 名称

        public string Name
        {
            get
            {
                string NameRet = default;
                NameRet = Data.Name;
                return NameRet;
            }

            set
            {
                object argIndex1 = value;
                Data = SRC.PDList.Item(ref argIndex1);
                Update();
            }
        }

        // 愛称
        public string Nickname0
        {
            get
            {
                string Nickname0Ret = default;
                Nickname0Ret = Data.Nickname;
                return Nickname0Ret;
            }
        }

        public string get_Nickname(bool dont_call_unit_nickname)
        {
            string NicknameRet = default;
            short idx;
            Unit u;
            string uname = default, vname;
            NicknameRet = Nickname0;

            // 愛称変更
            if (Unit_Renamed is null)
            {
                Expression.ReplaceSubExpression(ref NicknameRet);
                return default;
            }

            {
                var withBlock = Unit_Renamed;
                if (withBlock.CountPilot() > 0)
                {
                    if (!ReferenceEquals(withBlock.MainPilot(), this))
                    {
                        Expression.ReplaceSubExpression(ref NicknameRet);
                        return default;
                    }
                }

                u = Unit_Renamed;

                // パイロットステータスコマンド中の場合はユニットを検索する必要がある
                if (withBlock.Name == "ステータス表示用ダミーユニット")
                {
                    // メインパイロットかどうかチェック
                    vname = "搭乗順番[" + ID + "]";
                    if (Expression.IsLocalVariableDefined(ref vname))
                    {
                        // UPGRADE_WARNING: オブジェクト LocalVariableList.Item(vname).NumericValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        if (Conversions.ToBoolean(Operators.ConditionalCompareObjectNotEqual(Event_Renamed.LocalVariableList[vname].NumericValue, 1, false)))
                        {
                            return default;
                        }
                    }
                    else
                    {
                        return default;
                    }

                    vname = "搭乗ユニット[" + ID + "]";
                    if (Expression.IsLocalVariableDefined(ref vname))
                    {
                        // UPGRADE_WARNING: オブジェクト LocalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        uname = Conversions.ToString(Event_Renamed.LocalVariableList[vname].StringValue);
                    }

                    if (string.IsNullOrEmpty(uname))
                    {
                        return default;
                    }

                    object argIndex1 = uname;
                    u = SRC.UList.Item(ref argIndex1);
                }

                {
                    var withBlock1 = u;
                    string argfname = "パイロット愛称";
                    if (withBlock1.IsFeatureAvailable(ref argfname))
                    {
                        object argIndex2 = "パイロット愛称";
                        NicknameRet = withBlock1.FeatureData(ref argIndex2);
                        idx = (short)Strings.InStr(NicknameRet, "$(愛称)");
                        if (idx > 0)
                        {
                            NicknameRet = Strings.Left(NicknameRet, idx - 1) + Data.Nickname + Strings.Mid(NicknameRet, idx + 5);
                        }
                    }
                }
            }

            // PilotのNickname()とUnitのNickname()の呼び出しが無限に続かないように
            // Nickname()への呼び出しは無効化
            if (dont_call_unit_nickname)
            {
                string args2 = "Nickname()";
                string args3 = "";
                GeneralLib.ReplaceString(ref NicknameRet, ref args2, ref args3);
                string args21 = "nickname()";
                string args31 = "";
                GeneralLib.ReplaceString(ref NicknameRet, ref args21, ref args31);
            }

            // 愛称内の式置換のため、デフォルトユニットを一時的に変更する
            u = Event_Renamed.SelectedUnitForEvent;
            Event_Renamed.SelectedUnitForEvent = Unit_Renamed;
            Expression.ReplaceSubExpression(ref NicknameRet);
            Event_Renamed.SelectedUnitForEvent = u;
            return NicknameRet;
        }

        // 読み仮名
        public string KanaName
        {
            get
            {
                string KanaNameRet = default;
                short idx;
                Unit u;
                string uname = default, vname;
                KanaNameRet = Data.KanaName;

                // 愛称変更
                if (Unit_Renamed is null)
                {
                    Expression.ReplaceSubExpression(ref KanaNameRet);
                    return default;
                }

                {
                    var withBlock = Unit_Renamed;
                    if (withBlock.CountPilot() > 0)
                    {
                        if (!ReferenceEquals(withBlock.MainPilot(), this))
                        {
                            Expression.ReplaceSubExpression(ref KanaNameRet);
                            return default;
                        }
                    }

                    u = Unit_Renamed;

                    // パイロットステータスコマンド中の場合はユニットを検索する必要がある
                    if (withBlock.Name == "ステータス表示用ダミーユニット")
                    {
                        // メインパイロットかどうかチェック
                        vname = "搭乗順番[" + ID + "]";
                        if (Expression.IsLocalVariableDefined(ref vname))
                        {
                            // UPGRADE_WARNING: オブジェクト LocalVariableList.Item(vname).NumericValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                            if (Conversions.ToBoolean(Operators.ConditionalCompareObjectNotEqual(Event_Renamed.LocalVariableList[vname].NumericValue, 1, false)))
                            {
                                return default;
                            }
                        }
                        else
                        {
                            return default;
                        }

                        vname = "搭乗ユニット[" + ID + "]";
                        if (Expression.IsLocalVariableDefined(ref vname))
                        {
                            // UPGRADE_WARNING: オブジェクト LocalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                            uname = Conversions.ToString(Event_Renamed.LocalVariableList[vname].StringValue);
                        }

                        if (string.IsNullOrEmpty(uname))
                        {
                            return default;
                        }

                        object argIndex1 = uname;
                        u = SRC.UList.Item(ref argIndex1);
                    }

                    {
                        var withBlock1 = u;
                        string argfname = "パイロット読み仮名";
                        string argfname1 = "パイロット愛称";
                        if (withBlock1.IsFeatureAvailable(ref argfname))
                        {
                            object argIndex2 = "パイロット読み仮名";
                            KanaNameRet = withBlock1.FeatureData(ref argIndex2);
                            idx = (short)Strings.InStr(KanaNameRet, "$(読み仮名)");
                            if (idx > 0)
                            {
                                KanaNameRet = Strings.Left(KanaNameRet, idx - 1) + Data.KanaName + Strings.Mid(KanaNameRet, idx + 5);
                            }
                        }
                        else if (withBlock1.IsFeatureAvailable(ref argfname1))
                        {
                            object argIndex3 = "パイロット愛称";
                            KanaNameRet = withBlock1.FeatureData(ref argIndex3);
                            idx = (short)Strings.InStr(KanaNameRet, "$(愛称)");
                            if (idx > 0)
                            {
                                KanaNameRet = Strings.Left(KanaNameRet, idx - 1) + Data.Nickname + Strings.Mid(KanaNameRet, idx + 5);
                            }

                            KanaNameRet = GeneralLib.StrToHiragana(ref KanaNameRet);
                        }
                    }
                }

                // 読み仮名内の式置換のため、デフォルトユニットを一時的に変更する
                u = Event_Renamed.SelectedUnitForEvent;
                Event_Renamed.SelectedUnitForEvent = Unit_Renamed;
                Expression.ReplaceSubExpression(ref KanaNameRet);
                Event_Renamed.SelectedUnitForEvent = u;
                return KanaNameRet;
            }
        }

        // 性別
        public string Sex
        {
            get
            {
                string SexRet = default;
                SexRet = Data.Sex;
                if (Unit_Renamed is object)
                {
                    {
                        var withBlock = Unit_Renamed;
                        string argfname = "性別";
                        if (withBlock.IsFeatureAvailable(ref argfname))
                        {
                            object argIndex1 = "性別";
                            SexRet = withBlock.FeatureData(ref argIndex1);
                        }
                    }
                }

                return SexRet;
            }
        }

        // 搭乗するユニットのクラス
        // UPGRADE_NOTE: Class は Class_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        public string Class_Renamed
        {
            get
            {
                string Class_RenamedRet = default;
                Class_RenamedRet = Data.Class_Renamed;
                return Class_RenamedRet;
            }
        }

        // 倒したときに得られる経験値
        public short ExpValue
        {
            get
            {
                short ExpValueRet = default;
                ExpValueRet = Data.ExpValue;
                return ExpValueRet;
            }
        }

        // 性格
        public string Personality
        {
            get
            {
                string PersonalityRet = default;
                PersonalityRet = Data.Personality;

                // ユニットに乗っている？
                if (Unit_Renamed is null)
                {
                    return default;
                }

                {
                    var withBlock = Unit_Renamed;
                    // アイテム用特殊能力「性格変更」
                    string argfname = "性格変更";
                    if (withBlock.IsFeatureAvailable(ref argfname))
                    {
                        object argIndex1 = "性格変更";
                        PersonalityRet = withBlock.FeatureData(ref argIndex1);
                        return default;
                    }

                    // 追加パイロットの性格を優先させる
                    if (!IsAdditionalPilot)
                    {
                        if (withBlock.CountPilot() > 0)
                        {
                            object argIndex2 = 1;
                            if (ReferenceEquals(withBlock.Pilot(ref argIndex2), this))
                            {
                                PersonalityRet = withBlock.MainPilot().Data.Personality;
                            }
                        }
                    }
                }

                return PersonalityRet;
            }
        }

        // ビットマップ
        public string get_Bitmap(bool use_orig)
        {
            string BitmapRet = default;
            Unit u;
            string uname = default, vname;
            if (use_orig)
            {
                BitmapRet = Data.Bitmap0;
            }
            else
            {
                BitmapRet = Data.Bitmap;
            }

            // パイロット画像変更
            if (Unit_Renamed is null)
            {
                return default;
            }

            {
                var withBlock = Unit_Renamed;
                if (withBlock.CountPilot() > 0)
                {
                    if (!ReferenceEquals(withBlock.MainPilot(), this))
                    {
                        return default;
                    }
                }

                u = Unit_Renamed;

                // パイロットステータスコマンド中の場合はユニットを検索する必要がある
                if (withBlock.Name == "ステータス表示用ダミーユニット")
                {
                    // メインパイロットかどうかチェック
                    vname = "搭乗順番[" + ID + "]";
                    if (Expression.IsLocalVariableDefined(ref vname))
                    {
                        // UPGRADE_WARNING: オブジェクト LocalVariableList.Item(vname).NumericValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        if (Conversions.ToBoolean(Operators.ConditionalCompareObjectNotEqual(Event_Renamed.LocalVariableList[vname].NumericValue, 1, false)))
                        {
                            return default;
                        }
                    }
                    else
                    {
                        return default;
                    }

                    vname = "搭乗ユニット[" + ID + "]";
                    if (Expression.IsLocalVariableDefined(ref vname))
                    {
                        // UPGRADE_WARNING: オブジェクト LocalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        uname = Conversions.ToString(Event_Renamed.LocalVariableList[vname].StringValue);
                    }

                    if (string.IsNullOrEmpty(uname))
                    {
                        return default;
                    }

                    object argIndex1 = uname;
                    u = SRC.UList.Item(ref argIndex1);
                }

                object argIndex3 = "パイロット画像";
                if (u.IsConditionSatisfied(ref argIndex3))
                {
                    object argIndex2 = "パイロット画像";
                    string arglist = u.ConditionData(ref argIndex2);
                    BitmapRet = GeneralLib.LIndex(ref arglist, 2);
                }

                string argfname = "パイロット画像";
                if (u.IsFeatureAvailable(ref argfname))
                {
                    object argIndex4 = "パイロット画像";
                    BitmapRet = u.FeatureData(ref argIndex4);
                }
            }

            return BitmapRet;
        }

        // ＢＧＭ
        public string BGM
        {
            get
            {
                string BGMRet = default;
                BGMRet = Data.BGM;
                return BGMRet;
            }
        }

        // メッセージタイプ
        public string MessageType
        {
            get
            {
                string MessageTypeRet = default;
                MessageTypeRet = Name;

                // パイロット能力「メッセージ」
                string argsname = "メッセージ";
                if (IsSkillAvailable(ref argsname))
                {
                    object argIndex1 = "メッセージ";
                    MessageTypeRet = SkillData(ref argIndex1);
                }

                // 能力コピーで変身した場合はメッセージもコピー元パイロットのものを使う
                if (Unit_Renamed is object)
                {
                    {
                        var withBlock = Unit_Renamed;
                        object argIndex3 = "メッセージ";
                        if (withBlock.IsConditionSatisfied(ref argIndex3))
                        {
                            object argIndex2 = "メッセージ";
                            string arglist = withBlock.ConditionData(ref argIndex2);
                            MessageTypeRet = GeneralLib.LIndex(ref arglist, 2);
                        }
                    }
                }

                return MessageTypeRet;
            }
        }


        // 防御力
        public short Defense
        {
            get
            {
                short DefenseRet = default;
                string argoname1 = "防御力成長";
                string argoname2 = "防御力レベルアップ";
                if (Expression.IsOptionDefined(ref argoname1) | Expression.IsOptionDefined(ref argoname2))
                {
                    object argIndex1 = "耐久";
                    string argref_mode = "";
                    DefenseRet = (short)(100d + 5d * SkillLevel(ref argIndex1, ref_mode: ref argref_mode));
                    string argoname = "防御力低成長";
                    if (Expression.IsOptionDefined(ref argoname))
                    {
                        object argIndex2 = "防御成長";
                        string argref_mode1 = "";
                        DefenseRet = (short)(DefenseRet + (long)(Level * (1d + 2d * SkillLevel(ref argIndex2, ref_mode: ref argref_mode1))) / 2L);
                    }
                    else
                    {
                        object argIndex3 = "防御成長";
                        string argref_mode2 = "";
                        DefenseRet = (short)(DefenseRet + Conversion.Int(Level * (1d + SkillLevel(ref argIndex3, ref_mode: ref argref_mode2))));
                    }
                }
                else
                {
                    object argIndex4 = "耐久";
                    string argref_mode3 = "";
                    DefenseRet = (short)(100d + 5d * SkillLevel(ref argIndex4, ref_mode: ref argref_mode3));
                }

                return DefenseRet;
            }
        }


        // === レベル＆経験値関連処理 ===

        // レベル

        public short Level
        {
            get
            {
                short LevelRet = default;
                LevelRet = proLevel;
                return LevelRet;
            }

            set
            {
                if (proLevel == value)
                {
                    // 変化なし
                    return;
                }

                proLevel = value;
                Update();
            }
        }

        // 経験値

        public int Exp
        {
            get
            {
                int ExpRet = default;
                ExpRet = proEXP;
                return ExpRet;
            }

            set
            {
                short prev_level;
                prev_level = proLevel;

                // 500ごとにレベルアップ
                proEXP = (short)(value % 500);
                proLevel = (short)(proLevel + value / 500);

                // 経験値が下がる場合はレベルを下げる
                if (proEXP < 0)
                {
                    if (proLevel > 1)
                    {
                        proEXP = (short)(proEXP + 500);
                        proLevel = (short)(proLevel - 1);
                    }
                    else
                    {
                        // これ以上はレベルを下げられないので
                        proEXP = 0;
                    }
                }

                // レベル上限チェック
                if (value / 500 > 0)
                {
                    string argoname = "レベル限界突破";
                    if (Expression.IsOptionDefined(ref argoname))
                    {
                        if (proLevel > 999) // レベル999で打ち止め
                        {
                            proLevel = 999;
                            proEXP = 500;
                        }
                    }
                    else if (proLevel > 99) // レベル99で打ち止め
                    {
                        proLevel = 99;
                        proEXP = 500;
                    }
                }

                if (prev_level != proLevel)
                {
                    Update();
                }
            }
        }


        // 気力

        public short Morale
        {
            get
            {
                short MoraleRet = default;
                MoraleRet = proMorale;
                return MoraleRet;
            }

            set
            {
                SetMorale(value);
            }
        }

        public short MaxMorale
        {
            get
            {
                short MaxMoraleRet = default;
                MaxMoraleRet = 150;
                string argsname = "気力上限";
                if (IsSkillAvailable(ref argsname))
                {
                    object argIndex2 = "気力上限";
                    if (IsSkillLevelSpecified(ref argIndex2))
                    {
                        object argIndex1 = "気力上限";
                        string argref_mode = "";
                        MaxMoraleRet = (short)GeneralLib.MaxLng((int)SkillLevel(ref argIndex1, ref_mode: ref argref_mode), 0);
                    }
                }

                return MaxMoraleRet;
            }
        }

        public short MinMorale
        {
            get
            {
                short MinMoraleRet = default;
                MinMoraleRet = 50;
                string argsname = "気力下限";
                if (IsSkillAvailable(ref argsname))
                {
                    object argIndex2 = "気力下限";
                    if (IsSkillLevelSpecified(ref argIndex2))
                    {
                        object argIndex1 = "気力下限";
                        string argref_mode = "";
                        MinMoraleRet = (short)GeneralLib.MaxLng((int)SkillLevel(ref argIndex1, ref_mode: ref argref_mode), 0);
                    }
                }

                return MinMoraleRet;
            }
        }


        // === ＳＰ値関連処理 ===

        // 最大ＳＰ
        public int MaxSP
        {
            get
            {
                int MaxSPRet = default;
                short lv;

                // ＳＰなしの場合はレベルに関わらず0
                if (Data.SP <= 0)
                {
                    MaxSPRet = 0;
                    // ただし追加パイロットの場合は第１パイロットの最大ＳＰを使用
                    if (Unit_Renamed is object)
                    {
                        {
                            var withBlock = Unit_Renamed;
                            if (withBlock.CountPilot() > 0)
                            {
                                object argIndex2 = 1;
                                object argIndex3 = 1;
                                if (!ReferenceEquals(withBlock.Pilot(ref argIndex3), this))
                                {
                                    if (ReferenceEquals(withBlock.MainPilot(), this))
                                    {
                                        object argIndex1 = 1;
                                        MaxSPRet = withBlock.Pilot(ref argIndex1).MaxSP;
                                    }
                                }
                            }
                        }
                    }

                    return default;
                }

                // レベルによる上昇値
                lv = Level;
                if (lv > 99)
                {
                    lv = 100;
                }

                object argIndex4 = "追加レベル";
                string argref_mode = "";
                lv = (short)(lv + SkillLevel(ref argIndex4, ref_mode: ref argref_mode));
                if (lv > 40)
                {
                    MaxSPRet = lv + 40;
                }
                else
                {
                    MaxSPRet = 2 * lv;
                }

                string argsname = "ＳＰ低成長";
                string argsname1 = "ＳＰ高成長";
                if (IsSkillAvailable(ref argsname))
                {
                    MaxSPRet = MaxSPRet / 2;
                }
                else if (IsSkillAvailable(ref argsname1))
                {
                    MaxSPRet = (int)(1.5d * MaxSPRet);
                }

                string argoname = "ＳＰ低成長";
                if (Expression.IsOptionDefined(ref argoname))
                {
                    MaxSPRet = MaxSPRet / 2;
                }

                // 基本値を追加
                MaxSPRet = MaxSPRet + Data.SP;

                // 能力ＵＰ
                object argIndex5 = "ＳＰＵＰ";
                string argref_mode1 = "";
                MaxSPRet = (int)(MaxSPRet + SkillLevel(ref argIndex5, ref_mode: ref argref_mode1));

                // 能力ＤＯＷＮ
                object argIndex6 = "ＳＰＤＯＷＮ";
                string argref_mode2 = "";
                MaxSPRet = (int)(MaxSPRet - SkillLevel(ref argIndex6, ref_mode: ref argref_mode2));

                // 上限を超えないように
                if (MaxSPRet > 9999)
                {
                    MaxSPRet = 9999;
                }

                return MaxSPRet;
            }
        }

        // ＳＰ値

        public int SP
        {
            get
            {
                int SPRet = default;
                SPRet = proSP;

                // 追加パイロットかどうか判定

                if (Unit_Renamed is null)
                {
                    return default;
                }

                {
                    var withBlock = Unit_Renamed;
                    if (withBlock.CountPilot() == 0)
                    {
                        return default;
                    }

                    object argIndex1 = 1;
                    if (ReferenceEquals(withBlock.Pilot(ref argIndex1), this))
                    {
                        return default;
                    }

                    if (!ReferenceEquals(withBlock.MainPilot(), this))
                    {
                        return default;
                    }

                    // 追加パイロットだったので第１パイロットのＳＰ値を代わりに使う
                    if (Data.SP > 0)
                    {
                        // ＳＰを持つ場合は消費量を一致させる
                        object argIndex2 = 1;
                        {
                            var withBlock1 = withBlock.Pilot(ref argIndex2);
                            if (withBlock1.MaxSP > 0)
                            {
                                proSP = (short)(MaxSP * withBlock1.SP0 / withBlock1.MaxSP);
                                SPRet = proSP;
                            }
                        }
                    }
                    else
                    {
                        // ＳＰを持たない場合はそのまま使う
                        object argIndex3 = 1;
                        SPRet = withBlock.Pilot(ref argIndex3).SP0;
                    }
                }

                return SPRet;
            }

            set
            {
                int prev_sp;
                prev_sp = proSP;
                if (value > MaxSP)
                {
                    proSP = (short)MaxSP;
                }
                else if (value < 0)
                {
                    proSP = 0;
                }
                else
                {
                    proSP = (short)value;
                }

                // 追加パイロットかどうか判定

                if (Unit_Renamed is null)
                {
                    return;
                }

                {
                    var withBlock = Unit_Renamed;
                    if (withBlock.CountPilot() == 0)
                    {
                        return;
                    }

                    object argIndex1 = 1;
                    if (ReferenceEquals(withBlock.Pilot(ref argIndex1), this))
                    {
                        return;
                    }

                    if (!ReferenceEquals(withBlock.MainPilot(), this))
                    {
                        return;
                    }

                    // 追加パイロットだったので第１パイロットのＳＰ値を代わりに使う
                    object argIndex2 = 1;
                    {
                        var withBlock1 = withBlock.Pilot(ref argIndex2);
                        if (Data.SP > 0)
                        {
                            // 追加パイロットがＳＰを持つ場合は第１パイロットと消費率を一致させる
                            if (withBlock1.MaxSP > 0)
                            {
                                withBlock1.SP0 = withBlock1.MaxSP * proSP / MaxSP;
                                proSP = (short)(MaxSP * withBlock1.SP0 / withBlock1.MaxSP);
                            }
                        }
                        // 追加パイロットがＳＰを持たない場合は第１パイロットのＳＰ値をそのまま使う
                        else if (value > withBlock1.MaxSP)
                        {
                            withBlock1.SP0 = withBlock1.MaxSP;
                        }
                        else if (value < 0)
                        {
                            withBlock1.SP0 = 0;
                        }
                        else
                        {
                            withBlock1.SP0 = value;
                        }
                    }
                }
            }
        }

        public int SP0
        {
            get
            {
                int SP0Ret = default;
                SP0Ret = proSP;
                return SP0Ret;
            }

            set
            {
                proSP = (short)value;
            }
        }

        // 霊力

        public int Plana
        {
            get
            {
                int PlanaRet = default;
                string argsname = "霊力";
                if (IsSkillAvailable(ref argsname))
                {
                    PlanaRet = proPlana;
                }

                // 追加パイロットかどうか判定

                if (Unit_Renamed is null)
                {
                    return default;
                }

                {
                    var withBlock = Unit_Renamed;
                    if (withBlock.CountPilot() == 0)
                    {
                        return default;
                    }

                    object argIndex1 = 1;
                    if (ReferenceEquals(withBlock.Pilot(ref argIndex1), this))
                    {
                        return default;
                    }

                    if (!ReferenceEquals(withBlock.MainPilot(), this))
                    {
                        return default;
                    }

                    // 追加パイロットだったので第１パイロットの霊力を代わりに使う
                    string argsname1 = "霊力";
                    if (IsSkillAvailable(ref argsname1))
                    {
                        object argIndex2 = 1;
                        {
                            var withBlock1 = withBlock.Pilot(ref argIndex2);
                            if (withBlock1.MaxPlana() > 0)
                            {
                                proPlana = (short)(MaxPlana() * withBlock1.Plana0 / withBlock1.MaxPlana());
                                PlanaRet = proPlana;
                            }
                        }
                    }
                    else
                    {
                        object argIndex3 = 1;
                        PlanaRet = withBlock.Pilot(ref argIndex3).Plana0;
                    }
                }

                return PlanaRet;
            }

            set
            {
                int prev_plana;
                prev_plana = proPlana;
                if (value > MaxPlana())
                {
                    proPlana = (short)MaxPlana();
                }
                else if (value < 0)
                {
                    proPlana = 0;
                }
                else
                {
                    proPlana = (short)value;
                }

                // 追加パイロットかどうか判定

                if (Unit_Renamed is null)
                {
                    return;
                }

                {
                    var withBlock = Unit_Renamed;
                    if (withBlock.CountPilot() == 0)
                    {
                        return;
                    }

                    object argIndex1 = 1;
                    if (ReferenceEquals(withBlock.Pilot(ref argIndex1), this))
                    {
                        return;
                    }

                    if (!ReferenceEquals(withBlock.MainPilot(), this))
                    {
                        return;
                    }

                    // 追加パイロットだったので第１パイロットの霊力値を代わりに使う
                    object argIndex2 = 1;
                    {
                        var withBlock1 = withBlock.Pilot(ref argIndex2);
                        string argsname = "霊力";
                        if (IsSkillAvailable(ref argsname))
                        {
                            // 追加パイロットが霊力を持つ場合は第１パイロットと消費率を一致させる
                            if (withBlock1.MaxSP > 0)
                            {
                                withBlock1.Plana0 = withBlock1.MaxPlana() * proPlana / MaxPlana();
                                proPlana = (short)(MaxPlana() * withBlock1.Plana0 / withBlock1.MaxPlana());
                            }
                        }
                        // 追加パイロットが霊力を持たない場合は第１パイロットの霊力値をそのまま使う
                        else if (value > withBlock1.MaxPlana())
                        {
                            withBlock1.Plana0 = withBlock1.MaxPlana();
                        }
                        else if (value < 0)
                        {
                            withBlock1.Plana0 = 0;
                        }
                        else
                        {
                            withBlock1.Plana0 = value;
                        }
                    }
                }
            }
        }

        public int Plana0
        {
            get
            {
                int Plana0Ret = default;
                Plana0Ret = proPlana;
                return Plana0Ret;
            }

            set
            {
                proPlana = (short)value;
            }
        }


        // === スペシャルパワー関連処理 ===

        // スペシャルパワーの個数
        public short CountSpecialPower
        {
            get
            {
                short CountSpecialPowerRet = default;
                if (Data.SP <= 0)
                {
                    // ＳＰを持たない追加パイロットの場合は１番目のパイロットのデータを使う
                    if (Unit_Renamed is object)
                    {
                        {
                            var withBlock = Unit_Renamed;
                            if (withBlock.CountPilot() > 0)
                            {
                                object argIndex2 = 1;
                                object argIndex3 = 1;
                                if (!ReferenceEquals(withBlock.Pilot(ref argIndex3), this))
                                {
                                    if (ReferenceEquals(withBlock.MainPilot(), this))
                                    {
                                        object argIndex1 = 1;
                                        CountSpecialPowerRet = withBlock.Pilot(ref argIndex1).Data.CountSpecialPower(Level);
                                        return default;
                                    }
                                }
                            }
                        }
                    }
                }

                CountSpecialPowerRet = Data.CountSpecialPower(Level);
                return CountSpecialPowerRet;
            }
        }

        // idx番目のスペシャルパワー
        public string get_SpecialPower(short idx)
        {
            string SpecialPowerRet = default;
            if (Data.SP <= 0)
            {
                // ＳＰを持たない追加パイロットの場合は１番目のパイロットのデータを使う
                if (Unit_Renamed is object)
                {
                    {
                        var withBlock = Unit_Renamed;
                        if (withBlock.CountPilot() > 0)
                        {
                            object argIndex2 = 1;
                            object argIndex3 = 1;
                            if (!ReferenceEquals(withBlock.Pilot(ref argIndex3), this))
                            {
                                if (ReferenceEquals(withBlock.MainPilot(), this))
                                {
                                    object argIndex1 = 1;
                                    SpecialPowerRet = withBlock.Pilot(ref argIndex1).Data.SpecialPower(Level, idx);
                                    return default;
                                }
                            }
                        }
                    }
                }
            }

            SpecialPowerRet = Data.SpecialPower(Level, idx);
            return SpecialPowerRet;
        }


        // 能力値を更新
        public void Update()
        {
            short skill_num;
            var skill_name = new string[65];
            var skill_data = new SkillData[65];
            short i, j;
            double lv;
            SkillData sd;

            // 現在のレベルで使用可能な特殊能力の一覧を作成

            // 以前の一覧を削除
            {
                var withBlock = colSkill;
                var loopTo = (short)withBlock.Count;
                for (i = 1; i <= loopTo; i++)
                    withBlock.Remove(1);
            }

            // パイロットデータを参照しながら使用可能な特殊能力を検索
            skill_num = 0;
            foreach (SkillData currentSd in Data.colSkill)
            {
                sd = currentSd;
                {
                    var withBlock1 = sd;
                    if (Level >= withBlock1.NecessaryLevel)
                    {
                        // 既に登録済み？
                        if (withBlock1.Name == "ＳＰ消費減少" | withBlock1.Name == "スペシャルパワー自動発動" | withBlock1.Name == "ハンター")
                        {
                            // これらの特殊能力は同種の能力を複数持つことが出来る
                            var loopTo1 = skill_num;
                            for (i = 1; i <= loopTo1; i++)
                            {
                                if ((withBlock1.Name ?? "") == (skill_name[i] ?? ""))
                                {
                                    if ((withBlock1.StrData ?? "") == (skill_data[i].StrData ?? ""))
                                    {
                                        // ただしデータ指定まで同一であれば同じ能力と見なす
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            var loopTo2 = skill_num;
                            for (i = 1; i <= loopTo2; i++)
                            {
                                if ((withBlock1.Name ?? "") == (skill_name[i] ?? ""))
                                {
                                    break;
                                }
                            }
                        }

                        if (i > skill_num)
                        {
                            // 未登録
                            skill_num = (short)(skill_num + 1);
                            skill_name[skill_num] = withBlock1.Name;
                            skill_data[skill_num] = sd;
                        }
                        else if (withBlock1.NecessaryLevel > skill_data[i].NecessaryLevel)
                        {
                            // 登録済みである場合は習得レベルが高いものを優先
                            skill_data[i] = sd;
                        }
                    }
                }
            }

            // SetSkillコマンドで付加された特殊能力を検索
            string sname, alist, sdata;
            string buf;
            string argvname = "Ability(" + ID + ")";
            if (Expression.IsGlobalVariableDefined(ref argvname))
            {

                // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                alist = Conversions.ToString(Event_Renamed.GlobalVariableList["Ability(" + ID + ")"].StringValue);
                var loopTo3 = GeneralLib.LLength(ref alist);
                for (i = 1; i <= loopTo3; i++)
                {
                    sname = GeneralLib.LIndex(ref alist, i);
                    // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    buf = Conversions.ToString(Event_Renamed.GlobalVariableList["Ability(" + ID + "," + sname + ")"].StringValue);
                    sdata = GeneralLib.ListTail(ref buf, 2);

                    // 既に登録済み？
                    if (sname == "ＳＰ消費減少" | sname == "スペシャルパワー自動発動" | sname == "ハンター")
                    {
                        // これらの特殊能力は同種の能力を複数持つことが出来る
                        var loopTo4 = skill_num;
                        for (j = 1; j <= loopTo4; j++)
                        {
                            if ((sname ?? "") == (skill_name[j] ?? ""))
                            {
                                if ((sdata ?? "") == (skill_data[j].StrData ?? ""))
                                {
                                    // ただしデータ指定まで同一であれば同じ能力と見なす
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        var loopTo5 = skill_num;
                        for (j = 1; j <= loopTo5; j++)
                        {
                            if ((sname ?? "") == (skill_name[j] ?? ""))
                            {
                                break;
                            }
                        }
                    }

                    if (j > skill_num)
                    {
                        // 未登録
                        skill_num = j;
                        skill_name[j] = sname;
                    }

                    string argexpr1 = GeneralLib.LIndex(ref buf, 1);
                    if (GeneralLib.StrToDbl(ref argexpr1) == 0d)
                    {
                        // レベル0の場合は能力を封印
                        // UPGRADE_NOTE: オブジェクト skill_data() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                        skill_data[j] = null;
                    }
                    else
                    {
                        // PDListのデータを書き換えるわけにはいかないので
                        // アビリティデータを新規に作成
                        sd = new SkillData();
                        sd.Name = sname;
                        string argexpr = GeneralLib.LIndex(ref buf, 1);
                        sd.Level = GeneralLib.StrToDbl(ref argexpr);
                        if (sd.Level == -1)
                        {
                            sd.Level = SRC.DEFAULT_LEVEL;
                        }

                        sd.StrData = GeneralLib.ListTail(ref buf, 2);
                        skill_data[j] = sd;
                    }
                }
            }

            // 属性使用不能状態の際、対応する技能を封印する。
            if (Unit_Renamed is object)
            {
                var loopTo6 = skill_num;
                for (j = 1; j <= loopTo6; j++)
                {
                    if (skill_data[j] is object)
                    {
                        object argIndex1 = skill_data[j].Name + "使用不能";
                        if (Unit_Renamed.ConditionLifetime(ref argIndex1) > 0)
                        {
                            // UPGRADE_NOTE: オブジェクト skill_data() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                            skill_data[j] = null;
                        }
                    }
                }
            }

            // 使用可能な特殊能力を登録
            {
                var withBlock2 = colSkill;
                var loopTo7 = skill_num;
                for (i = 1; i <= loopTo7; i++)
                {
                    if (skill_data[i] is object)
                    {
                        switch (skill_name[i] ?? "")
                        {
                            case "ＳＰ消費減少":
                            case "スペシャルパワー自動発動":
                            case "ハンター":
                                {
                                    var loopTo8 = (short)(i - 1);
                                    for (j = 1; j <= loopTo8; j++)
                                    {
                                        if ((skill_name[i] ?? "") == (skill_name[j] ?? ""))
                                        {
                                            break;
                                        }
                                    }

                                    if (j >= i)
                                    {
                                        withBlock2.Add(skill_data[i], skill_name[i]);
                                    }
                                    else
                                    {
                                        withBlock2.Add(skill_data[i], skill_name[i] + ":" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(i));
                                    }

                                    break;
                                }

                            default:
                                {
                                    withBlock2.Add(skill_data[i], skill_name[i]);
                                    break;
                                }
                        }
                    }
                }
            }

            // これから下は能力値の計算

            // 基本値
            {
                var withBlock3 = Data;
                InfightBase = withBlock3.Infight;
                ShootingBase = withBlock3.Shooting;
                HitBase = withBlock3.Hit;
                DodgeBase = withBlock3.Dodge;
                TechniqueBase = withBlock3.Technique;
                IntuitionBase = withBlock3.Intuition;
                Adaption = withBlock3.Adaption;
            }

            // レベルによる追加分
            object argIndex2 = "追加レベル";
            string argref_mode = "";
            lv = Level + SkillLevel(ref argIndex2, ref_mode: ref argref_mode);
            string argoname = "攻撃力低成長";
            if (Expression.IsOptionDefined(ref argoname))
            {
                object argIndex3 = "格闘成長";
                string argref_mode1 = "";
                InfightBase = (short)(InfightBase + (long)(lv * (1d + 2d * SkillLevel(ref argIndex3, ref_mode: ref argref_mode1))) / 2L);
                object argIndex4 = "射撃成長";
                string argref_mode2 = "";
                ShootingBase = (short)(ShootingBase + (long)(lv * (1d + 2d * SkillLevel(ref argIndex4, ref_mode: ref argref_mode2))) / 2L);
            }
            else
            {
                object argIndex5 = "格闘成長";
                string argref_mode3 = "";
                InfightBase = (short)(InfightBase + Conversion.Int(lv * (1d + SkillLevel(ref argIndex5, ref_mode: ref argref_mode3))));
                object argIndex6 = "射撃成長";
                string argref_mode4 = "";
                ShootingBase = (short)(ShootingBase + Conversion.Int(lv * (1d + SkillLevel(ref argIndex6, ref_mode: ref argref_mode4))));
            }

            object argIndex7 = "命中成長";
            string argref_mode5 = "";
            HitBase = (short)(HitBase + Conversion.Int(lv * (2d + SkillLevel(ref argIndex7, ref_mode: ref argref_mode5))));
            object argIndex8 = "回避成長";
            string argref_mode6 = "";
            DodgeBase = (short)(DodgeBase + Conversion.Int(lv * (2d + SkillLevel(ref argIndex8, ref_mode: ref argref_mode6))));
            object argIndex9 = "技量成長";
            string argref_mode7 = "";
            TechniqueBase = (short)(TechniqueBase + Conversion.Int(lv * (1d + SkillLevel(ref argIndex9, ref_mode: ref argref_mode7))));
            object argIndex10 = "反応成長";
            string argref_mode8 = "";
            IntuitionBase = (short)(IntuitionBase + Conversion.Int(lv * (1d + SkillLevel(ref argIndex10, ref_mode: ref argref_mode8))));

            // 能力ＵＰ
            object argIndex11 = "格闘ＵＰ";
            string argref_mode9 = "";
            InfightBase = (short)(InfightBase + SkillLevel(ref argIndex11, ref_mode: ref argref_mode9));
            object argIndex12 = "射撃ＵＰ";
            string argref_mode10 = "";
            ShootingBase = (short)(ShootingBase + SkillLevel(ref argIndex12, ref_mode: ref argref_mode10));
            object argIndex13 = "命中ＵＰ";
            string argref_mode11 = "";
            HitBase = (short)(HitBase + SkillLevel(ref argIndex13, ref_mode: ref argref_mode11));
            object argIndex14 = "回避ＵＰ";
            string argref_mode12 = "";
            DodgeBase = (short)(DodgeBase + SkillLevel(ref argIndex14, ref_mode: ref argref_mode12));
            object argIndex15 = "技量ＵＰ";
            string argref_mode13 = "";
            TechniqueBase = (short)(TechniqueBase + SkillLevel(ref argIndex15, ref_mode: ref argref_mode13));
            object argIndex16 = "反応ＵＰ";
            string argref_mode14 = "";
            IntuitionBase = (short)(IntuitionBase + SkillLevel(ref argIndex16, ref_mode: ref argref_mode14));

            // 能力ＤＯＷＮ
            object argIndex17 = "格闘ＤＯＷＮ";
            string argref_mode15 = "";
            InfightBase = (short)(InfightBase - SkillLevel(ref argIndex17, ref_mode: ref argref_mode15));
            object argIndex18 = "射撃ＤＯＷＮ";
            string argref_mode16 = "";
            ShootingBase = (short)(ShootingBase - SkillLevel(ref argIndex18, ref_mode: ref argref_mode16));
            object argIndex19 = "命中ＤＯＷＮ";
            string argref_mode17 = "";
            HitBase = (short)(HitBase - SkillLevel(ref argIndex19, ref_mode: ref argref_mode17));
            object argIndex20 = "回避ＤＯＷＮ";
            string argref_mode18 = "";
            DodgeBase = (short)(DodgeBase - SkillLevel(ref argIndex20, ref_mode: ref argref_mode18));
            object argIndex21 = "技量ＤＯＷＮ";
            string argref_mode19 = "";
            TechniqueBase = (short)(TechniqueBase - SkillLevel(ref argIndex21, ref_mode: ref argref_mode19));
            object argIndex22 = "反応ＤＯＷＮ";
            string argref_mode20 = "";
            IntuitionBase = (short)(IntuitionBase - SkillLevel(ref argIndex22, ref_mode: ref argref_mode20));

            // 上限を超えないように
            InfightBase = (short)GeneralLib.MinLng(InfightBase, 9999);
            ShootingBase = (short)GeneralLib.MinLng(ShootingBase, 9999);
            HitBase = (short)GeneralLib.MinLng(HitBase, 9999);
            DodgeBase = (short)GeneralLib.MinLng(DodgeBase, 9999);
            TechniqueBase = (short)GeneralLib.MinLng(TechniqueBase, 9999);
            IntuitionBase = (short)GeneralLib.MinLng(IntuitionBase, 9999);

            // これから下は特殊能力による修正値の計算

            // まずは修正値を初期化
            InfightMod = 0;
            ShootingMod = 0;
            HitMod = 0;
            DodgeMod = 0;
            TechniqueMod = 0;
            IntuitionMod = 0;

            // パイロット用特殊能力による修正

            object argIndex23 = "超感覚";
            string argref_mode21 = "";
            lv = SkillLevel(ref argIndex23, ref_mode: ref argref_mode21);
            if (lv > 0d)
            {
                HitMod = (short)(HitMod + 2d * lv + 3d);
                DodgeMod = (short)(DodgeMod + 2d * lv + 3d);
            }

            object argIndex24 = "知覚強化";
            string argref_mode22 = "";
            lv = SkillLevel(ref argIndex24, ref_mode: ref argref_mode22);
            if (lv > 0d)
            {
                HitMod = (short)(HitMod + 2d * lv + 3d);
                DodgeMod = (short)(DodgeMod + 2d * lv + 3d);
            }

            object argIndex25 = "念力";
            string argref_mode23 = "";
            lv = SkillLevel(ref argIndex25, ref_mode: ref argref_mode23);
            if (lv > 0d)
            {
                HitMod = (short)(HitMod + 2d * lv + 3d);
                DodgeMod = (short)(DodgeMod + 2d * lv + 3d);
            }

            object argIndex26 = "超反応";
            string argref_mode24 = "";
            lv = SkillLevel(ref argIndex26, ref_mode: ref argref_mode24);
            HitMod = (short)(HitMod + 2d * lv);
            DodgeMod = (short)(DodgeMod + 2d * lv);
            string argsname = "サイボーグ";
            if (IsSkillAvailable(ref argsname))
            {
                HitMod = (short)(HitMod + 5);
                DodgeMod = (short)(DodgeMod + 5);
            }

            string argsname1 = "悟り";
            if (IsSkillAvailable(ref argsname1))
            {
                HitMod = (short)(HitMod + 10);
                DodgeMod = (short)(DodgeMod + 10);
            }

            string argsname2 = "超能力";
            if (IsSkillAvailable(ref argsname2))
            {
                HitMod = (short)(HitMod + 5);
                DodgeMod = (short)(DodgeMod + 5);
            }

            // これから下はユニットによる修正値の計算

            // ユニットに乗っていない？
            if (Unit_Renamed is null)
            {
                goto SkipUnitMod;
            }

            var padaption = new short[5];
            {
                var withBlock4 = Unit_Renamed;
                // クイックセーブ処理などで実際には乗っていない場合
                if (withBlock4.CountPilot() == 0)
                {
                    return;
                }

                // サブパイロット＆サポートパイロットによるサポート
                if (ReferenceEquals(this, withBlock4.MainPilot()) & withBlock4.Status == "出撃")
                {
                    var loopTo9 = withBlock4.CountPilot();
                    for (i = 2; i <= loopTo9; i++)
                    {
                        object argIndex36 = i;
                        {
                            var withBlock5 = withBlock4.Pilot(ref argIndex36);
                            object argIndex27 = "格闘サポート";
                            string argref_mode25 = "";
                            InfightMod = (short)(InfightMod + 2d * withBlock5.SkillLevel(ref argIndex27, ref_mode: ref argref_mode25));
                            if (HasMana())
                            {
                                object argIndex28 = "魔力サポート";
                                string argref_mode26 = "";
                                ShootingMod = (short)(ShootingMod + 2d * withBlock5.SkillLevel(ref argIndex28, ref_mode: ref argref_mode26));
                            }
                            else
                            {
                                object argIndex29 = "射撃サポート";
                                string argref_mode27 = "";
                                ShootingMod = (short)(ShootingMod + 2d * withBlock5.SkillLevel(ref argIndex29, ref_mode: ref argref_mode27));
                            }

                            object argIndex30 = "サポート";
                            string argref_mode28 = "";
                            HitMod = (short)(HitMod + 3d * withBlock5.SkillLevel(ref argIndex30, ref_mode: ref argref_mode28));
                            object argIndex31 = "命中サポート";
                            string argref_mode29 = "";
                            HitMod = (short)(HitMod + 2d * withBlock5.SkillLevel(ref argIndex31, ref_mode: ref argref_mode29));
                            object argIndex32 = "サポート";
                            string argref_mode30 = "";
                            DodgeMod = (short)(DodgeMod + 3d * withBlock5.SkillLevel(ref argIndex32, ref_mode: ref argref_mode30));
                            object argIndex33 = "回避サポート";
                            string argref_mode31 = "";
                            DodgeMod = (short)(DodgeMod + 2d * withBlock5.SkillLevel(ref argIndex33, ref_mode: ref argref_mode31));
                            object argIndex34 = "技量サポート";
                            string argref_mode32 = "";
                            TechniqueMod = (short)(TechniqueMod + 2d * withBlock5.SkillLevel(ref argIndex34, ref_mode: ref argref_mode32));
                            object argIndex35 = "反応サポート";
                            string argref_mode33 = "";
                            IntuitionMod = (short)(IntuitionMod + 2d * withBlock5.SkillLevel(ref argIndex35, ref_mode: ref argref_mode33));
                        }
                    }

                    var loopTo10 = withBlock4.CountSupport();
                    for (i = 1; i <= loopTo10; i++)
                    {
                        object argIndex46 = i;
                        {
                            var withBlock6 = withBlock4.Support(ref argIndex46);
                            object argIndex37 = "格闘サポート";
                            string argref_mode34 = "";
                            InfightMod = (short)(InfightMod + 2d * withBlock6.SkillLevel(ref argIndex37, ref_mode: ref argref_mode34));
                            if (HasMana())
                            {
                                object argIndex38 = "魔力サポート";
                                string argref_mode35 = "";
                                ShootingMod = (short)(ShootingMod + 2d * withBlock6.SkillLevel(ref argIndex38, ref_mode: ref argref_mode35));
                            }
                            else
                            {
                                object argIndex39 = "射撃サポート";
                                string argref_mode36 = "";
                                ShootingMod = (short)(ShootingMod + 2d * withBlock6.SkillLevel(ref argIndex39, ref_mode: ref argref_mode36));
                            }

                            object argIndex40 = "サポート";
                            string argref_mode37 = "";
                            HitMod = (short)(HitMod + 3d * withBlock6.SkillLevel(ref argIndex40, ref_mode: ref argref_mode37));
                            object argIndex41 = "命中サポート";
                            string argref_mode38 = "";
                            HitMod = (short)(HitMod + 2d * withBlock6.SkillLevel(ref argIndex41, ref_mode: ref argref_mode38));
                            object argIndex42 = "サポート";
                            string argref_mode39 = "";
                            DodgeMod = (short)(DodgeMod + 3d * withBlock6.SkillLevel(ref argIndex42, ref_mode: ref argref_mode39));
                            object argIndex43 = "回避サポート";
                            string argref_mode40 = "";
                            DodgeMod = (short)(DodgeMod + 2d * withBlock6.SkillLevel(ref argIndex43, ref_mode: ref argref_mode40));
                            object argIndex44 = "技量サポート";
                            string argref_mode41 = "";
                            TechniqueMod = (short)(TechniqueMod + 2d * withBlock6.SkillLevel(ref argIndex44, ref_mode: ref argref_mode41));
                            object argIndex45 = "反応サポート";
                            string argref_mode42 = "";
                            IntuitionMod = (short)(IntuitionMod + 2d * withBlock6.SkillLevel(ref argIndex45, ref_mode: ref argref_mode42));
                        }
                    }

                    string argfname = "追加サポート";
                    if (withBlock4.IsFeatureAvailable(ref argfname))
                    {
                        {
                            var withBlock7 = withBlock4.AdditionalSupport();
                            object argIndex47 = "格闘サポート";
                            string argref_mode43 = "";
                            InfightMod = (short)(InfightMod + 2d * withBlock7.SkillLevel(ref argIndex47, ref_mode: ref argref_mode43));
                            if (HasMana())
                            {
                                object argIndex48 = "魔力サポート";
                                string argref_mode44 = "";
                                ShootingMod = (short)(ShootingMod + 2d * withBlock7.SkillLevel(ref argIndex48, ref_mode: ref argref_mode44));
                            }
                            else
                            {
                                object argIndex49 = "射撃サポート";
                                string argref_mode45 = "";
                                ShootingMod = (short)(ShootingMod + 2d * withBlock7.SkillLevel(ref argIndex49, ref_mode: ref argref_mode45));
                            }

                            object argIndex50 = "サポート";
                            string argref_mode46 = "";
                            HitMod = (short)(HitMod + 3d * withBlock7.SkillLevel(ref argIndex50, ref_mode: ref argref_mode46));
                            object argIndex51 = "命中サポート";
                            string argref_mode47 = "";
                            HitMod = (short)(HitMod + 2d * withBlock7.SkillLevel(ref argIndex51, ref_mode: ref argref_mode47));
                            object argIndex52 = "サポート";
                            string argref_mode48 = "";
                            DodgeMod = (short)(DodgeMod + 3d * withBlock7.SkillLevel(ref argIndex52, ref_mode: ref argref_mode48));
                            object argIndex53 = "回避サポート";
                            string argref_mode49 = "";
                            DodgeMod = (short)(DodgeMod + 2d * withBlock7.SkillLevel(ref argIndex53, ref_mode: ref argref_mode49));
                            object argIndex54 = "技量サポート";
                            string argref_mode50 = "";
                            TechniqueMod = (short)(TechniqueMod + 2d * withBlock7.SkillLevel(ref argIndex54, ref_mode: ref argref_mode50));
                            object argIndex55 = "反応サポート";
                            string argref_mode51 = "";
                            IntuitionMod = (short)(IntuitionMod + 2d * withBlock7.SkillLevel(ref argIndex55, ref_mode: ref argref_mode51));
                        }
                    }
                }

                // ユニット＆アイテムによる強化
                var loopTo11 = withBlock4.CountFeature();
                for (i = 1; i <= loopTo11; i++)
                {
                    object argIndex56 = i;
                    switch (withBlock4.Feature(ref argIndex56) ?? "")
                    {
                        case "格闘強化":
                            {
                                string localFeatureData() { object argIndex1 = i; var ret = withBlock4.FeatureData(ref argIndex1); return ret; }

                                string localLIndex() { string arglist = hs2cde20588fb24d85bea5cf26fad46fbc(); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                                int localStrToLng() { string argexpr = hsc0c629727e364218a343e72f775d5378(); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

                                if (Morale >= localStrToLng())
                                {
                                    double localFeatureLevel() { object argIndex1 = i; var ret = withBlock4.FeatureLevel(ref argIndex1); return ret; }

                                    InfightMod = (short)(InfightMod + 5d * localFeatureLevel());
                                }

                                break;
                            }

                        case "射撃強化":
                            {
                                string localFeatureData1() { object argIndex1 = i; var ret = withBlock4.FeatureData(ref argIndex1); return ret; }

                                string localLIndex1() { string arglist = hs8f37be02bf88436e950e311a12d5b37e(); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                                int localStrToLng1() { string argexpr = hs75f1e2fd554a46828757d5c3791b5757(); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

                                if (Morale >= localStrToLng1())
                                {
                                    double localFeatureLevel1() { object argIndex1 = i; var ret = withBlock4.FeatureLevel(ref argIndex1); return ret; }

                                    ShootingMod = (short)(ShootingMod + 5d * localFeatureLevel1());
                                }

                                break;
                            }

                        case "命中強化":
                            {
                                string localFeatureData2() { object argIndex1 = i; var ret = withBlock4.FeatureData(ref argIndex1); return ret; }

                                string localLIndex2() { string arglist = hs11e93ac1f3284a4f93380ee6473c818c(); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                                int localStrToLng2() { string argexpr = hsa2ad2f0f65c84d6f8292a2b9a1fd3fd3(); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

                                if (Morale >= localStrToLng2())
                                {
                                    double localFeatureLevel2() { object argIndex1 = i; var ret = withBlock4.FeatureLevel(ref argIndex1); return ret; }

                                    HitMod = (short)(HitMod + 5d * localFeatureLevel2());
                                }

                                break;
                            }

                        case "回避強化":
                            {
                                string localFeatureData3() { object argIndex1 = i; var ret = withBlock4.FeatureData(ref argIndex1); return ret; }

                                string localLIndex3() { string arglist = hsacf802b5acb245cfbca1d3ca09fa7324(); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                                int localStrToLng3() { string argexpr = hs9f2075d9bdc24b7f9cf3f7845c40a78a(); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

                                if (Morale >= localStrToLng3())
                                {
                                    double localFeatureLevel3() { object argIndex1 = i; var ret = withBlock4.FeatureLevel(ref argIndex1); return ret; }

                                    DodgeMod = (short)(DodgeMod + 5d * localFeatureLevel3());
                                }

                                break;
                            }

                        case "技量強化":
                            {
                                string localFeatureData4() { object argIndex1 = i; var ret = withBlock4.FeatureData(ref argIndex1); return ret; }

                                string localLIndex4() { string arglist = hs3db15ab6c56047db9b6236f45e649ebd(); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                                int localStrToLng4() { string argexpr = hsd8d478d12f3941fca4b635b001945700(); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

                                if (Morale >= localStrToLng4())
                                {
                                    double localFeatureLevel4() { object argIndex1 = i; var ret = withBlock4.FeatureLevel(ref argIndex1); return ret; }

                                    TechniqueMod = (short)(TechniqueMod + 5d * localFeatureLevel4());
                                }

                                break;
                            }

                        case "反応強化":
                            {
                                string localFeatureData5() { object argIndex1 = i; var ret = withBlock4.FeatureData(ref argIndex1); return ret; }

                                string localLIndex5() { string arglist = hs0419ff33196e4e48af748fdce12580b1(); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                                int localStrToLng5() { string argexpr = hsabf5cf29dc0446c781dea0167fb94b44(); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

                                if (Morale >= localStrToLng5())
                                {
                                    double localFeatureLevel5() { object argIndex1 = i; var ret = withBlock4.FeatureLevel(ref argIndex1); return ret; }

                                    IntuitionMod = (short)(IntuitionMod + 5d * localFeatureLevel5());
                                }

                                break;
                            }
                    }
                }

                // 地形適応変更
                string argfname1 = "パイロット地形適応変更";
                if (withBlock4.IsFeatureAvailable(ref argfname1))
                {
                    for (i = 1; i <= 4; i++)
                    {
                        switch (Strings.Mid(Adaption, i, 1) ?? "")
                        {
                            case "S":
                                {
                                    padaption[i] = 5;
                                    break;
                                }

                            case "A":
                                {
                                    padaption[i] = 4;
                                    break;
                                }

                            case "B":
                                {
                                    padaption[i] = 3;
                                    break;
                                }

                            case "C":
                                {
                                    padaption[i] = 2;
                                    break;
                                }

                            case "D":
                                {
                                    padaption[i] = 1;
                                    break;
                                }

                            case "E":
                            case "-":
                                {
                                    padaption[i] = 0;
                                    break;
                                }
                        }
                    }

                    // 地形適応変更能力による修正
                    var loopTo12 = withBlock4.CountFeature();
                    for (i = 1; i <= loopTo12; i++)
                    {
                        object argIndex57 = i;
                        if (withBlock4.Feature(ref argIndex57) == "パイロット地形適応変更")
                        {
                            for (j = 1; j <= 4; j++)
                            {
                                string localFeatureData8() { object argIndex1 = i; var ret = withBlock4.FeatureData(ref argIndex1); return ret; }

                                string localLIndex8() { string arglist = hsb8c32b5b97d840f1a1974442798ae710(); var ret = GeneralLib.LIndex(ref arglist, j); return ret; }

                                string argexpr2 = localLIndex8();
                                if (GeneralLib.StrToLng(ref argexpr2) >= 0)
                                {
                                    // 修正値がプラスのとき
                                    if (padaption[j] < 4)
                                    {
                                        string localFeatureData6() { object argIndex1 = i; var ret = withBlock4.FeatureData(ref argIndex1); return ret; }

                                        string localLIndex6() { string arglist = hs8f1b1c4829bf4c8ea402eb72f2930e63(); var ret = GeneralLib.LIndex(ref arglist, j); return ret; }

                                        int localStrToLng6() { string argexpr = hsdf089849003448bebc02763c21e401ad(); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

                                        padaption[j] = (short)(padaption[j] + localStrToLng6());
                                        // 地形適応はAより高くはならない
                                        if (padaption[j] > 4)
                                        {
                                            padaption[j] = 4;
                                        }
                                    }
                                }
                                else
                                {
                                    // 修正値がマイナスのときは本来の地形適応が"A"以上でも処理を行なう
                                    string localFeatureData7() { object argIndex1 = i; var ret = withBlock4.FeatureData(ref argIndex1); return ret; }

                                    string localLIndex7() { string arglist = hs8e6f6c053bcd4824a879080031dfde79(); var ret = GeneralLib.LIndex(ref arglist, j); return ret; }

                                    int localStrToLng7() { string argexpr = hs210e4a3c7ff64bf2865702c94fd878d3(); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

                                    padaption[j] = (short)(padaption[j] + localStrToLng7());
                                }
                            }
                        }
                    }

                    Adaption = "";
                    for (i = 1; i <= 4; i++)
                    {
                        switch (padaption[i])
                        {
                            case var @case when @case >= 5:
                                {
                                    Adaption = Adaption + "S";
                                    break;
                                }

                            case 4:
                                {
                                    Adaption = Adaption + "A";
                                    break;
                                }

                            case 3:
                                {
                                    Adaption = Adaption + "B";
                                    break;
                                }

                            case 2:
                                {
                                    Adaption = Adaption + "C";
                                    break;
                                }

                            case 1:
                                {
                                    Adaption = Adaption + "D";
                                    break;
                                }

                            case var case1 when case1 <= 0:
                                {
                                    Adaption = Adaption + "-";
                                    break;
                                }
                        }
                    }
                }
            }

            // 気力の値を気力上限・気力下限の範囲にする
            SetMorale(Morale);
            SkipUnitMod:
            ;


            // 基本値と修正値の合計から実際の能力値を算出
            Infight = (short)GeneralLib.MinLng((short)(InfightBase + InfightMod) + InfightMod2, 9999);
            Shooting = (short)GeneralLib.MinLng((short)(ShootingBase + ShootingMod) + ShootingMod2, 9999);
            Hit = (short)GeneralLib.MinLng((short)(HitBase + HitMod) + HitMod2, 9999);
            Dodge = (short)GeneralLib.MinLng((short)(DodgeBase + DodgeMod) + DodgeMod2, 9999);
            Technique = (short)GeneralLib.MinLng((short)(TechniqueBase + TechniqueMod) + TechniqueMod2, 9999);
            Intuition = (short)GeneralLib.MinLng((short)(IntuitionBase + IntuitionMod) + IntuitionMod2, 9999);
        }

        // 周りのユニットによる支援効果を更新
        public void UpdateSupportMod()
        {
            Unit u, my_unit;
            string my_party;
            short my_cmd_rank;
            short cmd_rank, cmd_rank2;
            var cmd_level = default(double);
            double cs_level;
            short range, max_range;
            bool mod_stack;
            var rel_lv = default(short);
            string team, uteam;
            short j, i, k;

            // 支援効果による修正値を初期化

            Infight = (short)(InfightBase + InfightMod);
            Shooting = (short)(ShootingBase + ShootingMod);
            Hit = (short)(HitBase + HitMod);
            Dodge = (short)(DodgeBase + DodgeMod);
            Technique = (short)(TechniqueBase + TechniqueMod);
            Intuition = (short)(IntuitionBase + IntuitionMod);
            InfightMod2 = 0;
            ShootingMod2 = 0;
            HitMod2 = 0;
            DodgeMod2 = 0;
            TechniqueMod2 = 0;
            IntuitionMod2 = 0;
            MoraleMod = 0;

            // ステータス表示時には支援効果を無視
            if (string.IsNullOrEmpty(Map.MapFileName))
            {
                return;
            }

            // ユニットに乗っていなければここで終了
            if (Unit_Renamed is null)
            {
                return;
            }

            // 一旦乗っているユニットを記録しておく
            my_unit = Unit_Renamed;
            {
                var withBlock = Unit_Renamed;
                // ユニットが出撃していなければここで終了
                if (withBlock.Status != "出撃")
                {
                    return;
                }

                if (!ReferenceEquals(Unit_Renamed, Map.MapDataForUnit[withBlock.x, withBlock.y]))
                {
                    return;
                }

                // メインパイロットでなければここで終了
                if (withBlock.CountPilot() == 0)
                {
                    return;
                }

                if (!ReferenceEquals(this, withBlock.MainPilot()))
                {
                    return;
                }

                // 正常な判断が出来ないユニットは支援を受けられない
                object argIndex1 = "暴走";
                if (withBlock.IsConditionSatisfied(ref argIndex1))
                {
                    return;
                }

                object argIndex2 = "混乱";
                if (withBlock.IsConditionSatisfied(ref argIndex2))
                {
                    return;
                }

                // 支援を受けられるかどうかの判定用に陣営を参照しておく
                my_party = withBlock.Party;

                // 指揮効果判定用に自分の階級レベルを算出
                string argsname = "階級";
                if (IsSkillAvailable(ref argsname))
                {
                    object argIndex3 = "階級";
                    string argref_mode = "";
                    my_cmd_rank = (short)SkillLevel(ref argIndex3, ref_mode: ref argref_mode);
                    cmd_rank = my_cmd_rank;
                }
                else
                {
                    if (Strings.InStr(Name, "(ザコ)") == 0 & Strings.InStr(Name, "(汎用)") == 0)
                    {
                        my_cmd_rank = SRC.DEFAULT_LEVEL;
                    }
                    else
                    {
                        my_cmd_rank = 0;
                    }

                    cmd_rank = 0;
                }

                // 自分が所属しているチーム名
                object argIndex4 = "チーム";
                team = SkillData(ref argIndex4);

                // 周りのユニットを調べる
                cs_level = SRC.DEFAULT_LEVEL;
                max_range = 5;
                var loopTo = (short)GeneralLib.MinLng(withBlock.x + max_range, Map.MapWidth);
                for (i = (short)GeneralLib.MaxLng(withBlock.x - max_range, 1); i <= loopTo; i++)
                {
                    var loopTo1 = (short)GeneralLib.MinLng(withBlock.y + max_range, Map.MapHeight);
                    for (j = (short)GeneralLib.MaxLng(withBlock.y - max_range, 1); j <= loopTo1; j++)
                    {
                        // ユニット間の距離が範囲内？
                        range = (short)(Math.Abs((short)(withBlock.x - i)) + Math.Abs((short)(withBlock.y - j)));
                        if (range > max_range)
                        {
                            goto NextUnit;
                        }

                        u = Map.MapDataForUnit[i, j];
                        if (u is null)
                        {
                            goto NextUnit;
                        }

                        if (ReferenceEquals(u, Unit_Renamed))
                        {
                            goto NextUnit;
                        }

                        {
                            var withBlock1 = u;
                            // ユニットにパイロットが乗っていなければ無視
                            if (withBlock1.CountPilot() == 0)
                            {
                                goto NextUnit;
                            }

                            // 陣営が一致していないと支援は受けられない
                            switch (my_party ?? "")
                            {
                                case "味方":
                                case "ＮＰＣ":
                                    {
                                        switch (withBlock1.Party ?? "")
                                        {
                                            case "敵":
                                            case "中立":
                                                {
                                                    goto NextUnit;
                                                    break;
                                                }
                                        }

                                        break;
                                    }

                                default:
                                    {
                                        if ((my_party ?? "") != (withBlock1.Party ?? ""))
                                        {
                                            goto NextUnit;
                                        }

                                        break;
                                    }
                            }

                            // 相手が正常な判断能力を持っていない場合も支援は受けられない
                            object argIndex5 = "暴走";
                            if (withBlock1.IsConditionSatisfied(ref argIndex5))
                            {
                                goto NextUnit;
                            }

                            object argIndex6 = "混乱";
                            if (withBlock1.IsConditionSatisfied(ref argIndex6))
                            {
                                goto NextUnit;
                            }
                        }

                        {
                            var withBlock2 = u.MainPilot(true);
                            // 同じチームに所属している？
                            object argIndex7 = "チーム";
                            uteam = withBlock2.SkillData(ref argIndex7);
                            if ((team ?? "") != (uteam ?? "") & !string.IsNullOrEmpty(uteam))
                            {
                                goto NextUnit;
                            }

                            // 広域サポート
                            if (range <= 2)
                            {
                                object argIndex8 = "広域サポート";
                                string argref_mode1 = "";
                                cs_level = GeneralLib.MaxDbl(cs_level, withBlock2.SkillLevel(ref argIndex8, ref_mode: ref argref_mode1));
                            }

                            // 指揮効果
                            if (my_cmd_rank >= 0)
                            {
                                if (range > withBlock2.CommandRange())
                                {
                                    goto NextUnit;
                                }

                                object argIndex9 = "階級";
                                string argref_mode2 = "";
                                cmd_rank2 = (short)withBlock2.SkillLevel(ref argIndex9, ref_mode: ref argref_mode2);
                                if (cmd_rank2 > cmd_rank)
                                {
                                    cmd_rank = cmd_rank2;
                                    object argIndex10 = "指揮";
                                    string argref_mode3 = "";
                                    cmd_level = withBlock2.SkillLevel(ref argIndex10, ref_mode: ref argref_mode3);
                                }
                                else if (cmd_rank2 == cmd_rank)
                                {
                                    object argIndex11 = "指揮";
                                    string argref_mode4 = "";
                                    cmd_level = GeneralLib.MaxDbl(cmd_level, withBlock2.SkillLevel(ref argIndex11, ref_mode: ref argref_mode4));
                                }
                            }
                        }

                        NextUnit:
                        ;
                    }
                }

                // 追加パイロットの場合は乗っているユニットが変化してしまうことがあるので
                // 変化してしまった場合は元に戻しておく
                if (!ReferenceEquals(my_unit, Unit_Renamed))
                {
                    my_unit.MainPilot();
                }

                // 広域サポートによる修正
                if (cs_level != SRC.DEFAULT_LEVEL)
                {
                    HitMod2 = (short)(HitMod2 + 5d * cs_level);
                    DodgeMod2 = (short)(DodgeMod2 + 5d * cs_level);
                }

                // 指揮能力による修正
                switch (my_cmd_rank)
                {
                    case SRC.DEFAULT_LEVEL:
                        {
                            break;
                        }
                    // 修正なし
                    case 0:
                        {
                            HitMod2 = (short)(HitMod2 + 5d * cmd_level);
                            DodgeMod2 = (short)(DodgeMod2 + 5d * cmd_level);
                            break;
                        }

                    default:
                        {
                            // 自分が階級レベルを持っている場合はより高い階級レベルを
                            // 持つパイロットの指揮効果のみを受ける
                            if (cmd_rank > my_cmd_rank)
                            {
                                HitMod2 = (short)(HitMod2 + 5d * cmd_level);
                                DodgeMod2 = (short)(DodgeMod2 + 5d * cmd_level);
                            }

                            break;
                        }
                }

                // 支援効果による修正を能力値に加算
                Infight = (short)(Infight + InfightMod2);
                Shooting = (short)(Shooting + ShootingMod2);
                Hit = (short)(Hit + HitMod2);
                Dodge = (short)(Dodge + DodgeMod2);
                Technique = (short)(Technique + TechniqueMod2);
                Intuition = (short)(Intuition + IntuitionMod2);

                // 信頼補正
                string argoname = "信頼補正";
                if (!Expression.IsOptionDefined(ref argoname))
                {
                    return;
                }

                if (Strings.InStr(Name, "(ザコ)") > 0)
                {
                    return;
                }

                // 信頼補正が重複する？
                string argoname1 = "信頼補正重複";
                mod_stack = Expression.IsOptionDefined(ref argoname1);

                // 同じユニットに乗っているサポートパイロットからの補正
                if (mod_stack)
                {
                    var loopTo2 = withBlock.CountSupport();
                    for (i = 1; i <= loopTo2; i++)
                    {
                        Pilot localSupport() { object argIndex1 = i; var ret = withBlock.Support(ref argIndex1); return ret; }

                        short localRelation() { var argt = hs3f1fac77d5e34a6ea4e03c266c7e9333(); var ret = this.Relation(ref argt); return ret; }

                        rel_lv = (short)(rel_lv + localRelation());
                    }

                    string argfname = "追加サポート";
                    if (withBlock.IsFeatureAvailable(ref argfname))
                    {
                        rel_lv = (short)(rel_lv + Relation(ref withBlock.AdditionalSupport()));
                    }
                }
                else
                {
                    var loopTo3 = withBlock.CountSupport();
                    for (i = 1; i <= loopTo3; i++)
                    {
                        Pilot localSupport1() { object argIndex1 = i; var ret = withBlock.Support(ref argIndex1); return ret; }

                        short localRelation1() { var argt = hsb76f4c2df155488baa39bba755dc1adf(); var ret = this.Relation(ref argt); return ret; }

                        rel_lv = (short)GeneralLib.MaxLng(localRelation1(), rel_lv);
                    }

                    string argfname1 = "追加サポート";
                    if (withBlock.IsFeatureAvailable(ref argfname1))
                    {
                        rel_lv = (short)GeneralLib.MaxLng(Relation(ref withBlock.AdditionalSupport()), rel_lv);
                    }
                }

                // 周囲のユニットからの補正
                string argoname2 = "信頼補正範囲拡大";
                if (Expression.IsOptionDefined(ref argoname2))
                {
                    max_range = 2;
                }
                else
                {
                    max_range = 1;
                }

                var loopTo4 = (short)GeneralLib.MinLng(withBlock.x + max_range, Map.MapWidth);
                for (i = (short)GeneralLib.MaxLng(withBlock.x - max_range, 1); i <= loopTo4; i++)
                {
                    var loopTo5 = (short)GeneralLib.MinLng(withBlock.y + max_range, Map.MapHeight);
                    for (j = (short)GeneralLib.MaxLng(withBlock.y - max_range, 1); j <= loopTo5; j++)
                    {
                        // ユニット間の距離が範囲内？
                        range = (short)(Math.Abs((short)(withBlock.x - i)) + Math.Abs((short)(withBlock.y - j)));
                        if (range > max_range)
                        {
                            goto NextUnit2;
                        }

                        u = Map.MapDataForUnit[i, j];
                        if (u is null)
                        {
                            goto NextUnit2;
                        }

                        if (ReferenceEquals(u, Unit_Renamed))
                        {
                            goto NextUnit2;
                        }
                        // ユニットにパイロットが乗っていなければ無視
                        if (u.CountPilot() == 0)
                        {
                            goto NextUnit2;
                        }

                        // 味方かどうか判定
                        switch (my_party ?? "")
                        {
                            case "味方":
                            case "ＮＰＣ":
                                {
                                    switch (u.Party ?? "")
                                    {
                                        case "敵":
                                        case "中立":
                                            {
                                                goto NextUnit2;
                                                break;
                                            }
                                    }

                                    break;
                                }

                            default:
                                {
                                    if ((my_party ?? "") != (u.Party ?? ""))
                                    {
                                        goto NextUnit2;
                                    }

                                    break;
                                }
                        }

                        if (mod_stack)
                        {
                            short localRelation2() { var argt = u.MainPilot(true); var ret = Relation(ref argt); return ret; }

                            rel_lv = (short)(rel_lv + localRelation2());
                            var loopTo6 = u.CountPilot();
                            for (k = 2; k <= loopTo6; k++)
                            {
                                Pilot localPilot() { object argIndex1 = k; var ret = u.Pilot(ref argIndex1); return ret; }

                                short localRelation3() { var argt = hs36c94c32aee741e3bafc14d32f76052f(); var ret = this.Relation(ref argt); return ret; }

                                rel_lv = (short)(rel_lv + localRelation3());
                            }

                            var loopTo7 = u.CountSupport();
                            for (k = 1; k <= loopTo7; k++)
                            {
                                Pilot localSupport2() { object argIndex1 = k; var ret = u.Support(ref argIndex1); return ret; }

                                short localRelation4() { var argt = hsa741d7124c89423e8fb9729b82c97548(); var ret = this.Relation(ref argt); return ret; }

                                rel_lv = (short)(rel_lv + localRelation4());
                            }

                            string argfname2 = "追加サポート";
                            if (u.IsFeatureAvailable(ref argfname2))
                            {
                                rel_lv = (short)(rel_lv + Relation(ref u.AdditionalSupport()));
                            }
                        }
                        else
                        {
                            short localRelation5() { var argt = u.MainPilot(true); var ret = Relation(ref argt); return ret; }

                            rel_lv = (short)GeneralLib.MaxLng(localRelation5(), rel_lv);
                            var loopTo8 = u.CountPilot();
                            for (k = 2; k <= loopTo8; k++)
                            {
                                Pilot localPilot1() { object argIndex1 = k; var ret = u.Pilot(ref argIndex1); return ret; }

                                short localRelation6() { var argt = hse54d99b562974d6badbd51c8be741cb0(); var ret = this.Relation(ref argt); return ret; }

                                rel_lv = (short)GeneralLib.MaxLng(localRelation6(), rel_lv);
                            }

                            var loopTo9 = u.CountSupport();
                            for (k = 1; k <= loopTo9; k++)
                            {
                                Pilot localSupport3() { object argIndex1 = k; var ret = u.Support(ref argIndex1); return ret; }

                                short localRelation7() { var argt = hse623540d85d84776ba4c0ea7daeb9431(); var ret = this.Relation(ref argt); return ret; }

                                rel_lv = (short)GeneralLib.MaxLng(localRelation7(), rel_lv);
                            }

                            string argfname3 = "追加サポート";
                            if (u.IsFeatureAvailable(ref argfname3))
                            {
                                rel_lv = (short)GeneralLib.MaxLng(Relation(ref u.AdditionalSupport()), rel_lv);
                            }
                        }

                        NextUnit2:
                        ;
                    }
                }

                // 追加パイロットの場合は乗っているユニットが変化してしまうことがあるので
                // 変化してしまった場合は元に戻しておく
                if (!ReferenceEquals(my_unit, Unit_Renamed))
                {
                    my_unit.MainPilot();
                }

                // 信頼補正を設定
                switch (rel_lv)
                {
                    case 1:
                        {
                            MoraleMod = (short)(MoraleMod + 5);
                            break;
                        }

                    case 2:
                        {
                            MoraleMod = (short)(MoraleMod + 8);
                            break;
                        }

                    case var @case when @case > 2:
                        {
                            MoraleMod = (short)(MoraleMod + 2 * rel_lv + 4);
                            break;
                        }
                }
            }
        }

        private void SetMorale(short new_morale)
        {
            short maxm;
            short minm;
            maxm = MaxMorale;
            minm = MinMorale;
            if (new_morale > maxm)
            {
                proMorale = maxm;
            }
            else if (new_morale < minm)
            {
                proMorale = minm;
            }
            else
            {
                proMorale = new_morale;
            }
        }


        // === 霊力関連処理 ===

        // 霊力最大値
        public int MaxPlana()
        {
            int MaxPlanaRet = default;
            short lv;
            string argsname = "霊力";
            if (!IsSkillAvailable(ref argsname))
            {
                // 霊力能力を持たない場合
                MaxPlanaRet = 0;

                // 追加パイロットの場合は第１パイロットの霊力を代わりに使う
                if (Unit_Renamed is object)
                {
                    {
                        var withBlock = Unit_Renamed;
                        if (withBlock.CountPilot() > 0)
                        {
                            object argIndex2 = 1;
                            object argIndex3 = 1;
                            if (!ReferenceEquals(withBlock.Pilot(ref argIndex3), this))
                            {
                                if (ReferenceEquals(withBlock.MainPilot(), this))
                                {
                                    object argIndex1 = 1;
                                    MaxPlanaRet = withBlock.Pilot(ref argIndex1).MaxPlana();
                                }
                            }
                        }
                    }
                }

                return MaxPlanaRet;
            }

            // 霊力基本値
            object argIndex4 = "霊力";
            string argref_mode = "";
            MaxPlanaRet = (int)SkillLevel(ref argIndex4, ref_mode: ref argref_mode);

            // レベルによる増加分
            lv = (short)GeneralLib.MinLng(Level, 100);
            string argsname1 = "霊力成長";
            if (IsSkillAvailable(ref argsname1))
            {
                object argIndex5 = "霊力成長";
                string argref_mode1 = "";
                MaxPlanaRet = (int)(MaxPlanaRet + (long)(1.5d * lv * (10d + SkillLevel(ref argIndex5, ref_mode: ref argref_mode1))) / 10L);
            }
            else
            {
                MaxPlanaRet = (int)(MaxPlanaRet + 1.5d * lv);
            }

            return MaxPlanaRet;
        }


        // === 特殊能力関連処理 ===

        // 特殊能力の総数
        public short CountSkill()
        {
            short CountSkillRet = default;
            CountSkillRet = (short)colSkill.Count;
            return CountSkillRet;
        }

        // 特殊能力
        public string Skill(ref object Index)
        {
            string SkillRet = default;
            SkillData sd;
            sd = (SkillData)colSkill[Index];
            SkillRet = sd.Name;
            return SkillRet;
        }

        // 現在のレベルにおいて特殊能力 sname が使用可能か
        public bool IsSkillAvailable(ref string sname)
        {
            bool IsSkillAvailableRet = default;
            SkillData sd;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 47388


            Input:

                    On Error GoTo ErrorHandler

             */
            sd = (SkillData)colSkill[sname];
            IsSkillAvailableRet = true;
            return IsSkillAvailableRet;
            ErrorHandler:
            ;


            // 特殊能力付加＆強化による修正
            if (Unit_Renamed is object)
            {
                {
                    var withBlock = Unit_Renamed;
                    if (withBlock.CountCondition() == 0)
                    {
                        return IsSkillAvailableRet;
                    }

                    if (withBlock.CountPilot() == 0)
                    {
                        return IsSkillAvailableRet;
                    }

                    object argIndex1 = 1;
                    object argIndex2 = 1;
                    if (!ReferenceEquals(this, withBlock.MainPilot()) & !ReferenceEquals(this, withBlock.Pilot(ref argIndex2)))
                    {
                        return IsSkillAvailableRet;
                    }

                    bool localIsConditionSatisfied() { object argIndex1 = (object)(sname + "付加２"); var ret = withBlock.IsConditionSatisfied(ref argIndex1); return ret; }

                    object argIndex3 = sname + "付加";
                    if (withBlock.IsConditionSatisfied(ref argIndex3))
                    {
                        IsSkillAvailableRet = true;
                        return IsSkillAvailableRet;
                    }
                    else if (localIsConditionSatisfied())
                    {
                        IsSkillAvailableRet = true;
                        return IsSkillAvailableRet;
                    }

                    object argIndex5 = sname + "強化";
                    if (withBlock.IsConditionSatisfied(ref argIndex5))
                    {
                        object argIndex4 = sname + "強化";
                        if (withBlock.ConditionLevel(ref argIndex4) > 0d)
                        {
                            IsSkillAvailableRet = true;
                            return IsSkillAvailableRet;
                        }
                    }

                    object argIndex7 = sname + "強化２";
                    if (withBlock.IsConditionSatisfied(ref argIndex7))
                    {
                        object argIndex6 = sname + "強化２";
                        if (withBlock.ConditionLevel(ref argIndex6) > 0d)
                        {
                            IsSkillAvailableRet = true;
                            return IsSkillAvailableRet;
                        }
                    }
                }
            }

            IsSkillAvailableRet = false;
        }

        // 現在のレベルにおいて特殊能力 sname が使用可能か
        // (付加による影響を無視した場合)
        public bool IsSkillAvailable2(ref string sname)
        {
            bool IsSkillAvailable2Ret = default;
            SkillData sd;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 48641


            Input:

                    On Error GoTo ErrorHandler

             */
            sd = (SkillData)colSkill[sname];
            IsSkillAvailable2Ret = true;
            return IsSkillAvailable2Ret;
            ErrorHandler:
            ;
            IsSkillAvailable2Ret = false;
        }

        // 現在のレベルにおける特殊能力 Index のレベル
        // データでレベル指定がない場合はレベル 1
        // 特殊能力が使用不能の場合はレベル 0
        public double SkillLevel(ref object Index, [Optional, DefaultParameterValue("")] ref string ref_mode)
        {
            double SkillLevelRet = default;
            string sname;
            SkillData sd;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 49058


            Input:

                    On Error GoTo ErrorHandler

             */
            sd = (SkillData)colSkill[Index];
            sname = sd.Name;
            SkillLevelRet = sd.Level;
            if (SkillLevelRet == SRC.DEFAULT_LEVEL)
            {
                SkillLevelRet = 1d;
            }

            ErrorHandler:
            ;
            if (string.IsNullOrEmpty(sname))
            {
                if (Information.IsNumeric(Index))
                {
                    return SkillLevelRet;
                }
                else
                {
                    // UPGRADE_WARNING: オブジェクト Index の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    sname = Conversions.ToString(Index);
                }
            }

            if (ref_mode == "修正値")
            {
                SkillLevelRet = 0d;
            }
            else if (ref_mode == "基本値")
            {
                return SkillLevelRet;
            }

            // 重複可能な能力は特殊能力付加で置き換えられことはない
            switch (sname ?? "")
            {
                case "ハンター":
                case "ＳＰ消費減少":
                case "スペシャルパワー自動発動":
                    {
                        if (Information.IsNumeric(Index))
                        {
                            return SkillLevelRet;
                        }

                        break;
                    }
            }

            // 特殊能力付加＆強化による修正
            if (Unit_Renamed is null)
            {
                return SkillLevelRet;
            }

            {
                var withBlock = Unit_Renamed;
                if (withBlock.CountCondition() == 0)
                {
                    return SkillLevelRet;
                }

                if (withBlock.CountPilot() == 0)
                {
                    return SkillLevelRet;
                }

                object argIndex1 = 1;
                object argIndex2 = 1;
                if (!ReferenceEquals(this, withBlock.MainPilot()) & !ReferenceEquals(this, withBlock.Pilot(ref argIndex2)))
                {
                    return SkillLevelRet;
                }

                bool localIsConditionSatisfied() { object argIndex1 = sname + "付加２"; var ret = withBlock.IsConditionSatisfied(ref argIndex1); return ret; }

                object argIndex5 = sname + "付加";
                if (withBlock.IsConditionSatisfied(ref argIndex5))
                {
                    object argIndex3 = sname + "付加";
                    SkillLevelRet = withBlock.ConditionLevel(ref argIndex3);
                    if (SkillLevelRet == SRC.DEFAULT_LEVEL)
                    {
                        SkillLevelRet = 1d;
                    }
                }
                else if (localIsConditionSatisfied())
                {
                    object argIndex4 = sname + "付加２";
                    SkillLevelRet = withBlock.ConditionLevel(ref argIndex4);
                    if (SkillLevelRet == SRC.DEFAULT_LEVEL)
                    {
                        SkillLevelRet = 1d;
                    }
                }

                object argIndex6 = sname + "強化";
                if (withBlock.IsConditionSatisfied(ref argIndex6))
                {
                    double localConditionLevel() { object argIndex1 = sname + "強化"; var ret = withBlock.ConditionLevel(ref argIndex1); return ret; }

                    SkillLevelRet = SkillLevelRet + localConditionLevel();
                }

                object argIndex7 = sname + "強化２";
                if (withBlock.IsConditionSatisfied(ref argIndex7))
                {
                    double localConditionLevel1() { object argIndex1 = sname + "強化２"; var ret = withBlock.ConditionLevel(ref argIndex1); return ret; }

                    SkillLevelRet = SkillLevelRet + localConditionLevel1();
                }
            }

            return SkillLevelRet;
        }

        // 特殊能力 Index にレベル指定がなされているか判定
        public bool IsSkillLevelSpecified(ref object Index)
        {
            bool IsSkillLevelSpecifiedRet = default;
            string sname;
            SkillData sd;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 51143


            Input:

                    On Error GoTo ErrorHandler

             */
            sd = (SkillData)colSkill[Index];
            if (sd.Level != SRC.DEFAULT_LEVEL)
            {
                IsSkillLevelSpecifiedRet = true;
                sname = sd.Name;
            }

            return IsSkillLevelSpecifiedRet;
            ErrorHandler:
            ;
            if (string.IsNullOrEmpty(sname))
            {
                if (Information.IsNumeric(Index))
                {
                    return IsSkillLevelSpecifiedRet;
                }
                else
                {
                    // UPGRADE_WARNING: オブジェクト Index の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    sname = Conversions.ToString(Index);
                }
            }

            // 特殊能力付加＆強化による修正
            if (Unit_Renamed is null)
            {
                return IsSkillLevelSpecifiedRet;
            }

            {
                var withBlock = Unit_Renamed;
                if (withBlock.CountCondition() == 0)
                {
                    return IsSkillLevelSpecifiedRet;
                }

                if (withBlock.CountPilot() == 0)
                {
                    return IsSkillLevelSpecifiedRet;
                }

                object argIndex1 = 1;
                object argIndex2 = 1;
                if (!ReferenceEquals(this, withBlock.MainPilot()) & !ReferenceEquals(this, withBlock.Pilot(ref argIndex2)))
                {
                    return IsSkillLevelSpecifiedRet;
                }

                bool localIsConditionSatisfied() { object argIndex1 = sname + "付加２"; var ret = withBlock.IsConditionSatisfied(ref argIndex1); return ret; }

                object argIndex5 = sname + "付加";
                if (withBlock.IsConditionSatisfied(ref argIndex5))
                {
                    object argIndex3 = sname + "付加";
                    if (withBlock.ConditionLevel(ref argIndex3) != SRC.DEFAULT_LEVEL)
                    {
                        IsSkillLevelSpecifiedRet = true;
                    }
                }
                else if (localIsConditionSatisfied())
                {
                    object argIndex4 = sname + "付加２";
                    if (withBlock.ConditionLevel(ref argIndex4) != SRC.DEFAULT_LEVEL)
                    {
                        IsSkillLevelSpecifiedRet = true;
                    }
                }

                bool localIsConditionSatisfied1() { object argIndex1 = sname + "強化２"; var ret = withBlock.IsConditionSatisfied(ref argIndex1); return ret; }

                object argIndex6 = sname + "強化";
                if (withBlock.IsConditionSatisfied(ref argIndex6))
                {
                    IsSkillLevelSpecifiedRet = true;
                }
                else if (localIsConditionSatisfied1())
                {
                    IsSkillLevelSpecifiedRet = true;
                }
            }
        }

        // 特殊能力のデータ
        public string SkillData(ref object Index)
        {
            string SkillDataRet = default;
            string sname;
            SkillData sd;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 52787


            Input:

                    On Error GoTo ErrorHandler

             */
            sd = (SkillData)colSkill[Index];
            sname = sd.Name;
            SkillDataRet = sd.StrData;
            ErrorHandler:
            ;
            if (string.IsNullOrEmpty(sname))
            {
                if (Information.IsNumeric(Index))
                {
                    return SkillDataRet;
                }
                else
                {
                    // UPGRADE_WARNING: オブジェクト Index の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    sname = Conversions.ToString(Index);
                }
            }

            // 重複可能な能力は特殊能力付加で置き換えられことはない
            switch (sname ?? "")
            {
                case "ハンター":
                case "ＳＰ消費減少":
                case "スペシャルパワー自動発動":
                    {
                        if (Information.IsNumeric(Index))
                        {
                            return SkillDataRet;
                        }

                        break;
                    }
            }

            // 特殊能力付加＆強化による修正
            if (Unit_Renamed is null)
            {
                return SkillDataRet;
            }

            {
                var withBlock = Unit_Renamed;
                if (withBlock.CountCondition() == 0)
                {
                    return SkillDataRet;
                }

                if (withBlock.CountPilot() == 0)
                {
                    return SkillDataRet;
                }

                object argIndex1 = 1;
                object argIndex2 = 1;
                if (!ReferenceEquals(this, withBlock.MainPilot()) & !ReferenceEquals(this, withBlock.Pilot(ref argIndex2)))
                {
                    return SkillDataRet;
                }

                bool localIsConditionSatisfied() { object argIndex1 = sname + "付加２"; var ret = withBlock.IsConditionSatisfied(ref argIndex1); return ret; }

                object argIndex5 = sname + "付加";
                if (withBlock.IsConditionSatisfied(ref argIndex5))
                {
                    object argIndex3 = sname + "付加";
                    SkillDataRet = withBlock.ConditionData(ref argIndex3);
                }
                else if (localIsConditionSatisfied())
                {
                    object argIndex4 = sname + "付加２";
                    SkillDataRet = withBlock.ConditionData(ref argIndex4);
                }

                object argIndex7 = sname + "強化";
                if (withBlock.IsConditionSatisfied(ref argIndex7))
                {
                    string localConditionData() { object argIndex1 = sname + "強化"; var ret = withBlock.ConditionData(ref argIndex1); return ret; }

                    if (Strings.Len(localConditionData()) > 0)
                    {
                        object argIndex6 = sname + "強化";
                        SkillDataRet = withBlock.ConditionData(ref argIndex6);
                    }
                }

                object argIndex9 = sname + "強化２";
                if (withBlock.IsConditionSatisfied(ref argIndex9))
                {
                    string localConditionData1() { object argIndex1 = sname + "強化２"; var ret = withBlock.ConditionData(ref argIndex1); return ret; }

                    if (Strings.Len(localConditionData1()) > 0)
                    {
                        object argIndex8 = sname + "強化２";
                        SkillDataRet = withBlock.ConditionData(ref argIndex8);
                    }
                }
            }

            return SkillDataRet;
        }

        // 特殊能力の名称
        public string SkillName(ref object Index)
        {
            string SkillNameRet = default;
            SkillData sd;
            string sname;
            string buf;

            // パイロットが所有している特殊能力の中から検索
            short i;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 54690


            Input:

                    'パイロットが所有している特殊能力の中から検索
                    On Error GoTo ErrorHandler

             */
            sd = (SkillData)colSkill[Index];
            sname = sd.Name;

            // 能力強化系は非表示
            if (Strings.Right(sname, 2) == "ＵＰ" | Strings.Right(sname, 4) == "ＤＯＷＮ")
            {
                SkillNameRet = "非表示";
                return SkillNameRet;
            }

            switch (sname ?? "")
            {
                case "追加レベル":
                case "メッセージ":
                case "魔力所有":
                    {
                        // 非表示の能力
                        SkillNameRet = "非表示";
                        return SkillNameRet;
                    }

                case "得意技":
                case "不得手":
                    {
                        // 別名指定が存在しない能力
                        SkillNameRet = sname;
                        return SkillNameRet;
                    }
            }

            if (Strings.Len(sd.StrData) > 0)
            {
                SkillNameRet = GeneralLib.LIndex(ref sd.StrData, 1);
                switch (SkillNameRet ?? "")
                {
                    case "非表示":
                        {
                            return SkillNameRet;
                        }

                    case "解説":
                        {
                            SkillNameRet = "非表示";
                            return SkillNameRet;
                        }
                }
            }
            else
            {
                SkillNameRet = sname;
            }

            // レベル指定
            if (sd.Level != SRC.DEFAULT_LEVEL & Strings.InStr(SkillNameRet, "Lv") == 0 & Strings.Left(SkillNameRet, 1) != "(")
            {
                SkillNameRet = SkillNameRet + "Lv" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(sd.Level);
            }

            ErrorHandler:
            ;
            if (string.IsNullOrEmpty(sname))
            {
                if (Information.IsNumeric(Index))
                {
                    return SkillNameRet;
                }
                else
                {
                    // UPGRADE_WARNING: オブジェクト Index の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    sname = Conversions.ToString(Index);
                }
            }

            if (sname == "耐久")
            {
                string argoname = "防御力成長";
                string argoname1 = "防御力レベルアップ";
                if (Expression.IsOptionDefined(ref argoname) | Expression.IsOptionDefined(ref argoname1))
                {
                    // 防御力成長オプション使用時には耐久能力を非表示
                    SkillNameRet = "非表示";
                    return SkillNameRet;
                }
            }

            // 得意技＆不得手は名称変更されない
            switch (sname ?? "")
            {
                case "得意技":
                case "不得手":
                    {
                        SkillNameRet = sname;
                        return SkillNameRet;
                    }
            }

            // SetSkillコマンドで封印されている場合
            if (string.IsNullOrEmpty(SkillNameRet))
            {
                string argvname = "Ability(" + ID + "," + sname + ")";
                if (Expression.IsGlobalVariableDefined(ref argvname))
                {
                    // オリジナルの名称を使用
                    SkillNameRet = Data.SkillName(Level, ref sname);
                    if (Strings.InStr(SkillNameRet, "非表示") > 0)
                    {
                        SkillNameRet = "非表示";
                        return SkillNameRet;
                    }
                }
            }

            // 重複可能な能力は特殊能力付加で名称が置き換えられことはない
            switch (sname ?? "")
            {
                case "ハンター":
                case "スペシャルパワー自動発動":
                    {
                        if (Information.IsNumeric(Index))
                        {
                            if (Strings.Left(SkillNameRet, 1) == "(")
                            {
                                SkillNameRet = Strings.Mid(SkillNameRet, 2);
                                string argstr2 = ")";
                                SkillNameRet = Strings.Left(SkillNameRet, GeneralLib.InStr2(ref SkillNameRet, ref argstr2) - 1);
                            }

                            return SkillNameRet;
                        }

                        break;
                    }

                case "ＳＰ消費減少":
                    {
                        if (Information.IsNumeric(Index))
                        {
                            if (Strings.Left(SkillNameRet, 1) == "(")
                            {
                                SkillNameRet = Strings.Mid(SkillNameRet, 2);
                                string argstr21 = ")";
                                SkillNameRet = Strings.Left(SkillNameRet, GeneralLib.InStr2(ref SkillNameRet, ref argstr21) - 1);
                            }

                            i = (short)Strings.InStr(SkillNameRet, "Lv");
                            if (i > 0)
                            {
                                SkillNameRet = Strings.Left(SkillNameRet, i - 1);
                            }

                            return SkillNameRet;
                        }

                        break;
                    }
            }

            // 特殊能力付加＆強化による修正
            if (Unit_Renamed is object)
            {
                {
                    var withBlock = Unit_Renamed;
                    if (withBlock.CountCondition() > 0 & withBlock.CountPilot() > 0)
                    {
                        object argIndex9 = 1;
                        if (ReferenceEquals(withBlock.MainPilot(), this) | ReferenceEquals(withBlock.Pilot(ref argIndex9), this))
                        {
                            // ユニット用特殊能力による付加
                            object argIndex2 = sname + "付加２";
                            if (withBlock.IsConditionSatisfied(ref argIndex2))
                            {
                                string localConditionData() { object argIndex1 = sname + "付加２"; var ret = withBlock.ConditionData(ref argIndex1); return ret; }

                                string arglist = localConditionData();
                                buf = GeneralLib.LIndex(ref arglist, 1);
                                if (!string.IsNullOrEmpty(buf))
                                {
                                    SkillNameRet = buf;
                                }
                                else if (string.IsNullOrEmpty(SkillNameRet))
                                {
                                    SkillNameRet = sname;
                                }

                                if (Strings.InStr(SkillNameRet, "非表示") > 0)
                                {
                                    SkillNameRet = "非表示";
                                    return SkillNameRet;
                                }

                                // レベル指定
                                object argIndex1 = sname + "付加２";
                                if (withBlock.ConditionLevel(ref argIndex1) != SRC.DEFAULT_LEVEL)
                                {
                                    if (Strings.InStr(SkillNameRet, "Lv") > 0)
                                    {
                                        SkillNameRet = Strings.Left(SkillNameRet, Strings.InStr(SkillNameRet, "Lv") - 1);
                                    }

                                    double localConditionLevel() { object argIndex1 = sname + "付加２"; var ret = withBlock.ConditionLevel(ref argIndex1); return ret; }

                                    SkillNameRet = SkillNameRet + "Lv" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(localConditionLevel());
                                }
                            }

                            // アビリティによる付加
                            object argIndex4 = sname + "付加";
                            if (withBlock.IsConditionSatisfied(ref argIndex4))
                            {
                                string localConditionData1() { object argIndex1 = sname + "付加"; var ret = withBlock.ConditionData(ref argIndex1); return ret; }

                                string arglist1 = localConditionData1();
                                buf = GeneralLib.LIndex(ref arglist1, 1);
                                if (!string.IsNullOrEmpty(buf))
                                {
                                    SkillNameRet = buf;
                                }
                                else if (string.IsNullOrEmpty(SkillNameRet))
                                {
                                    SkillNameRet = sname;
                                }

                                if (Strings.InStr(SkillNameRet, "非表示") > 0)
                                {
                                    SkillNameRet = "非表示";
                                    return SkillNameRet;
                                }

                                // レベル指定
                                object argIndex3 = sname + "付加";
                                if (withBlock.ConditionLevel(ref argIndex3) != SRC.DEFAULT_LEVEL)
                                {
                                    if (Strings.InStr(SkillNameRet, "Lv") > 0)
                                    {
                                        SkillNameRet = Strings.Left(SkillNameRet, Strings.InStr(SkillNameRet, "Lv") - 1);
                                    }

                                    double localConditionLevel1() { object argIndex1 = sname + "付加"; var ret = withBlock.ConditionLevel(ref argIndex1); return ret; }

                                    SkillNameRet = SkillNameRet + "Lv" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(localConditionLevel1());
                                }
                            }

                            // ユニット用特殊能力による強化
                            object argIndex6 = sname + "強化２";
                            if (withBlock.IsConditionSatisfied(ref argIndex6))
                            {
                                if (string.IsNullOrEmpty(SkillNameRet))
                                {
                                    // 強化される能力をパイロットが持っていなかった場合
                                    string localConditionData2() { object argIndex1 = sname + "強化２"; var ret = withBlock.ConditionData(ref argIndex1); return ret; }

                                    string arglist2 = localConditionData2();
                                    SkillNameRet = GeneralLib.LIndex(ref arglist2, 1);
                                    if (string.IsNullOrEmpty(SkillNameRet))
                                    {
                                        SkillNameRet = sname;
                                    }

                                    if (Strings.InStr(SkillNameRet, "非表示") > 0)
                                    {
                                        SkillNameRet = "非表示";
                                        return SkillNameRet;
                                    }

                                    SkillNameRet = SkillNameRet + "Lv0";
                                }

                                if (sname != "同調率" & sname != "霊力")
                                {
                                    object argIndex5 = sname + "強化２";
                                    if (withBlock.ConditionLevel(ref argIndex5) >= 0d)
                                    {
                                        double localConditionLevel2() { object argIndex1 = sname + "強化２"; var ret = withBlock.ConditionLevel(ref argIndex1); return ret; }

                                        SkillNameRet = SkillNameRet + "+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(localConditionLevel2());
                                    }
                                    else
                                    {
                                        double localConditionLevel3() { object argIndex1 = sname + "強化２"; var ret = withBlock.ConditionLevel(ref argIndex1); return ret; }

                                        SkillNameRet = SkillNameRet + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(localConditionLevel3());
                                    }
                                }
                            }

                            // アビリティによる強化
                            object argIndex8 = sname + "強化";
                            if (withBlock.IsConditionSatisfied(ref argIndex8))
                            {
                                if (string.IsNullOrEmpty(SkillNameRet))
                                {
                                    // 強化される能力をパイロットが持っていなかった場合
                                    string localConditionData3() { object argIndex1 = sname + "強化"; var ret = withBlock.ConditionData(ref argIndex1); return ret; }

                                    string arglist3 = localConditionData3();
                                    SkillNameRet = GeneralLib.LIndex(ref arglist3, 1);
                                    if (string.IsNullOrEmpty(SkillNameRet))
                                    {
                                        SkillNameRet = sname;
                                    }

                                    if (Strings.InStr(SkillNameRet, "非表示") > 0)
                                    {
                                        SkillNameRet = "非表示";
                                        return SkillNameRet;
                                    }

                                    SkillNameRet = SkillNameRet + "Lv0";
                                }

                                if (sname != "同調率" & sname != "霊力")
                                {
                                    object argIndex7 = sname + "強化";
                                    if (withBlock.ConditionLevel(ref argIndex7) >= 0d)
                                    {
                                        double localConditionLevel4() { object argIndex1 = sname + "強化"; var ret = withBlock.ConditionLevel(ref argIndex1); return ret; }

                                        SkillNameRet = SkillNameRet + "+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(localConditionLevel4());
                                    }
                                    else
                                    {
                                        double localConditionLevel5() { object argIndex1 = sname + "強化"; var ret = withBlock.ConditionLevel(ref argIndex1); return ret; }

                                        SkillNameRet = SkillNameRet + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(localConditionLevel5());
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // 能力強化系は非表示
            if (Strings.Right(sname, 2) == "ＵＰ" | Strings.Right(sname, 4) == "ＤＯＷＮ")
            {
                SkillNameRet = "非表示";
                return SkillNameRet;
            }

            switch (sname ?? "")
            {
                case "追加レベル":
                case "メッセージ":
                case "魔力所有":
                    {
                        // 非表示の能力
                        SkillNameRet = "非表示";
                        return SkillNameRet;
                    }

                case "耐久":
                    {
                        string argoname2 = "防御力成長";
                        string argoname3 = "防御力レベルアップ";
                        if (Expression.IsOptionDefined(ref argoname2) | Expression.IsOptionDefined(ref argoname3))
                        {
                            // 防御力成長オプション使用時には耐久能力を非表示
                            SkillNameRet = "非表示";
                            return SkillNameRet;
                        }

                        break;
                    }
            }

            // これらの能力からはレベル指定を除く
            switch (sname ?? "")
            {
                case "階級":
                case "同調率":
                case "霊力":
                case "ＳＰ消費減少":
                    {
                        i = (short)Strings.InStr(SkillNameRet, "Lv");
                        if (i > 0)
                        {
                            SkillNameRet = Strings.Left(SkillNameRet, i - 1);
                        }

                        break;
                    }
            }

            // レベル非表示用の括弧を削除
            if (Strings.Left(SkillNameRet, 1) == "(")
            {
                SkillNameRet = Strings.Mid(SkillNameRet, 2);
                string argstr22 = ")";
                SkillNameRet = Strings.Left(SkillNameRet, GeneralLib.InStr2(ref SkillNameRet, ref argstr22) - 1);
            }

            if (string.IsNullOrEmpty(SkillNameRet))
            {
                SkillNameRet = sname;
            }

            return SkillNameRet;
        }

        // 特殊能力名称（レベル表示抜き）
        public string SkillName0(ref object Index)
        {
            string SkillName0Ret = default;
            SkillData sd;
            string sname;
            string buf;

            // パイロットが所有している特殊能力の中から検索
            short i;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 63575


            Input:

                    'パイロットが所有している特殊能力の中から検索
                    On Error GoTo ErrorHandler

             */
            sd = (SkillData)colSkill[Index];
            sname = sd.Name;

            // 能力強化系は非表示
            if (Strings.Right(sname, 2) == "ＵＰ" | Strings.Right(sname, 4) == "ＤＯＷＮ")
            {
                SkillName0Ret = "非表示";
                return SkillName0Ret;
            }

            switch (sname ?? "")
            {
                case "追加レベル":
                case "メッセージ":
                case "魔力所有":
                    {
                        // 非表示の能力
                        SkillName0Ret = "非表示";
                        return SkillName0Ret;
                    }

                case "得意技":
                case "不得手":
                    {
                        // 別名指定が存在しない能力
                        SkillName0Ret = sname;
                        return SkillName0Ret;
                    }
            }

            if (Strings.Len(sd.StrData) > 0)
            {
                SkillName0Ret = GeneralLib.LIndex(ref sd.StrData, 1);
                if (SkillName0Ret == "非表示")
                {
                    return SkillName0Ret;
                }
            }
            else
            {
                SkillName0Ret = sname;
            }

            ErrorHandler:
            ;
            if (string.IsNullOrEmpty(sname))
            {
                if (Information.IsNumeric(Index))
                {
                    return SkillName0Ret;
                }
                else
                {
                    // UPGRADE_WARNING: オブジェクト Index の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    sname = Conversions.ToString(Index);
                }
            }

            if (sname == "耐久")
            {
                string argoname = "防御力成長";
                string argoname1 = "防御力レベルアップ";
                if (Expression.IsOptionDefined(ref argoname) | Expression.IsOptionDefined(ref argoname1))
                {
                    // 防御力成長オプション使用時には耐久能力を非表示
                    SkillName0Ret = "非表示";
                    return SkillName0Ret;
                }
            }

            // 得意技＆不得手は名称変更されない
            switch (sname ?? "")
            {
                case "得意技":
                case "不得手":
                    {
                        SkillName0Ret = sname;
                        return SkillName0Ret;
                    }
            }

            // SetSkillコマンドで封印されている場合
            if (string.IsNullOrEmpty(SkillName0Ret))
            {
                string argvname = "Ability(" + ID + "," + sname + ")";
                if (Expression.IsGlobalVariableDefined(ref argvname))
                {
                    // オリジナルの名称を使用
                    SkillName0Ret = Data.SkillName(Level, ref sname);
                    if (Strings.InStr(SkillName0Ret, "非表示") > 0)
                    {
                        SkillName0Ret = "非表示";
                        return SkillName0Ret;
                    }
                }
            }

            // 重複可能な能力は特殊能力付加で名称が置き換えられことはない
            switch (sname ?? "")
            {
                case "ハンター":
                case "ＳＰ消費減少":
                case "スペシャルパワー自動発動":
                    {
                        if (Information.IsNumeric(Index))
                        {
                            return SkillName0Ret;
                        }

                        break;
                    }
            }

            // 特殊能力付加＆強化による修正
            if (Unit_Renamed is object)
            {
                {
                    var withBlock = Unit_Renamed;
                    if (withBlock.CountCondition() > 0 & withBlock.CountPilot() > 0)
                    {
                        object argIndex5 = 1;
                        if (ReferenceEquals(withBlock.MainPilot(), this) | ReferenceEquals(withBlock.Pilot(ref argIndex5), this))
                        {
                            // ユニット用特殊能力による付加
                            object argIndex1 = sname + "付加２";
                            if (withBlock.IsConditionSatisfied(ref argIndex1))
                            {
                                string localConditionData() { object argIndex1 = sname + "付加２"; var ret = withBlock.ConditionData(ref argIndex1); return ret; }

                                string arglist = localConditionData();
                                buf = GeneralLib.LIndex(ref arglist, 1);
                                if (!string.IsNullOrEmpty(buf))
                                {
                                    SkillName0Ret = buf;
                                }
                                else if (string.IsNullOrEmpty(SkillName0Ret))
                                {
                                    SkillName0Ret = sname;
                                }

                                if (Strings.InStr(SkillName0Ret, "非表示") > 0)
                                {
                                    SkillName0Ret = "非表示";
                                    return SkillName0Ret;
                                }
                            }

                            // アビリティによる付加
                            object argIndex2 = sname + "付加";
                            if (withBlock.IsConditionSatisfied(ref argIndex2))
                            {
                                string localConditionData1() { object argIndex1 = sname + "付加"; var ret = withBlock.ConditionData(ref argIndex1); return ret; }

                                string arglist1 = localConditionData1();
                                buf = GeneralLib.LIndex(ref arglist1, 1);
                                if (!string.IsNullOrEmpty(buf))
                                {
                                    SkillName0Ret = buf;
                                }
                                else if (string.IsNullOrEmpty(SkillName0Ret))
                                {
                                    SkillName0Ret = sname;
                                }

                                if (Strings.InStr(SkillName0Ret, "非表示") > 0)
                                {
                                    SkillName0Ret = "非表示";
                                    return SkillName0Ret;
                                }
                            }

                            // ユニット用特殊能力による強化
                            if (string.IsNullOrEmpty(SkillName0Ret))
                            {
                                object argIndex3 = sname + "強化２";
                                if (withBlock.IsConditionSatisfied(ref argIndex3))
                                {
                                    string localConditionData2() { object argIndex1 = sname + "強化２"; var ret = withBlock.ConditionData(ref argIndex1); return ret; }

                                    string arglist2 = localConditionData2();
                                    SkillName0Ret = GeneralLib.LIndex(ref arglist2, 1);
                                    if (string.IsNullOrEmpty(SkillName0Ret))
                                    {
                                        SkillName0Ret = sname;
                                    }

                                    if (Strings.InStr(SkillName0Ret, "非表示") > 0)
                                    {
                                        SkillName0Ret = "非表示";
                                        return SkillName0Ret;
                                    }
                                }
                            }

                            // アビリティによる強化
                            if (string.IsNullOrEmpty(SkillName0Ret))
                            {
                                object argIndex4 = sname + "強化";
                                if (withBlock.IsConditionSatisfied(ref argIndex4))
                                {
                                    string localConditionData3() { object argIndex1 = sname + "強化"; var ret = withBlock.ConditionData(ref argIndex1); return ret; }

                                    string arglist3 = localConditionData3();
                                    SkillName0Ret = GeneralLib.LIndex(ref arglist3, 1);
                                    if (string.IsNullOrEmpty(SkillName0Ret))
                                    {
                                        SkillName0Ret = sname;
                                    }

                                    if (Strings.InStr(SkillName0Ret, "非表示") > 0)
                                    {
                                        SkillName0Ret = "非表示";
                                        return SkillName0Ret;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // 該当するものが無ければエリアスから検索
            if (string.IsNullOrEmpty(SkillName0Ret))
            {
                {
                    var withBlock1 = SRC.ALDList;
                    var loopTo = withBlock1.Count();
                    for (i = 1; i <= loopTo; i++)
                    {
                        object argIndex6 = i;
                        {
                            var withBlock2 = withBlock1.Item(ref argIndex6);
                            if ((withBlock2.get_AliasType(1) ?? "") == (sname ?? ""))
                            {
                                SkillName0Ret = withBlock2.Name;
                                return SkillName0Ret;
                            }
                        }
                    }
                }

                SkillName0Ret = sname;
            }

            // 能力強化系は非表示
            if (Strings.Right(sname, 2) == "ＵＰ" | Strings.Right(sname, 4) == "ＤＯＷＮ")
            {
                SkillName0Ret = "非表示";
                return SkillName0Ret;
            }

            switch (sname ?? "")
            {
                case "追加レベル":
                case "メッセージ":
                case "魔力所有":
                    {
                        // 非表示の能力
                        SkillName0Ret = "非表示";
                        return SkillName0Ret;
                    }

                case "耐久":
                    {
                        string argoname2 = "防御力成長";
                        string argoname3 = "防御力レベルアップ";
                        if (Expression.IsOptionDefined(ref argoname2) | Expression.IsOptionDefined(ref argoname3))
                        {
                            // 防御力成長オプション使用時には耐久能力を非表示
                            SkillName0Ret = "非表示";
                            return SkillName0Ret;
                        }

                        break;
                    }
            }

            // レベル非表示用の括弧を削除
            if (Strings.Left(SkillName0Ret, 1) == "(")
            {
                SkillName0Ret = Strings.Mid(SkillName0Ret, 2);
                string argstr2 = ")";
                SkillName0Ret = Strings.Left(SkillName0Ret, GeneralLib.InStr2(ref SkillName0Ret, ref argstr2) - 1);
            }

            // レベル指定を削除
            i = (short)Strings.InStr(SkillName0Ret, "Lv");
            if (i > 0)
            {
                SkillName0Ret = Strings.Left(SkillName0Ret, i - 1);
            }

            return SkillName0Ret;
        }

        // 特殊能力名称（必要技能判定用）
        // 名称からレベル指定を削除し、名称が非表示にされている場合は元の特殊能力名
        // もしくはエリアス名を使用する。
        public string SkillNameForNS(ref string stype)
        {
            string SkillNameForNSRet = default;
            SkillData sd;
            string buf;
            short i;

            // 非表示の特殊能力
            if (Strings.Right(stype, 2) == "ＵＰ" | Strings.Right(stype, 4) == "ＤＯＷＮ")
            {
                SkillNameForNSRet = stype;
                return SkillNameForNSRet;
            }

            if (stype == "メッセージ")
            {
                SkillNameForNSRet = stype;
                return SkillNameForNSRet;

                // パイロットが所有している特殊能力の中から検索
            };
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 69788


            Input:

                    'パイロットが所有している特殊能力の中から検索
                    On Error GoTo ErrorHandler

             */
            sd = (SkillData)colSkill[stype];
            if (Strings.Len(sd.StrData) > 0)
            {
                SkillNameForNSRet = GeneralLib.LIndex(ref sd.StrData, 1);
            }
            else
            {
                SkillNameForNSRet = stype;
            }

            ErrorHandler:
            ;


            // SetSkillコマンドで封印されている場合
            if (string.IsNullOrEmpty(SkillNameForNSRet))
            {
                string argvname = "Ability(" + ID + "," + stype + ")";
                if (Expression.IsGlobalVariableDefined(ref argvname))
                {
                    // オリジナルの名称を使用
                    SkillNameForNSRet = Data.SkillName(Level, ref stype);
                    if (Strings.InStr(SkillNameForNSRet, "非表示") > 0)
                    {
                        SkillNameForNSRet = "非表示";
                    }
                }
            }

            // 特殊能力付加＆強化による修正
            if (Unit_Renamed is object)
            {
                {
                    var withBlock = Unit_Renamed;
                    if (withBlock.CountCondition() > 0 & withBlock.CountPilot() > 0)
                    {
                        object argIndex5 = 1;
                        if (ReferenceEquals(this, withBlock.MainPilot()) | ReferenceEquals(this, withBlock.Pilot(ref argIndex5)))
                        {
                            // ユニット用特殊能力による付加
                            object argIndex1 = stype + "付加２";
                            if (withBlock.IsConditionSatisfied(ref argIndex1))
                            {
                                string localConditionData() { object argIndex1 = (object)(stype + "付加２"); var ret = withBlock.ConditionData(ref argIndex1); return ret; }

                                string arglist = localConditionData();
                                buf = GeneralLib.LIndex(ref arglist, 1);
                                if (!string.IsNullOrEmpty(buf))
                                {
                                    SkillNameForNSRet = buf;
                                }
                                else if (string.IsNullOrEmpty(SkillNameForNSRet))
                                {
                                    SkillNameForNSRet = stype;
                                }

                                if (Strings.InStr(SkillNameForNSRet, "非表示") > 0)
                                {
                                    SkillNameForNSRet = "非表示";
                                }
                            }

                            // アビリティによる付加
                            object argIndex2 = stype + "付加";
                            if (withBlock.IsConditionSatisfied(ref argIndex2))
                            {
                                string localConditionData1() { object argIndex1 = (object)(stype + "付加"); var ret = withBlock.ConditionData(ref argIndex1); return ret; }

                                string arglist1 = localConditionData1();
                                buf = GeneralLib.LIndex(ref arglist1, 1);
                                if (!string.IsNullOrEmpty(buf))
                                {
                                    SkillNameForNSRet = buf;
                                }
                                else if (string.IsNullOrEmpty(SkillNameForNSRet))
                                {
                                    SkillNameForNSRet = stype;
                                }

                                if (Strings.InStr(SkillNameForNSRet, "非表示") > 0)
                                {
                                    SkillNameForNSRet = "非表示";
                                }
                            }

                            // ユニット用特殊能力による強化
                            if (string.IsNullOrEmpty(SkillNameForNSRet))
                            {
                                object argIndex3 = stype + "強化２";
                                if (withBlock.IsConditionSatisfied(ref argIndex3))
                                {
                                    string localConditionData2() { object argIndex1 = (object)(stype + "強化２"); var ret = withBlock.ConditionData(ref argIndex1); return ret; }

                                    string arglist2 = localConditionData2();
                                    SkillNameForNSRet = GeneralLib.LIndex(ref arglist2, 1);
                                    if (string.IsNullOrEmpty(SkillNameForNSRet))
                                    {
                                        SkillNameForNSRet = stype;
                                    }

                                    if (Strings.InStr(SkillNameForNSRet, "非表示") > 0)
                                    {
                                        SkillNameForNSRet = "非表示";
                                    }
                                }
                            }

                            // アビリティによる強化
                            if (string.IsNullOrEmpty(SkillNameForNSRet))
                            {
                                object argIndex4 = stype + "強化";
                                if (withBlock.IsConditionSatisfied(ref argIndex4))
                                {
                                    string localConditionData3() { object argIndex1 = (object)(stype + "強化"); var ret = withBlock.ConditionData(ref argIndex1); return ret; }

                                    string arglist3 = localConditionData3();
                                    SkillNameForNSRet = GeneralLib.LIndex(ref arglist3, 1);
                                    if (string.IsNullOrEmpty(SkillNameForNSRet))
                                    {
                                        SkillNameForNSRet = stype;
                                    }

                                    if (Strings.InStr(SkillNameForNSRet, "非表示") > 0)
                                    {
                                        SkillNameForNSRet = "非表示";
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // 該当するものが無ければエリアスから検索
            if (string.IsNullOrEmpty(SkillNameForNSRet) | SkillNameForNSRet == "非表示")
            {
                {
                    var withBlock1 = SRC.ALDList;
                    var loopTo = withBlock1.Count();
                    for (i = 1; i <= loopTo; i++)
                    {
                        object argIndex6 = i;
                        {
                            var withBlock2 = withBlock1.Item(ref argIndex6);
                            if ((withBlock2.get_AliasType(1) ?? "") == (stype ?? ""))
                            {
                                SkillNameForNSRet = withBlock2.Name;
                                return SkillNameForNSRet;
                            }
                        }
                    }
                }

                SkillNameForNSRet = stype;
            }

            // レベル非表示用の括弧を削除
            if (Strings.Left(SkillNameForNSRet, 1) == "(")
            {
                SkillNameForNSRet = Strings.Mid(SkillNameForNSRet, 2);
                string argstr2 = ")";
                SkillNameForNSRet = Strings.Left(SkillNameForNSRet, GeneralLib.InStr2(ref SkillNameForNSRet, ref argstr2) - 1);
            }

            // レベル表示を削除
            i = (short)Strings.InStr(SkillNameForNSRet, "Lv");
            if (i > 0)
            {
                SkillNameForNSRet = Strings.Left(SkillNameForNSRet, i - 1);
            }

            return SkillNameForNSRet;
        }

        // 特殊能力の種類
        public string SkillType(ref string sname)
        {
            string SkillTypeRet = default;
            short i;
            string sname0, sname2;
            if (string.IsNullOrEmpty(sname))
            {
                return SkillTypeRet;
            }

            i = (short)Strings.InStr(sname, "Lv");
            if (i > 0)
            {
                sname0 = Strings.Left(sname, i - 1);
            }
            else
            {
                sname0 = sname;
            }

            // エリアスデータが定義されている？
            object argIndex2 = sname0;
            if (SRC.ALDList.IsDefined(ref argIndex2))
            {
                object argIndex1 = sname0;
                {
                    var withBlock = SRC.ALDList.Item(ref argIndex1);
                    SkillTypeRet = withBlock.get_AliasType(1);
                    return SkillTypeRet;
                }
            }

            // 特殊能力一覧から検索
            foreach (SkillData sd in colSkill)
            {
                if ((sname0 ?? "") == (sd.Name ?? ""))
                {
                    SkillTypeRet = sd.Name;
                    return SkillTypeRet;
                }

                sname2 = GeneralLib.LIndex(ref sd.StrData, 1);
                if ((sname0 ?? "") == (sname2 ?? ""))
                {
                    SkillTypeRet = sd.Name;
                    return SkillTypeRet;
                }

                if (Strings.Left(sname2, 1) == "(")
                {
                    if (Strings.Right(sname2, 1) == ")")
                    {
                        sname2 = Strings.Mid(sname2, 2, Strings.Len(sname2) - 2);
                        if ((sname ?? "") == (sname2 ?? ""))
                        {
                            SkillTypeRet = sd.Name;
                            return SkillTypeRet;
                        }
                    }
                }
            }

            // その能力を修得していない
            SkillTypeRet = sname0;

            // 特殊能力付加による修正
            if (Unit_Renamed is object)
            {
                {
                    var withBlock1 = Unit_Renamed;
                    if (Conversions.ToBoolean(withBlock1.CountCondition() & Conversions.ToShort(withBlock1.CountPilot() > 0)))
                    {
                        object argIndex5 = 1;
                        if (ReferenceEquals(this, withBlock1.MainPilot()) | ReferenceEquals(this, withBlock1.Pilot(ref argIndex5)))
                        {
                            var loopTo = withBlock1.CountCondition();
                            for (i = 1; i <= loopTo; i++)
                            {
                                string localCondition() { object argIndex1 = i; var ret = withBlock1.Condition(ref argIndex1); return ret; }

                                string localCondition1() { object argIndex1 = i; var ret = withBlock1.Condition(ref argIndex1); return ret; }

                                if (Strings.Right(localCondition(), 2) == "付加")
                                {
                                    string localConditionData() { object argIndex1 = i; var ret = withBlock1.ConditionData(ref argIndex1); return ret; }

                                    string arglist = localConditionData();
                                    if ((GeneralLib.LIndex(ref arglist, 1) ?? "") == (sname0 ?? ""))
                                    {
                                        object argIndex3 = i;
                                        SkillTypeRet = withBlock1.Condition(ref argIndex3);
                                        SkillTypeRet = Strings.Left(SkillTypeRet, Strings.Len(SkillTypeRet) - 2);
                                        break;
                                    }
                                }
                                else if (Strings.Right(localCondition1(), 3) == "付加２")
                                {
                                    string localConditionData1() { object argIndex1 = i; var ret = withBlock1.ConditionData(ref argIndex1); return ret; }

                                    string arglist1 = localConditionData1();
                                    if ((GeneralLib.LIndex(ref arglist1, 1) ?? "") == (sname0 ?? ""))
                                    {
                                        object argIndex4 = i;
                                        SkillTypeRet = withBlock1.Condition(ref argIndex4);
                                        SkillTypeRet = Strings.Left(SkillTypeRet, Strings.Len(SkillTypeRet) - 3);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return SkillTypeRet;
        }

        // スペシャルパワー sname を修得しているか？
        public bool IsSpecialPowerAvailable(ref string sname)
        {
            bool IsSpecialPowerAvailableRet = default;
            if (Data.SP <= 0)
            {
                // ＳＰを持たない追加パイロットの場合は１番目のパイロットのデータを使う
                if (Unit_Renamed is object)
                {
                    {
                        var withBlock = Unit_Renamed;
                        if (withBlock.CountPilot() > 0)
                        {
                            object argIndex2 = 1;
                            object argIndex3 = 1;
                            if (!ReferenceEquals(withBlock.Pilot(ref argIndex3), this))
                            {
                                if (ReferenceEquals(withBlock.MainPilot(), this))
                                {
                                    object argIndex1 = 1;
                                    IsSpecialPowerAvailableRet = Unit_Renamed.Pilot(ref argIndex1).Data.IsSpecialPowerAvailable(Level, ref sname);
                                    return IsSpecialPowerAvailableRet;
                                }
                            }
                        }
                    }
                }
            }

            IsSpecialPowerAvailableRet = Data.IsSpecialPowerAvailable(Level, ref sname);
            return IsSpecialPowerAvailableRet;
        }

        // スペシャルパワー sname が有用か？
        public bool IsSpecialPowerUseful(ref string sname)
        {
            bool IsSpecialPowerUsefulRet = default;
            SpecialPowerData localItem() { object argIndex1 = sname; var ret = SRC.SPDList.Item(ref argIndex1); return ret; }

            var argp = this;
            IsSpecialPowerUsefulRet = localItem().Useful(ref argp);
            return IsSpecialPowerUsefulRet;
        }

        // スペシャルパワー sname に必要なＳＰ値
        public short SpecialPowerCost(ref string sname)
        {
            short SpecialPowerCostRet = default;
            short i, j;
            string adata;
            if (Data.SP <= 0)
            {
                // ＳＰを持たない追加パイロットの場合は１番目のパイロットのデータを使う
                if (Unit_Renamed is object)
                {
                    {
                        var withBlock = Unit_Renamed;
                        if (withBlock.CountPilot() > 0)
                        {
                            object argIndex2 = 1;
                            object argIndex3 = 1;
                            if (!ReferenceEquals(withBlock.Pilot(ref argIndex3), this))
                            {
                                if (ReferenceEquals(withBlock.MainPilot(), this))
                                {
                                    object argIndex1 = 1;
                                    SpecialPowerCostRet = withBlock.Pilot(ref argIndex1).SpecialPowerCost(ref sname);
                                    return SpecialPowerCostRet;
                                }
                            }
                        }
                    }
                }
            }

            // 基本消費ＳＰ値
            SpecialPowerCostRet = Data.SpecialPowerCost(sname);

            // 特殊能力による消費ＳＰ値修正
            string argsname = "超能力";
            string argsname1 = "集中力";
            if (IsSkillAvailable(ref argsname) | IsSkillAvailable(ref argsname1))
            {
                SpecialPowerCostRet = (short)(0.8d * SpecialPowerCostRet);
            }

            string argsname2 = "知覚強化";
            if (IsSkillAvailable(ref argsname2))
            {
                SpecialPowerCostRet = (short)(1.2d * SpecialPowerCostRet);
            }

            // ＳＰ消費減少能力
            if (Unit_Renamed is object)
            {
                {
                    var withBlock1 = Unit_Renamed;
                    if (withBlock1.CountPilot() > 0)
                    {
                        if (ReferenceEquals(withBlock1.MainPilot(), this))
                        {
                            object argIndex6 = "ＳＰ消費減少付加";
                            object argIndex7 = "ＳＰ消費減少付加２";
                            if (withBlock1.IsConditionSatisfied(ref argIndex6) | withBlock1.IsConditionSatisfied(ref argIndex7))
                            {
                                object argIndex4 = "ＳＰ消費減少";
                                adata = SkillData(ref argIndex4);
                                var loopTo = GeneralLib.LLength(ref adata);
                                for (i = 2; i <= loopTo; i++)
                                {
                                    if ((sname ?? "") == (GeneralLib.LIndex(ref adata, i) ?? ""))
                                    {
                                        object argIndex5 = "ＳＰ消費減少";
                                        string argref_mode = "";
                                        SpecialPowerCostRet = (short)((long)((10d - SkillLevel(ref argIndex5, ref_mode: ref argref_mode)) * SpecialPowerCostRet) / 10L);
                                        return SpecialPowerCostRet;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            var loopTo1 = CountSkill();
            for (i = 1; i <= loopTo1; i++)
            {
                object argIndex9 = i;
                if (Skill(ref argIndex9) == "ＳＰ消費減少")
                {
                    object argIndex8 = i;
                    adata = SkillData(ref argIndex8);
                    var loopTo2 = GeneralLib.LLength(ref adata);
                    for (j = 2; j <= loopTo2; j++)
                    {
                        if ((sname ?? "") == (GeneralLib.LIndex(ref adata, j) ?? ""))
                        {
                            double localSkillLevel() { object argIndex1 = i; string argref_mode = ""; var ret = SkillLevel(ref argIndex1, ref_mode: ref argref_mode); return ret; }

                            SpecialPowerCostRet = (short)((long)((10d - localSkillLevel()) * SpecialPowerCostRet) / 10L);
                            return SpecialPowerCostRet;
                        }
                    }
                }
            }

            return SpecialPowerCostRet;
        }

        // スペシャルパワー sname を実行する
        public void UseSpecialPower(ref string sname, double sp_mod = 1d)
        {
            Unit my_unit;
            bool localIsDefined() { object argIndex1 = sname; var ret = SRC.SPDList.IsDefined(ref argIndex1); return ret; }

            if (!localIsDefined())
            {
                return;
            }

            Status.ClearUnitStatus();
            Commands.SelectedPilot = this;

            // スペシャルパワー使用メッセージ
            SpecialPowerData localItem() { object argIndex1 = sname; var ret = SRC.SPDList.Item(ref argIndex1); return ret; }

            SpecialPowerData localItem1() { object argIndex1 = sname; var ret = SRC.SPDList.Item(ref argIndex1); return ret; }

            string argename = "復活";
            string argename1 = "自爆";
            if (Conversions.ToBoolean(Operators.AndObject(Operators.AndObject(sp_mod != 2d, !localItem().IsEffectAvailable(ref argename)), !localItem1().IsEffectAvailable(ref argename1))))
            {
                if (Unit_Renamed.IsMessageDefined(ref sname))
                {
                    Unit argu1 = null;
                    Unit argu2 = null;
                    GUI.OpenMessageForm(u1: ref argu1, u2: ref argu2);
                    string argmsg_mode = "";
                    Unit_Renamed.PilotMessage(ref sname, msg_mode: ref argmsg_mode);
                    GUI.CloseMessageForm();
                }
            }

            // 同じ追加パイロットを持つユニットが複数いる場合、パイロットのUnitが
            // 変化してしまうことがあるため、元のUnitを記録しておく
            my_unit = Unit_Renamed;

            // スペシャルパワーアニメを表示
            SpecialPowerData localItem2() { object argIndex1 = sname; var ret = SRC.SPDList.Item(ref argIndex1); return ret; }

            if (!localItem2().PlayAnimation())
            {
                // メッセージ表示のみ
                Unit argu21 = null;
                GUI.OpenMessageForm(ref Unit_Renamed, u2: ref argu21);
                GUI.DisplaySysMessage(get_Nickname(false) + "は" + sname + "を使った。");
            }

            // Unitが変化した場合に元に戻す
            if (!ReferenceEquals(my_unit, Unit_Renamed))
            {
                my_unit.MainPilot();
            }

            // スペシャルパワーを実行
            SpecialPowerData localItem3() { object argIndex1 = sname; var ret = SRC.SPDList.Item(ref argIndex1); return ret; }

            var argp = this;
            localItem3().Execute(ref argp);

            // Unitが変化した場合に元に戻す
            if (!ReferenceEquals(my_unit, Unit_Renamed))
            {
                my_unit.CurrentForm().MainPilot();
            }

            SP = (int)(SP - sp_mod * SpecialPowerCost(ref sname));
            GUI.CloseMessageForm();
        }


        // === ユニット搭乗＆下乗関連処理 ===

        // ユニット u に搭乗
        public void Ride(ref Unit u, bool is_support = false)
        {
            double hp_ratio, en_ratio;
            double plana_ratio;

            // 既に乗っていればなにもしない
            if (ReferenceEquals(Unit_Renamed, u))
            {
                return;
            }

            {
                var withBlock = u;
                hp_ratio = 100 * withBlock.HP / (double)withBlock.MaxHP;
                en_ratio = 100 * withBlock.EN / (double)withBlock.MaxEN;

                // 現在の霊力値を記録
                if (MaxPlana() > 0)
                {
                    plana_ratio = 100 * Plana / (double)MaxPlana();
                }
                else
                {
                    plana_ratio = -1;
                }

                Unit_Renamed = u;
                short localInStrNotNest1() { string argstring1 = Class_Renamed; string argstring2 = "サポート)"; var ret = GeneralLib.InStrNotNest(ref argstring1, ref argstring2); this.Class_Renamed = argstring1; return ret; }

                short localLLength() { string arglist = Class_Renamed; var ret = GeneralLib.LLength(ref arglist); this.Class_Renamed = arglist; return ret; }

                string argfname = "ダミーユニット";
                if (localInStrNotNest1() > 0 & localLLength() == 1 & !withBlock.IsFeatureAvailable(ref argfname))
                {
                    // サポートにしかなれないパイロットの場合
                    var argp = this;
                    withBlock.AddSupport(ref argp);
                }
                else if (IsSupport(ref u))
                {
                    // 同じユニットクラスに対して通常パイロットとサポートの両方のパターン
                    // がいける場合は通常パイロットを優先
                    short localInStrNotNest() { string argstring1 = Class_Renamed; string argstring2 = u.Class0 + " "; var ret = GeneralLib.InStrNotNest(ref argstring1, ref argstring2); this.Class_Renamed = argstring1; return ret; }

                    if (withBlock.CountPilot() < Math.Abs(withBlock.Data.PilotNum) & localInStrNotNest() > 0 & !is_support)
                    {
                        var argp2 = this;
                        withBlock.AddPilot(ref argp2);
                    }
                    else
                    {
                        var argp3 = this;
                        withBlock.AddSupport(ref argp3);
                    }
                }
                else
                {
                    // パイロットが既に規定数の場合は全パイロットを降ろす
                    if (withBlock.CountPilot() == Math.Abs(withBlock.Data.PilotNum))
                    {
                        object argIndex1 = 1;
                        withBlock.Pilot(ref argIndex1).GetOff();
                    }

                    var argp1 = this;
                    withBlock.AddPilot(ref argp1);
                }

                // Pilotコマンドで作成されたパイロットは全て味方なので搭乗時に変更が必要
                Party = withBlock.Party0;

                // ユニットのステータスをアップデート
                withBlock.Update();

                // 霊力値のアップデート
                if (plana_ratio >= 0d)
                {
                    Plana = (int)((long)(MaxPlana() * plana_ratio) / 100L);
                }
                else
                {
                    Plana = MaxPlana();
                }

                // パイロットが乗り込むことによるＨＰ＆ＥＮの増減に対応
                withBlock.HP = (int)((long)(withBlock.MaxHP * hp_ratio) / 100L);
                withBlock.EN = (int)((long)(withBlock.MaxEN * en_ratio) / 100L);
            }
        }

        // パイロットをユニットから降ろす
        public void GetOff(bool without_leave = false)
        {
            short i;

            // 既に降りている？
            if (Unit_Renamed is null)
            {
                return;
            }

            {
                var withBlock = Unit_Renamed;
                var loopTo = withBlock.CountSupport();
                for (i = 1; i <= loopTo; i++)
                {
                    object argIndex2 = i;
                    if (ReferenceEquals(withBlock.Support(ref argIndex2), this))
                    {
                        // サポートパイロットとして乗り込んでいる場合
                        object argIndex1 = i;
                        withBlock.DeleteSupport(ref argIndex1);
                        withBlock.Update();
                        // UPGRADE_NOTE: オブジェクト Unit_Renamed をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                        Unit_Renamed = null;
                        Update();
                        return;
                    }
                }

                // 出撃していた場合は退却
                if (!without_leave)
                {
                    if (withBlock.Status == "出撃")
                    {
                        withBlock.Status = "待機";
                        // UPGRADE_NOTE: オブジェクト MapDataForUnit() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                        Map.MapDataForUnit[withBlock.x, withBlock.y] = null;
                        GUI.EraseUnitBitmap(withBlock.x, withBlock.y, false);
                    }
                }

                // 通常のパイロットの場合は、そのユニットに乗っていた他のパイロットも降ろされる
                var loopTo1 = withBlock.CountPilot();
                for (i = 1; i <= loopTo1; i++)
                {
                    // UPGRADE_NOTE: オブジェクト Unit_Renamed.Pilot().Unit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                    object argIndex3 = 1;
                    withBlock.Pilot(ref argIndex3).Unit_Renamed = null;
                    object argIndex4 = 1;
                    withBlock.DeletePilot(ref argIndex4);
                }

                var loopTo2 = withBlock.CountSupport();
                for (i = 1; i <= loopTo2; i++)
                {
                    // UPGRADE_NOTE: オブジェクト Unit_Renamed.Support().Unit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                    object argIndex5 = 1;
                    withBlock.Support(ref argIndex5).Unit_Renamed = null;
                    object argIndex6 = 1;
                    withBlock.DeleteSupport(ref argIndex6);
                }

                withBlock.Update();
            }

            // UPGRADE_NOTE: オブジェクト Unit_Renamed をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            Unit_Renamed = null;
            Update();
        }

        // パイロットがユニット u のサポートかどうか
        public bool IsSupport(ref Unit u)
        {
            bool IsSupportRet = default;
            string uclass, pclass;
            short i, j;
            string argfname = "ダミーユニット";
            if (u.IsFeatureAvailable(ref argfname))
            {
                // ダミーユニットの場合はサポートパイロットも通常のパイロットとして扱う
                IsSupportRet = false;
                return IsSupportRet;
            }

            // サポート指定が存在する？
            short localInStrNotNest() { string argstring1 = Class_Renamed; string argstring2 = "サポート)"; var ret = GeneralLib.InStrNotNest(ref argstring1, ref argstring2); this.Class_Renamed = argstring1; return ret; }

            if (localInStrNotNest() == 0)
            {
                IsSupportRet = false;
                return IsSupportRet;
            }

            if (u.CountPilot() == 0)
            {
                // パイロットが乗っていないユニットの場合は通常パイロットを優先
                string arglist1 = Class_Renamed;
                var loopTo = GeneralLib.LLength(ref arglist1);
                for (i = 1; i <= loopTo; i++)
                {
                    string arglist = Class_Renamed;
                    pclass = GeneralLib.LIndex(ref arglist, i);
                    this.Class_Renamed = arglist;
                    if ((u.Class_Renamed ?? "") == (pclass ?? "") | (u.Class_Renamed ?? "") == (pclass + "(" + Name + "専用)" ?? "") | (u.Class_Renamed ?? "") == (pclass + "(" + get_Nickname(false) + "専用)" ?? "") | (u.Class_Renamed ?? "") == (pclass + "(" + Sex + "専用)" ?? ""))
                    {
                        // 通常のパイロットとして搭乗可能であればサポートでないとみなす
                        IsSupportRet = false;
                        return IsSupportRet;
                    }
                }

                this.Class_Renamed = arglist1;
            }
            else
            {
                // 通常のパイロットとして搭乗している？
                var loopTo1 = u.CountPilot();
                for (i = 1; i <= loopTo1; i++)
                {
                    object argIndex1 = i;
                    if (ReferenceEquals(u.Pilot(ref argIndex1), this))
                    {
                        IsSupportRet = false;
                        return IsSupportRet;
                    }
                }
            }

            uclass = u.Class0;

            // 通常のサポート？
            string arglist2 = Class_Renamed;
            var loopTo2 = GeneralLib.LLength(ref arglist2);
            for (i = 1; i <= loopTo2; i++)
            {
                string localLIndex() { string arglist = Class_Renamed; var ret = GeneralLib.LIndex(ref arglist, i); this.Class_Renamed = arglist; return ret; }

                if ((uclass + "(サポート)" ?? "") == (localLIndex() ?? ""))
                {
                    IsSupportRet = true;
                    return IsSupportRet;
                }
            }

            this.Class_Renamed = arglist2;

            // パイロットが乗っていないユニットの場合はここで終了
            if (u.CountPilot() == 0)
            {
                IsSupportRet = false;
                return IsSupportRet;
            }

            // 専属サポート？
            {
                var withBlock = u.MainPilot();
                string arglist4 = Class_Renamed;
                var loopTo3 = GeneralLib.LLength(ref arglist4);
                for (i = 1; i <= loopTo3; i++)
                {
                    string arglist3 = Class_Renamed;
                    pclass = GeneralLib.LIndex(ref arglist3, i);
                    this.Class_Renamed = arglist3;
                    if ((pclass ?? "") == (uclass + "(" + withBlock.Name + "専属サポート)" ?? "") | (pclass ?? "") == (uclass + "(" + withBlock.get_Nickname(false) + "専属サポート)" ?? "") | (pclass ?? "") == (uclass + "(" + withBlock.Sex + "専属サポート)" ?? ""))
                    {
                        IsSupportRet = true;
                        return IsSupportRet;
                    }

                    var loopTo4 = withBlock.CountSkill();
                    for (j = 1; j <= loopTo4; j++)
                    {
                        string localSkill() { object argIndex1 = j; var ret = withBlock.Skill(ref argIndex1); return ret; }

                        if ((pclass ?? "") == (uclass + "(" + localSkill() + "専属サポート)" ?? ""))
                        {
                            IsSupportRet = true;
                            return IsSupportRet;
                        }
                    }
                }

                this.Class_Renamed = arglist4;
            }

            IsSupportRet = false;
            return IsSupportRet;
        }

        // ユニット u に乗ることができるかどうか
        public bool IsAbleToRide(ref Unit u)
        {
            bool IsAbleToRideRet = default;
            string uclass, pclass;
            short i;
            {
                var withBlock = u;
                // 汎用ユニットは必要技能を満たせばＯＫ
                if (withBlock.Class_Renamed == "汎用")
                {
                    IsAbleToRideRet = true;
                    goto CheckNecessarySkill;
                }

                // 人間ユニット指定を除いて判定
                if (Strings.Left(withBlock.Class_Renamed, 1) == "(" & Strings.Right(withBlock.Class_Renamed, 1) == ")")
                {
                    uclass = Strings.Mid(withBlock.Class_Renamed, 2, Strings.Len(withBlock.Class_Renamed) - 2);
                }
                else
                {
                    uclass = withBlock.Class_Renamed;
                }

                // サポートかどうかをまず判定しておく
                if (IsSupport(ref u))
                {
                    IsAbleToRideRet = true;
                    // 必要技能をチェックする
                    goto CheckNecessarySkill;
                }

                string arglist1 = Class_Renamed;
                var loopTo = GeneralLib.LLength(ref arglist1);
                for (i = 1; i <= loopTo; i++)
                {
                    string arglist = Class_Renamed;
                    pclass = GeneralLib.LIndex(ref arglist, i);
                    this.Class_Renamed = arglist;
                    if ((uclass ?? "") == (pclass ?? "") | (uclass ?? "") == (pclass + "(" + get_Nickname(false) + "専用)" ?? "") | (uclass ?? "") == (pclass + "(" + Name + "専用)" ?? "") | (uclass ?? "") == (pclass + "(" + Sex + "専用)" ?? ""))
                    {
                        IsAbleToRideRet = true;
                        // 必要技能をチェックする
                        goto CheckNecessarySkill;
                    }
                }

                this.Class_Renamed = arglist1; // ユニットクラスは複数設定可能

                // クラスが合わない
                IsAbleToRideRet = false;
                return IsAbleToRideRet;
                CheckNecessarySkill:
                ;


                // 必要技能＆不必要技能をチェック

                // 両能力を持っていない場合はチェック不要
                string argfname = "必要技能";
                string argfname1 = "不必要技能";
                if (!withBlock.IsFeatureAvailable(ref argfname) & !withBlock.IsFeatureAvailable(ref argfname1))
                {
                    return IsAbleToRideRet;
                }

                var loopTo1 = withBlock.CountFeature();
                for (i = 1; i <= loopTo1; i++)
                {
                    string localFeature() { object argIndex1 = i; var ret = withBlock.Feature(ref argIndex1); return ret; }

                    object argIndex1 = i;
                    if (withBlock.Feature(ref argIndex1) == "必要技能")
                    {
                        string localFeatureData() { object argIndex1 = i; var ret = withBlock.FeatureData(ref argIndex1); return ret; }

                        bool localIsNecessarySkillSatisfied() { string argnabilities = hs98e3424915b54fdbb83dbc9aa23c37a5(); var argp = this; var ret = withBlock.IsNecessarySkillSatisfied(ref argnabilities, ref argp); return ret; }

                        if (!localIsNecessarySkillSatisfied())
                        {
                            IsAbleToRideRet = false;
                            return IsAbleToRideRet;
                        }
                    }
                    else if (localFeature() == "不必要技能")
                    {
                        string localFeatureData1() { object argIndex1 = i; var ret = withBlock.FeatureData(ref argIndex1); return ret; }

                        string argnabilities = localFeatureData1();
                        var argp = this;
                        if (withBlock.IsNecessarySkillSatisfied(ref argnabilities, ref argp))
                        {
                            IsAbleToRideRet = false;
                            return IsAbleToRideRet;
                        }
                    }
                }
            }

            return IsAbleToRideRet;
        }


        // === 一時中断関連処理 ===

        // 一時中断用データをファイルにセーブする
        public void Dump()
        {
            FileSystem.WriteLine(SRC.SaveDataFileNumber, Name, ID, Party);
            FileSystem.WriteLine(SRC.SaveDataFileNumber, Level, Exp);
            FileSystem.WriteLine(SRC.SaveDataFileNumber, SP, Morale, Plana);
            FileSystem.WriteLine(SRC.SaveDataFileNumber, Alive, Away, SupportIndex);
            if (Unit_Renamed is null)
            {
                FileSystem.WriteLine(SRC.SaveDataFileNumber, "-");
            }
            else
            {
                FileSystem.WriteLine(SRC.SaveDataFileNumber, Unit_Renamed.ID);
            }
        }

        // 一時中断用データをファイルからロードする
        public void Restore()
        {
            var sbuf = default(string);
            var ibuf = default(short);
            var bbuf = default(bool);

            // Name, ID, Party
            FileSystem.Input(SRC.SaveDataFileNumber, ref sbuf);
            Name = sbuf;
            FileSystem.Input(SRC.SaveDataFileNumber, ref sbuf);
            ID = sbuf;
            FileSystem.Input(SRC.SaveDataFileNumber, ref sbuf);
            Party = sbuf;

            // Leve, Exp
            FileSystem.Input(SRC.SaveDataFileNumber, ref ibuf);
            Level = ibuf;
            FileSystem.Input(SRC.SaveDataFileNumber, ref ibuf);
            Exp = ibuf;

            // SP, Morale, Plana
            FileSystem.Input(SRC.SaveDataFileNumber, ref ibuf);
            SP = ibuf;
            FileSystem.Input(SRC.SaveDataFileNumber, ref ibuf);
            Morale = ibuf;
            FileSystem.Input(SRC.SaveDataFileNumber, ref ibuf);
            Plana = ibuf;

            // Alive, Away, SupportIndex
            FileSystem.Input(SRC.SaveDataFileNumber, ref bbuf);
            Alive = bbuf;
            FileSystem.Input(SRC.SaveDataFileNumber, ref bbuf);
            Away = bbuf;
            FileSystem.Input(SRC.SaveDataFileNumber, ref ibuf);
            SupportIndex = ibuf;

            // Unit
            sbuf = FileSystem.LineInput(SRC.SaveDataFileNumber);
        }

        // 一時中断用データのリンク情報をファイルからロードする
        public void RestoreLinkInfo()
        {
            string sbuf;
            short ibuf;

            // Name, ID, Party
            sbuf = FileSystem.LineInput(SRC.SaveDataFileNumber);

            // Leve, Exp
            sbuf = FileSystem.LineInput(SRC.SaveDataFileNumber);

            // SP, Morale, Plana
            sbuf = FileSystem.LineInput(SRC.SaveDataFileNumber);

            // Alive, Away, SupportIndex
            sbuf = FileSystem.LineInput(SRC.SaveDataFileNumber);

            // Unit
            FileSystem.Input(SRC.SaveDataFileNumber, ref sbuf);
            object argIndex1 = sbuf;
            Unit_Renamed = SRC.UList.Item(ref argIndex1);
        }

        // 一時中断用データのパラメータ情報をファイルからロードする
        public void RestoreParameter()
        {
            string sbuf;
            var ibuf = default(short);

            // Name, ID, Party
            sbuf = FileSystem.LineInput(SRC.SaveDataFileNumber);

            // Leve, Exp
            sbuf = FileSystem.LineInput(SRC.SaveDataFileNumber);

            // SP, Morale, Plana
            FileSystem.Input(SRC.SaveDataFileNumber, ref ibuf);
            SP = ibuf;
            FileSystem.Input(SRC.SaveDataFileNumber, ref ibuf);
            Morale = ibuf;
            FileSystem.Input(SRC.SaveDataFileNumber, ref ibuf);
            Plana = ibuf;

            // Alive, Away, SupportIndex
            sbuf = FileSystem.LineInput(SRC.SaveDataFileNumber);

            // Unit
            sbuf = FileSystem.LineInput(SRC.SaveDataFileNumber);
        }


        // === その他 ===

        // 全回復
        public void FullRecover()
        {
            // 闘争本能によって初期気力は変化する
            string argsname = "闘争本能";
            if (IsSkillAvailable(ref argsname))
            {
                if (MinMorale > 100)
                {
                    object argIndex1 = "闘争本能";
                    string argref_mode = "";
                    SetMorale((short)(MinMorale + 5d * SkillLevel(ref argIndex1, ref_mode: ref argref_mode)));
                }
                else
                {
                    object argIndex2 = "闘争本能";
                    string argref_mode1 = "";
                    SetMorale((short)(100d + 5d * SkillLevel(ref argIndex2, ref_mode: ref argref_mode1)));
                }
            }
            else
            {
                SetMorale(100);
            }

            SP = MaxSP;
            Plana = MaxPlana();
        }

        // 同調率
        public short SynchroRate()
        {
            short SynchroRateRet = default;
            short lv;
            string argsname = "同調率";
            if (!IsSkillAvailable(ref argsname))
            {
                return SynchroRateRet;
            }

            // 同調率基本値
            object argIndex1 = "同調率";
            string argref_mode = "";
            SynchroRateRet = (short)SkillLevel(ref argIndex1, ref_mode: ref argref_mode);

            // レベルによる増加分
            lv = (short)GeneralLib.MinLng(Level, 100);
            string argsname1 = "同調率成長";
            if (IsSkillAvailable(ref argsname1))
            {
                object argIndex2 = "同調率成長";
                string argref_mode1 = "";
                SynchroRateRet = (short)(SynchroRateRet + (long)(lv * (10d + SkillLevel(ref argIndex2, ref_mode: ref argref_mode1))) / 10L);
            }
            else
            {
                SynchroRateRet = (short)(SynchroRateRet + lv);
            }

            return SynchroRateRet;
        }

        // 指揮範囲
        public short CommandRange()
        {
            short CommandRangeRet = default;
            // 指揮能力を持っていなければ範囲は0
            string argsname = "指揮";
            if (!IsSkillAvailable(ref argsname))
            {
                CommandRangeRet = 0;
                return CommandRangeRet;
            }

            // 指揮能力を持っている場合は階級レベルに依存
            object argIndex1 = "階級";
            string argref_mode = "";
            switch (SkillLevel(ref argIndex1, ref_mode: ref argref_mode))
            {
                case var @case when 0d <= @case && @case <= 6d:
                    {
                        CommandRangeRet = 2;
                        break;
                    }

                case var case1 when 7d <= case1 && case1 <= 9d:
                    {
                        CommandRangeRet = 3;
                        break;
                    }

                case var case2 when 10d <= case2 && case2 <= 12d:
                    {
                        CommandRangeRet = 4;
                        break;
                    }

                default:
                    {
                        CommandRangeRet = 5;
                        break;
                    }
            }

            return CommandRangeRet;
        }

        // 行動決定に用いられる戦闘判断力
        public short TacticalTechnique0()
        {
            short TacticalTechnique0Ret = default;
            object argIndex1 = "戦術";
            string argref_mode = "";
            TacticalTechnique0Ret = (short)(TechniqueBase - Level + 10d * SkillLevel(ref argIndex1, ref_mode: ref argref_mode));
            return TacticalTechnique0Ret;
        }

        public short TacticalTechnique()
        {
            short TacticalTechniqueRet = default;
            // 正常な判断能力がある？
            if (Unit_Renamed is object)
            {
                {
                    var withBlock = Unit_Renamed;
                    object argIndex1 = "混乱";
                    object argIndex2 = "暴走";
                    object argIndex3 = "狂戦士";
                    if (withBlock.IsConditionSatisfied(ref argIndex1) | withBlock.IsConditionSatisfied(ref argIndex2) | withBlock.IsConditionSatisfied(ref argIndex3))
                    {
                        return TacticalTechniqueRet;
                    }
                }
            }

            TacticalTechniqueRet = TacticalTechnique0();
            return TacticalTechniqueRet;
        }

        // イベントコマンド SetRelation で設定した値を返す
        public short Relation(ref Pilot t)
        {
            short RelationRet = default;
            string argexpr = "関係:" + Name + ":" + t.Name;
            RelationRet = (short)Expression.GetValueAsLong(ref argexpr);
            return RelationRet;
        }

        // 射撃能力が「魔力」と表示されるかどうか
        public bool HasMana()
        {
            bool HasManaRet = default;
            string argsname = "術";
            string argsname1 = "魔力所有";
            if (IsSkillAvailable(ref argsname) | IsSkillAvailable(ref argsname1))
            {
                HasManaRet = true;
                return HasManaRet;
            }

            string argoname = "魔力使用";
            if (Expression.IsOptionDefined(ref argoname))
            {
                HasManaRet = true;
                return HasManaRet;
            }

            HasManaRet = false;
            return HasManaRet;
        }
    }
}