using SRCCore.Lib;
using SRCCore.VB;
using System;
using System.Linq;

namespace SRCCore.Units
{
    // === 各種基本ステータス ===
    public partial class Unit
    {
        // ユニット名称
        public string Name
        {
            get
            {
                return strName;
            }

            set
            {
                strName = value;
                Data = SRC.UDList.Item(value);
                Update();
            }
        }

        // 愛称
        public string Nickname0
        {
            get
            {
                string Nickname0Ret = default;
                int idx;
                Unit u;
                Nickname0Ret = Data.Nickname;

                // 愛称変更能力による変更
                if (IsFeatureAvailable("愛称変更"))
                {
                    Nickname0Ret = FeatureData("愛称変更");
                    idx = (int)Strings.InStr(Nickname0Ret, "$(愛称)");
                    if (idx > 0)
                    {
                        Nickname0Ret = Strings.Left(Nickname0Ret, idx - 1) + Data.Nickname + Strings.Mid(Nickname0Ret, idx + 5);
                    }

                    idx = (int)Strings.InStr(Nickname0Ret, "$(パイロット愛称)");
                    if (idx > 0)
                    {
                        if (CountPilot() > 0)
                        {
                            Nickname0Ret = Strings.Left(Nickname0Ret, idx - 1) + MainPilot().get_Nickname(true) + Strings.Mid(Nickname0Ret, idx + 10);
                        }
                        else
                        {
                            Nickname0Ret = Strings.Left(Nickname0Ret, idx - 1) + "○○" + Strings.Mid(Nickname0Ret, idx + 10);
                        }
                    }
                }

                u = Event.SelectedUnitForEvent;
                Event.SelectedUnitForEvent = this;
                Expression.ReplaceSubExpression(ref Nickname0Ret);
                Event.SelectedUnitForEvent = u;
                return Nickname0Ret;
            }
        }

        // メッセージ中で表示する際の愛称は等身大基準ではパイロット名を使う
        public string Nickname
        {
            get
            {
                string NicknameRet = default;
                if (Expression.IsOptionDefined("等身大基準"))
                {
                    if (CountPilot() > 0)
                    {
                        {
                            var withBlock = MainPilot();
                            if (Strings.InStr(withBlock.Name, "(ザコ)") == 0 && Strings.InStr(withBlock.Name, "(汎用)") == 0)
                            {
                                NicknameRet = MainPilot().get_Nickname(false);
                                return default;
                            }
                        }
                    }
                }

                NicknameRet = Nickname0;
                return NicknameRet;
            }
        }

        // 読み仮名
        public string KanaName
        {
            get
            {
                string KanaNameRet = Data.KanaName;
                int idx;

                // 読み仮名変更能力による変更
                if (IsFeatureAvailable("読み仮名変更"))
                {
                    KanaNameRet = FeatureData("読み仮名変更");
                    idx = (int)Strings.InStr(KanaNameRet, "$(読み仮名)");
                    if (idx > 0)
                    {
                        KanaNameRet = Strings.Left(KanaNameRet, idx - 1) + Data.KanaName + Strings.Mid(KanaNameRet, idx + 5);
                    }

                    idx = (int)Strings.InStr(KanaNameRet, "$(パイロット読み仮名)");
                    if (idx > 0)
                    {
                        if (CountPilot() > 0)
                        {
                            KanaNameRet = Strings.Left(KanaNameRet, idx - 1) + MainPilot().KanaName + Strings.Mid(KanaNameRet, idx + 10);
                        }
                        else
                        {
                            KanaNameRet = Strings.Left(KanaNameRet, idx - 1) + "○○" + Strings.Mid(KanaNameRet, idx + 10);
                        }
                    }
                }
                else if (IsFeatureAvailable("愛称変更"))
                {
                    KanaNameRet = FeatureData("愛称変更");
                    idx = (int)Strings.InStr(KanaNameRet, "$(愛称)");
                    if (idx > 0)
                    {
                        KanaNameRet = Strings.Left(KanaNameRet, idx - 1) + Data.Nickname + Strings.Mid(KanaNameRet, idx + 5);
                    }

                    idx = (int)Strings.InStr(KanaNameRet, "$(パイロット愛称)");
                    if (idx > 0)
                    {
                        if (CountPilot() > 0)
                        {
                            KanaNameRet = Strings.Left(KanaNameRet, idx - 1) + MainPilot().get_Nickname(false) + Strings.Mid(KanaNameRet, idx + 10);
                        }
                        else
                        {
                            KanaNameRet = Strings.Left(KanaNameRet, idx - 1) + "○○" + Strings.Mid(KanaNameRet, idx + 10);
                        }
                    }

                    KanaNameRet = GeneralLib.StrToHiragana(KanaNameRet);
                }

                var u = Event.SelectedUnitForEvent;
                Event.SelectedUnitForEvent = this;
                Expression.ReplaceSubExpression(ref KanaNameRet);
                Event.SelectedUnitForEvent = u;
                return KanaNameRet;
            }
        }

        // ユニットランク

        public int Rank
        {
            get => intRank;
            set
            {
                if (intRank == value)
                {
                    return;
                }

                intRank = value;
                if (intRank > 999)
                {
                    intRank = 999;
                }

                // パラメータを更新
                Update();
            }
        }

        public void RankUp(int incrRank = 1, double hp_ratio = 1d, double en_ratio = 1d)
        {
            Rank = Rank + incrRank;
            HP = (int)(MaxHP * hp_ratio);
            EN = (int)(MaxEN * en_ratio);

            // 他形態のランクも上げておく
            foreach (var of in OtherForms)
            {
                of.Rank = Rank;
            }
            // 合体形態が主形態の分離形態が改造された場合は他の分離形態のユニットの
            // ランクも上げる
            if (IsFeatureAvailable("合体"))
            {
                var mu = Features.Where(fd => fd.Name == "合体")
                    .Select(fd => new
                    {
                        u = SRC.UDList.Item(GeneralLib.LIndex(fd.Data, 2)),
                        fd = fd,
                    })
                    .Where(x => x.u != null)
                    .Where(x => (GeneralLib.LLength(x.fd.Data) == 3 && x.u.IsFeatureAvailable("主形態"))
                        || (GeneralLib.LLength(x.fd.Data) != 3 && !x.u.IsFeatureAvailable("制限時間")))
                    .Select(x => x.u)
                    .FirstOrDefault();
                if (mu != null)
                {
                    if (mu.IsFeatureAvailable("分離"))
                    {
                        var buf = mu.FeatureData("分離");
                        foreach (var pu in GeneralLib.ToList(buf).Skip(1).Select(x => SRC.UList.Item(x)).Where(x => x != null))
                        {
                            pu.Rank = GeneralLib.MaxLng(Rank, pu.Rank);
                            foreach (var of in pu.OtherForms)
                            {
                                of.Rank = pu.Rank;
                                pu.HP = (int)(pu.MaxHP * hp_ratio);
                                pu.EN = (int)(pu.MaxEN * en_ratio);
                            }
                        }
                    }
                }
            }

            // 合体ユニットの場合は分離形態のユニットのランクも上げる
            if (IsFeatureAvailable("分離"))
            {
                var buf = FeatureData("分離");
                foreach (var pu in GeneralLib.ToList(buf).Skip(1).Select(x => SRC.UList.Item(x)).Where(x => x != null))
                {
                    pu.Rank = GeneralLib.MaxLng(Rank, pu.Rank);
                    foreach (var of in pu.OtherForms)
                    {
                        of.Rank = pu.Rank;
                        pu.HP = (int)(pu.MaxHP * hp_ratio);
                        pu.EN = (int)(pu.MaxEN * en_ratio);
                    }
                }
            }
        }

        // ボスランク
        public int BossRank
        {
            get => intBossRank;
            set
            {
                if (intBossRank == value)
                {
                    return;
                }

                intBossRank = value;

                // パラメータを更新
                Update();
            }
        }

        // ユニットクラス
        public string Class => Data.Class;

        // ユニットクラスから余分な指定を除いたもの
        public string Class0
        {
            get
            {
                string Class0Ret = default;
                int i, n;
                Class0Ret = Data.Class;

                // 人間ユニット指定を削除
                if (Strings.Left(Class0Ret, 1) == "(")
                {
                    Class0Ret = Strings.Mid(Class0Ret, 2, Strings.Len(Class0Ret) - 2);
                }

                // 専用指定を削除
                if (Strings.Right(Class0Ret, 3) == "専用)")
                {
                    n = 1;
                    i = (int)(Strings.Len(Class0Ret) - 2);
                    do
                    {
                        i = (int)(i - 1);
                        switch (Strings.Mid(Class0Ret, i, 1) ?? "")
                        {
                            case "(":
                                {
                                    n = (int)(n - 1);
                                    if (n == 0)
                                    {
                                        Class0Ret = Strings.Left(Class0Ret, i - 1);
                                        break;
                                    }

                                    break;
                                }

                            case ")":
                                {
                                    n = (int)(n + 1);
                                    break;
                                }
                        }
                    }
                    while (i > 1);
                }

                return Class0Ret;
            }
        }

        // ユニットサイズ
        public string Size
        {
            get
            {
                if (IsFeatureAvailable("サイズ変更"))
                {
                    return FeatureData("サイズ変更");
                }

                return Data.Size;
            }
        }

        // 最大ＨＰ値
        public int MaxHP
        {
            get
            {
                int MaxHPRet = lngMaxHP;

                // パイロットによる修正
                if (CountPilot() > 0)
                {
                    // 霊力変換器装備ユニットは霊力に応じて最大ＨＰが変化
                    if (IsFeatureAvailable("霊力変換器"))
                    {
                        MaxHPRet = (int)(MaxHPRet + 10d * PlanaLevel());
                    }

                    // オーラ変換器装備ユニットはオーラレベルに応じて最大ＨＰが変化
                    if (IsFeatureAvailable("オーラ変換器"))
                    {
                        MaxHPRet = (int)(MaxHPRet + 100d * AuraLevel());
                    }
                }

                // 最大ＨＰは最低でも1
                if (MaxHPRet < 1)
                {
                    MaxHPRet = 1;
                }

                return MaxHPRet;
            }
        }

        // 最大ＥＮ値
        public int MaxEN
        {
            get
            {
                int MaxENRet = intMaxEN;

                // パイロットによる修正
                if (CountPilot() > 0)
                {
                    // 霊力変換器装備ユニットは霊力に応じて最大ＥＮが変化
                    if (IsFeatureAvailable("霊力変換器"))
                    {
                        MaxENRet = (int)(MaxENRet + 0.5d * PlanaLevel());
                    }

                    // オーラ変換器装備ユニットはオーラレベルに応じて最大ＥＮが変化
                    if (IsFeatureAvailable("オーラ変換器"))
                    {
                        MaxENRet = (int)(MaxENRet + 10d * AuraLevel());
                    }
                }

                // 最大ＥＮは最低でも5
                if (MaxENRet < 5)
                {
                    MaxENRet = 5;
                }

                return MaxENRet;
            }
        }

        // ＨＰ
        public int HP
        {
            get
            {
                return lngHP;
            }

            set
            {
                lngHP = value;
                if (lngHP > MaxHP)
                {
                    lngHP = MaxHP;
                }
                else if (lngHP < 0)
                {
                    lngHP = 0;
                }
            }
        }

        //// ＥＮ
        public int EN
        {
            get => intEN;
            set
            {
                intEN = value;
                if (intEN > MaxEN)
                {
                    intEN = MaxEN;
                }
                else if (intEN < 0)
                {
                    intEN = 0;
                }
            }
        }

        // 移動力
        public int Speed0 => intSpeed;

        public int Speed
        {
            get
            {
                int SpeedRet = Speed0;

                // 元々移動力が0の場合は0のまま
                if (SpeedRet == 0 && !IsFeatureAvailable("テレポート") && !IsFeatureAvailable("ジャンプ"))
                {
                    return 0;
                }

                // 特殊状態による移動力修正
                if (IsUnderSpecialPowerEffect("移動力強化"))
                {
                    SpeedRet = (int)(SpeedRet + SpecialPowerEffectLevel("移動力強化"));
                }
                else if (IsConditionSatisfied("移動力ＵＰ"))
                {
                    if (Expression.IsOptionDefined("大型マップ"))
                    {
                        SpeedRet = SpeedRet + 2;
                    }
                    else
                    {
                        SpeedRet = SpeedRet + 1;
                    }
                }

                if (IsConditionSatisfied("移動力ＤＯＷＮ"))
                {
                    SpeedRet = GeneralLib.MaxLng(SpeedRet / 2, 1);
                }

                // 霊力による移動力ＵＰ
                if (IsFeatureAvailable("霊力変換器"))
                {
                    if (CountPilot() > 0)
                    {
                        SpeedRet = (int)(SpeedRet + 0.01d * PlanaLevel());
                    }
                }

                // スペシャルパワーによる移動力低下
                if (IsUnderSpecialPowerEffect("移動力低下"))
                {
                    SpeedRet = GeneralLib.MaxLng(SpeedRet / 2, 1);
                }

                // 移動不能の場合は移動力０
                if (IsConditionSatisfied("移動不能"))
                {
                    SpeedRet = 0;
                }

                // ＥＮ切れにより移動できない場合
                if (Status == "出撃")
                {
                    switch (Area ?? "")
                    {
                        case "空中":
                        case "宇宙":
                            {
                                if (EN < 5)
                                {
                                    SpeedRet = 0;
                                }

                                break;
                            }

                        case "地中":
                            {
                                if (EN < 10)
                                {
                                    SpeedRet = 0;
                                }

                                break;
                            }
                    }
                }

                return SpeedRet;
            }
        }


        // 移動形態
        public string Transportation
        {
            get
            {
                string TransportationRet = Data.Transportation;

                // 特殊能力による移動可能地形追加
                if (IsFeatureAvailable("空中移動"))
                {
                    if (Strings.InStr(TransportationRet, "空") == 0)
                    {
                        TransportationRet = "空" + TransportationRet;
                    }
                }

                if (IsFeatureAvailable("陸上移動"))
                {
                    if (Strings.InStr(TransportationRet, "陸") == 0)
                    {
                        TransportationRet = "陸" + TransportationRet;
                    }
                }

                if (IsFeatureAvailable("水中移動"))
                {
                    if (Strings.InStr(TransportationRet, "水") == 0)
                    {
                        TransportationRet = "水" + TransportationRet;
                    }
                }

                if (IsFeatureAvailable("地中移動"))
                {
                    if (Strings.InStr(TransportationRet, "地中") == 0)
                    {
                        TransportationRet = TransportationRet + "地中";
                    }
                }

                if (IsFeatureAvailable("宇宙移動"))
                {
                    if (string.IsNullOrEmpty(TransportationRet))
                    {
                        TransportationRet = "宇宙";
                    }
                }

                return TransportationRet;
            }
        }


        // 地形適応
        public int get_Adaption(int idx)
        {
            int AdaptionRet = default;
            int uad = default, pad = default;

            // ユニット側の地形適応を算出
            switch (Strings.Mid(strAdaption, idx, 1) ?? "")
            {
                case "S":
                    {
                        uad = 5;
                        break;
                    }

                case "A":
                    {
                        uad = 4;
                        break;
                    }

                case "B":
                    {
                        uad = 3;
                        break;
                    }

                case "C":
                    {
                        uad = 2;
                        break;
                    }

                case "D":
                    {
                        uad = 1;
                        break;
                    }

                case "-":
                    {
                        AdaptionRet = 0;
                        return default;
                    }
            }

            // パイロット側の地形適応を算出
            if (CountPilot() > 0)
            {
                switch (Strings.Mid(MainPilot().Adaption, idx, 1) ?? "")
                {
                    case "S":
                        {
                            pad = 5;
                            break;
                        }

                    case "A":
                        {
                            pad = 4;
                            break;
                        }

                    case "B":
                        {
                            pad = 3;
                            break;
                        }

                    case "C":
                        {
                            pad = 2;
                            break;
                        }

                    case "D":
                        {
                            pad = 1;
                            break;
                        }

                    case "-":
                        {
                            AdaptionRet = 0;
                            return default;
                        }
                }
            }
            else
            {
                pad = 4;
            }

            if (Expression.IsOptionDefined("地形適応総和計算"))
            {
                // ユニットとパイロットの地形適応の総和から地形適応を決定
                switch ((int)(uad + pad))
                {
                    case 9:
                    case 10:
                        {
                            AdaptionRet = 5;
                            break;
                        }

                    case 7:
                    case 8:
                        {
                            AdaptionRet = 4;
                            break;
                        }

                    case 5:
                    case 6:
                        {
                            AdaptionRet = 3;
                            break;
                        }

                    case 3:
                    case 4:
                        {
                            AdaptionRet = 2;
                            break;
                        }

                    case 1:
                    case 2:
                        {
                            AdaptionRet = 1;
                            break;
                        }

                    default:
                        {
                            AdaptionRet = 0;
                            break;
                        }
                }
            }
            // ユニットとパイロットの地形適応の低い方を使用
            else if (uad > pad)
            {
                AdaptionRet = pad;
            }
            else
            {
                AdaptionRet = uad;
            }

            return AdaptionRet;
        }

        // 地形適応による修正値
        public double get_AdaptionMod(int idx, int ad_mod)
        {
            double AdaptionModRet = default;
            int uad;
            uad = get_Adaption(idx);

            // 元々属性がSでない限り、Sにはしない
            if (uad == 5)
            {
                uad = (int)GeneralLib.MinLng(uad + ad_mod, 5);
            }
            else
            {
                uad = (int)GeneralLib.MinLng(uad + ad_mod, 4);
            }

            // Optionコマンドの設定に応じて適応修正値を設定
            if (Expression.IsOptionDefined("地形適応修正緩和"))
            {
                if (Expression.IsOptionDefined("地形適応修正繰り下げ"))
                {
                    switch (uad)
                    {
                        case 5:
                            {
                                AdaptionModRet = 1.1d;
                                break;
                            }

                        case 4:
                            {
                                AdaptionModRet = 1d;
                                break;
                            }

                        case 3:
                            {
                                AdaptionModRet = 0.9d;
                                break;
                            }

                        case 2:
                            {
                                AdaptionModRet = 0.8d;
                                break;
                            }

                        case 1:
                            {
                                AdaptionModRet = 0.7d;
                                break;
                            }

                        default:
                            {
                                AdaptionModRet = 0d;
                                break;
                            }
                    }
                }
                else
                {
                    switch (uad)
                    {
                        case 5:
                            {
                                AdaptionModRet = 1.2d;
                                break;
                            }

                        case 4:
                            {
                                AdaptionModRet = 1.1d;
                                break;
                            }

                        case 3:
                            {
                                AdaptionModRet = 1d;
                                break;
                            }

                        case 2:
                            {
                                AdaptionModRet = 0.9d;
                                break;
                            }

                        case 1:
                            {
                                AdaptionModRet = 0.8d;
                                break;
                            }

                        default:
                            {
                                AdaptionModRet = 0d;
                                break;
                            }
                    }
                }
            }
            else
            {
                if (Expression.IsOptionDefined("地形適応修正繰り下げ"))
                {
                    switch (uad)
                    {
                        case 5:
                            {
                                AdaptionModRet = 1.2d;
                                break;
                            }

                        case 4:
                            {
                                AdaptionModRet = 1d;
                                break;
                            }

                        case 3:
                            {
                                AdaptionModRet = 0.8d;
                                break;
                            }

                        case 2:
                            {
                                AdaptionModRet = 0.6d;
                                break;
                            }

                        case 1:
                            {
                                AdaptionModRet = 0.4d;
                                break;
                            }

                        default:
                            {
                                AdaptionModRet = 0d;
                                break;
                            }
                    }
                }
                else
                {
                    switch (uad)
                    {
                        case 5:
                            {
                                AdaptionModRet = 1.4d;
                                break;
                            }

                        case 4:
                            {
                                AdaptionModRet = 1.2d;
                                break;
                            }

                        case 3:
                            {
                                AdaptionModRet = 1d;
                                break;
                            }

                        case 2:
                            {
                                AdaptionModRet = 0.8d;
                                break;
                            }

                        case 1:
                            {
                                AdaptionModRet = 0.6d;
                                break;
                            }

                        default:
                            {
                                AdaptionModRet = 0d;
                                break;
                            }
                    }
                }
            }

            return AdaptionModRet;
        }


        // 装甲
        public int get_Armor(string ref_mode)
        {
            int ArmorRet = lngArmor;
            // ステータス表示用
            switch (ref_mode ?? "")
            {
                case "基本値":
                    {
                        if (IsConditionSatisfied("装甲劣化"))
                        {
                            ArmorRet = ArmorRet / 2;
                        }

                        if (IsConditionSatisfied("石化"))
                        {
                            ArmorRet = 2 * ArmorRet;
                        }

                        if (IsConditionSatisfied("凍結"))
                        {
                            ArmorRet = ArmorRet / 2;
                        }

                        return ArmorRet;
                    }

                case "修正値":
                    {
                        ArmorRet = 0;
                        break;
                    }
            }

            // パイロットによる修正
            if (CountPilot() > 0)
            {
                // 霊力による装甲修正
                if (IsFeatureAvailable("霊力変換器"))
                {
                    ArmorRet = (int)(ArmorRet + 5d * PlanaLevel());
                }

                // サイキックドライブ装備ユニットは超能力レベルに応じて装甲が変化
                if (IsFeatureAvailable("サイキックドライブ"))
                {
                    ArmorRet = (int)(ArmorRet + 100d * PsychicLevel());
                }

                // オーラ変換器装備ユニットはオーラレベルに応じて装甲が変化
                if (IsFeatureAvailable("オーラ変換器"))
                {
                    ArmorRet = (int)(ArmorRet + 50d * AuraLevel());
                }
            }

            // 装甲が劣化している場合は装甲値は半減
            if (IsConditionSatisfied("装甲劣化"))
            {
                ArmorRet = ArmorRet / 2;
            }

            // 石化しているユニットはとても固い……
            if (IsConditionSatisfied("石化"))
            {
                ArmorRet = 2 * ArmorRet;
            }

            // 凍っているユニットは脆くなる
            if (IsConditionSatisfied("凍結"))
            {
                ArmorRet = ArmorRet / 2;
            }

            return ArmorRet;
        }

        // 運動性
        public int get_Mobility(string ref_mode)
        {
            int MobilityRet = intMobility;
            switch (ref_mode ?? "")
            {
                case "基本値":
                    {
                        return MobilityRet;
                    }

                case "修正値":
                    {
                        MobilityRet = 0;
                        break;
                    }
            }

            // パイロットによる修正
            if (CountPilot() > 0)
            {
                // サイキックドライブ装備ユニットは超能力レベルに応じて運動性が変化
                if (IsFeatureAvailable("サイキックドライブ"))
                {
                    MobilityRet = (int)(MobilityRet + 5d * PsychicLevel());
                }

                // オーラ変換器装備ユニットはオーラレベルに応じて運動性が変化
                if (IsFeatureAvailable("オーラ変換器"))
                {
                    MobilityRet = (int)(MobilityRet + 2d * AuraLevel());
                }

                // シンクロドライブ装備ユニットは同調率レベルに応じて運動性が変化
                if (IsFeatureAvailable("シンクロドライブ"))
                {
                    if (MainPilot().SynchroRate() > 0)
                    {
                        MobilityRet = (int)(MobilityRet + (long)(SyncLevel() - 50d) / 2L);
                    }
                }
            }

            return MobilityRet;
        }

        // 描画すべきビットマップ
        public string CurrentBitmap()
        {
            if (IsConditionSatisfied("非表示付加"))
            {
                return "-.bmp";
            }
            // TODO Unitフォルダ以外の走査
            return SRC.FileSystem.PathCombine("Unit", CurrentForm().get_Bitmap(false));
        }

        // ビットマップ
        public string get_Bitmap(bool use_orig)
        {
            if (IsConditionSatisfied("ユニット画像"))
            {
                return GeneralLib.LIndex(Condition("ユニット画像").StrData, 2);
            }

            if (IsFeatureAvailable("ユニット画像"))
            {
                return FeatureData("ユニット画像");
            }

            if (use_orig)
            {
                return Data.Bitmap0;
            }
            else
            {
                return Data.Bitmap;
            }
        }

        // 修理費(獲得資金)
        public int Value
        {
            get
            {
                int ValueRet = Data.Value;
                if (IsFeatureAvailable("修理費修正"))
                {
                    ValueRet = GeneralLib.MaxLng((int)(ValueRet + 1000d * FeatureLevel("修理費修正")), 0);
                }

                if (BossRank > 0)
                {
                    ValueRet = (int)(ValueRet * (1d + 0.5d * BossRank + 0.05d * Rank));
                }
                else
                {
                    ValueRet = (int)(ValueRet * (1d + 0.05d * Rank));
                }

                return ValueRet;
            }
        }

        // 経験値
        public int ExpValue
        {
            get
            {
                int ExpValueRet = Data.ExpValue;
                if (IsFeatureAvailable("経験値修正"))
                {
                    ExpValueRet = GeneralLib.MaxLng((int)(ExpValueRet + 10d * FeatureLevel("経験値修正")), 0);
                }

                if (BossRank > 0)
                {
                    ExpValueRet = ExpValueRet + 20 * BossRank;
                }

                return ExpValueRet;
            }
        }

        // 残り行動数
        public int Action => Math.Max(MaxAction() - UsedAction, 0);
    }
}
