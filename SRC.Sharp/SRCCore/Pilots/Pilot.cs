// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using Newtonsoft.Json;
using SRCCore.Models;
using SRCCore.Units;
using SRCCore.VB;
using System.Collections.Generic;

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

        // 一時中断用データからの復元（VB: Restore。値は JSON から読み込み済み）
        // Unit は UnitId から引くので、VB の RestoreLinkInfo に当たる処理は要らない
        public void Restore(SRC src)
        {
            SRC = src;

            // VB の Dump は IsAdditionalPilot を保存しないので、読み込んだ直後は False になる。
            // 追加パイロットは Unit.MainPilot で改めて探され、そこで True に戻る
            // （(ザコ)・(汎用) の追加パイロットを、ロードのたびに作り直さないようにする）
            IsAdditionalPilot = false;
        }

        // 一時中断用データのパラメータ情報を復元する（VB: RestoreParameter）
        // 霊力・ＳＰの最大値は特殊能力で変動するので、ユニットとのリンクと特殊能力の設定が
        // 終わってから、保存した値を改めて設定し直す（最大値・最小値で切り詰める）。
        // VB の Dump は SP・Plana のプロパティの値（追加パイロットなら第１パイロットに合わせた値）を保存するので、
        // ここでもプロパティの値を設定し直す
        public void RestoreParameter()
        {
            var sp = SP;
            var morale = Morale;
            var plana = Plana;
            SP = sp;
            Morale = morale;
            Plana = plana;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Porting", "SRCP003", Justification = "VB の Data（オブジェクトの参照。初めは Nothing）を名前で持つ。null は Nothing に当たる")]
        [JsonProperty]
        public string PilotName { get; set; }
        // パイロットデータへのポインタ
        public PilotData Data { get => SRC.PDList.Item(PilotName); set { PilotName = value?.Name; } }

        // 識別用ＩＤ
        [JsonProperty]
        public string ID = "";
        // 所属陣営
        [JsonProperty]
        public string Party = "";
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Porting", "SRCP003", Justification = "VB の Unit（オブジェクトの参照。未搭乗は Nothing）を ID で持つ。null は Nothing に当たる")]
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
        public string Adaption = "";

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
        public IList<SkillData> Skills => colSkill.List;
    }
}
