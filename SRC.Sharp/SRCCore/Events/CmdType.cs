// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

namespace SRCCore.Events
{
    // イベントコマンドの種類
    public enum CmdType
    {
        NullCmd = 0,
        NopCmd,
        ArcCmd,
        ArrayCmd,
        AskCmd,
        AttackCmd,
        AutoTalkCmd,
        BossRankCmd,
        BreakCmd,
        CallCmd,
        ReturnCmd,
        CallInterMissionCommandCmd,
        CancelCmd,
        CenterCmd,
        ChangeAreaCmd,
        ChangeLayerCmd,
        ChangeMapCmd,
        ChangeModeCmd,
        ChangePartyCmd,
        ChangeTerrainCmd,
        ChangeUnitBitmapCmd,
        ChargeCmd,
        CircleCmd,
        ClearEventCmd,
        ClearImageCmd,
        ClearLayerCmd,
        ClearObjCmd,
        ClearPictureCmd,
        ClearSkillCmd,
        ClearSpecialPowerCmd,
        ClearStatusCmd,
        CloseCmd,
        ClsCmd,
        ColorCmd,
        ColorFilterCmd,
        CombineCmd,
        ConfirmCmd,
        ContinueCmd,
        CopyArrayCmd,
        CopyFileCmd,
        CreateCmd,
        CreateFolderCmd,
        DebugCmd,
        DestroyCmd,
        DisableCmd,
        DoCmd,
        LoopCmd,
        DrawOptionCmd,
        DrawWidthCmd,
        EnableCmd,
        EquipCmd,
        EscapeCmd,
        ExchangeItemCmd,
        ExecCmd,
        ExitCmd,
        ExplodeCmd,
        ExpUpCmd,
        FadeInCmd,
        FadeOutCmd,
        FillColorCmd,
        FillStyleCmd,
        FinishCmd,
        FixCmd,
        FontCmd,
        ForCmd,
        ForEachCmd,
        NextCmd,
        ForgetCmd,
        GameClearCmd,
        GameOverCmd,
        FreeMemoryCmd,
        GetOffCmd,
        GlobalCmd,
        GotoCmd,
        HideCmd,
        HotPointCmd,
        IfCmd,
        ElseCmd,
        ElseIfCmd,
        EndIfCmd,
        IncrCmd,
        IncreaseMoraleCmd,
        InputCmd,
        IntermissionCommandCmd,
        ItemCmd,
        JoinCmd,
        KeepBGMCmd,
        LandCmd,
        LaunchCmd,
        LeaveCmd,
        LevelUpCmd,
        LineCmd,
        LineReadCmd,
        LoadCmd,
        LocalCmd,
        MakePilotListCmd,
        MakeUnitListCmd,
        MapAbilityCmd,
        MapAttackCmd,
        MoneyCmd,
        MonotoneCmd,
        MoveCmd,
        NightCmd,
        NoonCmd,
        OpenCmd,
        OptionCmd,
        OrganizeCmd,
        OvalCmd,
        PaintPictureCmd,
        PaintStringCmd,
        PaintStringRCmd,
        PaintSysStringCmd,
        PilotCmd,
        PlayMIDICmd,
        PlaySoundCmd,
        PolygonCmd,
        PrintCmd,
        PSetCmd,
        QuestionCmd,
        QuickLoadCmd,
        QuitCmd,
        RankUpCmd,
        ReadCmd,
        RecoverENCmd,
        RecoverHPCmd,
        RecoverPlanaCmd,
        RecoverSPCmd,
        RedrawCmd,
        RefreshCmd,
        ReleaseCmd,
        RemoveFileCmd,
        RemoveFolderCmd,
        RemoveItemCmd,
        RemovePilotCmd,
        RemoveUnitCmd,
        RenameBGMCmd,
        RenameFileCmd,
        RenameTermCmd,
        ReplacePilotCmd,
        RequireCmd,
        RestoreEventCmd,
        RideCmd,
        SaveDataCmd,
        SelectCmd,
        SelectTargetCmd,
        SepiaCmd,
        SetCmd,
        SetSkillCmd,
        SetBulletCmd,
        SetMessageCmd,
        SetRelationCmd,
        SetStatusStringColorCmd,
        SetStatusCmd,
        SetStockCmd,
        SetWindowColorCmd,
        SetWindowFrameWidthCmd,
        ShowCmd,
        ShowImageCmd,
        ShowUnitStatusCmd,
        SkipCmd,
        SortCmd,
        SpecialPowerCmd,
        SplitCmd,
        StartBGMCmd,
        StopBGMCmd,
        StopSummoningCmd,
        SupplyCmd,
        SunsetCmd,
        SwapCmd,
        SwitchCmd,
        CaseCmd,
        CaseElseCmd,
        EndSwCmd,
        TalkCmd,
        EndCmd,
        SuspendCmd,
        TelopCmd,
        TransformCmd,
        UnitCmd,
        UnsetCmd,
        UpgradeCmd,
        UpVarCmd,
        UseAbilityCmd,
        WaitCmd,
        WaterCmd,
        WhiteInCmd,
        WhiteOutCmd,
        WriteCmd,
        PlayFlashCmd,
        ClearFlashCmd
    }
}
