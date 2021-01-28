// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

namespace SRC.Core.Events
{
    // イベントラベルの種類
    public enum LabelType
    {
        NormalLabel = 0,
        PrologueEventLabel,
        StartEventLabel,
        EpilogueEventLabel,
        TurnEventLabel,
        DamageEventLabel,
        DestructionEventLabel,
        TotalDestructionEventLabel,
        AttackEventLabel,
        AfterAttackEventLabel,
        TalkEventLabel,
        ContactEventLabel,
        EnterEventLabel,
        EscapeEventLabel,
        LandEventLabel,
        UseEventLabel,
        AfterUseEventLabel,
        TransformEventLabel,
        CombineEventLabel,
        SplitEventLabel,
        FinishEventLabel,
        LevelUpEventLabel,
        RequirementEventLabel,
        ResumeEventLabel,
        MapCommandEventLabel,
        UnitCommandEventLabel,
        EffectEventLabel
    }
}
