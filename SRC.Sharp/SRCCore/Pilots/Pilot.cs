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
    // 作成されたパイロットのクラス
    public partial class Pilot
    {
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
        public int SupportIndex;

        // レベル
        private int proLevel;

        // 経験値
        private int proEXP;
        // ＳＰ
        private int proSP;
        // 気力
        private int proMorale;
        // 霊力
        private int proPlana;

        // 能力値
        public int Infight;
        public int Shooting;
        public int Hit;
        public int Dodge;
        public int Technique;
        public int Intuition;
        public string Adaption;

        // 能力値の基本値
        public int InfightBase;
        public int ShootingBase;
        public int HitBase;
        public int DodgeBase;
        public int TechniqueBase;
        public int IntuitionBase;

        // 能力値の修正値

        // 特殊能力＆自ユニットによる修正
        public int InfightMod;
        public int ShootingMod;
        public int HitMod;
        public int DodgeMod;
        public int TechniqueMod;
        public int IntuitionMod;

        // 他ユニットによる支援修正
        public int InfightMod2;
        public int ShootingMod2;
        public int HitMod2;
        public int DodgeMod2;
        public int TechniqueMod2;
        public int IntuitionMod2;

        // 気力修正値
        public int MoraleMod;

        // 特殊能力
        private SrcCollection<SkillData> colSkill = new SrcCollection<SkillData>();

        // 名称
        public string Name => Data.Name;

        protected SRC SRC { get; }
        private IGUI GUI => SRC.GUI;

        public Pilot(SRC src, PilotData data)
        {
            SRC = src;
            Data = data;

            // Impl
            //Update();
        }
    }
}
