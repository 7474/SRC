// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Lib;
using SRCCore.Maps;
using SRCCore.Models;
using SRCCore.VB;
using System;
using System.Linq;

namespace SRCCore.Units
{
    // === 武器関連処理 ===
    public class UnitWeapon
    {
        public Unit Unit { get; }
        public WeaponData WeaponData { get; }
        public WeaponData UpdatedWeaponData { get; private set; }

        private SRC SRC;
        private Events.Event Event => SRC.Event;
        private Map Map => SRC.Map;
        private Expressions.Expression Expression => SRC.Expression;
        private Commands.Command Commands => SRC.Commands;

        // 近接武器か
        private bool IsCrossRange()
        {
            return IsWeaponClassifiedAs("武") || IsWeaponClassifiedAs("突") || IsWeaponClassifiedAs("接");
        }

        // ユニットの何番目の武器か
        private int WeaponNo()
        {
            return Unit.Weapons.IndexOf(this) + 1;
        }

        // 武器
        public UnitWeapon(SRC src, Unit u, WeaponData wd)
        {
            SRC = src;

            Unit = u;
            WeaponData = wd;
            // 既定値として入れておく
            UpdatedWeaponData = wd;
        }

        public string Name => WeaponData.Name;

        // 武器の愛称
        public string WeaponNickname()
        {
            // 愛称内の式置換のため、デフォルトユニットを一時的に変更する
            var lastU = Event.SelectedUnitForEvent;
            try
            {
                Event.SelectedUnitForEvent = Unit;
                return WeaponData.Nickname();
            }
            finally
            {
                Event.SelectedUnitForEvent = lastU;
            }
        }

        // 武器の攻撃力
        // tarea は敵のいる地形
        public int WeaponPower(string tarea)
        {
            //int pat;
            //// 攻撃補正一時保存
            //double ed_atk;
            int WeaponPowerRet = UpdatedWeaponData.Power;

            // 「体」属性を持つ武器は残りＨＰに応じて攻撃力が増える
            if (IsWeaponClassifiedAs("体"))
            {
                WeaponPowerRet = (int)(WeaponPowerRet + Unit.HP / (double)Unit.MaxHP * 100d * WeaponLevel("体"));
            }

            // 「尽」属性を持つ武器は残りＥＮに応じて攻撃力が増える
            if (IsWeaponClassifiedAs("尽"))
            {
                if (Unit.EN >= WeaponENConsumption())
                {
                    WeaponPowerRet = (int)(WeaponPowerRet + (Unit.EN - WeaponENConsumption()) * WeaponLevel("尽"));
                }
            }

            // ダメージ固定武器
            double wad;
            if (IsWeaponClassifiedAs("固"))
            {
                // 武器一覧の場合は攻撃力をそのまま表示
                if (string.IsNullOrEmpty(tarea))
                {
                    return WeaponPowerRet;
                }

                // マップ攻撃は攻撃開始時に保存した攻撃力をそのまま使う
                if (IsWeaponClassifiedAs("Ｍ"))
                {
                    // XXX この辺の参照考えると UnitWeapon として切り出したのイマイチだったかもなぁ。
                    if (Unit.SelectedMapAttackPower > 0)
                    {
                        WeaponPowerRet = Unit.SelectedMapAttackPower;
                    }
                }

                // 地形適応による修正のみを適用
                wad = WeaponAdaption(tarea);

                // 地形適応修正繰り下げオプションの効果は適用しない
                if (Expression.IsOptionDefined("地形適応修正繰り下げ"))
                {
                    if (Expression.IsOptionDefined("地形適応修正緩和"))
                    {
                        wad = wad + 0.1d;
                    }
                    else
                    {
                        wad = wad + 0.2d;
                    }
                }

                // 地形適応がＡの場合に攻撃力と同じダメージを与えるようにする
                if (Expression.IsOptionDefined("地形適応修正緩和"))
                {
                    wad = wad - 0.1d;
                }
                else
                {
                    wad = wad - 0.2d;
                }

                if (wad > 0d)
                {
                    WeaponPowerRet = (int)(WeaponPowerRet * wad);
                }
                else
                {
                    WeaponPowerRet = 0;
                }

                return WeaponPowerRet;
            }

            // 部隊ユニットはダメージを受けると攻撃力が低下
            if (Unit.IsFeatureAvailable("部隊ユニット"))
            {
                WeaponPowerRet = (int)((WeaponPowerRet * (50d + 50 * Unit.HP / (double)Unit.MaxHP)) / 100);
            }

            // 標的のいる地形が設定されていないときは武器の一覧表示用なので各種補正を省く
            if (string.IsNullOrEmpty(tarea))
            {
                return WeaponPowerRet;
            }

            {
                var pilot = Unit.MainPilot();
                // TODO Impl
                //if (SRC.BCList.IsDefined("攻撃補正"))
                //{
                //    // バトルコンフィグデータの設定による修正
                //    if (IsWeaponClassifiedAs("複"))
                //    {
                //        pat = (pilot.Infight + pilot.Shooting) / 2;
                //    }
                //    else if (IsWeaponClassifiedAs("格闘系"))
                //    {
                //        pat = pilot.Infight;
                //    }
                //    else
                //    {
                //        pat = pilot.Shooting;
                //    }

                //    // 事前にデータを登録
                //    BCVariable.DataReset();
                //    BCVariable.MeUnit = this;
                //    BCVariable.AtkUnit = this;
                //    // UPGRADE_NOTE: オブジェクト BCVariable.DefUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                //    BCVariable.DefUnit = null;
                //    BCVariable.WeaponNumber = w;
                //    BCVariable.AttackExp = pat;
                //    BCVariable.WeaponPower = WeaponPowerRet;
                //    WeaponPowerRet = SRC.BCList.Item("攻撃補正").Calculate();
                //}
                //else
                {
                    // パイロットの攻撃力による修正
                    if (IsWeaponClassifiedAs("複"))
                    {
                        WeaponPowerRet = WeaponPowerRet * (pilot.Infight + pilot.Shooting) / 200;
                    }
                    else if (IsWeaponClassifiedAs("格闘系"))
                    {
                        WeaponPowerRet = WeaponPowerRet * pilot.Infight / 100;
                    }
                    else
                    {
                        WeaponPowerRet = WeaponPowerRet * pilot.Shooting / 100;
                    }

                    // 気力による修正
                    if (Expression.IsOptionDefined("気力効果小"))
                    {
                        WeaponPowerRet = WeaponPowerRet * (50 + (pilot.Morale + pilot.MoraleMod) / 2) / 100;
                    }
                    else
                    {
                        WeaponPowerRet = WeaponPowerRet * (pilot.Morale + pilot.MoraleMod) / 100;
                    }
                }

                // 覚悟
                if (Unit.HP <= Unit.MaxHP / 4)
                {
                    if (pilot.IsSkillAvailable("覚悟"))
                    {
                        if (Expression.IsOptionDefined("ダメージ倍率低下"))
                        {
                            WeaponPowerRet = (int)(1.1d * WeaponPowerRet);
                        }
                        else
                        {
                            WeaponPowerRet = (int)(1.2d * WeaponPowerRet);
                        }
                    }
                }
            }

            // マップ攻撃用に攻撃力算出
            if (tarea == "初期値")
            {
                return WeaponPowerRet;
            }

            // マップ攻撃は攻撃開始時に保存した攻撃力をそのまま使う
            if (IsWeaponClassifiedAs("Ｍ"))
            {
                if (Unit.SelectedMapAttackPower > 0)
                {
                    WeaponPowerRet = Unit.SelectedMapAttackPower;
                }
            }

            // 地形補正
            // TODO Impl
            //if (SRC.BCList.IsDefined("攻撃地形補正"))
            //{
            //    // 事前にデータを登録
            //    BCVariable.DataReset();
            //    BCVariable.MeUnit = this;
            //    BCVariable.AtkUnit = this;
            //    BCVariable.DefUnit = null;
            //    BCVariable.WeaponNumber = w;
            //    BCVariable.AttackExp = WeaponPowerRet;
            //    BCVariable.TerrainAdaption = WeaponAdaption(tarea);
            //    WeaponPowerRet = SRC.BCList.Item("攻撃地形補正").Calculate();
            //}
            //else
            {
                WeaponPowerRet = (int)(WeaponPowerRet * WeaponAdaption(tarea));
            }

            return WeaponPowerRet;
        }

        // 武器 w の地形 tarea におけるダメージ修正値
        public double WeaponAdaption(string tarea)
        {
            return 1d;
            // TODO Impl
            //    double WeaponAdaptionRet = default;
            //    int wad = default, uad, xad;
            //    int ind;

            //    // 武器の地形適応値の計算に使用する適応値を決定
            //    switch (tarea ?? "")
            //    {
            //        case "空中":
            //            {
            //                ind = 1;
            //                break;
            //            }

            //        case "地上":
            //            {
            //                if (Map.TerrainClass(x, y) == "月面")
            //                {
            //                    ind = 4;
            //                }
            //                else
            //                {
            //                    ind = 2;
            //                }

            //                break;
            //            }

            //        case "水上":
            //            {
            //                if (Strings.Mid(Weapon(w).Adaption, 3, 1) == "A")
            //                {
            //                    ind = 3;
            //                }
            //                else
            //                {
            //                    ind = 2;
            //                }

            //                break;
            //            }

            //        case "水中":
            //            {
            //                ind = 3;
            //                break;
            //            }

            //        case "宇宙":
            //            {
            //                ind = 4;
            //                break;
            //            }

            //        case "地中":
            //            {
            //                WeaponAdaptionRet = 0d;
            //                return WeaponAdaptionRet;
            //            }

            //        default:
            //            {
            //                xad = 4;
            //                goto CalcAdaption;
            //                break;
            //            }
            //    }

            //    // 武器の地形適応値
            //    switch (Strings.Mid(Weapon(w).Adaption, ind, 1) ?? "")
            //    {
            //        case "S":
            //            {
            //                wad = 5;
            //                break;
            //            }

            //        case "A":
            //            {
            //                wad = 4;
            //                break;
            //            }

            //        case "B":
            //            {
            //                wad = 3;
            //                break;
            //            }

            //        case "C":
            //            {
            //                wad = 2;
            //                break;
            //            }

            //        case "D":
            //            {
            //                wad = 1;
            //                break;
            //            }

            //        case "-":
            //            {
            //                WeaponAdaptionRet = 0d;
            //                return WeaponAdaptionRet;
            //            }
            //    }

            //    // ユニットの地形適応値の計算に使用する適応値を決定
            //    string argattr4 = "武";
            //    string argattr5 = "突";
            //    string argattr6 = "接";
            //    if (!IsWeaponClassifiedAs(argattr4) & !IsWeaponClassifiedAs(argattr5) & !IsWeaponClassifiedAs(argattr6))
            //    {
            //        // 格闘戦以外の場合はユニットがいる地形を参照
            //        switch (Area ?? "")
            //        {
            //            case "空中":
            //                {
            //                    ind = 1;
            //                    break;
            //                }

            //            case "地上":
            //                {
            //                    if (Map.TerrainClass(x, y) == "月面")
            //                    {
            //                        ind = 4;
            //                    }
            //                    else
            //                    {
            //                        ind = 2;
            //                    }

            //                    break;
            //                }

            //            case "水上":
            //                {
            //                    ind = 2;
            //                    break;
            //                }

            //            case "水中":
            //                {
            //                    ind = 3;
            //                    break;
            //                }

            //            case "宇宙":
            //                {
            //                    ind = 4;
            //                    break;
            //                }

            //            case "地中":
            //                {
            //                    WeaponAdaptionRet = 0d;
            //                    return WeaponAdaptionRet;
            //                }
            //        }
            //        // ユニットの地形適応値
            //        uad = get_Adaption(ind);
            //    }
            //    else
            //    {
            //        // 格闘戦の場合はターゲットがいる地形を参照
            //        switch (tarea ?? "")
            //        {
            //            case "空中":
            //                {
            //                    uad = get_Adaption(1);
            //                    // ジャンプ攻撃
            //                    string argattr1 = "Ｊ";
            //                    if (IsWeaponClassifiedAs(argattr1))
            //                    {
            //                        string argattr = "Ｊ";
            //                        uad = (uad + WeaponLevel(argattr));
            //                    }

            //                    break;
            //                }

            //            case "地上":
            //                {
            //                    if (get_Adaption(2) > 0)
            //                    {
            //                        uad = get_Adaption(2);
            //                    }
            //                    else
            //                    {
            //                        // 空中専用ユニットが地上のユニットに格闘戦をしかけられるようにする
            //                        uad = GeneralLib.MaxLng(get_Adaption(1) - 1, 0);
            //                    }

            //                    break;
            //                }

            //            case "水上":
            //                {
            //                    // 水中専用ユニットが水上のユニットに格闘戦をしかけられるようにする
            //                    uad = GeneralLib.MaxDbl(get_Adaption(2), get_Adaption(3));
            //                    if (uad <= 0)
            //                    {
            //                        // 空中専用ユニットが地上のユニットに格闘戦をしかけられるようにする
            //                        uad = GeneralLib.MaxLng(get_Adaption(1) - 1, 0);
            //                    }

            //                    break;
            //                }

            //            case "水中":
            //                {
            //                    uad = get_Adaption(3);
            //                    break;
            //                }

            //            case "宇宙":
            //                {
            //                    uad = get_Adaption(4);
            //                    if (Area == "地上" & Map.TerrainClass(x, y) == "月面")
            //                    {
            //                        // 月面からのジャンプ攻撃
            //                        string argattr3 = "Ｊ";
            //                        if (IsWeaponClassifiedAs(argattr3))
            //                        {
            //                            string argattr2 = "Ｊ";
            //                            uad = (uad + WeaponLevel(argattr2));
            //                        }
            //                    }

            //                    break;
            //                }

            //            default:
            //                {
            //                    uad = get_Adaption(ind);
            //                    break;
            //                }
            //        }
            //    }

            //    // 地形適応が命中率に適応される場合、ユニットの地形適応は攻撃可否の判定にのみ用いる
            //    string argoname = "地形適応命中率修正";
            //    if (Expression.IsOptionDefined(argoname))
            //    {
            //        if (uad > 0)
            //        {
            //            xad = wad;
            //            goto CalcAdaption;
            //        }
            //        else
            //        {
            //            WeaponAdaptionRet = 0d;
            //            return WeaponAdaptionRet;
            //        }
            //    }

            //    // 武器側とユニット側の地形適応の低い方を優先
            //    if (uad > wad)
            //    {
            //        xad = wad;
            //    }
            //    else
            //    {
            //        xad = uad;
            //    }

            //CalcAdaption:
            //    ;


            //    // Optionコマンドの設定に従って地形適応値を算出
            //    string argoname3 = "地形適応修正緩和";
            //    if (Expression.IsOptionDefined(argoname3))
            //    {
            //        string argoname1 = "地形適応修正繰り下げ";
            //        if (Expression.IsOptionDefined(argoname1))
            //        {
            //            switch (xad)
            //            {
            //                case 5:
            //                    {
            //                        WeaponAdaptionRet = 1.1d;
            //                        break;
            //                    }

            //                case 4:
            //                    {
            //                        WeaponAdaptionRet = 1d;
            //                        break;
            //                    }

            //                case 3:
            //                    {
            //                        WeaponAdaptionRet = 0.9d;
            //                        break;
            //                    }

            //                case 2:
            //                    {
            //                        WeaponAdaptionRet = 0.8d;
            //                        break;
            //                    }

            //                case 1:
            //                    {
            //                        WeaponAdaptionRet = 0.7d;
            //                        break;
            //                    }

            //                default:
            //                    {
            //                        WeaponAdaptionRet = 0d;
            //                        break;
            //                    }
            //            }
            //        }
            //        else
            //        {
            //            switch (xad)
            //            {
            //                case 5:
            //                    {
            //                        WeaponAdaptionRet = 1.2d;
            //                        break;
            //                    }

            //                case 4:
            //                    {
            //                        WeaponAdaptionRet = 1.1d;
            //                        break;
            //                    }

            //                case 3:
            //                    {
            //                        WeaponAdaptionRet = 1d;
            //                        break;
            //                    }

            //                case 2:
            //                    {
            //                        WeaponAdaptionRet = 0.9d;
            //                        break;
            //                    }

            //                case 1:
            //                    {
            //                        WeaponAdaptionRet = 0.8d;
            //                        break;
            //                    }

            //                default:
            //                    {
            //                        WeaponAdaptionRet = 0d;
            //                        break;
            //                    }
            //            }
            //        }
            //    }
            //    else
            //    {
            //        string argoname2 = "地形適応修正繰り下げ";
            //        if (Expression.IsOptionDefined(argoname2))
            //        {
            //            switch (xad)
            //            {
            //                case 5:
            //                    {
            //                        WeaponAdaptionRet = 1.2d;
            //                        break;
            //                    }

            //                case 4:
            //                    {
            //                        WeaponAdaptionRet = 1d;
            //                        break;
            //                    }

            //                case 3:
            //                    {
            //                        WeaponAdaptionRet = 0.8d;
            //                        break;
            //                    }

            //                case 2:
            //                    {
            //                        WeaponAdaptionRet = 0.6d;
            //                        break;
            //                    }

            //                case 1:
            //                    {
            //                        WeaponAdaptionRet = 0.4d;
            //                        break;
            //                    }

            //                default:
            //                    {
            //                        WeaponAdaptionRet = 0d;
            //                        break;
            //                    }
            //            }
            //        }
            //        else
            //        {
            //            switch (xad)
            //            {
            //                case 5:
            //                    {
            //                        WeaponAdaptionRet = 1.4d;
            //                        break;
            //                    }

            //                case 4:
            //                    {
            //                        WeaponAdaptionRet = 1.2d;
            //                        break;
            //                    }

            //                case 3:
            //                    {
            //                        WeaponAdaptionRet = 1d;
            //                        break;
            //                    }

            //                case 2:
            //                    {
            //                        WeaponAdaptionRet = 0.8d;
            //                        break;
            //                    }

            //                case 1:
            //                    {
            //                        WeaponAdaptionRet = 0.6d;
            //                        break;
            //                    }

            //                default:
            //                    {
            //                        WeaponAdaptionRet = 0d;
            //                        break;
            //                    }
            //            }
            //        }
            //    }

            //    return WeaponAdaptionRet;
        }

        public int WeaponMinRange()
        {
            return UpdatedWeaponData.MinRange;
        }
        // 武器 w の最大射程
        public int WeaponMaxRange()
        {
            int WeaponMaxRangeRet = UpdatedWeaponData.MaxRange;
            return WeaponMaxRangeRet;
            // TODO Impl
            //// 最大射程がもともと１ならそれ以上変化しない
            //if (WeaponMaxRangeRet == 1)
            //{
            //    return WeaponMaxRangeRet;
            //}

            //// マップ攻撃には適用されない
            //if (IsWeaponClassifiedAs("Ｍ"))
            //{
            //    return WeaponMaxRangeRet;
            //}

            //// 接近戦武器には適用されない
            //if (IsCrossRange())
            //{
            //    return WeaponMaxRangeRet;
            //}

            //// 有線式誘導攻撃には適用されない
            //if (IsWeaponClassifiedAs("有"))
            //{
            //    return WeaponMaxRangeRet;
            //}

            //// スペシャルパワーによる射程延長
            //if (IsUnderSpecialPowerEffect("射程延長"))
            //{
            //    WeaponMaxRangeRet = (WeaponMaxRangeRet + SpecialPowerEffectLevel("射程延長"));
            //}

            //return WeaponMaxRangeRet;
        }

        // 武器 w の消費ＥＮ
        public int WeaponENConsumption()
        {
            int WeaponENConsumptionRet = UpdatedWeaponData.ENConsumption;
            return WeaponENConsumptionRet;
            // TODO Impl
            //// UPGRADE_NOTE: rate は rate_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
            //double rate_Renamed;
            //int i;
            //{
            //    var withBlock = Weapon(w);
            //    WeaponENConsumptionRet = withBlock.ENConsumption;

            //    // パイロットの能力によって術及び技の消費ＥＮは減少する
            //    if (CountPilot() > 0)
            //    {
            //        // 術に該当するか？
            //        if (IsSpellWeapon(w))
            //        {
            //            // 術に該当する場合は術技能によってＥＮ消費量を変える
            //            object argIndex1 = "術";
            //            string argref_mode = "";
            //            switch (MainPilot().SkillLevel(argIndex1, ref_mode: argref_mode))
            //            {
            //                case 1d:
            //                    {
            //                        break;
            //                    }

            //                case 2d:
            //                    {
            //                        WeaponENConsumptionRet = (0.9d * WeaponENConsumptionRet);
            //                        break;
            //                    }

            //                case 3d:
            //                    {
            //                        WeaponENConsumptionRet = (0.8d * WeaponENConsumptionRet);
            //                        break;
            //                    }

            //                case 4d:
            //                    {
            //                        WeaponENConsumptionRet = (0.7d * WeaponENConsumptionRet);
            //                        break;
            //                    }

            //                case 5d:
            //                    {
            //                        WeaponENConsumptionRet = (0.6d * WeaponENConsumptionRet);
            //                        break;
            //                    }

            //                case 6d:
            //                    {
            //                        WeaponENConsumptionRet = (0.5d * WeaponENConsumptionRet);
            //                        break;
            //                    }

            //                case 7d:
            //                    {
            //                        WeaponENConsumptionRet = (0.45d * WeaponENConsumptionRet);
            //                        break;
            //                    }

            //                case 8d:
            //                    {
            //                        WeaponENConsumptionRet = (0.4d * WeaponENConsumptionRet);
            //                        break;
            //                    }

            //                case 9d:
            //                    {
            //                        WeaponENConsumptionRet = (0.35d * WeaponENConsumptionRet);
            //                        break;
            //                    }

            //                case var @case when @case >= 10d:
            //                    {
            //                        WeaponENConsumptionRet = (0.3d * WeaponENConsumptionRet);
            //                        break;
            //                    }
            //            }

            //            WeaponENConsumptionRet = GeneralLib.MinLng(GeneralLib.MaxLng(WeaponENConsumptionRet, 5), withBlock.ENConsumption);
            //        }

            //        // 技に該当するか？
            //        if (IsFeatWeapon(w))
            //        {
            //            // 技に該当する場合は技技能によってＥＮ消費量を変える
            //            object argIndex2 = "技";
            //            string argref_mode1 = "";
            //            switch (MainPilot().SkillLevel(argIndex2, ref_mode: argref_mode1))
            //            {
            //                case 1d:
            //                    {
            //                        break;
            //                    }

            //                case 2d:
            //                    {
            //                        WeaponENConsumptionRet = (0.9d * WeaponENConsumptionRet);
            //                        break;
            //                    }

            //                case 3d:
            //                    {
            //                        WeaponENConsumptionRet = (0.8d * WeaponENConsumptionRet);
            //                        break;
            //                    }

            //                case 4d:
            //                    {
            //                        WeaponENConsumptionRet = (0.7d * WeaponENConsumptionRet);
            //                        break;
            //                    }

            //                case 5d:
            //                    {
            //                        WeaponENConsumptionRet = (0.6d * WeaponENConsumptionRet);
            //                        break;
            //                    }

            //                case 6d:
            //                    {
            //                        WeaponENConsumptionRet = (0.5d * WeaponENConsumptionRet);
            //                        break;
            //                    }

            //                case 7d:
            //                    {
            //                        WeaponENConsumptionRet = (0.45d * WeaponENConsumptionRet);
            //                        break;
            //                    }

            //                case 8d:
            //                    {
            //                        WeaponENConsumptionRet = (0.4d * WeaponENConsumptionRet);
            //                        break;
            //                    }

            //                case 9d:
            //                    {
            //                        WeaponENConsumptionRet = (0.35d * WeaponENConsumptionRet);
            //                        break;
            //                    }

            //                case var case1 when case1 >= 10d:
            //                    {
            //                        WeaponENConsumptionRet = (0.3d * WeaponENConsumptionRet);
            //                        break;
            //                    }
            //            }

            //            WeaponENConsumptionRet = GeneralLib.MinLng(GeneralLib.MaxLng(WeaponENConsumptionRet, 5), withBlock.ENConsumption);
            //        }
            //    }

            //    // ＥＮ消費減少能力による修正
            //    rate_Renamed = 1d;
            //    string argfname = "ＥＮ消費減少";
            //    if (IsFeatureAvailable(argfname))
            //    {
            //        var loopTo = CountFeature();
            //        for (i = 1; i <= loopTo; i++)
            //        {
            //            object argIndex3 = i;
            //            if (Feature(argIndex3) == "ＥＮ消費減少")
            //            {
            //                double localFeatureLevel() { object argIndex1 = i; var ret = FeatureLevel(argIndex1); return ret; }

            //                rate_Renamed = rate_Renamed - 0.1d * localFeatureLevel();
            //            }
            //        }
            //    }

            //    if (rate_Renamed < 0.1d)
            //    {
            //        rate_Renamed = 0.1d;
            //    }

            //    WeaponENConsumptionRet = (rate_Renamed * WeaponENConsumptionRet);
            //}

            //return WeaponENConsumptionRet;
        }

        // 武器 w の命中率
        public int WeaponPrecision()
        {
            return UpdatedWeaponData.Precision;
        }

        // 武器 w のＣＴ率
        public int WeaponCritical()
        {
            return UpdatedWeaponData.Critical;
        }

        // 武器 w の属性
        public string WeaponClass()
        {
            return UpdatedWeaponData.Class;
        }

        // 武器 w が武器属性 attr を持っているかどうか
        public bool IsWeaponClassifiedAs(string attr)
        {
            bool IsWeaponClassifiedAsRet = default;
            string wclass = WeaponClass();

            // 属性が２文字以下ならそのまま判定
            if (Strings.Len(attr) <= 2)
            {
                if (GeneralLib.InStrNotNest(WeaponClass(), attr) > 0)
                {
                    IsWeaponClassifiedAsRet = true;
                }
                else
                {
                    IsWeaponClassifiedAsRet = false;
                }

                return IsWeaponClassifiedAsRet;
            }

            // 属性の頭文字が弱攻剋ならそのまま判定
            if (Strings.InStr("弱効剋", Strings.Left(attr, 1)) > 0)
            {
                if (GeneralLib.InStrNotNest(WeaponClass(), attr) > 0)
                {
                    IsWeaponClassifiedAsRet = true;
                }
                else
                {
                    IsWeaponClassifiedAsRet = false;
                }

                return IsWeaponClassifiedAsRet;
            }

            // 条件が複雑な場合
            switch (attr ?? "")
            {
                case "格闘系":
                    {
                        if (GeneralLib.InStrNotNest(WeaponClass(), "格") > 0)
                        {
                            IsWeaponClassifiedAsRet = true;
                        }
                        else if (GeneralLib.InStrNotNest(WeaponClass(), "射") > 0)
                        {
                            IsWeaponClassifiedAsRet = false;
                        }
                        else if (WeaponMaxRange() == 1)
                        {
                            IsWeaponClassifiedAsRet = true;
                        }
                        else
                        {
                            IsWeaponClassifiedAsRet = false;
                        }

                        return IsWeaponClassifiedAsRet;
                    }

                case "射撃系":
                    {
                        if (GeneralLib.InStrNotNest(WeaponClass(), "格") > 0)
                        {
                            IsWeaponClassifiedAsRet = false;
                        }
                        else if (GeneralLib.InStrNotNest(WeaponClass(), "射") > 0)
                        {
                            IsWeaponClassifiedAsRet = true;
                        }
                        else if (WeaponMaxRange() == 1)
                        {
                            IsWeaponClassifiedAsRet = false;
                        }
                        else
                        {
                            IsWeaponClassifiedAsRet = true;
                        }

                        break;
                    }

                case "移動後攻撃可":
                    {
                        if (Unit.IsUnderSpecialPowerEffect("全武器移動後使用可能")
                            && GeneralLib.InStrNotNest(WeaponClass(), "Ｍ") == 0
                            && GeneralLib.InStrNotNest(WeaponClass(), "Ｑ") == 0)
                        {
                            IsWeaponClassifiedAsRet = true;
                        }
                        else if (WeaponMaxRange() == 1)
                        {
                            if (GeneralLib.InStrNotNest(WeaponClass(), "Ｑ") == 0)
                            {
                                IsWeaponClassifiedAsRet = true;
                            }
                            else
                            {
                                IsWeaponClassifiedAsRet = false;
                            }
                        }
                        else if (GeneralLib.InStrNotNest(WeaponClass(), "Ｐ") > 0)
                        {
                            IsWeaponClassifiedAsRet = true;
                        }

                        break;
                    }
            }

            return IsWeaponClassifiedAsRet;
        }

        // 武器 w の属性 attr におけるレベル
        public double WeaponLevel(string attr)
        {
            return 0;
            // TODO Impl
            //double WeaponLevelRet = default;
            //string attrlv, wclass;
            //int start_idx, i;
            //string c;
            //;
            ///* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 164587
            //        On Error GoTo ErrorHandler
            // */
            //attrlv = attr + "L";

            //// 武器属性を調べてみる
            //wclass = strWeaponClass[w];

            //// レベル指定があるか？
            //start_idx = GeneralLib.InStrNotNest(WeaponClass(), attrlv);
            //if (start_idx == 0)
            //{
            //    return WeaponLevelRet;
            //}

            //// レベル指定部分の切り出し
            //start_idx = (start_idx + Strings.Len(attrlv));
            //i = start_idx;
            //while (true)
            //{
            //    c = Strings.Mid(WeaponClass(), i, 1);
            //    if (string.IsNullOrEmpty(c))
            //    {
            //        break;
            //    }

            //    switch (Strings.Asc(c))
            //    {
            //        case var @case when 45 <= @case && @case <= 46:
            //        case var case1 when 48 <= case1 && case1 <= 57: // "-", ".", 0-9
            //            {
            //                break;
            //            }

            //        default:
            //            {
            //                break;
            //            }
            //    }

            //    i = (i + 1);
            //}

            //WeaponLevelRet = Conversions.ToDouble(Strings.Mid(WeaponClass(), start_idx, i - start_idx));
            //return WeaponLevelRet;
            //ErrorHandler:
            //;
            //string argmsg = Name + "の" + "武装「" + Weapon(w).Name + "」の" + "属性「" + attr + "」のレベル指定が不正です";
            //GUI.ErrorMessage(argmsg);
        }

        // 武器 w の属性 attr にレベル指定がなされているか
        public bool IsWeaponLevelSpecified(string attr)
        {
            return Strings.InStr(WeaponClass(), attr + "L") > 0;
        }

        // 武器 w が通常武器かどうか
        public bool IsNormalWeapon()
        {
            // 特殊効果属性を持つ？
            foreach (var str in WeaponClass())
            {
                int ret = Strings.InStr("Ｓ縛劣中石凍痺眠乱魅恐踊狂ゾ害憑盲毒撹不止黙除即告脱Ｄ低吹Ｋ引転衰滅盗習写化弱効剋", str.ToString());
                if (ret > 0)
                {
                    return false;
                }
            }
            return true;
        }

        // 武器が持つ特殊効果の数を返す
        public int CountWeaponEffect()
        {
            return 0;
            // TODO Impl
            //int CountWeaponEffectRet = default;
            //string wclass, wattr;
            //int i, ret;
            //wclass = strWeaponClass[w];
            //var loopTo = Strings.Len(wclass);
            //for (i = 1; i <= loopTo; i++)
            //{
            //    // 弱Ｓのような入れ子があれば、入れ子の分カウントを進める
            //    wattr = GeneralLib.GetClassBundle(WeaponClass(), i, 1);

            //    // 非表示部分は無視
            //    if (wattr == "|")
            //    {
            //        break;
            //    }

            //    // ＣＴ時発動系
            //    ret = Strings.InStr("Ｓ縛劣中石凍痺眠乱魅恐踊狂ゾ害憑盲毒撹不止黙除即告脱Ｄ低吹Ｋ引転衰滅盗習写化弱効剋", wattr);
            //    if (ret > 0)
            //    {
            //        CountWeaponEffectRet = (CountWeaponEffectRet + 1);
            //    }

            //    // それ以外
            //    ret = Strings.InStr("先再忍貫固殺無浸破間浄吸減奪", wattr);
            //    if (ret > 0)
            //    {
            //        CountWeaponEffectRet = (CountWeaponEffectRet + 1);
            //    }
            //}

            //return CountWeaponEffectRet;
        }

        // 武器 w が術かどうか
        public bool IsSpellWeapon()
        {
            return false;
            // TODO Impl
            //bool IsSpellWeaponRet = default;
            //int i;
            //string nskill;
            //if (IsWeaponClassifiedAs("術"))
            //{
            //    IsSpellWeaponRet = true;
            //    return IsSpellWeaponRet;
            //}

            //{
            //    var withBlock = MainPilot();
            //    var loopTo = GeneralLib.LLength(Weapon(w).NecessarySkill);
            //    for (i = 1; i <= loopTo; i++)
            //    {
            //        nskill = GeneralLib.LIndex(Weapon(w).NecessarySkill, i);
            //        if (Strings.InStr(nskill, "Lv") > 0)
            //        {
            //            nskill = Strings.Left(nskill, Strings.InStr(nskill, "Lv") - 1);
            //        }

            //        if (withBlock.SkillType(nskill) == "術")
            //        {
            //            IsSpellWeaponRet = true;
            //            return IsSpellWeaponRet;
            //        }
            //    }
            //}

            //return IsSpellWeaponRet;
        }

        // 武器 w が技かどうか
        public bool IsFeatWeapon()
        {
            return false;
            // TODO Impl
            //bool IsFeatWeaponRet = default;
            //int i;
            //string nskill;
            //string argattr = "技";
            //if (IsWeaponClassifiedAs(argattr))
            //{
            //    IsFeatWeaponRet = true;
            //    return IsFeatWeaponRet;
            //}

            //{
            //    var withBlock = MainPilot();
            //    var loopTo = GeneralLib.LLength(Weapon(w).NecessarySkill);
            //    for (i = 1; i <= loopTo; i++)
            //    {
            //        nskill = GeneralLib.LIndex(Weapon(w).NecessarySkill, i);
            //        if (Strings.InStr(nskill, "Lv") > 0)
            //        {
            //            nskill = Strings.Left(nskill, Strings.InStr(nskill, "Lv") - 1);
            //        }

            //        if (withBlock.SkillType(nskill) == "技")
            //        {
            //            IsFeatWeaponRet = true;
            //            return IsFeatWeaponRet;
            //        }
            //    }
            //}

            //return IsFeatWeaponRet;
        }

        // 武器 w が使用可能かどうか
        // ref_mode はユニットの状態（移動前、移動後）を示す
        public bool IsWeaponAvailable(string ref_mode)
        {
            // イベントコマンド「Disable」で封印されている？
            if (Unit.IsDisabled(Name))
            {
                return false;
            }

            // パイロットが乗っていなければ常に使用可能と判定
            if (Unit.CountPilot() == 0)
            {
                return true;
            }

            // 必要技能＆必要条件
            if (ref_mode != "必要技能無視")
            {
                if (!IsWeaponMastered())
                {
                    return false;
                }

                if (!IsWeaponEnabled())
                {
                    return false;
                }
            }

            // ステータス表示では必要技能だけ満たしていればＯＫ
            if (ref_mode == "インターミッション" || string.IsNullOrEmpty(ref_mode))
            {
                return true;
            }

            {
                var p = Unit.MainPilot();
                // 必要気力
                if (UpdatedWeaponData.NecessaryMorale > 0)
                {
                    if (p.Morale < UpdatedWeaponData.NecessaryMorale)
                    {
                        return false;
                    }
                }

                // 霊力消費攻撃
                if (GeneralLib.InStrNotNest(WeaponClass(), "霊") > 0)
                {
                    if (p.Plana < WeaponLevel("霊") * 5d)
                    {
                        return false;
                    }
                }
                else if (GeneralLib.InStrNotNest(WeaponClass(), "プ") > 0)
                {
                    if (p.Plana < WeaponLevel("プ") * 5d)
                    {
                        return false;
                    }
                }
            }

            // 属性使用不能状態
            if (Unit.ConditionLifetime("オーラ使用不能") > 0)
            {
                if (IsWeaponClassifiedAs("オ"))
                {
                    return false;
                }
            }

            if (Unit.ConditionLifetime("超能力使用不能") > 0)
            {
                if (IsWeaponClassifiedAs("超"))
                {
                    return false;
                }
            }

            if (Unit.ConditionLifetime("同調率使用不能") > 0)
            {
                if (IsWeaponClassifiedAs("シ"))
                {
                    return false;
                }
            }

            if (Unit.ConditionLifetime("超感覚使用不能") > 0)
            {
                if (IsWeaponClassifiedAs("サ"))
                {
                    return false;
                }
            }

            if (Unit.ConditionLifetime("知覚強化使用不能") > 0)
            {
                if (IsWeaponClassifiedAs("サ"))
                {
                    return false;
                }
            }

            if (Unit.ConditionLifetime("霊力使用不能") > 0)
            {
                if (IsWeaponClassifiedAs("霊"))
                {
                    return false;
                }
            }

            if (Unit.ConditionLifetime("術使用不能") > 0)
            {
                if (IsWeaponClassifiedAs("術"))
                {
                    return false;
                }
            }

            if (Unit.ConditionLifetime("技使用不能") > 0)
            {
                if (IsWeaponClassifiedAs("技"))
                {
                    return false;
                }
            }

            foreach (var condition in Unit.Conditions.Where(x => x.Name.EndsWith("属性使用不能")))
            {
                if (GeneralLib.InStrNotNest(WeaponClass(), condition.Name.Replace("属性使用不能", "")) > 0)
                {
                    return false;
                }
            }

            // 弾数が足りるか
            if (UpdatedWeaponData.Bullet > 0)
            {
                if (Bullet() < 1)
                {
                    return false;
                }
            }

            // ＥＮが足りるか
            if (UpdatedWeaponData.ENConsumption > 0)
            {
                if (Unit.EN < WeaponENConsumption())
                {
                    return false;
                }
            }

            // お金が足りるか……
            if (Unit.Party == "味方")
            {
                if (GeneralLib.InStrNotNest(WeaponClass(), "銭") > 0)
                {
                    if (SRC.Money < GeneralLib.MaxLng((int)WeaponLevel("銭"), 1) * Unit.Value / 10)
                    {
                        return false;
                    }
                }
            }

            // 攻撃不能？
            if (ref_mode != "ステータス")
            {
                if (Unit.IsConditionSatisfied("攻撃不能"))
                {
                    return false;
                }
            }

            if (Unit.Area == "地中")
            {
                return false;
            }

            // 移動不能時には移動型マップ攻撃は使用不能
            if (Unit.IsConditionSatisfied("移動不能"))
            {
                if (GeneralLib.InStrNotNest(WeaponClass(), "Ｍ移") > 0)
                {
                    return false;
                }
            }

            // 術および音は沈黙状態では使用不能
            if (Unit.IsConditionSatisfied("沈黙"))
            {
                if (IsSpellWeapon() || GeneralLib.InStrNotNest(WeaponClass(), "音") > 0)
                {
                    return false;
                }
            }

            // 合体技の処理
            if (GeneralLib.InStrNotNest(WeaponClass(), "合") > 0)
            {
                if (!IsCombinationAttackAvailable())
                {
                    return false;
                }
            }

            // 変形技の場合は今いる地形で変形できる必要あり
            if (GeneralLib.InStrNotNest(WeaponClass(), "変") > 0)
            {
                if (Unit.IsConditionSatisfied("形態固定"))
                {
                    return false;
                }

                if (Unit.IsConditionSatisfied("機体固定"))
                {
                    return false;
                }

                // XXX 変形技が妥当に定義されていなかった場合
                if (Unit.IsFeatureAvailable("変形技"))
                {
                    foreach (var feature in Unit.Features
                        .Where(x => x.Name == "変形技")
                        .Where(x => x.DataL.FirstOrDefault() == UpdatedWeaponData.Name))
                    {
                        if (!Unit.OtherForm(feature.DataL.Skip(1).FirstOrDefault()).IsAbleToEnter(Unit.x, Unit.y))
                        {
                            return false;
                        }
                    }
                }
                else if (Unit.IsFeatureAvailable("ノーマルモード"))
                {
                    if (!Unit.OtherForm(GeneralLib.LIndex(Unit.FeatureData("ノーマルモード"), 1)).IsAbleToEnter(Unit.x, Unit.y))
                    {
                        return false;
                    }
                }
            }

            // 瀕死時限定
            if (GeneralLib.InStrNotNest(WeaponClass(), "瀕") > 0)
            {
                if (Unit.HP > Unit.MaxHP / 4)
                {
                    return false;
                }
            }

            // 自動チャージ攻撃を再充填中
            if (Unit.IsConditionSatisfied(WeaponNickname() + "充填中"))
            {
                return false;
            }
            // 共有武器＆アビリティが充填中の場合も使用不可
            if (GeneralLib.InStrNotNest(WeaponClass(), "共") > 0)
            {
                int lv = (int)WeaponLevel("共");
                if (Unit.Weapons
                    .Where(x => x.IsWeaponClassifiedAs("共"))
                    .Where(x => x.WeaponLevel("共") == lv)
                    .Where(x => Unit.IsConditionSatisfied(x.WeaponNickname() + "充填中"))
                    .Any())
                {
                    return false;
                }

                // TODO Impl アビリティ
                //if (Unit.AbilityDatas
                //    .Where(x => x.IsAbilityClassifiedAs("共"))
                //    .Where(x => x.AbilityLevel("共") == lv)
                //    .Where(x => Unit.IsConditionSatisfied(x.AbilityNickname() + "充填中"))
                //    .Any())
                //{
                //    return false;
                //}
            }

            // 能力コピー
            if (GeneralLib.InStrNotNest(WeaponClass(), "写") > 0
                || GeneralLib.InStrNotNest(WeaponClass(), "化") > 0)
            {
                if (Unit.IsFeatureAvailable("ノーマルモード"))
                {
                    // 既に変身済みの場合はコピー出来ない
                    return false;
                }
            }

            // 使用禁止
            if (GeneralLib.InStrNotNest(WeaponClass(), "禁") > 0)
            {
                return false;
            }

            // チャージ判定であればここまででＯＫ
            if (ref_mode == "チャージ")
            {
                return true;
            }

            // チャージ式攻撃
            if (GeneralLib.InStrNotNest(WeaponClass(), "Ｃ") > 0)
            {
                if (!Unit.IsConditionSatisfied("チャージ完了"))
                {
                    return false;
                }
            }

            if (ref_mode == "ステータス")
            {
                return true;
            }

            // 反撃かどうかの判定
            // 自軍のフェイズでなければ反撃時である
            if ((Unit.Party ?? "") != (SRC.Stage ?? ""))
            {
                // 反撃ではマップ攻撃、合体技は使用できない
                if (GeneralLib.InStrNotNest(WeaponClass(), "Ｍ") > 0
                    || GeneralLib.InStrNotNest(WeaponClass(), "合") > 0)
                {
                    return false;
                }

                // 攻撃専用武器
                if (GeneralLib.InStrNotNest(WeaponClass(), "攻") > 0)
                {
                    var loopTo4 = Strings.Len(WeaponClass());
                    for (var i = 1; i <= loopTo4; i++)
                    {
                        if (Strings.Mid(WeaponClass(), i, 1) == "攻")
                        {
                            if (i == 1)
                            {
                                return false;
                            }

                            if (Strings.Mid(WeaponClass(), i - 1, 1) != "低")
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            else
            {
                // 反撃専用攻撃
                if (GeneralLib.InStrNotNest(WeaponClass(), "反") > 0)
                {
                    return false;
                }
            }

            // 移動前か後か……
            if (ref_mode == "移動前"
                || ref_mode == "必要技能無視"
                || !ReferenceEquals(Commands.SelectedUnit, Unit))
            {
                return true;
            }

            // 移動後の場合
            if (Unit.IsUnderSpecialPowerEffect("全武器移動後使用可能")
                && !(GeneralLib.InStrNotNest(WeaponClass(), "Ｍ") > 0))
            {
                return true;
            }
            else if (WeaponMaxRange() > 1)
            {
                if (GeneralLib.InStrNotNest(WeaponClass(), "Ｐ") > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (GeneralLib.InStrNotNest(WeaponClass(), "Ｑ") > 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        // 武器 w の使用技能を満たしているか。
        public bool IsWeaponMastered()
        {
            return true;
            // TODO Impl
            //bool IsWeaponMasteredRet = default;
            //Pilot argp = null;
            //IsWeaponMasteredRet = IsNecessarySkillSatisfied(Weapon(w).NecessarySkill, p: argp);
            //return IsWeaponMasteredRet;
        }

        // 武器 w の使用条件を満たしているか。
        public bool IsWeaponEnabled()
        {
            return true;
            // TODO Impl
            //bool IsWeaponEnabledRet = default;
            //Pilot argp = null;
            //IsWeaponEnabledRet = IsNecessarySkillSatisfied(Weapon(w).NecessaryCondition, p: argp);
            //return IsWeaponEnabledRet;
        }

        // 武器が使用可能であり、かつ射程内に敵がいるかどうか
        public bool IsWeaponUseful(string ref_mode)
        {
            bool IsWeaponUsefulRet = default;
            int i, j;
            Unit u;
            int max_range;

            // 武器が使用可能か？
            if (!IsWeaponAvailable(ref_mode))
            {
                IsWeaponUsefulRet = false;
                return IsWeaponUsefulRet;
            }

            // 扇型マップ攻撃は特殊なので判定ができない
            // 移動型マップ攻撃は移動手段として使うことを考慮
            if (IsWeaponClassifiedAs("Ｍ扇") || IsWeaponClassifiedAs("Ｍ移"))
            {
                IsWeaponUsefulRet = true;
                return IsWeaponUsefulRet;
            }

            max_range = WeaponMaxRange();

            // 投下型マップ攻撃は効果範囲が広い
            max_range = (int)(max_range + WeaponLevel("Ｍ投"));

            // 敵の存在判定
            var loopTo = GeneralLib.MinLng(Unit.x + max_range, Map.MapWidth);
            for (i = GeneralLib.MaxLng(Unit.x - max_range, 1); i <= loopTo; i++)
            {
                var loopTo1 = GeneralLib.MinLng(Unit.y + max_range, Map.MapHeight);
                for (j = GeneralLib.MaxLng(Unit.y - max_range, 1); j <= loopTo1; j++)
                {
                    u = Map.MapDataForUnit[i, j];
                    if (u is null)
                    {
                        goto NextUnit;
                    }

                    // TODO Impl
                    //switch (Unit.Party ?? "")
                    //{
                    //    case "味方":
                    //    case "ＮＰＣ":
                    //        {
                    //            switch (u.Party ?? "")
                    //            {
                    //                case "味方":
                    //                case "ＮＰＣ":
                    //                    {
                    //                        // ステータス異常の場合は味方ユニットでも排除可能
                    //                        if (!u.IsConditionSatisfied("暴走") 
                    //                            && !u.IsConditionSatisfied("混乱") 
                    //                            && !u.IsConditionSatisfied("魅了") 
                    //                            && !u.IsConditionSatisfied("憑依") 
                    //                            && !u.IsConditionSatisfied("睡眠"))
                    //                        {
                    //                            goto NextUnit;
                    //                        }

                    //                        break;
                    //                    }
                    //            }

                    //            break;
                    //        }

                    //    default:
                    //        {
                    //            if ((Unit.Party ?? "") == (u.Party ?? ""))
                    //            {
                    //                // ステータス異常の場合は味方ユニットでも排除可能
                    //                if (!u.IsConditionSatisfied("暴走") 
                    //                    && !u.IsConditionSatisfied("混乱") 
                    //                    && !u.IsConditionSatisfied("魅了") 
                    //                    && !u.IsConditionSatisfied("憑依"))
                    //                {
                    //                    goto NextUnit;
                    //                }
                    //            }

                    //            break;
                    //        }
                    //}

                    if (IsTargetWithinRange(u))
                    {
                        if (UpdatedWeaponData.Power > 0)
                        {
                            if (Damage(u, true) != 0)
                            {
                                IsWeaponUsefulRet = true;
                                return IsWeaponUsefulRet;
                            }
                        }
                        else if (CriticalProbability(u) > 0)
                        {
                            IsWeaponUsefulRet = true;
                            return IsWeaponUsefulRet;
                        }
                    }

                NextUnit:
                    ;
                }
            }

            // 敵は見つからなかった
            IsWeaponUsefulRet = false;
            return IsWeaponUsefulRet;
        }

        // ユニット t が武器 w の射程範囲内にいるかをチェック
        public bool IsTargetWithinRange(Unit t)
        {
            int max_range, distance, range_mod;
            // 距離を算出
            distance = (Math.Abs((Unit.x - t.x)) + Math.Abs((Unit.y - t.y)));

            // Ｍ投武器は目標地点からさらに効果範囲が伸びるので射程修正を行う
            range_mod = (int)WeaponLevel("Ｍ投");

            // 最大射程チェック
            max_range = WeaponMaxRange();
            if (distance > (max_range + range_mod))
            {
                return false;
            }

            // 最小射程チェック
            if (distance < (UpdatedWeaponData.MinRange - range_mod))
            {
                return false;
            }

            // TODO Impl
            //// 敵がステルスの場合
            //if (t.IsFeatureAvailable("ステルス"))
            //{
            //    if (t.IsFeatureLevelSpecified("ステルス"))
            //    {
            //        lv = t.FeatureLevel("ステルス");
            //    }
            //    else
            //    {
            //        lv = 3;
            //    }

            //    if (!t.IsConditionSatisfied("ステルス無効") 
            //        && !IsFeatureAvailable("ステルス無効化") 
            //        && distance > lv)
            //    {
            //        IsTargetWithinRangeRet = false;
            //        return IsTargetWithinRangeRet;
            //    }
            //}

            //// 隠れ身中？
            //if (t.IsUnderSpecialPowerEffect("隠れ身"))
            //{
            //    if (!t.IsUnderSpecialPowerEffect("無防備"))
            //    {
            //        IsTargetWithinRangeRet = false;
            //        return IsTargetWithinRangeRet;
            //    }
            //}

            //// 攻撃できない地形にいる場合は射程外とみなす
            //if (WeaponAdaption(t.Area) == 0d)
            //{
            //    IsTargetWithinRangeRet = false;
            //    return IsTargetWithinRangeRet;
            //}

            //// 合体技で射程が１の場合は相手を囲んでいる必要がある
            //if (IsWeaponClassifiedAs("合") && !IsWeaponClassifiedAs("Ｍ") & max_range == 1)
            //{
            //    CombinationPartner("武装", w, partners, t.x, t.y);
            //    if (Information.UBound(partners) == 0)
            //    {
            //        IsTargetWithinRangeRet = false;
            //        return IsTargetWithinRangeRet;
            //    }
            //}

            return true;
        }

        // 移動を併用した場合にユニット t が武器 w の射程範囲内にいるかをチェック
        public bool IsTargetReachable(Unit t)
        {
            bool IsTargetReachableRet = default;
            int i, j;
            int max_range, min_range;
            var partners = default(Unit[]);
            // 地形適応をチェック
            if (WeaponAdaption(t.Area) == 0d)
            {
                return false;
            }

            // 隠れ身使用中？
            if (t.IsUnderSpecialPowerEffect("隠れ身"))
            {
                if (!t.IsUnderSpecialPowerEffect("無防備"))
                {
                    return false;
                }
            }

            // 射程計算
            min_range = UpdatedWeaponData.MinRange;
            max_range = WeaponMaxRange();
            // TODO Impl
            //// 敵がステルスの場合
            //if (t.IsFeatureAvailable("ステルス") 
            //    && !t.IsConditionSatisfied("ステルス無効") 
            //    && !IsFeatureAvailable("ステルス無効化"))
            //{
            //    if (t.IsFeatureLevelSpecified("ステルス"))
            //    {
            //        max_range = GeneralLib.MinLng(max_range, (t.FeatureLevel("ステルス") + 1d));
            //    }
            //    else
            //    {
            //        max_range = GeneralLib.MinLng(max_range, 4);
            //    }
            //}

            //// 隣接していれば必ず届く
            //if (min_range == 1 & Math.Abs((x - t.x)) + Math.Abs((y - t.y)) == 1)
            //{
            //    // ただし合体技の場合は例外……
            //    // 合体技で射程が１の場合は相手を囲んでいる必要がある
            //    string argattr = "合";
            //    string argattr1 = "Ｍ";
            //    if (IsWeaponClassifiedAs(argattr) & !IsWeaponClassifiedAs(argattr1) & WeaponMaxRange(w) == 1)
            //    {
            //        string argctype_Renamed = "武装";
            //        CombinationPartner(argctype_Renamed, w, partners, t.x, t.y);
            //        if (Information.UBound(partners) == 0)
            //        {
            //            IsTargetReachableRet = false;
            //            return false;
            //        }
            //    }

            //    return true;
            //}

            // 移動範囲から敵に攻撃が届くかをチェック
            var loopTo = GeneralLib.MinLng(t.x + max_range, Map.MapWidth);
            for (i = GeneralLib.MaxLng(t.x - max_range, 1); i <= loopTo; i++)
            {
                var loopTo1 = GeneralLib.MinLng(t.y + (max_range - Math.Abs((t.x - i))), Map.MapHeight);
                for (j = GeneralLib.MaxLng(t.y - (max_range - Math.Abs((t.x - i))), 1); j <= loopTo1; j++)
                {
                    if (min_range <= (Math.Abs((t.x - i)) + Math.Abs((t.y - i))))
                    {
                        if (!Map.MaskData[i, j])
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        // 武器 w のユニット t に対する命中率
        // 敵ユニットはスペシャルパワー等による補正を考慮しないので
        // is_true_value によって補正を省くかどうかを指定できるようにしている
        public int HitProbability(Unit t, bool is_true_value)
        {
            //return 75;
            // TODO impl
            //int HitProbabilityRet = default;
            int prob;
            //int mpskill;
            //int i, j;
            //Unit u;
            //var wclass = default(string);
            //double ecm_lv = default, eccm_lv = default;
            //string buf;
            //string fdata;
            //double flevel, prob_mod;
            //int nmorale;
            // 命中、回避、地形補正、サイズ補正の数値を定義
            int ed_hit, ed_avd;
            double ed_aradap, ed_size;

            // 初期値
            ed_aradap = 1d;

            // スペシャルパワーによる捨て身状態
            if (t.IsUnderSpecialPowerEffect("無防備"))
            {
                return 100;
            }

            // パイロットの技量によって命中率を正確に予測できるか左右される
            var mpskill = Unit.MainPilot().TacticalTechnique();

            // スペシャルパワーによる影響
            if (is_true_value || mpskill >= 160)
            {
                if (t.IsUnderSpecialPowerEffect("絶対回避"))
                {
                    return 0;
                }

                if (Unit.IsUnderSpecialPowerEffect("絶対命中"))
                {
                    return 1000;
                }
            }

            // 自ユニットによる修正
            var atackerPilot = Unit.MainPilot();
            //{
            //    var withBlock = MainPilot();
            //    object argIndex2 = "命中補正";
            //    if (SRC.BCList.IsDefined(argIndex2))
            //    {
            //        // 命中を一時保存
            //        // 事前にデータを登録
            //        BCVariable.DataReset();
            //        BCVariable.MeUnit = this;
            //        BCVariable.AtkUnit = this;
            //        BCVariable.DefUnit = t;
            //        BCVariable.WeaponNumber = w;
            //        BCVariable.AttackExp = WeaponPrecision(w);
            //        string argIndex1 = "命中補正";
            //        ed_hit = SRC.BCList.Item(argIndex1).Calculate();
            //    }
            //    else
            //    {
            // 命中を一時保存
            ed_hit = 100 + atackerPilot.Hit + atackerPilot.Intuition + Unit.get_Mobility("") + WeaponPrecision();
            //    }
            //}

            // 敵ユニットによる修正
            var targetPilot = t.MainPilot();
            //{
            //    var withBlock1 = t.MainPilot();
            //    object argIndex4 = "回避補正";
            //    if (SRC.BCList.IsDefined(argIndex4))
            //    {
            //        // 回避を一時保存
            //        // 事前にデータを登録
            //        BCVariable.DataReset();
            //        BCVariable.MeUnit = t;
            //        BCVariable.AtkUnit = this;
            //        BCVariable.DefUnit = t;
            //        BCVariable.WeaponNumber = w;
            //        string argIndex3 = "回避補正";
            //        ed_avd = SRC.BCList.Item(argIndex3).Calculate();
            //    }
            //    else
            //    {
            // 回避を一時保存
            ed_avd = (targetPilot.Dodge + targetPilot.Intuition) + t.get_Mobility("");
            //    }
            //}

            //// 地形適応、サイズ修正の位置を変更
            //var uadaption = default(double);
            //string tarea;
            //int tx, ty;
            //{
            //    var withBlock2 = t;
            //    // 地形修正
            //    if (withBlock2.Area != "空中" & (withBlock2.Area != "宇宙" | Map.TerrainClass(withBlock2.x, withBlock2.y) != "月面"))
            //    {
            //        // 地形修正を一時保存
            //        ed_aradap = ed_aradap * (100 - Map.TerrainEffectForHit(withBlock2.x, withBlock2.y)) / 100d;
            //    }

            //    // 地形適応修正
            //    string argoname = "地形適応命中率修正";
            //    if (Expression.IsOptionDefined(argoname))
            //    {

            //        // 接近戦攻撃の場合はターゲット側の地形を参照
            //        string argattr = "武";
            //        string argattr1 = "突";
            //        string argattr2 = "接";
            //        if (IsWeaponClassifiedAs(argattr) | IsWeaponClassifiedAs(argattr1) | IsWeaponClassifiedAs(argattr2))
            //        {
            //            tarea = withBlock2.Area;
            //            tx = withBlock2.x;
            //            ty = withBlock2.y;
            //        }
            //        else
            //        {
            //            tarea = Area;
            //            tx = x;
            //            ty = y;
            //        }

            //        switch (tarea ?? "")
            //        {
            //            case "空中":
            //                {
            //                    uadaption = get_AdaptionMod(1, 0);
            //                    // ジャンプ攻撃の場合はＪ属性による修正を加える
            //                    string argarea_name = "空";
            //                    if ((withBlock2.Area == "空中" | withBlock2.Area == "宇宙") & Area != "空中" & Area != "宇宙" & !IsTransAvailable(argarea_name))
            //                    {
            //                        string argstring2 = "武";
            //                        string argstring21 = "突";
            //                        string argstring22 = "接";
            //                        if (Conversions.ToBoolean(GeneralLib.InStrNotNest(WeaponClass(), argstring2) | GeneralLib.InStrNotNest(WeaponClass(), argstring21) | GeneralLib.InStrNotNest(WeaponClass(), argstring22)))
            //                        {
            //                            string argattr3 = "Ｊ";
            //                            uadaption = get_AdaptionMod(1, WeaponLevel(argattr3));
            //                        }
            //                    }

            //                    break;
            //                }

            //            case "地上":
            //                {
            //                    if (Map.TerrainClass(tx, ty) == "月面")
            //                    {
            //                        uadaption = get_AdaptionMod(4, 0);
            //                    }
            //                    else
            //                    {
            //                        uadaption = get_AdaptionMod(2, 0);
            //                    }

            //                    break;
            //                }

            //            case "水上":
            //                {
            //                    uadaption = get_AdaptionMod(2, 0);
            //                    break;
            //                }

            //            case "水中":
            //                {
            //                    uadaption = get_AdaptionMod(3, 0);
            //                    break;
            //                }

            //            case "宇宙":
            //                {
            //                    uadaption = get_AdaptionMod(4, 0);
            //                    break;
            //                }

            //            case "地中":
            //                {
            //                    HitProbabilityRet = 0;
            //                    return HitProbabilityRet;
            //                }
            //        }

            //        // 地形修正を一時保存
            //        ed_aradap = ed_aradap * uadaption;
            //    }

            // サイズ補正
            switch (t.Size ?? "")
            {
                case "M":
                    {
                        ed_size = 1d;
                        break;
                    }

                case "L":
                    {
                        ed_size = 1.2d;
                        break;
                    }

                case "S":
                    {
                        ed_size = 0.8d;
                        break;
                    }

                case "LL":
                    {
                        ed_size = 1.4d;
                        break;
                    }

                case "SS":
                    {
                        ed_size = 0.5d;
                        break;
                    }

                case "XL":
                    {
                        ed_size = 2d;
                        break;
                    }
                default:
                    // XXX 元は処理してない
                    ed_size = 1d;
                    break;
            }
            //}

            //// 命中率計算実行
            //object argIndex6 = "命中率";
            //if (SRC.BCList.IsDefined(argIndex6))
            //{
            //    // 事前にデータを登録
            //    BCVariable.DataReset();
            //    BCVariable.MeUnit = this;
            //    BCVariable.AtkUnit = this;
            //    BCVariable.DefUnit = t;
            //    BCVariable.WeaponNumber = w;
            //    BCVariable.AttackVariable = ed_hit;
            //    BCVariable.DffenceVariable = ed_avd;
            //    BCVariable.TerrainAdaption = ed_aradap;
            //    BCVariable.SizeMod = ed_size;
            //    string argIndex5 = "命中率";
            //    prob = SRC.BCList.Item(argIndex5).Calculate();
            //}
            //else
            {
                prob = (int)((ed_hit - ed_avd) * ed_aradap * ed_size);
            }

            //// 不意打ち
            //string argfname = "ステルス";
            //object argIndex7 = "ステルス無効";
            //string argfname1 = "ステルス無効化";
            //if (IsFeatureAvailable(argfname) & !IsConditionSatisfied(argIndex7) & !t.IsFeatureAvailable(argfname1))
            //{
            //    prob = prob + 20;
            //}

            //wclass = WeaponClass(w);
            //int uad;
            //{
            //    var withBlock3 = t;
            //    // 散属性武器は指定したレベル以上離れるほど命中がアップ
            //    string argstring23 = "散";
            //    if (GeneralLib.InStrNotNest(WeaponClass(), argstring23) > 0)
            //    {
            //        switch ((Math.Abs((x - withBlock3.x)) + Math.Abs((y - withBlock3.y))))
            //        {
            //            case 1:
            //                {
            //                    break;
            //                }
            //            // 修正なし
            //            case 2:
            //                {
            //                    prob = prob + 5;
            //                    break;
            //                }

            //            case 3:
            //                {
            //                    prob = prob + 10;
            //                    break;
            //                }

            //            case 4:
            //                {
            //                    prob = prob + 15;
            //                    break;
            //                }

            //            default:
            //                {
            //                    prob = prob + 20;
            //                    break;
            //                }
            //        }
            //    }

            //    string argstring27 = "サ";
            //    string argstring28 = "有";
            //    string argstring29 = "誘";
            //    string argstring210 = "追";
            //    string argstring211 = "武";
            //    string argstring212 = "突";
            //    string argstring213 = "接";
            //    if (GeneralLib.InStrNotNest(WeaponClass(), argstring27) == 0 & GeneralLib.InStrNotNest(WeaponClass(), argstring28) == 0 & GeneralLib.InStrNotNest(WeaponClass(), argstring29) == 0 & GeneralLib.InStrNotNest(WeaponClass(), argstring210) == 0 & GeneralLib.InStrNotNest(WeaponClass(), argstring211) == 0 & GeneralLib.InStrNotNest(WeaponClass(), argstring212) == 0 & GeneralLib.InStrNotNest(WeaponClass(), argstring213) == 0)
            //    {
            //        // 距離修正
            //        string argoname3 = "距離修正";
            //        if (Expression.IsOptionDefined(argoname3))
            //        {
            //            string argstring24 = "Ｈ";
            //            string argstring25 = "Ｍ";
            //            if (GeneralLib.InStrNotNest(WeaponClass(), argstring24) == 0 & GeneralLib.InStrNotNest(WeaponClass(), argstring25) == 0)
            //            {
            //                string argoname1 = "大型マップ";
            //                string argoname2 = "小型マップ";
            //                if (Expression.IsOptionDefined(argoname1))
            //                {
            //                    switch ((Math.Abs((x - withBlock3.x)) + Math.Abs((y - withBlock3.y))))
            //                    {
            //                        case var @case when 1 <= @case && @case <= 4:
            //                            {
            //                                break;
            //                            }
            //                        // 修正なし
            //                        case 5:
            //                        case 6:
            //                            {
            //                                prob = (0.9d * prob);
            //                                break;
            //                            }

            //                        case 7:
            //                        case 8:
            //                            {
            //                                prob = (0.8d * prob);
            //                                break;
            //                            }

            //                        case 9:
            //                        case 10:
            //                            {
            //                                prob = (0.7d * prob);
            //                                break;
            //                            }

            //                        default:
            //                            {
            //                                prob = (0.6d * prob);
            //                                break;
            //                            }
            //                    }
            //                }
            //                else if (Expression.IsOptionDefined(argoname2))
            //                {
            //                    switch ((Math.Abs((x - withBlock3.x)) + Math.Abs((y - withBlock3.y))))
            //                    {
            //                        case 1:
            //                            {
            //                                break;
            //                            }
            //                        // 修正なし
            //                        case 2:
            //                            {
            //                                prob = (0.9d * prob);
            //                                break;
            //                            }

            //                        case 3:
            //                            {
            //                                prob = (0.8d * prob);
            //                                break;
            //                            }

            //                        case 4:
            //                            {
            //                                prob = (0.75d * prob);
            //                                break;
            //                            }

            //                        case 5:
            //                            {
            //                                prob = (0.7d * prob);
            //                                break;
            //                            }

            //                        case 6:
            //                            {
            //                                prob = (0.65d * prob);
            //                                break;
            //                            }

            //                        default:
            //                            {
            //                                prob = (0.6d * prob);
            //                                break;
            //                            }
            //                    }
            //                }
            //                else
            //                {
            //                    switch ((Math.Abs((x - withBlock3.x)) + Math.Abs((y - withBlock3.y))))
            //                    {
            //                        case var case1 when 1 <= case1 && case1 <= 3:
            //                            {
            //                                break;
            //                            }
            //                        // 修正なし
            //                        case 4:
            //                            {
            //                                prob = (0.9d * prob);
            //                                break;
            //                            }

            //                        case 5:
            //                            {
            //                                prob = (0.8d * prob);
            //                                break;
            //                            }

            //                        case 6:
            //                            {
            //                                prob = (0.7d * prob);
            //                                break;
            //                            }

            //                        default:
            //                            {
            //                                prob = (0.6d * prob);
            //                                break;
            //                            }
            //                    }
            //                }
            //            }
            //        }

            //        // ＥＣＭ
            //        var loopTo = GeneralLib.MinLng(withBlock3.x + 2, Map.MapWidth);
            //        for (i = GeneralLib.MaxLng(withBlock3.x - 2, 1); i <= loopTo; i++)
            //        {
            //            var loopTo1 = GeneralLib.MinLng(withBlock3.y + 2, Map.MapHeight);
            //            for (j = GeneralLib.MaxLng(withBlock3.y - 2, 1); j <= loopTo1; j++)
            //            {
            //                if (Math.Abs((withBlock3.x - i)) + Math.Abs((withBlock3.y - j)) <= 3)
            //                {
            //                    u = Map.MapDataForUnit[i, j];
            //                    if (u is object)
            //                    {
            //                        var argt = this;
            //                        if (u.IsAlly(t))
            //                        {
            //                            object argIndex8 = "ＥＣＭ";
            //                            ecm_lv = GeneralLib.MaxDbl(ecm_lv, u.FeatureLevel(argIndex8));
            //                        }
            //                        else if (u.IsAlly(argt))
            //                        {
            //                            object argIndex9 = "ＥＣＭ";
            //                            eccm_lv = GeneralLib.MaxDbl(eccm_lv, u.FeatureLevel(argIndex9));
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //        // ホーミング攻撃はＥＣＭの影響を強く受ける
            //        string argstring26 = "Ｈ";
            //        if (GeneralLib.InStrNotNest(WeaponClass(), argstring26) > 0)
            //        {
            //            prob = ((long)(prob * (100d - 10d * GeneralLib.MaxDbl(ecm_lv - eccm_lv, 0d))) / 100L);
            //        }
            //        else
            //        {
            //            prob = ((long)(prob * (100d - 5d * GeneralLib.MaxDbl(ecm_lv - eccm_lv, 0d))) / 100L);
            //        }
            //    }

            //    // ステルスによる補正
            //    string argfname2 = "ステルス";
            //    string argfname3 = "ステルス無効化";
            //    if (withBlock3.IsFeatureAvailable(argfname2) & !IsFeatureAvailable(argfname3))
            //    {
            //        object argIndex11 = "ステルス";
            //        if (withBlock3.IsFeatureLevelSpecified(argIndex11))
            //        {
            //            object argIndex10 = "ステルス";
            //            if (Math.Abs((x - withBlock3.x)) + Math.Abs((y - withBlock3.y)) > withBlock3.FeatureLevel(argIndex10))
            //            {
            //                prob = (prob * 0.8d);
            //            }
            //        }
            //        else if (Math.Abs((x - withBlock3.x)) + Math.Abs((y - withBlock3.y)) > 3)
            //        {
            //            prob = (prob * 0.8d);
            //        }
            //    }

            //    // 地上から空中の敵に攻撃する
            //    if ((withBlock3.Area == "空中" | withBlock3.Area == "宇宙") & Area != "空中" & Area != "宇宙")
            //    {
            //        string argstring216 = "武";
            //        string argstring217 = "突";
            //        string argstring218 = "接";
            //        if (Conversions.ToBoolean(GeneralLib.InStrNotNest(WeaponClass(), argstring216) | GeneralLib.InStrNotNest(WeaponClass(), argstring217) | GeneralLib.InStrNotNest(WeaponClass(), argstring218)))
            //        {
            //            // ジャンプ攻撃
            //            string argoname4 = "地形適応命中率修正";
            //            if (!Expression.IsOptionDefined(argoname4))
            //            {
            //                string argarea_name1 = "空";
            //                if (!IsTransAvailable(argarea_name1))
            //                {
            //                    uad = get_Adaption(1);
            //                    string argstring214 = "Ｊ";
            //                    if (GeneralLib.InStrNotNest(WeaponClass(), argstring214) > 0)
            //                    {
            //                        string argattr4 = "Ｊ";
            //                        uad = GeneralLib.MinLng((uad + WeaponLevel(argattr4)), 4);
            //                    }

            //                    uad = GeneralLib.MinLng(uad, 4);
            //                    prob = (uad + 6) * prob / 10;
            //                }
            //            }
            //        }
            //        else
            //        {
            //            // 通常攻撃
            //            string argoname5 = "高度修正";
            //            if (Expression.IsOptionDefined(argoname5))
            //            {
            //                string argstring215 = "空";
            //                if (GeneralLib.InStrNotNest(WeaponClass(), argstring215) == 0)
            //                {
            //                    prob = (0.7d * prob);
            //                }
            //            }
            //        }
            //    }

            //    // 局地戦能力
            //    string argfname4 = "地形適応";
            //    if (withBlock3.IsFeatureAvailable(argfname4))
            //    {
            //        var loopTo2 = withBlock3.CountFeature();
            //        for (i = 1; i <= loopTo2; i++)
            //        {
            //            object argIndex13 = i;
            //            if (withBlock3.Feature(argIndex13) == "地形適応")
            //            {
            //                object argIndex12 = i;
            //                buf = withBlock3.FeatureData(argIndex12);
            //                var loopTo3 = GeneralLib.LLength(buf);
            //                for (j = 2; j <= loopTo3; j++)
            //                {
            //                    if ((Map.TerrainName(withBlock3.x, withBlock3.y) ?? "") == (GeneralLib.LIndex(buf, j) ?? ""))
            //                    {
            //                        prob = prob - 10;
            //                        break;
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    // 攻撃回避
            //    string argfname5 = "攻撃回避";
            //    if (withBlock3.IsFeatureAvailable(argfname5))
            //    {
            //        prob_mod = 0d;
            //        var loopTo4 = withBlock3.CountFeature();
            //        for (i = 1; i <= loopTo4; i++)
            //        {
            //            object argIndex16 = i;
            //            if (withBlock3.Feature(argIndex16) == "攻撃回避")
            //            {
            //                object argIndex14 = i;
            //                fdata = withBlock3.FeatureData(argIndex14);
            //                object argIndex15 = i;
            //                flevel = withBlock3.FeatureLevel(argIndex15);

            //                // 必要条件
            //                if (Information.IsNumeric(GeneralLib.LIndex(fdata, 3)))
            //                {
            //                    nmorale = Conversions.Toint(GeneralLib.LIndex(fdata, 3));
            //                }
            //                else
            //                {
            //                    nmorale = 0;
            //                }

            //                // 発動可能？
            //                bool localIsAttributeClassified() { string argaclass1 = GeneralLib.LIndex(fdata, 2); var ret = withBlock3.IsAttributeClassified(argaclass1, wclass); return ret; }

            //                if (withBlock3.MainPilot().Morale >= nmorale & localIsAttributeClassified())
            //                {
            //                    // 攻撃回避発動
            //                    prob_mod = prob_mod + flevel;
            //                }
            //            }
            //        }

            //        prob = ((long)(prob * (10d - prob_mod)) / 10L);
            //    }

            //    // 動けなければ絶対に命中
            //    object argIndex17 = "行動不能";
            //    object argIndex18 = "麻痺";
            //    object argIndex19 = "睡眠";
            //    object argIndex20 = "石化";
            //    object argIndex21 = "凍結";
            //    string argsptype3 = "行動不能";
            //    if (withBlock3.IsConditionSatisfied(argIndex17) | withBlock3.IsConditionSatisfied(argIndex18) | withBlock3.IsConditionSatisfied(argIndex19) | withBlock3.IsConditionSatisfied(argIndex20) | withBlock3.IsConditionSatisfied(argIndex21) | withBlock3.IsUnderSpecialPowerEffect(argsptype3))
            //    {
            //        HitProbabilityRet = 1000;
            //        return HitProbabilityRet;
            //    }

            //    // ステータス異常による修正
            //    string argstring219 = "Ｈ";
            //    string argstring220 = "追";
            //    if (GeneralLib.InStrNotNest(WeaponClass(), argstring219) == 0 & GeneralLib.InStrNotNest(WeaponClass(), argstring220) == 0)
            //    {
            //        object argIndex22 = "撹乱";
            //        if (IsConditionSatisfied(argIndex22))
            //        {
            //            prob = prob / 2;
            //        }

            //        object argIndex23 = "恐怖";
            //        if (IsConditionSatisfied(argIndex23))
            //        {
            //            prob = prob / 2;
            //        }

            //        object argIndex24 = "盲目";
            //        if (IsConditionSatisfied(argIndex24))
            //        {
            //            prob = prob / 2;
            //        }
            //    }

            //    // ターゲットのステータス異常による修正
            //    object argIndex25 = "盲目";
            //    if (withBlock3.IsConditionSatisfied(argIndex25))
            //    {
            //        prob = (1.5d * prob);
            //    }

            //    object argIndex26 = "チャージ";
            //    if (withBlock3.IsConditionSatisfied(argIndex26))
            //    {
            //        prob = (1.5d * prob);
            //    }

            //    object argIndex27 = "消耗";
            //    if (withBlock3.IsConditionSatisfied(argIndex27))
            //    {
            //        prob = (1.5d * prob);
            //    }

            //    object argIndex28 = "狂戦士";
            //    if (withBlock3.IsConditionSatisfied(argIndex28))
            //    {
            //        prob = (1.5d * prob);
            //    }

            //    object argIndex29 = "移動不能";
            //    if (withBlock3.IsConditionSatisfied(argIndex29))
            //    {
            //        prob = (1.5d * prob);
            //    }

            //    // 底力
            //    if (HP <= MaxHP / 4)
            //    {
            //        {
            //            var withBlock4 = MainPilot();
            //            string argsname = "超底力";
            //            string argsname1 = "底力";
            //            if (withBlock4.IsSkillAvailable(argsname))
            //            {
            //                prob = prob + 50;
            //            }
            //            else if (withBlock4.IsSkillAvailable(argsname1))
            //            {
            //                prob = prob + 30;
            //            }
            //        }
            //    }

            //    if (withBlock3.HP <= withBlock3.MaxHP / 4)
            //    {
            //        {
            //            var withBlock5 = withBlock3.MainPilot();
            //            string argsname2 = "超底力";
            //            string argsname3 = "底力";
            //            if (withBlock5.IsSkillAvailable(argsname2))
            //            {
            //                prob = prob - 50;
            //            }
            //            else if (withBlock5.IsSkillAvailable(argsname3))
            //            {
            //                prob = prob - 30;
            //            }
            //        }
            //    }

            //    // スペシャルパワー及び特殊状態による補正
            //    if (is_true_value | mpskill >= 160)
            //    {
            //        string argsptype4 = "命中強化";
            //        object argIndex30 = "運動性ＵＰ";
            //        if (IsUnderSpecialPowerEffect(argsptype4))
            //        {
            //            string argsname4 = "命中強化";
            //            prob = (prob + 10d * SpecialPowerEffectLevel(argsname4));
            //        }
            //        else if (IsConditionSatisfied(argIndex30))
            //        {
            //            prob = prob + 15;
            //        }

            //        string argsptype5 = "回避強化";
            //        object argIndex31 = "運動性ＵＰ";
            //        if (withBlock3.IsUnderSpecialPowerEffect(argsptype5))
            //        {
            //            string argsname5 = "回避強化";
            //            prob = (prob - 10d * withBlock3.SpecialPowerEffectLevel(argsname5));
            //        }
            //        else if (withBlock3.IsConditionSatisfied(argIndex31))
            //        {
            //            prob = prob - 15;
            //        }

            //        object argIndex32 = "運動性ＤＯＷＮ";
            //        if (IsConditionSatisfied(argIndex32))
            //        {
            //            prob = prob - 15;
            //        }

            //        object argIndex33 = "運動性ＤＯＷＮ";
            //        if (withBlock3.IsConditionSatisfied(argIndex33))
            //        {
            //            prob = prob + 15;
            //        }

            //        string argsptype6 = "命中低下";
            //        if (IsUnderSpecialPowerEffect(argsptype6))
            //        {
            //            string argsname6 = "命中低下";
            //            prob = (prob - 10d * SpecialPowerEffectLevel(argsname6));
            //        }

            //        string argsptype7 = "回避低下";
            //        if (withBlock3.IsUnderSpecialPowerEffect(argsptype7))
            //        {
            //            string argsname7 = "回避低下";
            //            prob = (prob + 10d * withBlock3.SpecialPowerEffectLevel(argsname7));
            //        }

            //        string argsptype8 = "命中率低下";
            //        if (IsUnderSpecialPowerEffect(argsptype8))
            //        {
            //            string argsname8 = "命中率低下";
            //            prob = ((long)(prob * (10d - SpecialPowerEffectLevel(argsname8))) / 10L);
            //        }
            //    }
            //}

            //// 最終命中率を定義する。これがないときは何もしない
            //object argIndex35 = "最終命中率";
            //if (SRC.BCList.IsDefined(argIndex35))
            //{
            //    // 事前にデータを登録
            //    BCVariable.DataReset();
            //    BCVariable.MeUnit = this;
            //    BCVariable.AtkUnit = this;
            //    BCVariable.DefUnit = t;
            //    BCVariable.WeaponNumber = w;
            //    BCVariable.LastVariable = prob;
            //    string argIndex34 = "最終命中率";
            //    prob = SRC.BCList.Item(argIndex34).Calculate();
            //}

            return Math.Max(0, prob);
        }

        // 武器 w のユニット t に対するダメージ
        // 敵ユニットはスペシャルパワー等による補正を考慮しないので
        // is_true_value によって補正を省くかどうかを指定できるようにしている
        public int Damage(Unit t, bool is_true_value, bool is_support_attack = false)
        {
            //int arm, arm_mod;
            //int j, i, idx;
            //string ch, wclass, buf;
            //int mpskill;
            //string fname, fdata;
            //double flevel;
            //double slevel;
            //string sdata;
            //int nmorale;
            //bool neautralize;
            //double lv_mod;
            //string opt;
            //string tname;
            //double dmg_mod, uadaption;
            //// 装甲、装甲補正一時保存
            //double ed_amr;
            //double ed_amr_fix;
            //wclass = WeaponClass();

            //return WeaponPower(t.Area);
            //// TODO Impl

            int DamageRet = 0;

            // パイロットの技量によってダメージを正確に予測できるか左右される
            var mpskill = Unit.MainPilot().TacticalTechnique();
            // 武器攻撃力
            DamageRet = WeaponPower(t.Area);
            // 攻撃力が0の場合は常にダメージ0
            if (DamageRet == 0)
            {
                return DamageRet;
            }

            // 基本装甲値
            var arm = t.get_Armor("");

            // アーマー能力
            //if (!t.IsFeatureAvailable("アーマー"))
            //{
            //    goto SkipArmor;
            //}
            //// ザコはアーマーを考慮しない
            //if (!is_true_value & mpskill < 150)
            //{
            //    goto SkipArmor;
            //}

            //arm_mod = 0;
            //var loopTo = t.CountFeature();
            //for (i = 1; i <= loopTo; i++)
            //{
            //    object argIndex6 = i;
            //    if (t.Feature(argIndex6) == "アーマー")
            //    {
            //        object argIndex1 = i;
            //        fname = t.FeatureName0(argIndex1);
            //        object argIndex2 = i;
            //        fdata = t.FeatureData(argIndex2);
            //        object argIndex3 = i;
            //        flevel = t.FeatureLevel(argIndex3);

            //        // 必要条件
            //        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 3)))
            //        {
            //            nmorale = Conversions.Toint(GeneralLib.LIndex(fdata, 3));
            //        }
            //        else
            //        {
            //            nmorale = 0;
            //        }

            //        // オプション
            //        neautralize = false;
            //        slevel = 0d;
            //        var loopTo1 = GeneralLib.LLength(fdata);
            //        for (j = 4; j <= loopTo1; j++)
            //        {
            //            opt = GeneralLib.LIndex(fdata, j);
            //            idx = Strings.InStr(opt, "*");
            //            if (idx > 0)
            //            {
            //                string argexpr = Strings.Mid(opt, idx + 1);
            //                lv_mod = GeneralLib.StrToDbl(argexpr);
            //                opt = Strings.Left(opt, idx - 1);
            //            }
            //            else
            //            {
            //                lv_mod = -1;
            //            }

            //            switch (opt ?? "")
            //            {
            //                case "能力必要":
            //                    {
            //                        break;
            //                    }
            //                // スキップ
            //                case "同調率":
            //                    {
            //                        if (lv_mod == -1)
            //                        {
            //                            lv_mod = 5d;
            //                        }

            //                        slevel = lv_mod * (t.MainPilot().SynchroRate() - 30);
            //                        if (Strings.InStr(fdata, "能力必要") > 0)
            //                        {
            //                            if (slevel == -30 * lv_mod)
            //                            {
            //                                neautralize = true;
            //                            }
            //                        }
            //                        else if (slevel == -30 * lv_mod)
            //                        {
            //                            slevel = 0d;
            //                        }

            //                        break;
            //                    }

            //                case "霊力":
            //                    {
            //                        if (lv_mod == -1)
            //                        {
            //                            lv_mod = 2d;
            //                        }

            //                        slevel = lv_mod * t.MainPilot().Plana;
            //                        if (Strings.InStr(fdata, "能力必要") > 0)
            //                        {
            //                            if (slevel == 0d)
            //                            {
            //                                neautralize = true;
            //                            }
            //                        }

            //                        break;
            //                    }

            //                case "オーラ":
            //                    {
            //                        if (lv_mod == -1)
            //                        {
            //                            lv_mod = 50d;
            //                        }

            //                        slevel = lv_mod * t.AuraLevel();
            //                        if (Strings.InStr(fdata, "能力必要") > 0)
            //                        {
            //                            if (slevel == 0d)
            //                            {
            //                                neautralize = true;
            //                            }
            //                        }

            //                        break;
            //                    }

            //                case "超能力":
            //                    {
            //                        if (lv_mod == -1)
            //                        {
            //                            lv_mod = 50d;
            //                        }

            //                        slevel = lv_mod * t.PsychicLevel();
            //                        if (Strings.InStr(fdata, "能力必要") > 0)
            //                        {
            //                            if (slevel == 0d)
            //                            {
            //                                neautralize = true;
            //                            }
            //                        }

            //                        break;
            //                    }

            //                case "超感覚":
            //                    {
            //                        if (lv_mod == -1)
            //                        {
            //                            lv_mod = 50d;
            //                        }

            //                        object argIndex4 = "超感覚";
            //                        string argref_mode = "";
            //                        object argIndex5 = "知覚強化";
            //                        string argref_mode1 = "";
            //                        slevel = lv_mod * (t.MainPilot().SkillLevel(argIndex4, ref_mode: argref_mode) + t.MainPilot().SkillLevel(argIndex5, ref_mode: argref_mode1));
            //                        if (Strings.InStr(fdata, "能力必要") > 0)
            //                        {
            //                            if (slevel == 0d)
            //                            {
            //                                neautralize = true;
            //                            }
            //                        }

            //                        break;
            //                    }

            //                default:
            //                    {
            //                        if (lv_mod == -1)
            //                        {
            //                            lv_mod = 50d;
            //                        }

            //                        double localSkillLevel() { object argIndex1 = opt; string argref_mode = ""; var ret = t.MainPilot().SkillLevel(argIndex1, ref_mode: argref_mode); return ret; }

            //                        slevel = lv_mod * localSkillLevel();
            //                        if (Strings.InStr(fdata, "能力必要") > 0)
            //                        {
            //                            if (slevel == 0d)
            //                            {
            //                                neautralize = true;
            //                            }
            //                        }

            //                        break;
            //                    }
            //            }
            //        }

            //        // 発動可能？
            //        bool localIsAttributeClassified() { string argaclass1 = GeneralLib.LIndex(fdata, 2); var ret = t.IsAttributeClassified(argaclass1, wclass); return ret; }

            //        if (t.MainPilot().Morale >= nmorale & localIsAttributeClassified() & !neautralize)
            //        {
            //            // アーマー発動
            //            arm_mod = (arm_mod + 100d * flevel + slevel);
            //        }
            //    }
            //}

            //// 装甲が劣化している場合はアーマーによる装甲追加も半減
            //object argIndex7 = "装甲劣化";
            //if (t.IsConditionSatisfied(argIndex7))
            //{
            //    arm_mod = arm_mod / 2;
            //}

            //arm = arm + arm_mod;
            //SkipArmor:
            //    ;


            //    // 地形適応による装甲修正
            //    if (!Expression.IsOptionDefined("地形適応命中率修正"))
            //    {
            //        switch (t.Area ?? "")
            //        {
            //            case "空中":
            //                {
            //                    uadaption = t.get_AdaptionMod(1, 0);
            //                    break;
            //                }

            //            case "地上":
            //                {
            //                    if (Map.TerrainClass(t.x, t.y) == "月面")
            //                    {
            //                        uadaption = t.get_AdaptionMod(4, 0);
            //                    }
            //                    else
            //                    {
            //                        uadaption = t.get_AdaptionMod(2, 0);
            //                    }

            //                    break;
            //                }

            //            case "水上":
            //                {
            //                    uadaption = t.get_AdaptionMod(2, 0);
            //                    break;
            //                }

            //            case "水中":
            //                {
            //                    uadaption = t.get_AdaptionMod(3, 0);
            //                    break;
            //                }

            //            case "宇宙":
            //                {
            //                    uadaption = t.get_AdaptionMod(4, 0);
            //                    break;
            //                }

            //            case "地中":
            //                {
            //                    DamageRet = 0;
            //                    return DamageRet;
            //                }
            //        }

            //        if (uadaption == 0d)
            //        {
            //            uadaption = 0.6d;
            //        }
            //    }
            //    else if (t.Area == "地中")
            //    {
            //        DamageRet = 0;
            //        return DamageRet;
            //    }
            //    else
            //    {
            //        uadaption = 1d;
            //    }

            //    // 不屈による装甲修正
            //    string argsname = "不屈";
            //    if (t.MainPilot().IsSkillAvailable(argsname))
            //    {
            //        string argoname1 = "防御力倍率低下";
            //        if (Expression.IsOptionDefined(argoname1))
            //        {
            //            if (t.HP <= t.MaxHP / 8)
            //            {
            //                arm = (1.15d * arm);
            //            }
            //            else if (t.HP <= t.MaxHP / 4)
            //            {
            //                arm = (1.1d * arm);
            //            }
            //            else if (t.HP <= t.MaxHP / 2)
            //            {
            //                arm = (1.05d * arm);
            //            }
            //        }
            //        else if (t.HP <= t.MaxHP / 8)
            //        {
            //            arm = (1.3d * arm);
            //        }
            //        else if (t.HP <= t.MaxHP / 4)
            //        {
            //            arm = (1.2d * arm);
            //        }
            //        else if (t.HP <= t.MaxHP / 2)
            //        {
            //            arm = (1.1d * arm);
            //        }
            //    }

            //    // スペシャルパワーによる無防備状態
            //    string argsptype = "無防備";
            //    if (t.IsUnderSpecialPowerEffect(argsptype))
            //    {
            //        arm = 0;
            //    }

            //    if (is_true_value | mpskill >= 160)
            //    {
            //        // スペシャルパワーによる修正
            //        string argsptype1 = "装甲強化";
            //        // 装甲強化
            //        object argIndex8 = "防御力ＵＰ";
            //        if (t.IsUnderSpecialPowerEffect(argsptype1))
            //        {
            //            string argsname1 = "装甲強化";
            //            arm = (arm * (1d + 0.1d * t.SpecialPowerEffectLevel(argsname1)));
            //        }
            //        else if (t.IsConditionSatisfied(argIndex8))
            //        {
            //            string argoname2 = "防御力倍率低下";
            //            if (Expression.IsOptionDefined(argoname2))
            //            {
            //                arm = (1.25d * arm);
            //            }
            //            else
            //            {
            //                arm = (1.5d * arm);
            //            }
            //        }

            //        string argsptype2 = "装甲低下";
            //        object argIndex9 = "防御力ＤＯＷＮ";
            //        if (t.IsUnderSpecialPowerEffect(argsptype2))
            //        {
            //            string argsname2 = "装甲低下";
            //            arm = (arm * (1d + 0.1d * t.SpecialPowerEffectLevel(argsname2)));
            //        }
            //        else if (t.IsConditionSatisfied(argIndex9))
            //        {
            //            arm = (0.75d * arm);
            //        }
            //    }

            //    // 貫通型攻撃
            //    string argsptype3 = "貫通攻撃";
            //    string argattr2 = "貫";
            //    if (IsUnderSpecialPowerEffect(argsptype3))
            //    {
            //        arm = arm / 2;
            //    }
            //    else if (IsWeaponClassifiedAs(argattr2))
            //    {
            //        string argattr1 = "貫";
            //        if (IsWeaponLevelSpecified(argattr1))
            //        {
            //            string argattr = "貫";
            //            arm = ((long)(arm * (10d - WeaponLevel(argattr))) / 10L);
            //        }
            //        else
            //        {
            //            arm = arm / 2;
            //        }
            //    }

            //    if (is_true_value | mpskill >= 140)
            //    {
            //        // 弱点
            //        if (t.Weakness(wclass))
            //        {
            //            arm = arm / 2;
            //        }
            //        // 吸収する場合は装甲を無視して判定
            //        else if (!t.Effective(wclass) & t.Absorb(wclass))
            //        {
            //            arm = 0;
            //        }
            //    }

            //    object argIndex11 = "防御補正";
            //    if (SRC.BCList.IsDefined(argIndex11))
            //    {
            //        // バトルコンフィグデータによる計算実行
            //        BCVariable.DataReset();
            //        BCVariable.MeUnit = t;
            //        BCVariable.AtkUnit = this;
            //        BCVariable.DefUnit = t;
            //        BCVariable.WeaponNumber = w;
            //        BCVariable.Armor = arm;
            //        BCVariable.TerrainAdaption = uadaption;
            //        string argIndex10 = "防御補正";
            //        arm = SRC.BCList.Item(argIndex10).Calculate();
            //    }
            //    else
            //    {
            //        {
            //            var withBlock = t.MainPilot();
            //            // 気力による装甲修正
            //            string argoname3 = "気力効果小";
            //            if (Expression.IsOptionDefined(argoname3))
            //            {
            //                arm = arm * (50 + (withBlock.Morale + withBlock.MoraleMod) / 2) / 100;
            //            }
            //            else
            //            {
            //                arm = arm * (withBlock.Morale + withBlock.MoraleMod) / 100;
            //            }

            //            // レベルアップによる装甲修正＋耐久能力
            //            arm = arm * withBlock.Defense / 100;
            //        }

            //        // 地形適応による装甲修正
            //        arm = (arm * uadaption);
            //    }

            //    // ダメージ固定武器の場合は装甲と地形＆距離修正を無視
            //    string argstring2 = "固";
            //    if (GeneralLib.InStrNotNest(WeaponClass(), argstring2) > 0)
            //    {
            //        goto SkipDamageMod;
            //    }

            //    object argIndex13 = "ダメージ";
            //    if (SRC.BCList.IsDefined(argIndex13))
            //    {
            //        // バトルコンフィグデータによる計算実行
            //        // 事前にデータを登録
            //        BCVariable.DataReset();
            //        BCVariable.MeUnit = this;
            //        BCVariable.AtkUnit = this;
            //        BCVariable.DefUnit = t;
            //        BCVariable.WeaponNumber = w;
            //        BCVariable.AttackVariable = DamageRet;
            //        BCVariable.DffenceVariable = arm;
            //        if (Map.TerrainClass(t.x, t.y) == "月面")
            //        {
            //            if (t.Area == "地上")
            //            {
            //                BCVariable.TerrainAdaption = (100 - Map.TerrainEffectForDamage(t.x, t.y)) / 100d;
            //            }
            //            else
            //            {
            //                BCVariable.TerrainAdaption = 1d;
            //            }
            //        }
            //        else if (t.Area != "空中")
            //        {
            //            BCVariable.TerrainAdaption = (100 - Map.TerrainEffectForDamage(t.x, t.y)) / 100d;
            //        }
            //        else
            //        {
            //            BCVariable.TerrainAdaption = 1d;
            //        }

            //        string argIndex12 = "ダメージ";
            //        DamageRet = SRC.BCList.Item(argIndex12).Calculate();
            //    }
            //    else
            {
                // 装甲値によってダメージを軽減
                DamageRet = DamageRet - arm;

                // TODO Impl
                //// 地形補正
                //if (Map.Terrain(t.x, t.y).Class == "月面")
                //{
                //    if (t.Area == "地上")
                //    {
                //        DamageRet = (DamageRet * ((100 - Map.Terrain(t.x, t.y).EffectForDamage) / 100d));
                //    }
                //}
                //else if (t.Area != "空中")
                //{
                //    DamageRet = (DamageRet * ((100 - Map.Terrain(t.x, t.y).EffectForDamage) / 100d));
                //}
            }

            //    // 散属性武器は離れるほどダメージダウン
            //    string argstring21 = "散";
            //    if (GeneralLib.InStrNotNest(WeaponClass(), argstring21) > 0)
            //    {
            //        switch ((Math.Abs((x - t.x)) + Math.Abs((y - t.y))))
            //        {
            //            case 1:
            //                {
            //                    break;
            //                }
            //            // 修正なし
            //            case 2:
            //                {
            //                    DamageRet = (0.95d * DamageRet);
            //                    break;
            //                }

            //            case 3:
            //                {
            //                    DamageRet = (0.9d * DamageRet);
            //                    break;
            //                }

            //            case 4:
            //                {
            //                    DamageRet = (0.85d * DamageRet);
            //                    break;
            //                }

            //            default:
            //                {
            //                    DamageRet = (0.8d * DamageRet);
            //                    break;
            //                }
            //        }
            //    }

            //    // 距離修正
            //    string argoname6 = "距離修正";
            //    if (Expression.IsOptionDefined(argoname6))
            //    {
            //        string argstring22 = "実";
            //        string argstring23 = "武";
            //        string argstring24 = "突";
            //        string argstring25 = "接";
            //        string argstring26 = "爆";
            //        if (GeneralLib.InStrNotNest(WeaponClass(), argstring22) == 0 & GeneralLib.InStrNotNest(WeaponClass(), argstring23) == 0 & GeneralLib.InStrNotNest(WeaponClass(), argstring24) == 0 & GeneralLib.InStrNotNest(WeaponClass(), argstring25) == 0 & GeneralLib.InStrNotNest(WeaponClass(), argstring26) == 0)
            //        {
            //            string argoname4 = "大型マップ";
            //            string argoname5 = "小型マップ";
            //            if (Expression.IsOptionDefined(argoname4))
            //            {
            //                switch ((Math.Abs((x - t.x)) + Math.Abs((y - t.y))))
            //                {
            //                    case var @case when 1 <= @case && @case <= 4:
            //                        {
            //                            break;
            //                        }
            //                    // 修正なし
            //                    case 5:
            //                    case 6:
            //                        {
            //                            DamageRet = (0.95d * DamageRet);
            //                            break;
            //                        }

            //                    case 7:
            //                    case 8:
            //                        {
            //                            DamageRet = (0.9d * DamageRet);
            //                            break;
            //                        }

            //                    case 9:
            //                    case 10:
            //                        {
            //                            DamageRet = (0.85d * DamageRet);
            //                            break;
            //                        }

            //                    default:
            //                        {
            //                            DamageRet = (0.8d * DamageRet);
            //                            break;
            //                        }
            //                }
            //            }
            //            else if (Expression.IsOptionDefined(argoname5))
            //            {
            //                switch ((Math.Abs((x - t.x)) + Math.Abs((y - t.y))))
            //                {
            //                    case 1:
            //                        {
            //                            break;
            //                        }
            //                    // 修正なし
            //                    case 2:
            //                        {
            //                            DamageRet = (0.95d * DamageRet);
            //                            break;
            //                        }

            //                    case 3:
            //                        {
            //                            DamageRet = (0.9d * DamageRet);
            //                            break;
            //                        }

            //                    case 4:
            //                        {
            //                            DamageRet = (0.85d * DamageRet);
            //                            break;
            //                        }

            //                    case 5:
            //                        {
            //                            DamageRet = (0.8d * DamageRet);
            //                            break;
            //                        }

            //                    case 6:
            //                        {
            //                            DamageRet = (0.75d * DamageRet);
            //                            break;
            //                        }

            //                    default:
            //                        {
            //                            DamageRet = (0.7d * DamageRet);
            //                            break;
            //                        }
            //                }
            //            }
            //            else
            //            {
            //                switch ((Math.Abs((x - t.x)) + Math.Abs((y - t.y))))
            //                {
            //                    case var case1 when 1 <= case1 && case1 <= 3:
            //                        {
            //                            break;
            //                        }
            //                    // 修正なし
            //                    case 4:
            //                        {
            //                            DamageRet = (0.95d * DamageRet);
            //                            break;
            //                        }

            //                    case 5:
            //                        {
            //                            DamageRet = (0.9d * DamageRet);
            //                            break;
            //                        }

            //                    case 6:
            //                        {
            //                            DamageRet = (0.85d * DamageRet);
            //                            break;
            //                        }

            //                    default:
            //                        {
            //                            DamageRet = (0.8d * DamageRet);
            //                            break;
            //                        }
            //                }
            //            }
            //        }
            //    }

            //SkipDamageMod:
            //    ;


            //    // 封印攻撃は弱点もしくは有効を持つユニット以外には効かない
            //    string argstring27 = "封";
            //    if (GeneralLib.InStrNotNest(WeaponClass(), argstring27) > 0)
            //    {
            //        buf = t.strWeakness + t.strEffective;
            //        var loopTo2 = Strings.Len(buf);
            //        for (i = 1; i <= loopTo2; i++)
            //        {
            //            // 属性をひとまとめずつ取得
            //            ch = GeneralLib.GetClassBundle(buf, i);
            //            if (ch != "物" & ch != "魔")
            //            {
            //                if (GeneralLib.InStrNotNest(WeaponClass(), ch) > 0)
            //                {
            //                    break;
            //                }
            //            }
            //        }

            //        if (i > Strings.Len(buf))
            //        {
            //            DamageRet = 0;
            //            return DamageRet;
            //        }
            //    }

            //    // 限定攻撃は指定属性に対して弱点もしくは有効を持つユニット以外には効かない
            //    string argstring28 = "限";
            //    idx = GeneralLib.InStrNotNest(WeaponClass(), argstring28);
            //    if (idx > 0)
            //    {
            //        buf = t.strWeakness + t.strEffective;
            //        var loopTo3 = Strings.Len(buf);
            //        for (i = 1; i <= loopTo3; i++)
            //        {
            //            // 属性をひとまとめずつ取得
            //            ch = GeneralLib.GetClassBundle(buf, i);
            //            if (ch != "物" & ch != "魔")
            //            {
            //                if (GeneralLib.InStrNotNest(WeaponClass(), ch) > idx)
            //                {
            //                    break;
            //                }
            //            }
            //        }

            //        if (i > Strings.Len(buf))
            //        {
            //            DamageRet = 0;
            //            return DamageRet;
            //        }
            //    }

            //    // 特定レベル限定攻撃
            //    string argattr4 = "対";
            //    if (WeaponLevel(argattr4) > 0d)
            //    {
            //        // UPGRADE_WARNING: Mod に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            //        string argattr3 = "対";
            //        if (t.MainPilot().Level % WeaponLevel(argattr3) != 0d)
            //        {
            //            DamageRet = 0;
            //            return DamageRet;
            //        }
            //    }

            //    if (is_true_value | mpskill >= 140)
            //    {
            //        // 弱点、有効、吸収を優先
            //        if (!t.Weakness(wclass) & !t.Effective(wclass) & !t.Absorb(wclass))
            //        {
            //            // 無効化
            //            if (t.Immune(wclass))
            //            {
            //                DamageRet = 0;
            //                return DamageRet;
            //            }
            //            // 耐性
            //            else if (t.Resist(wclass))
            //            {
            //                DamageRet = DamageRet / 2;
            //            }
            //        }
            //    }

            //    // 盲目状態には視覚攻撃は効かない
            //    if (is_true_value | mpskill >= 140)
            //    {
            //        string argstring29 = "視";
            //        if (GeneralLib.InStrNotNest(WeaponClass(), argstring29) > 0)
            //        {
            //            object argIndex14 = "盲目";
            //            if (t.IsConditionSatisfied(argIndex14))
            //            {
            //                DamageRet = 0;
            //                return DamageRet;
            //            }
            //        }
            //    }

            //    // 機械には精神攻撃は効かない
            //    if (is_true_value | mpskill >= 140)
            //    {
            //        string argstring210 = "精";
            //        if (GeneralLib.InStrNotNest(WeaponClass(), argstring210) > 0)
            //        {
            //            if (t.MainPilot().Personality == "機械")
            //            {
            //                DamageRet = 0;
            //                return DamageRet;
            //            }
            //        }
            //    }

            //    // 性別限定武器
            //    string argstring211 = "♂";
            //    if (GeneralLib.InStrNotNest(WeaponClass(), argstring211) > 0)
            //    {
            //        if (t.MainPilot().Sex != "男性")
            //        {
            //            DamageRet = 0;
            //            return DamageRet;
            //        }
            //    }

            //    string argstring212 = "♀";
            //    if (GeneralLib.InStrNotNest(WeaponClass(), argstring212) > 0)
            //    {
            //        if (t.MainPilot().Sex != "女性")
            //        {
            //            DamageRet = 0;
            //            return DamageRet;
            //        }
            //    }

            //    // 寝こみを襲うとダメージ1.5倍
            //    object argIndex15 = "睡眠";
            //    if (t.IsConditionSatisfied(argIndex15))
            //    {
            //        DamageRet = (1.5d * DamageRet);
            //    }

            //    {
            //        var withBlock1 = MainPilot();
            //        // 高気力時のダメージ増加能力
            //        if (withBlock1.Morale >= 130)
            //        {
            //            string argoname7 = "ダメージ倍率低下";
            //            if (Expression.IsOptionDefined(argoname7))
            //            {
            //                string argsname3 = "潜在力開放";
            //                if (withBlock1.IsSkillAvailable(argsname3))
            //                {
            //                    DamageRet = (1.2d * DamageRet);
            //                }

            //                string argfname1 = "ブースト";
            //                if (IsFeatureAvailable(argfname1))
            //                {
            //                    DamageRet = (1.2d * DamageRet);
            //                }
            //            }
            //            else
            //            {
            //                string argsname4 = "潜在力開放";
            //                if (withBlock1.IsSkillAvailable(argsname4))
            //                {
            //                    DamageRet = (1.25d * DamageRet);
            //                }

            //                string argfname2 = "ブースト";
            //                if (IsFeatureAvailable(argfname2))
            //                {
            //                    DamageRet = (1.25d * DamageRet);
            //                }
            //            }
            //        }

            //        // 得意技
            //        string argsname5 = "得意技";
            //        if (withBlock1.IsSkillAvailable(argsname5))
            //        {
            //            object argIndex16 = "得意技";
            //            sdata = withBlock1.SkillData(argIndex16);
            //            var loopTo4 = Strings.Len(sdata);
            //            for (i = 1; i <= loopTo4; i++)
            //            {
            //                string argstring213 = Strings.Mid(sdata, i, 1);
            //                if (GeneralLib.InStrNotNest(WeaponClass(), argstring213) > 0)
            //                {
            //                    DamageRet = (1.2d * DamageRet);
            //                    break;
            //                }
            //            }
            //        }

            //        // 不得手
            //        string argsname6 = "不得手";
            //        if (withBlock1.IsSkillAvailable(argsname6))
            //        {
            //            object argIndex17 = "不得手";
            //            sdata = withBlock1.SkillData(argIndex17);
            //            var loopTo5 = Strings.Len(sdata);
            //            for (i = 1; i <= loopTo5; i++)
            //            {
            //                string argstring214 = Strings.Mid(sdata, i, 1);
            //                if (GeneralLib.InStrNotNest(WeaponClass(), argstring214) > 0)
            //                {
            //                    DamageRet = (0.8d * DamageRet);
            //                    break;
            //                }
            //            }
            //        }
            //    }

            //    // ハンター能力
            //    // (ターゲットのMainPilotを参照するため、「With .MainPilot」は使えない)
            //    string argsname7 = "ハンター";
            //    if (MainPilot().IsSkillAvailable(argsname7))
            //    {
            //        var loopTo6 = MainPilot().CountSkill();
            //        for (i = 1; i <= loopTo6; i++)
            //        {
            //            object argIndex19 = i;
            //            if (MainPilot().Skill(argIndex19) == "ハンター")
            //            {
            //                object argIndex18 = i;
            //                sdata = MainPilot().SkillData(argIndex18);
            //                var loopTo7 = GeneralLib.LLength(sdata);
            //                for (j = 2; j <= loopTo7; j++)
            //                {
            //                    tname = GeneralLib.LIndex(sdata, j);
            //                    if ((t.Name ?? "") == (tname ?? "") | (t.Class0 ?? "") == (tname ?? "") | (t.Size + "サイズ" ?? "") == (tname ?? "") | (t.MainPilot().Name ?? "") == (tname ?? "") | (t.MainPilot().Sex ?? "") == (tname ?? ""))
            //                    {
            //                        break;
            //                    }
            //                }

            //                if (j <= GeneralLib.LLength(sdata))
            //                {
            //                    double localSkillLevel1() { object argIndex1 = i; string argref_mode = ""; var ret = MainPilot().SkillLevel(argIndex1, ref_mode: argref_mode); return ret; }

            //                    DamageRet = ((long)((10d + localSkillLevel1()) * DamageRet) / 10L);
            //                    break;
            //                }
            //            }
            //        }

            //        object argIndex22 = "ハンター付加";
            //        object argIndex23 = "ハンター付加２";
            //        if (IsConditionSatisfied(argIndex22) | IsConditionSatisfied(argIndex23))
            //        {
            //            object argIndex20 = "ハンター";
            //            sdata = MainPilot().SkillData(argIndex20);
            //            var loopTo8 = GeneralLib.LLength(sdata);
            //            for (i = 2; i <= loopTo8; i++)
            //            {
            //                tname = GeneralLib.LIndex(sdata, i);
            //                if ((t.Name ?? "") == (tname ?? "") | (t.Class0 ?? "") == (tname ?? "") | (t.Size + "サイズ" ?? "") == (tname ?? "") | (t.MainPilot().Name ?? "") == (tname ?? "") | (t.MainPilot().Sex ?? "") == (tname ?? ""))
            //                {
            //                    break;
            //                }
            //            }

            //            if (i <= GeneralLib.LLength(sdata))
            //            {
            //                object argIndex21 = "ハンター";
            //                string argref_mode2 = "";
            //                DamageRet = ((long)((10d + MainPilot().SkillLevel(argIndex21, ref_mode: argref_mode2)) * DamageRet) / 10L);
            //            }
            //        }
            //    }

            //    // スペシャルパワー、特殊状態によるダメージ増加
            //    dmg_mod = 1d;
            //    object argIndex24 = "攻撃力ＵＰ";
            //    object argIndex25 = "狂戦士";
            //    if (IsConditionSatisfied(argIndex24) | IsConditionSatisfied(argIndex25))
            //    {
            //        string argoname8 = "ダメージ倍率低下";
            //        if (Expression.IsOptionDefined(argoname8))
            //        {
            //            dmg_mod = 1.2d;
            //        }
            //        else
            //        {
            //            dmg_mod = 1.25d;
            //        }
            //    }
            //    // サポートアタックの場合はスペシャルパワーによる修正が無い
            //    if (!is_support_attack)
            //    {
            //        if (is_true_value | mpskill >= 160)
            //        {
            //            // スペシャルパワーによるダメージ増加は特殊状態による増加と重複しない
            //            string argsname8 = "ダメージ増加";
            //            dmg_mod = GeneralLib.MaxDbl(dmg_mod, 1d + 0.1d * SpecialPowerEffectLevel(argsname8));
            //            string argsname9 = "被ダメージ増加";
            //            dmg_mod = dmg_mod + 0.1d * t.SpecialPowerEffectLevel(argsname9);
            //        }
            //    }

            //    DamageRet = (dmg_mod * DamageRet);

            //    // スペシャルパワー、特殊状態、サポートアタックによるダメージ低下
            //    if (is_true_value | mpskill >= 160)
            //    {
            //        dmg_mod = 1d;
            //        string argsname10 = "ダメージ低下";
            //        dmg_mod = dmg_mod - 0.1d * SpecialPowerEffectLevel(argsname10);
            //        string argsname11 = "被ダメージ低下";
            //        dmg_mod = dmg_mod - 0.1d * t.SpecialPowerEffectLevel(argsname11);
            //        DamageRet = (dmg_mod * DamageRet);
            //    }

            //    object argIndex26 = "攻撃力ＤＯＷＮ";
            //    if (IsConditionSatisfied(argIndex26))
            //    {
            //        DamageRet = (0.75d * DamageRet);
            //    }

            //    object argIndex27 = "恐怖";
            //    if (IsConditionSatisfied(argIndex27))
            //    {
            //        DamageRet = (0.8d * DamageRet);
            //    }

            //    if (is_support_attack)
            //    {
            //        // サポートアタックダメージ低下
            //        string argoname9 = "サポートアタックダメージ低下";
            //        if (Expression.IsOptionDefined(argoname9))
            //        {
            //            DamageRet = (0.7d * DamageRet);
            //        }
            //    }

            //    // レジスト能力
            //    dmg_mod = 0d;
            //    string argfname3 = "レジスト";
            //    if (!t.IsFeatureAvailable(argfname3))
            //    {
            //        goto SkipResist;
            //    }
            //    // ザコはレジストを考慮しない
            //    if (!is_true_value & mpskill < 150)
            //    {
            //        goto SkipResist;
            //    }

            //    var loopTo9 = t.CountFeature();
            //    for (i = 1; i <= loopTo9; i++)
            //    {
            //        object argIndex33 = i;
            //        if (t.Feature(argIndex33) == "レジスト")
            //        {
            //            object argIndex28 = i;
            //            fname = t.FeatureName0(argIndex28);
            //            object argIndex29 = i;
            //            fdata = t.FeatureData(argIndex29);
            //            object argIndex30 = i;
            //            flevel = t.FeatureLevel(argIndex30);

            //            // 必要条件
            //            if (Information.IsNumeric(GeneralLib.LIndex(fdata, 3)))
            //            {
            //                nmorale = Conversions.Toint(GeneralLib.LIndex(fdata, 3));
            //            }
            //            else
            //            {
            //                nmorale = 0;
            //            }

            //            // オプション
            //            neautralize = false;
            //            slevel = 0d;
            //            var loopTo10 = GeneralLib.LLength(fdata);
            //            for (j = 4; j <= loopTo10; j++)
            //            {
            //                opt = GeneralLib.LIndex(fdata, j);
            //                idx = Strings.InStr(opt, "*");
            //                if (idx > 0)
            //                {
            //                    string argexpr1 = Strings.Mid(opt, idx + 1);
            //                    lv_mod = GeneralLib.StrToDbl(argexpr1);
            //                    opt = Strings.Left(opt, idx - 1);
            //                }
            //                else
            //                {
            //                    lv_mod = -1;
            //                }

            //                switch (opt ?? "")
            //                {
            //                    case "能力必要":
            //                        {
            //                            break;
            //                        }
            //                    // スキップ
            //                    case "同調率":
            //                        {
            //                            if (lv_mod == -1)
            //                            {
            //                                lv_mod = 0.5d;
            //                            }

            //                            slevel = lv_mod * (t.MainPilot().SynchroRate() - 30);
            //                            if (Strings.InStr(fdata, "能力必要") > 0)
            //                            {
            //                                if (slevel == -30 * lv_mod)
            //                                {
            //                                    neautralize = true;
            //                                }
            //                            }
            //                            else if (slevel == -30 * lv_mod)
            //                            {
            //                                slevel = 0d;
            //                            }

            //                            break;
            //                        }

            //                    case "霊力":
            //                        {
            //                            if (lv_mod == -1)
            //                            {
            //                                lv_mod = 0.2d;
            //                            }

            //                            slevel = lv_mod * t.MainPilot().Plana;
            //                            if (Strings.InStr(fdata, "能力必要") > 0)
            //                            {
            //                                if (slevel == 0d)
            //                                {
            //                                    neautralize = true;
            //                                }
            //                            }

            //                            break;
            //                        }

            //                    case "オーラ":
            //                        {
            //                            if (lv_mod == -1)
            //                            {
            //                                lv_mod = 5d;
            //                            }

            //                            slevel = lv_mod * t.AuraLevel();
            //                            if (Strings.InStr(fdata, "能力必要") > 0)
            //                            {
            //                                if (slevel == 0d)
            //                                {
            //                                    neautralize = true;
            //                                }
            //                            }

            //                            break;
            //                        }

            //                    case "超能力":
            //                        {
            //                            if (lv_mod == -1)
            //                            {
            //                                lv_mod = 5d;
            //                            }

            //                            slevel = lv_mod * t.PsychicLevel();
            //                            if (Strings.InStr(fdata, "能力必要") > 0)
            //                            {
            //                                if (slevel == 0d)
            //                                {
            //                                    neautralize = true;
            //                                }
            //                            }

            //                            break;
            //                        }

            //                    case "超感覚":
            //                        {
            //                            if (lv_mod == -1)
            //                            {
            //                                lv_mod = 5d;
            //                            }

            //                            object argIndex31 = "超感覚";
            //                            string argref_mode3 = "";
            //                            object argIndex32 = "知覚強化";
            //                            string argref_mode4 = "";
            //                            slevel = lv_mod * (t.MainPilot().SkillLevel(argIndex31, ref_mode: argref_mode3) + t.MainPilot().SkillLevel(argIndex32, ref_mode: argref_mode4));
            //                            if (Strings.InStr(fdata, "能力必要") > 0)
            //                            {
            //                                if (slevel == 0d)
            //                                {
            //                                    neautralize = true;
            //                                }
            //                            }

            //                            break;
            //                        }

            //                    default:
            //                        {
            //                            if (lv_mod == -1)
            //                            {
            //                                lv_mod = 5d;
            //                            }

            //                            double localSkillLevel2() { object argIndex1 = opt; string argref_mode = ""; var ret = t.MainPilot().SkillLevel(argIndex1, ref_mode: argref_mode); return ret; }

            //                            slevel = lv_mod * localSkillLevel2();
            //                            if (Strings.InStr(fdata, "能力必要") > 0)
            //                            {
            //                                if (slevel == 0d)
            //                                {
            //                                    neautralize = true;
            //                                }
            //                            }

            //                            break;
            //                        }
            //                }
            //            }

            //            // 発動可能？
            //            bool localIsAttributeClassified1() { string argaclass1 = GeneralLib.LIndex(fdata, 2); var ret = t.IsAttributeClassified(argaclass1, wclass); return ret; }

            //            if (t.MainPilot().Morale >= nmorale & localIsAttributeClassified1() & !neautralize)
            //            {
            //                // レジスト発動
            //                dmg_mod = dmg_mod + 10d * flevel + slevel;
            //            }
            //        }
            //    }

            //    DamageRet = ((long)(DamageRet * (100d - dmg_mod)) / 100L);
            //SkipResist:
            //    ;
            //    object argIndex35 = "最終ダメージ";
            //    if (SRC.BCList.IsDefined(argIndex35))
            //    {
            //        // バトルコンフィグデータによる計算実行
            //        BCVariable.DataReset();
            //        BCVariable.MeUnit = this;
            //        BCVariable.AtkUnit = this;
            //        BCVariable.DefUnit = t;
            //        BCVariable.WeaponNumber = w;
            //        BCVariable.LastVariable = DamageRet;
            //        string argIndex34 = "最終ダメージ";
            //        DamageRet = SRC.BCList.Item(argIndex34).Calculate();
            //    }

            //// 最低ダメージは10
            //if (dmg_mod < 100d)
            //{
            //    if (DamageRet < 10)
            //    {
            //        if (Expression.IsOptionDefined("ダメージ下限解除"))
            //        {
            //            DamageRet = GeneralLib.MaxLng(DamageRet, 0);
            //        }
            //        else if (Expression.IsOptionDefined("ダメージ下限１"))
            //        {
            //            DamageRet = GeneralLib.MaxLng(DamageRet, 1);
            //        }
            //        else
            //        {
            //            DamageRet = 10;
            //        }
            //    }
            //}

            //    // ダメージを吸収する場合は最後に反転
            //    if (is_true_value | mpskill >= 140)
            //    {
            //        // 弱点、有効を優先
            //        if (!t.Weakness(wclass) & !t.Effective(wclass))
            //        {
            //            // 吸収
            //            if (DamageRet > 0 & t.Absorb(wclass))
            //            {
            //                DamageRet = -DamageRet / 2;
            //            }
            //        }
            //    }

            return DamageRet;
        }

        // クリティカルの発生率
        public int CriticalProbability(Unit t, string def_mode = "")
        {
            return 0;
            // TODO Impl
            //int CriticalProbabilityRet = default;
            //int i, prob, idx;
            //string wclass;
            //string buf, c;
            //var is_special = default(bool);
            //// クリティカル攻撃、防御の一時保存変数
            //int ed_crtatk, ed_crtdfe;
            //if (IsNormalWeapon(w))
            //{
            //    // 通常攻撃

            //    // スペシャルパワーとの効果の重ね合わせが禁止されている場合
            //    if (Expression.IsOptionDefined("スペシャルパワー使用時クリティカル無効") 
            //        || Expression.IsOptionDefined("精神コマンド使用時クリティカル無効"))
            //    {
            //        if (IsUnderSpecialPowerEffect("ダメージ増加"))
            //        {
            //            return CriticalProbabilityRet;
            //        }
            //    }

            //    // 攻撃側による補正
            //    if (SRC.BCList.IsDefined("クリティカル攻撃補正"))
            //    {
            //        // バトルコンフィグデータの設定による修正
            //        // 一時保存変数に一時保存
            //        // 事前にデータを登録
            //        BCVariable.DataReset();
            //        BCVariable.MeUnit = this;
            //        BCVariable.AtkUnit = this;
            //        BCVariable.DefUnit = t;
            //        BCVariable.WeaponNumber = w;
            //        BCVariable.AttackExp = WeaponCritical(w);
            //        string argIndex1 = "クリティカル攻撃補正";
            //        ed_crtatk = SRC.BCList.Item(argIndex1).Calculate();
            //    }
            //    else
            //    {
            //        // 一時保存変数に一時保存
            //        ed_crtatk = (WeaponCritical(w) + this.MainPilot().Technique);
            //    }

            //    // 防御側による補正
            //    if (SRC.BCList.IsDefined("クリティカル防御補正"))
            //    {
            //        // バトルコンフィグデータの設定による修正
            //        // 一時保存変数に一時保存
            //        // 事前にデータを登録
            //        BCVariable.DataReset();
            //        BCVariable.MeUnit = t;
            //        BCVariable.AtkUnit = this;
            //        BCVariable.DefUnit = t;
            //        BCVariable.WeaponNumber = w;
            //        string argIndex3 = "クリティカル防御補正";
            //        ed_crtdfe = SRC.BCList.Item(argIndex3).Calculate();
            //    }
            //    else
            //    {
            //        // 一時保存変数に一時保存
            //        ed_crtdfe = t.MainPilot().Technique;
            //    }

            //    // クリティカル発生率計算
            //    if (SRC.BCList.IsDefined("クリティカル発生率"))
            //    {
            //        // 事前にデータを登録
            //        BCVariable.DataReset();
            //        BCVariable.MeUnit = this;
            //        BCVariable.AtkUnit = this;
            //        BCVariable.DefUnit = t;
            //        BCVariable.WeaponNumber = w;
            //        BCVariable.AttackVariable = ed_crtatk;
            //        BCVariable.DffenceVariable = ed_crtdfe;
            //        string argIndex5 = "クリティカル発生率";
            //        prob = SRC.BCList.Item(argIndex5).Calculate();
            //    }
            //    else
            //    {
            //        prob = (ed_crtatk - ed_crtdfe);
            //    }

            //    // 超反応による修正
            //    object argIndex7 = "超反応";
            //    string argref_mode = "";
            //    object argIndex8 = "超反応";
            //    string argref_mode1 = "";
            //    prob = (prob + 2d * MainPilot().SkillLevel(argIndex7, ref_mode: argref_mode) - 2d * t.MainPilot().SkillLevel(argIndex8, ref_mode: argref_mode1));

            //    // 超能力による修正
            //    string argsname = "超能力";
            //    if (MainPilot().IsSkillAvailable(argsname))
            //    {
            //        prob = (prob + 5);
            //    }

            //    // 底力、超底力、覚悟による修正
            //    if (HP <= MaxHP / 4)
            //    {
            //        string argsname1 = "底力";
            //        string argsname2 = "超底力";
            //        string argsname3 = "覚悟";
            //        if (MainPilot().IsSkillAvailable(argsname1) | MainPilot().IsSkillAvailable(argsname2) | MainPilot().IsSkillAvailable(argsname3))
            //        {
            //            prob = (prob + 50);
            //        }
            //    }

            //    // スペシャルパワーにる修正
            //    string argsname4 = "クリティカル率増加";
            //    prob = (prob + 10d * SpecialPowerEffectLevel(argsname4));
            //}
            //else
            //{
            //    // 特殊効果を伴う攻撃
            //    is_special = true;

            //    // 攻撃側による補正
            //    object argIndex10 = "特殊効果攻撃補正";
            //    if (SRC.BCList.IsDefined(argIndex10))
            //    {
            //        // バトルコンフィグデータの設定による修正
            //        // 一時保存変数に一時保存
            //        // 事前にデータを登録
            //        BCVariable.DataReset();
            //        BCVariable.MeUnit = this;
            //        BCVariable.AtkUnit = this;
            //        BCVariable.DefUnit = t;
            //        BCVariable.WeaponNumber = w;
            //        BCVariable.AttackExp = WeaponCritical(w);
            //        string argIndex9 = "特殊効果攻撃補正";
            //        ed_crtatk = SRC.BCList.Item(argIndex9).Calculate();
            //    }
            //    else
            //    {
            //        // 一時保存変数に一時保存
            //        ed_crtatk = (WeaponCritical(w) + this.MainPilot().Technique / 2);
            //    }

            //    // 防御側による補正
            //    object argIndex12 = "特殊効果防御補正";
            //    if (SRC.BCList.IsDefined(argIndex12))
            //    {
            //        // バトルコンフィグデータの設定による修正
            //        // 一時保存変数に一時保存
            //        // 事前にデータを登録
            //        BCVariable.DataReset();
            //        BCVariable.MeUnit = t;
            //        BCVariable.AtkUnit = this;
            //        BCVariable.DefUnit = t;
            //        BCVariable.WeaponNumber = w;
            //        // 特殊効果の場合は相手がザコの時に確率が増加
            //        if (Strings.InStr(t.MainPilot().Name, "(ザコ)") > 0)
            //        {
            //            BCVariable.CommonEnemy = 30;
            //        }

            //        string argIndex11 = "特殊効果防御補正";
            //        ed_crtdfe = SRC.BCList.Item(argIndex11).Calculate();
            //    }
            //    else
            //    {
            //        // 一時保存変数に一時保存
            //        ed_crtdfe = (t.MainPilot().Technique / 2);

            //        // 特殊効果の場合は相手がザコの時に確率が増加
            //        if (Strings.InStr(t.MainPilot().Name, "(ザコ)") > 0)
            //        {
            //            // 一時保存変数に一時保存
            //            ed_crtdfe = (ed_crtdfe - 30);
            //        }
            //    }

            //    // 特殊効果発生率計算
            //    object argIndex14 = "特殊効果発生率";
            //    if (SRC.BCList.IsDefined(argIndex14))
            //    {
            //        // 事前にデータを登録
            //        BCVariable.DataReset();
            //        BCVariable.MeUnit = this;
            //        BCVariable.AtkUnit = this;
            //        BCVariable.DefUnit = t;
            //        BCVariable.WeaponNumber = w;
            //        BCVariable.AttackVariable = ed_crtatk;
            //        BCVariable.DffenceVariable = ed_crtdfe;
            //        string argIndex13 = "特殊効果発生率";
            //        prob = SRC.BCList.Item(argIndex13).Calculate();
            //    }
            //    else
            //    {
            //        prob = (ed_crtatk - ed_crtdfe);
            //    }

            //    // 抵抗力による修正
            //    object argIndex15 = "抵抗力";
            //    prob = (prob - 10d * t.FeatureLevel(argIndex15));
            //}

            //// 不意打ち
            //string argfname = "ステルス";
            //object argIndex16 = "ステルス無効";
            //string argfname1 = "ステルス無効化";
            //string argattr = "忍";
            //if (IsFeatureAvailable(argfname) & !IsConditionSatisfied(argIndex16) & !t.IsFeatureAvailable(argfname1) & IsWeaponClassifiedAs(argattr))
            //{
            //    prob = (prob + 10);
            //}

            //// 相手が動けなければ確率アップ
            //object argIndex17 = "行動不能";
            //object argIndex18 = "石化";
            //object argIndex19 = "凍結";
            //object argIndex20 = "麻痺";
            //object argIndex21 = "睡眠";
            //string argsptype1 = "行動不能";
            //if (t.IsConditionSatisfied(argIndex17) | t.IsConditionSatisfied(argIndex18) | t.IsConditionSatisfied(argIndex19) | t.IsConditionSatisfied(argIndex20) | t.IsConditionSatisfied(argIndex21) | t.IsUnderSpecialPowerEffect(argsptype1))
            //{
            //    prob = (prob + 10);
            //}

            //// 以下の修正は特殊効果発動確率にのみ影響
            //if (is_special)
            //{
            //    wclass = WeaponClass(w);

            //    // 封印攻撃は弱点、有効を持つユニット以外には効かない
            //    string argstring2 = "封";
            //    if (GeneralLib.InStrNotNest(WeaponClass(), argstring2) > 0)
            //    {
            //        buf = t.strWeakness + t.strEffective;
            //        var loopTo = Strings.Len(buf);
            //        for (i = 1; i <= loopTo; i++)
            //        {
            //            // 属性をひとまとめずつ取得
            //            c = GeneralLib.GetClassBundle(buf, i);
            //            if (c != "物" & c != "魔")
            //            {
            //                if (GeneralLib.InStrNotNest(WeaponClass(), c) > 0)
            //                {
            //                    break;
            //                }
            //            }
            //        }

            //        if (i > Strings.Len(buf))
            //        {
            //            CriticalProbabilityRet = 0;
            //            return CriticalProbabilityRet;
            //        }
            //    }

            //    // 限定攻撃は弱点、有効を持つユニット以外には効かない
            //    string argstring21 = "限";
            //    idx = GeneralLib.InStrNotNest(WeaponClass(), argstring21);
            //    if (idx > 0)
            //    {
            //        buf = t.strWeakness + t.strEffective;
            //        var loopTo1 = Strings.Len(buf);
            //        for (i = 1; i <= loopTo1; i++)
            //        {
            //            // 属性をひとまとめずつ取得
            //            c = GeneralLib.GetClassBundle(buf, i);
            //            if (c != "物" & c != "魔")
            //            {
            //                if (GeneralLib.InStrNotNest(WeaponClass(), c) > idx)
            //                {
            //                    break;
            //                }
            //            }
            //        }

            //        if (i > Strings.Len(buf))
            //        {
            //            CriticalProbabilityRet = 0;
            //            return CriticalProbabilityRet;
            //        }
            //    }

            //    // 特定レベル限定攻撃
            //    string argstring22 = "対";
            //    if (GeneralLib.InStrNotNest(WeaponClass(), argstring22) > 0)
            //    {
            //        // UPGRADE_WARNING: Mod に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            //        string argattr1 = "対";
            //        if (t.MainPilot().Level % WeaponLevel(argattr1) != 0d)
            //        {
            //            CriticalProbabilityRet = 0;
            //            return CriticalProbabilityRet;
            //        }
            //    }

            //    // クリティカル率については、
            //    // 弱、効属性の指定属性に対しての防御特性を考慮する。
            //    buf = "";
            //    string argstring23 = "弱";
            //    i = GeneralLib.InStrNotNest(WeaponClass(), argstring23);
            //    while (i > 0)
            //    {
            //        buf = buf + Strings.Mid(GeneralLib.GetClassBundle(WeaponClass(), i), 2);
            //        string argstring24 = "弱";
            //        i = GeneralLib.InStrNotNest(WeaponClass(), argstring24, (i + 1));
            //    }

            //    string argstring25 = "効";
            //    i = GeneralLib.InStrNotNest(WeaponClass(), argstring25);
            //    while (i > 0)
            //    {
            //        buf = buf + Strings.Mid(GeneralLib.GetClassBundle(WeaponClass(), i), 2);
            //        string argstring26 = "効";
            //        i = GeneralLib.InStrNotNest(WeaponClass(), argstring26, (i + 1));
            //    }

            //    buf = buf + wclass;

            //    // 弱点
            //    // 変化なし
            //    // 封印技
            //    string argstring27 = "封";
            //    // 限定技
            //    string argstring28 = "限";
            //    if (t.Weakness(buf))
            //    {
            //        prob = (prob + 10);
            //    }
            //    // 有効
            //    else if (t.Effective(buf))
            //    {
            //    }
            //    else if (GeneralLib.InStrNotNest(WeaponClass(), argstring27) > 0)
            //    {
            //        CriticalProbabilityRet = 0;
            //        return CriticalProbabilityRet;
            //    }
            //    else if (GeneralLib.InStrNotNest(WeaponClass(), argstring28) > 0)
            //    {
            //        CriticalProbabilityRet = 0;
            //        return CriticalProbabilityRet;
            //    }
            //    // 吸収
            //    else if (t.Absorb(buf))
            //    {
            //        CriticalProbabilityRet = 0;
            //        return CriticalProbabilityRet;
            //    }
            //    // 無効化
            //    else if (t.Immune(buf))
            //    {
            //        CriticalProbabilityRet = 0;
            //        return CriticalProbabilityRet;
            //    }
            //    // 耐性
            //    else if (t.Resist(buf))
            //    {
            //        prob = (prob / 2);
            //    }

            //    // 盲目状態には視覚攻撃は効かない
            //    string argstring29 = "視";
            //    if (GeneralLib.InStrNotNest(WeaponClass(), argstring29) > 0)
            //    {
            //        object argIndex22 = "盲目";
            //        if (t.IsConditionSatisfied(argIndex22))
            //        {
            //            CriticalProbabilityRet = 0;
            //            return CriticalProbabilityRet;
            //        }
            //    }

            //    // 機械には精神攻撃は効かない
            //    string argstring210 = "精";
            //    if (GeneralLib.InStrNotNest(WeaponClass(), argstring210) > 0)
            //    {
            //        if (t.MainPilot().Personality == "機械")
            //        {
            //            CriticalProbabilityRet = 0;
            //            return CriticalProbabilityRet;
            //        }
            //    }

            //    // 性別限定武器
            //    string argstring211 = "♂";
            //    if (GeneralLib.InStrNotNest(WeaponClass(), argstring211) > 0)
            //    {
            //        if (t.MainPilot().Sex != "男性")
            //        {
            //            CriticalProbabilityRet = 0;
            //            return CriticalProbabilityRet;
            //        }
            //    }

            //    string argstring212 = "♀";
            //    if (GeneralLib.InStrNotNest(WeaponClass(), argstring212) > 0)
            //    {
            //        if (t.MainPilot().Sex != "女性")
            //        {
            //            CriticalProbabilityRet = 0;
            //            return CriticalProbabilityRet;
            //        }
            //    }
            //}

            //// 防御時はクリティカル発生確率が半減
            //if (def_mode == "防御")
            //{
            //    prob = (prob / 2);
            //}

            //// 最終クリティカル/特殊効果を定義する。これがないときは何もしない
            //if (IsNormalWeapon(w))
            //{
            //    // クリティカル
            //    object argIndex24 = "最終クリティカル発生率";
            //    if (SRC.BCList.IsDefined(argIndex24))
            //    {
            //        // 事前にデータを登録
            //        BCVariable.DataReset();
            //        BCVariable.MeUnit = this;
            //        BCVariable.AtkUnit = this;
            //        BCVariable.DefUnit = t;
            //        BCVariable.WeaponNumber = w;
            //        BCVariable.LastVariable = prob;
            //        string argIndex23 = "最終クリティカル発生率";
            //        prob = SRC.BCList.Item(argIndex23).Calculate();
            //    }
            //}
            //else
            //{
            //    // 特殊効果
            //    object argIndex26 = "最終特殊効果発生率";
            //    if (SRC.BCList.IsDefined(argIndex26))
            //    {
            //        // 事前にデータを登録
            //        BCVariable.DataReset();
            //        BCVariable.MeUnit = this;
            //        BCVariable.AtkUnit = this;
            //        BCVariable.DefUnit = t;
            //        BCVariable.WeaponNumber = w;
            //        BCVariable.LastVariable = prob;
            //        string argIndex25 = "最終特殊効果発生率";
            //        prob = SRC.BCList.Item(argIndex25).Calculate();
            //    }
            //}

            //if (prob > 100)
            //{
            //    CriticalProbabilityRet = 100;
            //}
            //else if (prob < 1)
            //{
            //    CriticalProbabilityRet = 1;
            //}
            //else
            //{
            //    CriticalProbabilityRet = prob;
            //}

            //return CriticalProbabilityRet;
        }

        // 武器wでユニットtに攻撃をかけた時のダメージの期待値
        public int ExpDamage(Unit t, bool is_true_value, double dmg_mod = 0d)
        {
            int ExpDamageRet = 0;
            int dmg;
            int j, i, idx;
            double slevel;
            string wclass;
            string fname, fdata;
            double flevel;
            int ecost, nmorale;
            bool neautralize;
            double lv_mod;
            string opt;
            wclass = WeaponClass();

            // 攻撃力が0であれば常にダメージ0
            string argtarea = "";
            if (WeaponPower(argtarea) <= 0)
            {
                return ExpDamageRet;
            }

            // ダメージ
            dmg = Damage(t, is_true_value);

            // ダメージに修正を加える場合
            if (dmg_mod > 0d)
            {
                if (GeneralLib.InStrNotNest(WeaponClass(), "殺") == 0)
                {
                    dmg = (int)(dmg * dmg_mod);
                }
            }

            // 抹殺攻撃は一撃で相手を倒せない限り効果がない
            if (GeneralLib.InStrNotNest(WeaponClass(), "殺") > 0)
            {
                if (t.HP > dmg)
                {
                    return ExpDamageRet;
                }
            }

            // ダメージが与えられない場合
            if (dmg <= 0)
            {
                // 地形適応や封印武器、限定武器、性別限定武器、無効化、吸収が原因であれば期待値は0
                if (WeaponAdaption(t.Area) == 0d
                    || GeneralLib.InStrNotNest(WeaponClass(), "封") > 0
                    || GeneralLib.InStrNotNest(WeaponClass(), "限") > 0
                    || GeneralLib.InStrNotNest(WeaponClass(), "♂") > 0
                    || GeneralLib.InStrNotNest(WeaponClass(), "♀") > 0
                    || t.Immune(wclass) | t.Absorb(wclass))
                {
                    return ExpDamageRet;
                }

                // それ以外の要因であればダミーでダメージwとする。
                // こうしておかないと敵が攻撃が無駄の場合はまったく自分から
                // 攻撃しなくなってしまうので。
                // 単純にExpDamage=1などとしないのは攻撃力の高い武器を優先させて使わせるため
                ExpDamageRet = WeaponNo();
                return ExpDamageRet;
            }

            return dmg;

            //TODO Impl
            //// バリア無効化
            //string argstring27 = "無";
            //string argsptype = "防御能力無効化";
            //if (GeneralLib.InStrNotNest(WeaponClass(), argstring27) > 0 | IsUnderSpecialPowerEffect(argsptype))
            //{
            //    // 抹殺攻撃は一撃で相手を倒せない限り効果がない
            //    string argstring26 = "殺";
            //    if (GeneralLib.InStrNotNest(WeaponClass(), argstring26) > 0)
            //    {
            //        if (t.HP > dmg)
            //        {
            //            return ExpDamageRet;
            //        }
            //    }

            //    ExpDamageRet = dmg;
            //    return ExpDamageRet;
            //}

            //// 技量の低い敵はバリアを考慮せず攻撃をかける
            //{
            //    var withBlock = MainPilot();
            //    if (!is_true_value & withBlock.TacticalTechnique() < 150)
            //    {
            //        // 抹殺攻撃は一撃で相手を倒せない限り効果がない
            //        if (GeneralLib.InStrNotNest(WeaponClass(), "殺") > 0)
            //        {
            //            if (t.HP > dmg)
            //            {
            //                return ExpDamageRet;
            //            }
            //        }

            //        ExpDamageRet = dmg;
            //        return ExpDamageRet;
            //    }
            //}
            //// バリア能力
            //var loopTo = t.CountFeature();
            //for (i = 1; i <= loopTo; i++)
            //{
            //    object argIndex8 = i;
            //    if (t.Feature(argIndex8) == "バリア")
            //    {
            //        object argIndex1 = i;
            //        fname = t.FeatureName0(argIndex1);
            //        object argIndex2 = i;
            //        fdata = t.FeatureData(argIndex2);
            //        object argIndex3 = i;
            //        flevel = t.FeatureLevel(argIndex3);

            //        // 必要条件
            //        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 3)))
            //        {
            //            ecost = Conversions.Toint(GeneralLib.LIndex(fdata, 3));
            //        }
            //        else
            //        {
            //            ecost = 10;
            //        }

            //        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 4)))
            //        {
            //            nmorale = Conversions.Toint(GeneralLib.LIndex(fdata, 4));
            //        }
            //        else
            //        {
            //            nmorale = 0;
            //        }

            //        // オプション
            //        neautralize = false;
            //        slevel = 0d;
            //        var loopTo1 = GeneralLib.LLength(fdata);
            //        for (j = 5; j <= loopTo1; j++)
            //        {
            //            opt = GeneralLib.LIndex(fdata, j);
            //            idx = Strings.InStr(opt, "*");
            //            if (idx > 0)
            //            {
            //                string argexpr = Strings.Mid(opt, idx + 1);
            //                lv_mod = GeneralLib.StrToDbl(argexpr);
            //                opt = Strings.Left(opt, idx - 1);
            //            }
            //            else
            //            {
            //                lv_mod = -1;
            //            }

            //            switch (opt ?? "")
            //            {
            //                case "相殺":
            //                    {
            //                        object argIndex4 = "バリア";
            //                        string argfdata2 = FeatureData(argIndex4);
            //                        if (IsSameCategory(fdata, argfdata2) & Math.Abs((x - t.x)) + Math.Abs((y - t.y)) == 1)
            //                        {
            //                            neautralize = true;
            //                        }

            //                        break;
            //                    }

            //                case "中和":
            //                    {
            //                        object argIndex6 = "バリア";
            //                        string argfdata21 = FeatureData(argIndex6);
            //                        if (IsSameCategory(fdata, argfdata21) & Math.Abs((x - t.x)) + Math.Abs((y - t.y)) == 1)
            //                        {
            //                            object argIndex5 = "バリア";
            //                            flevel = flevel - FeatureLevel(argIndex5);
            //                            if (flevel <= 0d)
            //                            {
            //                                neautralize = true;
            //                            }
            //                        }

            //                        break;
            //                    }

            //                case "近接無効":
            //                    {
            //                        string argstring29 = "武";
            //                        string argstring210 = "突";
            //                        string argstring211 = "接";
            //                        if (GeneralLib.InStrNotNest(WeaponClass(), argstring29) > 0 | GeneralLib.InStrNotNest(WeaponClass(), argstring210) > 0 | GeneralLib.InStrNotNest(WeaponClass(), argstring211) > 0)
            //                        {
            //                            neautralize = true;
            //                        }

            //                        break;
            //                    }

            //                case "手動":
            //                    {
            //                        neautralize = true;
            //                        break;
            //                    }

            //                case "能力必要":
            //                    {
            //                        break;
            //                    }
            //                // スキップ
            //                case "同調率":
            //                    {
            //                        if (lv_mod == -1)
            //                        {
            //                            lv_mod = 20d;
            //                        }

            //                        slevel = lv_mod * (t.SyncLevel() - 30d);
            //                        if (Strings.InStr(fdata, "能力必要") > 0)
            //                        {
            //                            if (slevel == -30 * lv_mod)
            //                            {
            //                                neautralize = true;
            //                            }
            //                        }
            //                        else if (slevel == -30 * lv_mod)
            //                        {
            //                            slevel = 0d;
            //                        }

            //                        break;
            //                    }

            //                case "霊力":
            //                    {
            //                        if (lv_mod == -1)
            //                        {
            //                            lv_mod = 10d;
            //                        }

            //                        slevel = lv_mod * t.PlanaLevel();
            //                        if (Strings.InStr(fdata, "能力必要") > 0)
            //                        {
            //                            if (slevel == 0d)
            //                            {
            //                                neautralize = true;
            //                            }
            //                        }

            //                        break;
            //                    }

            //                case "オーラ":
            //                    {
            //                        if (lv_mod == -1)
            //                        {
            //                            lv_mod = 200d;
            //                        }

            //                        slevel = lv_mod * t.AuraLevel();
            //                        if (Strings.InStr(fdata, "能力必要") > 0)
            //                        {
            //                            if (slevel == 0d)
            //                            {
            //                                neautralize = true;
            //                            }
            //                        }

            //                        break;
            //                    }

            //                case "超能力":
            //                    {
            //                        if (lv_mod == -1)
            //                        {
            //                            lv_mod = 200d;
            //                        }

            //                        slevel = lv_mod * t.PsychicLevel();
            //                        if (Strings.InStr(fdata, "能力必要") > 0)
            //                        {
            //                            if (slevel == 0d)
            //                            {
            //                                neautralize = true;
            //                            }
            //                        }

            //                        break;
            //                    }

            //                default:
            //                    {
            //                        if (lv_mod == -1)
            //                        {
            //                            lv_mod = 200d;
            //                        }

            //                        slevel = lv_mod * t.SkillLevel(opt);
            //                        if (Strings.InStr(fdata, "能力必要") > 0)
            //                        {
            //                            if (slevel == 0d)
            //                            {
            //                                neautralize = true;
            //                            }
            //                        }

            //                        break;
            //                    }
            //            }
            //        }

            //        // バリア無効化で無効化されている？
            //        object argIndex7 = "バリア無効化";
            //        if (t.IsConditionSatisfied(argIndex7))
            //        {
            //            if (Strings.InStr(fdata, "バリア無効化無効") == 0)
            //            {
            //                neautralize = true;
            //            }
            //        }

            //        // 発動可能？
            //        bool localIsAttributeClassified() { string argaclass1 = GeneralLib.LIndex(fdata, 2); var ret = t.IsAttributeClassified(argaclass1, wclass); return ret; }

            //        if (t.EN >= ecost & t.MainPilot().Morale >= nmorale & localIsAttributeClassified() & !neautralize)
            //        {
            //            // バリア発動
            //            if (dmg <= 1000d * flevel + slevel)
            //            {
            //                ExpDamageRet = w;
            //                return ExpDamageRet;
            //            }
            //        }
            //    }
            //}

            //// フィールド能力
            //var loopTo2 = t.CountFeature();
            //for (i = 1; i <= loopTo2; i++)
            //{
            //    object argIndex16 = i;
            //    if (t.Feature(argIndex16) == "フィールド")
            //    {
            //        object argIndex9 = i;
            //        fname = t.FeatureName0(argIndex9);
            //        object argIndex10 = i;
            //        fdata = t.FeatureData(argIndex10);
            //        object argIndex11 = i;
            //        flevel = t.FeatureLevel(argIndex11);

            //        // 必要条件
            //        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 3)))
            //        {
            //            ecost = Conversions.Toint(GeneralLib.LIndex(fdata, 3));
            //        }
            //        else
            //        {
            //            ecost = 0;
            //        }

            //        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 4)))
            //        {
            //            nmorale = Conversions.Toint(GeneralLib.LIndex(fdata, 4));
            //        }
            //        else
            //        {
            //            nmorale = 0;
            //        }

            //        // オプション
            //        neautralize = false;
            //        slevel = 0d;
            //        var loopTo3 = GeneralLib.LLength(fdata);
            //        for (j = 5; j <= loopTo3; j++)
            //        {
            //            opt = GeneralLib.LIndex(fdata, j);
            //            idx = Strings.InStr(opt, "*");
            //            if (idx > 0)
            //            {
            //                string argexpr1 = Strings.Mid(opt, idx + 1);
            //                lv_mod = GeneralLib.StrToDbl(argexpr1);
            //                opt = Strings.Left(opt, idx - 1);
            //            }
            //            else
            //            {
            //                lv_mod = -1;
            //            }

            //            switch (opt ?? "")
            //            {
            //                case "相殺":
            //                    {
            //                        object argIndex12 = "フィールド";
            //                        string argfdata22 = FeatureData(argIndex12);
            //                        if (IsSameCategory(fdata, argfdata22) & Math.Abs((x - t.x)) + Math.Abs((y - t.y)) == 1)
            //                        {
            //                            neautralize = true;
            //                        }

            //                        break;
            //                    }

            //                case "中和":
            //                    {
            //                        object argIndex14 = "フィールド";
            //                        string argfdata23 = FeatureData(argIndex14);
            //                        if (IsSameCategory(fdata, argfdata23) & Math.Abs((x - t.x)) + Math.Abs((y - t.y)) == 1)
            //                        {
            //                            object argIndex13 = "フィールド";
            //                            flevel = flevel - FeatureLevel(argIndex13);
            //                            if (flevel <= 0d)
            //                            {
            //                                neautralize = true;
            //                            }
            //                        }

            //                        break;
            //                    }

            //                case "近接無効":
            //                    {
            //                        string argstring212 = "武";
            //                        string argstring213 = "突";
            //                        string argstring214 = "接";
            //                        if (GeneralLib.InStrNotNest(WeaponClass(), argstring212) > 0 | GeneralLib.InStrNotNest(WeaponClass(), argstring213) > 0 | GeneralLib.InStrNotNest(WeaponClass(), argstring214) > 0)
            //                        {
            //                            neautralize = true;
            //                        }

            //                        break;
            //                    }

            //                case "手動":
            //                    {
            //                        neautralize = true;
            //                        break;
            //                    }

            //                case "能力必要":
            //                    {
            //                        break;
            //                    }
            //                // スキップ
            //                case "同調率":
            //                    {
            //                        if (lv_mod == -1)
            //                        {
            //                            lv_mod = 20d;
            //                        }

            //                        slevel = lv_mod * (t.SyncLevel() - 30d);
            //                        if (Strings.InStr(fdata, "能力必要") > 0)
            //                        {
            //                            if (slevel == -30 * lv_mod)
            //                            {
            //                                neautralize = true;
            //                            }
            //                        }
            //                        else if (slevel == -30 * lv_mod)
            //                        {
            //                            slevel = 0d;
            //                        }

            //                        break;
            //                    }

            //                case "霊力":
            //                    {
            //                        if (lv_mod == -1)
            //                        {
            //                            lv_mod = 10d;
            //                        }

            //                        slevel = lv_mod * t.PlanaLevel();
            //                        if (Strings.InStr(fdata, "能力必要") > 0)
            //                        {
            //                            if (slevel == 0d)
            //                            {
            //                                neautralize = true;
            //                            }
            //                        }

            //                        break;
            //                    }

            //                case "オーラ":
            //                    {
            //                        if (lv_mod == -1)
            //                        {
            //                            lv_mod = 200d;
            //                        }

            //                        slevel = lv_mod * t.AuraLevel();
            //                        if (Strings.InStr(fdata, "能力必要") > 0)
            //                        {
            //                            if (slevel == 0d)
            //                            {
            //                                neautralize = true;
            //                            }
            //                        }

            //                        break;
            //                    }

            //                case "超能力":
            //                    {
            //                        if (lv_mod == -1)
            //                        {
            //                            lv_mod = 200d;
            //                        }

            //                        slevel = lv_mod * t.PsychicLevel();
            //                        if (Strings.InStr(fdata, "能力必要") > 0)
            //                        {
            //                            if (slevel == 0d)
            //                            {
            //                                neautralize = true;
            //                            }
            //                        }

            //                        break;
            //                    }

            //                default:
            //                    {
            //                        if (lv_mod == -1)
            //                        {
            //                            lv_mod = 200d;
            //                        }

            //                        slevel = lv_mod * t.SkillLevel(opt);
            //                        if (Strings.InStr(fdata, "能力必要") > 0)
            //                        {
            //                            if (slevel == 0d)
            //                            {
            //                                neautralize = true;
            //                            }
            //                        }

            //                        break;
            //                    }
            //            }
            //        }

            //        // バリア無効化で無効化されている？
            //        object argIndex15 = "バリア無効化";
            //        if (t.IsConditionSatisfied(argIndex15))
            //        {
            //            if (Strings.InStr(fdata, "バリア無効化無効") == 0)
            //            {
            //                neautralize = true;
            //            }
            //        }

            //        // 発動可能？
            //        bool localIsAttributeClassified1() { string argaclass1 = GeneralLib.LIndex(fdata, 2); var ret = t.IsAttributeClassified(argaclass1, wclass); return ret; }

            //        if (t.EN >= ecost & t.MainPilot().Morale >= nmorale & localIsAttributeClassified1() & !neautralize)
            //        {
            //            // フィールド発動
            //            if (dmg <= 500d * flevel + slevel)
            //            {
            //                ExpDamageRet = w;
            //                return ExpDamageRet;
            //            }
            //            else if (flevel > 0d | slevel > 0d)
            //            {
            //                dmg = (dmg - 500d * flevel - slevel);
            //            }
            //        }
            //    }
            //}

            //// プロテクション能力
            //var loopTo4 = t.CountFeature();
            //for (i = 1; i <= loopTo4; i++)
            //{
            //    object argIndex24 = i;
            //    if (t.Feature(argIndex24) == "プロテクション")
            //    {
            //        object argIndex17 = i;
            //        fname = t.FeatureName0(argIndex17);
            //        object argIndex18 = i;
            //        fdata = t.FeatureData(argIndex18);
            //        object argIndex19 = i;
            //        flevel = t.FeatureLevel(argIndex19);

            //        // 必要条件
            //        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 3)))
            //        {
            //            ecost = Conversions.Toint(GeneralLib.LIndex(fdata, 3));
            //        }
            //        else
            //        {
            //            ecost = 10;
            //        }

            //        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 4)))
            //        {
            //            nmorale = Conversions.Toint(GeneralLib.LIndex(fdata, 4));
            //        }
            //        else
            //        {
            //            nmorale = 0;
            //        }

            //        // オプション
            //        neautralize = false;
            //        slevel = 0d;
            //        var loopTo5 = GeneralLib.LLength(fdata);
            //        for (j = 5; j <= loopTo5; j++)
            //        {
            //            opt = GeneralLib.LIndex(fdata, j);
            //            idx = Strings.InStr(opt, "*");
            //            if (idx > 0)
            //            {
            //                string argexpr2 = Strings.Mid(opt, idx + 1);
            //                lv_mod = GeneralLib.StrToDbl(argexpr2);
            //                opt = Strings.Left(opt, idx - 1);
            //            }
            //            else
            //            {
            //                lv_mod = -1;
            //            }

            //            switch (opt ?? "")
            //            {
            //                case "相殺":
            //                    {
            //                        object argIndex20 = "プロテクション";
            //                        string argfdata24 = FeatureData(argIndex20);
            //                        if (IsSameCategory(fdata, argfdata24) & Math.Abs((x - t.x)) + Math.Abs((y - t.y)) == 1)
            //                        {
            //                            neautralize = true;
            //                        }

            //                        break;
            //                    }

            //                case "中和":
            //                    {
            //                        object argIndex22 = "プロテクション";
            //                        string argfdata25 = FeatureData(argIndex22);
            //                        if (IsSameCategory(fdata, argfdata25) & Math.Abs((x - t.x)) + Math.Abs((y - t.y)) == 1)
            //                        {
            //                            object argIndex21 = "プロテクション";
            //                            flevel = flevel - FeatureLevel(argIndex21);
            //                            if (flevel <= 0d)
            //                            {
            //                                neautralize = true;
            //                            }
            //                        }

            //                        break;
            //                    }

            //                case "近接無効":
            //                    {
            //                        string argstring215 = "武";
            //                        string argstring216 = "突";
            //                        string argstring217 = "接";
            //                        if (GeneralLib.InStrNotNest(WeaponClass(), argstring215) > 0 | GeneralLib.InStrNotNest(WeaponClass(), argstring216) > 0 | GeneralLib.InStrNotNest(WeaponClass(), argstring217) > 0)
            //                        {
            //                            neautralize = true;
            //                        }

            //                        break;
            //                    }

            //                case "手動":
            //                    {
            //                        neautralize = true;
            //                        break;
            //                    }

            //                case "能力必要":
            //                    {
            //                        break;
            //                    }
            //                // スキップ
            //                case "同調率":
            //                    {
            //                        if (lv_mod == -1)
            //                        {
            //                            lv_mod = 0.5d;
            //                        }

            //                        slevel = lv_mod * (t.SyncLevel() - 30d);
            //                        if (Strings.InStr(fdata, "能力必要") > 0)
            //                        {
            //                            if (slevel == -30 * lv_mod)
            //                            {
            //                                neautralize = true;
            //                            }
            //                        }
            //                        else if (slevel == -30 * lv_mod)
            //                        {
            //                            slevel = 0d;
            //                        }

            //                        break;
            //                    }

            //                case "霊力":
            //                    {
            //                        if (lv_mod == -1)
            //                        {
            //                            lv_mod = 0.2d;
            //                        }

            //                        slevel = lv_mod * t.PlanaLevel();
            //                        if (Strings.InStr(fdata, "能力必要") > 0)
            //                        {
            //                            if (slevel == 0d)
            //                            {
            //                                neautralize = true;
            //                            }
            //                        }

            //                        break;
            //                    }

            //                case "オーラ":
            //                    {
            //                        if (lv_mod == -1)
            //                        {
            //                            lv_mod = 5d;
            //                        }

            //                        slevel = lv_mod * t.AuraLevel();
            //                        if (Strings.InStr(fdata, "能力必要") > 0)
            //                        {
            //                            if (slevel == 0d)
            //                            {
            //                                neautralize = true;
            //                            }
            //                        }

            //                        break;
            //                    }

            //                case "超能力":
            //                    {
            //                        if (lv_mod == -1)
            //                        {
            //                            lv_mod = 5d;
            //                        }

            //                        slevel = lv_mod * t.PsychicLevel();
            //                        if (Strings.InStr(fdata, "能力必要") > 0)
            //                        {
            //                            if (slevel == 0d)
            //                            {
            //                                neautralize = true;
            //                            }
            //                        }

            //                        break;
            //                    }

            //                default:
            //                    {
            //                        if (lv_mod == -1)
            //                        {
            //                            lv_mod = 5d;
            //                        }

            //                        slevel = lv_mod * t.SkillLevel(opt);
            //                        if (Strings.InStr(fdata, "能力必要") > 0)
            //                        {
            //                            if (slevel == 0d)
            //                            {
            //                                neautralize = true;
            //                            }
            //                        }

            //                        break;
            //                    }
            //            }
            //        }

            //        // バリア無効化で無効化されている？
            //        object argIndex23 = "バリア無効化";
            //        if (t.IsConditionSatisfied(argIndex23))
            //        {
            //            if (Strings.InStr(fdata, "バリア無効化無効") == 0)
            //            {
            //                neautralize = true;
            //            }
            //        }

            //        // 発動可能？
            //        bool localIsAttributeClassified2() { string argaclass1 = GeneralLib.LIndex(fdata, 2); var ret = t.IsAttributeClassified(argaclass1, wclass); return ret; }

            //        if (t.EN >= ecost & t.MainPilot().Morale >= nmorale & localIsAttributeClassified2() & !neautralize)
            //        {
            //            // プロテクション発動
            //            dmg = ((long)(dmg * (100d - 10d * flevel - slevel)) / 100L);
            //            if (dmg <= 0)
            //            {
            //                ExpDamageRet = w;
            //                return ExpDamageRet;
            //            }
            //        }
            //    }
            //}

            //// 対ビーム用防御能力
            //string argstring218 = "Ｂ";
            //if (GeneralLib.InStrNotNest(WeaponClass(), argstring218) > 0)
            //{
            //    // ビーム吸収
            //    string argfname = "ビーム吸収";
            //    if (t.IsFeatureAvailable(argfname))
            //    {
            //        ExpDamageRet = w;
            //        return ExpDamageRet;
            //    }
            //}

            //// 抹殺攻撃は一撃で相手を倒せる場合にのみ有効
            //string argstring219 = "殺";
            //if (GeneralLib.InStrNotNest(WeaponClass(), argstring219) > 0)
            //{
            //    if (dmg < t.HP)
            //    {
            //        dmg = 0;
            //    }
            //}

            //// 盾防御
            //string argfname1 = "盾";
            //string argsname = "Ｓ防御";
            //string argattr1 = "精";
            //string argattr2 = "浸";
            //string argattr3 = "殺";
            //object argIndex27 = "盾付加";
            //object argIndex28 = "盾";
            //object argIndex29 = "盾ダメージ";
            //if (t.IsFeatureAvailable(argfname1) & t.MainPilot().IsSkillAvailable(argsname) & t.MaxAction() > 0 & !IsWeaponClassifiedAs(argattr1) & !IsWeaponClassifiedAs(argattr2) & !IsWeaponClassifiedAs(argattr3) & (t.IsConditionSatisfied(argIndex27) | t.FeatureLevel(argIndex28) > t.ConditionLevel(argIndex29)))
            //{
            //    string argattr = "破";
            //    if (IsWeaponClassifiedAs(argattr))
            //    {
            //        object argIndex25 = "Ｓ防御";
            //        string argref_mode = "";
            //        dmg = (dmg - 50d * (t.MainPilot().SkillLevel(argIndex25, ref_mode: argref_mode) + 4d));
            //    }
            //    else
            //    {
            //        object argIndex26 = "Ｓ防御";
            //        string argref_mode1 = "";
            //        dmg = (dmg - 100d * (t.MainPilot().SkillLevel(argIndex26, ref_mode: argref_mode1) + 4d));
            //    }
            //}

            //// ダメージが減少されて0以下になった場合もダミーで1ダメージ
            //if (dmg <= 0)
            //{
            //    dmg = 1;
            //}

            //// 抹殺攻撃は一撃で相手を倒せない限り効果がない
            //if (Strings.InStr(w.ToString(), "殺") > 0)
            //{
            //    if (t.HP > dmg)
            //    {
            //        return ExpDamageRet;
            //    }
            //}

            //ExpDamageRet = dmg;
            //return ExpDamageRet;
        }

        // 弾数
        public int Bullet()
        {
            // TODO Impl
            return 2;
            //int BulletRet = default;
            //BulletRet = (int)(dblBullet[w] * intMaxBullet[w]);
            //return BulletRet;
        }

        // 最大弾数
        public int MaxBullet()
        {
            // TODO Impl
            return 2;
            //int MaxBulletRet = default;
            //MaxBulletRet = intMaxBullet[w];
            //return MaxBulletRet;
        }

        // 弾数を設定
        public void SetBullet(int new_bullet)
        {
            // TODO Impl
            //if (new_bullet < 0)
            //{
            //    dblBullet[w] = 0d;
            //}
            //else if (intMaxBullet[w] > 0)
            //{
            //    dblBullet[w] = new_bullet / (double)intMaxBullet[w];
            //}
            //else
            //{
            //    dblBullet[w] = 1d;
            //}
        }

        // 合体技のパートナーを探す
        public void CombinationPartner(string ctype_Renamed, Unit[] partners, int tx = 0, int ty = 0, bool check_formation = false)
        {
            throw new NotImplementedException();
            //Unit u;
            //string uname;
            //int j, i, k;
            //int clevel = default, cnum = default;
            //var clist = default(string);
            //string cname;
            //int cmorale, cen, cplana = default;
            //int crange, loop_limit;

            //// 正常な判断が可能？
            //string argIndex1 = "混乱";
            //if (IsConditionSatisfied(argIndex1))
            //{
            //    partners = new Unit[1];
            //    return;
            //}

            //// 合体技のデータを調べておく
            //if (ctype_Renamed == "武装")
            //{
            //    cname = Weapon(w).Name;
            //    cen = WeaponENConsumption(w);
            //    cmorale = Weapon(w).NecessaryMorale;
            //    string argattr2 = "霊";
            //    string argattr3 = "プ";
            //    if (IsWeaponClassifiedAs(w, argattr2))
            //    {
            //        string argattr = "霊";
            //        cplana = (5d * WeaponLevel(w, argattr));
            //    }
            //    else if (IsWeaponClassifiedAs(w, argattr3))
            //    {
            //        string argattr1 = "プ";
            //        cplana = (5d * WeaponLevel(w, argattr1));
            //    }

            //    crange = WeaponMaxRange(w);
            //}
            //else
            //{
            //    cname = Ability(w).Name;
            //    cen = AbilityENConsumption(w);
            //    cmorale = Ability(w).NecessaryMorale;
            //    string argattr6 = "霊";
            //    string argattr7 = "プ";
            //    if (IsAbilityClassifiedAs(w, argattr6))
            //    {
            //        string argattr4 = "霊";
            //        cplana = (5d * AbilityLevel(w, argattr4));
            //    }
            //    else if (IsAbilityClassifiedAs(w, argattr7))
            //    {
            //        string argattr5 = "プ";
            //        cplana = (5d * AbilityLevel(w, argattr5));
            //    }

            //    crange = AbilityMaxRange(w);
            //}

            //// ユニットの特殊能力「合体技」の検索
            //var loopTo = CountFeature();
            //for (i = 1; i <= loopTo; i++)
            //{
            //    string argIndex4 = i;
            //    if (Feature(argIndex4) == "合体技")
            //    {
            //        string localFeatureData1() { string argIndex1 = i; var ret = FeatureData(argIndex1); return ret; }

            //        string arglist1 = localFeatureData1();
            //        if ((GeneralLib.LIndex(arglist1, 1) ?? "") == (cname ?? ""))
            //        {
            //            string argIndex3 = i;
            //            if (IsFeatureLevelSpecified(argIndex3))
            //            {
            //                string argIndex2 = i;
            //                clevel = FeatureLevel(argIndex2);
            //            }
            //            else
            //            {
            //                clevel = 0;
            //            }

            //            string localFeatureData() { string argIndex1 = i; var ret = FeatureData(argIndex1); return ret; }

            //            string arglist = localFeatureData();
            //            clist = GeneralLib.ListTail(arglist, 2);
            //            cnum = GeneralLib.LLength(clist);
            //            break;
            //        }
            //    }
            //}

            //if (i > CountFeature())
            //{
            //    partners = new Unit[1];
            //    return;
            //}

            //// 出撃していない場合
            //if (Status != "出撃" | string.IsNullOrEmpty(Map.MapFileName))
            //{
            //    // パートナーが仲間にいるだけでよい
            //    var loopTo1 = cnum;
            //    for (i = 1; i <= loopTo1; i++)
            //    {
            //        uname = GeneralLib.LIndex(clist, i);

            //        // パートナーがユニット名で指定されている場合
            //        string argIndex6 = uname;
            //        if (SRC.UList.IsDefined(argIndex6))
            //        {
            //            string argIndex5 = uname;
            //            {
            //                var withBlock = SRC.UList.Item(argIndex5);
            //                if (withBlock.Status == "出撃" | withBlock.Status == "待機")
            //                {
            //                    goto NextPartner;
            //                }
            //            }
            //        }

            //        // パートナーがパイロット名で指定されている場合
            //        string argIndex7 = uname;
            //        if (SRC.PList.IsDefined(argIndex7))
            //        {
            //            Pilot localItem1() { string argIndex1 = uname; var ret = SRC.PList.Item(argIndex1); return ret; }

            //            Pilot localItem2() { string argIndex1 = uname; var ret = SRC.PList.Item(argIndex1); return ret; }

            //            if (localItem2().Unit_Renamed is object)
            //            {
            //                Pilot localItem() { string argIndex1 = uname; var ret = SRC.PList.Item(argIndex1); return ret; }

            //                {
            //                    var withBlock1 = localItem().Unit_Renamed;
            //                    if (withBlock1.Status == "出撃" | withBlock1.Status == "待機")
            //                    {
            //                        goto NextPartner;
            //                    }
            //                }
            //            }
            //        }

            //        // パートナーが見つからなかった
            //        partners = new Unit[1];
            //        return;
            //    NextPartner:
            //        ;
            //    }
            //    // パートナーが全員仲間にいる
            //    partners = new Unit[(cnum + 1)];
            //    return;
            //}

            //// 合体技の基点の設定
            //if (tx == 0)
            //{
            //    tx = x;
            //}

            //if (ty == 0)
            //{
            //    ty = y;
            //}

            //// パートナーの検索範囲を設定

            //if (crange == 1)
            //{
            //    if (cnum >= 8)
            //    {
            //        // 射程１で８体合体以上の場合は２マス以内
            //        loop_limit = 12;
            //    }
            //    else if (cnum >= 4)
            //    {
            //        // 射程１で４体合体以上の場合は斜め隣接可
            //        loop_limit = 8;
            //    }
            //    else
            //    {
            //        // どれにも該当していなければ隣接のみ
            //        loop_limit = 4;
            //    }
            //}
            //else if (cnum >= 9)
            //{
            //    // 射程２以上で９体合体以上の場合は２マス以内
            //    loop_limit = 12;
            //}
            //else if (cnum >= 5)
            //{
            //    // 射程２以上で５体合体以上の場合は斜め隣接可
            //    loop_limit = 8;
            //}
            //else
            //{
            //    // どれにも該当していなければ隣接のみ
            //    loop_limit = 4;
            //}

            //// 合体技斜め隣接可オプション
            //string argoname = "合体技斜め隣接可";
            //if (Expression.IsOptionDefined(argoname))
            //{
            //    if (loop_limit == 4)
            //    {
            //        loop_limit = 8;
            //    }
            //}

            //partners = new Unit[1];
            //var loopTo2 = cnum;
            //for (i = 1; i <= loopTo2; i++)
            //{
            //    // パートナーの名称
            //    uname = GeneralLib.LIndex(clist, i);
            //    var loopTo3 = loop_limit;
            //    for (j = 1; j <= loopTo3; j++)
            //    {
            //        // パートナーの検索位置設定
            //        // UPGRADE_NOTE: オブジェクト u をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //        u = null;
            //        switch (j)
            //        {
            //            case 1:
            //                {
            //                    if (tx > 1)
            //                    {
            //                        u = Map.MapDataForUnit[tx - 1, ty];
            //                    }

            //                    break;
            //                }

            //            case 2:
            //                {
            //                    if (tx < Map.MapWidth)
            //                    {
            //                        u = Map.MapDataForUnit[tx + 1, ty];
            //                    }

            //                    break;
            //                }

            //            case 3:
            //                {
            //                    if (ty > 1)
            //                    {
            //                        u = Map.MapDataForUnit[tx, ty - 1];
            //                    }

            //                    break;
            //                }

            //            case 4:
            //                {
            //                    if (ty < Map.MapHeight)
            //                    {
            //                        u = Map.MapDataForUnit[tx, ty + 1];
            //                    }

            //                    break;
            //                }

            //            case 5:
            //                {
            //                    if (tx > 1 & ty > 1)
            //                    {
            //                        u = Map.MapDataForUnit[tx - 1, ty - 1];
            //                    }

            //                    break;
            //                }

            //            case 6:
            //                {
            //                    if (tx < Map.MapWidth & ty < Map.MapHeight)
            //                    {
            //                        u = Map.MapDataForUnit[tx + 1, ty + 1];
            //                    }

            //                    break;
            //                }

            //            case 7:
            //                {
            //                    if (tx > 1 & ty < Map.MapHeight)
            //                    {
            //                        u = Map.MapDataForUnit[tx - 1, ty + 1];
            //                    }

            //                    break;
            //                }

            //            case 8:
            //                {
            //                    if (tx < Map.MapWidth & ty > 1)
            //                    {
            //                        u = Map.MapDataForUnit[tx + 1, ty - 1];
            //                    }

            //                    break;
            //                }

            //            case 9:
            //                {
            //                    if (tx > 2)
            //                    {
            //                        u = Map.MapDataForUnit[tx - 2, ty];
            //                    }

            //                    break;
            //                }

            //            case 10:
            //                {
            //                    if (tx < Map.MapWidth - 1)
            //                    {
            //                        u = Map.MapDataForUnit[tx + 2, ty];
            //                    }

            //                    break;
            //                }

            //            case 11:
            //                {
            //                    if (ty > 2)
            //                    {
            //                        u = Map.MapDataForUnit[tx, ty - 2];
            //                    }

            //                    break;
            //                }

            //            case 12:
            //                {
            //                    if (ty < Map.MapHeight - 1)
            //                    {
            //                        u = Map.MapDataForUnit[tx, ty + 2];
            //                    }

            //                    break;
            //                }
            //        }

            //        // ユニットが存在する？
            //        if (u is null)
            //        {
            //            goto NextNeighbor;
            //        }

            //        {
            //            var withBlock2 = u;
            //            // 合体技のパートナーに該当する？
            //            if ((withBlock2.Name ?? "") != (uname ?? ""))
            //            {
            //                // パイロット名でも確認
            //                if ((withBlock2.MainPilot().Name ?? "") != (uname ?? ""))
            //                {
            //                    goto NextNeighbor;
            //                }
            //            }

            //            // ユニットが自分？
            //            if (ReferenceEquals(u, this))
            //            {
            //                goto NextNeighbor;
            //            }

            //            // 既に選択済み？
            //            var loopTo4 = Information.UBound(partners);
            //            for (k = 1; k <= loopTo4; k++)
            //            {
            //                if (ReferenceEquals(u, partners[k]))
            //                {
            //                    goto NextNeighbor;
            //                }
            //            }

            //            // ユニットが敵？
            //            if (IsEnemy(u))
            //            {
            //                goto NextNeighbor;
            //            }

            //            // 行動出来なければだめ
            //            string argIndex8 = "混乱";
            //            string argIndex9 = "恐怖";
            //            string argIndex10 = "憑依";
            //            if (withBlock2.MaxAction() == 0 | withBlock2.IsConditionSatisfied(argIndex8) | withBlock2.IsConditionSatisfied(argIndex9) | withBlock2.IsConditionSatisfied(argIndex10))
            //            {
            //                goto NextNeighbor;
            //            }

            //            // 合体技にレベルが設定されていればパイロット間の信頼度をチェック
            //            if (clevel > 0)
            //            {
            //                if (MainPilot().Relation(withBlock2.MainPilot()) < clevel | withBlock2.MainPilot().Relation(MainPilot()) < clevel)
            //                {
            //                    goto NextNeighbor;
            //                }
            //            }

            //            // パートナーが武器を使うための条件を満たしているかを判定
            //            if (!check_formation)
            //            {
            //                if (ctype_Renamed == "武装")
            //                {
            //                    // 合体技と同名の武器を検索
            //                    var loopTo5 = withBlock2.CountWeapon();
            //                    for (k = 1; k <= loopTo5; k++)
            //                    {
            //                        if ((withBlock2.Weapon(k).Name ?? "") == (cname ?? ""))
            //                        {
            //                            break;
            //                        }
            //                    }

            //                    if (k <= withBlock2.CountWeapon())
            //                    {
            //                        // 武器が使える？
            //                        if (!withBlock2.IsWeaponMastered(k))
            //                        {
            //                            goto NextNeighbor;
            //                        }

            //                        if (withBlock2.Weapon(k).NecessaryMorale > 0)
            //                        {
            //                            if (withBlock2.MainPilot().Morale < withBlock2.Weapon(k).NecessaryMorale)
            //                            {
            //                                goto NextNeighbor;
            //                            }
            //                        }

            //                        if (withBlock2.WeaponENConsumption(k) > 0)
            //                        {
            //                            if (withBlock2.EN < withBlock2.WeaponENConsumption(k))
            //                            {
            //                                goto NextNeighbor;
            //                            }
            //                        }

            //                        if (withBlock2.Weapon(k).Bullet > 0)
            //                        {
            //                            if (withBlock2.Bullet(k) == 0)
            //                            {
            //                                goto NextNeighbor;
            //                            }
            //                        }

            //                        string argattr10 = "霊";
            //                        string argattr11 = "プ";
            //                        if (withBlock2.WeaponLevel(k, argattr10) > 0d)
            //                        {
            //                            string argattr8 = "霊";
            //                            if (withBlock2.MainPilot().Plana < 5d * withBlock2.WeaponLevel(k, argattr8))
            //                            {
            //                                goto NextNeighbor;
            //                            }
            //                        }
            //                        else if (withBlock2.WeaponLevel(k, argattr11) > 0d)
            //                        {
            //                            string argattr9 = "プ";
            //                            if (withBlock2.MainPilot().Plana < 5d * withBlock2.WeaponLevel(k, argattr9))
            //                            {
            //                                goto NextNeighbor;
            //                            }
            //                        }
            //                    }
            //                    else
            //                    {
            //                        // 同名の武器を持っていなかった場合はチェック項目を限定
            //                        if (cmorale > 0)
            //                        {
            //                            if (withBlock2.MainPilot().Morale < cmorale)
            //                            {
            //                                goto NextNeighbor;
            //                            }
            //                        }

            //                        if (cen > 0)
            //                        {
            //                            if (withBlock2.EN < cen)
            //                            {
            //                                goto NextNeighbor;
            //                            }
            //                        }

            //                        if (cplana > 0)
            //                        {
            //                            if (withBlock2.MainPilot().Plana < cplana)
            //                            {
            //                                goto NextNeighbor;
            //                            }
            //                        }
            //                    }
            //                }
            //                else
            //                {
            //                    // 合体技と同名のアビリティを検索
            //                    var loopTo6 = withBlock2.CountAbility();
            //                    for (k = 1; k <= loopTo6; k++)
            //                    {
            //                        if ((withBlock2.Ability(k).Name ?? "") == (cname ?? ""))
            //                        {
            //                            break;
            //                        }
            //                    }

            //                    if (k <= withBlock2.CountAbility())
            //                    {
            //                        // アビリティが使える？
            //                        if (!withBlock2.IsAbilityMastered(k))
            //                        {
            //                            goto NextNeighbor;
            //                        }

            //                        if (withBlock2.Ability(k).NecessaryMorale > 0)
            //                        {
            //                            if (withBlock2.MainPilot().Morale < withBlock2.Ability(k).NecessaryMorale)
            //                            {
            //                                goto NextNeighbor;
            //                            }
            //                        }

            //                        if (withBlock2.AbilityENConsumption(k) > 0)
            //                        {
            //                            if (withBlock2.EN < withBlock2.AbilityENConsumption(k))
            //                            {
            //                                goto NextNeighbor;
            //                            }
            //                        }

            //                        if (withBlock2.Ability(k).Stock > 0)
            //                        {
            //                            if (withBlock2.Stock(k) == 0)
            //                            {
            //                                goto NextNeighbor;
            //                            }
            //                        }

            //                        string argattr14 = "霊";
            //                        string argattr15 = "プ";
            //                        if (withBlock2.AbilityLevel(k, argattr14) > 0d)
            //                        {
            //                            string argattr12 = "霊";
            //                            if (withBlock2.MainPilot().Plana < 5d * withBlock2.AbilityLevel(k, argattr12))
            //                            {
            //                                goto NextNeighbor;
            //                            }
            //                        }
            //                        else if (withBlock2.AbilityLevel(k, argattr15) > 0d)
            //                        {
            //                            string argattr13 = "プ";
            //                            if (withBlock2.MainPilot().Plana < 5d * withBlock2.AbilityLevel(k, argattr13))
            //                            {
            //                                goto NextNeighbor;
            //                            }
            //                        }
            //                    }
            //                    else
            //                    {
            //                        // 同名のアビリティを持っていなかった場合はチェック項目を限定
            //                        if (cmorale > 0)
            //                        {
            //                            if (withBlock2.MainPilot().Morale < cmorale)
            //                            {
            //                                goto NextNeighbor;
            //                            }
            //                        }

            //                        if (cen > 0)
            //                        {
            //                            if (withBlock2.EN < cen)
            //                            {
            //                                goto NextNeighbor;
            //                            }
            //                        }

            //                        if (cplana > 0)
            //                        {
            //                            if (withBlock2.MainPilot().Plana < cplana)
            //                            {
            //                                goto NextNeighbor;
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //            // フォーメーションのチェックだけの時も必要技能は調べておく
            //            else if (ctype_Renamed == "武装")
            //            {
            //                var loopTo7 = withBlock2.CountWeapon();
            //                for (k = 1; k <= loopTo7; k++)
            //                {
            //                    if ((withBlock2.Weapon(k).Name ?? "") == (cname ?? ""))
            //                    {
            //                        break;
            //                    }
            //                }

            //                if (k <= withBlock2.CountWeapon())
            //                {
            //                    if (!withBlock2.IsWeaponMastered(k))
            //                    {
            //                        goto NextNeighbor;
            //                    }
            //                }
            //            }
            //            else
            //            {
            //                var loopTo8 = withBlock2.CountAbility();
            //                for (k = 1; k <= loopTo8; k++)
            //                {
            //                    if ((withBlock2.Ability(k).Name ?? "") == (cname ?? ""))
            //                    {
            //                        break;
            //                    }
            //                }

            //                if (k <= withBlock2.CountAbility())
            //                {
            //                    if (!withBlock2.IsAbilityMastered(k))
            //                    {
            //                        goto NextNeighbor;
            //                    }
            //                }
            //            }
            //        }

            //        // 見つかったパートナーを記録
            //        Array.Resize(partners, i + 1);
            //        partners[i] = u;
            //        break;
            //    NextNeighbor:
            //        ;
            //    }

            //    // パートナーが見つからなかった？
            //    if (j > loop_limit)
            //    {
            //        partners = new Unit[1];
            //        return;
            //    }
            //}

            //// 合体技メッセージ判定用にパートナー一覧を記録
            //Commands.SelectedPartners = new Unit[Information.UBound(partners) + 1];
            //var loopTo9 = Information.UBound(partners);
            //for (i = 1; i <= loopTo9; i++)
            //    Commands.SelectedPartners[i] = partners[i];
        }

        // 合体技攻撃に必要なパートナーが見つかるか？
        public bool IsCombinationAttackAvailable(bool check_formation = false)
        {
            throw new NotImplementedException();
            //bool IsCombinationAttackAvailableRet = default;
            //Unit[] partners;
            //partners = new Unit[1];
            //string argattr = "Ｍ";
            //if (Status == "待機" | string.IsNullOrEmpty(Map.MapFileName))
            //{
            //    // 出撃時以外は相手が仲間にいるだけでＯＫ
            //    string argctype_Renamed = "武装";
            //    CombinationPartner(argctype_Renamed, w, partners, x, y);
            //}
            //else if (WeaponMaxRange(w) == 1 & !IsWeaponClassifiedAs(w, argattr))
            //{
            //    // 射程１の場合は自分の周りのいずれかの敵ユニットに対して合体技が使えればＯＫ
            //    if (x > 1)
            //    {
            //        if (Map.MapDataForUnit[x - 1, y] is object)
            //        {
            //            if (IsEnemy(Map.MapDataForUnit[x - 1, y]))
            //            {
            //                string argctype_Renamed2 = "武装";
            //                CombinationPartner(argctype_Renamed2, w, partners, (x - 1), y, check_formation);
            //            }
            //        }
            //    }

            //    if (Information.UBound(partners) == 0)
            //    {
            //        if (x < Map.MapWidth)
            //        {
            //            if (Map.MapDataForUnit[x + 1, y] is object)
            //            {
            //                if (IsEnemy(Map.MapDataForUnit[x + 1, y]))
            //                {
            //                    string argctype_Renamed3 = "武装";
            //                    CombinationPartner(argctype_Renamed3, w, partners, (x + 1), y, check_formation);
            //                }
            //            }
            //        }
            //    }

            //    if (Information.UBound(partners) == 0)
            //    {
            //        if (y > 1)
            //        {
            //            if (Map.MapDataForUnit[x, y - 1] is object)
            //            {
            //                if (IsEnemy(Map.MapDataForUnit[x, y - 1]))
            //                {
            //                    string argctype_Renamed4 = "武装";
            //                    CombinationPartner(argctype_Renamed4, w, partners, x, (y - 1), check_formation);
            //                }
            //            }
            //        }
            //    }

            //    if (Information.UBound(partners) == 0)
            //    {
            //        if (y < Map.MapHeight)
            //        {
            //            if (Map.MapDataForUnit[x, y + 1] is object)
            //            {
            //                if (IsEnemy(Map.MapDataForUnit[x, y + 1]))
            //                {
            //                    string argctype_Renamed5 = "武装";
            //                    CombinationPartner(argctype_Renamed5, w, partners, x, (y + 1), check_formation);
            //                }
            //            }
            //        }
            //    }
            //}
            //else
            //{
            //    // 射程２以上の場合は自分の周りにパートナーがいればＯＫ
            //    string argctype_Renamed1 = "武装";
            //    CombinationPartner(argctype_Renamed1, w, partners, x, y, check_formation);
            //}

            //// 条件を満たすパートナーの組が見つかったか判定
            //if (Information.UBound(partners) > 0)
            //{
            //    IsCombinationAttackAvailableRet = true;
            //}
            //else
            //{
            //    IsCombinationAttackAvailableRet = false;
            //}

            //return IsCombinationAttackAvailableRet;
        }

        public bool IdDisplayFor(WeaponListMode mode)
        {
            // Disableコマンドで使用不可にされた武器と使用できない合体技は表示しない
            // 必要技能を満たさない武器は表示しない
            var baseCondition = !Unit.IsDisabled(Name)
                && IsWeaponMastered()
                // XXX 条件によって check_formation 見直す
                && !(IsWeaponClassifiedAs("合") && IsCombinationAttackAvailable(true))
                ;
            return baseCondition;
        }

        public bool CanUseFor(WeaponListMode mode, Unit targetUnit)
        {
            var display = IdDisplayFor(mode);
            switch (mode)
            {
                case WeaponListMode.List:
                    return display;

                case WeaponListMode.BeforeMove:
                    return display && IsWeaponUseful("移動前");

                case WeaponListMode.AfterMove:
                    return display && IsWeaponUseful("移動後");

                case WeaponListMode.Counter:
                    return display && IsWeaponAvailable("移動前")
                        // ターゲットが射程外
                        && IsTargetWithinRange(targetUnit)
                        // マップ攻撃は武器選定外
                        && !IsWeaponClassifiedAs("Ｍ")
                        // 合体技は自分から攻撃をかける場合にのみ使用
                        && !IsWeaponClassifiedAs("合")
                        // ダメージを与えられる
                        && !(Damage(targetUnit, true) > 0)
                        // 特殊効果を与えられる
                        && !(IsNormalWeapon() && CriticalProbability(targetUnit) > 0)
                        ;

                default:
                    throw new NotSupportedException();
            }
        }
    }

    public enum WeaponListMode
    {
        List,
        BeforeMove,
        AfterMove,
        Counter,
    }
}
