// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using Newtonsoft.Json;
using SRCCore.Models;
using SRCCore.Units;
using SRCCore.VB;

namespace SRCCore.Pilots
{
    // 作成されたパイロットのクラス
    [JsonObject(MemberSerialization.OptIn)]
    public partial class Pilot
    {
        protected SRC SRC;
        private IGUI GUI => SRC.GUI;
        private Expressions.Expression Expression => SRC.Expression;
        private Events.Event Event => SRC.Event;
        private Commands.Command Commands => SRC.Commands;

        public Pilot(SRC src, PilotData data)
        {
            SRC = src;
            Data = data;

            Update();
        }

        [JsonConstructor]
        private Pilot() { }

        public void Restore(SRC src)
        {
            SRC = src;
        }

        [JsonProperty]
        public string PilotName { get; set; }
        // パイロットデータへのポインタ
        public PilotData Data { get => SRC.PDList.Item(PilotName); set { PilotName = value?.Name; } }

        // 識別用ＩＤ
        [JsonProperty]
        public string ID;
        // 所属陣営
        [JsonProperty]
        public string Party;
        [JsonProperty]
        public string UnitId { get; set; }
        // XXX こうしてるところシリアライズの時だけID解決のほうがいいかな。
        // 搭乗しているユニット
        // 未搭乗時は Nothing
        public Unit Unit { get => SRC.UList.Item(UnitId); set { UnitId = value?.ID; } }

        // 生きているかどうか
        [JsonProperty]
        public bool Alive;

        // Leaveしているかどうか
        [JsonProperty]
        public bool Away;

        // 追加パイロットかどうか
        [JsonProperty]
        public bool IsAdditionalPilot;

        // 追加サポートかどうか
        [JsonProperty]
        public bool IsAdditionalSupport;

        // サポートパイロットとして乗り込んだ時の順番
        [JsonProperty]
        public int SupportIndex;

        // レベル
        [JsonProperty]
        private int proLevel;

        // 経験値
        [JsonProperty]
        private int proEXP;
        // ＳＰ
        [JsonProperty]
        private int proSP;
        // 気力
        [JsonProperty]
        private int proMorale;
        // 霊力
        [JsonProperty]
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
    }
}
