using Microsoft.VisualBasic;

namespace Project1
{
    internal class Item
    {

        // Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
        // 本プログラムはフリーソフトであり、無保証です。
        // 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
        // 再頒布または改変することができます。

        // 作成されたアイテムのクラス

        // 識別子
        public string ID;
        // アイテムデータへのポインタ
        public ItemData Data;
        // アイテムを装備しているユニット
        // UPGRADE_NOTE: Unit は Unit_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        public Unit Unit_Renamed;
        // アイテムが存在しているか？ (RemoveItemされていないか？)
        public bool Exist;
        // アイテムが効力を発揮できているか？ (必要技能や武器クラス＆防具クラスを満たしているか？)
        public bool Activated;

        // クラスの初期化
        // UPGRADE_NOTE: Class_Initialize は Class_Initialize_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        private void Class_Initialize_Renamed()
        {
            Exist = true;
            Activated = true;
            // UPGRADE_NOTE: オブジェクト Data をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            Data = null;
            // UPGRADE_NOTE: オブジェクト Unit_Renamed をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            Unit_Renamed = null;
        }

        public Item() : base()
        {
            Class_Initialize_Renamed();
        }

        // クラスの解放
        // UPGRADE_NOTE: Class_Terminate は Class_Terminate_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        private void Class_Terminate_Renamed()
        {
            // UPGRADE_NOTE: オブジェクト Data をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            Data = null;
            // UPGRADE_NOTE: オブジェクト Unit_Renamed をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            Unit_Renamed = null;
        }

        ~Item()
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
                Data = SRC.IDList.Item(ref argIndex1);
            }
        }

        // 愛称
        public string Nickname()
        {
            string NicknameRet = default;
            Unit u;
            NicknameRet = Data.Nickname;

            // 愛称内の式置換のため、デフォルトユニットを一時的に変更する
            u = Event_Renamed.SelectedUnitForEvent;
            Event_Renamed.SelectedUnitForEvent = Unit_Renamed;
            Expression.ReplaceSubExpression(ref NicknameRet);
            Event_Renamed.SelectedUnitForEvent = u;
            return NicknameRet;
        }

        // 読み仮名
        public string KanaName()
        {
            string KanaNameRet = default;
            Unit u;
            KanaNameRet = Data.KanaName;

            // 読み仮名内の式置換のため、デフォルトユニットを一時的に変更する
            u = Event_Renamed.SelectedUnitForEvent;
            Event_Renamed.SelectedUnitForEvent = Unit_Renamed;
            Expression.ReplaceSubExpression(ref KanaNameRet);
            Event_Renamed.SelectedUnitForEvent = u;
            return KanaNameRet;
        }

        // クラス
        // UPGRADE_NOTE: Class は Class_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        public string Class_Renamed()
        {
            string Class_RenamedRet = default;
            Class_RenamedRet = Data.Class_Renamed;
            return Class_RenamedRet;
        }

        public string Class0()
        {
            string Class0Ret = default;
            short i, n;
            Class0Ret = Data.Class_Renamed;

            // 専用指定を削除
            if (Strings.Right(Class0Ret, 3) == "専用)")
            {
                n = 1;
                i = (short)(Strings.Len(Class0Ret) - 2);
                do
                {
                    i = (short)(i - 1);
                    switch (Strings.Mid(Class0Ret, i, 1) ?? "")
                    {
                        case "(":
                            {
                                n = (short)(n - 1);
                                if (n == 0)
                                {
                                    Class0Ret = Strings.Left(Class0Ret, i - 1);
                                    break;
                                }

                                break;
                            }

                        case ")":
                            {
                                n = (short)(n + 1);
                                break;
                            }
                    }
                }
                while (i > 0);
            }

            return Class0Ret;
        }

        // 装備個所
        public string Part()
        {
            string PartRet = default;
            PartRet = Data.Part;
            return PartRet;
        }

        // ＨＰ修正値
        public int HP()
        {
            int HPRet = default;
            HPRet = Data.HP;
            return HPRet;
        }

        // ＥＮ修正値
        public short EN()
        {
            short ENRet = default;
            ENRet = Data.EN;
            return ENRet;
        }

        // 装甲修正値
        public int Armor()
        {
            int ArmorRet = default;
            ArmorRet = Data.Armor;
            return ArmorRet;
        }

        // 運動性修正値
        public short Mobility()
        {
            short MobilityRet = default;
            MobilityRet = Data.Mobility;
            return MobilityRet;
        }

        // 移動力修正値
        public short Speed()
        {
            short SpeedRet = default;
            SpeedRet = Data.Speed;
            return SpeedRet;
        }

        // 特殊能力総数
        public short CountFeature()
        {
            short CountFeatureRet = default;
            CountFeatureRet = Data.CountFeature();
            return CountFeatureRet;
        }

        // 特殊能力
        public string Feature(ref object Index)
        {
            string FeatureRet = default;
            FeatureRet = Data.Feature(ref Index);
            return FeatureRet;
        }

        // 特殊能力の名称
        public string FeatureName(ref object Index)
        {
            string FeatureNameRet = default;
            FeatureNameRet = Data.FeatureName(ref Index);
            return FeatureNameRet;
        }

        // 特殊能力のレベル
        public double FeatureLevel(ref object Index)
        {
            double FeatureLevelRet = default;
            FeatureLevelRet = Data.FeatureLevel(ref Index);
            return FeatureLevelRet;
        }

        // 特殊能力のデータ
        public string FeatureData(ref object Index)
        {
            string FeatureDataRet = default;
            FeatureDataRet = Data.FeatureData(ref Index);
            return FeatureDataRet;
        }

        // 特殊能力の必要技能
        public string FeatureNecessarySkill(ref object Index)
        {
            string FeatureNecessarySkillRet = default;
            FeatureNecessarySkillRet = Data.FeatureNecessarySkill(ref Index);
            return FeatureNecessarySkillRet;
        }

        // 指定した特殊能力を持っているか？
        public bool IsFeatureAvailable(ref string fname)
        {
            bool IsFeatureAvailableRet = default;
            IsFeatureAvailableRet = Data.IsFeatureAvailable(ref fname);
            return IsFeatureAvailableRet;
        }

        // 武器データ
        public WeaponData Weapon(ref object Index)
        {
            WeaponData WeaponRet = default;
            WeaponRet = Data.Weapon(ref Index);
            return WeaponRet;
        }

        // 武器の総数
        public short CountWeapon()
        {
            short CountWeaponRet = default;
            CountWeaponRet = Data.CountWeapon();
            return CountWeaponRet;
        }

        // アビリティデータ
        public AbilityData Ability(ref object Index)
        {
            AbilityData AbilityRet = default;
            AbilityRet = Data.Ability(ref Index);
            return AbilityRet;
        }

        // アビリティの総数
        public short CountAbility()
        {
            short CountAbilityRet = default;
            CountAbilityRet = Data.CountAbility();
            return CountAbilityRet;
        }

        // サイズ(アイテムが消費するアイテムスロット数)
        public short Size()
        {
            short SizeRet = default;
            SizeRet = Data.Size();
            return SizeRet;
        }


        // アイテムが使用可能か？
        public bool IsAvailable(ref Unit u)
        {
            bool IsAvailableRet = default;
            short j, i, k;
            string iclass;
            string sname, fdata;
            IsAvailableRet = false;

            // イベントコマンド「Disable」
            string argvname = "Disable(" + Name + ")";
            if (Expression.IsGlobalVariableDefined(ref argvname))
            {
                return IsAvailableRet;
            }

            // 装備個所に適合しているか
            switch (Part() ?? "")
            {
                case "片手":
                case "両手":
                case "盾":
                    {
                        object argIndex1 = "装備個所";
                        if (Strings.InStr(u.FeatureData(ref argIndex1), "腕") == 0)
                        {
                            return IsAvailableRet;
                        }

                        break;
                    }

                case "肩":
                case "両肩":
                    {
                        object argIndex2 = "装備個所";
                        if (Strings.InStr(u.FeatureData(ref argIndex2), "肩") == 0)
                        {
                            return IsAvailableRet;
                        }

                        break;
                    }

                case "体":
                    {
                        object argIndex3 = "装備個所";
                        if (Strings.InStr(u.FeatureData(ref argIndex3), "体") == 0)
                        {
                            return IsAvailableRet;
                        }

                        break;
                    }

                case "頭":
                    {
                        object argIndex4 = "装備個所";
                        if (Strings.InStr(u.FeatureData(ref argIndex4), "頭") == 0)
                        {
                            return IsAvailableRet;
                        }

                        break;
                    }
            }

            // 武器クラス or 防具クラスに属しているか？
            switch (Part() ?? "")
            {
                case "武器":
                case "片手":
                case "両手":
                    {
                        iclass = u.WeaponProficiency() + " 固定 汎用";
                        var loopTo = GeneralLib.LLength(ref iclass);
                        for (i = 1; i <= loopTo; i++)
                        {
                            if ((Class0() ?? "") == (GeneralLib.LIndex(ref iclass, i) ?? ""))
                            {
                                IsAvailableRet = true;
                                break;
                            }
                        }

                        break;
                    }

                case "盾":
                case "体":
                case "頭":
                    {
                        iclass = u.ArmorProficiency() + " 固定 汎用";
                        var loopTo1 = GeneralLib.LLength(ref iclass);
                        for (i = 1; i <= loopTo1; i++)
                        {
                            if ((Class0() ?? "") == (GeneralLib.LIndex(ref iclass, i) ?? ""))
                            {
                                IsAvailableRet = true;
                                break;
                            }
                        }

                        break;
                    }

                default:
                    {
                        // その他のアイテムは常に利用可能
                        IsAvailableRet = true;
                        break;
                    }
            }

            if (!IsAvailableRet)
            {
                return IsAvailableRet;
            }

            // 技能チェックが必要？
            string argfname = "必要技能";
            string argfname1 = "不必要技能";
            if (!IsFeatureAvailable(ref argfname) & !IsFeatureAvailable(ref argfname1))
            {
                return IsAvailableRet;
            }
            // 必要技能をチェック
            var loopTo2 = CountFeature();
            for (i = 1; i <= loopTo2; i++)
            {
                object argIndex15 = i;
                switch (Feature(ref argIndex15) ?? "")
                {
                    case "必要技能":
                        {
                            string localFeatureData1() { object argIndex1 = i; var ret = FeatureData(ref argIndex1); return ret; }

                            bool localIsNecessarySkillSatisfied() { string argnabilities = hs6a6e2af77d324e3587224eb7d4498ee9(); Pilot argp = null; var ret = u.IsNecessarySkillSatisfied(ref argnabilities, p: ref argp); return ret; }

                            if (!localIsNecessarySkillSatisfied())
                            {
                                // アイテム自身により必要技能に指定された能力が封印されていた場合は
                                // 必要技能を満たしていると判定させるため、チェックする必要がある。

                                var loopTo3 = u.CountItem();
                                for (j = 1; j <= loopTo3; j++)
                                {
                                    Item localItem() { object argIndex1 = j; var ret = u.Item(ref argIndex1); return ret; }

                                    if (ReferenceEquals(this, localItem()))
                                    {
                                        break;
                                    }
                                }

                                if (j > u.CountItem())
                                {
                                    // 既に装備しているのでなければ装備しない
                                    IsAvailableRet = false;
                                    return IsAvailableRet;
                                }

                                if (u.CountPilot() > 0)
                                {
                                    string localFeatureData() { object argIndex1 = i; var ret = FeatureData(ref argIndex1); return ret; }

                                    string argsname = localFeatureData();
                                    sname = u.MainPilot().SkillType(ref argsname);
                                }
                                else
                                {
                                    object argIndex5 = i;
                                    sname = FeatureData(ref argIndex5);
                                }

                                // 必要技能が「～装備」？
                                if (Strings.Right(sname, 2) == "装備")
                                {
                                    if ((Strings.Left(sname, Strings.Len(sname) - 2) ?? "") == (Name ?? "") | (Strings.Left(sname, Strings.Len(sname) - 2) ?? "") == (Class0() ?? ""))
                                    {
                                        goto NextLoop;
                                    }
                                }

                                // 封印する能力が必要技能になっている？
                                var loopTo4 = CountFeature();
                                for (j = 1; j <= loopTo4; j++)
                                {
                                    object argIndex6 = j;
                                    switch (Feature(ref argIndex6) ?? "")
                                    {
                                        case "パイロット能力付加":
                                        case "パイロット能力強化":
                                            {
                                                break;
                                            }

                                        default:
                                            {
                                                goto NextLoop1;
                                                break;
                                            }
                                    }

                                    // 封印する能力名
                                    object argIndex7 = j;
                                    fdata = FeatureData(ref argIndex7);
                                    if (Strings.Left(fdata, 1) == "\"")
                                    {
                                        fdata = Strings.Mid(fdata, 2, Strings.Len(fdata) - 2);
                                    }

                                    if (Strings.InStr(fdata, "=") > 0)
                                    {
                                        fdata = Strings.Left(fdata, Strings.InStr(fdata, "=") - 1);
                                    }

                                    // 必要技能と封印する能力が一致している？
                                    if ((fdata ?? "") == (sname ?? ""))
                                    {
                                        goto NextLoop;
                                    }

                                    if (u.CountPilot() > 0)
                                    {
                                        object argIndex9 = fdata;
                                        if (SRC.ALDList.IsDefined(ref argIndex9))
                                        {
                                            object argIndex8 = fdata;
                                            {
                                                var withBlock = SRC.ALDList.Item(ref argIndex8);
                                                var loopTo5 = withBlock.Count;
                                                for (k = 1; k <= loopTo5; k++)
                                                {
                                                    if ((withBlock.get_AliasType(k) ?? "") == (sname ?? ""))
                                                    {
                                                        goto NextLoop;
                                                    }
                                                }
                                            }
                                        }
                                        else if ((u.MainPilot().SkillType(ref fdata) ?? "") == (sname ?? ""))
                                        {
                                            goto NextLoop;
                                        }
                                    }

                                    NextLoop1:
                                    ;
                                }

                                // 必要技能が満たされていなかった
                                IsAvailableRet = false;
                                return IsAvailableRet;
                            }

                            break;
                        }

                    case "不必要技能":
                        {
                            string localFeatureData3() { object argIndex1 = i; var ret = FeatureData(ref argIndex1); return ret; }

                            string argnabilities = localFeatureData3();
                            Pilot argp = null;
                            if (u.IsNecessarySkillSatisfied(ref argnabilities, p: ref argp))
                            {
                                // アイテム自身により不必要技能が満たされている場合は不必要技能を
                                // 無視させるため、チェックする必要がある。

                                var loopTo6 = u.CountItem();
                                for (j = 1; j <= loopTo6; j++)
                                {
                                    Item localItem1() { object argIndex1 = j; var ret = u.Item(ref argIndex1); return ret; }

                                    if (ReferenceEquals(this, localItem1()))
                                    {
                                        break;
                                    }
                                }

                                if (j > u.CountItem())
                                {
                                    // 既に装備しているのでなければ装備しない
                                    IsAvailableRet = false;
                                    return IsAvailableRet;
                                }

                                if (u.CountPilot() > 0)
                                {
                                    string localFeatureData2() { object argIndex1 = i; var ret = FeatureData(ref argIndex1); return ret; }

                                    string argsname1 = localFeatureData2();
                                    sname = u.MainPilot().SkillType(ref argsname1);
                                }
                                else
                                {
                                    object argIndex10 = i;
                                    sname = FeatureData(ref argIndex10);
                                }

                                // 不必要技能が「～装備」？
                                if (Strings.Right(sname, 2) == "装備")
                                {
                                    if ((Strings.Left(sname, Strings.Len(sname) - 2) ?? "") == (Name ?? "") | (Strings.Left(sname, Strings.Len(sname) - 2) ?? "") == (Class0() ?? ""))
                                    {
                                        goto NextLoop;
                                    }
                                }

                                // 付加する能力が不必要技能になっている？
                                var loopTo7 = CountFeature();
                                for (j = 1; j <= loopTo7; j++)
                                {
                                    object argIndex11 = j;
                                    switch (Feature(ref argIndex11) ?? "")
                                    {
                                        case "パイロット能力付加":
                                        case "パイロット能力強化":
                                            {
                                                break;
                                            }

                                        default:
                                            {
                                                goto NextLoop2;
                                                break;
                                            }
                                    }

                                    // 付加する能力名
                                    object argIndex12 = j;
                                    fdata = FeatureData(ref argIndex12);
                                    if (Strings.Left(fdata, 1) == "\"")
                                    {
                                        fdata = Strings.Mid(fdata, 2, Strings.Len(fdata) - 2);
                                    }

                                    if (Strings.InStr(fdata, "=") > 0)
                                    {
                                        fdata = Strings.Left(fdata, Strings.InStr(fdata, "=") - 1);
                                    }

                                    // 必要技能と付加する能力が一致している？
                                    if ((fdata ?? "") == (sname ?? ""))
                                    {
                                        goto NextLoop;
                                    }

                                    if (u.CountPilot() > 0)
                                    {
                                        object argIndex14 = fdata;
                                        if (SRC.ALDList.IsDefined(ref argIndex14))
                                        {
                                            object argIndex13 = fdata;
                                            {
                                                var withBlock1 = SRC.ALDList.Item(ref argIndex13);
                                                var loopTo8 = withBlock1.Count;
                                                for (k = 1; k <= loopTo8; k++)
                                                {
                                                    if ((withBlock1.get_AliasType(k) ?? "") == (sname ?? ""))
                                                    {
                                                        goto NextLoop;
                                                    }
                                                }
                                            }
                                        }
                                        else if ((u.MainPilot().SkillType(ref fdata) ?? "") == (sname ?? ""))
                                        {
                                            goto NextLoop;
                                        }
                                    }

                                    NextLoop2:
                                    ;
                                }

                                // 不必要技能が満たされていた
                                IsAvailableRet = false;
                                return IsAvailableRet;
                            }

                            break;
                        }
                }

                NextLoop:
                ;
            }

            return IsAvailableRet;
        }


        // 一時中断用データをファイルにセーブする
        public void Dump()
        {
            FileSystem.WriteLine(SRC.SaveDataFileNumber, Name, ID, Exist);
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
            var bbuf = default(bool);

            // Name, ID, Exist
            FileSystem.Input(SRC.SaveDataFileNumber, ref sbuf);
            Name = sbuf;
            FileSystem.Input(SRC.SaveDataFileNumber, ref sbuf);
            ID = sbuf;
            FileSystem.Input(SRC.SaveDataFileNumber, ref bbuf);
            Exist = bbuf;

            // Unit
            sbuf = FileSystem.LineInput(SRC.SaveDataFileNumber);
        }

        // 一時中断用データのリンク情報をファイルからロードする
        public void RestoreLinkInfo()
        {
            string sbuf;

            // Name, ID, Exist
            sbuf = FileSystem.LineInput(SRC.SaveDataFileNumber);

            // Unit
            FileSystem.Input(SRC.SaveDataFileNumber, ref sbuf);
            object argIndex2 = sbuf;
            if (SRC.UList.IsDefined(ref argIndex2))
            {
                object argIndex1 = sbuf;
                Unit_Renamed = SRC.UList.Item(ref argIndex1);
            }
        }

        // '一時中断用データのパラメータ情報をファイルからロードする
        public void RestoreParameter()
        {
            string sbuf;

            // Name, ID, Exist
            sbuf = FileSystem.LineInput(SRC.SaveDataFileNumber);

            // Unit
            sbuf = FileSystem.LineInput(SRC.SaveDataFileNumber);
        }
    }
}