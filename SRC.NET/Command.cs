using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Project1
{
    static class Commands
    {

        // Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
        // 本プログラムはフリーソフトであり、無保証です。
        // 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
        // 再頒布または改変することができます。

        // ユニット＆マップコマンドの実行を行うモジュール

        // ユニットコマンドのメニュー番号
        public const short MoveCmdID = 0;
        public const short TeleportCmdID = 1;
        public const short JumpCmdID = 2;
        public const short TalkCmdID = 3;
        public const short AttackCmdID = 4;
        public const short FixCmdID = 5;
        public const short SupplyCmdID = 6;
        public const short AbilityCmdID = 7;
        public const short ChargeCmdID = 8;
        public const short SpecialPowerCmdID = 9;
        public const short TransformCmdID = 10;
        public const short SplitCmdID = 11;
        public const short CombineCmdID = 12;
        public const short HyperModeCmdID = 13;
        public const short GroundCmdID = 14;
        public const short SkyCmdID = 15;
        public const short UndergroundCmdID = 16;
        public const short WaterCmdID = 17;
        public const short LaunchCmdID = 18;
        public const short ItemCmdID = 19;
        public const short DismissCmdID = 20;
        public const short OrderCmdID = 21;
        public const short FeatureListCmdID = 22;
        public const short WeaponListCmdID = 23;
        public const short AbilityListCmdID = 24;
        public const short UnitCommand1CmdID = 25;
        public const short UnitCommand2CmdID = 26;
        public const short UnitCommand3CmdID = 27;
        public const short UnitCommand4CmdID = 28;
        public const short UnitCommand5CmdID = 29;
        public const short UnitCommand6CmdID = 30;
        public const short UnitCommand7CmdID = 31;
        public const short UnitCommand8CmdID = 32;
        public const short UnitCommand9CmdID = 33;
        public const short UnitCommand10CmdID = 34;
        public const short WaitCmdID = 35;

        // マップコマンドのメニュー番号
        public const short EndTurnCmdID = 0;
        public const short DumpCmdID = 1;
        public const short UnitListCmdID = 2;
        public const short SearchSpecialPowerCmdID = 3;
        public const short GlobalMapCmdID = 4;
        public const short OperationObjectCmdID = 5;
        public const short MapCommand1CmdID = 6;
        public const short MapCommand2CmdID = 7;
        public const short MapCommand3CmdID = 8;
        public const short MapCommand4CmdID = 9;
        public const short MapCommand5CmdID = 10;
        public const short MapCommand6CmdID = 11;
        public const short MapCommand7CmdID = 12;
        public const short MapCommand8CmdID = 13;
        public const short MapCommand9CmdID = 14;
        public const short MapCommand10CmdID = 15;
        public const short AutoDefenseCmdID = 16;
        public const short ConfigurationCmdID = 17;
        public const short RestartCmdID = 18;
        public const short QuickLoadCmdID = 19;
        public const short QuickSaveCmdID = 20;

        // 現在のコマンドの進行状況
        public static string CommandState;

        // クリック待ちモード
        public static bool WaitClickMode;
        // 閲覧モード
        public static bool ViewMode;

        // マップコマンドラベルのリスト
        private static string[] MapCommandLabelList = new string[11];
        // ユニットコマンドラベルのリスト
        private static string[] UnitCommandLabelList = new string[11];

        // 現在選択されているもの
        public static Unit SelectedUnit; // ユニット
        public static string SelectedCommand; // コマンド
        public static Unit SelectedTarget; // ターゲット
        public static short SelectedX; // Ｘ座標
        public static short SelectedY; // Ｙ座標
        public static short SelectedWeapon; // 武器
        public static string SelectedWeaponName;
        public static short SelectedTWeapon; // 反撃武器
        public static string SelectedTWeaponName;
        public static string SelectedDefenseOption; // 防御方法
        public static short SelectedAbility; // アビリティ
        public static string SelectedAbilityName;
        public static Pilot SelectedPilot; // パイロット
        public static short SelectedItem; // リストボックス中のアイテム
        public static string SelectedSpecialPower; // スペシャルパワー
        public static Unit[] SelectedPartners; // 合体技のパートナー
                                               // ADD START MARGE
        public static short SelectedUnitMoveCost; // 選択したユニットの移動力消費量
                                                  // ADD END MARGE

        // 選択状況の記録用変数
        public static short SelectionStackIndex;
        public static Unit[] SavedSelectedUnit;
        public static Unit[] SavedSelectedTarget;
        public static Unit[] SavedSelectedUnitForEvent;
        public static Unit[] SavedSelectedTargetForEvent;
        public static short[] SavedSelectedWeapon;
        public static string[] SavedSelectedWeaponName;
        public static short[] SavedSelectedTWeapon;
        public static string[] SavedSelectedTWeaponName;
        public static string[] SavedSelectedDefenseOption;
        public static short[] SavedSelectedAbility;
        public static string[] SavedSelectedAbilityName;
        public static short[] SavedSelectedX;
        public static short[] SavedSelectedY;

        // 援護を使うかどうか
        public static bool UseSupportAttack;
        public static bool UseSupportGuard;

        // 「味方スペシャルパワー実行」を使ってスペシャルパワーを使用するかどうか
        private static bool WithDoubleSPConsumption;

        // 攻撃を行うユニット
        public static Unit AttackUnit;
        // 援護攻撃を行うユニット
        public static Unit SupportAttackUnit;
        // 援護防御を行うユニット
        public static Unit SupportGuardUnit;
        // 援護防御を行うユニットのＨＰ値
        public static double SupportGuardUnitHPRatio;
        // 援護防御を行うユニット(反撃時)
        public static Unit SupportGuardUnit2;
        // 援護防御を行うユニットのＨＰ値(反撃時)
        public static double SupportGuardUnitHPRatio2;

        // 移動前のユニットの情報
        private static short PrevUnitX;
        private static short PrevUnitY;
        private static string PrevUnitArea;
        private static short PrevUnitEN;
        private static string PrevCommand;

        // 移動したユニットの情報
        public static Unit MovedUnit;
        public static short MovedUnitSpeed;


        // コマンドの処理を進める
        // by_cancel = True の場合はコマンドをキャンセルした場合の処理
        public static void ProceedCommand(bool by_cancel = false)
        {
            short j, i, n;
            Unit u;
            string uname;
            string buf;
            LabelData lab;

            // 閲覧モードはキャンセルで終了。それ以外の入力は無視
            if (ViewMode)
            {
                if (by_cancel)
                {
                    ViewMode = false;
                }

                return;
            }

            // 処理が行われるまでこれ以降のコマンド受付を禁止
            // (スクロール禁止にしなければならないほどの時間はないため、LockGUIは使わない)
            GUI.IsGUILocked = true;

            // コマンド実行を行うということはシナリオプレイ中ということなので毎回初期化する。
            SRC.IsScenarioFinished = false;
            SRC.IsCanceled = false;

            // ポップアップメニュー上で押したマウスボタンが左右どちらかを判定するため、
            // あらかじめGetAsyncKeyState()を実行しておく必要がある
            GUI.GetAsyncKeyState(GUI.RButtonID);
            switch (CommandState ?? "")
            {
                case "ユニット選択":
                case "マップコマンド":
                    {
                        // UPGRADE_NOTE: オブジェクト SelectedUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                        SelectedUnit = null;
                        // ADD START MARGE
                        SelectedUnitMoveCost = 0;
                        // ADD END MARGE
                        if (1 <= GUI.PixelToMapX((short)GUI.MouseX) & GUI.PixelToMapX((short)GUI.MouseX) <= Map.MapWidth & 1 <= GUI.PixelToMapY((short)GUI.MouseY) & GUI.PixelToMapY((short)GUI.MouseY) <= Map.MapHeight)
                        {
                            SelectedUnit = Map.MapDataForUnit[GUI.PixelToMapX((short)GUI.MouseX), GUI.PixelToMapY((short)GUI.MouseY)];
                        }

                        if (SelectedUnit is null)
                        {
                            SelectedX = GUI.PixelToMapX((short)GUI.MouseX);
                            SelectedY = GUI.PixelToMapY((short)GUI.MouseY);
                            if (!string.IsNullOrEmpty(Map.MapFileName))
                            {
                                // 通常のステージ

                                Status.DisplayGlobalStatus();

                                // ターン終了
                                if (ViewMode)
                                {
                                    // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    GUI.MainForm.mnuMapCommandItem(EndTurnCmdID).Caption = "部隊編成に戻る";
                                }
                                else
                                {
                                    // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    GUI.MainForm.mnuMapCommandItem(EndTurnCmdID).Caption = "ターン終了";
                                }
                                // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                GUI.MainForm.mnuMapCommandItem(EndTurnCmdID).Visible = true;

                                // 中断
                                string argoname1 = "デバッグ";
                                if (Expression.IsOptionDefined(ref argoname1))
                                {
                                    // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    GUI.MainForm.mnuMapCommandItem(DumpCmdID).Visible = true;
                                }
                                else
                                {
                                    string argoname = "クイックセーブ不可";
                                    if (!Expression.IsOptionDefined(ref argoname))
                                    {
                                        // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                        GUI.MainForm.mnuMapCommandItem(DumpCmdID).Visible = true;
                                    }
                                    else
                                    {
                                        // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                        GUI.MainForm.mnuMapCommandItem(DumpCmdID).Visible = false;
                                    }
                                }

                                // 全体マップ
                                // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                GUI.MainForm.mnuMapCommandItem(GlobalMapCmdID).Visible = true;

                                // 作戦目的
                                string arglname = "勝利条件";
                                if (Event_Renamed.IsEventDefined(ref arglname))
                                {
                                    // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    GUI.MainForm.mnuMapCommandItem(OperationObjectCmdID).Visible = true;
                                }
                                else
                                {
                                    // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    GUI.MainForm.mnuMapCommandItem(OperationObjectCmdID).Visible = false;
                                }

                                // 自動反撃モード
                                // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                GUI.MainForm.mnuMapCommandItem(AutoDefenseCmdID).Visible = true;

                                // 設定変更
                                // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                GUI.MainForm.mnuMapCommandItem(ConfigurationCmdID).Visible = true;

                                // リスタート
                                if (SRC.IsRestartSaveDataAvailable & !ViewMode)
                                {
                                    // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    GUI.MainForm.mnuMapCommandItem(RestartCmdID).Visible = true;
                                }
                                else
                                {
                                    // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    GUI.MainForm.mnuMapCommandItem(RestartCmdID).Visible = false;
                                }

                                // クイックロード
                                if (SRC.IsQuickSaveDataAvailable & !ViewMode)
                                {
                                    // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    GUI.MainForm.mnuMapCommandItem(QuickLoadCmdID).Visible = true;
                                }
                                else
                                {
                                    // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    GUI.MainForm.mnuMapCommandItem(QuickLoadCmdID).Visible = false;
                                }

                                // クイックセーブ
                                string argoname3 = "デバッグ";
                                if (ViewMode)
                                {
                                    // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    GUI.MainForm.mnuMapCommandItem(QuickSaveCmdID).Visible = false;
                                }
                                else if (Expression.IsOptionDefined(ref argoname3))
                                {
                                    // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    GUI.MainForm.mnuMapCommandItem(QuickSaveCmdID).Visible = true;
                                }
                                else
                                {
                                    string argoname2 = "クイックセーブ不可";
                                    if (!Expression.IsOptionDefined(ref argoname2))
                                    {
                                        // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                        GUI.MainForm.mnuMapCommandItem(QuickSaveCmdID).Visible = true;
                                    }
                                    else
                                    {
                                        // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                        GUI.MainForm.mnuMapCommandItem(QuickSaveCmdID).Visible = false;
                                    }
                                }
                            }
                            else
                            {
                                // パイロットステータス・ユニットステータスのステージ
                                {
                                    var withBlock = GUI.MainForm;
                                    // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    withBlock.mnuMapCommandItem(EndTurnCmdID).Visible = false;
                                    // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    withBlock.mnuMapCommandItem(DumpCmdID).Visible = false;
                                    // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    withBlock.mnuMapCommandItem(GlobalMapCmdID).Visible = false;
                                    // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    withBlock.mnuMapCommandItem(OperationObjectCmdID).Visible = false;
                                    // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    withBlock.mnuMapCommandItem(AutoDefenseCmdID).Visible = false;
                                    // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    withBlock.mnuMapCommandItem(ConfigurationCmdID).Visible = false;
                                    // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    withBlock.mnuMapCommandItem(RestartCmdID).Visible = false;
                                    // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    withBlock.mnuMapCommandItem(QuickLoadCmdID).Visible = false;
                                    // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    withBlock.mnuMapCommandItem(QuickSaveCmdID).Visible = false;
                                }
                            }

                            // スペシャルパワー検索
                            // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                            GUI.MainForm.mnuMapCommandItem(SearchSpecialPowerCmdID).Visible = false;
                            // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                            string argtname = "スペシャルパワー";
                            Unit argu = null;
                            GUI.MainForm.mnuMapCommandItem(SearchSpecialPowerCmdID).Caption = Expression.Term(ref argtname, u: ref argu) + "検索";
                            foreach (Pilot p in SRC.PList)
                            {
                                if (p.Party == "味方")
                                {
                                    if (p.CountSpecialPower > 0)
                                    {
                                        // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                        GUI.MainForm.mnuMapCommandItem(SearchSpecialPowerCmdID).Visible = true;
                                        break;
                                    }
                                }
                            }

                            // イベントで定義されたマップコマンド
                            {
                                var withBlock1 = GUI.MainForm;
                                // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                withBlock1.mnuMapCommandItem(MapCommand1CmdID).Visible = false;
                                // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                withBlock1.mnuMapCommandItem(MapCommand2CmdID).Visible = false;
                                // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                withBlock1.mnuMapCommandItem(MapCommand3CmdID).Visible = false;
                                // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                withBlock1.mnuMapCommandItem(MapCommand4CmdID).Visible = false;
                                // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                withBlock1.mnuMapCommandItem(MapCommand5CmdID).Visible = false;
                                // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                withBlock1.mnuMapCommandItem(MapCommand6CmdID).Visible = false;
                                // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                withBlock1.mnuMapCommandItem(MapCommand7CmdID).Visible = false;
                                // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                withBlock1.mnuMapCommandItem(MapCommand8CmdID).Visible = false;
                                // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                withBlock1.mnuMapCommandItem(MapCommand9CmdID).Visible = false;
                                // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                withBlock1.mnuMapCommandItem(MapCommand10CmdID).Visible = false;
                            }

                            if (!ViewMode)
                            {
                                i = MapCommand1CmdID;
                                foreach (LabelData currentLab in Event_Renamed.colEventLabelList)
                                {
                                    lab = currentLab;
                                    if (lab.Name == Event_Renamed.LabelType.MapCommandEventLabel)
                                    {
                                        if (lab.Enable)
                                        {
                                            int localStrToLng() { string argexpr = lab.Para(3); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

                                            if (lab.CountPara() == 2)
                                            {
                                                // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                                GUI.MainForm.mnuMapCommandItem(i).Visible = true;
                                            }
                                            else if (localStrToLng() != 0)
                                            {
                                                // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                                GUI.MainForm.mnuMapCommandItem(i).Visible = true;
                                            }
                                        }

                                        // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                        if (GUI.MainForm.mnuMapCommandItem(i).Visible)
                                        {
                                            // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                            GUI.MainForm.mnuMapCommandItem(i).Caption = lab.Para(2);
                                            MapCommandLabelList[i - MapCommand1CmdID + 1] = lab.LineNum.ToString();
                                            i = (short)(i + 1);
                                            if (i > MapCommand10CmdID)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                }
                            }

                            CommandState = "マップコマンド";
                            GUI.IsGUILocked = false;
                            // ADD START 240a
                            // ここに来た時点でcancel=Trueはユニットのいないセルを右クリックした場合のみ
                            if (by_cancel)
                            {
                                if (GUI.NewGUIMode & !string.IsNullOrEmpty(Map.MapFileName))
                                {
                                    if (GUI.MouseX < GUI.MainPWidth / 2)
                                    {
                                        // UPGRADE_ISSUE: Control picUnitStatus は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                        GUI.MainForm.picUnitStatus.Move(GUI.MainPWidth - 240, 10);
                                    }
                                    else
                                    {
                                        // UPGRADE_ISSUE: Control picUnitStatus は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                        GUI.MainForm.picUnitStatus.Move(5, 10);
                                    }
                                    // UPGRADE_ISSUE: Control picUnitStatus は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    GUI.MainForm.picUnitStatus.Visible = true;
                                }
                            }
                            // ADD  END  240a
                            // UPGRADE_ISSUE: Control mnuMapCommand は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                            // UPGRADE_ISSUE: Form メソッド MainForm.PopupMenu はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                            GUI.MainForm.PopupMenu(GUI.MainForm.mnuMapCommand, 6, GUI.MouseX, GUI.MouseY + 6f);
                            return;
                        }

                        Event_Renamed.SelectedUnitForEvent = SelectedUnit;
                        SelectedWeapon = 0;
                        SelectedTWeapon = 0;
                        SelectedAbility = 0;
                        if (by_cancel)
                        {
                            // ユニット上でキャンセルボタンを押した場合は武器一覧
                            // もしくはアビリティ一覧を表示する
                            {
                                var withBlock2 = SelectedUnit;
                                // 情報が隠蔽されている場合は表示しない
                                string argoname4 = "ユニット情報隠蔽";
                                object argIndex1 = "識別済み";
                                object argIndex2 = "ユニット情報隠蔽";
                                string argfname = "ダミーユニット";
                                if (Expression.IsOptionDefined(ref argoname4) & !withBlock2.IsConditionSatisfied(ref argIndex1) & (withBlock2.Party0 == "敵" | withBlock2.Party0 == "中立") | withBlock2.IsConditionSatisfied(ref argIndex2) | withBlock2.IsFeatureAvailable(ref argfname))
                                {
                                    GUI.IsGUILocked = false;
                                    return;
                                }

                                if (withBlock2.CountWeapon() == 0 & withBlock2.CountAbility() > 0)
                                {
                                    AbilityListCommand();
                                }
                                else
                                {
                                    WeaponListCommand();
                                }
                            }

                            GUI.IsGUILocked = false;
                            return;
                        }

                        CommandState = "コマンド選択";
                        ProceedCommand(by_cancel);
                        break;
                    }

                case "コマンド選択":
                    {
                        // MOD START 240aClearUnitStatus
                        // If MainWidth <> 15 Then
                        // DisplayUnitStatus SelectedUnit
                        // End If
                        if (!GUI.NewGUIMode)
                        {
                            Status.DisplayUnitStatus(ref SelectedUnit);
                        }
                        else
                        {
                            Status.ClearUnitStatus();
                        }
                        // MOD  END  240a

                        // 武装一覧以外は一旦消しておく
                        {
                            var withBlock3 = GUI.MainForm;
                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                            withBlock3.mnuUnitCommandItem(WeaponListCmdID).Visible = true;
                            for (i = 0; i <= WeaponListCmdID - 1; i++)
                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                withBlock3.mnuUnitCommandItem(i).Visible = false;
                            for (i = WeaponListCmdID + 1; i <= WaitCmdID; i++)
                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                withBlock3.mnuUnitCommandItem(i).Visible = false;
                        }

                        Event_Renamed.SelectedUnitForEvent = SelectedUnit;
                        // UPGRADE_NOTE: オブジェクト SelectedTarget をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                        SelectedTarget = null;
                        // UPGRADE_NOTE: オブジェクト SelectedTargetForEvent をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                        Event_Renamed.SelectedTargetForEvent = null;
                        {
                            var withBlock4 = SelectedUnit;
                            // 特殊能力＆アビリティ一覧はどのユニットでも見れる可能性があるので
                            // 先に判定しておく

                            // 特殊能力一覧コマンド
                            var loopTo = withBlock4.CountAllFeature();
                            for (i = 1; i <= loopTo; i++)
                            {
                                string localAllFeature() { object argIndex1 = i; var ret = withBlock4.AllFeature(ref argIndex1); return ret; }

                                string localAllFeature1() { object argIndex1 = i; var ret = withBlock4.AllFeature(ref argIndex1); return ret; }

                                string localAllFeature2() { object argIndex1 = i; var ret = withBlock4.AllFeature(ref argIndex1); return ret; }

                                string localAllFeature3() { object argIndex1 = i; var ret = withBlock4.AllFeature(ref argIndex1); return ret; }

                                object argIndex5 = i;
                                if (!string.IsNullOrEmpty(withBlock4.AllFeatureName(ref argIndex5)))
                                {
                                    object argIndex4 = i;
                                    switch (withBlock4.AllFeature(ref argIndex4) ?? "")
                                    {
                                        case "合体":
                                            {
                                                string localAllFeatureData() { object argIndex1 = i; var ret = withBlock4.AllFeatureData(ref argIndex1); return ret; }

                                                string localLIndex() { string arglist = hsde8149624c274ab08211d9ffa37bf9bf(); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                                                object argIndex3 = localLIndex();
                                                if (SRC.UList.IsDefined(ref argIndex3))
                                                {
                                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                                    GUI.MainForm.mnuUnitCommandItem(FeatureListCmdID).Visible = true;
                                                    break;
                                                }

                                                break;
                                            }

                                        default:
                                            {
                                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                                GUI.MainForm.mnuUnitCommandItem(FeatureListCmdID).Visible = true;
                                                break;
                                            }
                                    }
                                }
                                else if (localAllFeature() == "パイロット能力付加" | localAllFeature1() == "パイロット能力強化")
                                {
                                    string localAllFeatureData1() { object argIndex1 = i; var ret = withBlock4.AllFeatureData(ref argIndex1); return ret; }

                                    if (Strings.InStr(localAllFeatureData1(), "非表示") == 0)
                                    {
                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                        GUI.MainForm.mnuUnitCommandItem(FeatureListCmdID).Visible = true;
                                        break;
                                    }
                                }
                                else if (localAllFeature2() == "武器クラス" | localAllFeature3() == "防具クラス")
                                {
                                    string argoname5 = "アイテム交換";
                                    if (Expression.IsOptionDefined(ref argoname5))
                                    {
                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                        GUI.MainForm.mnuUnitCommandItem(FeatureListCmdID).Visible = true;
                                        break;
                                    }
                                }
                            }

                            {
                                var withBlock5 = withBlock4.MainPilot();
                                var loopTo1 = withBlock5.CountSkill();
                                for (i = 1; i <= loopTo1; i++)
                                {
                                    string localSkillName0() { object argIndex1 = i; var ret = withBlock5.SkillName0(ref argIndex1); return ret; }

                                    string localSkillName01() { object argIndex1 = i; var ret = withBlock5.SkillName0(ref argIndex1); return ret; }

                                    if (localSkillName0() != "非表示" & !string.IsNullOrEmpty(localSkillName01()))
                                    {
                                        object argIndex6 = i;
                                        switch (withBlock5.Skill(ref argIndex6) ?? "")
                                        {
                                            case "耐久":
                                                {
                                                    string argoname6 = "防御力成長";
                                                    string argoname7 = "防御力レベルアップ";
                                                    if (!Expression.IsOptionDefined(ref argoname6) & !Expression.IsOptionDefined(ref argoname7))
                                                    {
                                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                                        GUI.MainForm.mnuUnitCommandItem(FeatureListCmdID).Visible = true;
                                                        break;
                                                    }

                                                    break;
                                                }

                                            case "追加レベル":
                                            case "格闘ＵＰ":
                                            case "射撃ＵＰ":
                                            case "命中ＵＰ":
                                            case "回避ＵＰ":
                                            case "技量ＵＰ":
                                            case "反応ＵＰ":
                                            case "ＳＰＵＰ":
                                            case "格闘ＤＯＷＮ":
                                            case "射撃ＤＯＷＮ":
                                            case "命中ＤＯＷＮ":
                                            case "回避ＤＯＷＮ":
                                            case "技量ＤＯＷＮ":
                                            case "反応ＤＯＷＮ":
                                            case "ＳＰＤＯＷＮ":
                                            case "メッセージ":
                                            case "魔力所有":
                                                {
                                                    break;
                                                }

                                            default:
                                                {
                                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                                    GUI.MainForm.mnuUnitCommandItem(FeatureListCmdID).Visible = true;
                                                    break;
                                                }
                                        }
                                    }
                                }
                            }

                            // アビリティ一覧コマンド
                            var loopTo2 = withBlock4.CountAbility();
                            for (i = 1; i <= loopTo2; i++)
                            {
                                string argattr = "合";
                                if (withBlock4.IsAbilityMastered(i) & !withBlock4.IsDisabled(ref withBlock4.Ability(i).Name) & (!withBlock4.IsAbilityClassifiedAs(i, ref argattr) | withBlock4.IsCombinationAbilityAvailable(i, true)) & !withBlock4.Ability(i).IsItem())
                                {
                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    string argtname1 = "アビリティ";
                                    GUI.MainForm.mnuUnitCommandItem(AbilityListCmdID).Caption = Expression.Term(ref argtname1, ref SelectedUnit) + "一覧";
                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    GUI.MainForm.mnuUnitCommandItem(AbilityListCmdID).Visible = true;
                                    break;
                                }
                            }

                            // 味方じゃない場合
                            object argIndex36 = "非操作";
                            if (withBlock4.Party != "味方" | withBlock4.IsConditionSatisfied(ref argIndex36) | ViewMode)
                            {
                                // 召喚ユニットは命令コマンドを使用可能
                                string argfname1 = "召喚ユニット";
                                object argIndex7 = "魅了";
                                object argIndex8 = "混乱";
                                object argIndex9 = "恐怖";
                                object argIndex10 = "暴走";
                                object argIndex11 = "狂戦士";
                                if (withBlock4.Party == "ＮＰＣ" & withBlock4.IsFeatureAvailable(ref argfname1) & !withBlock4.IsConditionSatisfied(ref argIndex7) & !withBlock4.IsConditionSatisfied(ref argIndex8) & !withBlock4.IsConditionSatisfied(ref argIndex9) & !withBlock4.IsConditionSatisfied(ref argIndex10) & !withBlock4.IsConditionSatisfied(ref argIndex11) & !ViewMode)
                                {
                                    if (withBlock4.Summoner is object)
                                    {
                                        if (withBlock4.Summoner.Party == "味方")
                                        {
                                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                            GUI.MainForm.mnuUnitCommandItem(OrderCmdID).Caption = "命令";
                                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                            GUI.MainForm.mnuUnitCommandItem(OrderCmdID).Visible = true;
                                        }
                                    }
                                }

                                // 魅了したユニットに対しても命令コマンドを使用可能
                                object argIndex12 = "魅了";
                                object argIndex13 = "混乱";
                                object argIndex14 = "恐怖";
                                object argIndex15 = "暴走";
                                object argIndex16 = "狂戦士";
                                if (withBlock4.Party == "ＮＰＣ" & withBlock4.IsConditionSatisfied(ref argIndex12) & !withBlock4.IsConditionSatisfied(ref argIndex13) & !withBlock4.IsConditionSatisfied(ref argIndex14) & !withBlock4.IsConditionSatisfied(ref argIndex15) & !withBlock4.IsConditionSatisfied(ref argIndex16) & !ViewMode)
                                {
                                    if (withBlock4.Master is object)
                                    {
                                        if (withBlock4.Master.Party == "味方")
                                        {
                                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                            GUI.MainForm.mnuUnitCommandItem(OrderCmdID).Caption = "命令";
                                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                            GUI.MainForm.mnuUnitCommandItem(OrderCmdID).Visible = true;
                                        }
                                    }
                                }

                                // ダミーユニットの場合はコマンド一覧を表示しない
                                string argfname2 = "ダミーユニット";
                                if (withBlock4.IsFeatureAvailable(ref argfname2))
                                {
                                    // 特殊能力一覧
                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    if (GUI.MainForm.mnuUnitCommandItem(FeatureListCmdID).Visible)
                                    {
                                        UnitCommand(FeatureListCmdID);
                                    }
                                    else
                                    {
                                        CommandState = "ユニット選択";
                                    }

                                    GUI.IsGUILocked = false;
                                    return;
                                }

                                if (!string.IsNullOrEmpty(Map.MapFileName))
                                {
                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    GUI.MainForm.mnuUnitCommandItem(MoveCmdID).Caption = "移動範囲";
                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    GUI.MainForm.mnuUnitCommandItem(MoveCmdID).Visible = true;
                                    var loopTo3 = withBlock4.CountWeapon();
                                    for (i = 1; i <= loopTo3; i++)
                                    {
                                        string argref_mode = "";
                                        string argattr1 = "Ｍ";
                                        if (withBlock4.IsWeaponAvailable(i, ref argref_mode) & !withBlock4.IsWeaponClassifiedAs(i, ref argattr1))
                                        {
                                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                            GUI.MainForm.mnuUnitCommandItem(AttackCmdID).Caption = "射程範囲";
                                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                            GUI.MainForm.mnuUnitCommandItem(AttackCmdID).Visible = true;
                                        }
                                    }
                                }

                                // ユニットステータスコマンド用
                                if (string.IsNullOrEmpty(Map.MapFileName))
                                {
                                    // 変形コマンド
                                    string argfname3 = "変形";
                                    if (withBlock4.IsFeatureAvailable(ref argfname3))
                                    {
                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                        object argIndex17 = "変形";
                                        GUI.MainForm.mnuUnitCommandItem(TransformCmdID).Caption = withBlock4.FeatureName(ref argIndex17);
                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                        if (GUI.MainForm.mnuUnitCommandItem(TransformCmdID).Caption == "")
                                        {
                                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                            GUI.MainForm.mnuUnitCommandItem(TransformCmdID).Caption = "変形";
                                        }

                                        object argIndex19 = "変形";
                                        string arglist1 = withBlock4.FeatureData(ref argIndex19);
                                        var loopTo4 = GeneralLib.LLength(ref arglist1);
                                        for (i = 2; i <= loopTo4; i++)
                                        {
                                            object argIndex18 = "変形";
                                            string arglist = withBlock4.FeatureData(ref argIndex18);
                                            uname = GeneralLib.LIndex(ref arglist, i);
                                            Unit localOtherForm() { object argIndex1 = uname; var ret = withBlock4.OtherForm(ref argIndex1); return ret; }

                                            if (localOtherForm().IsAvailable())
                                            {
                                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                                GUI.MainForm.mnuUnitCommandItem(TransformCmdID).Visible = true;
                                                break;
                                            }
                                        }
                                    }

                                    // 分離コマンド
                                    string argfname5 = "分離";
                                    if (withBlock4.IsFeatureAvailable(ref argfname5))
                                    {
                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                        GUI.MainForm.mnuUnitCommandItem(SplitCmdID).Visible = true;
                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                        object argIndex20 = "分離";
                                        GUI.MainForm.mnuUnitCommandItem(SplitCmdID).Caption = withBlock4.FeatureName(ref argIndex20);
                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                        if (GUI.MainForm.mnuUnitCommandItem(SplitCmdID).Caption == "")
                                        {
                                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                            GUI.MainForm.mnuUnitCommandItem(SplitCmdID).Caption = "分離";
                                        }

                                        object argIndex21 = "分離";
                                        buf = withBlock4.FeatureData(ref argIndex21);

                                        // 分離形態が利用出来ない場合は分離を行わない
                                        var loopTo5 = GeneralLib.LLength(ref buf);
                                        for (i = 2; i <= loopTo5; i++)
                                        {
                                            bool localIsDefined() { object argIndex1 = GeneralLib.LIndex(ref buf, i); var ret = SRC.UList.IsDefined(ref argIndex1); return ret; }

                                            if (!localIsDefined())
                                            {
                                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                                GUI.MainForm.mnuUnitCommandItem(SplitCmdID).Visible = false;
                                                break;
                                            }
                                        }

                                        // パイロットが足らない場合も分離を行わない
                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                        if (GUI.MainForm.mnuUnitCommandItem(SplitCmdID).Visible)
                                        {
                                            n = 0;
                                            var loopTo6 = GeneralLib.LLength(ref buf);
                                            for (i = 2; i <= loopTo6; i++)
                                            {
                                                Unit localItem() { object argIndex1 = GeneralLib.LIndex(ref buf, i); var ret = SRC.UList.Item(ref argIndex1); return ret; }

                                                {
                                                    var withBlock6 = localItem().Data;
                                                    string argfname4 = "召喚ユニット";
                                                    if (!withBlock6.IsFeatureAvailable(ref argfname4))
                                                    {
                                                        n = (short)(n + Math.Abs(withBlock6.PilotNum));
                                                    }
                                                }
                                            }

                                            if (withBlock4.CountPilot() < n)
                                            {
                                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                                GUI.MainForm.mnuUnitCommandItem(SplitCmdID).Visible = false;
                                            }
                                        }
                                    }

                                    string argfname6 = "パーツ分離";
                                    if (withBlock4.IsFeatureAvailable(ref argfname6))
                                    {
                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                        object argIndex22 = "パーツ分離";
                                        GUI.MainForm.mnuUnitCommandItem(SplitCmdID).Caption = withBlock4.FeatureName(ref argIndex22);
                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                        if (GUI.MainForm.mnuUnitCommandItem(SplitCmdID).Caption == "")
                                        {
                                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                            GUI.MainForm.mnuUnitCommandItem(SplitCmdID).Caption = "パーツ分離";
                                        }
                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                        GUI.MainForm.mnuUnitCommandItem(SplitCmdID).Visible = true;
                                    }

                                    // 合体コマンド
                                    string argfname8 = "合体";
                                    string argfname9 = "パーツ合体";
                                    if (withBlock4.IsFeatureAvailable(ref argfname8))
                                    {
                                        var loopTo7 = withBlock4.CountFeature();
                                        for (i = 1; i <= loopTo7; i++)
                                        {
                                            object argIndex24 = i;
                                            if (withBlock4.Feature(ref argIndex24) == "合体")
                                            {
                                                n = 0;
                                                // パートナーが存在しているか？
                                                string localFeatureData1() { object argIndex1 = i; var ret = withBlock4.FeatureData(ref argIndex1); return ret; }

                                                string arglist2 = localFeatureData1();
                                                var loopTo8 = GeneralLib.LLength(ref arglist2);
                                                for (j = 3; j <= loopTo8; j++)
                                                {
                                                    string localFeatureData() { object argIndex1 = i; var ret = withBlock4.FeatureData(ref argIndex1); return ret; }

                                                    string localLIndex1() { string arglist = hse3098790f77a4c3c8e351b0c8f045435(); var ret = GeneralLib.LIndex(ref arglist, j); return ret; }

                                                    object argIndex23 = localLIndex1();
                                                    u = SRC.UList.Item(ref argIndex23);
                                                    if (u is null)
                                                    {
                                                        break;
                                                    }

                                                    string argfname7 = "合体制限";
                                                    if (u.Status_Renamed != "出撃" & u.CurrentForm().IsFeatureAvailable(ref argfname7))
                                                    {
                                                        break;
                                                    }

                                                    n = (short)(n + 1);
                                                }

                                                // 合体先のユニットが作成されているか？
                                                string localFeatureData2() { object argIndex1 = i; var ret = withBlock4.FeatureData(ref argIndex1); return ret; }

                                                string localLIndex2() { string arglist = hse489dc5578704b21a62d1221f27f2c9c(); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                                                bool localIsDefined1() { object argIndex1 = (object)hs2edf74710015446592f60b6fcb7267d6(); var ret = SRC.UList.IsDefined(ref argIndex1); return ret; }

                                                if (!localIsDefined1())
                                                {
                                                    n = 0;
                                                }

                                                // すべての条件を満たしている場合
                                                string localFeatureData4() { object argIndex1 = i; var ret = withBlock4.FeatureData(ref argIndex1); return ret; }

                                                short localLLength() { string arglist = hs57a7b11782d04866bf1e5d24ed51c504(); var ret = GeneralLib.LLength(ref arglist); return ret; }

                                                if (n == localLLength() - 2)
                                                {
                                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                                    GUI.MainForm.mnuUnitCommandItem(CombineCmdID).Visible = true;
                                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                                    string localFeatureData3() { object argIndex1 = i; var ret = withBlock4.FeatureData(ref argIndex1); return ret; }

                                                    string arglist3 = localFeatureData3();
                                                    GUI.MainForm.mnuUnitCommandItem(CombineCmdID).Caption = GeneralLib.LIndex(ref arglist3, 1);
                                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                                    if (GUI.MainForm.mnuUnitCommandItem(CombineCmdID).Caption == "非表示")
                                                    {
                                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                                        GUI.MainForm.mnuUnitCommandItem(CombineCmdID).Caption = "合体";
                                                    }

                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    else if (withBlock4.IsFeatureAvailable(ref argfname9))
                                    {
                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                        GUI.MainForm.mnuUnitCommandItem(CombineCmdID).Caption = "パーツ合体";
                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                        GUI.MainForm.mnuUnitCommandItem(CombineCmdID).Visible = true;
                                    }

                                    object argIndex30 = "ノーマルモード付加";
                                    if (!withBlock4.IsConditionSatisfied(ref argIndex30))
                                    {
                                        // ハイパーモードコマンド
                                        string argfname10 = "ハイパーモード";
                                        string argfname11 = "ノーマルモード";
                                        if (withBlock4.IsFeatureAvailable(ref argfname10))
                                        {
                                            object argIndex25 = "ハイパーモード";
                                            string arglist4 = withBlock4.FeatureData(ref argIndex25);
                                            uname = GeneralLib.LIndex(ref arglist4, 2);
                                            Unit localOtherForm1() { object argIndex1 = uname; var ret = withBlock4.OtherForm(ref argIndex1); return ret; }

                                            if (localOtherForm1().IsAvailable())
                                            {
                                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                                GUI.MainForm.mnuUnitCommandItem(HyperModeCmdID).Visible = true;
                                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                                object argIndex26 = "ハイパーモード";
                                                string arglist5 = withBlock4.FeatureData(ref argIndex26);
                                                GUI.MainForm.mnuUnitCommandItem(HyperModeCmdID).Caption = GeneralLib.LIndex(ref arglist5, 1);
                                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                                if (GUI.MainForm.mnuUnitCommandItem(HyperModeCmdID).Caption == "非表示")
                                                {
                                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                                    GUI.MainForm.mnuUnitCommandItem(HyperModeCmdID).Caption = "ハイパーモード";
                                                }
                                            }
                                        }
                                        else if (withBlock4.IsFeatureAvailable(ref argfname11))
                                        {
                                            object argIndex27 = "ノーマルモード";
                                            string arglist6 = withBlock4.FeatureData(ref argIndex27);
                                            uname = GeneralLib.LIndex(ref arglist6, 1);
                                            Unit localOtherForm2() { object argIndex1 = uname; var ret = withBlock4.OtherForm(ref argIndex1); return ret; }

                                            if (localOtherForm2().IsAvailable())
                                            {
                                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                                GUI.MainForm.mnuUnitCommandItem(HyperModeCmdID).Visible = true;
                                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                                GUI.MainForm.mnuUnitCommandItem(HyperModeCmdID).Caption = "ノーマルモード";
                                                string localLIndex3() { object argIndex1 = "変形"; string arglist = withBlock4.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                                                if ((uname ?? "") == (localLIndex3() ?? ""))
                                                {
                                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                                    GUI.MainForm.mnuUnitCommandItem(HyperModeCmdID).Visible = false;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        // 変身解除
                                        object argIndex29 = "ノーマルモード";
                                        if (Strings.InStr(withBlock4.FeatureData(ref argIndex29), "手動解除") > 0)
                                        {
                                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                            GUI.MainForm.mnuUnitCommandItem(HyperModeCmdID).Visible = true;
                                            string argfname12 = "変身解除コマンド名";
                                            if (withBlock4.IsFeatureAvailable(ref argfname12))
                                            {
                                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                                object argIndex28 = "変身解除コマンド名";
                                                GUI.MainForm.mnuUnitCommandItem(HyperModeCmdID).Caption = withBlock4.FeatureData(ref argIndex28);
                                            }
                                            else if (withBlock4.IsHero())
                                            {
                                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                                GUI.MainForm.mnuUnitCommandItem(HyperModeCmdID).Caption = "変身解除";
                                            }
                                            else
                                            {
                                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                                GUI.MainForm.mnuUnitCommandItem(HyperModeCmdID).Caption = "特殊モード解除";
                                            }
                                        }
                                    }

                                    // 換装コマンド
                                    string argfname13 = "換装";
                                    if (withBlock4.IsFeatureAvailable(ref argfname13))
                                    {
                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                        GUI.MainForm.mnuUnitCommandItem(OrderCmdID).Caption = "換装";
                                        object argIndex32 = "換装";
                                        string arglist8 = withBlock4.FeatureData(ref argIndex32);
                                        var loopTo9 = GeneralLib.LLength(ref arglist8);
                                        for (i = 1; i <= loopTo9; i++)
                                        {
                                            object argIndex31 = "換装";
                                            string arglist7 = withBlock4.FeatureData(ref argIndex31);
                                            uname = GeneralLib.LIndex(ref arglist7, i);
                                            Unit localOtherForm3() { object argIndex1 = uname; var ret = withBlock4.OtherForm(ref argIndex1); return ret; }

                                            if (localOtherForm3().IsAvailable())
                                            {
                                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                                GUI.MainForm.mnuUnitCommandItem(OrderCmdID).Visible = true;
                                                break;
                                            }
                                        }

                                        // エリアスで換装の名称が変更されている？
                                        {
                                            var withBlock7 = SRC.ALDList;
                                            var loopTo10 = withBlock7.Count();
                                            for (i = 1; i <= loopTo10; i++)
                                            {
                                                object argIndex33 = i;
                                                {
                                                    var withBlock8 = withBlock7.Item(ref argIndex33);
                                                    if (withBlock8.get_AliasType(1) == "換装")
                                                    {
                                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                                        GUI.MainForm.mnuUnitCommandItem(OrderCmdID).Caption = withBlock8.Name;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                // ユニットコマンド
                                if (!ViewMode)
                                {
                                    i = UnitCommand1CmdID;
                                    foreach (LabelData currentLab1 in Event_Renamed.colEventLabelList)
                                    {
                                        lab = currentLab1;
                                        if (lab.Name == Event_Renamed.LabelType.UnitCommandEventLabel & lab.Enable)
                                        {
                                            string argexpr = lab.Para(3);
                                            buf = Expression.GetValueAsString(ref argexpr);
                                            if (SelectedUnit.Party == "味方" & ((buf ?? "") == (SelectedUnit.MainPilot().Name ?? "") | (buf ?? "") == (SelectedUnit.MainPilot().get_Nickname(false) ?? "") | (buf ?? "") == (SelectedUnit.Name ?? "")) | (buf ?? "") == (SelectedUnit.Party ?? "") | buf == "全")
                                            {
                                                int localGetValueAsLong() { string argexpr = lab.Para(4); var ret = Expression.GetValueAsLong(ref argexpr); return ret; }

                                                if (lab.CountPara() <= 3)
                                                {
                                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                                    GUI.MainForm.mnuUnitCommandItem(i).Visible = true;
                                                }
                                                else if (localGetValueAsLong() != 0)
                                                {
                                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                                    GUI.MainForm.mnuUnitCommandItem(i).Visible = true;
                                                }
                                            }

                                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                            if (GUI.MainForm.mnuUnitCommandItem(i).Visible)
                                            {
                                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                                GUI.MainForm.mnuUnitCommandItem(i).Caption = lab.Para(2);
                                                UnitCommandLabelList[i - UnitCommand1CmdID + 1] = lab.LineNum.ToString();
                                                i = (short)(i + 1);
                                                if (i > UnitCommand10CmdID)
                                                {
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }

                                // 未確認ユニットの場合は情報を隠蔽
                                string argoname8 = "ユニット情報隠蔽";
                                object argIndex34 = "識別済み";
                                object argIndex35 = "ユニット情報隠蔽";
                                if (Expression.IsOptionDefined(ref argoname8) & !withBlock4.IsConditionSatisfied(ref argIndex34) & (withBlock4.Party0 == "敵" | withBlock4.Party0 == "中立") | withBlock4.IsConditionSatisfied(ref argIndex35))
                                {
                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    GUI.MainForm.mnuUnitCommandItem(MoveCmdID).Visible = true;
                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    GUI.MainForm.mnuUnitCommandItem(AttackCmdID).Visible = false;
                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    GUI.MainForm.mnuUnitCommandItem(FeatureListCmdID).Visible = false;
                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    GUI.MainForm.mnuUnitCommandItem(WeaponListCmdID).Visible = false;
                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    GUI.MainForm.mnuUnitCommandItem(AbilityListCmdID).Visible = false;
                                    for (i = 1; i <= WaitCmdID; i++)
                                    {
                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                        if (GUI.MainForm.mnuUnitCommandItem(i).Visible)
                                        {
                                            break;
                                        }
                                    }

                                    if (i > WaitCmdID)
                                    {
                                        // 表示可能なコマンドがなかった
                                        CommandState = "ユニット選択";
                                        GUI.IsGUILocked = false;
                                        return;
                                    }
                                    // メニューコマンドを全て殺してしまうとエラーになるのでここで非表示
                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    GUI.MainForm.mnuUnitCommandItem(MoveCmdID).Visible = false;
                                }

                                GUI.IsGUILocked = false;
                                if (by_cancel)
                                {
                                    // UPGRADE_ISSUE: Control mnuUnitCommand は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    // UPGRADE_ISSUE: Form メソッド MainForm.PopupMenu はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                                    GUI.MainForm.PopupMenu(GUI.MainForm.mnuUnitCommand, 6, GUI.MouseX, GUI.MouseY + 5f);
                                }
                                else
                                {
                                    // UPGRADE_ISSUE: Control mnuUnitCommand は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    // UPGRADE_ISSUE: Form メソッド MainForm.PopupMenu はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                                    GUI.MainForm.PopupMenu(GUI.MainForm.mnuUnitCommand, 6, GUI.MouseX, GUI.MouseY - 6f);
                                }

                                return;
                            }

                            // 行動終了している場合
                            else if (withBlock4.Action == 0)
                            {
                                // 発進コマンドは使用可能
                                string argfname14 = "母艦";
                                if (withBlock4.IsFeatureAvailable(ref argfname14))
                                {
                                    if (withBlock4.Area != "地中")
                                    {
                                        if (withBlock4.CountUnitOnBoard() > 0)
                                        {
                                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                            GUI.MainForm.mnuUnitCommandItem(LaunchCmdID).Visible = true;
                                        }
                                    }
                                }

                                // ユニットコマンド
                                i = UnitCommand1CmdID;
                                foreach (LabelData currentLab2 in Event_Renamed.colEventLabelList)
                                {
                                    lab = currentLab2;
                                    if (lab.Name == Event_Renamed.LabelType.UnitCommandEventLabel & (lab.AsterNum == 1 | lab.AsterNum == 3))
                                    {
                                        if (lab.Enable)
                                        {
                                            buf = lab.Para(3);
                                            if (SelectedUnit.Party == "味方" & ((buf ?? "") == (SelectedUnit.MainPilot().Name ?? "") | (buf ?? "") == (SelectedUnit.MainPilot().get_Nickname(false) ?? "") | (buf ?? "") == (SelectedUnit.Name ?? "")) | (buf ?? "") == (SelectedUnit.Party ?? "") | buf == "全")
                                            {
                                                int localStrToLng1() { string argexpr = lab.Para(4); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

                                                if (lab.CountPara() <= 3)
                                                {
                                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                                    GUI.MainForm.mnuUnitCommandItem(i).Visible = true;
                                                }
                                                else if (localStrToLng1() != 0)
                                                {
                                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                                    GUI.MainForm.mnuUnitCommandItem(i).Visible = true;
                                                }
                                            }
                                        }

                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                        if (GUI.MainForm.mnuUnitCommandItem(i).Visible)
                                        {
                                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                            GUI.MainForm.mnuUnitCommandItem(i).Caption = lab.Para(2);
                                            UnitCommandLabelList[i - UnitCommand1CmdID + 1] = lab.LineNum.ToString();
                                            i = (short)(i + 1);
                                            if (i > UnitCommand10CmdID)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                }

                                GUI.IsGUILocked = false;
                                // UPGRADE_ISSUE: Control mnuUnitCommand は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                // UPGRADE_ISSUE: Form メソッド MainForm.PopupMenu はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                                GUI.MainForm.PopupMenu(GUI.MainForm.mnuUnitCommand, 6, GUI.MouseX, GUI.MouseY - 5f);
                                return;
                            }

                            // 移動コマンド
                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                            GUI.MainForm.mnuUnitCommandItem(MoveCmdID).Caption = "移動";
                            if (withBlock4.Speed <= 0)
                            {
                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                GUI.MainForm.mnuUnitCommandItem(WaitCmdID).Visible = true; // 待機
                            }
                            else
                            {
                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                GUI.MainForm.mnuUnitCommandItem(MoveCmdID).Visible = true;
                            } // 移動

                            // テレポートコマンド
                            string argfname15 = "テレポート";
                            if (withBlock4.IsFeatureAvailable(ref argfname15))
                            {
                                object argIndex38 = "テレポート";
                                if (Strings.Len(withBlock4.FeatureData(ref argIndex38)) > 0)
                                {
                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    object argIndex37 = "テレポート";
                                    string arglist9 = withBlock4.FeatureData(ref argIndex37);
                                    GUI.MainForm.mnuUnitCommandItem(TeleportCmdID).Caption = GeneralLib.LIndex(ref arglist9, 1);
                                }
                                else
                                {
                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    GUI.MainForm.mnuUnitCommandItem(TeleportCmdID).Caption = "テレポート";
                                }

                                object argIndex40 = "テレポート";
                                string arglist10 = withBlock4.FeatureData(ref argIndex40);
                                if (GeneralLib.LLength(ref arglist10) == 2)
                                {
                                    string localLIndex4() { object argIndex1 = "テレポート"; string arglist = withBlock4.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                                    string localLIndex5() { object argIndex1 = "テレポート"; string arglist = withBlock4.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                                    if (withBlock4.EN >= Conversions.ToShort(localLIndex5()))
                                    {
                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                        GUI.MainForm.mnuUnitCommandItem(TeleportCmdID).Visible = true;
                                    }
                                    // 通常移動がテレポートの場合
                                    string localLIndex6() { object argIndex1 = "テレポート"; string arglist = withBlock4.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                                    object argIndex39 = "テレポート";
                                    if (withBlock4.Speed0 == 0 | withBlock4.FeatureLevel(ref argIndex39) >= 0d & localLIndex6() == "0")
                                    {
                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                        GUI.MainForm.mnuUnitCommandItem(MoveCmdID).Visible = false;
                                    }
                                }
                                else if (withBlock4.EN >= 40)
                                {
                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    GUI.MainForm.mnuUnitCommandItem(TeleportCmdID).Visible = true;
                                }

                                object argIndex41 = "移動不能";
                                if (withBlock4.IsConditionSatisfied(ref argIndex41))
                                {
                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    GUI.MainForm.mnuUnitCommandItem(TeleportCmdID).Visible = false;
                                }
                            }

                            // ジャンプコマンド
                            string argfname16 = "ジャンプ";
                            if (withBlock4.IsFeatureAvailable(ref argfname16) & withBlock4.Area != "空中" & withBlock4.Area != "宇宙")
                            {
                                object argIndex43 = "ジャンプ";
                                if (Strings.Len(withBlock4.FeatureData(ref argIndex43)) > 0)
                                {
                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    object argIndex42 = "ジャンプ";
                                    string arglist11 = withBlock4.FeatureData(ref argIndex42);
                                    GUI.MainForm.mnuUnitCommandItem(JumpCmdID).Caption = GeneralLib.LIndex(ref arglist11, 1);
                                }
                                else
                                {
                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    GUI.MainForm.mnuUnitCommandItem(JumpCmdID).Caption = "ジャンプ";
                                }

                                object argIndex46 = "ジャンプ";
                                string arglist12 = withBlock4.FeatureData(ref argIndex46);
                                if (GeneralLib.LLength(ref arglist12) == 2)
                                {
                                    string localLIndex7() { object argIndex1 = "ジャンプ"; string arglist = withBlock4.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                                    string localLIndex8() { object argIndex1 = "ジャンプ"; string arglist = withBlock4.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                                    if (withBlock4.EN >= Conversions.ToShort(localLIndex8()))
                                    {
                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                        GUI.MainForm.mnuUnitCommandItem(JumpCmdID).Visible = true;
                                    }
                                    // 通常移動がジャンプの場合
                                    string localLIndex9() { object argIndex1 = "ジャンプ"; string arglist = withBlock4.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                                    object argIndex44 = "ジャンプ";
                                    if (withBlock4.Speed0 == 0 | withBlock4.FeatureLevel(ref argIndex44) >= 0d & localLIndex9() == "0")
                                    {
                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                        GUI.MainForm.mnuUnitCommandItem(MoveCmdID).Visible = false;
                                    }
                                }
                                else
                                {
                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    GUI.MainForm.mnuUnitCommandItem(JumpCmdID).Visible = true;
                                    object argIndex45 = "ジャンプ";
                                    if (withBlock4.FeatureLevel(ref argIndex45) >= 0d)
                                    {
                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                        GUI.MainForm.mnuUnitCommandItem(MoveCmdID).Visible = false;
                                    }
                                }

                                object argIndex47 = "移動不能";
                                if (withBlock4.IsConditionSatisfied(ref argIndex47))
                                {
                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    GUI.MainForm.mnuUnitCommandItem(JumpCmdID).Visible = false;
                                }
                            }

                            // 会話コマンド
                            for (i = 1; i <= 4; i++)
                            {
                                // UPGRADE_NOTE: オブジェクト u をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                                u = null;
                                switch (i)
                                {
                                    case 1:
                                        {
                                            if (withBlock4.x > 1)
                                            {
                                                u = Map.MapDataForUnit[withBlock4.x - 1, withBlock4.y];
                                            }

                                            break;
                                        }

                                    case 2:
                                        {
                                            if (withBlock4.x < Map.MapWidth)
                                            {
                                                u = Map.MapDataForUnit[withBlock4.x + 1, withBlock4.y];
                                            }

                                            break;
                                        }

                                    case 3:
                                        {
                                            if (withBlock4.y > 1)
                                            {
                                                u = Map.MapDataForUnit[withBlock4.x, withBlock4.y - 1];
                                            }

                                            break;
                                        }

                                    case 4:
                                        {
                                            if (withBlock4.y < Map.MapHeight)
                                            {
                                                u = Map.MapDataForUnit[withBlock4.x, withBlock4.y + 1];
                                            }

                                            break;
                                        }
                                }

                                if (u is object)
                                {
                                    string arglname1 = "会話 " + withBlock4.MainPilot().ID + " " + u.MainPilot().ID;
                                    if (Event_Renamed.IsEventDefined(ref arglname1))
                                    {
                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                        GUI.MainForm.mnuUnitCommandItem(TalkCmdID).Visible = true;
                                        break;
                                    }
                                }
                            }

                            // 攻撃コマンド
                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                            GUI.MainForm.mnuUnitCommandItem(AttackCmdID).Caption = "攻撃";
                            var loopTo11 = withBlock4.CountWeapon();
                            for (i = 1; i <= loopTo11; i++)
                            {
                                string argref_mode1 = "移動前";
                                if (withBlock4.IsWeaponUseful(i, ref argref_mode1))
                                {
                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    GUI.MainForm.mnuUnitCommandItem(AttackCmdID).Visible = true;
                                    break;
                                }
                            }

                            if (withBlock4.Area == "地中")
                            {
                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                GUI.MainForm.mnuUnitCommandItem(AttackCmdID).Visible = false;
                            }

                            object argIndex48 = "攻撃不能";
                            if (withBlock4.IsConditionSatisfied(ref argIndex48))
                            {
                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                GUI.MainForm.mnuUnitCommandItem(AttackCmdID).Visible = false;
                            }

                            // 修理コマンド
                            string argfname17 = "修理装置";
                            if (withBlock4.IsFeatureAvailable(ref argfname17) & withBlock4.Area != "地中")
                            {
                                for (i = 1; i <= 4; i++)
                                {
                                    // UPGRADE_NOTE: オブジェクト u をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                                    u = null;
                                    switch (i)
                                    {
                                        case 1:
                                            {
                                                if (withBlock4.x > 1)
                                                {
                                                    u = Map.MapDataForUnit[withBlock4.x - 1, withBlock4.y];
                                                }

                                                break;
                                            }

                                        case 2:
                                            {
                                                if (withBlock4.x < Map.MapWidth)
                                                {
                                                    u = Map.MapDataForUnit[withBlock4.x + 1, withBlock4.y];
                                                }

                                                break;
                                            }

                                        case 3:
                                            {
                                                if (withBlock4.y > 1)
                                                {
                                                    u = Map.MapDataForUnit[withBlock4.x, withBlock4.y - 1];
                                                }

                                                break;
                                            }

                                        case 4:
                                            {
                                                if (withBlock4.y < Map.MapHeight)
                                                {
                                                    u = Map.MapDataForUnit[withBlock4.x, withBlock4.y + 1];
                                                }

                                                break;
                                            }
                                    }

                                    if (u is object)
                                    {
                                        {
                                            var withBlock9 = u;
                                            object argIndex49 = "ゾンビ";
                                            if ((withBlock9.Party == "味方" | withBlock9.Party == "ＮＰＣ") & withBlock9.HP < withBlock9.MaxHP & !withBlock9.IsConditionSatisfied(ref argIndex49))
                                            {
                                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                                GUI.MainForm.mnuUnitCommandItem(FixCmdID).Visible = true;
                                                break;
                                            }
                                        }
                                    }
                                }

                                object argIndex51 = "修理装置";
                                if (Strings.Len(withBlock4.FeatureData(ref argIndex51)) > 0)
                                {
                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    object argIndex50 = "修理装置";
                                    string arglist13 = withBlock4.FeatureData(ref argIndex50);
                                    GUI.MainForm.mnuUnitCommandItem(FixCmdID).Caption = GeneralLib.LIndex(ref arglist13, 1);
                                    string localLIndex12() { object argIndex1 = "修理装置"; string arglist = withBlock4.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                                    if (Information.IsNumeric(localLIndex12()))
                                    {
                                        string localLIndex10() { object argIndex1 = "修理装置"; string arglist = withBlock4.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                                        string localLIndex11() { object argIndex1 = "修理装置"; string arglist = withBlock4.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                                        if (withBlock4.EN < Conversions.ToShort(localLIndex11()))
                                        {
                                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                            GUI.MainForm.mnuUnitCommandItem(FixCmdID).Visible = false;
                                        }
                                    }
                                }
                                else
                                {
                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    GUI.MainForm.mnuUnitCommandItem(FixCmdID).Caption = "修理装置";
                                }
                            }

                            // 補給コマンド
                            string argfname18 = "補給装置";
                            if (withBlock4.IsFeatureAvailable(ref argfname18) & withBlock4.Area != "地中")
                            {
                                for (i = 1; i <= 4; i++)
                                {
                                    // UPGRADE_NOTE: オブジェクト u をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                                    u = null;
                                    switch (i)
                                    {
                                        case 1:
                                            {
                                                if (withBlock4.x > 1)
                                                {
                                                    u = Map.MapDataForUnit[withBlock4.x - 1, withBlock4.y];
                                                }

                                                break;
                                            }

                                        case 2:
                                            {
                                                if (withBlock4.x < Map.MapWidth)
                                                {
                                                    u = Map.MapDataForUnit[withBlock4.x + 1, withBlock4.y];
                                                }

                                                break;
                                            }

                                        case 3:
                                            {
                                                if (withBlock4.y > 1)
                                                {
                                                    u = Map.MapDataForUnit[withBlock4.x, withBlock4.y - 1];
                                                }

                                                break;
                                            }

                                        case 4:
                                            {
                                                if (withBlock4.y < Map.MapHeight)
                                                {
                                                    u = Map.MapDataForUnit[withBlock4.x, withBlock4.y + 1];
                                                }

                                                break;
                                            }
                                    }

                                    if (u is object)
                                    {
                                        {
                                            var withBlock10 = u;
                                            if (withBlock10.Party == "味方" | withBlock10.Party == "ＮＰＣ")
                                            {
                                                object argIndex52 = "ゾンビ";
                                                if (withBlock10.EN < withBlock10.MaxEN & !withBlock10.IsConditionSatisfied(ref argIndex52))
                                                {
                                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                                    GUI.MainForm.mnuUnitCommandItem(SupplyCmdID).Visible = true;
                                                }
                                                else
                                                {
                                                    var loopTo12 = withBlock10.CountWeapon();
                                                    for (j = 1; j <= loopTo12; j++)
                                                    {
                                                        if (withBlock10.Bullet(j) < withBlock10.MaxBullet(j))
                                                        {
                                                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                                            GUI.MainForm.mnuUnitCommandItem(SupplyCmdID).Visible = true;
                                                            break;
                                                        }
                                                    }

                                                    var loopTo13 = withBlock10.CountAbility();
                                                    for (j = 1; j <= loopTo13; j++)
                                                    {
                                                        if (withBlock10.Stock(j) < withBlock10.MaxStock(j))
                                                        {
                                                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                                            GUI.MainForm.mnuUnitCommandItem(SupplyCmdID).Visible = true;
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                object argIndex54 = "補給装置";
                                if (Strings.Len(withBlock4.FeatureData(ref argIndex54)) > 0)
                                {
                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    object argIndex53 = "補給装置";
                                    string arglist14 = withBlock4.FeatureData(ref argIndex53);
                                    GUI.MainForm.mnuUnitCommandItem(SupplyCmdID).Caption = GeneralLib.LIndex(ref arglist14, 1);
                                    string localLIndex15() { object argIndex1 = "補給装置"; string arglist = withBlock4.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                                    if (Information.IsNumeric(localLIndex15()))
                                    {
                                        string localLIndex13() { object argIndex1 = "補給装置"; string arglist = withBlock4.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                                        string localLIndex14() { object argIndex1 = "補給装置"; string arglist = withBlock4.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                                        if (withBlock4.EN < Conversions.ToShort(localLIndex14()) | withBlock4.MainPilot().Morale < 100)
                                        {
                                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                            GUI.MainForm.mnuUnitCommandItem(SupplyCmdID).Visible = false;
                                        }
                                    }
                                }
                                else
                                {
                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    GUI.MainForm.mnuUnitCommandItem(SupplyCmdID).Caption = "補給装置";
                                }
                            }

                            // アビリティコマンド
                            n = 0;
                            var loopTo14 = withBlock4.CountAbility();
                            for (i = 1; i <= loopTo14; i++)
                            {
                                if (!withBlock4.Ability(i).IsItem() & withBlock4.IsAbilityMastered(i))
                                {
                                    n = (short)(n + 1);
                                    string argref_mode2 = "移動前";
                                    if (withBlock4.IsAbilityUseful(i, ref argref_mode2))
                                    {
                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                        GUI.MainForm.mnuUnitCommandItem(AbilityCmdID).Visible = true;
                                    }
                                }
                            }

                            if (withBlock4.Area == "地中")
                            {
                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                GUI.MainForm.mnuUnitCommandItem(AbilityCmdID).Visible = false;
                            }
                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                            string argtname2 = "アビリティ";
                            GUI.MainForm.mnuUnitCommandItem(AbilityCmdID).Caption = Expression.Term(ref argtname2, ref SelectedUnit);
                            if (n == 1)
                            {
                                var loopTo15 = withBlock4.CountAbility();
                                for (i = 1; i <= loopTo15; i++)
                                {
                                    if (!withBlock4.Ability(i).IsItem() & withBlock4.IsAbilityMastered(i))
                                    {
                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                        GUI.MainForm.mnuUnitCommandItem(AbilityCmdID).Caption = withBlock4.AbilityNickname(i);
                                        break;
                                    }
                                }
                            }

                            // チャージコマンド
                            object argIndex55 = "チャージ完了";
                            if (!withBlock4.IsConditionSatisfied(ref argIndex55))
                            {
                                var loopTo16 = withBlock4.CountWeapon();
                                for (i = 1; i <= loopTo16; i++)
                                {
                                    string argattr2 = "Ｃ";
                                    if (withBlock4.IsWeaponClassifiedAs(i, ref argattr2))
                                    {
                                        string argref_mode3 = "チャージ";
                                        if (withBlock4.IsWeaponAvailable(i, ref argref_mode3))
                                        {
                                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                            GUI.MainForm.mnuUnitCommandItem(ChargeCmdID).Visible = true;
                                            break;
                                        }
                                    }
                                }

                                var loopTo17 = withBlock4.CountAbility();
                                for (i = 1; i <= loopTo17; i++)
                                {
                                    string argattr3 = "Ｃ";
                                    if (withBlock4.IsAbilityClassifiedAs(i, ref argattr3))
                                    {
                                        string argref_mode4 = "チャージ";
                                        if (withBlock4.IsAbilityAvailable(i, ref argref_mode4))
                                        {
                                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                            GUI.MainForm.mnuUnitCommandItem(ChargeCmdID).Visible = true;
                                            break;
                                        }
                                    }
                                }
                            }

                            // スペシャルパワーコマンド
                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                            string argtname3 = "スペシャルパワー";
                            GUI.MainForm.mnuUnitCommandItem(SpecialPowerCmdID).Caption = Expression.Term(ref argtname3, ref SelectedUnit);
                            if (withBlock4.MainPilot().CountSpecialPower > 0)
                            {
                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                GUI.MainForm.mnuUnitCommandItem(SpecialPowerCmdID).Visible = true;
                            }
                            else
                            {
                                var loopTo18 = withBlock4.CountPilot();
                                for (i = 1; i <= loopTo18; i++)
                                {
                                    Pilot localPilot() { object argIndex1 = i; var ret = withBlock4.Pilot(ref argIndex1); return ret; }

                                    if (localPilot().CountSpecialPower > 0)
                                    {
                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                        GUI.MainForm.mnuUnitCommandItem(SpecialPowerCmdID).Visible = true;
                                        break;
                                    }
                                }

                                var loopTo19 = withBlock4.CountSupport();
                                for (i = 1; i <= loopTo19; i++)
                                {
                                    Pilot localSupport() { object argIndex1 = i; var ret = withBlock4.Support(ref argIndex1); return ret; }

                                    if (localSupport().CountSpecialPower > 0)
                                    {
                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                        GUI.MainForm.mnuUnitCommandItem(SpecialPowerCmdID).Visible = true;
                                        break;
                                    }
                                }

                                string argfname19 = "追加サポート";
                                if (withBlock4.IsFeatureAvailable(ref argfname19))
                                {
                                    if (withBlock4.AdditionalSupport().CountSpecialPower > 0)
                                    {
                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                        GUI.MainForm.mnuUnitCommandItem(SpecialPowerCmdID).Visible = true;
                                    }
                                }
                            }

                            object argIndex56 = "憑依";
                            object argIndex57 = "スペシャルパワー使用不能";
                            if (withBlock4.IsConditionSatisfied(ref argIndex56) | withBlock4.IsConditionSatisfied(ref argIndex57))
                            {
                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                GUI.MainForm.mnuUnitCommandItem(SpecialPowerCmdID).Visible = false;
                            }

                            // 変形コマンド
                            string argfname20 = "変形";
                            object argIndex61 = "変形";
                            object argIndex62 = "形態固定";
                            object argIndex63 = "機体固定";
                            if (withBlock4.IsFeatureAvailable(ref argfname20) & !string.IsNullOrEmpty(withBlock4.FeatureName(ref argIndex61)) & !withBlock4.IsConditionSatisfied(ref argIndex62) & !withBlock4.IsConditionSatisfied(ref argIndex63))
                            {
                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                object argIndex58 = "変形";
                                GUI.MainForm.mnuUnitCommandItem(TransformCmdID).Caption = withBlock4.FeatureName(ref argIndex58);
                                object argIndex60 = "変形";
                                string arglist16 = withBlock4.FeatureData(ref argIndex60);
                                var loopTo20 = GeneralLib.LLength(ref arglist16);
                                for (i = 2; i <= loopTo20; i++)
                                {
                                    object argIndex59 = "変形";
                                    string arglist15 = withBlock4.FeatureData(ref argIndex59);
                                    uname = GeneralLib.LIndex(ref arglist15, i);
                                    Unit localOtherForm4() { object argIndex1 = uname; var ret = withBlock4.OtherForm(ref argIndex1); return ret; }

                                    if (localOtherForm4().IsAvailable())
                                    {
                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                        GUI.MainForm.mnuUnitCommandItem(TransformCmdID).Visible = true;
                                        break;
                                    }
                                }
                            }

                            // 分離コマンド
                            string argfname22 = "分離";
                            object argIndex66 = "分離";
                            object argIndex67 = "形態固定";
                            object argIndex68 = "機体固定";
                            if (withBlock4.IsFeatureAvailable(ref argfname22) & !string.IsNullOrEmpty(withBlock4.FeatureName(ref argIndex66)) & !withBlock4.IsConditionSatisfied(ref argIndex67) & !withBlock4.IsConditionSatisfied(ref argIndex68))
                            {
                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                GUI.MainForm.mnuUnitCommandItem(SplitCmdID).Visible = true;
                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                object argIndex64 = "分離";
                                GUI.MainForm.mnuUnitCommandItem(SplitCmdID).Caption = withBlock4.FeatureName(ref argIndex64);
                                object argIndex65 = "分離";
                                buf = withBlock4.FeatureData(ref argIndex65);

                                // 分離形態が利用出来ない場合は分離を行わない
                                var loopTo21 = GeneralLib.LLength(ref buf);
                                for (i = 2; i <= loopTo21; i++)
                                {
                                    bool localIsDefined2() { object argIndex1 = GeneralLib.LIndex(ref buf, i); var ret = SRC.UList.IsDefined(ref argIndex1); return ret; }

                                    if (!localIsDefined2())
                                    {
                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                        GUI.MainForm.mnuUnitCommandItem(SplitCmdID).Visible = false;
                                        break;
                                    }
                                }

                                // パイロットが足らない場合も分離を行わない
                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                if (GUI.MainForm.mnuUnitCommandItem(SplitCmdID).Visible)
                                {
                                    n = 0;
                                    var loopTo22 = GeneralLib.LLength(ref buf);
                                    for (i = 2; i <= loopTo22; i++)
                                    {
                                        Unit localItem1() { object argIndex1 = GeneralLib.LIndex(ref buf, i); var ret = SRC.UList.Item(ref argIndex1); return ret; }

                                        {
                                            var withBlock11 = localItem1().Data;
                                            string argfname21 = "召喚ユニット";
                                            if (!withBlock11.IsFeatureAvailable(ref argfname21))
                                            {
                                                n = (short)(n + Math.Abs(withBlock11.PilotNum));
                                            }
                                        }
                                    }

                                    if (withBlock4.CountPilot() < n)
                                    {
                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                        GUI.MainForm.mnuUnitCommandItem(SplitCmdID).Visible = false;
                                    }
                                }
                            }

                            string argfname23 = "パーツ分離";
                            object argIndex70 = "パーツ分離";
                            if (withBlock4.IsFeatureAvailable(ref argfname23) & !string.IsNullOrEmpty(withBlock4.FeatureName(ref argIndex70)))
                            {
                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                object argIndex69 = "パーツ分離";
                                GUI.MainForm.mnuUnitCommandItem(SplitCmdID).Caption = withBlock4.FeatureName(ref argIndex69);
                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                GUI.MainForm.mnuUnitCommandItem(SplitCmdID).Visible = true;
                            }

                            // 合体コマンド
                            string argfname25 = "合体";
                            object argIndex74 = "形態固定";
                            object argIndex75 = "機体固定";
                            if (withBlock4.IsFeatureAvailable(ref argfname25) & !withBlock4.IsConditionSatisfied(ref argIndex74) & !withBlock4.IsConditionSatisfied(ref argIndex75))
                            {
                                var loopTo23 = withBlock4.CountFeature();
                                for (i = 1; i <= loopTo23; i++)
                                {
                                    // 3体以上からなる合体能力を持っているか？
                                    string localFeature() { object argIndex1 = i; var ret = withBlock4.Feature(ref argIndex1); return ret; }

                                    string localFeatureName() { object argIndex1 = i; var ret = withBlock4.FeatureName(ref argIndex1); return ret; }

                                    string localFeatureData10() { object argIndex1 = i; var ret = withBlock4.FeatureData(ref argIndex1); return ret; }

                                    short localLLength2() { string arglist = hsc81ad842db7849b2b1585eed09bb5348(); var ret = GeneralLib.LLength(ref arglist); return ret; }

                                    if (localFeature() == "合体" & !string.IsNullOrEmpty(localFeatureName()) & localLLength2() > 3)
                                    {
                                        n = 0;
                                        // パートナーは隣接しているか？
                                        string localFeatureData6() { object argIndex1 = i; var ret = withBlock4.FeatureData(ref argIndex1); return ret; }

                                        string arglist17 = localFeatureData6();
                                        var loopTo24 = GeneralLib.LLength(ref arglist17);
                                        for (j = 3; j <= loopTo24; j++)
                                        {
                                            string localFeatureData5() { object argIndex1 = i; var ret = withBlock4.FeatureData(ref argIndex1); return ret; }

                                            string localLIndex16() { string arglist = hsd2010d4d78e3489683c8d110139f0dc7(); var ret = GeneralLib.LIndex(ref arglist, j); return ret; }

                                            object argIndex71 = localLIndex16();
                                            u = SRC.UList.Item(ref argIndex71);
                                            if (u is null)
                                            {
                                                break;
                                            }

                                            if (!u.IsOperational())
                                            {
                                                break;
                                            }

                                            string argfname24 = "合体制限";
                                            if (u.Status_Renamed != "出撃" & u.CurrentForm().IsFeatureAvailable(ref argfname24))
                                            {
                                                break;
                                            }

                                            if (Math.Abs((short)(withBlock4.x - u.CurrentForm().x)) + Math.Abs((short)(withBlock4.y - u.CurrentForm().y)) > 2)
                                            {
                                                break;
                                            }

                                            n = (short)(n + 1);
                                        }

                                        // 合体先のユニットが作成され、かつ合体可能な状態にあるか？
                                        string localFeatureData7() { object argIndex1 = i; var ret = withBlock4.FeatureData(ref argIndex1); return ret; }

                                        string arglist18 = localFeatureData7();
                                        uname = GeneralLib.LIndex(ref arglist18, 2);
                                        object argIndex72 = uname;
                                        u = SRC.UList.Item(ref argIndex72);
                                        object argIndex73 = "行動不能";
                                        if (u is null)
                                        {
                                            n = 0;
                                        }
                                        else if (u.IsConditionSatisfied(ref argIndex73))
                                        {
                                            n = 0;
                                        }

                                        // すべての条件を満たしている場合
                                        string localFeatureData9() { object argIndex1 = i; var ret = withBlock4.FeatureData(ref argIndex1); return ret; }

                                        short localLLength1() { string arglist = hs7cff35930ba14e62b7ee40e2d0172e97(); var ret = GeneralLib.LLength(ref arglist); return ret; }

                                        if (n == localLLength1() - 2)
                                        {
                                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                            GUI.MainForm.mnuUnitCommandItem(CombineCmdID).Visible = true;
                                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                            string localFeatureData8() { object argIndex1 = i; var ret = withBlock4.FeatureData(ref argIndex1); return ret; }

                                            string arglist19 = localFeatureData8();
                                            GUI.MainForm.mnuUnitCommandItem(CombineCmdID).Caption = GeneralLib.LIndex(ref arglist19, 1);
                                            break;
                                        }
                                    }
                                }
                            }

                            object argIndex88 = "ノーマルモード付加";
                            if (!withBlock4.IsConditionSatisfied(ref argIndex88))
                            {
                                // ハイパーモードコマンド
                                string argfname26 = "ハイパーモード";
                                object argIndex79 = "ハイパーモード";
                                object argIndex80 = "ハイパーモード";
                                object argIndex81 = "ハイパーモード";
                                object argIndex82 = "ハイパーモード";
                                object argIndex83 = "ハイパーモード";
                                object argIndex84 = "形態固定";
                                object argIndex85 = "機体固定";
                                if (withBlock4.IsFeatureAvailable(ref argfname26) & (withBlock4.MainPilot().Morale >= (short)(10d * withBlock4.FeatureLevel(ref argIndex80)) + 100 | withBlock4.HP <= withBlock4.MaxHP / 4 & Strings.InStr(withBlock4.FeatureData(ref argIndex81), "気力発動") == 0) & Strings.InStr(withBlock4.FeatureData(ref argIndex82), "自動発動") == 0 & !string.IsNullOrEmpty(withBlock4.FeatureName(ref argIndex83)) & !withBlock4.IsConditionSatisfied(ref argIndex84) & !withBlock4.IsConditionSatisfied(ref argIndex85))
                                {
                                    object argIndex76 = "ハイパーモード";
                                    string arglist20 = withBlock4.FeatureData(ref argIndex76);
                                    uname = GeneralLib.LIndex(ref arglist20, 2);
                                    Unit localOtherForm5() { object argIndex1 = uname; var ret = withBlock4.OtherForm(ref argIndex1); return ret; }

                                    Unit localOtherForm6() { object argIndex1 = uname; var ret = withBlock4.OtherForm(ref argIndex1); return ret; }

                                    object argIndex78 = "行動不能";
                                    if (!localOtherForm5().IsConditionSatisfied(ref argIndex78) & localOtherForm6().IsAbleToEnter(withBlock4.x, withBlock4.y))
                                    {
                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                        GUI.MainForm.mnuUnitCommandItem(HyperModeCmdID).Visible = true;
                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                        object argIndex77 = "ハイパーモード";
                                        string arglist21 = withBlock4.FeatureData(ref argIndex77);
                                        GUI.MainForm.mnuUnitCommandItem(HyperModeCmdID).Caption = GeneralLib.LIndex(ref arglist21, 1);
                                    }
                                }
                            }
                            else
                            {
                                // 変身解除
                                object argIndex87 = "ノーマルモード";
                                if (Strings.InStr(withBlock4.FeatureData(ref argIndex87), "手動解除") > 0)
                                {
                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    GUI.MainForm.mnuUnitCommandItem(HyperModeCmdID).Visible = true;
                                    string argfname27 = "変身解除コマンド名";
                                    if (withBlock4.IsFeatureAvailable(ref argfname27))
                                    {
                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                        object argIndex86 = "変身解除コマンド名";
                                        GUI.MainForm.mnuUnitCommandItem(HyperModeCmdID).Caption = withBlock4.FeatureData(ref argIndex86);
                                    }
                                    else if (withBlock4.IsHero())
                                    {
                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                        GUI.MainForm.mnuUnitCommandItem(HyperModeCmdID).Caption = "変身解除";
                                    }
                                    else
                                    {
                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                        GUI.MainForm.mnuUnitCommandItem(HyperModeCmdID).Caption = "特殊モード解除";
                                    }
                                }
                            }

                            // 地上コマンド
                            if (Map.TerrainClass(withBlock4.x, withBlock4.y) == "陸" | Map.TerrainClass(withBlock4.x, withBlock4.y) == "屋内" | Map.TerrainClass(withBlock4.x, withBlock4.y) == "月面")
                            {
                                string argarea_name = "陸";
                                if (withBlock4.Area != "地上" & withBlock4.IsTransAvailable(ref argarea_name))
                                {
                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    GUI.MainForm.mnuUnitCommandItem(GroundCmdID).Caption = "地上";
                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    GUI.MainForm.mnuUnitCommandItem(GroundCmdID).Visible = true;
                                }
                            }
                            else if (Map.TerrainClass(withBlock4.x, withBlock4.y) == "水" | Map.TerrainClass(withBlock4.x, withBlock4.y) == "深水")
                            {
                                string argarea_name1 = "水上";
                                if (withBlock4.Area != "水上" & withBlock4.IsTransAvailable(ref argarea_name1))
                                {
                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    GUI.MainForm.mnuUnitCommandItem(GroundCmdID).Caption = "水上";
                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    GUI.MainForm.mnuUnitCommandItem(GroundCmdID).Visible = true;
                                }
                            }

                            // 空中コマンド
                            switch (Map.TerrainClass(withBlock4.x, withBlock4.y) ?? "")
                            {
                                case "宇宙":
                                    {
                                        break;
                                    }

                                case "月面":
                                    {
                                        string argarea_name2 = "空";
                                        string argarea_name3 = "宇宙";
                                        if ((withBlock4.IsTransAvailable(ref argarea_name2) | withBlock4.IsTransAvailable(ref argarea_name3)) & !(withBlock4.Area == "宇宙"))
                                        {
                                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                            GUI.MainForm.mnuUnitCommandItem(SkyCmdID).Caption = "宇宙";
                                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                            GUI.MainForm.mnuUnitCommandItem(SkyCmdID).Visible = true;
                                        }

                                        break;
                                    }

                                default:
                                    {
                                        string argarea_name4 = "空";
                                        if (withBlock4.IsTransAvailable(ref argarea_name4) & !(withBlock4.Area == "空中"))
                                        {
                                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                            GUI.MainForm.mnuUnitCommandItem(SkyCmdID).Caption = "空中";
                                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                            GUI.MainForm.mnuUnitCommandItem(SkyCmdID).Visible = true;
                                        }

                                        break;
                                    }
                            }

                            // 地中コマンド
                            string argarea_name5 = "地中";
                            if (withBlock4.IsTransAvailable(ref argarea_name5) & !(withBlock4.Area == "地中") & (Map.TerrainClass(withBlock4.x, withBlock4.y) == "陸" | Map.TerrainClass(withBlock4.x, withBlock4.y) == "月面"))
                            {
                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                GUI.MainForm.mnuUnitCommandItem(UndergroundCmdID).Visible = true;
                            }

                            // 水中コマンド
                            if (withBlock4.Area != "水中")
                            {
                                string argarea_name6 = "水";
                                string argfname28 = "水泳";
                                if (Map.TerrainClass(withBlock4.x, withBlock4.y) == "深水" & (withBlock4.IsTransAvailable(ref argarea_name6) | withBlock4.IsFeatureAvailable(ref argfname28)) & Strings.Mid(withBlock4.Data.Adaption, 3, 1) != "-")
                                {
                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    GUI.MainForm.mnuUnitCommandItem(WaterCmdID).Visible = true;
                                }
                                else if (Map.TerrainClass(withBlock4.x, withBlock4.y) == "水" & Strings.Mid(withBlock4.Data.Adaption, 3, 1) != "-")
                                {
                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    GUI.MainForm.mnuUnitCommandItem(WaterCmdID).Visible = true;
                                }
                            }

                            // 発進コマンド
                            string argfname29 = "母艦";
                            if (withBlock4.IsFeatureAvailable(ref argfname29) & withBlock4.Area != "地中")
                            {
                                if (withBlock4.CountUnitOnBoard() > 0)
                                {
                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    GUI.MainForm.mnuUnitCommandItem(LaunchCmdID).Visible = true;
                                }
                            }

                            // アイテムコマンド
                            var loopTo25 = withBlock4.CountAbility();
                            for (i = 1; i <= loopTo25; i++)
                            {
                                string argref_mode5 = "移動前";
                                if (withBlock4.IsAbilityUseful(i, ref argref_mode5) & withBlock4.Ability(i).IsItem())
                                {
                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    GUI.MainForm.mnuUnitCommandItem(ItemCmdID).Visible = true;
                                    break;
                                }
                            }

                            if (withBlock4.Area == "地中")
                            {
                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                GUI.MainForm.mnuUnitCommandItem(ItemCmdID).Visible = false;
                            }

                            // 召喚解除コマンド
                            var loopTo26 = withBlock4.CountServant();
                            for (i = 1; i <= loopTo26; i++)
                            {
                                Unit localServant() { object argIndex1 = i; var ret = withBlock4.Servant(ref argIndex1); return ret; }

                                {
                                    var withBlock12 = localServant().CurrentForm();
                                    switch (withBlock12.Status_Renamed ?? "")
                                    {
                                        case "出撃":
                                        case "格納":
                                            {
                                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                                GUI.MainForm.mnuUnitCommandItem(DismissCmdID).Visible = true;
                                                break;
                                            }

                                        case "旧主形態":
                                        case "旧形態":
                                            {
                                                // 合体後の形態が出撃中なら使用不可
                                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                                GUI.MainForm.mnuUnitCommandItem(DismissCmdID).Visible = true;
                                                var loopTo27 = withBlock12.CountFeature();
                                                for (j = 1; j <= loopTo27; j++)
                                                {
                                                    object argIndex90 = j;
                                                    if (withBlock12.Feature(ref argIndex90) == "合体")
                                                    {
                                                        string localFeatureData11() { object argIndex1 = j; var ret = withBlock12.FeatureData(ref argIndex1); return ret; }

                                                        string arglist22 = localFeatureData11();
                                                        uname = GeneralLib.LIndex(ref arglist22, 2);
                                                        object argIndex89 = uname;
                                                        if (SRC.UList.IsDefined(ref argIndex89))
                                                        {
                                                            Unit localItem2() { object argIndex1 = uname; var ret = SRC.UList.Item(ref argIndex1); return ret; }

                                                            {
                                                                var withBlock13 = localItem2().CurrentForm();
                                                                if (withBlock13.Status_Renamed == "出撃" | withBlock13.Status_Renamed == "格納")
                                                                {
                                                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                                                    GUI.MainForm.mnuUnitCommandItem(DismissCmdID).Visible = false;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }

                                                break;
                                            }
                                    }
                                }
                            }

                            string argfname30 = "召喚解除コマンド名";
                            if (withBlock4.IsFeatureAvailable(ref argfname30))
                            {
                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                object argIndex91 = "召喚解除コマンド名";
                                GUI.MainForm.mnuUnitCommandItem(DismissCmdID).Caption = withBlock4.FeatureData(ref argIndex91);
                            }
                            else
                            {
                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                GUI.MainForm.mnuUnitCommandItem(DismissCmdID).Caption = "召喚解除";
                            }

                            // ユニットコマンド
                            i = UnitCommand1CmdID;
                            foreach (LabelData currentLab3 in Event_Renamed.colEventLabelList)
                            {
                                lab = currentLab3;
                                if (lab.Name == Event_Renamed.LabelType.UnitCommandEventLabel)
                                {
                                    if (lab.Enable)
                                    {
                                        buf = lab.Para(3);
                                        if (SelectedUnit.Party == "味方" & ((buf ?? "") == (SelectedUnit.MainPilot().Name ?? "") | (buf ?? "") == (SelectedUnit.MainPilot().get_Nickname(false) ?? "") | (buf ?? "") == (SelectedUnit.Name ?? "")) | (buf ?? "") == (SelectedUnit.Party ?? "") | buf == "全")
                                        {
                                            int localStrToLng2() { string argexpr = lab.Para(4); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

                                            if (lab.CountPara() <= 3)
                                            {
                                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                                GUI.MainForm.mnuUnitCommandItem(i).Visible = true;
                                            }
                                            else if (localStrToLng2() != 0)
                                            {
                                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                                GUI.MainForm.mnuUnitCommandItem(i).Visible = true;
                                            }
                                        }
                                    }

                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    if (GUI.MainForm.mnuUnitCommandItem(i).Visible)
                                    {
                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                        GUI.MainForm.mnuUnitCommandItem(i).Caption = lab.Para(2);
                                        UnitCommandLabelList[i - UnitCommand1CmdID + 1] = lab.LineNum.ToString();
                                        i = (short)(i + 1);
                                        if (i > UnitCommand10CmdID)
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (!ReferenceEquals(SelectedUnit, Status.DisplayedUnit))
                        {
                            // MOD START 240a
                            // DisplayUnitStatus SelectedUnit
                            // 新ＧＵＩ使用時はクリック時にユニットステータスを表示しない
                            if (!GUI.NewGUIMode)
                            {
                                Status.DisplayUnitStatus(ref SelectedUnit);
                            }
                            // MOD  END  240a
                        }

                        GUI.IsGUILocked = false;
                        if (by_cancel)
                        {
                            // UPGRADE_ISSUE: Control mnuUnitCommand は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                            // UPGRADE_ISSUE: Form メソッド MainForm.PopupMenu はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                            GUI.MainForm.PopupMenu(GUI.MainForm.mnuUnitCommand, 6, GUI.MouseX, GUI.MouseY + 5f);
                        }
                        else
                        {
                            // UPGRADE_ISSUE: Control mnuUnitCommand は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                            // UPGRADE_ISSUE: Form メソッド MainForm.PopupMenu はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                            GUI.MainForm.PopupMenu(GUI.MainForm.mnuUnitCommand, 6, GUI.MouseX, GUI.MouseY - 6f);
                        }

                        break;
                    }

                case "移動後コマンド選択":
                    {
                        Event_Renamed.SelectedUnitForEvent = SelectedUnit;
                        {
                            var withBlock14 = SelectedUnit;
                            // 移動時にＥＮを消費している場合はステータスウィンドウを更新
                            // MOD START MARGE
                            // If MainWidth = 15 Then
                            if (!GUI.NewGUIMode)
                            {
                                // MOD END MARGE
                                if (PrevUnitEN != withBlock14.EN)
                                {
                                    Status.DisplayUnitStatus(ref SelectedUnit);
                                }
                            }

                            {
                                var withBlock15 = GUI.MainForm;
                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                withBlock15.mnuUnitCommandItem(WaitCmdID).Visible = true;
                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                withBlock15.mnuUnitCommandItem(MoveCmdID).Visible = false;
                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                withBlock15.mnuUnitCommandItem(TeleportCmdID).Visible = false;
                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                withBlock15.mnuUnitCommandItem(JumpCmdID).Visible = false;
                            }

                            // 会話コマンド
                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                            GUI.MainForm.mnuUnitCommandItem(TalkCmdID).Visible = false;
                            for (i = 1; i <= 4; i++)
                            {
                                // UPGRADE_NOTE: オブジェクト u をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                                u = null;
                                switch (i)
                                {
                                    case 1:
                                        {
                                            if (withBlock14.x > 1)
                                            {
                                                u = Map.MapDataForUnit[withBlock14.x - 1, withBlock14.y];
                                            }

                                            break;
                                        }

                                    case 2:
                                        {
                                            if (withBlock14.x < Map.MapWidth)
                                            {
                                                u = Map.MapDataForUnit[withBlock14.x + 1, withBlock14.y];
                                            }

                                            break;
                                        }

                                    case 3:
                                        {
                                            if (withBlock14.y > 1)
                                            {
                                                u = Map.MapDataForUnit[withBlock14.x, withBlock14.y - 1];
                                            }

                                            break;
                                        }

                                    case 4:
                                        {
                                            if (withBlock14.y < Map.MapHeight)
                                            {
                                                u = Map.MapDataForUnit[withBlock14.x, withBlock14.y + 1];
                                            }

                                            break;
                                        }
                                }

                                if (u is object)
                                {
                                    string arglname2 = "会話 " + withBlock14.MainPilot().ID + " " + u.MainPilot().ID;
                                    if (Event_Renamed.IsEventDefined(ref arglname2))
                                    {
                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                        GUI.MainForm.mnuUnitCommandItem(TalkCmdID).Visible = true;
                                        break;
                                    }
                                }
                            }

                            // 攻撃コマンド
                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                            GUI.MainForm.mnuUnitCommandItem(AttackCmdID).Caption = "攻撃";
                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                            GUI.MainForm.mnuUnitCommandItem(AttackCmdID).Visible = false;
                            var loopTo28 = withBlock14.CountWeapon();
                            for (i = 1; i <= loopTo28; i++)
                            {
                                string argref_mode6 = "移動後";
                                if (withBlock14.IsWeaponUseful(i, ref argref_mode6))
                                {
                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    GUI.MainForm.mnuUnitCommandItem(AttackCmdID).Visible = true;
                                    break;
                                }
                            }

                            if (withBlock14.Area == "地中")
                            {
                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                GUI.MainForm.mnuUnitCommandItem(AttackCmdID).Visible = false;
                            }

                            object argIndex92 = "攻撃不能";
                            if (withBlock14.IsConditionSatisfied(ref argIndex92))
                            {
                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                GUI.MainForm.mnuUnitCommandItem(AttackCmdID).Visible = false;
                            }

                            // 修理コマンド
                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                            GUI.MainForm.mnuUnitCommandItem(FixCmdID).Visible = false;
                            string argfname31 = "修理装置";
                            if (withBlock14.IsFeatureAvailable(ref argfname31) & withBlock14.Area != "地中")
                            {
                                for (i = 1; i <= 4; i++)
                                {
                                    // UPGRADE_NOTE: オブジェクト u をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                                    u = null;
                                    switch (i)
                                    {
                                        case 1:
                                            {
                                                if (withBlock14.x > 1)
                                                {
                                                    u = Map.MapDataForUnit[withBlock14.x - 1, withBlock14.y];
                                                }

                                                break;
                                            }

                                        case 2:
                                            {
                                                if (withBlock14.x < Map.MapWidth)
                                                {
                                                    u = Map.MapDataForUnit[withBlock14.x + 1, withBlock14.y];
                                                }

                                                break;
                                            }

                                        case 3:
                                            {
                                                if (withBlock14.y > 1)
                                                {
                                                    u = Map.MapDataForUnit[withBlock14.x, withBlock14.y - 1];
                                                }

                                                break;
                                            }

                                        case 4:
                                            {
                                                if (withBlock14.y < Map.MapHeight)
                                                {
                                                    u = Map.MapDataForUnit[withBlock14.x, withBlock14.y + 1];
                                                }

                                                break;
                                            }
                                    }

                                    if (u is object)
                                    {
                                        {
                                            var withBlock16 = u;
                                            if ((withBlock16.Party == "味方" | withBlock16.Party == "ＮＰＣ") & withBlock16.HP < withBlock16.MaxHP)
                                            {
                                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                                GUI.MainForm.mnuUnitCommandItem(FixCmdID).Visible = true;
                                                break;
                                            }
                                        }
                                    }
                                }

                                object argIndex94 = "修理装置";
                                if (Strings.Len(withBlock14.FeatureData(ref argIndex94)) > 0)
                                {
                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    object argIndex93 = "修理装置";
                                    string arglist23 = withBlock14.FeatureData(ref argIndex93);
                                    GUI.MainForm.mnuUnitCommandItem(FixCmdID).Caption = GeneralLib.LIndex(ref arglist23, 1);
                                    string localLIndex19() { object argIndex1 = "修理装置"; string arglist = withBlock14.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                                    if (Information.IsNumeric(localLIndex19()))
                                    {
                                        string localLIndex17() { object argIndex1 = "修理装置"; string arglist = withBlock14.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                                        string localLIndex18() { object argIndex1 = "修理装置"; string arglist = withBlock14.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                                        if (withBlock14.EN < Conversions.ToShort(localLIndex18()))
                                        {
                                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                            GUI.MainForm.mnuUnitCommandItem(FixCmdID).Visible = false;
                                        }
                                    }
                                }
                                else
                                {
                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    GUI.MainForm.mnuUnitCommandItem(FixCmdID).Caption = "修理装置";
                                }
                            }

                            // 補給コマンド
                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                            GUI.MainForm.mnuUnitCommandItem(SupplyCmdID).Visible = false;
                            string argfname32 = "補給装置";
                            if (withBlock14.IsFeatureAvailable(ref argfname32) & withBlock14.Area != "地中")
                            {
                                for (i = 1; i <= 4; i++)
                                {
                                    // UPGRADE_NOTE: オブジェクト u をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                                    u = null;
                                    switch (i)
                                    {
                                        case 1:
                                            {
                                                if (withBlock14.x > 1)
                                                {
                                                    u = Map.MapDataForUnit[withBlock14.x - 1, withBlock14.y];
                                                }

                                                break;
                                            }

                                        case 2:
                                            {
                                                if (withBlock14.x < Map.MapWidth)
                                                {
                                                    u = Map.MapDataForUnit[withBlock14.x + 1, withBlock14.y];
                                                }

                                                break;
                                            }

                                        case 3:
                                            {
                                                if (withBlock14.y > 1)
                                                {
                                                    u = Map.MapDataForUnit[withBlock14.x, withBlock14.y - 1];
                                                }

                                                break;
                                            }

                                        case 4:
                                            {
                                                if (withBlock14.y < Map.MapHeight)
                                                {
                                                    u = Map.MapDataForUnit[withBlock14.x, withBlock14.y + 1];
                                                }

                                                break;
                                            }
                                    }

                                    if (u is object)
                                    {
                                        {
                                            var withBlock17 = u;
                                            if (withBlock17.Party == "味方" | withBlock17.Party == "ＮＰＣ")
                                            {
                                                if (withBlock17.EN < withBlock17.MaxEN)
                                                {
                                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                                    GUI.MainForm.mnuUnitCommandItem(SupplyCmdID).Visible = true;
                                                }
                                                else
                                                {
                                                    var loopTo29 = withBlock17.CountWeapon();
                                                    for (j = 1; j <= loopTo29; j++)
                                                    {
                                                        if (withBlock17.Bullet(j) < withBlock17.MaxBullet(j))
                                                        {
                                                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                                            GUI.MainForm.mnuUnitCommandItem(SupplyCmdID).Visible = true;
                                                            break;
                                                        }
                                                    }

                                                    var loopTo30 = withBlock17.CountAbility();
                                                    for (j = 1; j <= loopTo30; j++)
                                                    {
                                                        if (withBlock17.Stock(j) < withBlock17.MaxStock(j))
                                                        {
                                                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                                            GUI.MainForm.mnuUnitCommandItem(SupplyCmdID).Visible = true;
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                object argIndex96 = "補給装置";
                                if (Strings.Len(withBlock14.FeatureData(ref argIndex96)) > 0)
                                {
                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    object argIndex95 = "補給装置";
                                    string arglist24 = withBlock14.FeatureData(ref argIndex95);
                                    GUI.MainForm.mnuUnitCommandItem(SupplyCmdID).Caption = GeneralLib.LIndex(ref arglist24, 1);
                                    string localLIndex22() { object argIndex1 = "補給装置"; string arglist = withBlock14.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                                    if (Information.IsNumeric(localLIndex22()))
                                    {
                                        string localLIndex20() { object argIndex1 = "補給装置"; string arglist = withBlock14.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                                        string localLIndex21() { object argIndex1 = "補給装置"; string arglist = withBlock14.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                                        if (withBlock14.EN < Conversions.ToShort(localLIndex21()) | withBlock14.MainPilot().Morale < 100)
                                        {
                                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                            GUI.MainForm.mnuUnitCommandItem(SupplyCmdID).Visible = false;
                                        }
                                    }
                                }
                                else
                                {
                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    GUI.MainForm.mnuUnitCommandItem(SupplyCmdID).Caption = "補給装置";
                                }

                                string argoname9 = "移動後補給不可";
                                string argsname = "補給";
                                if (Expression.IsOptionDefined(ref argoname9) & !SelectedUnit.MainPilot().IsSkillAvailable(ref argsname))
                                {
                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    GUI.MainForm.mnuUnitCommandItem(SupplyCmdID).Visible = false;
                                }
                            }

                            // アビリティコマンド
                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                            GUI.MainForm.mnuUnitCommandItem(AbilityCmdID).Visible = false;
                            n = 0;
                            var loopTo31 = withBlock14.CountAbility();
                            for (i = 1; i <= loopTo31; i++)
                            {
                                if (!withBlock14.Ability(i).IsItem())
                                {
                                    n = (short)(n + 1);
                                    string argref_mode7 = "移動後";
                                    if (withBlock14.IsAbilityUseful(i, ref argref_mode7))
                                    {
                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                        GUI.MainForm.mnuUnitCommandItem(AbilityCmdID).Visible = true;
                                    }
                                }
                            }

                            if (withBlock14.Area == "地中")
                            {
                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                GUI.MainForm.mnuUnitCommandItem(AbilityCmdID).Visible = false;
                            }
                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                            string argtname4 = "アビリティ";
                            GUI.MainForm.mnuUnitCommandItem(AbilityCmdID).Caption = Expression.Term(ref argtname4, ref SelectedUnit);
                            if (n == 1)
                            {
                                var loopTo32 = withBlock14.CountAbility();
                                for (i = 1; i <= loopTo32; i++)
                                {
                                    if (!withBlock14.Ability(i).IsItem())
                                    {
                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                        GUI.MainForm.mnuUnitCommandItem(AbilityCmdID).Caption = withBlock14.AbilityNickname(i);
                                        break;
                                    }
                                }
                            }

                            {
                                var withBlock18 = GUI.MainForm;
                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                withBlock18.mnuUnitCommandItem(ChargeCmdID).Visible = false;
                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                withBlock18.mnuUnitCommandItem(SpecialPowerCmdID).Visible = false;
                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                withBlock18.mnuUnitCommandItem(TransformCmdID).Visible = false;
                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                withBlock18.mnuUnitCommandItem(SplitCmdID).Visible = false;
                            }

                            // 合体コマンド
                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                            GUI.MainForm.mnuUnitCommandItem(CombineCmdID).Visible = false;
                            string argfname34 = "合体";
                            object argIndex100 = "形態固定";
                            object argIndex101 = "機体固定";
                            if (withBlock14.IsFeatureAvailable(ref argfname34) & !withBlock14.IsConditionSatisfied(ref argIndex100) & !withBlock14.IsConditionSatisfied(ref argIndex101))
                            {
                                var loopTo33 = withBlock14.CountFeature();
                                for (i = 1; i <= loopTo33; i++)
                                {
                                    // 3体以上からなる合体能力を持っているか？
                                    string localFeature1() { object argIndex1 = i; var ret = withBlock14.Feature(ref argIndex1); return ret; }

                                    string localFeatureName1() { object argIndex1 = i; var ret = withBlock14.FeatureName(ref argIndex1); return ret; }

                                    string localFeatureData17() { object argIndex1 = i; var ret = withBlock14.FeatureData(ref argIndex1); return ret; }

                                    short localLLength4() { string arglist = hs9469e7ffcaeb496ca82716c7891638cc(); var ret = GeneralLib.LLength(ref arglist); return ret; }

                                    if (localFeature1() == "合体" & !string.IsNullOrEmpty(localFeatureName1()) & localLLength4() > 3)
                                    {
                                        n = 0;
                                        string localFeatureData13() { object argIndex1 = i; var ret = withBlock14.FeatureData(ref argIndex1); return ret; }

                                        string arglist25 = localFeatureData13();
                                        var loopTo34 = GeneralLib.LLength(ref arglist25);
                                        for (j = 3; j <= loopTo34; j++)
                                        {
                                            string localFeatureData12() { object argIndex1 = i; var ret = withBlock14.FeatureData(ref argIndex1); return ret; }

                                            string localLIndex23() { string arglist = hs175f067af849438ea2ce369fbd24d08f(); var ret = GeneralLib.LIndex(ref arglist, j); return ret; }

                                            object argIndex97 = localLIndex23();
                                            u = SRC.UList.Item(ref argIndex97);
                                            if (u is null)
                                            {
                                                break;
                                            }

                                            if (!u.IsOperational())
                                            {
                                                break;
                                            }

                                            string argfname33 = "合体制限";
                                            if (u.Status_Renamed != "出撃" & u.CurrentForm().IsFeatureAvailable(ref argfname33))
                                            {
                                                break;
                                            }

                                            if (Math.Abs((short)(withBlock14.x - u.CurrentForm().x)) + Math.Abs((short)(withBlock14.y - u.CurrentForm().y)) > 2)
                                            {
                                                break;
                                            }

                                            n = (short)(n + 1);
                                        }

                                        string localFeatureData14() { object argIndex1 = i; var ret = withBlock14.FeatureData(ref argIndex1); return ret; }

                                        string arglist26 = localFeatureData14();
                                        uname = GeneralLib.LIndex(ref arglist26, 2);
                                        object argIndex98 = uname;
                                        u = SRC.UList.Item(ref argIndex98);
                                        object argIndex99 = "行動不能";
                                        if (u is null)
                                        {
                                            n = 0;
                                        }
                                        else if (u.IsConditionSatisfied(ref argIndex99))
                                        {
                                            n = 0;
                                        }

                                        string localFeatureData16() { object argIndex1 = i; var ret = withBlock14.FeatureData(ref argIndex1); return ret; }

                                        short localLLength3() { string arglist = hse55df985d6054cfe94b90350a9c471f6(); var ret = GeneralLib.LLength(ref arglist); return ret; }

                                        if (n == localLLength3() - 2)
                                        {
                                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                            GUI.MainForm.mnuUnitCommandItem(CombineCmdID).Visible = true;
                                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                            string localFeatureData15() { object argIndex1 = i; var ret = withBlock14.FeatureData(ref argIndex1); return ret; }

                                            string arglist27 = localFeatureData15();
                                            GUI.MainForm.mnuUnitCommandItem(CombineCmdID).Caption = GeneralLib.LIndex(ref arglist27, 1);
                                            break;
                                        }
                                    }
                                }
                            }

                            {
                                var withBlock19 = GUI.MainForm;
                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                withBlock19.mnuUnitCommandItem(HyperModeCmdID).Visible = false;
                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                withBlock19.mnuUnitCommandItem(GroundCmdID).Visible = false;
                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                withBlock19.mnuUnitCommandItem(SkyCmdID).Visible = false;
                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                withBlock19.mnuUnitCommandItem(UndergroundCmdID).Visible = false;
                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                withBlock19.mnuUnitCommandItem(WaterCmdID).Visible = false;
                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                withBlock19.mnuUnitCommandItem(LaunchCmdID).Visible = false;
                            }

                            // アイテムコマンド
                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                            GUI.MainForm.mnuUnitCommandItem(ItemCmdID).Visible = false;
                            var loopTo35 = withBlock14.CountAbility();
                            for (i = 1; i <= loopTo35; i++)
                            {
                                string argref_mode8 = "移動後";
                                if (withBlock14.IsAbilityUseful(i, ref argref_mode8) & withBlock14.Ability(i).IsItem())
                                {
                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    GUI.MainForm.mnuUnitCommandItem(ItemCmdID).Visible = true;
                                    break;
                                }
                            }

                            if (withBlock14.Area == "地中")
                            {
                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                GUI.MainForm.mnuUnitCommandItem(ItemCmdID).Visible = false;
                            }

                            {
                                var withBlock20 = GUI.MainForm;
                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                withBlock20.mnuUnitCommandItem(DismissCmdID).Visible = false;
                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                withBlock20.mnuUnitCommandItem(OrderCmdID).Visible = false;
                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                withBlock20.mnuUnitCommandItem(FeatureListCmdID).Visible = false;
                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                withBlock20.mnuUnitCommandItem(WeaponListCmdID).Visible = false;
                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                withBlock20.mnuUnitCommandItem(AbilityListCmdID).Visible = false;
                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                withBlock20.mnuUnitCommandItem(UnitCommand1CmdID).Visible = false;
                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                withBlock20.mnuUnitCommandItem(UnitCommand2CmdID).Visible = false;
                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                withBlock20.mnuUnitCommandItem(UnitCommand3CmdID).Visible = false;
                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                withBlock20.mnuUnitCommandItem(UnitCommand4CmdID).Visible = false;
                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                withBlock20.mnuUnitCommandItem(UnitCommand5CmdID).Visible = false;
                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                withBlock20.mnuUnitCommandItem(UnitCommand6CmdID).Visible = false;
                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                withBlock20.mnuUnitCommandItem(UnitCommand7CmdID).Visible = false;
                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                withBlock20.mnuUnitCommandItem(UnitCommand8CmdID).Visible = false;
                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                withBlock20.mnuUnitCommandItem(UnitCommand9CmdID).Visible = false;
                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                withBlock20.mnuUnitCommandItem(UnitCommand10CmdID).Visible = false;
                            }

                            // ユニットコマンド
                            i = UnitCommand1CmdID;
                            foreach (LabelData currentLab4 in Event_Renamed.colEventLabelList)
                            {
                                lab = currentLab4;
                                if (lab.Name == Event_Renamed.LabelType.UnitCommandEventLabel & lab.AsterNum >= 2)
                                {
                                    if (lab.Enable)
                                    {
                                        buf = lab.Para(3);
                                        if (SelectedUnit.Party == "味方" & ((buf ?? "") == (SelectedUnit.MainPilot().Name ?? "") | (buf ?? "") == (SelectedUnit.MainPilot().get_Nickname(false) ?? "") | (buf ?? "") == (SelectedUnit.Name ?? "")) | (buf ?? "") == (SelectedUnit.Party ?? "") | buf == "全")
                                        {
                                            int localStrToLng3() { string argexpr = lab.Para(4); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

                                            if (lab.CountPara() <= 3)
                                            {
                                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                                GUI.MainForm.mnuUnitCommandItem(i).Visible = true;
                                            }
                                            else if (localStrToLng3() != 0)
                                            {
                                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                                GUI.MainForm.mnuUnitCommandItem(i).Visible = true;
                                            }
                                        }
                                    }

                                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    if (GUI.MainForm.mnuUnitCommandItem(i).Visible)
                                    {
                                        // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                        GUI.MainForm.mnuUnitCommandItem(i).Caption = lab.Para(2);
                                        UnitCommandLabelList[i - UnitCommand1CmdID + 1] = lab.LineNum.ToString();
                                        i = (short)(i + 1);
                                        if (i > UnitCommand10CmdID)
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        GUI.IsGUILocked = false;
                        if (by_cancel)
                        {
                            // UPGRADE_ISSUE: Control mnuUnitCommand は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                            // UPGRADE_ISSUE: Form メソッド MainForm.PopupMenu はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                            GUI.MainForm.PopupMenu(GUI.MainForm.mnuUnitCommand, 6, GUI.MouseX, GUI.MouseY + 5f);
                        }
                        else
                        {
                            // UPGRADE_ISSUE: Control mnuUnitCommand は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                            // UPGRADE_ISSUE: Form メソッド MainForm.PopupMenu はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                            GUI.MainForm.PopupMenu(GUI.MainForm.mnuUnitCommand, 6, GUI.MouseX, GUI.MouseY - 6f);
                            Application.DoEvents();
                            // ＰＣに負荷がかかったような状態だとポップアップメニューの選択が
                            // うまく行えない場合があるのでやり直す
                            while (CommandState == "移動後コマンド選択" & SelectedCommand == "移動")
                            {
                                // UPGRADE_ISSUE: Control mnuUnitCommand は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                // UPGRADE_ISSUE: Form メソッド MainForm.PopupMenu はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                                GUI.MainForm.PopupMenu(GUI.MainForm.mnuUnitCommand, 6, GUI.MouseX, GUI.MouseY - 6f);
                                Application.DoEvents();
                            }
                        }

                        break;
                    }

                case "ターゲット選択":
                case "移動後ターゲット選択":
                    {
                        if (!Map.MaskData[GUI.PixelToMapX((short)GUI.MouseX), GUI.PixelToMapY((short)GUI.MouseY)])
                        {
                            SelectedX = GUI.PixelToMapX((short)GUI.MouseX);
                            SelectedY = GUI.PixelToMapY((short)GUI.MouseY);

                            // 自分自身を選択された場合
                            if (SelectedUnit.x == SelectedX & SelectedUnit.y == SelectedY)
                            {
                                if (SelectedCommand == "スペシャルパワー")
                                {
                                }
                                // 下に抜ける
                                else if (SelectedCommand == "アビリティ" | SelectedCommand == "マップアビリティ" | SelectedCommand == "アイテム" | SelectedCommand == "マップアイテム")
                                {
                                    if (SelectedUnit.AbilityMinRange(SelectedAbility) > 0)
                                    {
                                        // 自分自身は選択不可
                                        GUI.IsGUILocked = false;
                                        return;
                                    }
                                }
                                else if (SelectedCommand == "移動命令")
                                {
                                }
                                // 下に抜ける
                                else
                                {
                                    // 自分自身は選択不可
                                    GUI.IsGUILocked = false;
                                    return;
                                }
                            }

                            // 場所を選択するコマンド
                            switch (SelectedCommand ?? "")
                            {
                                // MOD START MARGE
                                // Case "移動"
                                case "移動":
                                case "再移動":
                                    {
                                        // MOD END MARGE
                                        FinishMoveCommand();
                                        GUI.IsGUILocked = false;
                                        return;
                                    }

                                case "テレポート":
                                    {
                                        FinishTeleportCommand();
                                        GUI.IsGUILocked = false;
                                        return;
                                    }

                                case "ジャンプ":
                                    {
                                        FinishJumpCommand();
                                        GUI.IsGUILocked = false;
                                        return;
                                    }

                                case "マップ攻撃":
                                    {
                                        MapAttackCommand();
                                        GUI.IsGUILocked = false;
                                        return;
                                    }

                                case "マップアビリティ":
                                case "マップアイテム":
                                    {
                                        MapAbilityCommand();
                                        GUI.IsGUILocked = false;
                                        return;
                                    }

                                case "発進":
                                    {
                                        FinishLaunchCommand();
                                        GUI.IsGUILocked = false;
                                        return;
                                    }

                                case "移動命令":
                                    {
                                        FinishOrderCommand();
                                        GUI.IsGUILocked = false;
                                        return;
                                    }
                            }

                            // これ以降はユニットを選択するコマンド

                            // 指定した地点にユニットがいる？
                            if (Map.MapDataForUnit[SelectedX, SelectedY] is null)
                            {
                                GUI.IsGUILocked = false;
                                return;
                            }

                            // ターゲットを選択
                            SelectedTarget = Map.MapDataForUnit[SelectedX, SelectedY];
                            switch (SelectedCommand ?? "")
                            {
                                case "攻撃":
                                    {
                                        FinishAttackCommand();
                                        break;
                                    }

                                case "アビリティ":
                                case "アイテム":
                                    {
                                        FinishAbilityCommand();
                                        break;
                                    }

                                case "会話":
                                    {
                                        FinishTalkCommand();
                                        break;
                                    }

                                case "修理":
                                    {
                                        FinishFixCommand();
                                        break;
                                    }

                                case "補給":
                                    {
                                        FinishSupplyCommand();
                                        break;
                                    }

                                case "スペシャルパワー":
                                    {
                                        FinishSpecialPowerCommand();
                                        break;
                                    }

                                case "攻撃命令":
                                case "護衛命令":
                                    {
                                        FinishOrderCommand();
                                        break;
                                    }
                            }
                        }

                        break;
                    }

                case "マップ攻撃使用":
                case "移動後マップ攻撃使用":
                    {
                        if (1 <= GUI.PixelToMapX((short)GUI.MouseX) & GUI.PixelToMapX((short)GUI.MouseX) <= Map.MapWidth)
                        {
                            if (1 <= GUI.PixelToMapY((short)GUI.MouseY) & GUI.PixelToMapY((short)GUI.MouseY) <= Map.MapHeight)
                            {
                                if (!Map.MaskData[GUI.PixelToMapX((short)GUI.MouseX), GUI.PixelToMapY((short)GUI.MouseY)])
                                {
                                    // 効果範囲内でクリックされればマップ攻撃発動
                                    if (SelectedCommand == "マップ攻撃")
                                    {
                                        MapAttackCommand();
                                    }
                                    else
                                    {
                                        MapAbilityCommand();
                                    }
                                }
                            }
                        }

                        break;
                    }
            }

            GUI.IsGUILocked = false;
        }

        // ＧＵＩの処理をキャンセル
        public static void CancelCommand()
        {
            short tmp_x, tmp_y;
            {
                var withBlock = SelectedUnit;
                switch (CommandState ?? "")
                {
                    case "ユニット選択":
                        {
                            break;
                        }

                    case "コマンド選択":
                        {
                            CommandState = "ユニット選択";
                            // ADD START
                            // 選択したコマンドを初期化
                            SelectedCommand = "";
                            // MOD START MARGE
                            // If MainWidth <> 15 Then
                            if (GUI.NewGUIMode)
                            {
                                // MOD  END  MARGE
                                Status.ClearUnitStatus();
                            }

                            break;
                        }

                    case "ターゲット選択":
                        {
                            // ADD START MARGE
                            if (SelectedCommand == "再移動")
                            {
                                WaitCommand();
                                return;
                            }
                            // ADD END MARGE
                            CommandState = "コマンド選択";
                            Status.DisplayUnitStatus(ref SelectedUnit);
                            GUI.RedrawScreen();
                            ProceedCommand(true);
                            break;
                        }

                    case "移動後コマンド選択":
                        {
                            CommandState = "ターゲット選択";
                            withBlock.Area = PrevUnitArea;
                            withBlock.Move(PrevUnitX, PrevUnitY, true, true);
                            withBlock.EN = PrevUnitEN;
                            if (!ReferenceEquals(SelectedUnit, Map.MapDataForUnit[PrevUnitX, PrevUnitY]))
                            {
                                // 発進をキャンセルした場合
                                SelectedTarget = SelectedUnit;
                                GUI.PaintUnitBitmap(ref SelectedTarget);
                                SelectedUnit = Map.MapDataForUnit[PrevUnitX, PrevUnitY];
                            }
                            // MOD START MARGE
                            // ElseIf MainWidth = 15 Then
                            else if (!GUI.NewGUIMode)
                            {
                                // MOD END MARGE
                                Status.DisplayUnitStatus(ref SelectedUnit);
                            }
                            // MOD START MARGE
                            // 移動後コマンドをキャンセルした場合、MoveCostを0にリセットする
                            SelectedUnitMoveCost = 0;
                            // MOD END MARGE
                            switch (SelectedCommand ?? "")
                            {
                                case "移動":
                                    {
                                        StartMoveCommand();
                                        break;
                                    }

                                case "テレポート":
                                    {
                                        StartTeleportCommand();
                                        break;
                                    }

                                case "ジャンプ":
                                    {
                                        StartJumpCommand();
                                        break;
                                    }

                                case "発進":
                                    {
                                        GUI.PaintUnitBitmap(ref SelectedTarget);
                                        break;
                                    }
                            }

                            break;
                        }

                    case "移動後ターゲット選択":
                        {
                            CommandState = "移動後コマンド選択";
                            Status.DisplayUnitStatus(ref SelectedUnit);
                            tmp_x = withBlock.x;
                            tmp_y = withBlock.y;
                            withBlock.x = PrevUnitX;
                            withBlock.y = PrevUnitY;
                            switch (PrevCommand ?? "")
                            {
                                case "移動":
                                    {
                                        Map.AreaInSpeed(ref SelectedUnit);
                                        break;
                                    }

                                case "テレポート":
                                    {
                                        Map.AreaInTeleport(ref SelectedUnit);
                                        break;
                                    }

                                case "ジャンプ":
                                    {
                                        Map.AreaInSpeed(ref SelectedUnit, true);
                                        break;
                                    }

                                case "発進":
                                    {
                                        {
                                            var withBlock1 = SelectedTarget;
                                            string localLIndex() { object argIndex1 = "テレポート"; string arglist = withBlock1.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                                            short localLLength() { object argIndex1 = "ジャンプ"; string arglist = withBlock1.FeatureData(ref argIndex1); var ret = GeneralLib.LLength(ref arglist); return ret; }

                                            string localLIndex1() { object argIndex1 = "ジャンプ"; string arglist = withBlock1.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                                            string argfname = "テレポート";
                                            string argfname1 = "ジャンプ";
                                            if (withBlock1.IsFeatureAvailable(ref argfname) & (withBlock1.Data.Speed == 0 | localLIndex() == "0"))
                                            {
                                                Map.AreaInTeleport(ref SelectedTarget);
                                            }
                                            else if (withBlock1.IsFeatureAvailable(ref argfname1) & (withBlock1.Data.Speed == 0 | localLLength() < 2 | localLIndex1() == "0"))
                                            {
                                                Map.AreaInSpeed(ref SelectedTarget, true);
                                            }
                                            else
                                            {
                                                Map.AreaInSpeed(ref SelectedTarget);
                                            }
                                        }

                                        break;
                                    }
                            }

                            withBlock.x = tmp_x;
                            withBlock.y = tmp_y;
                            SelectedCommand = PrevCommand;
                            Map.MaskData[tmp_x, tmp_y] = false;
                            GUI.MaskScreen();
                            ProceedCommand(true);
                            break;
                        }

                    case "マップ攻撃使用":
                    case "移動後マップ攻撃使用":
                        {
                            if (CommandState == "マップ攻撃使用")
                            {
                                CommandState = "ターゲット選択";
                            }
                            else
                            {
                                CommandState = "移動後ターゲット選択";
                            }

                            if (SelectedCommand == "マップ攻撃")
                            {
                                string argattr = "Ｍ直";
                                string argattr1 = "Ｍ移";
                                if (withBlock.IsWeaponClassifiedAs(SelectedWeapon, ref argattr))
                                {
                                    Map.AreaInCross(withBlock.x, withBlock.y, withBlock.WeaponMaxRange(SelectedWeapon), ref withBlock.Weapon(SelectedWeapon).MinRange);
                                }
                                else if (withBlock.IsWeaponClassifiedAs(SelectedWeapon, ref argattr1))
                                {
                                    Map.AreaInMoveAction(ref SelectedUnit, withBlock.WeaponMaxRange(SelectedWeapon));
                                }
                                else
                                {
                                    string arguparty = "すべて";
                                    Map.AreaInRange(withBlock.x, withBlock.y, withBlock.WeaponMaxRange(SelectedWeapon), withBlock.Weapon(SelectedWeapon).MinRange, ref arguparty);
                                }
                            }
                            else
                            {
                                string argattr2 = "Ｍ直";
                                string argattr3 = "Ｍ移";
                                if (withBlock.IsAbilityClassifiedAs(SelectedAbility, ref argattr2))
                                {
                                    short argmax_range = withBlock.AbilityMinRange(SelectedAbility);
                                    Map.AreaInCross(withBlock.x, withBlock.y, withBlock.AbilityMaxRange(SelectedAbility), ref argmax_range);
                                }
                                else if (withBlock.IsAbilityClassifiedAs(SelectedAbility, ref argattr3))
                                {
                                    Map.AreaInMoveAction(ref SelectedUnit, withBlock.AbilityMaxRange(SelectedAbility));
                                }
                                else
                                {
                                    string arguparty1 = "すべて";
                                    Map.AreaInRange(withBlock.x, withBlock.y, withBlock.AbilityMaxRange(SelectedAbility), withBlock.AbilityMinRange(SelectedAbility), ref arguparty1);
                                }
                            }

                            GUI.MaskScreen();
                            break;
                        }
                }
            }
        }


        // ユニットコマンドを実行
        public static void UnitCommand(short idx)
        {
            short prev_used_action;
            PrevCommand = SelectedCommand;
            {
                var withBlock = SelectedUnit;
                prev_used_action = withBlock.UsedAction;
                switch (idx)
                {
                    case MoveCmdID: // 移動
                        {
                            // なんらかの原因により、ユニットコマンドの選択がうまくいかなかった場合は
                            // 移動後のコマンド選択をやり直す
                            if (CommandState == "移動後コマンド選択")
                            {
                                Application.DoEvents();
                                return;
                            }

                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                            if (GUI.MainForm.mnuUnitCommandItem.Item(MoveCmdID).Caption == "移動")
                            {
                                StartMoveCommand();
                            }
                            else
                            {
                                ShowAreaInSpeedCommand();
                            }

                            break;
                        }

                    case TeleportCmdID: // テレポート
                        {
                            StartTeleportCommand();
                            break;
                        }

                    case JumpCmdID: // ジャンプ
                        {
                            StartJumpCommand();
                            break;
                        }

                    case TalkCmdID: // 会話
                        {
                            StartTalkCommand();
                            break;
                        }

                    case AttackCmdID: // 攻撃
                        {
                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                            if (GUI.MainForm.mnuUnitCommandItem.Item(AttackCmdID).Caption == "攻撃")
                            {
                                StartAttackCommand();
                            }
                            else
                            {
                                ShowAreaInRangeCommand();
                            }

                            break;
                        }

                    case FixCmdID: // 修理
                        {
                            StartFixCommand();
                            break;
                        }

                    case SupplyCmdID: // 補給
                        {
                            StartSupplyCommand();
                            break;
                        }

                    case AbilityCmdID: // アビリティ
                        {
                            StartAbilityCommand();
                            break;
                        }

                    case ChargeCmdID: // チャージ
                        {
                            ChargeCommand();
                            break;
                        }

                    case SpecialPowerCmdID: // 精神
                        {
                            StartSpecialPowerCommand();
                            break;
                        }

                    case TransformCmdID: // 変形
                        {
                            TransformCommand();
                            break;
                        }

                    case SplitCmdID: // 分離
                        {
                            SplitCommand();
                            break;
                        }

                    case CombineCmdID: // 合体
                        {
                            CombineCommand();
                            break;
                        }

                    case HyperModeCmdID: // ハイパーモード・変身解除
                        {
                            object argIndex1 = "ノーマルモード";
                            if (Strings.InStr(withBlock.FeatureData(ref argIndex1), "手動解除") > 0)
                            {
                                CancelTransformationCommand();
                            }
                            else
                            {
                                HyperModeCommand();
                            }

                            break;
                        }

                    case GroundCmdID: // 地上
                        {
                            GUI.LockGUI();
                            if (Map.TerrainClass(withBlock.x, withBlock.y) == "水" | Map.TerrainClass(withBlock.x, withBlock.y) == "深水")
                            {
                                withBlock.Area = "水上";
                            }
                            else
                            {
                                withBlock.Area = "地上";
                            }

                            withBlock.Update();
                            if (withBlock.IsMessageDefined(ref withBlock.Area))
                            {
                                Unit argu1 = null;
                                Unit argu2 = null;
                                GUI.OpenMessageForm(u1: ref argu1, u2: ref argu2);
                                string argmsg_mode = "";
                                withBlock.PilotMessage(ref withBlock.Area, msg_mode: ref argmsg_mode);
                                GUI.CloseMessageForm();
                            }

                            GUI.PaintUnitBitmap(ref SelectedUnit);
                            CommandState = "ユニット選択";
                            GUI.UnlockGUI();
                            break;
                        }

                    case SkyCmdID: // 空中
                        {
                            GUI.LockGUI();
                            if (Map.TerrainClass(withBlock.x, withBlock.y) == "月面")
                            {
                                withBlock.Area = "宇宙";
                            }
                            else
                            {
                                withBlock.Area = "空中";
                            }

                            withBlock.Update();
                            if (withBlock.IsMessageDefined(ref withBlock.Area))
                            {
                                Unit argu11 = null;
                                Unit argu21 = null;
                                GUI.OpenMessageForm(u1: ref argu11, u2: ref argu21);
                                string argmsg_mode1 = "";
                                withBlock.PilotMessage(ref withBlock.Area, msg_mode: ref argmsg_mode1);
                                GUI.CloseMessageForm();
                            }

                            GUI.PaintUnitBitmap(ref SelectedUnit);
                            CommandState = "ユニット選択";
                            GUI.UnlockGUI();
                            break;
                        }

                    case UndergroundCmdID: // 地中
                        {
                            GUI.LockGUI();
                            withBlock.Area = "地中";
                            withBlock.Update();
                            if (withBlock.IsMessageDefined(ref withBlock.Area))
                            {
                                Unit argu12 = null;
                                Unit argu22 = null;
                                GUI.OpenMessageForm(u1: ref argu12, u2: ref argu22);
                                string argmsg_mode2 = "";
                                withBlock.PilotMessage(ref withBlock.Area, msg_mode: ref argmsg_mode2);
                                GUI.CloseMessageForm();
                            }

                            GUI.PaintUnitBitmap(ref SelectedUnit);
                            CommandState = "ユニット選択";
                            GUI.UnlockGUI();
                            break;
                        }

                    case WaterCmdID: // 水中
                        {
                            GUI.LockGUI();
                            withBlock.Area = "水中";
                            withBlock.Update();
                            if (withBlock.IsMessageDefined(ref withBlock.Area))
                            {
                                Unit argu13 = null;
                                Unit argu23 = null;
                                GUI.OpenMessageForm(u1: ref argu13, u2: ref argu23);
                                string argmsg_mode3 = "";
                                withBlock.PilotMessage(ref withBlock.Area, msg_mode: ref argmsg_mode3);
                                GUI.CloseMessageForm();
                            }

                            GUI.PaintUnitBitmap(ref SelectedUnit);
                            CommandState = "ユニット選択";
                            GUI.UnlockGUI();
                            break;
                        }

                    case LaunchCmdID: // 発進
                        {
                            StartLaunchCommand();
                            break;
                        }

                    case ItemCmdID: // アイテム
                        {
                            StartAbilityCommand(true);
                            break;
                        }

                    case DismissCmdID: // 召喚解除
                        {
                            GUI.LockGUI();

                            // 召喚解除の使用イベント
                            Event_Renamed.HandleEvent("使用", withBlock.MainPilot().ID, "召喚解除");
                            if (SRC.IsScenarioFinished)
                            {
                                SRC.IsScenarioFinished = false;
                                GUI.UnlockGUI();
                                return;
                            }

                            if (SRC.IsCanceled)
                            {
                                SRC.IsCanceled = false;
                                return;
                            }

                            // 召喚ユニットを解放
                            withBlock.DismissServant();

                            // 召喚解除の使用後イベント
                            Event_Renamed.HandleEvent("使用後", withBlock.CurrentForm().MainPilot().ID, "召喚解除");
                            if (SRC.IsScenarioFinished)
                            {
                                SRC.IsScenarioFinished = false;
                            }

                            if (SRC.IsCanceled)
                            {
                                SRC.IsCanceled = false;
                            }

                            GUI.UnlockGUI();
                            break;
                        }

                    case OrderCmdID: // 命令/換装
                        {
                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                            if (GUI.MainForm.mnuUnitCommandItem.Item(OrderCmdID).Caption == "命令")
                            {
                                StartOrderCommand();
                            }
                            else
                            {
                                ExchangeFormCommand();
                            }

                            break;
                        }

                    case FeatureListCmdID: // 特殊能力一覧
                        {
                            FeatureListCommand();
                            break;
                        }

                    case WeaponListCmdID: // 武器一覧
                        {
                            WeaponListCommand();
                            break;
                        }

                    case AbilityListCmdID: // アビリティ一覧
                        {
                            AbilityListCommand();
                            break;
                        }

                    case var @case when UnitCommand1CmdID <= @case && @case <= UnitCommand10CmdID: // ユニットコマンド
                        {
                            GUI.LockGUI();

                            // ユニットコマンドの使用イベント
                            // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                            Event_Renamed.HandleEvent("使用", withBlock.MainPilot().ID, GUI.MainForm.mnuUnitCommandItem.Item(idx).Caption);
                            if (SRC.IsScenarioFinished)
                            {
                                SRC.IsScenarioFinished = false;
                                GUI.UnlockGUI();
                                return;
                            }

                            if (SRC.IsCanceled)
                            {
                                SRC.IsCanceled = false;
                                WaitCommand();
                                return;
                            }

                            // ユニットコマンドを実行
                            Event_Renamed.HandleEvent(UnitCommandLabelList[idx - UnitCommand1CmdID + 1]);
                            if (SRC.IsCanceled)
                            {
                                SRC.IsCanceled = false;
                                CancelCommand();
                                GUI.UnlockGUI();
                                return;
                            }

                            // ユニットコマンドの使用後イベント
                            if (withBlock.CurrentForm().CountPilot() > 0)
                            {
                                // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                Event_Renamed.HandleEvent("使用後", withBlock.CurrentForm().MainPilot().ID, GUI.MainForm.mnuUnitCommandItem.Item(idx).Caption);
                                if (SRC.IsScenarioFinished)
                                {
                                    SRC.IsScenarioFinished = false;
                                    GUI.UnlockGUI();
                                    return;
                                }
                            }

                            // ステータスウィンドウを更新
                            if (withBlock.CurrentForm().CountPilot() > 0)
                            {
                                Status.DisplayUnitStatus(ref withBlock.CurrentForm());
                            }

                            // 行動終了
                            if (withBlock.CurrentForm().UsedAction <= prev_used_action)
                            {
                                if (CommandState == "移動後コマンド選択")
                                {
                                    WaitCommand();
                                }
                                else
                                {
                                    CommandState = "ユニット選択";
                                    GUI.UnlockGUI();
                                }
                            }
                            else if (SRC.IsCanceled)
                            {
                                SRC.IsCanceled = false;
                            }
                            else
                            {
                                WaitCommand(true);
                            }

                            break;
                        }

                    case WaitCmdID: // 待機
                        {
                            WaitCommand();
                            break;
                        }

                    default:
                        {
                            // なんらかの原因により、ユニットコマンドの選択がうまくいかなかった場合は
                            // 移動後のコマンド選択をやり直す
                            if (CommandState == "移動後コマンド選択")
                            {
                                Application.DoEvents();
                                return;
                            }

                            break;
                        }
                }
            }
        }


        // 「移動」コマンドを開始
        // MOD START MARGE
        // Public Sub StartMoveCommand()
        private static void StartMoveCommand()
        {
            // MOD END MARGE
            SelectedCommand = "移動";
            Map.AreaInSpeed(ref SelectedUnit);
            string argoname = "大型マップ";
            if (!Expression.IsOptionDefined(ref argoname))
            {
                GUI.Center(SelectedUnit.x, SelectedUnit.y);
            }

            GUI.MaskScreen();
            // MOD START MARGE
            // If MainWidth <> 15 Then
            if (GUI.NewGUIMode)
            {
                // MOD END MARGE
                Application.DoEvents();
                Status.ClearUnitStatus();
            }

            CommandState = "ターゲット選択";
        }

        // 「移動」コマンドを終了
        // MOD START MARGE
        // Public Sub FinishMoveCommand()
        private static void FinishMoveCommand()
        {
            // MOD END MARGE
            short ret;
            GUI.LockGUI();
            {
                var withBlock = SelectedUnit;
                PrevUnitX = withBlock.x;
                PrevUnitY = withBlock.y;
                PrevUnitArea = withBlock.Area;
                PrevUnitEN = (short)withBlock.EN;

                // 移動後に着艦or合体する場合はプレイヤーに確認を取る
                if (Map.MapDataForUnit[SelectedX, SelectedY] is object)
                {
                    string argfname = "母艦";
                    string argfname1 = "母艦";
                    if (Map.MapDataForUnit[SelectedX, SelectedY].IsFeatureAvailable(ref argfname) & !withBlock.IsFeatureAvailable(ref argfname1))
                    {
                        ret = (short)Interaction.MsgBox("着艦しますか？", (MsgBoxStyle)((int)MsgBoxStyle.OkCancel + (int)MsgBoxStyle.Question), "着艦");
                    }
                    else
                    {
                        ret = (short)Interaction.MsgBox("合体しますか？", (MsgBoxStyle)((int)MsgBoxStyle.OkCancel + (int)MsgBoxStyle.Question), "合体");
                    }

                    if (ret == (int)MsgBoxResult.Cancel)
                    {
                        CancelCommand();
                        GUI.UnlockGUI();
                        return;
                    }
                }

                // ユニットを移動
                withBlock.Move(SelectedX, SelectedY);

                // 移動後に着艦または合体した？
                if (!ReferenceEquals(Map.MapDataForUnit[withBlock.x, withBlock.y], SelectedUnit))
                {
                    string argfname2 = "母艦";
                    string argfname3 = "母艦";
                    if (Map.MapDataForUnit[withBlock.x, withBlock.y].IsFeatureAvailable(ref argfname2) & !withBlock.IsFeatureAvailable(ref argfname3) & withBlock.CountPilot() > 0)
                    {
                        // 着艦メッセージ表示
                        string argmain_situation = "着艦(" + withBlock.Name + ")";
                        string argmain_situation1 = "着艦";
                        if (withBlock.IsMessageDefined(ref argmain_situation))
                        {
                            Unit argu1 = null;
                            Unit argu2 = null;
                            GUI.OpenMessageForm(u1: ref argu1, u2: ref argu2);
                            string argSituation = "着艦(" + withBlock.Name + ")";
                            string argmsg_mode = "";
                            withBlock.PilotMessage(ref argSituation, msg_mode: ref argmsg_mode);
                            GUI.CloseMessageForm();
                        }
                        else if (withBlock.IsMessageDefined(ref argmain_situation1))
                        {
                            Unit argu11 = null;
                            Unit argu21 = null;
                            GUI.OpenMessageForm(u1: ref argu11, u2: ref argu21);
                            string argSituation1 = "着艦";
                            string argmsg_mode1 = "";
                            withBlock.PilotMessage(ref argSituation1, msg_mode: ref argmsg_mode1);
                            GUI.CloseMessageForm();
                        }

                        string argmain_situation2 = "着艦";
                        string argsub_situation = withBlock.Name;
                        withBlock.SpecialEffect(ref argmain_situation2, ref argsub_situation);
                        withBlock.Name = argsub_situation;

                        // 収納イベント
                        SelectedTarget = Map.MapDataForUnit[withBlock.x, withBlock.y];
                        Event_Renamed.HandleEvent("収納", withBlock.MainPilot().ID);
                    }
                    else
                    {
                        // 合体後のユニットを選択
                        SelectedUnit = Map.MapDataForUnit[withBlock.x, withBlock.y];

                        // 合体イベント
                        Event_Renamed.HandleEvent("合体", SelectedUnit.MainPilot().ID, SelectedUnit.Name);
                    }

                    // 移動後の収納・合体イベントでステージが終了することがあるので
                    if (SRC.IsScenarioFinished)
                    {
                        SRC.IsScenarioFinished = false;
                        GUI.UnlockGUI();
                        return;
                    }

                    if (SRC.IsCanceled)
                    {
                        SRC.IsCanceled = false;
                        Status.ClearUnitStatus();
                        GUI.RedrawScreen();
                        CommandState = "ユニット選択";
                        GUI.UnlockGUI();
                        return;
                    }

                    // 残り行動数を減少させる
                    SelectedUnit.UseAction();

                    // 持続期間が「移動」のスペシャルパワー効果を削除
                    string argstype = "移動";
                    SelectedUnit.RemoveSpecialPowerInEffect(ref argstype);
                    Status.DisplayUnitStatus(ref SelectedUnit);
                    GUI.RedrawScreen();
                    CommandState = "ユニット選択";
                    GUI.UnlockGUI();
                    return;
                }
                // ADD START MARGE
                if (SelectedUnitMoveCost > 0)
                {
                    // 行動数を減らす
                    WaitCommand();
                    return;
                }

                SelectedUnitMoveCost = (short)Map.TotalMoveCost[withBlock.x, withBlock.y];
                // ADD END MARGE
            }

            CommandState = "移動後コマンド選択";
            GUI.UnlockGUI();
            ProceedCommand();
        }


        // 「テレポート」コマンドを開始
        // MOD START MARGE
        // Public Sub StartTeleportCommand()
        private static void StartTeleportCommand()
        {
            // MOD END MARGE
            SelectedCommand = "テレポート";
            Map.AreaInTeleport(ref SelectedUnit);
            string argoname = "大型マップ";
            if (!Expression.IsOptionDefined(ref argoname))
            {
                GUI.Center(SelectedUnit.x, SelectedUnit.y);
            }

            GUI.MaskScreen();
            // MOD START MARGE
            // If MainWidth <> 15 Then
            if (GUI.NewGUIMode)
            {
                // MOD END MARGE
                Application.DoEvents();
                Status.ClearUnitStatus();
            }

            CommandState = "ターゲット選択";
        }

        // 「テレポート」コマンドを終了
        // MOD START MARGE
        // Public Sub FinishTeleportCommand()
        private static void FinishTeleportCommand()
        {
            // MOD END MARGE
            short ret;
            GUI.LockGUI();
            {
                var withBlock = SelectedUnit;
                PrevUnitX = withBlock.x;
                PrevUnitY = withBlock.y;
                PrevUnitArea = withBlock.Area;
                PrevUnitEN = (short)withBlock.EN;

                // テレポート後に着艦or合体する場合はプレイヤーに確認を取る
                if (Map.MapDataForUnit[SelectedX, SelectedY] is object)
                {
                    string argfname = "母艦";
                    if (Map.MapDataForUnit[SelectedX, SelectedY].IsFeatureAvailable(ref argfname))
                    {
                        ret = (short)Interaction.MsgBox("着艦しますか？", (MsgBoxStyle)((int)MsgBoxStyle.OkCancel + (int)MsgBoxStyle.Question), "着艦");
                    }
                    else
                    {
                        ret = (short)Interaction.MsgBox("合体しますか？", (MsgBoxStyle)((int)MsgBoxStyle.OkCancel + (int)MsgBoxStyle.Question), "合体");
                    }

                    if (ret == (int)MsgBoxResult.Cancel)
                    {
                        CancelCommand();
                        GUI.UnlockGUI();
                        return;
                    }
                }

                // メッセージを表示
                object argIndex2 = "テレポート";
                string argmain_situation = "テレポート(" + withBlock.FeatureName(ref argIndex2) + ")";
                string argmain_situation1 = "テレポート";
                if (withBlock.IsMessageDefined(ref argmain_situation))
                {
                    Unit argu1 = null;
                    Unit argu2 = null;
                    GUI.OpenMessageForm(u1: ref argu1, u2: ref argu2);
                    object argIndex1 = "テレポート";
                    string argSituation = "テレポート(" + withBlock.FeatureName(ref argIndex1) + ")";
                    string argmsg_mode = "";
                    withBlock.PilotMessage(ref argSituation, msg_mode: ref argmsg_mode);
                    GUI.CloseMessageForm();
                }
                else if (withBlock.IsMessageDefined(ref argmain_situation1))
                {
                    Unit argu11 = null;
                    Unit argu21 = null;
                    GUI.OpenMessageForm(u1: ref argu11, u2: ref argu21);
                    string argSituation1 = "テレポート";
                    string argmsg_mode1 = "";
                    withBlock.PilotMessage(ref argSituation1, msg_mode: ref argmsg_mode1);
                    GUI.CloseMessageForm();
                }

                // アニメ表示
                bool localIsSpecialEffectDefined() { string argmain_situation = "テレポート"; object argIndex1 = "テレポート"; string argsub_situation = withBlock.FeatureName(ref argIndex1); var ret = withBlock.IsSpecialEffectDefined(ref argmain_situation, ref argsub_situation); return ret; }

                string argmain_situation4 = "テレポート";
                object argIndex6 = "テレポート";
                string argsub_situation2 = withBlock.FeatureName(ref argIndex6);
                if (withBlock.IsAnimationDefined(ref argmain_situation4, ref argsub_situation2))
                {
                    string argmain_situation2 = "テレポート";
                    object argIndex3 = "テレポート";
                    string argsub_situation = withBlock.FeatureName(ref argIndex3);
                    withBlock.PlayAnimation(ref argmain_situation2, ref argsub_situation);
                }
                else if (localIsSpecialEffectDefined())
                {
                    string argmain_situation3 = "テレポート";
                    object argIndex4 = "テレポート";
                    string argsub_situation1 = withBlock.FeatureName(ref argIndex4);
                    withBlock.SpecialEffect(ref argmain_situation3, ref argsub_situation1);
                }
                else if (SRC.BattleAnimation)
                {
                    object argIndex5 = "テレポート";
                    string arganame = "テレポート発動 Whiz.wav " + withBlock.FeatureName0(ref argIndex5);
                    Effect.ShowAnimation(ref arganame);
                }
                else
                {
                    string argwave_name = "Whiz.wav";
                    Sound.PlayWave(ref argwave_name);
                }

                // ＥＮを消費
                object argIndex7 = "テレポート";
                string arglist = withBlock.FeatureData(ref argIndex7);
                if (GeneralLib.LLength(ref arglist) == 2)
                {
                    string localLIndex() { object argIndex1 = "テレポート"; string arglist = withBlock.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                    string localLIndex1() { object argIndex1 = "テレポート"; string arglist = withBlock.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                    withBlock.EN = PrevUnitEN - Conversions.ToShort(localLIndex1());
                }
                else
                {
                    withBlock.EN = PrevUnitEN - 40;
                }

                // ユニットを移動
                withBlock.Move(SelectedX, SelectedY, true, false, true);
                GUI.RedrawScreen();

                // 移動後に着艦または合体した？
                if (!ReferenceEquals(Map.MapDataForUnit[SelectedX, SelectedY], SelectedUnit))
                {
                    string argfname1 = "母艦";
                    string argfname2 = "母艦";
                    if (Map.MapDataForUnit[SelectedX, SelectedY].IsFeatureAvailable(ref argfname1) & !withBlock.IsFeatureAvailable(ref argfname2) & withBlock.CountPilot() > 0)
                    {
                        // 着艦メッセージ表示
                        string argmain_situation5 = "着艦(" + withBlock.Name + ")";
                        string argmain_situation6 = "着艦";
                        if (withBlock.IsMessageDefined(ref argmain_situation5))
                        {
                            Unit argu12 = null;
                            Unit argu22 = null;
                            GUI.OpenMessageForm(u1: ref argu12, u2: ref argu22);
                            string argSituation2 = "着艦(" + withBlock.Name + ")";
                            string argmsg_mode2 = "";
                            withBlock.PilotMessage(ref argSituation2, msg_mode: ref argmsg_mode2);
                            GUI.CloseMessageForm();
                        }
                        else if (withBlock.IsMessageDefined(ref argmain_situation6))
                        {
                            Unit argu13 = null;
                            Unit argu23 = null;
                            GUI.OpenMessageForm(u1: ref argu13, u2: ref argu23);
                            string argSituation3 = "着艦";
                            string argmsg_mode3 = "";
                            withBlock.PilotMessage(ref argSituation3, msg_mode: ref argmsg_mode3);
                            GUI.CloseMessageForm();
                        }

                        string argmain_situation7 = "着艦";
                        string argsub_situation3 = withBlock.Name;
                        withBlock.SpecialEffect(ref argmain_situation7, ref argsub_situation3);
                        withBlock.Name = argsub_situation3;

                        // 収納イベント
                        SelectedTarget = Map.MapDataForUnit[SelectedX, SelectedY];
                        Event_Renamed.HandleEvent("収納", withBlock.MainPilot().ID);
                    }
                    else
                    {
                        // 合体後のユニットを選択
                        SelectedUnit = Map.MapDataForUnit[SelectedX, SelectedY];

                        // 合体イベント
                        Event_Renamed.HandleEvent("合体", SelectedUnit.MainPilot().ID, SelectedUnit.Name);
                    }

                    // 移動後の収納・合体イベントでステージが終了することがあるので
                    if (SRC.IsScenarioFinished)
                    {
                        SRC.IsScenarioFinished = false;
                        GUI.UnlockGUI();
                        return;
                    }

                    if (SRC.IsCanceled)
                    {
                        SRC.IsCanceled = false;
                        Status.ClearUnitStatus();
                        GUI.RedrawScreen();
                        CommandState = "ユニット選択";
                        GUI.UnlockGUI();
                        return;
                    }

                    // 残り行動数を減少させる
                    SelectedUnit.UseAction();

                    // 持続期間が「移動」のスペシャルパワー効果を削除
                    string argstype = "移動";
                    SelectedUnit.RemoveSpecialPowerInEffect(ref argstype);
                    Status.DisplayUnitStatus(ref Map.MapDataForUnit[SelectedX, SelectedY]);
                    GUI.RedrawScreen();
                    CommandState = "ユニット選択";
                    GUI.UnlockGUI();
                    return;
                }
                // ADD START MARGE
                SelectedUnitMoveCost = 100;
                // ADD END MARGE
            }

            CommandState = "移動後コマンド選択";
            GUI.UnlockGUI();
            ProceedCommand();
        }


        // 「ジャンプ」コマンドを開始
        // MOD START MARGE
        // Public Sub StartJumpCommand()
        private static void StartJumpCommand()
        {
            // MOD END MARGE
            SelectedCommand = "ジャンプ";
            Map.AreaInSpeed(ref SelectedUnit, true);
            string argoname = "大型マップ";
            if (!Expression.IsOptionDefined(ref argoname))
            {
                GUI.Center(SelectedUnit.x, SelectedUnit.y);
            }

            GUI.MaskScreen();
            // MOD START MARGE
            // If MainWidth <> 15 Then
            if (GUI.NewGUIMode)
            {
                // MOD END MARGE
                Application.DoEvents();
                Status.ClearUnitStatus();
            }

            CommandState = "ターゲット選択";
        }

        // 「ジャンプ」コマンドを終了
        // MOD START MARGE
        // Public Sub FinishJumpCommand()
        private static void FinishJumpCommand()
        {
            // MOD END MARGE
            short ret;
            GUI.LockGUI();
            {
                var withBlock = SelectedUnit;
                PrevUnitX = withBlock.x;
                PrevUnitY = withBlock.y;
                PrevUnitArea = withBlock.Area;
                PrevUnitEN = (short)withBlock.EN;

                // ジャンプ後に着艦or合体する場合はプレイヤーに確認を取る
                if (Map.MapDataForUnit[SelectedX, SelectedY] is object)
                {
                    string argfname = "母艦";
                    if (Map.MapDataForUnit[SelectedX, SelectedY].IsFeatureAvailable(ref argfname))
                    {
                        ret = (short)Interaction.MsgBox("着艦しますか？", (MsgBoxStyle)((int)MsgBoxStyle.OkCancel + (int)MsgBoxStyle.Question), "着艦");
                    }
                    else
                    {
                        ret = (short)Interaction.MsgBox("合体しますか？", (MsgBoxStyle)((int)MsgBoxStyle.OkCancel + (int)MsgBoxStyle.Question), "合体");
                    }

                    if (ret == (int)MsgBoxResult.Cancel)
                    {
                        CancelCommand();
                        GUI.UnlockGUI();
                        return;
                    }
                }

                // メッセージを表示
                object argIndex2 = "ジャンプ";
                string argmain_situation = "ジャンプ(" + withBlock.FeatureName(ref argIndex2) + ")";
                string argmain_situation1 = "ジャンプ";
                if (withBlock.IsMessageDefined(ref argmain_situation))
                {
                    Unit argu1 = null;
                    Unit argu2 = null;
                    GUI.OpenMessageForm(u1: ref argu1, u2: ref argu2);
                    object argIndex1 = "ジャンプ";
                    string argSituation = "ジャンプ(" + withBlock.FeatureName(ref argIndex1) + ")";
                    string argmsg_mode = "";
                    withBlock.PilotMessage(ref argSituation, msg_mode: ref argmsg_mode);
                    GUI.CloseMessageForm();
                }
                else if (withBlock.IsMessageDefined(ref argmain_situation1))
                {
                    Unit argu11 = null;
                    Unit argu21 = null;
                    GUI.OpenMessageForm(u1: ref argu11, u2: ref argu21);
                    string argSituation1 = "ジャンプ";
                    string argmsg_mode1 = "";
                    withBlock.PilotMessage(ref argSituation1, msg_mode: ref argmsg_mode1);
                    GUI.CloseMessageForm();
                }

                // アニメ表示
                bool localIsSpecialEffectDefined() { string argmain_situation = "ジャンプ"; object argIndex1 = "ジャンプ"; string argsub_situation = withBlock.FeatureName(ref argIndex1); var ret = withBlock.IsSpecialEffectDefined(ref argmain_situation, ref argsub_situation); return ret; }

                string argmain_situation4 = "ジャンプ";
                object argIndex5 = "ジャンプ";
                string argsub_situation2 = withBlock.FeatureName(ref argIndex5);
                if (withBlock.IsAnimationDefined(ref argmain_situation4, ref argsub_situation2))
                {
                    string argmain_situation2 = "ジャンプ";
                    object argIndex3 = "ジャンプ";
                    string argsub_situation = withBlock.FeatureName(ref argIndex3);
                    withBlock.PlayAnimation(ref argmain_situation2, ref argsub_situation);
                }
                else if (localIsSpecialEffectDefined())
                {
                    string argmain_situation3 = "ジャンプ";
                    object argIndex4 = "ジャンプ";
                    string argsub_situation1 = withBlock.FeatureName(ref argIndex4);
                    withBlock.SpecialEffect(ref argmain_situation3, ref argsub_situation1);
                }
                else
                {
                    string argwave_name = "Swing.wav";
                    Sound.PlayWave(ref argwave_name);
                }

                // ＥＮを消費
                object argIndex6 = "ジャンプ";
                string arglist = withBlock.FeatureData(ref argIndex6);
                if (GeneralLib.LLength(ref arglist) == 2)
                {
                    string localLIndex() { object argIndex1 = "ジャンプ"; string arglist = withBlock.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                    string localLIndex1() { object argIndex1 = "ジャンプ"; string arglist = withBlock.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                    withBlock.EN = PrevUnitEN - Conversions.ToShort(localLIndex1());
                }

                // ユニットを移動
                withBlock.Move(SelectedX, SelectedY, true, false, true);
                GUI.RedrawScreen();

                // 移動後に着艦または合体した？
                if (!ReferenceEquals(Map.MapDataForUnit[SelectedX, SelectedY], SelectedUnit))
                {
                    string argfname1 = "母艦";
                    string argfname2 = "母艦";
                    if (Map.MapDataForUnit[SelectedX, SelectedY].IsFeatureAvailable(ref argfname1) & !withBlock.IsFeatureAvailable(ref argfname2) & withBlock.CountPilot() > 0)
                    {
                        // 着艦メッセージ表示
                        string argmain_situation5 = "着艦(" + withBlock.Name + ")";
                        string argmain_situation6 = "着艦";
                        if (withBlock.IsMessageDefined(ref argmain_situation5))
                        {
                            Unit argu12 = null;
                            Unit argu22 = null;
                            GUI.OpenMessageForm(u1: ref argu12, u2: ref argu22);
                            string argSituation2 = "着艦(" + withBlock.Name + ")";
                            string argmsg_mode2 = "";
                            withBlock.PilotMessage(ref argSituation2, msg_mode: ref argmsg_mode2);
                            GUI.CloseMessageForm();
                        }
                        else if (withBlock.IsMessageDefined(ref argmain_situation6))
                        {
                            Unit argu13 = null;
                            Unit argu23 = null;
                            GUI.OpenMessageForm(u1: ref argu13, u2: ref argu23);
                            string argSituation3 = "着艦";
                            string argmsg_mode3 = "";
                            withBlock.PilotMessage(ref argSituation3, msg_mode: ref argmsg_mode3);
                            GUI.CloseMessageForm();
                        }

                        string argmain_situation7 = "着艦";
                        string argsub_situation3 = withBlock.Name;
                        withBlock.SpecialEffect(ref argmain_situation7, ref argsub_situation3);
                        withBlock.Name = argsub_situation3;

                        // 収納イベント
                        SelectedTarget = Map.MapDataForUnit[SelectedX, SelectedY];
                        Event_Renamed.HandleEvent("収納", withBlock.MainPilot().ID);
                    }
                    else
                    {
                        // 合体後のユニットを選択
                        SelectedUnit = Map.MapDataForUnit[SelectedX, SelectedY];

                        // 合体イベント
                        Event_Renamed.HandleEvent("合体", SelectedUnit.MainPilot().ID, SelectedUnit.Name);
                    }

                    // 移動後の収納・合体イベントでステージが終了することがあるので
                    if (SRC.IsScenarioFinished)
                    {
                        SRC.IsScenarioFinished = false;
                        GUI.UnlockGUI();
                        return;
                    }

                    if (SRC.IsCanceled)
                    {
                        SRC.IsCanceled = false;
                        Status.ClearUnitStatus();
                        GUI.RedrawScreen();
                        CommandState = "ユニット選択";
                        GUI.UnlockGUI();
                        return;
                    }

                    // 残り行動数を減少させる
                    SelectedUnit.UseAction();

                    // 持続期間が「移動」のスペシャルパワー効果を削除
                    string argstype = "移動";
                    SelectedUnit.RemoveSpecialPowerInEffect(ref argstype);
                    Status.DisplayUnitStatus(ref Map.MapDataForUnit[SelectedX, SelectedY]);
                    GUI.RedrawScreen();
                    CommandState = "ユニット選択";
                    GUI.UnlockGUI();
                    return;
                }
                // ADD START MARGE
                SelectedUnitMoveCost = 100;
                // ADD END MARGE
            }

            CommandState = "移動後コマンド選択";
            GUI.UnlockGUI();
            ProceedCommand();
        }


        // 「攻撃」コマンドを開始
        // MOD START MARGE
        // Public Sub StartAttackCommand()
        private static void StartAttackCommand()
        {
            // MOD END MARGE
            short i, j;
            Unit t;
            short min_range, max_range;
            var BGM = default(string);
            GUI.LockGUI();
            var partners = default(Unit[]);
            {
                var withBlock = SelectedUnit;
                // ＢＧＭの設定
                string argfname = "ＢＧＭ";
                if (withBlock.IsFeatureAvailable(ref argfname))
                {
                    object argIndex1 = "ＢＧＭ";
                    string argmidi_name = withBlock.FeatureData(ref argIndex1);
                    BGM = Sound.SearchMidiFile(ref argmidi_name);
                }

                if (Strings.Len(BGM) == 0)
                {
                    string argmidi_name1 = withBlock.MainPilot().BGM;
                    BGM = Sound.SearchMidiFile(ref argmidi_name1);
                    withBlock.MainPilot().BGM = argmidi_name1;
                }

                if (Strings.Len(BGM) == 0)
                {
                    string argbgm_name = "default";
                    BGM = Sound.BGMName(ref argbgm_name);
                }

                // 武器の選択
                UseSupportAttack = true;
                if (CommandState == "コマンド選択")
                {
                    string argcaption_msg = "武器選択";
                    string arglb_mode = "移動前";
                    SelectedWeapon = GUI.WeaponListBox(ref SelectedUnit, ref argcaption_msg, ref arglb_mode, ref BGM);
                }
                else
                {
                    string argcaption_msg1 = "武器選択";
                    string arglb_mode1 = "移動後";
                    SelectedWeapon = GUI.WeaponListBox(ref SelectedUnit, ref argcaption_msg1, ref arglb_mode1, ref BGM);
                }

                // キャンセル
                if (SelectedWeapon == 0)
                {
                    if (SRC.AutoMoveCursor)
                    {
                        GUI.RestoreCursorPos();
                    }

                    CancelCommand();
                    GUI.UnlockGUI();
                    return;
                }

                // 武器ＢＧＭの演奏
                string argfname1 = "武器ＢＧＭ";
                if (withBlock.IsFeatureAvailable(ref argfname1))
                {
                    var loopTo = withBlock.CountFeature();
                    for (i = 1; i <= loopTo; i++)
                    {
                        string localFeature() { object argIndex1 = i; var ret = withBlock.Feature(ref argIndex1); return ret; }

                        string localFeatureData2() { object argIndex1 = i; var ret = withBlock.FeatureData(ref argIndex1); return ret; }

                        string localLIndex() { string arglist = hs644c0746afde449eb03788895da90548(); var ret = GeneralLib.LIndex(ref arglist, 1); return ret; }

                        if (localFeature() == "武器ＢＧＭ" & (localLIndex() ?? "") == (withBlock.Weapon(SelectedWeapon).Name ?? ""))
                        {
                            string localFeatureData() { object argIndex1 = i; var ret = withBlock.FeatureData(ref argIndex1); return ret; }

                            string localFeatureData1() { object argIndex1 = i; var ret = withBlock.FeatureData(ref argIndex1); return ret; }

                            string argmidi_name2 = Strings.Mid(localFeatureData(), Strings.InStr(localFeatureData1(), " ") + 1);
                            BGM = Sound.SearchMidiFile(ref argmidi_name2);
                            if (Strings.Len(BGM) > 0)
                            {
                                Sound.ChangeBGM(ref BGM);
                            }

                            break;
                        }
                    }
                }

                // 選択した武器の種類により、この後のコマンドの進行の仕方が異なる
                string argattr = "Ｍ";
                if (withBlock.IsWeaponClassifiedAs(SelectedWeapon, ref argattr))
                {
                    SelectedCommand = "マップ攻撃";
                }
                else
                {
                    SelectedCommand = "攻撃";
                }

                // 武器の射程を求めておく
                min_range = withBlock.Weapon(SelectedWeapon).MinRange;
                max_range = withBlock.WeaponMaxRange(SelectedWeapon);

                // 攻撃範囲の表示
                string argattr2 = "Ｍ直";
                string argattr3 = "Ｍ拡";
                string argattr4 = "Ｍ扇";
                string argattr5 = "Ｍ全";
                string argattr6 = "Ｍ投";
                string argattr7 = "Ｍ線";
                string argattr8 = "Ｍ移";
                if (withBlock.IsWeaponClassifiedAs(SelectedWeapon, ref argattr2))
                {
                    Map.AreaInCross(withBlock.x, withBlock.y, min_range, ref max_range);
                }
                else if (withBlock.IsWeaponClassifiedAs(SelectedWeapon, ref argattr3))
                {
                    Map.AreaInWideCross(withBlock.x, withBlock.y, min_range, ref max_range);
                }
                else if (withBlock.IsWeaponClassifiedAs(SelectedWeapon, ref argattr4))
                {
                    string argattr1 = "Ｍ扇";
                    Map.AreaInSectorCross(withBlock.x, withBlock.y, min_range, ref max_range, (short)withBlock.WeaponLevel(SelectedWeapon, ref argattr1));
                }
                else if (withBlock.IsWeaponClassifiedAs(SelectedWeapon, ref argattr5) | withBlock.IsWeaponClassifiedAs(SelectedWeapon, ref argattr6) | withBlock.IsWeaponClassifiedAs(SelectedWeapon, ref argattr7))
                {
                    string arguparty1 = "すべて";
                    Map.AreaInRange(withBlock.x, withBlock.y, max_range, min_range, ref arguparty1);
                }
                else if (withBlock.IsWeaponClassifiedAs(SelectedWeapon, ref argattr8))
                {
                    Map.AreaInMoveAction(ref SelectedUnit, max_range);
                }
                else
                {
                    string arguparty = "味方の敵";
                    Map.AreaInRange(withBlock.x, withBlock.y, max_range, min_range, ref arguparty);
                }

                // 射程１の合体技はパートナーで相手を取り囲んでいないと使用できない
                string argattr9 = "合";
                string argattr10 = "Ｍ";
                if (max_range == 1 & withBlock.IsWeaponClassifiedAs(SelectedWeapon, ref argattr9) & !withBlock.IsWeaponClassifiedAs(SelectedWeapon, ref argattr10))
                {
                    for (i = 1; i <= 4; i++)
                    {
                        // UPGRADE_NOTE: オブジェクト t をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                        t = null;
                        switch (i)
                        {
                            case 1:
                                {
                                    if (withBlock.x > 1)
                                    {
                                        t = Map.MapDataForUnit[withBlock.x - 1, withBlock.y];
                                    }

                                    break;
                                }

                            case 2:
                                {
                                    if (withBlock.x < Map.MapWidth)
                                    {
                                        t = Map.MapDataForUnit[withBlock.x + 1, withBlock.y];
                                    }

                                    break;
                                }

                            case 3:
                                {
                                    if (withBlock.y > 1)
                                    {
                                        t = Map.MapDataForUnit[withBlock.x, withBlock.y - 1];
                                    }

                                    break;
                                }

                            case 4:
                                {
                                    if (withBlock.y < Map.MapHeight)
                                    {
                                        t = Map.MapDataForUnit[withBlock.x, withBlock.y + 1];
                                    }

                                    break;
                                }
                        }

                        if (t is object)
                        {
                            if (withBlock.IsEnemy(ref t))
                            {
                                string argctype_Renamed = "武装";
                                withBlock.CombinationPartner(ref argctype_Renamed, SelectedWeapon, ref partners, t.x, t.y);
                                if (Information.UBound(partners) == 0)
                                {
                                    Map.MaskData[t.x, t.y] = true;
                                }
                            }
                        }
                    }
                }

                // ユニットに対するマスクの設定
                string argattr15 = "Ｍ投";
                string argattr16 = "Ｍ線";
                if (!withBlock.IsWeaponClassifiedAs(SelectedWeapon, ref argattr15) & !withBlock.IsWeaponClassifiedAs(SelectedWeapon, ref argattr16))
                {
                    var loopTo1 = (short)GeneralLib.MinLng(withBlock.x + max_range, Map.MapWidth);
                    for (i = (short)GeneralLib.MaxLng(withBlock.x - max_range, 1); i <= loopTo1; i++)
                    {
                        var loopTo2 = (short)GeneralLib.MinLng(withBlock.y + max_range, Map.MapHeight);
                        for (j = (short)GeneralLib.MaxLng(withBlock.y - max_range, 1); j <= loopTo2; j++)
                        {
                            if (Map.MaskData[i, j])
                            {
                                goto NextLoop;
                            }

                            t = Map.MapDataForUnit[i, j];
                            if (t is null)
                            {
                                goto NextLoop;
                            }

                            // 武器の地形適応が有効？
                            if (withBlock.WeaponAdaption(SelectedWeapon, ref t.Area) == 0d)
                            {
                                Map.MaskData[i, j] = true;
                                goto NextLoop;
                            }

                            // 封印武器の対象属性外でない？
                            string argattr11 = "封";
                            if (withBlock.IsWeaponClassifiedAs(SelectedWeapon, ref argattr11))
                            {
                                if (withBlock.Weapon(SelectedWeapon).Power > 0 & withBlock.Damage(SelectedWeapon, ref t, true) == 0 | withBlock.CriticalProbability(SelectedWeapon, ref t) == 0)
                                {
                                    Map.MaskData[i, j] = true;
                                    goto NextLoop;
                                }
                            }

                            // 限定武器の対象属性外でない？
                            string argattr12 = "限";
                            if (withBlock.IsWeaponClassifiedAs(SelectedWeapon, ref argattr12))
                            {
                                if (withBlock.Weapon(SelectedWeapon).Power > 0 & withBlock.Damage(SelectedWeapon, ref t, true) == 0 | withBlock.Weapon(SelectedWeapon).Power == 0 & withBlock.CriticalProbability(SelectedWeapon, ref t) == 0)
                                {
                                    Map.MaskData[i, j] = true;
                                    goto NextLoop;
                                }
                            }

                            // 識別攻撃の場合の処理
                            string argattr13 = "識";
                            string argsptype = "識別攻撃";
                            if (withBlock.IsWeaponClassifiedAs(SelectedWeapon, ref argattr13) | withBlock.IsUnderSpecialPowerEffect(ref argsptype))
                            {
                                if (withBlock.IsAlly(ref t))
                                {
                                    Map.MaskData[i, j] = true;
                                    goto NextLoop;
                                }
                            }

                            // ステルス＆隠れ身チェック
                            string argattr14 = "Ｍ";
                            if (!withBlock.IsWeaponClassifiedAs(SelectedWeapon, ref argattr14))
                            {
                                if (!withBlock.IsTargetWithinRange(SelectedWeapon, ref t))
                                {
                                    Map.MaskData[i, j] = true;
                                    goto NextLoop;
                                }
                            }

                            NextLoop:
                            ;
                        }
                    }
                }

                Map.MaskData[withBlock.x, withBlock.y] = false;
                string argoname = "大型マップ";
                if (!Expression.IsOptionDefined(ref argoname))
                {
                    GUI.Center(withBlock.x, withBlock.y);
                }

                GUI.MaskScreen();
            }

            // ターゲット選択へ
            if (CommandState == "コマンド選択")
            {
                CommandState = "ターゲット選択";
            }
            else
            {
                CommandState = "移動後ターゲット選択";
            }

            // カーソル自動移動を行う？
            if (!SRC.AutoMoveCursor)
            {
                GUI.UnlockGUI();
                return;
            }

            // ＨＰがもっとも低いターゲットを探す
            // UPGRADE_NOTE: オブジェクト t をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            t = null;
            foreach (Unit u in SRC.UList)
            {
                if (u.Status_Renamed == "出撃" & (u.Party == "敵" | u.Party == "中立"))
                {
                    if (Map.MaskData[u.x, u.y] == false)
                    {
                        if (t is null)
                        {
                            t = u;
                        }
                        else if (u.HP < t.HP)
                        {
                            t = u;
                        }
                        else if (u.HP == t.HP)
                        {
                            if (Math.Pow(Math.Abs((short)(SelectedUnit.x - u.x)), 2d) + Math.Pow(Math.Abs((short)(SelectedUnit.y - u.y)), 2d) < Math.Pow(Math.Abs((short)(SelectedUnit.x - t.x)), 2d) + Math.Pow(Math.Abs((short)(SelectedUnit.y - t.y)), 2d))
                            {
                                t = u;
                            }
                        }
                    }
                }
            }

            // 適当なターゲットが見つからなければ自分自身を選択
            if (t is null)
            {
                t = SelectedUnit;
            }

            // カーソルを移動
            string argcursor_mode = "ユニット選択";
            GUI.MoveCursorPos(ref argcursor_mode, t);

            // ターゲットのステータスを表示
            if (!ReferenceEquals(SelectedUnit, t))
            {
                Status.DisplayUnitStatus(ref t);
            }

            GUI.UnlockGUI();
        }

        // 「攻撃」コマンドを終了
        // MOD START MARGE
        // Public Sub FinishAttackCommand()
        private static void FinishAttackCommand()
        {
            // MOD END MARGE
            short i;
            var earnings = default(int);
            string def_mode;
            var partners = default(Unit[]);
            var BGM = default(string);
            var is_suiside = default(bool);
            string wname, twname = default;
            short tx, ty;
            Unit attack_target;
            double attack_target_hp_ratio;
            Unit defense_target;
            double defense_target_hp_ratio;
            Unit defense_target2;
            var defense_target2_hp_ratio = default(double);
            var support_attack_done = default(bool);
            short w2;
            // ADD START MARGE
            bool is_p_weapon;
            // ADD END MARGE
            // MOD START MARGE
            // If MainWidth <> 15 Then
            if (GUI.NewGUIMode)
            {
                // MOD END MARGE
                Status.ClearUnitStatus();
            }

            GUI.LockGUI();
            wname = SelectedUnit.Weapon(SelectedWeapon).Name;
            SelectedWeaponName = wname;

            // ADD START MARGE
            // 移動後使用後可能な武器か記録しておく
            string argattr = "移動後攻撃可";
            is_p_weapon = SelectedUnit.IsWeaponClassifiedAs(SelectedWeapon, ref argattr);
            // ADD END MARGE

            // 合体技のパートナーを設定
            {
                var withBlock = SelectedUnit;
                string argattr1 = "合";
                if (withBlock.IsWeaponClassifiedAs(SelectedWeapon, ref argattr1))
                {
                    if (withBlock.WeaponMaxRange(SelectedWeapon) == 1)
                    {
                        string argctype_Renamed = "武装";
                        withBlock.CombinationPartner(ref argctype_Renamed, SelectedWeapon, ref partners, SelectedTarget.x, SelectedTarget.y);
                    }
                    else
                    {
                        string argctype_Renamed1 = "武装";
                        withBlock.CombinationPartner(ref argctype_Renamed1, SelectedWeapon, ref partners);
                    }
                }
                else
                {
                    SelectedPartners = new Unit[1];
                    partners = new Unit[1];
                }
            }

            // 敵の反撃手段を設定
            UseSupportGuard = true;
            string argamode = "反撃";
            int argmax_prob = 0;
            int argmax_dmg = 0;
            SelectedTWeapon = COM.SelectWeapon(ref SelectedTarget, ref SelectedUnit, ref argamode, max_prob: ref argmax_prob, max_dmg: ref argmax_dmg);
            string argattr2 = "間";
            if (SelectedUnit.IsWeaponClassifiedAs(SelectedWeapon, ref argattr2))
            {
                SelectedTWeapon = 0;
            }

            if (SelectedTWeapon > 0)
            {
                twname = SelectedTarget.Weapon(SelectedTWeapon).Name;
                SelectedTWeaponName = twname;
            }
            else
            {
                SelectedTWeaponName = "";
            }

            // 敵の防御行動を設定
            // UPGRADE_WARNING: オブジェクト SelectDefense() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            def_mode = Conversions.ToString(COM.SelectDefense(ref SelectedUnit, ref SelectedWeapon, ref SelectedTarget, ref SelectedTWeapon));
            if (!string.IsNullOrEmpty(def_mode))
            {
                if (SelectedTWeapon > 0)
                {
                    SelectedTWeapon = -1;
                }
            }

            SelectedDefenseOption = def_mode;

            // 戦闘前に一旦クリア
            // UPGRADE_NOTE: オブジェクト SupportAttackUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            SupportAttackUnit = null;
            // UPGRADE_NOTE: オブジェクト SupportGuardUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            SupportGuardUnit = null;
            // UPGRADE_NOTE: オブジェクト SupportGuardUnit2 をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            SupportGuardUnit2 = null;

            // 攻撃側の武器使用イベント
            Event_Renamed.HandleEvent("使用", SelectedUnit.MainPilot().ID, wname);
            if (SRC.IsScenarioFinished)
            {
                GUI.UnlockGUI();
                SRC.IsScenarioFinished = false;
                SelectedPartners = new Unit[1];
                return;
            }

            if (SRC.IsCanceled)
            {
                SRC.IsCanceled = false;
                WaitCommand();
                return;
            }

            // 敵の武器使用イベント
            if (SelectedTWeapon > 0)
            {
                SaveSelections();
                SwapSelections();
                Event_Renamed.HandleEvent("使用", SelectedUnit.MainPilot().ID, twname);
                RestoreSelections();
                if (SRC.IsScenarioFinished)
                {
                    SRC.IsScenarioFinished = false;
                    SelectedPartners = new Unit[1];
                    GUI.UnlockGUI();
                    return;
                }

                if (SRC.IsCanceled)
                {
                    SRC.IsCanceled = false;
                    SelectedPartners = new Unit[1];
                    WaitCommand();
                    return;
                }
            }

            // 攻撃イベント
            Event_Renamed.HandleEvent("攻撃", SelectedUnit.MainPilot().ID, SelectedTarget.MainPilot().ID);
            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
                SelectedPartners = new Unit[1];
                GUI.UnlockGUI();
                return;
            }

            if (SRC.IsCanceled)
            {
                SRC.IsCanceled = false;
                SelectedPartners = new Unit[1];
                WaitCommand();
                return;
            }

            // 敵がＢＧＭ能力を持つ場合はＢＧＭを変更
            {
                var withBlock1 = SelectedTarget;
                string argfname = "ＢＧＭ";
                if (withBlock1.IsFeatureAvailable(ref argfname) & Strings.InStr(withBlock1.MainPilot().Name, "(ザコ)") == 0)
                {
                    object argIndex1 = "ＢＧＭ";
                    string argmidi_name = withBlock1.FeatureData(ref argIndex1);
                    BGM = Sound.SearchMidiFile(ref argmidi_name);
                    if (Strings.Len(BGM) > 0)
                    {
                        Sound.BossBGM = false;
                        Sound.ChangeBGM(ref BGM);
                        Sound.BossBGM = true;
                    }
                }
            }

            // そうではなく、ボス用ＢＧＭが流れていれば味方のＢＧＭに切り替え
            if (Strings.Len(BGM) == 0 & Sound.BossBGM)
            {
                Sound.BossBGM = false;
                BGM = "";
                {
                    var withBlock2 = SelectedUnit;
                    string argfname1 = "武器ＢＧＭ";
                    if (withBlock2.IsFeatureAvailable(ref argfname1))
                    {
                        var loopTo = withBlock2.CountFeature();
                        for (i = 1; i <= loopTo; i++)
                        {
                            string localFeature() { object argIndex1 = i; var ret = withBlock2.Feature(ref argIndex1); return ret; }

                            string localFeatureData2() { object argIndex1 = i; var ret = withBlock2.FeatureData(ref argIndex1); return ret; }

                            string localLIndex() { string arglist = hs51a192742c114976b44a246d492333eb(); var ret = GeneralLib.LIndex(ref arglist, 1); return ret; }

                            if (localFeature() == "武器ＢＧＭ" & (localLIndex() ?? "") == (withBlock2.Weapon(SelectedWeapon).Name ?? ""))
                            {
                                string localFeatureData() { object argIndex1 = i; var ret = withBlock2.FeatureData(ref argIndex1); return ret; }

                                string localFeatureData1() { object argIndex1 = i; var ret = withBlock2.FeatureData(ref argIndex1); return ret; }

                                string argmidi_name1 = Strings.Mid(localFeatureData(), Strings.InStr(localFeatureData1(), " ") + 1);
                                BGM = Sound.SearchMidiFile(ref argmidi_name1);
                                break;
                            }
                        }
                    }

                    if (Strings.Len(BGM) == 0)
                    {
                        string argfname2 = "ＢＧＭ";
                        if (withBlock2.IsFeatureAvailable(ref argfname2))
                        {
                            object argIndex2 = "ＢＧＭ";
                            string argmidi_name2 = withBlock2.FeatureData(ref argIndex2);
                            BGM = Sound.SearchMidiFile(ref argmidi_name2);
                        }
                    }

                    if (Strings.Len(BGM) == 0)
                    {
                        string argmidi_name3 = withBlock2.MainPilot().BGM;
                        BGM = Sound.SearchMidiFile(ref argmidi_name3);
                        withBlock2.MainPilot().BGM = argmidi_name3;
                    }

                    if (Strings.Len(BGM) == 0)
                    {
                        string argbgm_name = "default";
                        BGM = Sound.BGMName(ref argbgm_name);
                    }

                    Sound.ChangeBGM(ref BGM);
                }
            }

            {
                var withBlock3 = SelectedUnit;
                // 攻撃参加ユニット以外はマスク
                foreach (Unit u in SRC.UList)
                {
                    if (u.Status_Renamed == "出撃")
                    {
                        Map.MaskData[u.x, u.y] = true;
                    }
                }

                // 合体技パートナーのハイライト表示
                var loopTo1 = (short)Information.UBound(partners);
                for (i = 1; i <= loopTo1; i++)
                {
                    {
                        var withBlock4 = partners[i];
                        Map.MaskData[withBlock4.x, withBlock4.y] = false;
                    }
                }

                Map.MaskData[withBlock3.x, withBlock3.y] = false;
                Map.MaskData[SelectedTarget.x, SelectedTarget.y] = false;
                if (!SRC.BattleAnimation)
                {
                    GUI.MaskScreen();
                }
            }

            // イベント用に戦闘に参加するユニットの情報を記録しておく
            AttackUnit = SelectedUnit;
            attack_target = SelectedUnit;
            attack_target_hp_ratio = SelectedUnit.HP / (double)SelectedUnit.MaxHP;
            defense_target = SelectedTarget;
            defense_target_hp_ratio = SelectedTarget.HP / (double)SelectedTarget.MaxHP;
            // UPGRADE_NOTE: オブジェクト defense_target2 をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            defense_target2 = null;
            // UPGRADE_NOTE: オブジェクト SupportAttackUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            SupportAttackUnit = null;
            // UPGRADE_NOTE: オブジェクト SupportGuardUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            SupportGuardUnit = null;
            // UPGRADE_NOTE: オブジェクト SupportGuardUnit2 をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            SupportGuardUnit2 = null;

            // ターゲットの位置を記録
            tx = SelectedTarget.x;
            ty = SelectedTarget.y;
            GUI.OpenMessageForm(ref SelectedTarget, ref SelectedUnit);

            // 相手の先制攻撃？
            {
                var withBlock5 = SelectedTarget;
                // MOD START MARGE
                // If SelectedTWeapon > 0 And .MaxAction > 0 And .IsWeaponAvailable(SelectedTWeapon, "移動前") Then
                // SelectedTWeapon > 0の判定は、IsWeaponAvailableで行うようにした
                string argref_mode1 = "移動前";
                if (withBlock5.MaxAction() > 0 & withBlock5.IsWeaponAvailable(SelectedTWeapon, ref argref_mode1))
                {
                    // MOD END MARGE
                    string argattr8 = "後";
                    if (!withBlock5.IsWeaponClassifiedAs(SelectedTWeapon, ref argattr8))
                    {
                        string argattr6 = "後";
                        string argattr7 = "先";
                        object argIndex3 = "先読み";
                        string argref_mode = "";
                        string argsptype = "カウンター";
                        if (SelectedUnit.IsWeaponClassifiedAs(SelectedWeapon, ref argattr6))
                        {
                            def_mode = "先制攻撃";
                            string argattr3 = "自";
                            if (withBlock5.IsWeaponClassifiedAs(SelectedTWeapon, ref argattr3))
                            {
                                is_suiside = true;
                            }

                            // 先制攻撃攻撃を実施
                            withBlock5.Attack(SelectedTWeapon, SelectedUnit, "先制攻撃", "");
                            SelectedTarget = withBlock5.CurrentForm();
                        }
                        else if (withBlock5.IsWeaponClassifiedAs(SelectedTWeapon, ref argattr7) | withBlock5.MainPilot().SkillLevel(ref argIndex3, ref_mode: ref argref_mode) >= GeneralLib.Dice(16) | withBlock5.IsUnderSpecialPowerEffect(ref argsptype))
                        {
                            def_mode = "先制攻撃";
                            string argattr4 = "自";
                            if (withBlock5.IsWeaponClassifiedAs(SelectedTWeapon, ref argattr4))
                            {
                                is_suiside = true;
                            }

                            // カウンター攻撃を実施
                            withBlock5.Attack(SelectedTWeapon, SelectedUnit, "カウンター", "");
                            SelectedTarget = withBlock5.CurrentForm();
                        }
                        else if (withBlock5.MaxCounterAttack() > withBlock5.UsedCounterAttack)
                        {
                            def_mode = "先制攻撃";
                            string argattr5 = "自";
                            if (withBlock5.IsWeaponClassifiedAs(SelectedTWeapon, ref argattr5))
                            {
                                is_suiside = true;
                            }

                            // カウンター攻撃の残り回数を減少
                            withBlock5.UsedCounterAttack = (short)(withBlock5.UsedCounterAttack + 1);

                            // カウンター攻撃を実施
                            withBlock5.Attack(SelectedTWeapon, SelectedUnit, "カウンター", "");
                            SelectedTarget = withBlock5.CurrentForm();
                        }
                    }

                    // 攻撃側のユニットがかばわれた場合は攻撃側のターゲットを再設定
                    if (SupportGuardUnit2 is object)
                    {
                        attack_target = SupportGuardUnit2;
                        attack_target_hp_ratio = SupportGuardUnitHPRatio2;
                    }
                }
            }

            // サポートアタックのパートナーを探す
            {
                var withBlock6 = SelectedUnit;
                if (withBlock6.Status_Renamed == "出撃" & SelectedTarget.Status_Renamed == "出撃" & UseSupportAttack)
                {
                    SupportAttackUnit = withBlock6.LookForSupportAttack(ref SelectedTarget);

                    // 合体技ではサポートアタック不能
                    if (0 < SelectedWeapon & SelectedWeapon <= withBlock6.CountWeapon())
                    {
                        string argattr9 = "合";
                        if (withBlock6.IsWeaponClassifiedAs(SelectedWeapon, ref argattr9))
                        {
                            // UPGRADE_NOTE: オブジェクト SupportAttackUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                            SupportAttackUnit = null;
                        }
                    }

                    // 魅了された場合
                    object argIndex4 = "魅了";
                    if (withBlock6.IsConditionSatisfied(ref argIndex4) & ReferenceEquals(withBlock6.Master, SelectedTarget))
                    {
                        // UPGRADE_NOTE: オブジェクト SupportAttackUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                        SupportAttackUnit = null;
                    }

                    // 憑依された場合
                    object argIndex5 = "憑依";
                    if (withBlock6.IsConditionSatisfied(ref argIndex5))
                    {
                        if ((withBlock6.Master.Party ?? "") == (SelectedTarget.Party ?? ""))
                        {
                            // UPGRADE_NOTE: オブジェクト SupportAttackUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                            SupportAttackUnit = null;
                        }
                    }

                    // 踊らされた場合
                    object argIndex6 = "踊り";
                    if (withBlock6.IsConditionSatisfied(ref argIndex6))
                    {
                        // UPGRADE_NOTE: オブジェクト SupportAttackUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                        SupportAttackUnit = null;
                    }
                }
            }

            // 攻撃の実施
            {
                var withBlock7 = SelectedUnit;
                object argIndex10 = "攻撃不能";
                if (withBlock7.Status_Renamed == "出撃" & withBlock7.MaxAction(true) > 0 & !withBlock7.IsConditionSatisfied(ref argIndex10) & SelectedTarget.Status_Renamed == "出撃")
                {
                    // まだ武器は使用可能か？
                    if (SelectedWeapon > withBlock7.CountWeapon())
                    {
                        SelectedWeapon = -1;
                    }
                    else if ((wname ?? "") != (withBlock7.Weapon(SelectedWeapon).Name ?? ""))
                    {
                        SelectedWeapon = -1;
                    }
                    else if (CommandState == "移動後ターゲット選択")
                    {
                        string argref_mode3 = "移動後";
                        if (!withBlock7.IsWeaponAvailable(SelectedWeapon, ref argref_mode3))
                        {
                            SelectedWeapon = -1;
                        }
                    }
                    else
                    {
                        string argref_mode2 = "移動前";
                        if (!withBlock7.IsWeaponAvailable(SelectedWeapon, ref argref_mode2))
                        {
                            SelectedWeapon = -1;
                        }
                    }

                    if (SelectedWeapon > 0)
                    {
                        if (!withBlock7.IsTargetWithinRange(SelectedWeapon, ref SelectedTarget))
                        {
                            SelectedWeapon = 0;
                        }
                    }

                    // 魅了された場合
                    object argIndex7 = "魅了";
                    if (withBlock7.IsConditionSatisfied(ref argIndex7) & ReferenceEquals(withBlock7.Master, SelectedTarget))
                    {
                        SelectedWeapon = -1;
                    }

                    // 憑依された場合
                    object argIndex8 = "憑依";
                    if (withBlock7.IsConditionSatisfied(ref argIndex8))
                    {
                        if ((withBlock7.Master.Party ?? "") == (SelectedTarget.Party0 ?? ""))
                        {
                            SelectedWeapon = -1;
                        }
                    }

                    // 踊らされた場合
                    object argIndex9 = "踊り";
                    if (withBlock7.IsConditionSatisfied(ref argIndex9))
                    {
                        SelectedWeapon = -1;
                    }

                    if (SelectedWeapon > 0)
                    {
                        if (SupportAttackUnit is object & withBlock7.MaxSyncAttack() > withBlock7.UsedSyncAttack)
                        {
                            // 同時援護攻撃
                            withBlock7.Attack(SelectedWeapon, SelectedTarget, "統率", def_mode);
                        }
                        else
                        {
                            // 通常攻撃
                            withBlock7.Attack(SelectedWeapon, SelectedTarget, "", def_mode);
                        }
                    }
                    else if (SelectedWeapon == 0)
                    {
                        string argmain_situation2 = "射程外";
                        string argsub_situation2 = "";
                        if (withBlock7.IsAnimationDefined(ref argmain_situation2, sub_situation: ref argsub_situation2))
                        {
                            string argmain_situation = "射程外";
                            string argsub_situation = "";
                            withBlock7.PlayAnimation(ref argmain_situation, sub_situation: ref argsub_situation);
                        }
                        else
                        {
                            string argmain_situation1 = "射程外";
                            string argsub_situation1 = "";
                            withBlock7.SpecialEffect(ref argmain_situation1, sub_situation: ref argsub_situation1);
                        }

                        string argSituation = "射程外";
                        string argmsg_mode = "";
                        withBlock7.PilotMessage(ref argSituation, msg_mode: ref argmsg_mode);
                    }
                }
                else
                {
                    SelectedWeapon = -1;
                }

                SelectedUnit = withBlock7.CurrentForm();

                // 防御側のユニットがかばわれた場合は2番目の防御側ユニットとして記録
                if (SupportGuardUnit is object)
                {
                    defense_target2 = SupportGuardUnit;
                    defense_target2_hp_ratio = SupportGuardUnitHPRatio;
                }
            }

            // 同時攻撃
            if (SupportAttackUnit is object)
            {
                if (SupportAttackUnit.Status_Renamed != "出撃" | SelectedUnit.Status_Renamed != "出撃" | SelectedTarget.Status_Renamed != "出撃")
                {
                    // UPGRADE_NOTE: オブジェクト SupportAttackUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                    SupportAttackUnit = null;
                }
            }

            if (SupportAttackUnit is object)
            {
                if (SelectedUnit.MaxSyncAttack() > SelectedUnit.UsedSyncAttack)
                {
                    {
                        var withBlock8 = SupportAttackUnit;
                        // サポートアタックに使う武器を決定
                        string argamode1 = "サポートアタック";
                        int argmax_prob1 = 0;
                        int argmax_dmg1 = 0;
                        w2 = COM.SelectWeapon(ref SupportAttackUnit, ref SelectedTarget, ref argamode1, max_prob: ref argmax_prob1, max_dmg: ref argmax_dmg1);
                        if (w2 > 0)
                        {
                            // サポートアタックを実施
                            Map.MaskData[withBlock8.x, withBlock8.y] = false;
                            if (!SRC.BattleAnimation)
                            {
                                GUI.MaskScreen();
                            }

                            string argmain_situation4 = "サポートアタック開始";
                            string argsub_situation4 = "";
                            if (withBlock8.IsAnimationDefined(ref argmain_situation4, sub_situation: ref argsub_situation4))
                            {
                                string argmain_situation3 = "サポートアタック開始";
                                string argsub_situation3 = "";
                                withBlock8.PlayAnimation(ref argmain_situation3, sub_situation: ref argsub_situation3);
                            }

                            object argu2 = SupportAttackUnit;
                            GUI.UpdateMessageForm(ref SelectedTarget, ref argu2);
                            withBlock8.Attack(w2, SelectedTarget, "同時援護攻撃", def_mode);
                        }
                    }

                    // 後始末
                    {
                        var withBlock9 = SupportAttackUnit.CurrentForm();
                        if (w2 > 0)
                        {
                            string argmain_situation6 = "サポートアタック終了";
                            string argsub_situation6 = "";
                            if (withBlock9.IsAnimationDefined(ref argmain_situation6, sub_situation: ref argsub_situation6))
                            {
                                string argmain_situation5 = "サポートアタック終了";
                                string argsub_situation5 = "";
                                withBlock9.PlayAnimation(ref argmain_situation5, sub_situation: ref argsub_situation5);
                            }

                            // サポートアタックの残り回数を減らす
                            withBlock9.UsedSupportAttack = (short)(withBlock9.UsedSupportAttack + 1);

                            // 同時援護攻撃の残り回数を減らす
                            SelectedUnit.UsedSyncAttack = (short)(SelectedUnit.UsedSyncAttack + 1);
                        }
                    }

                    support_attack_done = true;

                    // 防御側のユニットがかばわれた場合は本来の防御ユニットデータと
                    // 入れ替えて記録
                    if (SupportGuardUnit is object)
                    {
                        defense_target = SupportGuardUnit;
                        defense_target_hp_ratio = SupportGuardUnitHPRatio;
                    }
                }
            }

            // 反撃の実施
            {
                var withBlock10 = SelectedTarget;
                if (def_mode != "先制攻撃")
                {
                    if (withBlock10.Status_Renamed == "出撃" & withBlock10.Party != "味方" & SelectedUnit.Status_Renamed == "出撃")
                    {
                        // まだ武器は使用可能か？
                        if (SelectedTWeapon > 0)
                        {
                            string argref_mode4 = "移動前";
                            if (SelectedTWeapon > withBlock10.CountWeapon())
                            {
                                SelectedTWeapon = -1;
                            }
                            else if ((twname ?? "") != (withBlock10.Weapon(SelectedTWeapon).Name ?? "") | !withBlock10.IsWeaponAvailable(SelectedTWeapon, ref argref_mode4))
                            {
                                SelectedTWeapon = -1;
                            }
                        }

                        if (SelectedTWeapon > 0)
                        {
                            if (!withBlock10.IsTargetWithinRange(SelectedTWeapon, ref SelectedUnit))
                            {
                                // 敵が射程外に逃げていたら武器を再選択
                                string argamode2 = "反撃";
                                int argmax_prob2 = 0;
                                int argmax_dmg2 = 0;
                                SelectedTWeapon = COM.SelectWeapon(ref SelectedTarget, ref SelectedUnit, ref argamode2, max_prob: ref argmax_prob2, max_dmg: ref argmax_dmg2);
                            }
                        }

                        // 行動不能な場合
                        if (withBlock10.MaxAction() == 0)
                        {
                            SelectedTWeapon = -1;
                        }

                        // 魅了された場合
                        object argIndex11 = "魅了";
                        if (withBlock10.IsConditionSatisfied(ref argIndex11) & ReferenceEquals(withBlock10.Master, SelectedUnit))
                        {
                            SelectedTWeapon = -1;
                        }

                        // 憑依された場合
                        object argIndex12 = "憑依";
                        if (withBlock10.IsConditionSatisfied(ref argIndex12))
                        {
                            if ((withBlock10.Master.Party ?? "") == (SelectedUnit.Party ?? ""))
                            {
                                SelectedWeapon = -1;
                            }
                        }

                        // 踊らされた場合
                        object argIndex13 = "踊り";
                        if (withBlock10.IsConditionSatisfied(ref argIndex13))
                        {
                            SelectedTWeapon = -1;
                        }

                        if (SelectedTWeapon > 0 & string.IsNullOrEmpty(def_mode))
                        {
                            // 反撃を実施
                            string argattr10 = "自";
                            if (withBlock10.IsWeaponClassifiedAs(SelectedTWeapon, ref argattr10))
                            {
                                is_suiside = true;
                            }

                            withBlock10.Attack(SelectedTWeapon, SelectedUnit, "", "");

                            // 攻撃側のユニットがかばわれた場合は攻撃側のターゲットを再設定
                            if (SupportGuardUnit2 is object)
                            {
                                attack_target = SupportGuardUnit2;
                                attack_target_hp_ratio = SupportGuardUnitHPRatio2;
                            }
                        }
                        else if (SelectedTWeapon == 0 & withBlock10.x == tx & withBlock10.y == ty)
                        {
                            // 反撃出来る武器がなかった場合は射程外メッセージを表示
                            string argmain_situation9 = "射程外";
                            string argsub_situation9 = "";
                            if (withBlock10.IsAnimationDefined(ref argmain_situation9, sub_situation: ref argsub_situation9))
                            {
                                string argmain_situation7 = "射程外";
                                string argsub_situation7 = "";
                                withBlock10.PlayAnimation(ref argmain_situation7, sub_situation: ref argsub_situation7);
                            }
                            else
                            {
                                string argmain_situation8 = "射程外";
                                string argsub_situation8 = "";
                                withBlock10.SpecialEffect(ref argmain_situation8, sub_situation: ref argsub_situation8);
                            }

                            string argSituation1 = "射程外";
                            string argmsg_mode1 = "";
                            withBlock10.PilotMessage(ref argSituation1, msg_mode: ref argmsg_mode1);
                        }
                        else
                        {
                            SelectedTWeapon = -1;
                        }
                    }
                    else
                    {
                        SelectedTWeapon = -1;
                    }
                }
            }

            // サポートアタック
            if (SupportAttackUnit is object)
            {
                if (SupportAttackUnit.Status_Renamed != "出撃" | SelectedUnit.Status_Renamed != "出撃" | SelectedTarget.Status_Renamed != "出撃" | support_attack_done)
                {
                    // UPGRADE_NOTE: オブジェクト SupportAttackUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                    SupportAttackUnit = null;
                }
            }

            if (SupportAttackUnit is object)
            {
                {
                    var withBlock11 = SupportAttackUnit;
                    // サポートアタックに使う武器を決定
                    string argamode3 = "サポートアタック";
                    int argmax_prob3 = 0;
                    int argmax_dmg3 = 0;
                    w2 = COM.SelectWeapon(ref SupportAttackUnit, ref SelectedTarget, ref argamode3, max_prob: ref argmax_prob3, max_dmg: ref argmax_dmg3);
                    if (w2 > 0)
                    {
                        // サポートアタックを実施
                        Map.MaskData[withBlock11.x, withBlock11.y] = false;
                        if (!SRC.BattleAnimation)
                        {
                            GUI.MaskScreen();
                        }

                        string argmain_situation11 = "サポートアタック開始";
                        string argsub_situation11 = "";
                        if (withBlock11.IsAnimationDefined(ref argmain_situation11, sub_situation: ref argsub_situation11))
                        {
                            string argmain_situation10 = "サポートアタック開始";
                            string argsub_situation10 = "";
                            withBlock11.PlayAnimation(ref argmain_situation10, sub_situation: ref argsub_situation10);
                        }

                        object argu21 = SupportAttackUnit;
                        GUI.UpdateMessageForm(ref SelectedTarget, ref argu21);
                        withBlock11.Attack(w2, SelectedTarget, "援護攻撃", def_mode);
                    }
                }

                // 後始末
                {
                    var withBlock12 = SupportAttackUnit.CurrentForm();
                    string argmain_situation13 = "サポートアタック終了";
                    string argsub_situation13 = "";
                    if (withBlock12.IsAnimationDefined(ref argmain_situation13, sub_situation: ref argsub_situation13))
                    {
                        string argmain_situation12 = "サポートアタック終了";
                        string argsub_situation12 = "";
                        withBlock12.PlayAnimation(ref argmain_situation12, sub_situation: ref argsub_situation12);
                    }

                    // サポートアタックの残り回数を減らす
                    if (w2 > 0)
                    {
                        withBlock12.UsedSupportAttack = (short)(withBlock12.UsedSupportAttack + 1);
                    }
                }

                // 防御側のユニットがかばわれた場合は本来の防御ユニットデータと
                // 入れ替えて記録
                if (SupportGuardUnit is object)
                {
                    defense_target = SupportGuardUnit;
                    defense_target_hp_ratio = SupportGuardUnitHPRatio;
                }
            }

            SelectedTarget = SelectedTarget.CurrentForm();
            {
                var withBlock13 = SelectedUnit;
                if (withBlock13.Status_Renamed == "出撃")
                {
                    // 攻撃をかけたユニットがまだ生き残っていれば経験値＆資金を獲得

                    if (SelectedTarget.Status_Renamed == "破壊" & !is_suiside)
                    {
                        // 敵を破壊した場合

                        // 経験値を獲得
                        string argexp_situation = "破壊";
                        string argexp_mode = "";
                        withBlock13.GetExp(ref SelectedTarget, ref argexp_situation, exp_mode: ref argexp_mode);
                        string argoname = "合体技パートナー経験値無効";
                        if (!Expression.IsOptionDefined(ref argoname))
                        {
                            var loopTo2 = (short)Information.UBound(partners);
                            for (i = 1; i <= loopTo2; i++)
                            {
                                string argexp_situation1 = "破壊";
                                string argexp_mode1 = "パートナー";
                                partners[i].CurrentForm().GetExp(ref SelectedTarget, ref argexp_situation1, ref argexp_mode1);
                            }
                        }

                        // 獲得する資金を算出
                        earnings = SelectedTarget.Value / 2;

                        // スペシャルパワーによる獲得資金増加
                        string argsptype1 = "獲得資金増加";
                        if (withBlock13.IsUnderSpecialPowerEffect(ref argsptype1))
                        {
                            string argsname = "獲得資金増加";
                            earnings = (int)(earnings * (1d + 0.1d * withBlock13.SpecialPowerEffectLevel(ref argsname)));
                        }

                        // パイロット能力による獲得資金増加
                        string argsname1 = "資金獲得";
                        if (withBlock13.IsSkillAvailable(ref argsname1))
                        {
                            string argsptype2 = "獲得資金増加";
                            string argoname1 = "収得効果重複";
                            if (!withBlock13.IsUnderSpecialPowerEffect(ref argsptype2) | Expression.IsOptionDefined(ref argoname1))
                            {
                                earnings = (int)GeneralLib.MinDbl(earnings * ((10d + withBlock13.SkillLevel("資金獲得", 5d)) / 10d), 999999999d);
                            }
                        }

                        // 資金を獲得
                        SRC.IncrMoney(earnings);
                        if (earnings > 0)
                        {
                            string argtname = "資金";
                            GUI.DisplaySysMessage(Microsoft.VisualBasic.Compatibility.VB6.Support.Format(earnings) + "の" + Expression.Term(ref argtname, ref SelectedUnit) + "を得た。");
                        }

                        // スペシャルパワー効果「敵破壊時再行動」
                        string argsptype3 = "敵破壊時再行動";
                        if (withBlock13.IsUnderSpecialPowerEffect(ref argsptype3))
                        {
                            withBlock13.UsedAction = (short)(withBlock13.UsedAction - 1);
                        }
                    }
                    else
                    {
                        // 相手を破壊できなかった場合

                        // 経験値を獲得
                        string argexp_situation2 = "攻撃";
                        string argexp_mode2 = "";
                        withBlock13.GetExp(ref SelectedTarget, ref argexp_situation2, exp_mode: ref argexp_mode2);
                        string argoname2 = "合体技パートナー経験値無効";
                        if (!Expression.IsOptionDefined(ref argoname2))
                        {
                            var loopTo3 = (short)Information.UBound(partners);
                            for (i = 1; i <= loopTo3; i++)
                            {
                                string argexp_situation3 = "攻撃";
                                string argexp_mode3 = "パートナー";
                                partners[i].CurrentForm().GetExp(ref SelectedTarget, ref argexp_situation3, ref argexp_mode3);
                            }
                        }
                    }

                    // スペシャルパワー「獲得資金増加」「獲得経験値増加」の効果はここで削除する
                    string argstype = "戦闘終了";
                    withBlock13.RemoveSpecialPowerInEffect(ref argstype);
                    if (earnings > 0)
                    {
                        string argstype1 = "敵破壊";
                        withBlock13.RemoveSpecialPowerInEffect(ref argstype1);
                    }
                }
            }

            {
                var withBlock14 = SelectedTarget;
                if (withBlock14.Status_Renamed == "出撃")
                {
                    // 持続期間が「戦闘終了」のスペシャルパワー効果を削除
                    string argstype2 = "戦闘終了";
                    withBlock14.RemoveSpecialPowerInEffect(ref argstype2);
                }
            }

            GUI.CloseMessageForm();
            Status.ClearUnitStatus();

            // 状態＆データ更新
            {
                var withBlock15 = attack_target.CurrentForm();
                withBlock15.UpdateCondition();
                withBlock15.Update();
            }

            if (SupportAttackUnit is object)
            {
                {
                    var withBlock16 = SupportAttackUnit.CurrentForm();
                    withBlock16.UpdateCondition();
                    withBlock16.Update();
                }
            }

            {
                var withBlock17 = defense_target.CurrentForm();
                withBlock17.UpdateCondition();
                withBlock17.Update();
            }

            if (defense_target2 is object)
            {
                {
                    var withBlock18 = defense_target2.CurrentForm();
                    withBlock18.UpdateCondition();
                    withBlock18.Update();
                }
            }

            // 破壊＆損傷率イベント発生

            if (SelectedWeapon <= 0)
            {
                SelectedWeaponName = "";
            }

            if (SelectedTWeapon <= 0)
            {
                SelectedTWeaponName = "";
            }

            // 攻撃を受けた攻撃側ユニット
            {
                var withBlock19 = attack_target.CurrentForm();
                if (withBlock19.Status_Renamed == "破壊")
                {
                    Event_Renamed.HandleEvent("破壊", withBlock19.MainPilot().ID);
                }
                else if (withBlock19.Status_Renamed == "出撃" & withBlock19.HP / (double)withBlock19.MaxHP < attack_target_hp_ratio)
                {
                    Event_Renamed.HandleEvent("損傷率", withBlock19.MainPilot().ID, 100 * (withBlock19.MaxHP - withBlock19.HP) / withBlock19.MaxHP);
                }
            }

            if (SRC.IsScenarioFinished)
            {
                GUI.UnlockGUI();
                SRC.IsScenarioFinished = false;
                SelectedPartners = new Unit[1];
                return;
            }

            if (SRC.IsCanceled)
            {
                SRC.IsCanceled = false;
                goto EndAttack;
            }

            SelectedUnit = SelectedUnit.CurrentForm();

            // ターゲット側のイベント処理を行うためにユニットの入れ替えを行う
            SaveSelections();
            SwapSelections();

            // 攻撃を受けた防御側ユニット
            {
                var withBlock20 = defense_target.CurrentForm();
                if (withBlock20.CountPilot() > 0)
                {
                    if (withBlock20.Status_Renamed == "破壊")
                    {
                        Event_Renamed.HandleEvent("破壊", withBlock20.MainPilot().ID);
                    }
                    else if (withBlock20.Status_Renamed == "出撃" & withBlock20.HP / (double)withBlock20.MaxHP < defense_target_hp_ratio)
                    {
                        Event_Renamed.HandleEvent("損傷率", withBlock20.MainPilot().ID, 100 * (withBlock20.MaxHP - withBlock20.HP) / withBlock20.MaxHP);
                    }
                }
            }

            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
                SelectedPartners = new Unit[1];
                return;
            }

            // 攻撃を受けた防御側ユニットその2
            if (defense_target2 is object)
            {
                if (!ReferenceEquals(defense_target2.CurrentForm(), defense_target.CurrentForm()))
                {
                    {
                        var withBlock21 = defense_target2.CurrentForm();
                        if (withBlock21.CountPilot() > 0)
                        {
                            if (withBlock21.Status_Renamed == "破壊")
                            {
                                Event_Renamed.HandleEvent("破壊", withBlock21.MainPilot().ID);
                            }
                            else if (withBlock21.Status_Renamed == "出撃" & withBlock21.HP / (double)withBlock21.MaxHP < defense_target2_hp_ratio)
                            {
                                Event_Renamed.HandleEvent("損傷率", withBlock21.MainPilot().ID, 100 * (withBlock21.MaxHP - withBlock21.HP) / withBlock21.MaxHP);
                            }
                        }
                    }
                }
            }

            RestoreSelections();
            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
                SelectedPartners = new Unit[1];
                GUI.UnlockGUI();
                return;
            }

            if (SRC.IsCanceled)
            {
                SRC.IsCanceled = false;
                SelectedPartners = new Unit[1];
                GUI.UnlockGUI();
                return;
            }

            // 武器の使用後イベント
            if (SelectedUnit.Status_Renamed == "出撃" & SelectedWeapon > 0)
            {
                Event_Renamed.HandleEvent("使用後", SelectedUnit.MainPilot().ID, wname);
                if (SRC.IsScenarioFinished)
                {
                    SRC.IsScenarioFinished = false;
                    SelectedPartners = new Unit[1];
                    GUI.UnlockGUI();
                    return;
                }

                if (SRC.IsCanceled)
                {
                    SRC.IsCanceled = false;
                    SelectedPartners = new Unit[1];
                    GUI.UnlockGUI();
                    return;
                }
            }

            if (SelectedTarget.Status_Renamed == "出撃" & SelectedTWeapon > 0)
            {
                SaveSelections();
                SwapSelections();
                Event_Renamed.HandleEvent("使用後", SelectedUnit.MainPilot().ID, twname);
                RestoreSelections();
                if (SRC.IsScenarioFinished)
                {
                    SRC.IsScenarioFinished = false;
                    SelectedPartners = new Unit[1];
                    GUI.UnlockGUI();
                    return;
                }

                if (SRC.IsCanceled)
                {
                    SRC.IsCanceled = false;
                    SelectedPartners = new Unit[1];
                    GUI.UnlockGUI();
                    return;
                }
            }

            // 攻撃後イベント
            if (SelectedUnit.Status_Renamed == "出撃" & SelectedTarget.Status_Renamed == "出撃")
            {
                Event_Renamed.HandleEvent("攻撃後", SelectedUnit.MainPilot().ID, SelectedTarget.MainPilot().ID);
                if (SRC.IsScenarioFinished)
                {
                    SRC.IsScenarioFinished = false;
                    SelectedPartners = new Unit[1];
                    GUI.UnlockGUI();
                    return;
                }

                if (SRC.IsCanceled)
                {
                    SRC.IsCanceled = false;
                    SelectedPartners = new Unit[1];
                    GUI.UnlockGUI();
                    return;
                }
            }

            // もし敵が移動していれば進入イベント
            {
                var withBlock22 = SelectedTarget;
                // UPGRADE_NOTE: オブジェクト SelectedTarget をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                SelectedTarget = null;
                if (withBlock22.Status_Renamed == "出撃")
                {
                    if (withBlock22.x != tx | withBlock22.y != ty)
                    {
                        Event_Renamed.HandleEvent("進入", withBlock22.MainPilot().ID, withBlock22.x, withBlock22.y);
                        if (SRC.IsScenarioFinished)
                        {
                            SRC.IsScenarioFinished = false;
                            SelectedPartners = new Unit[1];
                            GUI.UnlockGUI();
                            return;
                        }

                        if (SRC.IsCanceled)
                        {
                            SRC.IsCanceled = false;
                            SelectedPartners = new Unit[1];
                            GUI.UnlockGUI();
                            return;
                        }
                    }
                }
            }

            EndAttack:
            ;


            // 合体技のパートナーの行動数を減らす
            string argoname3 = "合体技パートナー行動数無消費";
            if (!Expression.IsOptionDefined(ref argoname3))
            {
                var loopTo4 = (short)Information.UBound(partners);
                for (i = 1; i <= loopTo4; i++)
                    partners[i].CurrentForm().UseAction();
            }

            SelectedPartners = new Unit[1];

            // ハイパーモード＆ノーマルモードの自動発動をチェック
            SRC.UList.CheckAutoHyperMode();
            SRC.UList.CheckAutoNormalMode();

            // ADD START MARGE
            // 再移動
            if (is_p_weapon & SelectedUnit.Status_Renamed == "出撃")
            {
                string argsname2 = "遊撃";
                if (SelectedUnit.MainPilot().IsSkillAvailable(ref argsname2) & SelectedUnit.Speed * 2 > SelectedUnitMoveCost)
                {
                    // 進入イベント
                    if (SelectedUnitMoveCost > 0)
                    {
                        Event_Renamed.HandleEvent("進入", SelectedUnit.MainPilot().ID, SelectedUnit.x, SelectedUnit.y);
                        if (SRC.IsScenarioFinished)
                        {
                            SRC.IsScenarioFinished = false;
                            return;
                        }
                    }

                    // ユニットが既に出撃していない？
                    if (SelectedUnit.Status_Renamed != "出撃")
                    {
                        GUI.RedrawScreen();
                        Status.ClearUnitStatus();
                        return;
                    }

                    SelectedCommand = "再移動";
                    Map.AreaInSpeed(ref SelectedUnit);
                    string argoname4 = "大型マップ";
                    if (!Expression.IsOptionDefined(ref argoname4))
                    {
                        GUI.Center(SelectedUnit.x, SelectedUnit.y);
                    }

                    GUI.MaskScreen();
                    if (GUI.NewGUIMode)
                    {
                        Application.DoEvents();
                        Status.ClearUnitStatus();
                    }
                    else
                    {
                        Status.DisplayUnitStatus(ref SelectedUnit);
                    }

                    CommandState = "ターゲット選択";
                    return;
                }
            }
            // ADD END MARGE

            // 行動数を減らす
            WaitCommand();
        }

        // マップ攻撃による「攻撃」コマンドを終了
        // MOD START MARGE
        // Public Sub MapAttackCommand()
        private static void MapAttackCommand()
        {
            // MOD END MARGE
            short i;
            var partners = default(Unit[]);
            // ADD START MARGE
            bool is_p_weapon;
            // ADD END MARGE

            {
                var withBlock = SelectedUnit;
                // ADD START MARGE
                // 移動後使用後可能な武器か記録しておく
                string argattr = "移動後攻撃可";
                is_p_weapon = withBlock.IsWeaponClassifiedAs(SelectedWeapon, ref argattr);
                // ADD END MARGE
                // 攻撃目標地点を選択して初めて攻撃範囲が分かるタイプのマップ攻撃
                // の場合は再度プレイヤーの選択を促す必要がある
                if (CommandState == "ターゲット選択" | CommandState == "移動後ターゲット選択")
                {
                    string argattr4 = "Ｍ投";
                    string argattr5 = "Ｍ移";
                    string argattr6 = "Ｍ線";
                    if (withBlock.IsWeaponClassifiedAs(SelectedWeapon, ref argattr4))
                    {
                        if (CommandState == "ターゲット選択")
                        {
                            CommandState = "マップ攻撃使用";
                        }
                        else
                        {
                            CommandState = "移動後マップ攻撃使用";
                        }

                        // 攻撃目標地点
                        SelectedX = GUI.PixelToMapX((short)GUI.MouseX);
                        SelectedY = GUI.PixelToMapY((short)GUI.MouseY);

                        // 攻撃範囲を設定
                        string argattr3 = "識";
                        string argsptype = "識別攻撃";
                        if (withBlock.IsWeaponClassifiedAs(SelectedWeapon, ref argattr3) | withBlock.IsUnderSpecialPowerEffect(ref argsptype))
                        {
                            string argattr1 = "Ｍ投";
                            string arguparty = "味方の敵";
                            Map.AreaInRange(SelectedX, SelectedY, (short)withBlock.WeaponLevel(SelectedWeapon, ref argattr1), 1, ref arguparty);
                        }
                        else
                        {
                            string argattr2 = "Ｍ投";
                            string arguparty1 = "すべて";
                            Map.AreaInRange(SelectedX, SelectedY, (short)withBlock.WeaponLevel(SelectedWeapon, ref argattr2), 1, ref arguparty1);
                        }

                        GUI.MaskScreen();
                        return;
                    }
                    else if (withBlock.IsWeaponClassifiedAs(SelectedWeapon, ref argattr5))
                    {
                        // 攻撃目標地点
                        SelectedX = GUI.PixelToMapX((short)GUI.MouseX);
                        SelectedY = GUI.PixelToMapY((short)GUI.MouseY);

                        // 攻撃目標地点に他のユニットがいては駄目
                        if (Map.MapDataForUnit[SelectedX, SelectedY] is object)
                        {
                            GUI.MaskScreen();
                            return;
                        }

                        if (CommandState == "ターゲット選択")
                        {
                            CommandState = "マップ攻撃使用";
                        }
                        else
                        {
                            CommandState = "移動後マップ攻撃使用";
                        }

                        // 攻撃範囲を設定
                        Map.AreaInPointToPoint(withBlock.x, withBlock.y, SelectedX, SelectedY);
                        GUI.MaskScreen();
                        return;
                    }
                    else if (withBlock.IsWeaponClassifiedAs(SelectedWeapon, ref argattr6))
                    {
                        if (CommandState == "ターゲット選択")
                        {
                            CommandState = "マップ攻撃使用";
                        }
                        else
                        {
                            CommandState = "移動後マップ攻撃使用";
                        }

                        // 攻撃目標地点
                        SelectedX = GUI.PixelToMapX((short)GUI.MouseX);
                        SelectedY = GUI.PixelToMapY((short)GUI.MouseY);

                        // 攻撃範囲を設定
                        Map.AreaInPointToPoint(withBlock.x, withBlock.y, SelectedX, SelectedY);
                        GUI.MaskScreen();
                        return;
                    }
                }

                // 合体技パートナーの設定
                string argattr7 = "合";
                if (withBlock.IsWeaponClassifiedAs(SelectedWeapon, ref argattr7))
                {
                    string argctype_Renamed = "武装";
                    withBlock.CombinationPartner(ref argctype_Renamed, SelectedWeapon, ref partners);
                }
                else
                {
                    SelectedPartners = new Unit[1];
                    partners = new Unit[1];
                }

                if (GUI.MainWidth != 15)
                {
                    Status.ClearUnitStatus();
                }

                GUI.LockGUI();
                SelectedTWeapon = 0;

                // マップ攻撃による攻撃を行う
                withBlock.MapAttack(SelectedWeapon, SelectedX, SelectedY);
                SelectedUnit = withBlock.CurrentForm();
                // UPGRADE_NOTE: オブジェクト SelectedTarget をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                SelectedTarget = null;
                if (SRC.IsScenarioFinished)
                {
                    SRC.IsScenarioFinished = false;
                    SelectedPartners = new Unit[1];
                    GUI.UnlockGUI();
                    return;
                }

                if (SRC.IsCanceled)
                {
                    SRC.IsCanceled = false;
                    SelectedPartners = new Unit[1];
                    WaitCommand();
                    return;
                }
            }

            // 合体技のパートナーの行動数を減らす
            string argoname = "合体技パートナー行動数無消費";
            if (!Expression.IsOptionDefined(ref argoname))
            {
                var loopTo = (short)Information.UBound(partners);
                for (i = 1; i <= loopTo; i++)
                    partners[i].CurrentForm().UseAction();
            }

            SelectedPartners = new Unit[1];

            // ADD START MARGE
            // 再移動
            if (is_p_weapon & SelectedUnit.Status_Renamed == "出撃")
            {
                string argsname = "遊撃";
                if (SelectedUnit.MainPilot().IsSkillAvailable(ref argsname) & SelectedUnit.Speed * 2 > SelectedUnitMoveCost)
                {
                    // 進入イベント
                    if (SelectedUnitMoveCost > 0)
                    {
                        Event_Renamed.HandleEvent("進入", SelectedUnit.MainPilot().ID, SelectedUnit.x, SelectedUnit.y);
                        if (SRC.IsScenarioFinished)
                        {
                            SRC.IsScenarioFinished = false;
                            return;
                        }
                    }

                    // ユニットが既に出撃していない？
                    if (SelectedUnit.Status_Renamed != "出撃")
                    {
                        GUI.RedrawScreen();
                        Status.ClearUnitStatus();
                        return;
                    }

                    SelectedCommand = "再移動";
                    Map.AreaInSpeed(ref SelectedUnit);
                    string argoname1 = "大型マップ";
                    if (!Expression.IsOptionDefined(ref argoname1))
                    {
                        GUI.Center(SelectedUnit.x, SelectedUnit.y);
                    }

                    GUI.MaskScreen();
                    if (GUI.NewGUIMode)
                    {
                        Application.DoEvents();
                        Status.ClearUnitStatus();
                    }
                    else
                    {
                        Status.DisplayUnitStatus(ref SelectedUnit);
                    }

                    CommandState = "ターゲット選択";
                    return;
                }
            }
            // ADD END MARGE

            // 行動終了
            WaitCommand();
        }


        // 「アビリティ」コマンドを開始
        // is_item=True の場合は「アイテム」コマンドによる使い捨てアイテムのアビリティ
        // MOD STAR MARGE
        // Public Sub StartAbilityCommand(Optional ByVal is_item As Boolean)
        private static void StartAbilityCommand(bool is_item = false)
        {
            // MOD END MARGE
            short i, j;
            Unit t;
            short min_range, max_range;
            string cap;
            GUI.LockGUI();

            // 使用するアビリティを選択
            if (is_item)
            {
                cap = "アイテム選択";
            }
            else
            {
                string argtname = "アビリティ";
                cap = Expression.Term(ref argtname, ref SelectedUnit) + "選択";
            }

            if (CommandState == "コマンド選択")
            {
                string arglb_mode = "移動前";
                SelectedAbility = GUI.AbilityListBox(ref SelectedUnit, ref cap, ref arglb_mode, is_item);
            }
            else
            {
                string arglb_mode1 = "移動後";
                SelectedAbility = GUI.AbilityListBox(ref SelectedUnit, ref cap, ref arglb_mode1, is_item);
            }

            // キャンセル
            if (SelectedAbility == 0)
            {
                if (SRC.AutoMoveCursor)
                {
                    GUI.RestoreCursorPos();
                }

                CancelCommand();
                GUI.UnlockGUI();
                return;
            }

            // アビリティ専用ＢＧＭがあればそれを演奏
            string BGM;
            {
                var withBlock = SelectedUnit;
                string argfname = "アビリティＢＧＭ";
                if (withBlock.IsFeatureAvailable(ref argfname))
                {
                    var loopTo = withBlock.CountFeature();
                    for (i = 1; i <= loopTo; i++)
                    {
                        string localFeature() { object argIndex1 = i; var ret = withBlock.Feature(ref argIndex1); return ret; }

                        string localFeatureData2() { object argIndex1 = i; var ret = withBlock.FeatureData(ref argIndex1); return ret; }

                        string localLIndex() { string arglist = hs8866f19275ca4c5cbfc3bb7415f2da30(); var ret = GeneralLib.LIndex(ref arglist, 1); return ret; }

                        if (localFeature() == "アビリティＢＧＭ" & (localLIndex() ?? "") == (withBlock.Ability(SelectedAbility).Name ?? ""))
                        {
                            string localFeatureData() { object argIndex1 = i; var ret = withBlock.FeatureData(ref argIndex1); return ret; }

                            string localFeatureData1() { object argIndex1 = i; var ret = withBlock.FeatureData(ref argIndex1); return ret; }

                            string argmidi_name = Strings.Mid(localFeatureData(), Strings.InStr(localFeatureData1(), " ") + 1);
                            BGM = Sound.SearchMidiFile(ref argmidi_name);
                            if (Strings.Len(BGM) > 0)
                            {
                                Sound.ChangeBGM(ref BGM);
                            }

                            break;
                        }
                    }
                }
            }

            // 射程0のアビリティはその場で実行
            var is_transformation = default(bool);
            if (SelectedUnit.Ability(SelectedAbility).MaxRange == 0)
            {
                SelectedTarget = SelectedUnit;

                // 変身アビリティであるか判定
                var loopTo1 = SelectedUnit.Ability(SelectedAbility).CountEffect();
                for (i = 1; i <= loopTo1; i++)
                {
                    object argIndex1 = i;
                    if (SelectedUnit.Ability(SelectedAbility).EffectType(ref argIndex1) == "変身")
                    {
                        is_transformation = true;
                        break;
                    }
                }

                SelectedAbilityName = SelectedUnit.Ability(SelectedAbility).Name;

                // 使用イベント
                Event_Renamed.HandleEvent("使用", SelectedUnit.MainPilot().ID, SelectedAbilityName);
                if (SRC.IsScenarioFinished)
                {
                    SRC.IsScenarioFinished = false;
                    GUI.UnlockGUI();
                    return;
                }

                if (SRC.IsCanceled)
                {
                    SRC.IsCanceled = false;
                    WaitCommand();
                    return;
                }

                // アビリティを実行
                SelectedUnit.ExecuteAbility(SelectedAbility, ref SelectedUnit);
                SelectedUnit = SelectedUnit.CurrentForm();
                GUI.CloseMessageForm();

                // 破壊イベント
                {
                    var withBlock1 = SelectedUnit;
                    if (withBlock1.Status_Renamed == "破壊")
                    {
                        if (withBlock1.CountPilot() > 0)
                        {
                            Event_Renamed.HandleEvent("破壊", withBlock1.MainPilot().ID);
                            if (SRC.IsScenarioFinished)
                            {
                                SRC.IsScenarioFinished = false;
                                GUI.UnlockGUI();
                                return;
                            }

                            if (SRC.IsCanceled)
                            {
                                SRC.IsCanceled = false;
                                GUI.UnlockGUI();
                                return;
                            }
                        }

                        WaitCommand();
                        return;
                    }
                }

                // 使用後イベント
                {
                    var withBlock2 = SelectedUnit;
                    if (withBlock2.CountPilot() > 0)
                    {
                        Event_Renamed.HandleEvent("使用後", withBlock2.MainPilot().ID, SelectedAbilityName);
                        if (SRC.IsScenarioFinished)
                        {
                            SRC.IsScenarioFinished = false;
                            GUI.UnlockGUI();
                            return;
                        }

                        if (SRC.IsCanceled)
                        {
                            SRC.IsCanceled = false;
                            GUI.UnlockGUI();
                            return;
                        }
                    }
                }

                // 変身アビリティの場合は行動終了しない
                if (!is_transformation | CommandState == "移動後コマンド選択")
                {
                    WaitCommand();
                }
                else
                {
                    if (SelectedUnit.Status_Renamed == "出撃")
                    {
                        // カーソル自動移動
                        if (SRC.AutoMoveCursor)
                        {
                            string argcursor_mode = "ユニット選択";
                            GUI.MoveCursorPos(ref argcursor_mode, SelectedUnit);
                        }

                        Status.DisplayUnitStatus(ref SelectedUnit);
                    }
                    else
                    {
                        Status.ClearUnitStatus();
                    }

                    CommandState = "ユニット選択";
                    GUI.UnlockGUI();
                }

                return;
            }

            var partners = default(Unit[]);
            {
                var withBlock3 = SelectedUnit;
                // マップ型アビリティかどうかで今後のコマンド処理の進行の仕方が異なる
                if (is_item)
                {
                    string argattr = "Ｍ";
                    if (withBlock3.IsAbilityClassifiedAs(SelectedAbility, ref argattr))
                    {
                        SelectedCommand = "マップアイテム";
                    }
                    else
                    {
                        SelectedCommand = "アイテム";
                    }
                }
                else
                {
                    string argattr1 = "Ｍ";
                    if (withBlock3.IsAbilityClassifiedAs(SelectedAbility, ref argattr1))
                    {
                        SelectedCommand = "マップアビリティ";
                    }
                    else
                    {
                        SelectedCommand = "アビリティ";
                    }
                }

                // アビリティの射程を求めておく
                min_range = withBlock3.AbilityMinRange(SelectedAbility);
                max_range = withBlock3.AbilityMaxRange(SelectedAbility);

                // アビリティの効果範囲を設定
                string argattr3 = "Ｍ直";
                string argattr4 = "Ｍ拡";
                string argattr5 = "Ｍ扇";
                string argattr6 = "Ｍ移";
                if (withBlock3.IsAbilityClassifiedAs(SelectedAbility, ref argattr3))
                {
                    Map.AreaInCross(withBlock3.x, withBlock3.y, min_range, ref max_range);
                }
                else if (withBlock3.IsAbilityClassifiedAs(SelectedAbility, ref argattr4))
                {
                    Map.AreaInWideCross(withBlock3.x, withBlock3.y, min_range, ref max_range);
                }
                else if (withBlock3.IsAbilityClassifiedAs(SelectedAbility, ref argattr5))
                {
                    string argattr2 = "Ｍ扇";
                    Map.AreaInSectorCross(withBlock3.x, withBlock3.y, min_range, ref max_range, (short)withBlock3.AbilityLevel(SelectedAbility, ref argattr2));
                }
                else if (withBlock3.IsAbilityClassifiedAs(SelectedAbility, ref argattr6))
                {
                    Map.AreaInMoveAction(ref SelectedUnit, max_range);
                }
                else
                {
                    string arguparty = "すべて";
                    Map.AreaInRange(withBlock3.x, withBlock3.y, max_range, min_range, ref arguparty);
                }

                // 射程１の合体技はパートナーで相手を取り囲んでいないと使用できない
                string argattr7 = "合";
                string argattr8 = "Ｍ";
                if (withBlock3.IsAbilityClassifiedAs(SelectedAbility, ref argattr7) & !withBlock3.IsAbilityClassifiedAs(SelectedAbility, ref argattr8) & withBlock3.Ability(SelectedAbility).MaxRange == 1)
                {
                    for (i = 1; i <= 4; i++)
                    {
                        // UPGRADE_NOTE: オブジェクト t をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                        t = null;
                        switch (i)
                        {
                            case 1:
                                {
                                    if (withBlock3.x > 1)
                                    {
                                        t = Map.MapDataForUnit[withBlock3.x - 1, withBlock3.y];
                                    }

                                    break;
                                }

                            case 2:
                                {
                                    if (withBlock3.x < Map.MapWidth)
                                    {
                                        t = Map.MapDataForUnit[withBlock3.x + 1, withBlock3.y];
                                    }

                                    break;
                                }

                            case 3:
                                {
                                    if (withBlock3.y > 1)
                                    {
                                        t = Map.MapDataForUnit[withBlock3.x, withBlock3.y - 1];
                                    }

                                    break;
                                }

                            case 4:
                                {
                                    if (withBlock3.y < Map.MapHeight)
                                    {
                                        t = Map.MapDataForUnit[withBlock3.x, withBlock3.y + 1];
                                    }

                                    break;
                                }
                        }

                        if (t is object)
                        {
                            if (withBlock3.IsAlly(ref t))
                            {
                                string argctype_Renamed = "アビリティ";
                                withBlock3.CombinationPartner(ref argctype_Renamed, SelectedAbility, ref partners, t.x, t.y);
                                if (Information.UBound(partners) == 0)
                                {
                                    Map.MaskData[t.x, t.y] = true;
                                }
                            }
                        }
                    }
                }

                // ユニットがいるマスの処理
                string argattr9 = "Ｍ投";
                string argattr10 = "Ｍ線";
                string argattr11 = "Ｍ移";
                if (!withBlock3.IsAbilityClassifiedAs(SelectedAbility, ref argattr9) & !withBlock3.IsAbilityClassifiedAs(SelectedAbility, ref argattr10) & !withBlock3.IsAbilityClassifiedAs(SelectedAbility, ref argattr11))
                {
                    var loopTo2 = (short)GeneralLib.MinLng(withBlock3.x + max_range, Map.MapWidth);
                    for (i = (short)GeneralLib.MaxLng(withBlock3.x - max_range, 1); i <= loopTo2; i++)
                    {
                        var loopTo3 = (short)GeneralLib.MinLng(withBlock3.y + max_range, Map.MapHeight);
                        for (j = (short)GeneralLib.MaxLng(withBlock3.y - max_range, 1); j <= loopTo3; j++)
                        {
                            if (!Map.MaskData[i, j])
                            {
                                t = Map.MapDataForUnit[i, j];
                                if (t is object)
                                {
                                    // 有効？
                                    if (withBlock3.IsAbilityEffective(SelectedAbility, ref t))
                                    {
                                        Map.MaskData[i, j] = false;
                                    }
                                    else
                                    {
                                        Map.MaskData[i, j] = true;
                                    }
                                }
                            }
                        }
                    }
                }

                // 支援専用アビリティは自分には使用できない
                if (!Map.MaskData[withBlock3.x, withBlock3.y])
                {
                    string argattr12 = "援";
                    if (withBlock3.IsAbilityClassifiedAs(SelectedAbility, ref argattr12))
                    {
                        Map.MaskData[withBlock3.x, withBlock3.y] = true;
                    }
                }

                string argoname = "大型マップ";
                if (!Expression.IsOptionDefined(ref argoname))
                {
                    GUI.Center(withBlock3.x, withBlock3.y);
                }

                GUI.MaskScreen();
            }

            if (CommandState == "コマンド選択")
            {
                CommandState = "ターゲット選択";
            }
            else
            {
                CommandState = "移動後ターゲット選択";
            }

            // カーソル自動移動を行う？
            if (!SRC.AutoMoveCursor)
            {
                GUI.UnlockGUI();
                return;
            }

            // 自分から最も近い味方ユニットを探す
            // UPGRADE_NOTE: オブジェクト t をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            t = null;
            foreach (Unit u in SRC.UList)
            {
                if (u.Status_Renamed == "出撃" & u.Party == "味方")
                {
                    if (Map.MaskData[u.x, u.y] == false & !ReferenceEquals(u, SelectedUnit))
                    {
                        if (t is null)
                        {
                            t = u;
                        }
                        else if (Math.Pow(Math.Abs((short)(SelectedUnit.x - u.x)), 2d) + Math.Pow(Math.Abs((short)(SelectedUnit.y - u.y)), 2d) < Math.Pow(Math.Abs((short)(SelectedUnit.x - t.x)), 2d) + Math.Pow(Math.Abs((short)(SelectedUnit.y - t.y)), 2d))
                        {
                            t = u;
                        }
                    }
                }
            }

            // 適当がユニットがなければ自分自身を選択
            if (t is null)
            {
                t = SelectedUnit;
            }

            // カーソルを移動
            string argcursor_mode1 = "ユニット選択";
            GUI.MoveCursorPos(ref argcursor_mode1, t);

            // ターゲットのステータスを表示
            if (!ReferenceEquals(SelectedUnit, t))
            {
                Status.DisplayUnitStatus(ref t);
            }

            GUI.UnlockGUI();
        }

        // 「アビリティ」コマンドを終了
        // MOD START MARGE
        // Public Sub FinishAbilityCommand()
        private static void FinishAbilityCommand()
        {
            // MOD END MARGE
            short i;
            var partners = default(Unit[]);
            string aname;
            // ADD START MARGE
            bool is_p_ability;
            // ADD END MARGE

            // MOD START MARGE
            // If MainWidth <> 15 Then
            if (GUI.NewGUIMode)
            {
                // MOD END MARGE
                Status.ClearUnitStatus();
            }

            GUI.LockGUI();

            // 合体技のパートナーを設定
            {
                var withBlock = SelectedUnit;
                string argattr = "合";
                if (withBlock.IsAbilityClassifiedAs(SelectedAbility, ref argattr))
                {
                    if (withBlock.AbilityMaxRange(SelectedAbility) == 1)
                    {
                        string argctype_Renamed = "アビリティ";
                        withBlock.CombinationPartner(ref argctype_Renamed, SelectedAbility, ref partners, SelectedTarget.x, SelectedTarget.y);
                    }
                    else
                    {
                        string argctype_Renamed1 = "アビリティ";
                        withBlock.CombinationPartner(ref argctype_Renamed1, SelectedAbility, ref partners);
                    }
                }
                else
                {
                    SelectedPartners = new Unit[1];
                    partners = new Unit[1];
                }
            }

            aname = SelectedUnit.Ability(SelectedAbility).Name;
            SelectedAbilityName = aname;

            // ADD START MARGE
            // 移動後使用後可能なアビリティか記録しておく
            string argattr1 = "Ｐ";
            string argattr2 = "Ｑ";
            is_p_ability = SelectedUnit.IsAbilityClassifiedAs(SelectedAbility, ref argattr1) | SelectedUnit.AbilityMaxRange(SelectedAbility) == 1 & !SelectedUnit.IsAbilityClassifiedAs(SelectedAbility, ref argattr2);
            // ADD END MARGE

            // 使用イベント
            Event_Renamed.HandleEvent("使用", SelectedUnit.MainPilot().ID, aname);
            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
                SelectedPartners = new Unit[1];
                GUI.UnlockGUI();
                return;
            }

            if (SRC.IsCanceled)
            {
                SRC.IsCanceled = false;
                SelectedPartners = new Unit[1];
                WaitCommand();
                return;
            }

            {
                var withBlock1 = SelectedUnit;
                foreach (Unit u in SRC.UList)
                {
                    if (u.Status_Renamed == "出撃")
                    {
                        Map.MaskData[u.x, u.y] = true;
                    }
                }

                // 合体技パートナーのハイライト表示
                string argattr3 = "合";
                if (withBlock1.IsAbilityClassifiedAs(SelectedAbility, ref argattr3))
                {
                    var loopTo = (short)Information.UBound(partners);
                    for (i = 1; i <= loopTo; i++)
                    {
                        {
                            var withBlock2 = partners[i];
                            Map.MaskData[withBlock2.x, withBlock2.y] = false;
                        }
                    }
                }

                Map.MaskData[withBlock1.x, withBlock1.y] = false;
                Map.MaskData[SelectedTarget.x, SelectedTarget.y] = false;
                if (!SRC.BattleAnimation)
                {
                    GUI.MaskScreen();
                }

                // アビリティを実行
                withBlock1.ExecuteAbility(SelectedAbility, ref SelectedTarget);
                SelectedUnit = withBlock1.CurrentForm();
                GUI.CloseMessageForm();
                Status.ClearUnitStatus();
            }

            // 破壊イベント
            {
                var withBlock3 = SelectedUnit;
                if (withBlock3.Status_Renamed == "破壊")
                {
                    if (withBlock3.CountPilot() > 0)
                    {
                        Event_Renamed.HandleEvent("破壊", withBlock3.MainPilot().ID);
                        if (SRC.IsScenarioFinished)
                        {
                            SRC.IsScenarioFinished = false;
                            SelectedPartners = new Unit[1];
                            GUI.UnlockGUI();
                            return;
                        }

                        if (SRC.IsCanceled)
                        {
                            SRC.IsCanceled = false;
                            SelectedPartners = new Unit[1];
                            GUI.UnlockGUI();
                            return;
                        }
                    }

                    WaitCommand();
                    return;
                }
            }

            // 使用後イベント
            {
                var withBlock4 = SelectedUnit;
                if (withBlock4.CountPilot() > 0)
                {
                    Event_Renamed.HandleEvent("使用後", withBlock4.MainPilot().ID, aname);
                    if (SRC.IsScenarioFinished)
                    {
                        SRC.IsScenarioFinished = false;
                        SelectedPartners = new Unit[1];
                        GUI.UnlockGUI();
                        return;
                    }

                    if (SRC.IsCanceled)
                    {
                        SRC.IsCanceled = false;
                        SelectedPartners = new Unit[1];
                        GUI.UnlockGUI();
                        return;
                    }
                }
            }

            // 合体技のパートナーの行動数を減らす
            string argoname = "合体技パートナー行動数無消費";
            if (!Expression.IsOptionDefined(ref argoname))
            {
                var loopTo1 = (short)Information.UBound(partners);
                for (i = 1; i <= loopTo1; i++)
                    partners[i].CurrentForm().UseAction();
            }

            SelectedPartners = new Unit[1];

            // ADD START MARGE
            // 再移動
            if (is_p_ability & SelectedUnit.Status_Renamed == "出撃")
            {
                string argsname = "遊撃";
                if (SelectedUnit.MainPilot().IsSkillAvailable(ref argsname) & SelectedUnit.Speed * 2 > SelectedUnitMoveCost)
                {
                    // 進入イベント
                    if (SelectedUnitMoveCost > 0)
                    {
                        Event_Renamed.HandleEvent("進入", SelectedUnit.MainPilot().ID, SelectedUnit.x, SelectedUnit.y);
                        if (SRC.IsScenarioFinished)
                        {
                            SRC.IsScenarioFinished = false;
                            return;
                        }
                    }

                    // ユニットが既に出撃していない？
                    if (SelectedUnit.Status_Renamed != "出撃")
                    {
                        GUI.RedrawScreen();
                        Status.ClearUnitStatus();
                        return;
                    }

                    SelectedCommand = "再移動";
                    Map.AreaInSpeed(ref SelectedUnit);
                    string argoname1 = "大型マップ";
                    if (!Expression.IsOptionDefined(ref argoname1))
                    {
                        GUI.Center(SelectedUnit.x, SelectedUnit.y);
                    }

                    GUI.MaskScreen();
                    if (GUI.NewGUIMode)
                    {
                        Application.DoEvents();
                        Status.ClearUnitStatus();
                    }
                    else
                    {
                        Status.DisplayUnitStatus(ref SelectedUnit);
                    }

                    CommandState = "ターゲット選択";
                    return;
                }
            }
            // ADD END MARGE

            // 行動終了
            WaitCommand();
        }

        // マップ型「アビリティ」コマンドを終了
        // MOD START MARGE
        // Public Sub MapAbilityCommand()
        private static void MapAbilityCommand()
        {
            // MOD END MARGE
            short i;
            var partners = default(Unit[]);
            // ADD START MARGE
            bool is_p_ability;
            // ADD END MARGE

            {
                var withBlock = SelectedUnit;
                // ADD START MARGE
                // 移動後使用後可能なアビリティか記録しておく
                string argattr = "Ｐ";
                string argattr1 = "Ｑ";
                is_p_ability = withBlock.IsAbilityClassifiedAs(SelectedAbility, ref argattr) | withBlock.AbilityMaxRange(SelectedAbility) == 1 & !withBlock.IsAbilityClassifiedAs(SelectedAbility, ref argattr1);
                // ADD END MARGE

                // 目標地点を選択して初めて効果範囲が分かるタイプのマップアビリティ
                // の場合は再度プレイヤーの選択を促す必要がある
                if (CommandState == "ターゲット選択" | CommandState == "移動後ターゲット選択")
                {
                    string argattr3 = "Ｍ投";
                    string argattr4 = "Ｍ移";
                    string argattr5 = "Ｍ線";
                    if (withBlock.IsAbilityClassifiedAs(SelectedAbility, ref argattr3))
                    {
                        if (CommandState == "ターゲット選択")
                        {
                            CommandState = "マップ攻撃使用";
                        }
                        else
                        {
                            CommandState = "移動後マップ攻撃使用";
                        }

                        // 目標地点
                        SelectedX = GUI.PixelToMapX((short)GUI.MouseX);
                        SelectedY = GUI.PixelToMapY((short)GUI.MouseY);

                        // 効果範囲を設定
                        string argattr2 = "Ｍ投";
                        string arguparty = "味方";
                        Map.AreaInRange(SelectedX, SelectedY, (short)withBlock.AbilityLevel(SelectedAbility, ref argattr2), 1, ref arguparty);
                        GUI.MaskScreen();
                        return;
                    }
                    else if (withBlock.IsAbilityClassifiedAs(SelectedAbility, ref argattr4))
                    {
                        SelectedX = GUI.PixelToMapX((short)GUI.MouseX);
                        SelectedY = GUI.PixelToMapY((short)GUI.MouseY);
                        if (Map.MapDataForUnit[SelectedX, SelectedY] is object)
                        {
                            GUI.MaskScreen();
                            return;
                        }

                        // 目標地点
                        if (CommandState == "ターゲット選択")
                        {
                            CommandState = "マップ攻撃使用";
                        }
                        else
                        {
                            CommandState = "移動後マップ攻撃使用";
                        }

                        // 効果範囲を設定
                        Map.AreaInPointToPoint(withBlock.x, withBlock.y, SelectedX, SelectedY);
                        GUI.MaskScreen();
                        return;
                    }
                    else if (withBlock.IsAbilityClassifiedAs(SelectedAbility, ref argattr5))
                    {
                        if (CommandState == "ターゲット選択")
                        {
                            CommandState = "マップ攻撃使用";
                        }
                        else
                        {
                            CommandState = "移動後マップ攻撃使用";
                        }

                        // 目標地点
                        SelectedX = GUI.PixelToMapX((short)GUI.MouseX);
                        SelectedY = GUI.PixelToMapY((short)GUI.MouseY);

                        // 効果範囲を設定
                        Map.AreaInPointToPoint(withBlock.x, withBlock.y, SelectedX, SelectedY);
                        GUI.MaskScreen();
                        return;
                    }
                }

                // 合体技パートナーの設定
                string argattr6 = "合";
                if (withBlock.IsAbilityClassifiedAs(SelectedAbility, ref argattr6))
                {
                    string argctype_Renamed = "アビリティ";
                    withBlock.CombinationPartner(ref argctype_Renamed, SelectedAbility, ref partners);
                }
                else
                {
                    SelectedPartners = new Unit[1];
                    partners = new Unit[1];
                }
            }

            if (GUI.MainWidth != 15)
            {
                Status.ClearUnitStatus();
            }

            GUI.LockGUI();

            // アビリティを実行
            SelectedUnit.ExecuteMapAbility(SelectedAbility, SelectedX, SelectedY);
            SelectedUnit = SelectedUnit.CurrentForm();
            // UPGRADE_NOTE: オブジェクト SelectedTarget をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            SelectedTarget = null;
            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
                SelectedPartners = new Unit[1];
                GUI.UnlockGUI();
                return;
            }

            if (SRC.IsCanceled)
            {
                SRC.IsCanceled = false;
                SelectedPartners = new Unit[1];
                WaitCommand();
                return;
            }

            // 合体技のパートナーの行動数を減らす
            string argoname = "合体技パートナー行動数無消費";
            if (!Expression.IsOptionDefined(ref argoname))
            {
                var loopTo = (short)Information.UBound(partners);
                for (i = 1; i <= loopTo; i++)
                    partners[i].CurrentForm().UseAction();
            }

            SelectedPartners = new Unit[1];

            // ADD START MARGE
            // 再移動
            if (is_p_ability & SelectedUnit.Status_Renamed == "出撃")
            {
                string argsname = "遊撃";
                if (SelectedUnit.MainPilot().IsSkillAvailable(ref argsname) & SelectedUnit.Speed * 2 > SelectedUnitMoveCost)
                {
                    // 進入イベント
                    if (SelectedUnitMoveCost > 0)
                    {
                        Event_Renamed.HandleEvent("進入", SelectedUnit.MainPilot().ID, SelectedUnit.x, SelectedUnit.y);
                        if (SRC.IsScenarioFinished)
                        {
                            SRC.IsScenarioFinished = false;
                            return;
                        }
                    }

                    // ユニットが既に出撃していない？
                    if (SelectedUnit.Status_Renamed != "出撃")
                    {
                        GUI.RedrawScreen();
                        Status.ClearUnitStatus();
                        return;
                    }

                    SelectedCommand = "再移動";
                    Map.AreaInSpeed(ref SelectedUnit);
                    string argoname1 = "大型マップ";
                    if (!Expression.IsOptionDefined(ref argoname1))
                    {
                        GUI.Center(SelectedUnit.x, SelectedUnit.y);
                    }

                    GUI.MaskScreen();
                    if (GUI.NewGUIMode)
                    {
                        Application.DoEvents();
                        Status.ClearUnitStatus();
                    }
                    else
                    {
                        Status.DisplayUnitStatus(ref SelectedUnit);
                    }

                    CommandState = "ターゲット選択";
                    return;
                }
            }
            // ADD END MARGE

            // 行動終了
            WaitCommand();
        }


        // スペシャルパワーコマンドを開始
        // MOD START MARGE
        // Public Sub StartSpecialPowerCommand()
        private static void StartSpecialPowerCommand()
        {
            // MOD END MARGE
            short n, i, j, ret;
            string[] pname_list;
            string[] pid_list;
            string[] sp_list;
            string[] list;
            string[] id_list;
            string sname;
            SpecialPowerData sd;
            Unit u;
            Pilot p;
            var strkey_list = default(string[]);
            short max_item;
            string max_str;
            string buf;
            bool found;
            GUI.LockGUI();
            SelectedCommand = "スペシャルパワー";
            {
                var withBlock = SelectedUnit;
                pname_list = new string[1];
                pid_list = new string[1];
                GUI.ListItemFlag = new bool[1];

                // スペシャルパワーを使用可能なパイロットの一覧を作成
                n = (short)(withBlock.CountPilot() + withBlock.CountSupport());
                string argfname = "追加サポート";
                if (withBlock.IsFeatureAvailable(ref argfname))
                {
                    n = (short)(n + 1);
                }

                var loopTo = n;
                for (i = 1; i <= loopTo; i++)
                {
                    if (i <= withBlock.CountPilot())
                    {
                        // メインパイロット＆サブパイロット
                        object argIndex1 = i;
                        p = withBlock.Pilot(ref argIndex1);
                        if (i == 1)
                        {
                            // １番目のパイロットの場合はメインパイロットを使用
                            p = withBlock.MainPilot();

                            // ただし２人乗り以上のユニットで、メインパイロットが
                            // スペシャルパワーを持たない場合はそのまま１番目のパイロットを使用
                            if (withBlock.CountPilot() > 1)
                            {
                                object argIndex3 = 1;
                                if (p.Data.SP <= 0 & withBlock.Pilot(ref argIndex3).Data.SP > 0)
                                {
                                    object argIndex2 = 1;
                                    p = withBlock.Pilot(ref argIndex2);
                                }
                            }

                            // サブパイロットがメインパイロットを勤めている場合も
                            // １番目のパイロットを使用
                            var loopTo1 = withBlock.CountPilot();
                            for (j = 2; j <= loopTo1; j++)
                            {
                                Pilot localPilot() { object argIndex1 = j; var ret = withBlock.Pilot(ref argIndex1); return ret; }

                                if (ReferenceEquals(p, localPilot()))
                                {
                                    object argIndex4 = 1;
                                    p = withBlock.Pilot(ref argIndex4);
                                    break;
                                }
                            }
                        }
                        else
                        {
                        }

                        if (p.CountSpecialPower == 0)
                        {
                            goto NextPilot;
                        }

                        Array.Resize(ref pname_list, Information.UBound(pname_list) + 1 + 1);
                        Array.Resize(ref pid_list, Information.UBound(pname_list) + 1 + 1);
                        Array.Resize(ref GUI.ListItemFlag, Information.UBound(pname_list) + 1);
                        GUI.ListItemFlag[Information.UBound(pname_list)] = false;
                        pid_list[Information.UBound(pname_list)] = p.ID;
                        string localRightPaddedString() { string argbuf = p.get_Nickname(false); var ret = GeneralLib.RightPaddedString(ref argbuf, 17); p.get_Nickname(false) = argbuf; return ret; }

                        string localRightPaddedString1() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.SP) + "/" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.MaxSP); var ret = GeneralLib.RightPaddedString(ref argbuf, 8); return ret; }

                        pname_list[Information.UBound(pname_list)] = localRightPaddedString() + localRightPaddedString1();
                        var loopTo2 = p.CountSpecialPower;
                        for (j = 1; j <= loopTo2; j++)
                        {
                            sname = p.get_SpecialPower(j);
                            if (p.SP >= p.SpecialPowerCost(ref sname))
                            {
                                SpecialPowerData localItem() { object argIndex1 = sname; var ret = SRC.SPDList.Item(ref argIndex1); return ret; }

                                pname_list[Information.UBound(pname_list)] = pname_list[Information.UBound(pname_list)] + localItem().ShortName;
                            }
                        }
                    }
                    else if (i <= (short)(withBlock.CountPilot() + withBlock.CountSupport()))
                    {
                        // サポートパイロット
                        object argIndex5 = i - withBlock.CountPilot();
                        {
                            var withBlock2 = withBlock.Support(ref argIndex5);
                            if (withBlock2.CountSpecialPower == 0)
                            {
                                goto NextPilot;
                            }

                            Array.Resize(ref pname_list, Information.UBound(pname_list) + 1 + 1);
                            Array.Resize(ref pid_list, Information.UBound(pname_list) + 1 + 1);
                            Array.Resize(ref GUI.ListItemFlag, Information.UBound(pname_list) + 1);
                            GUI.ListItemFlag[Information.UBound(pname_list)] = false;
                            pid_list[Information.UBound(pname_list)] = withBlock2.ID;
                            string localRightPaddedString4() { string argbuf = withBlock2.get_Nickname(false); var ret = GeneralLib.RightPaddedString(ref argbuf, 17); withBlock2.get_Nickname(false) = argbuf; return ret; }

                            string localRightPaddedString5() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock2.SP) + "/" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock2.MaxSP); var ret = GeneralLib.RightPaddedString(ref argbuf, 8); return ret; }

                            pname_list[Information.UBound(pname_list)] = localRightPaddedString4() + localRightPaddedString5();
                            var loopTo4 = withBlock2.CountSpecialPower;
                            for (j = 1; j <= loopTo4; j++)
                            {
                                sname = withBlock2.get_SpecialPower(j);
                                if (withBlock2.SP >= withBlock2.SpecialPowerCost(ref sname))
                                {
                                    SpecialPowerData localItem2() { object argIndex1 = sname; var ret = SRC.SPDList.Item(ref argIndex1); return ret; }

                                    pname_list[Information.UBound(pname_list)] = pname_list[Information.UBound(pname_list)] + localItem2().ShortName;
                                }
                            }
                        }
                    }
                    else
                    {
                        // 追加サポートパイロット
                        {
                            var withBlock1 = withBlock.AdditionalSupport();
                            if (withBlock1.CountSpecialPower == 0)
                            {
                                goto NextPilot;
                            }

                            Array.Resize(ref pname_list, Information.UBound(pname_list) + 1 + 1);
                            Array.Resize(ref pid_list, Information.UBound(pname_list) + 1 + 1);
                            Array.Resize(ref GUI.ListItemFlag, Information.UBound(pname_list) + 1);
                            GUI.ListItemFlag[Information.UBound(pname_list)] = false;
                            pid_list[Information.UBound(pname_list)] = withBlock1.ID;
                            string localRightPaddedString2() { string argbuf = withBlock1.get_Nickname(false); var ret = GeneralLib.RightPaddedString(ref argbuf, 17); withBlock1.get_Nickname(false) = argbuf; return ret; }

                            string localRightPaddedString3() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock1.SP) + "/" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock1.MaxSP); var ret = GeneralLib.RightPaddedString(ref argbuf, 8); return ret; }

                            pname_list[Information.UBound(pname_list)] = localRightPaddedString2() + localRightPaddedString3();
                            var loopTo3 = withBlock1.CountSpecialPower;
                            for (j = 1; j <= loopTo3; j++)
                            {
                                sname = withBlock1.get_SpecialPower(j);
                                if (withBlock1.SP >= withBlock1.SpecialPowerCost(ref sname))
                                {
                                    SpecialPowerData localItem1() { object argIndex1 = sname; var ret = SRC.SPDList.Item(ref argIndex1); return ret; }

                                    pname_list[Information.UBound(pname_list)] = pname_list[Information.UBound(pname_list)] + localItem1().ShortName;
                                }
                            }
                        }
                    }

                    NextPilot:
                    ;
                }

                GUI.TopItem = 1;
                if (Information.UBound(pname_list) > 1)
                {
                    // どのパイロットを使うか選択
                    string argoname = "等身大基準";
                    if (Expression.IsOptionDefined(ref argoname))
                    {
                        string arglb_caption = "キャラクター選択";
                        string argtname = "SP";
                        string argtname1 = "SP";
                        string arglb_info = "キャラクター     " + Expression.Term(ref argtname, ref SelectedUnit, 2) + "/Max" + Expression.Term(ref argtname1, ref SelectedUnit, 2);
                        string arglb_mode = "連続表示,カーソル移動";
                        i = GUI.ListBox(ref arglb_caption, ref pname_list, ref arglb_info, ref arglb_mode);
                    }
                    else
                    {
                        string arglb_caption1 = "パイロット選択";
                        string argtname2 = "SP";
                        string argtname3 = "SP";
                        string arglb_info1 = "パイロット       " + Expression.Term(ref argtname2, ref SelectedUnit, 2) + "/Max" + Expression.Term(ref argtname3, ref SelectedUnit, 2);
                        string arglb_mode1 = "連続表示,カーソル移動";
                        i = GUI.ListBox(ref arglb_caption1, ref pname_list, ref arglb_info1, ref arglb_mode1);
                    }
                }
                else
                {
                    // 一人しかいないので選択の必要なし
                    i = 1;
                }

                // 誰もスペシャルパワーを使えなければキャンセル
                if (i == 0)
                {
                    My.MyProject.Forms.frmListBox.Hide();
                    if (SRC.AutoMoveCursor)
                    {
                        GUI.RestoreCursorPos();
                    }

                    GUI.UnlockGUI();
                    CancelCommand();
                    return;
                }

                // スペシャルパワーを使うパイロットを設定
                var tmp = pid_list;
                object argIndex6 = tmp[i];
                SelectedPilot = SRC.PList.Item(ref argIndex6);
                // そのパイロットのステータスを表示
                if (Information.UBound(pname_list) > 1)
                {
                    Status.DisplayPilotStatus(SelectedPilot);
                }
            }

            {
                var withBlock3 = SelectedPilot;
                // 使用可能なスペシャルパワーの一覧を作成
                sp_list = new string[(withBlock3.CountSpecialPower + 1)];
                GUI.ListItemFlag = new bool[(withBlock3.CountSpecialPower + 1)];
                var loopTo5 = withBlock3.CountSpecialPower;
                for (i = 1; i <= loopTo5; i++)
                {
                    sname = withBlock3.get_SpecialPower(i);
                    string localLeftPaddedString() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock3.SpecialPowerCost(ref sname)); var ret = GeneralLib.LeftPaddedString(ref argbuf, 3); return ret; }

                    SpecialPowerData localItem3() { object argIndex1 = sname; var ret = SRC.SPDList.Item(ref argIndex1); return ret; }

                    sp_list[i] = GeneralLib.RightPaddedString(ref sname, 13) + localLeftPaddedString() + "  " + localItem3().Comment;
                    if (withBlock3.SP >= withBlock3.SpecialPowerCost(ref sname))
                    {
                        if (!withBlock3.IsSpecialPowerUseful(ref sname))
                        {
                            GUI.ListItemFlag[i] = true;
                        }
                    }
                    else
                    {
                        GUI.ListItemFlag[i] = true;
                    }
                }
            }

            // どのコマンドを使用するかを選択
            {
                var withBlock4 = SelectedPilot;
                GUI.TopItem = 1;
                string argtname4 = "スペシャルパワー";
                string arglb_caption2 = Expression.Term(ref argtname4, ref SelectedUnit) + "選択";
                string argtname5 = "SP";
                string argtname6 = "SP";
                string arglb_info2 = "名称         消費" + Expression.Term(ref argtname5, ref SelectedUnit) + "（" + withBlock4.get_Nickname(false) + " " + Expression.Term(ref argtname6, ref SelectedUnit) + "=" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock4.SP) + "/" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock4.MaxSP) + "）";
                string arglb_mode2 = "カーソル移動(行きのみ)";
                i = GUI.ListBox(ref arglb_caption2, ref sp_list, ref arglb_info2, ref arglb_mode2);
            }

            // キャンセル
            if (i == 0)
            {
                Status.DisplayUnitStatus(ref SelectedUnit);
                // カーソル自動移動
                if (SRC.AutoMoveCursor)
                {
                    string argcursor_mode = "ユニット選択";
                    GUI.MoveCursorPos(ref argcursor_mode, SelectedUnit);
                }

                GUI.UnlockGUI();
                CancelCommand();
                return;
            }

            // 使用するスペシャルパワーを設定
            SelectedSpecialPower = SelectedPilot.get_SpecialPower(i);

            // 味方スペシャルパワー実行の効果により他のパイロットが持っているスペシャルパワーを
            // 使う場合は記録しておき、後で消費ＳＰを倍にする必要がある
            SpecialPowerData localItem5() { object argIndex1 = SelectedSpecialPower; var ret = SRC.SPDList.Item(ref argIndex1); return ret; }

            if (localItem5().EffectType(1) == "味方スペシャルパワー実行")
            {
                // スペシャルパワー一覧
                list = new string[1];
                var loopTo6 = SRC.SPDList.Count();
                for (i = 1; i <= loopTo6; i++)
                {
                    object argIndex7 = i;
                    {
                        var withBlock5 = SRC.SPDList.Item(ref argIndex7);
                        if (withBlock5.EffectType(1) != "味方スペシャルパワー実行" & withBlock5.ShortName != "非表示")
                        {
                            Array.Resize(ref list, Information.UBound(list) + 1 + 1);
                            Array.Resize(ref strkey_list, Information.UBound(list) + 1);
                            list[Information.UBound(list)] = withBlock5.Name;
                            strkey_list[Information.UBound(list)] = withBlock5.KanaName;
                        }
                    }
                }

                GUI.ListItemFlag = new bool[Information.UBound(list) + 1];

                // ソート
                var loopTo7 = (short)(Information.UBound(strkey_list) - 1);
                for (i = 1; i <= loopTo7; i++)
                {
                    max_item = i;
                    max_str = strkey_list[i];
                    var loopTo8 = (short)Information.UBound(strkey_list);
                    for (j = (short)(i + 1); j <= loopTo8; j++)
                    {
                        if (Strings.StrComp(strkey_list[j], max_str, (CompareMethod)1) == -1)
                        {
                            max_item = j;
                            max_str = strkey_list[j];
                        }
                    }

                    if (max_item != i)
                    {
                        buf = list[i];
                        list[i] = list[max_item];
                        list[max_item] = buf;
                        buf = strkey_list[i];
                        strkey_list[i] = max_str;
                        strkey_list[max_item] = buf;
                    }
                }

                // スペシャルパワーを使用可能なパイロットがいるかどうかを判定
                var loopTo9 = (short)Information.UBound(list);
                for (i = 1; i <= loopTo9; i++)
                {
                    GUI.ListItemFlag[i] = true;
                    foreach (Pilot currentP in SRC.PList)
                    {
                        p = currentP;
                        if (p.Party == "味方")
                        {
                            if (p.Unit_Renamed is object)
                            {
                                object argIndex8 = "憑依";
                                if (p.Unit_Renamed.Status_Renamed == "出撃" & !p.Unit_Renamed.IsConditionSatisfied(ref argIndex8))
                                {
                                    // 本当に乗っている？
                                    found = false;
                                    {
                                        var withBlock6 = p.Unit_Renamed;
                                        if (ReferenceEquals(p, withBlock6.MainPilot()))
                                        {
                                            found = true;
                                        }
                                        else
                                        {
                                            var loopTo10 = withBlock6.CountPilot();
                                            for (j = 2; j <= loopTo10; j++)
                                            {
                                                Pilot localPilot1() { object argIndex1 = j; var ret = withBlock6.Pilot(ref argIndex1); return ret; }

                                                if (ReferenceEquals(p, localPilot1()))
                                                {
                                                    found = true;
                                                    break;
                                                }
                                            }

                                            var loopTo11 = withBlock6.CountSupport();
                                            for (j = 1; j <= loopTo11; j++)
                                            {
                                                Pilot localSupport() { object argIndex1 = j; var ret = withBlock6.Support(ref argIndex1); return ret; }

                                                if (ReferenceEquals(p, localSupport()))
                                                {
                                                    found = true;
                                                    break;
                                                }
                                            }

                                            if (ReferenceEquals(p, withBlock6.AdditionalSupport()))
                                            {
                                                found = true;
                                            }
                                        }
                                    }

                                    if (found)
                                    {
                                        if (p.IsSpecialPowerAvailable(ref list[i]))
                                        {
                                            GUI.ListItemFlag[i] = false;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                // 各スペシャルパワーが使用可能か判定
                {
                    var withBlock7 = SelectedPilot;
                    var loopTo12 = (short)Information.UBound(list);
                    for (i = 1; i <= loopTo12; i++)
                    {
                        if (!GUI.ListItemFlag[i] & withBlock7.SP >= 2 * withBlock7.SpecialPowerCost(ref list[i]))
                        {
                            if (!withBlock7.IsSpecialPowerUseful(ref list[i]))
                            {
                                GUI.ListItemFlag[i] = true;
                            }
                        }
                        else
                        {
                            GUI.ListItemFlag[i] = true;
                        }
                    }
                }

                // スペシャルパワーの解説を設定
                GUI.ListItemComment = new string[Information.UBound(list) + 1];
                var loopTo13 = (short)Information.UBound(list);
                for (i = 1; i <= loopTo13; i++)
                {
                    SpecialPowerData localItem4() { var tmp = list; object argIndex1 = tmp[i]; var ret = SRC.SPDList.Item(ref argIndex1); return ret; }

                    GUI.ListItemComment[i] = localItem4().Comment;
                }

                // 検索するスペシャルパワーを選択
                GUI.TopItem = 1;
                string argtname7 = "スペシャルパワー";
                Unit argu = null;
                string arglb_caption3 = Expression.Term(ref argtname7, u: ref argu) + "検索";
                ret = GUI.MultiColumnListBox(ref arglb_caption3, ref list, true);
                if (ret == 0)
                {
                    SelectedSpecialPower = "";
                    CancelCommand();
                    GUI.UnlockGUI();
                    return;
                }

                // スペシャルパワー使用メッセージ
                if (SelectedUnit.IsMessageDefined(ref SelectedSpecialPower))
                {
                    Unit argu1 = null;
                    Unit argu2 = null;
                    GUI.OpenMessageForm(u1: ref argu1, u2: ref argu2);
                    string argmsg_mode = "";
                    SelectedUnit.PilotMessage(ref SelectedSpecialPower, msg_mode: ref argmsg_mode);
                    GUI.CloseMessageForm();
                }

                SelectedSpecialPower = list[ret];
                WithDoubleSPConsumption = true;
            }
            else
            {
                WithDoubleSPConsumption = false;
            }

            object argIndex9 = SelectedSpecialPower;
            sd = SRC.SPDList.Item(ref argIndex9);

            // ターゲットを選択する必要があるスペシャルパワーの場合
            switch (sd.TargetType ?? "")
            {
                case "味方":
                case "敵":
                case "任意":
                    {
                        // マップ上のユニットからターゲットを選択する

                        Unit argu11 = null;
                        Unit argu21 = null;
                        GUI.OpenMessageForm(u1: ref argu11, u2: ref argu21);
                        GUI.DisplaySysMessage(SelectedPilot.get_Nickname(false) + "は" + SelectedSpecialPower + "を使った。;" + "ターゲットを選んでください。");
                        GUI.CloseMessageForm();

                        // ターゲットのエリアを設定
                        var loopTo14 = Map.MapWidth;
                        for (i = 1; i <= loopTo14; i++)
                        {
                            var loopTo15 = Map.MapHeight;
                            for (j = 1; j <= loopTo15; j++)
                            {
                                Map.MaskData[i, j] = true;
                                u = Map.MapDataForUnit[i, j];
                                if (u is null)
                                {
                                    goto NextLoop;
                                }

                                // 陣営が合っている？
                                switch (sd.TargetType ?? "")
                                {
                                    case "味方":
                                        {
                                            {
                                                var withBlock8 = u;
                                                if (withBlock8.Party != "味方" & withBlock8.Party0 != "味方" & withBlock8.Party != "ＮＰＣ" & withBlock8.Party0 != "ＮＰＣ")
                                                {
                                                    goto NextLoop;
                                                }
                                            }

                                            break;
                                        }

                                    case "敵":
                                        {
                                            {
                                                var withBlock9 = u;
                                                if (withBlock9.Party == "味方" & withBlock9.Party0 == "味方" | withBlock9.Party == "ＮＰＣ" & withBlock9.Party0 == "ＮＰＣ")
                                                {
                                                    goto NextLoop;
                                                }
                                            }

                                            break;
                                        }
                                }

                                // スペシャルパワーを適用可能？
                                if (!sd.Effective(ref SelectedPilot, ref u))
                                {
                                    goto NextLoop;
                                }

                                Map.MaskData[i, j] = false;
                                NextLoop:
                                ;
                            }
                        }

                        GUI.MaskScreen();
                        CommandState = "ターゲット選択";
                        GUI.UnlockGUI();
                        return;
                    }

                case "破壊味方":
                    {
                        // 破壊された味方ユニットの中からターゲットを選択する

                        Unit argu12 = null;
                        Unit argu22 = null;
                        GUI.OpenMessageForm(u1: ref argu12, u2: ref argu22);
                        GUI.DisplaySysMessage(SelectedPilot.get_Nickname(false) + "は" + SelectedSpecialPower + "を使った。;" + "復活させるユニットを選んでください。");
                        GUI.CloseMessageForm();

                        // 破壊された味方ユニットのリストを作成
                        list = new string[1];
                        id_list = new string[1];
                        GUI.ListItemFlag = new bool[1];
                        foreach (Unit currentU in SRC.UList)
                        {
                            u = currentU;
                            if (u.Party0 == "味方" & u.Status_Renamed == "破壊" & (u.CountPilot() > 0 | u.Data.PilotNum == 0))
                            {
                                Array.Resize(ref list, Information.UBound(list) + 1 + 1);
                                Array.Resize(ref id_list, Information.UBound(list) + 1);
                                Array.Resize(ref GUI.ListItemFlag, Information.UBound(list) + 1);
                                string localRightPaddedString6() { string argbuf = u.Nickname; var ret = GeneralLib.RightPaddedString(ref argbuf, 28); u.Nickname = argbuf; return ret; }

                                string localRightPaddedString7() { string argbuf = u.MainPilot().get_Nickname(false); var ret = GeneralLib.RightPaddedString(ref argbuf, 18); u.MainPilot().get_Nickname(false) = argbuf; return ret; }

                                string localLeftPaddedString1() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(u.MainPilot().Level); var ret = GeneralLib.LeftPaddedString(ref argbuf, 3); return ret; }

                                list[Information.UBound(list)] = localRightPaddedString6() + localRightPaddedString7() + localLeftPaddedString1();
                                id_list[Information.UBound(list)] = u.ID;
                                GUI.ListItemFlag[Information.UBound(list)] = false;
                            }
                        }

                        GUI.TopItem = 1;
                        string arglb_caption4 = "ユニット選択";
                        string arglb_info3 = "ユニット名                  パイロット     レベル";
                        string arglb_mode3 = "";
                        i = GUI.ListBox(ref arglb_caption4, ref list, ref arglb_info3, lb_mode: ref arglb_mode3);
                        if (i == 0)
                        {
                            GUI.UnlockGUI();
                            CancelCommand();
                            return;
                        }

                        var tmp1 = id_list;
                        object argIndex10 = tmp1[i];
                        SelectedTarget = SRC.UList.Item(ref argIndex10);
                        break;
                    }
            }

            // 自爆を選択した場合は確認を取る
            // UPGRADE_WARNING: オブジェクト sd.IsEffectAvailable(自爆) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            string argename = "自爆";
            if (Conversions.ToBoolean(sd.IsEffectAvailable(ref argename)))
            {
                ret = (short)Interaction.MsgBox("自爆させますか？", (MsgBoxStyle)((int)MsgBoxStyle.OkCancel + (int)MsgBoxStyle.Question), "自爆");
                if (ret == (int)MsgBoxResult.Cancel)
                {
                    GUI.UnlockGUI();
                    return;
                }
            }

            // 使用イベント
            Event_Renamed.HandleEvent("使用", SelectedUnit.MainPilot().ID, SelectedSpecialPower);
            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
                GUI.UnlockGUI();
                return;
            }

            if (SRC.IsCanceled)
            {
                SRC.IsCanceled = false;
                GUI.UnlockGUI();
                return;
            }

            // スペシャルパワーを使用
            if (WithDoubleSPConsumption)
            {
                SelectedPilot.UseSpecialPower(ref SelectedSpecialPower, 2d);
            }
            else
            {
                SelectedPilot.UseSpecialPower(ref SelectedSpecialPower);
            }

            SelectedUnit = SelectedUnit.CurrentForm();

            // カーソル自動移動
            if (SRC.AutoMoveCursor)
            {
                string argcursor_mode1 = "ユニット選択";
                GUI.MoveCursorPos(ref argcursor_mode1, SelectedUnit);
            }

            // ステータスウィンドウを更新
            Status.DisplayUnitStatus(ref SelectedUnit);

            // 使用後イベント
            Event_Renamed.HandleEvent("使用後", SelectedUnit.MainPilot().ID, SelectedSpecialPower);
            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
            }

            if (SRC.IsCanceled)
            {
                SRC.IsCanceled = false;
            }

            SelectedSpecialPower = "";
            GUI.UnlockGUI();
            CommandState = "ユニット選択";
        }

        // スペシャルパワーコマンドを終了
        // MOD START MARGE
        // Public Sub FinishSpecialPowerCommand()
        private static void FinishSpecialPowerCommand()
        {
            // MOD END MARGE
            short i, ret;
            GUI.LockGUI();

            // 自爆を選択した場合は確認を取る
            object argIndex1 = SelectedSpecialPower;
            {
                var withBlock = SRC.SPDList.Item(ref argIndex1);
                var loopTo = withBlock.CountEffect();
                for (i = 1; i <= loopTo; i++)
                {
                    if (withBlock.EffectType(i) == "自爆")
                    {
                        ret = (short)Interaction.MsgBox("自爆させますか？", (MsgBoxStyle)((int)MsgBoxStyle.OkCancel + (int)MsgBoxStyle.Question), "自爆");
                        if (ret == (int)MsgBoxResult.Cancel)
                        {
                            CommandState = "ユニット選択";
                            GUI.UnlockGUI();
                            return;
                        }

                        break;
                    }
                }
            }

            // 使用イベント
            Event_Renamed.HandleEvent("使用", SelectedUnit.MainPilot().ID, SelectedSpecialPower);
            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
                CommandState = "ユニット選択";
                GUI.UnlockGUI();
                return;
            }

            if (SRC.IsCanceled)
            {
                SRC.IsCanceled = false;
                CommandState = "ユニット選択";
                GUI.UnlockGUI();
                return;
            }

            // スペシャルパワーを使用
            if (WithDoubleSPConsumption)
            {
                SelectedPilot.UseSpecialPower(ref SelectedSpecialPower, 2d);
            }
            else
            {
                SelectedPilot.UseSpecialPower(ref SelectedSpecialPower);
            }

            SelectedUnit = SelectedUnit.CurrentForm();

            // ステータスウィンドウを更新
            if (SelectedTarget is object)
            {
                if (SelectedTarget.CurrentForm().Status_Renamed == "出撃")
                {
                    Status.DisplayUnitStatus(ref SelectedTarget);
                }
            }

            // 使用後イベント
            Event_Renamed.HandleEvent("使用後", SelectedUnit.MainPilot().ID, SelectedSpecialPower);
            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
            }

            if (SRC.IsCanceled)
            {
                SRC.IsCanceled = false;
            }

            SelectedSpecialPower = "";
            GUI.UnlockGUI();
            CommandState = "ユニット選択";
        }


        // 「修理」コマンドを開始
        // MOD START MARGE
        // Public Sub StartFixCommand()
        private static void StartFixCommand()
        {
            // MOD END MARGE
            short j, i, k;
            Unit t;
            string fname;
            SelectedCommand = "修理";

            // 射程範囲？を表示
            {
                var withBlock = SelectedUnit;
                string arguparty = "味方";
                Map.AreaInRange(withBlock.x, withBlock.y, 1, 1, ref arguparty);
                var loopTo = Map.MapWidth;
                for (i = 1; i <= loopTo; i++)
                {
                    var loopTo1 = Map.MapHeight;
                    for (j = 1; j <= loopTo1; j++)
                    {
                        if (!Map.MaskData[i, j] & Map.MapDataForUnit[i, j] is object)
                        {
                            {
                                var withBlock1 = Map.MapDataForUnit[i, j];
                                object argIndex1 = "ゾンビ";
                                if (withBlock1.HP == withBlock1.MaxHP | withBlock1.IsConditionSatisfied(ref argIndex1))
                                {
                                    Map.MaskData[i, j] = true;
                                }

                                string argfname = "修理不可";
                                if (withBlock1.IsFeatureAvailable(ref argfname))
                                {
                                    object argIndex5 = "修理不可";
                                    object argIndex6 = "修理不可";
                                    var loopTo2 = (short)Conversions.ToInteger(withBlock1.FeatureData(ref argIndex6));
                                    for (k = 2; k <= loopTo2; k++)
                                    {
                                        object argIndex2 = "修理不可";
                                        string arglist = withBlock1.FeatureData(ref argIndex2);
                                        fname = GeneralLib.LIndex(ref arglist, k);
                                        if (Strings.Left(fname, 1) == "!")
                                        {
                                            fname = Strings.Mid(fname, 2);
                                            object argIndex3 = "修理装置";
                                            if ((fname ?? "") != (SelectedUnit.FeatureName0(ref argIndex3) ?? ""))
                                            {
                                                Map.MaskData[i, j] = true;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            object argIndex4 = "修理装置";
                                            if ((fname ?? "") == (SelectedUnit.FeatureName0(ref argIndex4) ?? ""))
                                            {
                                                Map.MaskData[i, j] = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                Map.MaskData[withBlock.x, withBlock.y] = false;
            }

            GUI.MaskScreen();

            // カーソル自動移動
            if (SRC.AutoMoveCursor)
            {
                // UPGRADE_NOTE: オブジェクト t をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                t = null;
                foreach (Unit u in SRC.UList)
                {
                    if (u.Status_Renamed == "出撃" & u.Party == "味方")
                    {
                        if (Map.MaskData[u.x, u.y] == false & !ReferenceEquals(u, SelectedUnit))
                        {
                            if (t is null)
                            {
                                t = u;
                            }
                            else if (u.MaxHP - u.HP > t.MaxHP - t.HP)
                            {
                                t = u;
                            }
                        }
                    }
                }

                if (t is null)
                {
                    t = SelectedUnit;
                }

                string argcursor_mode = "ユニット選択";
                GUI.MoveCursorPos(ref argcursor_mode, t);
                if (!ReferenceEquals(SelectedUnit, t))
                {
                    Status.DisplayUnitStatus(ref t);
                }
            }

            if (CommandState == "コマンド選択")
            {
                CommandState = "ターゲット選択";
            }
            else
            {
                CommandState = "移動後ターゲット選択";
            }
        }

        // 「修理」コマンドを終了
        // MOD START MARGE
        // Public Sub FinishFixCommand()
        private static void FinishFixCommand()
        {
            // MOD END MARGE
            int tmp;
            GUI.LockGUI();
            GUI.OpenMessageForm(ref SelectedTarget, ref SelectedUnit);
            {
                var withBlock = SelectedUnit;
                // 選択内容を変更
                Event_Renamed.SelectedUnitForEvent = SelectedUnit;
                Event_Renamed.SelectedTargetForEvent = SelectedTarget;

                // 修理メッセージ＆特殊効果
                string argmain_situation = "修理";
                if (withBlock.IsMessageDefined(ref argmain_situation))
                {
                    string argSituation = "修理";
                    string argmsg_mode = "";
                    withBlock.PilotMessage(ref argSituation, msg_mode: ref argmsg_mode);
                }

                string argmain_situation3 = "修理";
                object argIndex3 = "修理装置";
                string argsub_situation2 = withBlock.FeatureName(ref argIndex3);
                if (withBlock.IsAnimationDefined(ref argmain_situation3, ref argsub_situation2))
                {
                    string argmain_situation1 = "修理";
                    object argIndex1 = "修理装置";
                    string argsub_situation = withBlock.FeatureName(ref argIndex1);
                    withBlock.PlayAnimation(ref argmain_situation1, ref argsub_situation);
                }
                else
                {
                    string argmain_situation2 = "修理";
                    object argIndex2 = "修理装置";
                    string argsub_situation1 = withBlock.FeatureName(ref argIndex2);
                    withBlock.SpecialEffect(ref argmain_situation2, ref argsub_situation1);
                }

                object argIndex4 = "修理装置";
                GUI.DisplaySysMessage(withBlock.Nickname + "は" + SelectedTarget.Nickname + "に" + withBlock.FeatureName(ref argIndex4) + "を使った。");

                // 修理を実行
                tmp = SelectedTarget.HP;
                object argIndex7 = "修理装置";
                switch (withBlock.FeatureLevel(ref argIndex7))
                {
                    case 1d:
                    case -1:
                        {
                            object argIndex5 = "修理";
                            string argref_mode = "";
                            SelectedTarget.RecoverHP(30d + 3d * SelectedUnit.MainPilot().SkillLevel(ref argIndex5, ref_mode: ref argref_mode));
                            break;
                        }

                    case 2d:
                        {
                            object argIndex6 = "修理";
                            string argref_mode1 = "";
                            SelectedTarget.RecoverHP(50d + 5d * SelectedUnit.MainPilot().SkillLevel(ref argIndex6, ref_mode: ref argref_mode1));
                            break;
                        }

                    case 3d:
                        {
                            SelectedTarget.RecoverHP(100d);
                            break;
                        }
                }

                string localLIndex2() { object argIndex1 = "修理装置"; string arglist = withBlock.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                if (Information.IsNumeric(localLIndex2()))
                {
                    string localLIndex() { object argIndex1 = "修理装置"; string arglist = withBlock.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                    string localLIndex1() { object argIndex1 = "修理装置"; string arglist = withBlock.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                    withBlock.EN = withBlock.EN - Conversions.ToShort(localLIndex1());
                }

                string argmsg = "+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(SelectedTarget.HP - tmp);
                GUI.DrawSysString(SelectedTarget.x, SelectedTarget.y, ref argmsg);
                object argu2 = SelectedUnit;
                GUI.UpdateMessageForm(ref SelectedTarget, ref argu2);
                string argtname = "ＨＰ";
                GUI.DisplaySysMessage(SelectedTarget.Nickname + "の" + Expression.Term(ref argtname, ref SelectedTarget) + "が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(SelectedTarget.HP - tmp) + "回復した。");

                // 経験値獲得
                string argexp_situation = "修理";
                string argexp_mode = "";
                withBlock.GetExp(ref SelectedTarget, ref argexp_situation, exp_mode: ref argexp_mode);
                if (GUI.MessageWait < 10000)
                {
                    GUI.Sleep(GUI.MessageWait);
                }
            }

            GUI.CloseMessageForm();

            // 形態変化のチェック
            SelectedTarget.Update();
            SelectedTarget.CurrentForm().CheckAutoHyperMode();
            SelectedTarget.CurrentForm().CheckAutoNormalMode();

            // 行動終了
            WaitCommand();
        }


        // 「補給」コマンドを開始
        // MOD START MARGE
        // Public Sub StartSupplyCommand()
        private static void StartSupplyCommand()
        {
            // MOD END MARGE
            short j, i, k;
            Unit t;
            SelectedCommand = "補給";

            // 射程範囲？を表示
            {
                var withBlock = SelectedUnit;
                string arguparty = "味方";
                Map.AreaInRange(withBlock.x, withBlock.y, 1, 1, ref arguparty);
                var loopTo = Map.MapWidth;
                for (i = 1; i <= loopTo; i++)
                {
                    var loopTo1 = Map.MapHeight;
                    for (j = 1; j <= loopTo1; j++)
                    {
                        if (!Map.MaskData[i, j] & Map.MapDataForUnit[i, j] is object)
                        {
                            Map.MaskData[i, j] = true;
                            {
                                var withBlock1 = Map.MapDataForUnit[i, j];
                                object argIndex1 = "ゾンビ";
                                if (withBlock1.EN < withBlock1.MaxEN & !withBlock1.IsConditionSatisfied(ref argIndex1))
                                {
                                    Map.MaskData[i, j] = false;
                                }
                                else
                                {
                                    var loopTo2 = withBlock1.CountWeapon();
                                    for (k = 1; k <= loopTo2; k++)
                                    {
                                        if (withBlock1.Bullet(k) < withBlock1.MaxBullet(k))
                                        {
                                            Map.MaskData[i, j] = false;
                                            break;
                                        }
                                    }

                                    var loopTo3 = withBlock1.CountAbility();
                                    for (k = 1; k <= loopTo3; k++)
                                    {
                                        if (withBlock1.Stock(k) < withBlock1.MaxStock(k))
                                        {
                                            Map.MaskData[i, j] = false;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                Map.MaskData[withBlock.x, withBlock.y] = false;
            }

            GUI.MaskScreen();

            // カーソル自動移動
            if (SRC.AutoMoveCursor)
            {
                // UPGRADE_NOTE: オブジェクト t をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                t = null;
                foreach (Unit u in SRC.UList)
                {
                    if (u.Status_Renamed == "出撃" & u.Party == "味方")
                    {
                        if (Map.MaskData[u.x, u.y] == false & !ReferenceEquals(u, SelectedUnit))
                        {
                            t = u;
                            break;
                        }
                    }
                }

                if (t is null)
                {
                    t = SelectedUnit;
                }

                string argcursor_mode = "ユニット選択";
                GUI.MoveCursorPos(ref argcursor_mode, t);
                if (!ReferenceEquals(SelectedUnit, t))
                {
                    Status.DisplayUnitStatus(ref t);
                }
            }

            if (CommandState == "コマンド選択")
            {
                CommandState = "ターゲット選択";
            }
            else
            {
                CommandState = "移動後ターゲット選択";
            }
        }

        // 「補給」コマンドを終了
        // MOD START MARGE
        // Public Sub FinishSupplyCommand()
        private static void FinishSupplyCommand()
        {
            // MOD END MARGE

            GUI.LockGUI();
            GUI.OpenMessageForm(ref SelectedTarget, ref SelectedUnit);
            {
                var withBlock = SelectedUnit;
                // 選択内容を変更
                Event_Renamed.SelectedUnitForEvent = SelectedUnit;
                Event_Renamed.SelectedTargetForEvent = SelectedTarget;

                // 補給メッセージ＆特殊効果
                string argmain_situation = "補給";
                if (withBlock.IsMessageDefined(ref argmain_situation))
                {
                    string argSituation = "補給";
                    string argmsg_mode = "";
                    withBlock.PilotMessage(ref argSituation, msg_mode: ref argmsg_mode);
                }

                string argmain_situation3 = "補給";
                object argIndex3 = "補給装置";
                string argsub_situation2 = withBlock.FeatureName(ref argIndex3);
                if (withBlock.IsAnimationDefined(ref argmain_situation3, ref argsub_situation2))
                {
                    string argmain_situation1 = "補給";
                    object argIndex1 = "補給装置";
                    string argsub_situation = withBlock.FeatureName(ref argIndex1);
                    withBlock.PlayAnimation(ref argmain_situation1, ref argsub_situation);
                }
                else
                {
                    string argmain_situation2 = "補給";
                    object argIndex2 = "補給装置";
                    string argsub_situation1 = withBlock.FeatureName(ref argIndex2);
                    withBlock.SpecialEffect(ref argmain_situation2, ref argsub_situation1);
                }

                object argIndex4 = "補給装置";
                GUI.DisplaySysMessage(withBlock.Nickname + "は" + SelectedTarget.Nickname + "に" + withBlock.FeatureName(ref argIndex4) + "を使った。");

                // 補給を実施
                SelectedTarget.FullSupply();
                SelectedTarget.IncreaseMorale(-10);
                string localLIndex2() { object argIndex1 = "補給装置"; string arglist = withBlock.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                if (Information.IsNumeric(localLIndex2()))
                {
                    string localLIndex() { object argIndex1 = "補給装置"; string arglist = withBlock.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                    string localLIndex1() { object argIndex1 = "補給装置"; string arglist = withBlock.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                    withBlock.EN = withBlock.EN - Conversions.ToShort(localLIndex1());
                }

                object argu2 = SelectedUnit;
                GUI.UpdateMessageForm(ref SelectedTarget, ref argu2);
                string argtname = "ＥＮ";
                GUI.DisplaySysMessage(SelectedTarget.Nickname + "の弾数と" + Expression.Term(ref argtname, ref SelectedTarget) + "が全快した。");

                // 経験値を獲得
                string argexp_situation = "補給";
                string argexp_mode = "";
                withBlock.GetExp(ref SelectedTarget, ref argexp_situation, exp_mode: ref argexp_mode);
                if (GUI.MessageWait < 10000)
                {
                    GUI.Sleep(GUI.MessageWait);
                }
            }

            // 形態変化のチェック
            SelectedTarget.Update();
            SelectedTarget.CurrentForm().CheckAutoHyperMode();
            SelectedTarget.CurrentForm().CheckAutoNormalMode();
            GUI.CloseMessageForm();

            // 行動終了
            WaitCommand();
        }


        // 「チャージ」コマンド
        // MOD START MARGE
        // Public Sub ChargeCommand()
        private static void ChargeCommand()
        {
            // MOD END MARGE
            short ret;
            Unit[] partners;
            short i;
            GUI.LockGUI();
            ret = (short)Interaction.MsgBox("チャージを開始しますか？", (MsgBoxStyle)((int)MsgBoxStyle.OkCancel + (int)MsgBoxStyle.Question), "チャージ開始");
            if (ret == (int)MsgBoxResult.Cancel)
            {
                CancelCommand();
                GUI.UnlockGUI();
                return;
            }

            // 使用イベント
            Event_Renamed.HandleEvent("使用", SelectedUnit.MainPilot().ID, "チャージ");
            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
                CommandState = "ユニット選択";
                GUI.UnlockGUI();
                return;
            }

            if (SRC.IsCanceled)
            {
                SRC.IsCanceled = false;
                CommandState = "ユニット選択";
                GUI.UnlockGUI();
                return;
            }

            {
                var withBlock = SelectedUnit;
                // チャージのメッセージを表示
                string argmain_situation = "チャージ";
                if (withBlock.IsMessageDefined(ref argmain_situation))
                {
                    Unit argu1 = null;
                    Unit argu2 = null;
                    GUI.OpenMessageForm(u1: ref argu1, u2: ref argu2);
                    string argSituation = "チャージ";
                    string argmsg_mode = "";
                    withBlock.PilotMessage(ref argSituation, msg_mode: ref argmsg_mode);
                    GUI.CloseMessageForm();
                }

                // アニメ表示を行う
                string argmain_situation3 = "チャージ";
                string argsub_situation2 = "";
                string argmain_situation4 = "チャージ";
                string argsub_situation3 = "";
                if (withBlock.IsAnimationDefined(ref argmain_situation3, sub_situation: ref argsub_situation2))
                {
                    string argmain_situation1 = "チャージ";
                    string argsub_situation = "";
                    withBlock.PlayAnimation(ref argmain_situation1, sub_situation: ref argsub_situation);
                }
                else if (withBlock.IsSpecialEffectDefined(ref argmain_situation4, sub_situation: ref argsub_situation3))
                {
                    string argmain_situation2 = "チャージ";
                    string argsub_situation1 = "";
                    withBlock.SpecialEffect(ref argmain_situation2, sub_situation: ref argsub_situation1);
                }
                else
                {
                    string argwave_name = "Charge.wav";
                    Sound.PlayWave(ref argwave_name);
                }

                // チャージ攻撃のパートナーを探す
                partners = new Unit[1];
                var loopTo = withBlock.CountWeapon();
                for (i = 1; i <= loopTo; i++)
                {
                    string argattr = "Ｃ";
                    string argattr1 = "合";
                    if (withBlock.IsWeaponClassifiedAs(i, ref argattr) & withBlock.IsWeaponClassifiedAs(i, ref argattr1))
                    {
                        string argref_mode = "チャージ";
                        if (withBlock.IsWeaponAvailable(i, ref argref_mode))
                        {
                            string argctype_Renamed = "武装";
                            withBlock.CombinationPartner(ref argctype_Renamed, i, ref partners);
                            break;
                        }
                    }
                }

                if (Information.UBound(partners) == 0)
                {
                    var loopTo1 = withBlock.CountAbility();
                    for (i = 1; i <= loopTo1; i++)
                    {
                        string argattr2 = "Ｃ";
                        string argattr3 = "合";
                        if (withBlock.IsAbilityClassifiedAs(i, ref argattr2) & withBlock.IsAbilityClassifiedAs(i, ref argattr3))
                        {
                            string argref_mode1 = "チャージ";
                            if (withBlock.IsAbilityAvailable(i, ref argref_mode1))
                            {
                                string argctype_Renamed1 = "アビリティ";
                                withBlock.CombinationPartner(ref argctype_Renamed1, i, ref partners);
                                break;
                            }
                        }
                    }
                }

                // ユニットの状態をチャージ中に
                string argcname = "チャージ";
                string argcdata = "";
                withBlock.AddCondition(ref argcname, 1, cdata: ref argcdata);

                // チャージ攻撃のパートナーもチャージ中にする
                var loopTo2 = (short)Information.UBound(partners);
                for (i = 1; i <= loopTo2; i++)
                {
                    {
                        var withBlock1 = partners[i];
                        string argcname1 = "チャージ";
                        string argcdata1 = "";
                        withBlock1.AddCondition(ref argcname1, 1, cdata: ref argcdata1);
                    }
                }
            }

            // 使用後イベント
            Event_Renamed.HandleEvent("使用後", SelectedUnit.MainPilot().ID, "チャージ");
            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
                CommandState = "ユニット選択";
                GUI.UnlockGUI();
                return;
            }

            if (SRC.IsCanceled)
            {
                SRC.IsCanceled = false;
                CommandState = "ユニット選択";
                GUI.UnlockGUI();
                return;
            }

            GUI.UnlockGUI();

            // 行動終了
            WaitCommand();
        }

        // 「会話」コマンドを開始
        // MOD START MARGE
        // Public Sub StartTalkCommand()
        private static void StartTalkCommand()
        {
            // MOD END MARGE
            short i, j;
            Unit t;
            SelectedCommand = "会話";

            // UPGRADE_NOTE: オブジェクト t をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            t = null;

            // 会話可能なユニットを表示
            {
                var withBlock = SelectedUnit;
                string arguparty = "";
                Map.AreaInRange(withBlock.x, withBlock.y, 1, 1, ref arguparty);
                var loopTo = Map.MapWidth;
                for (i = 1; i <= loopTo; i++)
                {
                    var loopTo1 = Map.MapHeight;
                    for (j = 1; j <= loopTo1; j++)
                    {
                        if (!Map.MaskData[i, j])
                        {
                            if (Map.MapDataForUnit[i, j] is object)
                            {
                                bool localIsEventDefined() { var arglname = "会話 " + withBlock.MainPilot().ID + " " + Map.MapDataForUnit[i, j].MainPilot.ID; var ret = Event_Renamed.IsEventDefined(ref arglname); return ret; }

                                if (!localIsEventDefined())
                                {
                                    Map.MaskData[i, j] = true;
                                    t = Map.MapDataForUnit[i, j];
                                }
                            }
                        }
                    }
                }

                Map.MaskData[withBlock.x, withBlock.y] = false;
            }

            GUI.MaskScreen();

            // カーソル自動移動
            if (SRC.AutoMoveCursor)
            {
                if (t is object)
                {
                    string argcursor_mode = "ユニット選択";
                    GUI.MoveCursorPos(ref argcursor_mode, t);
                    Status.DisplayUnitStatus(ref t);
                }
            }

            if (CommandState == "コマンド選択")
            {
                CommandState = "ターゲット選択";
            }
            else
            {
                CommandState = "移動後ターゲット選択";
            }
        }

        // 「会話」コマンドを終了
        // MOD START MARGE
        // Public Sub FinishTalkCommand()
        private static void FinishTalkCommand()
        {
            // MOD END MARGE
            Pilot p;
            GUI.LockGUI();
            if (SelectedUnit.CountPilot() > 0)
            {
                object argIndex1 = 1;
                p = SelectedUnit.Pilot(ref argIndex1);
            }
            else
            {
                // UPGRADE_NOTE: オブジェクト p をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                p = null;
            }

            // 会話イベントを実施
            Event_Renamed.HandleEvent("会話", SelectedUnit.MainPilot().ID, SelectedTarget.MainPilot().ID);
            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
                return;
            }

            if (p is object)
            {
                if (p.Unit_Renamed is object)
                {
                    SelectedUnit = p.Unit_Renamed;
                }
            }

            GUI.UnlockGUI();

            // 行動終了
            WaitCommand();
        }

        // 「変形」コマンド
        // MOD START MARGE
        // Public Sub TransformCommand()
        private static void TransformCommand()
        {
            // MOD END MARGE
            string[] list;
            string[] list_id;
            short i;
            short ret;
            string uname, fdata;
            string prev_uname;

            // MOD START MARGE
            // If MainWidth <> 15 Then
            if (GUI.NewGUIMode)
            {
                // MOD END MARGE
                Status.ClearUnitStatus();
            }

            GUI.LockGUI();
            object argIndex1 = "変形";
            fdata = SelectedUnit.FeatureData(ref argIndex1);
            if (string.IsNullOrEmpty(Map.MapFileName))
            {
                // ユニットステータスコマンドの場合
                {
                    var withBlock = SelectedUnit;
                    list = new string[1];
                    list_id = new string[1];
                    // 変形可能な形態一覧を作成
                    var loopTo = GeneralLib.LLength(ref fdata);
                    for (i = 2; i <= loopTo; i++)
                    {
                        object argIndex2 = GeneralLib.LIndex(ref fdata, i);
                        {
                            var withBlock1 = withBlock.OtherForm(ref argIndex2);
                            if (withBlock1.IsAvailable())
                            {
                                Array.Resize(ref list, Information.UBound(list) + 1 + 1);
                                Array.Resize(ref list_id, Information.UBound(list) + 1);
                                list[Information.UBound(list)] = withBlock1.Nickname;
                                list_id[Information.UBound(list)] = withBlock1.Name;
                            }
                        }
                    }

                    GUI.ListItemFlag = new bool[Information.UBound(list) + 1];

                    // 変形する形態を選択
                    if (Information.UBound(list) > 1)
                    {
                        GUI.TopItem = 1;
                        string arglb_caption = "変形";
                        string arglb_info = "名前";
                        string arglb_mode = "カーソル移動";
                        ret = GUI.ListBox(ref arglb_caption, ref list, ref arglb_info, ref arglb_mode);
                        if (ret == 0)
                        {
                            CancelCommand();
                            GUI.UnlockGUI();
                            return;
                        }
                    }
                    else
                    {
                        ret = 1;
                    }

                    // 変形を実施
                    Unit localOtherForm() { var tmp = list_id; object argIndex1 = tmp[(int)ret]; var ret = withBlock.OtherForm(ref argIndex1); return ret; }

                    string argnew_form = localOtherForm().Name;
                    withBlock.Transform(ref argnew_form);
                    localOtherForm().Name = argnew_form;

                    // ユニットリストの表示を更新
                    string argsmode = "";
                    Event_Renamed.MakeUnitList(smode: ref argsmode);

                    // ステータスウィンドウの表示を更新
                    Status.DisplayUnitStatus(ref withBlock.CurrentForm());

                    // コマンドを終了
                    GUI.UnlockGUI();
                    CommandState = "ユニット選択";
                    return;
                }
            }

            // 変形可能な形態の一覧を作成
            list = new string[1];
            list_id = new string[1];
            GUI.ListItemFlag = new bool[1];
            var loopTo1 = GeneralLib.LLength(ref fdata);
            for (i = 2; i <= loopTo1; i++)
            {
                object argIndex3 = GeneralLib.LIndex(ref fdata, i);
                {
                    var withBlock2 = SelectedUnit.OtherForm(ref argIndex3);
                    if (withBlock2.IsAvailable())
                    {
                        Array.Resize(ref list, Information.UBound(list) + 1 + 1);
                        Array.Resize(ref list_id, Information.UBound(list) + 1);
                        Array.Resize(ref GUI.ListItemFlag, Information.UBound(list) + 1);
                        list[Information.UBound(list)] = withBlock2.Nickname0;
                        list_id[Information.UBound(list)] = withBlock2.Name;
                        if (withBlock2.IsAbleToEnter(SelectedUnit.x, SelectedUnit.y) | string.IsNullOrEmpty(Map.MapFileName))
                        {
                            GUI.ListItemFlag[Information.UBound(list)] = false;
                        }
                        else
                        {
                            GUI.ListItemFlag[Information.UBound(list)] = true;
                        }
                    }
                }
            }

            // 変形先の形態を選択
            if (Information.UBound(list) == 1)
            {
                if (GUI.ListItemFlag[1])
                {
                    Interaction.MsgBox("この地形では" + GeneralLib.LIndex(ref fdata, 1) + "できません");
                    CancelCommand();
                    GUI.UnlockGUI();
                    return;
                }

                ret = 1;
            }
            else
            {
                GUI.TopItem = 1;
                if (!SelectedUnit.IsHero())
                {
                    string arglb_caption1 = "変形先";
                    string arglb_info1 = "名前";
                    string arglb_mode1 = "カーソル移動";
                    ret = GUI.ListBox(ref arglb_caption1, ref list, ref arglb_info1, ref arglb_mode1);
                }
                else
                {
                    string arglb_caption2 = "変身先";
                    string arglb_info2 = "名前";
                    string arglb_mode2 = "カーソル移動";
                    ret = GUI.ListBox(ref arglb_caption2, ref list, ref arglb_info2, ref arglb_mode2);
                }

                if (ret == 0)
                {
                    CancelCommand();
                    GUI.UnlockGUI();
                    return;
                }
            }

            uname = list_id[ret];
            string BGM;
            {
                var withBlock3 = SelectedUnit;
                // ダイアログでメッセージを表示させるため追加パイロットをあらかじめ作成
                object argIndex6 = uname;
                {
                    var withBlock4 = SRC.UDList.Item(ref argIndex6);
                    string argfname = "追加パイロット";
                    if (withBlock4.IsFeatureAvailable(ref argfname))
                    {
                        bool localIsDefined1() { object argIndex1 = "追加パイロット"; object argIndex2 = withBlock4.FeatureData(ref argIndex1); var ret = SRC.PList.IsDefined(ref argIndex2); return ret; }

                        if (!localIsDefined1())
                        {
                            bool localIsDefined() { object argIndex1 = "追加パイロット"; object argIndex2 = withBlock4.FeatureData(ref argIndex1); var ret = SRC.PDList.IsDefined(ref argIndex2); return ret; }

                            if (!localIsDefined())
                            {
                                object argIndex4 = "追加パイロット";
                                string argmsg = uname + "の追加パイロット「" + withBlock4.FeatureData(ref argIndex4) + "」のデータが見つかりません";
                                GUI.ErrorMessage(ref argmsg);
                                SRC.TerminateSRC();
                            }

                            object argIndex5 = "追加パイロット";
                            string argpname = withBlock4.FeatureData(ref argIndex5);
                            string argpparty = SelectedUnit.Party0;
                            string arggid = "";
                            SRC.PList.Add(ref argpname, SelectedUnit.MainPilot().Level, ref argpparty, gid: ref arggid);
                            SelectedUnit.Party0 = argpparty;
                        }
                    }
                }

                // ＢＧＭの変更
                string argfname1 = "変形ＢＧＭ";
                if (withBlock3.IsFeatureAvailable(ref argfname1))
                {
                    var loopTo2 = withBlock3.CountFeature();
                    for (i = 1; i <= loopTo2; i++)
                    {
                        string localFeature() { object argIndex1 = i; var ret = withBlock3.Feature(ref argIndex1); return ret; }

                        string localFeatureData2() { object argIndex1 = i; var ret = withBlock3.FeatureData(ref argIndex1); return ret; }

                        string localLIndex() { string arglist = hs6c18ebb7075745309751cd168b7bf5f0(); var ret = GeneralLib.LIndex(ref arglist, 1); return ret; }

                        if (localFeature() == "変形ＢＧＭ" & (localLIndex() ?? "") == (uname ?? ""))
                        {
                            string localFeatureData() { object argIndex1 = i; var ret = withBlock3.FeatureData(ref argIndex1); return ret; }

                            string localFeatureData1() { object argIndex1 = i; var ret = withBlock3.FeatureData(ref argIndex1); return ret; }

                            string argmidi_name = Strings.Mid(localFeatureData(), Strings.InStr(localFeatureData1(), " ") + 1);
                            BGM = Sound.SearchMidiFile(ref argmidi_name);
                            if (Strings.Len(BGM) > 0)
                            {
                                Sound.ChangeBGM(ref BGM);
                                GUI.Sleep(500);
                            }

                            break;
                        }
                    }
                }

                // メッセージを表示
                bool localIsMessageDefined2() { string argmain_situation = "変形(" + withBlock3.Name + "=>" + uname + ")"; var ret = withBlock3.IsMessageDefined(ref argmain_situation); return ret; }

                bool localIsMessageDefined3() { string argmain_situation = "変形(" + uname + ")"; var ret = withBlock3.IsMessageDefined(ref argmain_situation); return ret; }

                bool localIsMessageDefined4() { object argIndex1 = "変形"; string argmain_situation = "変形(" + withBlock3.FeatureName(ref argIndex1) + ")"; var ret = withBlock3.IsMessageDefined(ref argmain_situation); return ret; }

                if (localIsMessageDefined2() | localIsMessageDefined3() | localIsMessageDefined4())
                {
                    GUI.Center(withBlock3.x, withBlock3.y);
                    GUI.RefreshScreen();
                    Unit argu1 = null;
                    Unit argu2 = null;
                    GUI.OpenMessageForm(u1: ref argu1, u2: ref argu2);
                    bool localIsMessageDefined() { string argmain_situation = "変形(" + uname + ")"; var ret = withBlock3.IsMessageDefined(ref argmain_situation); return ret; }

                    bool localIsMessageDefined1() { object argIndex1 = "変形"; string argmain_situation = "変形(" + withBlock3.FeatureName(ref argIndex1) + ")"; var ret = withBlock3.IsMessageDefined(ref argmain_situation); return ret; }

                    string argmain_situation = "変形(" + withBlock3.Name + "=>" + uname + ")";
                    if (withBlock3.IsMessageDefined(ref argmain_situation))
                    {
                        string argSituation = "変形(" + withBlock3.Name + "=>" + uname + ")";
                        string argmsg_mode = "";
                        withBlock3.PilotMessage(ref argSituation, msg_mode: ref argmsg_mode);
                    }
                    else if (localIsMessageDefined())
                    {
                        string argSituation1 = "変形(" + uname + ")";
                        string argmsg_mode1 = "";
                        withBlock3.PilotMessage(ref argSituation1, msg_mode: ref argmsg_mode1);
                    }
                    else if (localIsMessageDefined1())
                    {
                        object argIndex7 = "変形";
                        string argSituation2 = "変形(" + withBlock3.FeatureName(ref argIndex7) + ")";
                        string argmsg_mode2 = "";
                        withBlock3.PilotMessage(ref argSituation2, msg_mode: ref argmsg_mode2);
                    }

                    GUI.CloseMessageForm();
                }

                // アニメ表示
                bool localIsAnimationDefined() { string argmain_situation = "変形(" + uname + ")"; string argsub_situation = ""; var ret = withBlock3.IsAnimationDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                bool localIsAnimationDefined1() { object argIndex1 = "変形"; string argmain_situation = "変形(" + withBlock3.FeatureName(ref argIndex1) + ")"; string argsub_situation = ""; var ret = withBlock3.IsAnimationDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                bool localIsSpecialEffectDefined() { string argmain_situation = "変形(" + withBlock3.Name + "=>" + uname + ")"; string argsub_situation = ""; var ret = withBlock3.IsSpecialEffectDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                bool localIsSpecialEffectDefined1() { string argmain_situation = "変形(" + uname + ")"; string argsub_situation = ""; var ret = withBlock3.IsSpecialEffectDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                bool localIsSpecialEffectDefined2() { object argIndex1 = "変形"; string argmain_situation = "変形(" + withBlock3.FeatureName(ref argIndex1) + ")"; string argsub_situation = ""; var ret = withBlock3.IsSpecialEffectDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                string argmain_situation7 = "変形(" + withBlock3.Name + "=>" + uname + ")";
                string argsub_situation6 = "";
                if (withBlock3.IsAnimationDefined(ref argmain_situation7, sub_situation: ref argsub_situation6))
                {
                    string argmain_situation1 = "変形(" + withBlock3.Name + "=>" + uname + ")";
                    string argsub_situation = "";
                    withBlock3.PlayAnimation(ref argmain_situation1, sub_situation: ref argsub_situation);
                }
                else if (localIsAnimationDefined())
                {
                    string argmain_situation2 = "変形(" + uname + ")";
                    string argsub_situation1 = "";
                    withBlock3.PlayAnimation(ref argmain_situation2, sub_situation: ref argsub_situation1);
                }
                else if (localIsAnimationDefined1())
                {
                    object argIndex8 = "変形";
                    string argmain_situation3 = "変形(" + withBlock3.FeatureName(ref argIndex8) + ")";
                    string argsub_situation2 = "";
                    withBlock3.PlayAnimation(ref argmain_situation3, sub_situation: ref argsub_situation2);
                }
                else if (localIsSpecialEffectDefined())
                {
                    string argmain_situation4 = "変形(" + withBlock3.Name + "=>" + uname + ")";
                    string argsub_situation3 = "";
                    withBlock3.SpecialEffect(ref argmain_situation4, sub_situation: ref argsub_situation3);
                }
                else if (localIsSpecialEffectDefined1())
                {
                    string argmain_situation5 = "変形(" + uname + ")";
                    string argsub_situation4 = "";
                    withBlock3.SpecialEffect(ref argmain_situation5, sub_situation: ref argsub_situation4);
                }
                else if (localIsSpecialEffectDefined2())
                {
                    object argIndex9 = "変形";
                    string argmain_situation6 = "変形(" + withBlock3.FeatureName(ref argIndex9) + ")";
                    string argsub_situation5 = "";
                    withBlock3.SpecialEffect(ref argmain_situation6, sub_situation: ref argsub_situation5);
                }
            }

            // 変形
            prev_uname = SelectedUnit.Name;
            SelectedUnit.Transform(ref uname);
            SelectedUnit = Map.MapDataForUnit[SelectedUnit.x, SelectedUnit.y];

            // 変形をキャンセルする？
            if (SelectedUnit.Action == 0)
            {
                ret = (short)Interaction.MsgBox("この形態ではこれ以上の行動が出来ません。" + Constants.vbCr + Constants.vbLf + "それでも変形しますか？", (MsgBoxStyle)((int)MsgBoxStyle.OkCancel + (int)MsgBoxStyle.Question), "変形");
                if (ret == (int)MsgBoxResult.Cancel)
                {
                    SelectedUnit.Transform(ref prev_uname);
                    SelectedUnit = Map.MapDataForUnit[SelectedUnit.x, SelectedUnit.y];
                    object argIndex11 = "消耗";
                    if (SelectedUnit.IsConditionSatisfied(ref argIndex11))
                    {
                        object argIndex10 = "消耗";
                        SelectedUnit.DeleteCondition(ref argIndex10);
                    }
                }

                GUI.RedrawScreen();
            }

            // 変形イベント
            {
                var withBlock5 = SelectedUnit.CurrentForm();
                Event_Renamed.HandleEvent("変形", withBlock5.MainPilot().ID, withBlock5.Name);
            }

            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
                Status.ClearUnitStatus();
                GUI.RedrawScreen();
                CommandState = "ユニット選択";
                GUI.UnlockGUI();
                return;
            }

            SRC.IsCanceled = false;

            // ハイパーモード・ノーマルモードの自動発動をチェック
            SelectedUnit.CurrentForm().CheckAutoHyperMode();
            SelectedUnit.CurrentForm().CheckAutoNormalMode();

            // カーソル自動移動
            if (SelectedUnit.Status_Renamed == "出撃")
            {
                if (SRC.AutoMoveCursor)
                {
                    string argcursor_mode = "ユニット選択";
                    GUI.MoveCursorPos(ref argcursor_mode, SelectedUnit);
                }

                Status.DisplayUnitStatus(ref SelectedUnit);
            }

            CommandState = "ユニット選択";
            GUI.UnlockGUI();
        }

        // 「ハイパーモード」コマンド
        // MOD START MARGE
        // Public Sub HyperModeCommand()
        private static void HyperModeCommand()
        {
            // MOD END MARGE
            string uname, fname;
            short i;

            // MOD START MARGE
            // If MainWidth <> 15 Then
            if (GUI.NewGUIMode)
            {
                // MOD END MARGE
                Status.ClearUnitStatus();
            }

            GUI.LockGUI();
            object argIndex1 = "ハイパーモード";
            string arglist = SelectedUnit.FeatureData(ref argIndex1);
            uname = GeneralLib.LIndex(ref arglist, 2);
            object argIndex2 = "ハイパーモード";
            fname = SelectedUnit.FeatureName(ref argIndex2);
            if (string.IsNullOrEmpty(Map.MapFileName))
            {
                // ユニットステータスコマンドの場合
                {
                    var withBlock = SelectedUnit;
                    string argfname = "ハイパーモード";
                    if (!withBlock.IsFeatureAvailable(ref argfname))
                    {
                        object argIndex3 = "ノーマルモード";
                        string arglist1 = SelectedUnit.FeatureData(ref argIndex3);
                        uname = GeneralLib.LIndex(ref arglist1, 1);
                    }

                    // ハイパーモードを発動
                    withBlock.Transform(ref uname);

                    // ユニットリストの表示を更新
                    string argsmode = "";
                    Event_Renamed.MakeUnitList(smode: ref argsmode);

                    // ステータスウィンドウの表示を更新
                    Status.DisplayUnitStatus(ref withBlock.CurrentForm());

                    // コマンドを終了
                    GUI.UnlockGUI();
                    CommandState = "ユニット選択";
                    return;
                }
            }

            // ハイパーモードを発動可能かどうかチェック
            object argIndex4 = uname;
            {
                var withBlock1 = SelectedUnit.OtherForm(ref argIndex4);
                if (!withBlock1.IsAbleToEnter(SelectedUnit.x, SelectedUnit.y) & !string.IsNullOrEmpty(Map.MapFileName))
                {
                    Interaction.MsgBox("この地形では変形できません");
                    GUI.UnlockGUI();
                    CancelCommand();
                    return;
                }
            }

            // ダイアログでメッセージを表示させるため追加パイロットをあらかじめ作成
            object argIndex7 = uname;
            {
                var withBlock2 = SRC.UDList.Item(ref argIndex7);
                string argfname1 = "追加パイロット";
                if (withBlock2.IsFeatureAvailable(ref argfname1))
                {
                    bool localIsDefined1() { object argIndex1 = "追加パイロット"; object argIndex2 = withBlock2.FeatureData(ref argIndex1); var ret = SRC.PList.IsDefined(ref argIndex2); return ret; }

                    if (!localIsDefined1())
                    {
                        bool localIsDefined() { object argIndex1 = "追加パイロット"; object argIndex2 = withBlock2.FeatureData(ref argIndex1); var ret = SRC.PDList.IsDefined(ref argIndex2); return ret; }

                        if (!localIsDefined())
                        {
                            object argIndex5 = "追加パイロット";
                            string argmsg = uname + "の追加パイロット「" + withBlock2.FeatureData(ref argIndex5) + "」のデータが見つかりません";
                            GUI.ErrorMessage(ref argmsg);
                            SRC.TerminateSRC();
                        }

                        object argIndex6 = "追加パイロット";
                        string argpname = withBlock2.FeatureData(ref argIndex6);
                        string argpparty = SelectedUnit.Party0;
                        string arggid = "";
                        SRC.PList.Add(ref argpname, SelectedUnit.MainPilot().Level, ref argpparty, gid: ref arggid);
                        SelectedUnit.Party0 = argpparty;
                    }
                }
            }

            string BGM;
            {
                var withBlock3 = SelectedUnit;
                // ＢＧＭを変更
                string argfname2 = "ハイパーモードＢＧＭ";
                if (withBlock3.IsFeatureAvailable(ref argfname2))
                {
                    var loopTo = withBlock3.CountFeature();
                    for (i = 1; i <= loopTo; i++)
                    {
                        string localFeature() { object argIndex1 = i; var ret = withBlock3.Feature(ref argIndex1); return ret; }

                        string localFeatureData2() { object argIndex1 = i; var ret = withBlock3.FeatureData(ref argIndex1); return ret; }

                        string localLIndex() { string arglist = hs79a81f167161473a965a38e7883f62a2(); var ret = GeneralLib.LIndex(ref arglist, 1); return ret; }

                        if (localFeature() == "ハイパーモードＢＧＭ" & (localLIndex() ?? "") == (uname ?? ""))
                        {
                            string localFeatureData() { object argIndex1 = i; var ret = withBlock3.FeatureData(ref argIndex1); return ret; }

                            string localFeatureData1() { object argIndex1 = i; var ret = withBlock3.FeatureData(ref argIndex1); return ret; }

                            string argmidi_name = Strings.Mid(localFeatureData(), Strings.InStr(localFeatureData1(), " ") + 1);
                            BGM = Sound.SearchMidiFile(ref argmidi_name);
                            if (Strings.Len(BGM) > 0)
                            {
                                Sound.ChangeBGM(ref BGM);
                                GUI.Sleep(500);
                            }

                            break;
                        }
                    }
                }

                // メッセージを表示
                bool localIsMessageDefined2() { string argmain_situation = "ハイパーモード(" + withBlock3.Name + "=>" + uname + ")"; var ret = withBlock3.IsMessageDefined(ref argmain_situation); return ret; }

                bool localIsMessageDefined3() { string argmain_situation = "ハイパーモード(" + uname + ")"; var ret = withBlock3.IsMessageDefined(ref argmain_situation); return ret; }

                bool localIsMessageDefined4() { string argmain_situation = "ハイパーモード(" + fname + ")"; var ret = withBlock3.IsMessageDefined(ref argmain_situation); return ret; }

                string argmain_situation1 = "ハイパーモード";
                if (localIsMessageDefined2() | localIsMessageDefined3() | localIsMessageDefined4() | withBlock3.IsMessageDefined(ref argmain_situation1))
                {
                    GUI.Center(withBlock3.x, withBlock3.y);
                    GUI.RefreshScreen();
                    Unit argu1 = null;
                    Unit argu2 = null;
                    GUI.OpenMessageForm(u1: ref argu1, u2: ref argu2);
                    bool localIsMessageDefined() { string argmain_situation = "ハイパーモード(" + uname + ")"; var ret = withBlock3.IsMessageDefined(ref argmain_situation); return ret; }

                    bool localIsMessageDefined1() { string argmain_situation = "ハイパーモード(" + fname + ")"; var ret = withBlock3.IsMessageDefined(ref argmain_situation); return ret; }

                    string argmain_situation = "ハイパーモード(" + withBlock3.Name + "=>" + uname + ")";
                    if (withBlock3.IsMessageDefined(ref argmain_situation))
                    {
                        string argSituation = "ハイパーモード(" + withBlock3.Name + "=>" + uname + ")";
                        string argmsg_mode = "";
                        withBlock3.PilotMessage(ref argSituation, msg_mode: ref argmsg_mode);
                    }
                    else if (localIsMessageDefined())
                    {
                        string argSituation2 = "ハイパーモード(" + uname + ")";
                        string argmsg_mode2 = "";
                        withBlock3.PilotMessage(ref argSituation2, msg_mode: ref argmsg_mode2);
                    }
                    else if (localIsMessageDefined1())
                    {
                        string argSituation3 = "ハイパーモード(" + fname + ")";
                        string argmsg_mode3 = "";
                        withBlock3.PilotMessage(ref argSituation3, msg_mode: ref argmsg_mode3);
                    }
                    else
                    {
                        string argSituation1 = "ハイパーモード";
                        string argmsg_mode1 = "";
                        withBlock3.PilotMessage(ref argSituation1, msg_mode: ref argmsg_mode1);
                    }

                    GUI.CloseMessageForm();
                }

                // アニメ表示
                bool localIsAnimationDefined() { string argmain_situation = "ハイパーモード(" + uname + ")"; string argsub_situation = ""; var ret = withBlock3.IsAnimationDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                bool localIsAnimationDefined1() { string argmain_situation = "ハイパーモード(" + fname + ")"; string argsub_situation = ""; var ret = withBlock3.IsAnimationDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                bool localIsSpecialEffectDefined() { string argmain_situation = "ハイパーモード(" + withBlock3.Name + "=>" + uname + ")"; string argsub_situation = ""; var ret = withBlock3.IsSpecialEffectDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                bool localIsSpecialEffectDefined1() { string argmain_situation = "ハイパーモード(" + uname + ")"; string argsub_situation = ""; var ret = withBlock3.IsSpecialEffectDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                bool localIsSpecialEffectDefined2() { string argmain_situation = "ハイパーモード(" + fname + ")"; string argsub_situation = ""; var ret = withBlock3.IsSpecialEffectDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                string argmain_situation10 = "ハイパーモード(" + withBlock3.Name + "=>" + uname + ")";
                string argsub_situation8 = "";
                string argmain_situation11 = "ハイパーモード";
                string argsub_situation9 = "";
                if (withBlock3.IsAnimationDefined(ref argmain_situation10, sub_situation: ref argsub_situation8))
                {
                    string argmain_situation2 = "ハイパーモード(" + withBlock3.Name + "=>" + uname + ")";
                    string argsub_situation = "";
                    withBlock3.PlayAnimation(ref argmain_situation2, sub_situation: ref argsub_situation);
                }
                else if (localIsAnimationDefined())
                {
                    string argmain_situation4 = "ハイパーモード(" + uname + ")";
                    string argsub_situation2 = "";
                    withBlock3.PlayAnimation(ref argmain_situation4, sub_situation: ref argsub_situation2);
                }
                else if (localIsAnimationDefined1())
                {
                    string argmain_situation5 = "ハイパーモード(" + fname + ")";
                    string argsub_situation3 = "";
                    withBlock3.PlayAnimation(ref argmain_situation5, sub_situation: ref argsub_situation3);
                }
                else if (withBlock3.IsAnimationDefined(ref argmain_situation11, sub_situation: ref argsub_situation9))
                {
                    string argmain_situation6 = "ハイパーモード";
                    string argsub_situation4 = "";
                    withBlock3.PlayAnimation(ref argmain_situation6, sub_situation: ref argsub_situation4);
                }
                else if (localIsSpecialEffectDefined())
                {
                    string argmain_situation7 = "ハイパーモード(" + withBlock3.Name + "=>" + uname + ")";
                    string argsub_situation5 = "";
                    withBlock3.SpecialEffect(ref argmain_situation7, sub_situation: ref argsub_situation5);
                }
                else if (localIsSpecialEffectDefined1())
                {
                    string argmain_situation8 = "ハイパーモード(" + uname + ")";
                    string argsub_situation6 = "";
                    withBlock3.SpecialEffect(ref argmain_situation8, sub_situation: ref argsub_situation6);
                }
                else if (localIsSpecialEffectDefined2())
                {
                    string argmain_situation9 = "ハイパーモード(" + fname + ")";
                    string argsub_situation7 = "";
                    withBlock3.SpecialEffect(ref argmain_situation9, sub_situation: ref argsub_situation7);
                }
                else
                {
                    string argmain_situation3 = "ハイパーモード";
                    string argsub_situation1 = "";
                    withBlock3.SpecialEffect(ref argmain_situation3, sub_situation: ref argsub_situation1);
                }
            }

            // ハイパーモード発動
            SelectedUnit.Transform(ref uname);

            // ハイパーモード・ノーマルモードの自動発動をチェック
            SelectedUnit.CurrentForm().CheckAutoHyperMode();
            SelectedUnit.CurrentForm().CheckAutoNormalMode();
            SelectedUnit = Map.MapDataForUnit[SelectedUnit.x, SelectedUnit.y];

            // 変形イベント
            {
                var withBlock4 = SelectedUnit.CurrentForm();
                Event_Renamed.HandleEvent("変形", withBlock4.MainPilot().ID, withBlock4.Name);
            }

            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
                Status.ClearUnitStatus();
                GUI.RedrawScreen();
                CommandState = "ユニット選択";
                GUI.UnlockGUI();
                return;
            }

            SRC.IsCanceled = false;

            // カーソル自動移動
            if (SelectedUnit.Status_Renamed == "出撃")
            {
                if (SRC.AutoMoveCursor)
                {
                    string argcursor_mode = "ユニット選択";
                    GUI.MoveCursorPos(ref argcursor_mode, SelectedUnit);
                }

                Status.DisplayUnitStatus(ref SelectedUnit);
            }

            CommandState = "ユニット選択";
            GUI.UnlockGUI();
        }

        // 「変身解除」コマンド
        // MOD START MARGE
        // Public Sub CancelTransformationCommand()
        private static void CancelTransformationCommand()
        {
            // MOD END MARGE
            short ret;

            // MOD START MARGE
            // If MainWidth <> 15 Then
            if (GUI.NewGUIMode)
            {
                // MOD END MARGE
                Status.ClearUnitStatus();
            }

            GUI.LockGUI();
            {
                var withBlock = SelectedUnit;
                if (string.IsNullOrEmpty(Map.MapFileName))
                {
                    // ユニットステータスコマンドの場合
                    string localLIndex() { object argIndex1 = "ノーマルモード"; string arglist = withBlock.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 1); return ret; }

                    string argnew_form = localLIndex();
                    withBlock.Transform(ref argnew_form);
                    string argsmode = "";
                    Event_Renamed.MakeUnitList(smode: ref argsmode);
                    Status.DisplayUnitStatus(ref withBlock.CurrentForm());
                    GUI.UnlockGUI();
                    CommandState = "ユニット選択";
                    return;
                }

                if (withBlock.IsHero())
                {
                    ret = (short)Interaction.MsgBox("変身を解除しますか？", (MsgBoxStyle)((int)MsgBoxStyle.OkCancel + (int)MsgBoxStyle.Question), "変身解除");
                }
                else
                {
                    ret = (short)Interaction.MsgBox("特殊モードを解除しますか？", (MsgBoxStyle)((int)MsgBoxStyle.OkCancel + (int)MsgBoxStyle.Question), "特殊モード解除");
                }

                if (ret == (int)MsgBoxResult.Cancel)
                {
                    GUI.UnlockGUI();
                    CancelCommand();
                    return;
                }

                string localLIndex1() { object argIndex1 = "ノーマルモード"; string arglist = withBlock.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 1); return ret; }

                string argnew_form1 = localLIndex1();
                withBlock.Transform(ref argnew_form1);
                SelectedUnit = Map.MapDataForUnit[withBlock.x, withBlock.y];
            }

            // カーソル自動移動
            if (SRC.AutoMoveCursor)
            {
                string argcursor_mode = "ユニット選択";
                GUI.MoveCursorPos(ref argcursor_mode, SelectedUnit);
            }

            Status.DisplayUnitStatus(ref SelectedUnit);
            GUI.RedrawScreen();

            // 変形イベント
            Event_Renamed.HandleEvent("変形", SelectedUnit.MainPilot().ID, SelectedUnit.Name);
            SRC.IsScenarioFinished = false;
            SRC.IsCanceled = false;
            CommandState = "ユニット選択";
            GUI.UnlockGUI();
        }

        // 「分離」コマンド
        // MOD START MARGE
        // Public Sub SplitCommand()
        private static void SplitCommand()
        {
            // MOD END MARGE
            string uname, tname, fname;
            short ret;
            string BGM;

            // MOD START MARGE
            // If MainWidth <> 15 Then
            if (GUI.NewGUIMode)
            {
                // MOD END MARGE
                Status.ClearUnitStatus();
            }

            GUI.LockGUI();
            if (string.IsNullOrEmpty(Map.MapFileName))
            {
                // ユニットステータスコマンドの場合

                // 分離を実施
                {
                    var withBlock = SelectedUnit;
                    string argfname = "パーツ分離";
                    if (withBlock.IsFeatureAvailable(ref argfname))
                    {
                        object argIndex1 = "パーツ分離";
                        string arglist = withBlock.FeatureData(ref argIndex1);
                        tname = GeneralLib.LIndex(ref arglist, 2);
                        withBlock.Transform(ref tname);
                    }
                    else
                    {
                        withBlock.Split_Renamed();
                    }

                    SRC.UList.CheckAutoHyperMode();
                    SRC.UList.CheckAutoNormalMode();
                    Status.DisplayUnitStatus(ref Map.MapDataForUnit[withBlock.x, withBlock.y]);
                }

                // ユニットリストの表示を更新
                string argsmode = "";
                Event_Renamed.MakeUnitList(smode: ref argsmode);

                // コマンドを終了
                CommandState = "ユニット選択";
                GUI.UnlockGUI();
                return;
            }

            {
                var withBlock1 = SelectedUnit;
                string argfname3 = "パーツ分離";
                if (withBlock1.IsFeatureAvailable(ref argfname3))
                {
                    // パーツ分離を行う場合

                    ret = (short)Interaction.MsgBox("パーツを分離しますか？", (MsgBoxStyle)((int)MsgBoxStyle.OkCancel + (int)MsgBoxStyle.Question), "パーツ分離");
                    if (ret == (int)MsgBoxResult.Cancel)
                    {
                        GUI.UnlockGUI();
                        CancelCommand();
                        return;
                    }

                    object argIndex2 = "パーツ分離";
                    string arglist1 = withBlock1.FeatureData(ref argIndex2);
                    tname = GeneralLib.LIndex(ref arglist1, 2);
                    Unit localOtherForm() { object argIndex1 = tname; var ret = withBlock1.OtherForm(ref argIndex1); return ret; }

                    if (!localOtherForm().IsAbleToEnter(withBlock1.x, withBlock1.y))
                    {
                        Interaction.MsgBox("この地形では分離できません");
                        GUI.UnlockGUI();
                        CancelCommand();
                        return;
                    }

                    // ＢＧＭ変更
                    string argfname1 = "分離ＢＧＭ";
                    if (withBlock1.IsFeatureAvailable(ref argfname1))
                    {
                        object argIndex3 = "分離ＢＧＭ";
                        string argmidi_name = withBlock1.FeatureData(ref argIndex3);
                        BGM = Sound.SearchMidiFile(ref argmidi_name);
                        if (Strings.Len(BGM) > 0)
                        {
                            object argIndex4 = "分離ＢＧＭ";
                            string argbgm_name = withBlock1.FeatureData(ref argIndex4);
                            Sound.StartBGM(ref argbgm_name);
                            GUI.Sleep(500);
                        }
                    }

                    object argIndex5 = "パーツ分離";
                    fname = withBlock1.FeatureName(ref argIndex5);

                    // メッセージを表示
                    bool localIsMessageDefined1() { string argmain_situation = "分離(" + withBlock1.Name + ")"; var ret = withBlock1.IsMessageDefined(ref argmain_situation); return ret; }

                    bool localIsMessageDefined2() { string argmain_situation = "分離(" + fname + ")"; var ret = withBlock1.IsMessageDefined(ref argmain_situation); return ret; }

                    string argmain_situation1 = "分離";
                    if (localIsMessageDefined1() | localIsMessageDefined2() | withBlock1.IsMessageDefined(ref argmain_situation1))
                    {
                        GUI.Center(withBlock1.x, withBlock1.y);
                        GUI.RefreshScreen();
                        Unit argu1 = null;
                        Unit argu2 = null;
                        GUI.OpenMessageForm(u1: ref argu1, u2: ref argu2);
                        bool localIsMessageDefined() { string argmain_situation = "分離(" + fname + ")"; var ret = withBlock1.IsMessageDefined(ref argmain_situation); return ret; }

                        string argmain_situation = "分離(" + withBlock1.Name + ")";
                        if (withBlock1.IsMessageDefined(ref argmain_situation))
                        {
                            string argSituation = "分離(" + withBlock1.Name + ")";
                            string argmsg_mode = "";
                            withBlock1.PilotMessage(ref argSituation, msg_mode: ref argmsg_mode);
                        }
                        else if (localIsMessageDefined())
                        {
                            string argSituation2 = "分離(" + fname + ")";
                            string argmsg_mode2 = "";
                            withBlock1.PilotMessage(ref argSituation2, msg_mode: ref argmsg_mode2);
                        }
                        else
                        {
                            string argSituation1 = "分離";
                            string argmsg_mode1 = "";
                            withBlock1.PilotMessage(ref argSituation1, msg_mode: ref argmsg_mode1);
                        }

                        GUI.CloseMessageForm();
                    }

                    // アニメ表示
                    bool localIsAnimationDefined() { string argmain_situation = "分離(" + fname + ")"; string argsub_situation = ""; var ret = withBlock1.IsAnimationDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                    bool localIsSpecialEffectDefined() { string argmain_situation = "分離(" + withBlock1.Name + ")"; string argsub_situation = ""; var ret = withBlock1.IsSpecialEffectDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                    bool localIsSpecialEffectDefined1() { string argmain_situation = "分離(" + fname + ")"; string argsub_situation = ""; var ret = withBlock1.IsSpecialEffectDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                    string argmain_situation8 = "分離(" + withBlock1.Name + ")";
                    string argsub_situation6 = "";
                    string argmain_situation9 = "分離";
                    string argsub_situation7 = "";
                    if (withBlock1.IsAnimationDefined(ref argmain_situation8, sub_situation: ref argsub_situation6))
                    {
                        string argmain_situation2 = "分離(" + withBlock1.Name + ")";
                        string argsub_situation = "";
                        withBlock1.PlayAnimation(ref argmain_situation2, sub_situation: ref argsub_situation);
                    }
                    else if (localIsAnimationDefined())
                    {
                        string argmain_situation4 = "分離(" + fname + ")";
                        string argsub_situation2 = "";
                        withBlock1.PlayAnimation(ref argmain_situation4, sub_situation: ref argsub_situation2);
                    }
                    else if (withBlock1.IsAnimationDefined(ref argmain_situation9, sub_situation: ref argsub_situation7))
                    {
                        string argmain_situation5 = "分離";
                        string argsub_situation3 = "";
                        withBlock1.PlayAnimation(ref argmain_situation5, sub_situation: ref argsub_situation3);
                    }
                    else if (localIsSpecialEffectDefined())
                    {
                        string argmain_situation6 = "分離(" + withBlock1.Name + ")";
                        string argsub_situation4 = "";
                        withBlock1.SpecialEffect(ref argmain_situation6, sub_situation: ref argsub_situation4);
                    }
                    else if (localIsSpecialEffectDefined1())
                    {
                        string argmain_situation7 = "分離(" + fname + ")";
                        string argsub_situation5 = "";
                        withBlock1.SpecialEffect(ref argmain_situation7, sub_situation: ref argsub_situation5);
                    }
                    else
                    {
                        string argmain_situation3 = "分離";
                        string argsub_situation1 = "";
                        withBlock1.SpecialEffect(ref argmain_situation3, sub_situation: ref argsub_situation1);
                    }

                    // パーツ分離
                    uname = withBlock1.Name;
                    withBlock1.Transform(ref tname);
                    SelectedUnit = Map.MapDataForUnit[withBlock1.x, withBlock1.y];
                    Status.DisplayUnitStatus(ref SelectedUnit);
                }
                else
                {
                    // 通常の分離を行う場合

                    ret = (short)Interaction.MsgBox("分離しますか？", (MsgBoxStyle)((int)MsgBoxStyle.OkCancel + (int)MsgBoxStyle.Question), "分離");
                    if (ret == (int)MsgBoxResult.Cancel)
                    {
                        GUI.UnlockGUI();
                        CancelCommand();
                        return;
                    }

                    // ＢＧＭを変更
                    string argfname2 = "分離ＢＧＭ";
                    if (withBlock1.IsFeatureAvailable(ref argfname2))
                    {
                        object argIndex6 = "分離ＢＧＭ";
                        string argmidi_name1 = withBlock1.FeatureData(ref argIndex6);
                        BGM = Sound.SearchMidiFile(ref argmidi_name1);
                        if (Strings.Len(BGM) > 0)
                        {
                            object argIndex7 = "分離ＢＧＭ";
                            string argbgm_name1 = withBlock1.FeatureData(ref argIndex7);
                            Sound.StartBGM(ref argbgm_name1);
                            GUI.Sleep(500);
                        }
                    }

                    object argIndex8 = "分離";
                    fname = withBlock1.FeatureName(ref argIndex8);

                    // メッセージを表示
                    bool localIsMessageDefined4() { string argmain_situation = "分離(" + withBlock1.Name + ")"; var ret = withBlock1.IsMessageDefined(ref argmain_situation); return ret; }

                    bool localIsMessageDefined5() { string argmain_situation = "分離(" + fname + ")"; var ret = withBlock1.IsMessageDefined(ref argmain_situation); return ret; }

                    string argmain_situation11 = "分離";
                    if (localIsMessageDefined4() | localIsMessageDefined5() | withBlock1.IsMessageDefined(ref argmain_situation11))
                    {
                        GUI.Center(withBlock1.x, withBlock1.y);
                        GUI.RefreshScreen();
                        Unit argu11 = null;
                        Unit argu21 = null;
                        GUI.OpenMessageForm(u1: ref argu11, u2: ref argu21);
                        bool localIsMessageDefined3() { string argmain_situation = "分離(" + fname + ")"; var ret = withBlock1.IsMessageDefined(ref argmain_situation); return ret; }

                        string argmain_situation10 = "分離(" + withBlock1.Name + ")";
                        if (withBlock1.IsMessageDefined(ref argmain_situation10))
                        {
                            string argSituation3 = "分離(" + withBlock1.Name + ")";
                            string argmsg_mode3 = "";
                            withBlock1.PilotMessage(ref argSituation3, msg_mode: ref argmsg_mode3);
                        }
                        else if (localIsMessageDefined3())
                        {
                            string argSituation5 = "分離(" + fname + ")";
                            string argmsg_mode5 = "";
                            withBlock1.PilotMessage(ref argSituation5, msg_mode: ref argmsg_mode5);
                        }
                        else
                        {
                            string argSituation4 = "分離";
                            string argmsg_mode4 = "";
                            withBlock1.PilotMessage(ref argSituation4, msg_mode: ref argmsg_mode4);
                        }

                        GUI.CloseMessageForm();
                    }

                    // アニメ表示
                    bool localIsAnimationDefined1() { string argmain_situation = "分離(" + fname + ")"; string argsub_situation = ""; var ret = withBlock1.IsAnimationDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                    bool localIsSpecialEffectDefined2() { string argmain_situation = "分離(" + withBlock1.Name + ")"; string argsub_situation = ""; var ret = withBlock1.IsSpecialEffectDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                    bool localIsSpecialEffectDefined3() { string argmain_situation = "分離(" + fname + ")"; string argsub_situation = ""; var ret = withBlock1.IsSpecialEffectDefined(ref argmain_situation, sub_situation: ref argsub_situation); return ret; }

                    string argmain_situation18 = "分離(" + withBlock1.Name + ")";
                    string argsub_situation14 = "";
                    string argmain_situation19 = "分離";
                    string argsub_situation15 = "";
                    if (withBlock1.IsAnimationDefined(ref argmain_situation18, sub_situation: ref argsub_situation14))
                    {
                        string argmain_situation12 = "分離(" + withBlock1.Name + ")";
                        string argsub_situation8 = "";
                        withBlock1.PlayAnimation(ref argmain_situation12, sub_situation: ref argsub_situation8);
                    }
                    else if (localIsAnimationDefined1())
                    {
                        string argmain_situation14 = "分離(" + fname + ")";
                        string argsub_situation10 = "";
                        withBlock1.PlayAnimation(ref argmain_situation14, sub_situation: ref argsub_situation10);
                    }
                    else if (withBlock1.IsAnimationDefined(ref argmain_situation19, sub_situation: ref argsub_situation15))
                    {
                        string argmain_situation15 = "分離";
                        string argsub_situation11 = "";
                        withBlock1.PlayAnimation(ref argmain_situation15, sub_situation: ref argsub_situation11);
                    }
                    else if (localIsSpecialEffectDefined2())
                    {
                        string argmain_situation16 = "分離(" + withBlock1.Name + ")";
                        string argsub_situation12 = "";
                        withBlock1.SpecialEffect(ref argmain_situation16, sub_situation: ref argsub_situation12);
                    }
                    else if (localIsSpecialEffectDefined3())
                    {
                        string argmain_situation17 = "分離(" + fname + ")";
                        string argsub_situation13 = "";
                        withBlock1.SpecialEffect(ref argmain_situation17, sub_situation: ref argsub_situation13);
                    }
                    else
                    {
                        string argmain_situation13 = "分離";
                        string argsub_situation9 = "";
                        withBlock1.SpecialEffect(ref argmain_situation13, sub_situation: ref argsub_situation9);
                    }

                    // 分離
                    uname = withBlock1.Name;
                    withBlock1.Split_Renamed();

                    // 選択ユニットを再設定
                    string localLIndex() { object argIndex1 = "分離"; string arglist = withBlock1.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                    object argIndex9 = localLIndex();
                    SelectedUnit = SRC.UList.Item(ref argIndex9);
                    Status.DisplayUnitStatus(ref SelectedUnit);
                }
            }

            // 分離イベント
            Event_Renamed.HandleEvent("分離", SelectedUnit.MainPilot().ID, uname);
            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
                Status.ClearUnitStatus();
                GUI.RedrawScreen();
                CommandState = "ユニット選択";
                GUI.UnlockGUI();
                return;
            }

            SRC.IsCanceled = false;

            // カーソル自動移動
            if (SRC.AutoMoveCursor)
            {
                string argcursor_mode = "ユニット選択";
                GUI.MoveCursorPos(ref argcursor_mode, SelectedUnit);
            }

            // ハイパーモード＆ノーマルモードの自動発動チェック
            SRC.UList.CheckAutoHyperMode();
            SRC.UList.CheckAutoNormalMode();
            CommandState = "ユニット選択";
            GUI.UnlockGUI();
        }

        // 「合体」コマンド
        // MOD START MARGE
        // Public Sub CombineCommand()
        private static void CombineCommand()
        {
            // MOD END MARGE
            short i, j;
            string[] list;
            Unit u;

            // MOD START MARGE
            // If MainWidth <> 15 Then
            if (GUI.NewGUIMode)
            {
                // MOD END MARGE
                Status.ClearUnitStatus();
            }

            GUI.LockGUI();
            list = new string[1];
            GUI.ListItemFlag = new bool[1];
            {
                var withBlock = SelectedUnit;
                if (string.IsNullOrEmpty(Map.MapFileName))
                {
                    // ユニットステータスコマンドの時
                    // パーツ合体ならば……
                    // UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                    string argfname = "パーツ合体";
                    if (GUI.MainForm.mnuUnitCommandItem.Item(CombineCmdID).Caption == "パーツ合体" & withBlock.IsFeatureAvailable(ref argfname))
                    {
                        // パーツ合体を実施
                        object argIndex1 = "パーツ合体";
                        string argnew_form = withBlock.FeatureData(ref argIndex1);
                        withBlock.Transform(ref argnew_form);
                        Status.DisplayUnitStatus(ref Map.MapDataForUnit[withBlock.x, withBlock.y]);
                        Map.MapDataForUnit[withBlock.x, withBlock.y].CheckAutoHyperMode();
                        Map.MapDataForUnit[withBlock.x, withBlock.y].CheckAutoNormalMode();

                        // ユニットリストの表示を更新
                        string argsmode = "";
                        Event_Renamed.MakeUnitList(smode: ref argsmode);

                        // コマンドを終了
                        CommandState = "ユニット選択";
                        GUI.UnlockGUI();
                        return;
                    }
                }

                // 選択可能な合体パターンのリストを作成
                var loopTo = withBlock.CountFeature();
                for (i = 1; i <= loopTo; i++)
                {
                    string localFeature() { object argIndex1 = i; var ret = withBlock.Feature(ref argIndex1); return ret; }

                    string localFeatureData4() { object argIndex1 = i; var ret = withBlock.FeatureData(ref argIndex1); return ret; }

                    short localLLength() { string arglist = hsdebc20cc3cfc47d3be3b16b45d9b88b4(); var ret = GeneralLib.LLength(ref arglist); return ret; }

                    if (localFeature() == "合体" & (localLLength() > 3 | string.IsNullOrEmpty(Map.MapFileName)))
                    {
                        string localFeatureData() { object argIndex1 = i; var ret = withBlock.FeatureData(ref argIndex1); return ret; }

                        string localLIndex() { string arglist = hs8ad4cb28843f4cd79e5881037333d438(); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                        bool localIsDefined() { object argIndex1 = (object)hs340135fe58cf47dbaf5b402aedf48d4a(); var ret = SRC.UList.IsDefined(ref argIndex1); return ret; }

                        if (!localIsDefined())
                        {
                            goto NextLoop;
                        }

                        string localFeatureData2() { object argIndex1 = i; var ret = withBlock.FeatureData(ref argIndex1); return ret; }

                        string arglist = localFeatureData2();
                        var loopTo1 = GeneralLib.LLength(ref arglist);
                        for (j = 3; j <= loopTo1; j++)
                        {
                            string localFeatureData1() { object argIndex1 = i; var ret = withBlock.FeatureData(ref argIndex1); return ret; }

                            string localLIndex1() { string arglist = hs751afdc814f14f178b9bc1e2c197a85c(); var ret = GeneralLib.LIndex(ref arglist, j); return ret; }

                            object argIndex2 = localLIndex1();
                            u = SRC.UList.Item(ref argIndex2);
                            if (u is null)
                            {
                                goto NextLoop;
                            }

                            if (!u.IsOperational())
                            {
                                goto NextLoop;
                            }

                            string argfname1 = "合体制限";
                            if (u.Status_Renamed != "出撃" & u.CurrentForm().IsFeatureAvailable(ref argfname1))
                            {
                                goto NextLoop;
                            }

                            if (!string.IsNullOrEmpty(Map.MapFileName))
                            {
                                if (Math.Abs((short)(withBlock.x - u.CurrentForm().x)) > 2 | Math.Abs((short)(withBlock.y - u.CurrentForm().y)) > 2)
                                {
                                    goto NextLoop;
                                }
                            }
                        }

                        Array.Resize(ref list, Information.UBound(list) + 1 + 1);
                        GUI.ListItemFlag = new bool[Information.UBound(list) + 1];
                        string localFeatureData3() { object argIndex1 = i; var ret = withBlock.FeatureData(ref argIndex1); return ret; }

                        string arglist1 = localFeatureData3();
                        list[Information.UBound(list)] = GeneralLib.LIndex(ref arglist1, 2);
                        GUI.ListItemFlag[Information.UBound(list)] = false;
                    }

                    NextLoop:
                    ;
                }

                // どの合体を行うかを選択
                if (Information.UBound(list) == 1)
                {
                    i = 1;
                }
                else
                {
                    GUI.TopItem = 1;
                    string arglb_caption = "合体後の形態";
                    string arglb_info = "名前";
                    string arglb_mode = "";
                    i = GUI.ListBox(ref arglb_caption, ref list, ref arglb_info, lb_mode: ref arglb_mode);
                    if (i == 0)
                    {
                        CancelCommand();
                        GUI.UnlockGUI();
                        return;
                    }
                }
            }

            if (string.IsNullOrEmpty(Map.MapFileName))
            {
                // ユニットステータスコマンドの時
                SelectedUnit.Combine(ref list[i], true);

                // ハイパーモード・ノーマルモードの自動発動をチェック
                SRC.UList.CheckAutoHyperMode();
                SRC.UList.CheckAutoNormalMode();

                // ユニットリストの表示を更新
                string argsmode1 = "";
                Event_Renamed.MakeUnitList(smode: ref argsmode1);

                // コマンドを終了
                CommandState = "ユニット選択";
                GUI.UnlockGUI();
                return;
            }

            // 合体！
            SelectedUnit.Combine(ref list[i]);

            // ハイパーモード＆ノーマルモードの自動発動
            SRC.UList.CheckAutoHyperMode();
            SRC.UList.CheckAutoNormalMode();

            // 合体後のユニットを選択しておく
            SelectedUnit = Map.MapDataForUnit[SelectedUnit.x, SelectedUnit.y];

            // 行動数消費
            SelectedUnit.UseAction();

            // カーソル自動移動
            if (SRC.AutoMoveCursor)
            {
                string argcursor_mode = "ユニット選択";
                GUI.MoveCursorPos(ref argcursor_mode, SelectedUnit);
            }

            Status.DisplayUnitStatus(ref SelectedUnit);

            // 合体イベント
            Event_Renamed.HandleEvent("合体", SelectedUnit.MainPilot().ID, SelectedUnit.Name);
            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
                GUI.UnlockGUI();
                return;
            }

            if (SRC.IsCanceled)
            {
                SRC.IsCanceled = false;
                Status.ClearUnitStatus();
                GUI.RedrawScreen();
                CommandState = "ユニット選択";
                GUI.UnlockGUI();
                return;
            }

            // 行動終了
            WaitCommand(true);
        }

        // 「換装」コマンド
        // MOD START MARGE
        // Public Sub ExchangeFormCommand()
        public static void ExchangeFormCommand()
        {
            // MOD END MARGE
            string[] list;
            string[] id_list;
            short j, i, k;
            int max_value;
            short ret;
            string fdata;
            string[] farray;
            GUI.LockGUI();
            {
                var withBlock = SelectedUnit;
                object argIndex1 = "換装";
                fdata = withBlock.FeatureData(ref argIndex1);

                // 選択可能な換装先のリストを作成
                list = new string[1];
                id_list = new string[1];
                GUI.ListItemComment = new string[1];
                var loopTo = GeneralLib.LLength(ref fdata);
                for (i = 1; i <= loopTo; i++)
                {
                    object argIndex5 = GeneralLib.LIndex(ref fdata, i);
                    {
                        var withBlock1 = withBlock.OtherForm(ref argIndex5);
                        if (withBlock1.IsAvailable())
                        {
                            Array.Resize(ref list, Information.UBound(list) + 1 + 1);
                            Array.Resize(ref id_list, Information.UBound(list) + 1);
                            Array.Resize(ref GUI.ListItemComment, Information.UBound(list) + 1);
                            id_list[Information.UBound(list)] = withBlock1.Name;

                            // 各形態の表示内容を作成
                            if ((SelectedUnit.Nickname0 ?? "") == (withBlock1.Nickname ?? ""))
                            {
                                string argbuf = withBlock1.Name;
                                list[Information.UBound(list)] = GeneralLib.RightPaddedString(ref argbuf, 27);
                                withBlock1.Name = argbuf;
                            }
                            else
                            {
                                string argbuf1 = withBlock1.Nickname0;
                                list[Information.UBound(list)] = GeneralLib.RightPaddedString(ref argbuf1, 27);
                                withBlock1.Nickname0 = argbuf1;
                            }

                            string localLeftPaddedString() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock1.MaxHP); var ret = GeneralLib.LeftPaddedString(ref argbuf, 6); return ret; }

                            string localLeftPaddedString1() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock1.MaxEN); var ret = GeneralLib.LeftPaddedString(ref argbuf, 5); return ret; }

                            string localLeftPaddedString2() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock1.get_Armor("")); var ret = GeneralLib.LeftPaddedString(ref argbuf, 5); return ret; }

                            string localLeftPaddedString3() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock1.get_Mobility("")); var ret = GeneralLib.LeftPaddedString(ref argbuf, 5); return ret; }

                            list[Information.UBound(list)] = list[Information.UBound(list)] + localLeftPaddedString() + localLeftPaddedString1() + localLeftPaddedString2() + localLeftPaddedString3() + " " + withBlock1.Data.Adaption;

                            // 最大攻撃力
                            max_value = 0;
                            var loopTo1 = withBlock1.CountWeapon();
                            for (j = 1; j <= loopTo1; j++)
                            {
                                string argattr = "合";
                                if (withBlock1.IsWeaponMastered(j) & !withBlock1.IsDisabled(ref withBlock1.Weapon(j).Name) & !withBlock1.IsWeaponClassifiedAs(j, ref argattr))
                                {
                                    string argtarea1 = "";
                                    if (withBlock1.WeaponPower(j, ref argtarea1) > max_value)
                                    {
                                        string argtarea = "";
                                        max_value = withBlock1.WeaponPower(j, ref argtarea);
                                    }
                                }
                            }

                            string localLeftPaddedString4() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(max_value); var ret = GeneralLib.LeftPaddedString(ref argbuf, 7); return ret; }

                            list[Information.UBound(list)] = list[Information.UBound(list)] + localLeftPaddedString4();

                            // 最大射程
                            max_value = 0;
                            var loopTo2 = withBlock1.CountWeapon();
                            for (j = 1; j <= loopTo2; j++)
                            {
                                string argattr1 = "合";
                                if (withBlock1.IsWeaponMastered(j) & !withBlock1.IsDisabled(ref withBlock1.Weapon(j).Name) & !withBlock1.IsWeaponClassifiedAs(j, ref argattr1))
                                {
                                    if (withBlock1.WeaponMaxRange(j) > max_value)
                                    {
                                        max_value = withBlock1.WeaponMaxRange(j);
                                    }
                                }
                            }

                            string localLeftPaddedString5() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(max_value); var ret = GeneralLib.LeftPaddedString(ref argbuf, 5); return ret; }

                            list[Information.UBound(list)] = list[Information.UBound(list)] + localLeftPaddedString5();

                            // 換装先が持つ特殊能力一覧
                            farray = new string[1];
                            var loopTo3 = withBlock1.CountFeature();
                            for (j = 1; j <= loopTo3; j++)
                            {
                                object argIndex4 = j;
                                if (!string.IsNullOrEmpty(withBlock1.FeatureName(ref argIndex4)))
                                {
                                    // 重複する特殊能力は表示しないようチェック
                                    var loopTo4 = (short)Information.UBound(farray);
                                    for (k = 1; k <= loopTo4; k++)
                                    {
                                        object argIndex2 = j;
                                        if ((withBlock1.FeatureName(ref argIndex2) ?? "") == (farray[k] ?? ""))
                                        {
                                            break;
                                        }
                                    }

                                    if (k > Information.UBound(farray))
                                    {
                                        string localFeatureName() { object argIndex1 = j; var ret = withBlock1.FeatureName(ref argIndex1); return ret; }

                                        GUI.ListItemComment[Information.UBound(list)] = GUI.ListItemComment[Information.UBound(list)] + localFeatureName() + " ";
                                        Array.Resize(ref farray, Information.UBound(farray) + 1 + 1);
                                        object argIndex3 = j;
                                        farray[Information.UBound(farray)] = withBlock1.FeatureName(ref argIndex3);
                                    }
                                }
                            }
                        }
                    }
                }

                GUI.ListItemFlag = new bool[Information.UBound(list) + 1];

                // どの形態に換装するかを選択
                GUI.TopItem = 1;
                string arglb_caption = "変更先選択";
                string argtname = "ＨＰ";
                Unit argu = null;
                string argtname1 = "ＥＮ";
                Unit argu1 = null;
                string argtname2 = "装甲";
                Unit argu2 = null;
                string argtname3 = "運動";
                Unit argu3 = null;
                string arglb_info = "ユニット                     " + Expression.Term(ref argtname, ref argu, 4) + " " + Expression.Term(ref argtname1, ref argu1, 4) + " " + Expression.Term(ref argtname2, ref argu2, 4) + " " + Expression.Term(ref argtname3, ref argu3, 4) + " " + "適応 攻撃力 射程";
                string arglb_mode = "カーソル移動,コメント";
                ret = GUI.ListBox(ref arglb_caption, ref list, ref arglb_info, ref arglb_mode);
                if (ret == 0)
                {
                    CancelCommand();
                    GUI.UnlockGUI();
                    return;
                }

                // 換装を実施
                Unit localOtherForm() { var tmp = id_list; object argIndex1 = tmp[(int)ret]; var ret = withBlock.OtherForm(ref argIndex1); return ret; }

                string argnew_form = localOtherForm().Name;
                withBlock.Transform(ref argnew_form);
                localOtherForm().Name = argnew_form;

                // ユニットリストの再構築
                string argsmode = "";
                Event_Renamed.MakeUnitList(smode: ref argsmode);

                // カーソル自動移動
                if (SRC.AutoMoveCursor)
                {
                    string argcursor_mode = "ユニット選択";
                    GUI.MoveCursorPos(ref argcursor_mode, withBlock.CurrentForm());
                }

                Status.DisplayUnitStatus(ref withBlock.CurrentForm());
            }

            CommandState = "ユニット選択";
            GUI.UnlockGUI();
        }


        // 「発進」コマンドを開始
        // MOD START MARGE
        // Public Sub StartLaunchCommand()
        private static void StartLaunchCommand()
        {
            // MOD END MARGE
            short i, ret;
            string[] list;
            {
                var withBlock = SelectedUnit;
                list = new string[(withBlock.CountUnitOnBoard() + 1)];
                GUI.ListItemID = new string[(withBlock.CountUnitOnBoard() + 1)];
                GUI.ListItemFlag = new bool[(withBlock.CountUnitOnBoard() + 1)];
            }

            // 母艦に搭載しているユニットの一覧を作成
            var loopTo = SelectedUnit.CountUnitOnBoard();
            for (i = 1; i <= loopTo; i++)
            {
                object argIndex1 = i;
                {
                    var withBlock1 = SelectedUnit.UnitOnBoard(ref argIndex1);
                    string localRightPaddedString() { string argbuf = withBlock1.Nickname0; var ret = GeneralLib.RightPaddedString(ref argbuf, 25); withBlock1.Nickname0 = argbuf; return ret; }

                    string localRightPaddedString1() { string argbuf = withBlock1.MainPilot().get_Nickname(false); var ret = GeneralLib.RightPaddedString(ref argbuf, 17); withBlock1.MainPilot().get_Nickname(false) = argbuf; return ret; }

                    string localLeftPaddedString() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock1.MainPilot().Level); var ret = GeneralLib.LeftPaddedString(ref argbuf, 2); return ret; }

                    string localRightPaddedString2() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock1.HP) + "/" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock1.MaxHP); var ret = GeneralLib.RightPaddedString(ref argbuf, 12); return ret; }

                    string localRightPaddedString3() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock1.EN) + "/" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock1.MaxEN); var ret = GeneralLib.RightPaddedString(ref argbuf, 8); return ret; }

                    list[i] = localRightPaddedString() + localRightPaddedString1() + localLeftPaddedString() + " " + localRightPaddedString2() + localRightPaddedString3();
                    GUI.ListItemID[i] = withBlock1.ID;
                    if (withBlock1.Action > 0)
                    {
                        GUI.ListItemFlag[i] = false;
                    }
                    else
                    {
                        GUI.ListItemFlag[i] = true;
                    }
                }
            }

            // どのユニットを発進させるか選択
            GUI.TopItem = 1;
            string arglb_caption = "ユニット選択";
            string argtname = "ＨＰ";
            Unit argu = null;
            string argtname1 = "ＥＮ";
            Unit argu1 = null;
            string arglb_info = "ユニット名               パイロット       Lv " + Expression.Term(ref argtname, ref argu, 8) + Expression.Term(ref argtname1, u: ref argu1);
            string arglb_mode = "カーソル移動";
            ret = GUI.ListBox(ref arglb_caption, ref list, ref arglb_info, ref arglb_mode);

            // キャンセルされた？
            if (ret == 0)
            {
                GUI.ListItemID = new string[1];
                CancelCommand();
                return;
            }

            SelectedCommand = "発進";

            // ユニットの発進処理
            var tmp = GUI.ListItemID;
            object argIndex2 = tmp[ret];
            SelectedTarget = SRC.UList.Item(ref argIndex2);
            {
                var withBlock2 = SelectedTarget;
                withBlock2.x = SelectedUnit.x;
                withBlock2.y = SelectedUnit.y;
                string localLIndex() { object argIndex1 = "テレポート"; string arglist = withBlock2.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                short localLLength() { object argIndex1 = "ジャンプ"; string arglist = withBlock2.FeatureData(ref argIndex1); var ret = GeneralLib.LLength(ref arglist); return ret; }

                string localLIndex1() { object argIndex1 = "ジャンプ"; string arglist = withBlock2.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                string argfname = "テレポート";
                string argfname1 = "ジャンプ";
                if (withBlock2.IsFeatureAvailable(ref argfname) & (withBlock2.Data.Speed == 0 | localLIndex() == "0"))
                {
                    // テレポートによる発進
                    Map.AreaInTeleport(ref SelectedTarget);
                }
                else if (withBlock2.IsFeatureAvailable(ref argfname1) & (withBlock2.Data.Speed == 0 | localLLength() < 2 | localLIndex1() == "0"))
                {
                    // ジャンプによる発進
                    Map.AreaInSpeed(ref SelectedTarget, true);
                }
                else
                {
                    // 通常移動による発進
                    Map.AreaInSpeed(ref SelectedTarget);
                }

                // 母艦を中央表示
                GUI.Center(withBlock2.x, withBlock2.y);

                // 発進させるユニットを母艦の代わりに表示
                if (withBlock2.BitmapID == 0)
                {
                    object argIndex3 = withBlock2.Name;
                    {
                        var withBlock3 = SRC.UList.Item(ref argIndex3);
                        if ((SelectedTarget.Party0 ?? "") == (withBlock3.Party0 ?? "") & withBlock3.BitmapID != 0 & (SelectedTarget.get_Bitmap(false) ?? "") == (withBlock3.get_Bitmap(false) ?? ""))
                        {
                            SelectedTarget.BitmapID = withBlock3.BitmapID;
                        }
                        else
                        {
                            SelectedTarget.BitmapID = GUI.MakeUnitBitmap(ref SelectedTarget);
                        }
                    }

                    withBlock2.Name = Conversions.ToString(argIndex3);
                }

                GUI.MaskScreen();
            }

            GUI.ListItemID = new string[1];
            if (CommandState == "コマンド選択")
            {
                CommandState = "ターゲット選択";
            }
        }

        // 「発進」コマンドを終了
        // MOD START MARGE
        // Public Sub FinishLaunchCommand()
        private static void FinishLaunchCommand()
        {
            // MOD END MARGE
            short ret;
            GUI.LockGUI();
            {
                var withBlock = SelectedTarget;
                // 発進コマンドの目的地にユニットがいた場合
                if (Map.MapDataForUnit[SelectedX, SelectedY] is object)
                {
                    string argfname = "母艦";
                    if (Map.MapDataForUnit[SelectedX, SelectedY].IsFeatureAvailable(ref argfname))
                    {
                        ret = (short)Interaction.MsgBox("着艦しますか？", (MsgBoxStyle)((int)MsgBoxStyle.OkCancel + (int)MsgBoxStyle.Question), "着艦");
                    }
                    else
                    {
                        ret = (short)Interaction.MsgBox("合体しますか？", (MsgBoxStyle)((int)MsgBoxStyle.OkCancel + (int)MsgBoxStyle.Question), "合体");
                    }

                    if (ret == (int)MsgBoxResult.Cancel)
                    {
                        CancelCommand();
                        GUI.UnlockGUI();
                        return;
                    }
                }

                // メッセージの表示
                string argmain_situation = "発進(" + withBlock.Name + ")";
                string argmain_situation1 = "発進";
                if (withBlock.IsMessageDefined(ref argmain_situation))
                {
                    Unit argu1 = null;
                    Unit argu2 = null;
                    GUI.OpenMessageForm(u1: ref argu1, u2: ref argu2);
                    string argSituation = "発進(" + withBlock.Name + ")";
                    string argmsg_mode = "";
                    withBlock.PilotMessage(ref argSituation, msg_mode: ref argmsg_mode);
                    GUI.CloseMessageForm();
                }
                else if (withBlock.IsMessageDefined(ref argmain_situation1))
                {
                    Unit argu11 = null;
                    Unit argu21 = null;
                    GUI.OpenMessageForm(u1: ref argu11, u2: ref argu21);
                    string argSituation1 = "発進";
                    string argmsg_mode1 = "";
                    withBlock.PilotMessage(ref argSituation1, msg_mode: ref argmsg_mode1);
                    GUI.CloseMessageForm();
                }

                string argmain_situation2 = "発進";
                string argsub_situation = withBlock.Name;
                withBlock.SpecialEffect(ref argmain_situation2, ref argsub_situation);
                withBlock.Name = argsub_situation;
                PrevUnitArea = withBlock.Area;
                PrevUnitEN = (short)withBlock.EN;
                withBlock.Status_Renamed = "出撃";

                // 指定した位置に発進したユニットを移動
                withBlock.Move(SelectedX, SelectedY);
            }

            // 発進したユニットを母艦から降ろす
            {
                var withBlock1 = SelectedUnit;
                PrevUnitX = withBlock1.x;
                PrevUnitY = withBlock1.y;
                withBlock1.UnloadUnit(ref (object)SelectedTarget.ID);

                // 母艦の位置には発進したユニットが表示されているので元に戻しておく
                Map.MapDataForUnit[withBlock1.x, withBlock1.y] = SelectedUnit;
                GUI.PaintUnitBitmap(ref SelectedUnit);
            }

            SelectedUnit = SelectedTarget;
            {
                var withBlock2 = SelectedUnit;
                if ((Map.MapDataForUnit[withBlock2.x, withBlock2.y].ID ?? "") != (withBlock2.ID ?? ""))
                {
                    GUI.RedrawScreen();
                    CommandState = "ユニット選択";
                    GUI.UnlockGUI();
                    return;
                }
            }

            CommandState = "移動後コマンド選択";
            GUI.UnlockGUI();
            ProceedCommand();
        }


        // 「命令」コマンドを開始
        // MOD START MARGE
        // Public Sub StartOrderCommand()
        private static void StartOrderCommand()
        {
            // MOD END MARGE
            string[] list;
            short i, ret, j;
            GUI.LockGUI();
            list = new string[5];
            GUI.ListItemFlag = new bool[5];

            // 可能な命令内容一覧を作成
            list[1] = "自由：自由に行動させる";
            list[2] = "移動：指定した位置に移動";
            list[3] = "攻撃：指定した敵を攻撃";
            list[4] = "護衛：指定したユニットを護衛";
            if (SelectedUnit.Summoner is object | SelectedUnit.Master is object)
            {
                Array.Resize(ref list, 6);
                Array.Resize(ref GUI.ListItemFlag, 6);
                if (SelectedUnit.Master is object)
                {
                    list[5] = "帰還：主人の所に戻る";
                }
                else
                {
                    list[5] = "帰還：召喚主の所に戻る";
                }
            }

            // 命令する行動パターンを選択
            string arglb_caption = "命令";
            string arglb_info = "行動パターン";
            string arglb_mode = "";
            ret = GUI.ListBox(ref arglb_caption, ref list, ref arglb_info, lb_mode: ref arglb_mode);

            // 選択された行動パターンに応じてターゲット領域を表示
            switch (ret)
            {
                case 0:
                    {
                        CancelCommand();
                        break;
                    }

                case 1: // 自由
                    {
                        SelectedUnit.Mode = "通常";
                        CommandState = "ユニット選択";
                        Status.DisplayUnitStatus(ref SelectedUnit);
                        break;
                    }

                case 2: // 移動
                    {
                        SelectedCommand = "移動命令";
                        var loopTo = Map.MapWidth;
                        for (i = 1; i <= loopTo; i++)
                        {
                            var loopTo1 = Map.MapHeight;
                            for (j = 1; j <= loopTo1; j++)
                                Map.MaskData[i, j] = false;
                        }

                        GUI.MaskScreen();
                        CommandState = "ターゲット選択";
                        break;
                    }

                case 3: // 攻撃
                    {
                        SelectedCommand = "攻撃命令";
                        string arguparty = "味方の敵";
                        Map.AreaWithUnit(ref arguparty);
                        Map.MaskData[SelectedUnit.x, SelectedUnit.y] = true;
                        GUI.MaskScreen();
                        CommandState = "ターゲット選択";
                        break;
                    }

                case 4: // 護衛
                    {
                        SelectedCommand = "護衛命令";
                        string arguparty1 = "味方";
                        Map.AreaWithUnit(ref arguparty1);
                        Map.MaskData[SelectedUnit.x, SelectedUnit.y] = true;
                        GUI.MaskScreen();
                        CommandState = "ターゲット選択";
                        break;
                    }

                case 5: // 帰還
                    {
                        if (SelectedUnit.Master is object)
                        {
                            SelectedUnit.Mode = SelectedUnit.Master.MainPilot().ID;
                        }
                        else
                        {
                            SelectedUnit.Mode = SelectedUnit.Summoner.MainPilot().ID;
                        }

                        CommandState = "ユニット選択";
                        Status.DisplayUnitStatus(ref SelectedUnit);
                        break;
                    }
            }

            GUI.UnlockGUI();
        }

        // 「命令」コマンドを終了
        // MOD START MARGE
        // Public Sub FinishOrderCommand()
        private static void FinishOrderCommand()
        {
            // MOD END MARGE
            switch (SelectedCommand ?? "")
            {
                case "移動命令":
                    {
                        SelectedUnit.Mode = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(SelectedX) + " " + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(SelectedY);
                        break;
                    }

                case "攻撃命令":
                case "護衛命令":
                    {
                        SelectedUnit.Mode = SelectedTarget.MainPilot().ID;
                        break;
                    }
            }

            if (ReferenceEquals(Status.DisplayedUnit, SelectedUnit))
            {
                Status.DisplayUnitStatus(ref SelectedUnit);
            }

            GUI.RedrawScreen();
            CommandState = "ユニット選択";
        }


        // 「特殊能力一覧」コマンド
        // MOD START MARGE
        // Public Sub FeatureListCommand()
        private static void FeatureListCommand()
        {
            // MOD END MARGE
            string[] list;
            var id_list = default(string[]);
            bool[] is_unit_feature;
            short i, j;
            var ret = default(short);
            string fname0, fname, ftype;
            GUI.LockGUI();

            // 表示する特殊能力名一覧の作成
            list = new string[1];
            var id_ist = new object[1];
            is_unit_feature = new bool[1];

            // 武器・防具クラス
            string argoname = "アイテム交換";
            if (Expression.IsOptionDefined(ref argoname))
            {
                {
                    var withBlock = SelectedUnit;
                    string argfname = "武器クラス";
                    string argfname1 = "防具クラス";
                    if (withBlock.IsFeatureAvailable(ref argfname) | withBlock.IsFeatureAvailable(ref argfname1))
                    {
                        Array.Resize(ref list, Information.UBound(list) + 1 + 1);
                        Array.Resize(ref id_list, Information.UBound(list) + 1);
                        Array.Resize(ref is_unit_feature, Information.UBound(list) + 1);
                        list[Information.UBound(list)] = "武器・防具クラス";
                        id_list[Information.UBound(list)] = "武器・防具クラス";
                        is_unit_feature[Information.UBound(list)] = true;
                    }
                }
            }

            {
                var withBlock1 = SelectedUnit.MainPilot();
                // パイロット特殊能力
                var loopTo = withBlock1.CountSkill();
                for (i = 1; i <= loopTo; i++)
                {
                    object argIndex3 = i;
                    switch (withBlock1.Skill(ref argIndex3) ?? "")
                    {
                        case "得意技":
                        case "不得手":
                            {
                                object argIndex1 = i;
                                fname = withBlock1.Skill(ref argIndex1);
                                break;
                            }

                        default:
                            {
                                object argIndex2 = i;
                                fname = withBlock1.SkillName(ref argIndex2);
                                break;
                            }
                    }

                    // 非表示の能力は除く
                    if (Strings.InStr(fname, "非表示") > 0)
                    {
                        goto NextSkill;
                    }

                    // 既に表示されていればスキップ
                    var loopTo1 = (short)Information.UBound(list);
                    for (j = 1; j <= loopTo1; j++)
                    {
                        if ((list[j] ?? "") == (fname ?? ""))
                        {
                            goto NextSkill;
                        }
                    }

                    // リストに追加
                    Array.Resize(ref list, Information.UBound(list) + 1 + 1);
                    Array.Resize(ref id_list, Information.UBound(list) + 1);
                    list[Information.UBound(list)] = fname;
                    id_list[Information.UBound(list)] = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(i);
                    NextSkill:
                    ;
                }
            }

            {
                var withBlock2 = SelectedUnit;
                // 付加・強化されたパイロット用特殊能力
                var loopTo2 = withBlock2.CountCondition();
                for (i = 1; i <= loopTo2; i++)
                {
                    // パイロット能力付加または強化？
                    string localCondition() { object argIndex1 = i; var ret = withBlock2.Condition(ref argIndex1); return ret; }

                    string localCondition1() { object argIndex1 = i; var ret = withBlock2.Condition(ref argIndex1); return ret; }

                    if (Strings.Right(localCondition(), 3) != "付加２" & Strings.Right(localCondition1(), 3) != "強化２")
                    {
                        goto NextSkill2;
                    }

                    string localCondition2() { object argIndex1 = i; var ret = withBlock2.Condition(ref argIndex1); return ret; }

                    string localCondition3() { object argIndex1 = i; var ret = withBlock2.Condition(ref argIndex1); return ret; }

                    ftype = Strings.Left(localCondition2(), Strings.Len(localCondition3()) - 3);

                    // 非表示の能力？
                    string localConditionData() { object argIndex1 = i; var ret = withBlock2.ConditionData(ref argIndex1); return ret; }

                    string arglist = localConditionData();
                    switch (GeneralLib.LIndex(ref arglist, 1) ?? "")
                    {
                        case "非表示":
                        case "解説":
                            {
                                goto NextSkill2;
                                break;
                            }
                    }

                    // 有効時間が残っている？
                    object argIndex4 = i;
                    if (withBlock2.ConditionLifetime(ref argIndex4) == 0)
                    {
                        goto NextSkill2;
                    }

                    // 表示名称
                    object argIndex5 = ftype;
                    fname = withBlock2.MainPilot().SkillName(ref argIndex5);
                    if (Strings.InStr(fname, "非表示") > 0)
                    {
                        goto NextSkill2;
                    }

                    // 既に表示していればスキップ
                    var loopTo3 = (short)Information.UBound(list);
                    for (j = 1; j <= loopTo3; j++)
                    {
                        if ((list[j] ?? "") == (fname ?? ""))
                        {
                            goto NextSkill2;
                        }
                    }

                    // リストに追加
                    Array.Resize(ref list, Information.UBound(list) + 1 + 1);
                    Array.Resize(ref id_list, Information.UBound(list) + 1);
                    list[Information.UBound(list)] = fname;
                    id_list[Information.UBound(list)] = ftype;
                    NextSkill2:
                    ;
                }

                Array.Resize(ref is_unit_feature, Information.UBound(list) + 1);

                // ユニット用特殊能力
                // 付加された特殊能力より先に固有の特殊能力を表示
                if (withBlock2.CountAllFeature() > withBlock2.AdditionalFeaturesNum)
                {
                    i = (short)(withBlock2.AdditionalFeaturesNum + 1);
                }
                else
                {
                    i = 1;
                }

                while (i <= withBlock2.CountAllFeature())
                {
                    // 非表示の特殊能力を排除
                    object argIndex6 = i;
                    if (string.IsNullOrEmpty(withBlock2.AllFeatureName(ref argIndex6)))
                    {
                        goto NextFeature;
                    }

                    // 合体の場合は合体後の形態が作成されていなければならない
                    string localAllFeature() { object argIndex1 = i; var ret = withBlock2.AllFeature(ref argIndex1); return ret; }

                    string localAllFeatureData() { object argIndex1 = i; var ret = withBlock2.AllFeatureData(ref argIndex1); return ret; }

                    string localLIndex() { string arglist = hs7afb75fef08b43c283a05523ef7388cb(); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

                    bool localIsDefined() { object argIndex1 = (object)hse6256782c58b487b8147a3f247066e6f(); var ret = SRC.UList.IsDefined(ref argIndex1); return ret; }

                    if (localAllFeature() == "合体" & !localIsDefined())
                    {
                        goto NextFeature;
                    }

                    // 既に表示していればスキップ
                    var loopTo4 = (short)Information.UBound(list);
                    for (j = 1; j <= loopTo4; j++)
                    {
                        string localAllFeatureName() { object argIndex1 = i; var ret = withBlock2.AllFeatureName(ref argIndex1); return ret; }

                        if ((list[j] ?? "") == (localAllFeatureName() ?? ""))
                        {
                            goto NextFeature;
                        }
                    }

                    // リストに追加
                    Array.Resize(ref list, Information.UBound(list) + 1 + 1);
                    Array.Resize(ref id_list, Information.UBound(list) + 1);
                    Array.Resize(ref is_unit_feature, Information.UBound(list) + 1);
                    object argIndex7 = i;
                    list[Information.UBound(list)] = withBlock2.AllFeatureName(ref argIndex7);
                    id_list[Information.UBound(list)] = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(i);
                    is_unit_feature[Information.UBound(list)] = true;
                    NextFeature:
                    ;
                    if (i == withBlock2.AdditionalFeaturesNum)
                    {
                        break;
                    }
                    else if (i == withBlock2.CountFeature())
                    {
                        // 付加された特殊能力は後から表示
                        if (withBlock2.AdditionalFeaturesNum > 0)
                        {
                            i = 0;
                        }
                    }

                    i = (short)(i + 1);
                }

                // アビリティで付加・強化されたパイロット用特殊能力
                var loopTo5 = withBlock2.CountCondition();
                for (i = 1; i <= loopTo5; i++)
                {
                    // パイロット能力付加または強化？
                    string localCondition4() { object argIndex1 = i; var ret = withBlock2.Condition(ref argIndex1); return ret; }

                    string localCondition5() { object argIndex1 = i; var ret = withBlock2.Condition(ref argIndex1); return ret; }

                    if (Strings.Right(localCondition4(), 2) != "付加" & Strings.Right(localCondition5(), 2) != "強化")
                    {
                        goto NextSkill3;
                    }

                    string localCondition6() { object argIndex1 = i; var ret = withBlock2.Condition(ref argIndex1); return ret; }

                    string localCondition7() { object argIndex1 = i; var ret = withBlock2.Condition(ref argIndex1); return ret; }

                    ftype = Strings.Left(localCondition6(), Strings.Len(localCondition7()) - 2);

                    // 非表示の能力？
                    if (ftype == "メッセージ")
                    {
                        goto NextSkill3;
                    }

                    string localConditionData1() { object argIndex1 = i; var ret = withBlock2.ConditionData(ref argIndex1); return ret; }

                    string arglist1 = localConditionData1();
                    switch (GeneralLib.LIndex(ref arglist1, 1) ?? "")
                    {
                        case "非表示":
                        case "解説":
                            {
                                goto NextSkill3;
                                break;
                            }
                    }

                    // 有効時間が残っている？
                    object argIndex8 = i;
                    if (withBlock2.ConditionLifetime(ref argIndex8) == 0)
                    {
                        goto NextSkill3;
                    }

                    // 表示名称
                    object argIndex9 = ftype;
                    if (string.IsNullOrEmpty(withBlock2.FeatureName0(ref argIndex9)))
                    {
                        goto NextSkill3;
                    }

                    object argIndex10 = ftype;
                    fname = withBlock2.MainPilot().SkillName0(ref argIndex10);
                    if (Strings.InStr(fname, "非表示") > 0)
                    {
                        goto NextSkill3;
                    }

                    // 付加されたユニット用特殊能力として既に表示していればスキップ
                    var loopTo6 = (short)Information.UBound(list);
                    for (j = 1; j <= loopTo6; j++)
                    {
                        if ((list[j] ?? "") == (fname ?? ""))
                        {
                            goto NextSkill3;
                        }
                    }

                    object argIndex11 = ftype;
                    fname = withBlock2.MainPilot().SkillName(ref argIndex11);
                    if (Strings.InStr(fname, "Lv") > 0)
                    {
                        fname0 = Strings.Left(fname, Strings.InStr(fname, "Lv") - 1);
                    }
                    else
                    {
                        fname0 = fname;
                    }

                    // パイロット用特殊能力として既に表示していればスキップ
                    var loopTo7 = (short)Information.UBound(list);
                    for (j = 1; j <= loopTo7; j++)
                    {
                        if ((list[j] ?? "") == (fname ?? "") | (list[j] ?? "") == (fname0 ?? ""))
                        {
                            goto NextSkill3;
                        }
                    }

                    // リストに追加
                    Array.Resize(ref list, Information.UBound(list) + 1 + 1);
                    Array.Resize(ref id_list, Information.UBound(list) + 1);
                    Array.Resize(ref is_unit_feature, Information.UBound(list) + 1);
                    list[Information.UBound(list)] = fname;
                    id_list[Information.UBound(list)] = ftype;
                    is_unit_feature[Information.UBound(list)] = false;
                    NextSkill3:
                    ;
                }
            }

            GUI.ListItemFlag = new bool[Information.UBound(list) + 1];
            switch (Information.UBound(list))
            {
                case 0:
                    {
                        break;
                    }

                case 1:
                    {
                        if (SRC.AutoMoveCursor)
                        {
                            GUI.SaveCursorPos();
                        }

                        if (id_list[ret] == "武器・防具クラス")
                        {
                            Help.FeatureHelp(ref SelectedUnit, id_list[1], false);
                        }
                        else if (is_unit_feature[1])
                        {
                            Help.FeatureHelp(ref SelectedUnit, id_list[1], GeneralLib.StrToLng(ref id_list[1]) <= SelectedUnit.AdditionalFeaturesNum);
                        }
                        else
                        {
                            Help.SkillHelp(ref SelectedUnit.MainPilot(), ref id_list[1]);
                        }

                        if (SRC.AutoMoveCursor)
                        {
                            GUI.RestoreCursorPos();
                        }

                        break;
                    }

                default:
                    {
                        GUI.TopItem = 1;
                        string arglb_caption = "特殊能力一覧";
                        string arglb_info = "能力名";
                        string arglb_mode = "表示のみ";
                        ret = GUI.ListBox(ref arglb_caption, ref list, ref arglb_info, ref arglb_mode);
                        if (SRC.AutoMoveCursor)
                        {
                            string argcursor_mode = "ダイアログ";
                            GUI.MoveCursorPos(ref argcursor_mode);
                        }

                        while (true)
                        {
                            string arglb_caption1 = "特殊能力一覧";
                            string arglb_info1 = "能力名";
                            string arglb_mode1 = "連続表示";
                            ret = GUI.ListBox(ref arglb_caption1, ref list, ref arglb_info1, ref arglb_mode1);
                            // listが一定なので連続表示を流用
                            My.MyProject.Forms.frmListBox.Hide();
                            if (ret == 0)
                            {
                                break;
                            }

                            if (id_list[ret] == "武器・防具クラス")
                            {
                                Help.FeatureHelp(ref SelectedUnit, id_list[ret], false);
                            }
                            else if (is_unit_feature[ret])
                            {
                                Help.FeatureHelp(ref SelectedUnit, id_list[ret], Conversions.ToDouble(id_list[ret]) <= SelectedUnit.AdditionalFeaturesNum);
                            }
                            else
                            {
                                Help.SkillHelp(ref SelectedUnit.MainPilot(), ref id_list[ret]);
                            }
                        }

                        if (SRC.AutoMoveCursor)
                        {
                            GUI.RestoreCursorPos();
                        }

                        break;
                    }
            }

            CommandState = "ユニット選択";
            GUI.UnlockGUI();
        }

        // 「武器一覧」コマンド
        // MOD START MARGE
        // Public Sub WeaponListCommand()
        private static void WeaponListCommand()
        {
            // MOD END MARGE
            string[] list;
            short i;
            string buf;
            short w;
            string wclass;
            string atype, alevel;
            string c;
            GUI.LockGUI();
            short min_range, max_range;
            while (true)
            {
                string argcaption_msg = "武装一覧";
                string arglb_mode = "一覧";
                string argBGM = "";
                w = GUI.WeaponListBox(ref SelectedUnit, ref argcaption_msg, ref arglb_mode, BGM: ref argBGM);
                SelectedWeapon = w;
                if (SelectedWeapon <= 0)
                {
                    // キャンセル
                    if (SRC.AutoMoveCursor)
                    {
                        GUI.RestoreCursorPos();
                    }

                    My.MyProject.Forms.frmListBox.Hide();
                    GUI.UnlockGUI();
                    CommandState = "ユニット選択";
                    return;
                }

                // 指定された武器の属性一覧を作成
                list = new string[1];
                i = 0;
                {
                    var withBlock = SelectedUnit;
                    wclass = withBlock.WeaponClass(w);
                    while (i <= Strings.Len(wclass))
                    {
                        i = (short)(i + 1);
                        buf = GeneralLib.GetClassBundle(ref wclass, ref i);
                        atype = "";
                        alevel = "";

                        // 非表示？
                        if (buf == "|")
                        {
                            break;
                        }

                        // Ｍ属性
                        if (Strings.Mid(wclass, i, 1) == "Ｍ")
                        {
                            i = (short)(i + 1);
                            buf = buf + Strings.Mid(wclass, i, 1);
                        }

                        // レベル指定
                        if (Strings.Mid(wclass, i + 1, 1) == "L")
                        {
                            i = (short)(i + 2);
                            c = Strings.Mid(wclass, i, 1);
                            while (Information.IsNumeric(c) | c == "." | c == "-")
                            {
                                alevel = alevel + c;
                                i = (short)(i + 1);
                                c = Strings.Mid(wclass, i, 1);
                            }

                            i = (short)(i - 1);
                        }

                        // 属性の名称
                        atype = Help.AttributeName(ref SelectedUnit, ref buf);
                        if (Strings.Len(atype) > 0)
                        {
                            Array.Resize(ref list, Information.UBound(list) + 1 + 1);
                            if (Strings.Len(alevel) > 0)
                            {
                                string localRightPaddedString() { string argbuf = buf + "L" + alevel; var ret = GeneralLib.RightPaddedString(ref argbuf, 8); return ret; }

                                list[Information.UBound(list)] = localRightPaddedString() + atype + "レベル" + Strings.StrConv(alevel, VbStrConv.Wide);
                            }
                            else
                            {
                                list[Information.UBound(list)] = GeneralLib.RightPaddedString(ref buf, 8) + atype;
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(Map.MapFileName))
                    {
                        Array.Resize(ref list, Information.UBound(list) + 1 + 1);
                        list[Information.UBound(list)] = "射程範囲";
                    }

                    if (Information.UBound(list) > 0)
                    {
                        GUI.TopItem = 1;
                        while (true)
                        {
                            if (Information.UBound(list) == 1 & list[1] == "射程範囲")
                            {
                                i = 1;
                            }
                            else
                            {
                                GUI.ListItemFlag = new bool[Information.UBound(list) + 1];
                                string arglb_caption = "武器属性一覧";
                                string arglb_info = "属性    効果";
                                string arglb_mode1 = "連続表示";
                                i = GUI.ListBox(ref arglb_caption, ref list, ref arglb_info, ref arglb_mode1);
                            }

                            if (i == 0)
                            {
                                // キャンセル
                                break;
                            }
                            else if (list[i] == "射程範囲")
                            {
                                My.MyProject.Forms.frmListBox.Hide();

                                // 武器の射程を求めておく
                                min_range = withBlock.Weapon(w).MinRange;
                                max_range = withBlock.WeaponMaxRange(w);

                                // 射程範囲表示
                                string argattr3 = "Ｐ";
                                string argattr4 = "Ｑ";
                                string argattr5 = "Ｍ直";
                                string argattr6 = "Ｍ拡";
                                string argattr7 = "Ｍ扇";
                                string argattr8 = "Ｍ全";
                                string argattr9 = "Ｍ線";
                                string argattr10 = "Ｍ投";
                                string argattr11 = "Ｍ移";
                                if ((max_range == 1 | withBlock.IsWeaponClassifiedAs(w, ref argattr3)) & !withBlock.IsWeaponClassifiedAs(w, ref argattr4))
                                {
                                    string arguparty = withBlock.Party + "の敵";
                                    Map.AreaInReachable(ref SelectedUnit, max_range, ref arguparty);
                                }
                                else if (withBlock.IsWeaponClassifiedAs(w, ref argattr5))
                                {
                                    Map.AreaInCross(withBlock.x, withBlock.y, min_range, ref max_range);
                                }
                                else if (withBlock.IsWeaponClassifiedAs(w, ref argattr6))
                                {
                                    Map.AreaInWideCross(withBlock.x, withBlock.y, min_range, ref max_range);
                                }
                                else if (withBlock.IsWeaponClassifiedAs(w, ref argattr7))
                                {
                                    string argattr = "Ｍ扇";
                                    Map.AreaInSectorCross(withBlock.x, withBlock.y, min_range, ref max_range, (short)withBlock.WeaponLevel(w, ref argattr));
                                }
                                else if (withBlock.IsWeaponClassifiedAs(w, ref argattr8) | withBlock.IsWeaponClassifiedAs(w, ref argattr9))
                                {
                                    string arguparty2 = "すべて";
                                    Map.AreaInRange(withBlock.x, withBlock.y, max_range, min_range, ref arguparty2);
                                }
                                else if (withBlock.IsWeaponClassifiedAs(w, ref argattr10))
                                {
                                    string argattr1 = "Ｍ投";
                                    max_range = (short)(max_range + withBlock.WeaponLevel(w, ref argattr1));
                                    string argattr2 = "Ｍ投";
                                    min_range = (short)(min_range - withBlock.WeaponLevel(w, ref argattr2));
                                    min_range = (short)GeneralLib.MaxLng(min_range, 1);
                                    string arguparty3 = "すべて";
                                    Map.AreaInRange(withBlock.x, withBlock.y, max_range, min_range, ref arguparty3);
                                }
                                else if (withBlock.IsWeaponClassifiedAs(w, ref argattr11))
                                {
                                    Map.AreaInMoveAction(ref SelectedUnit, max_range);
                                }
                                else
                                {
                                    string arguparty1 = withBlock.Party + "の敵";
                                    Map.AreaInRange(withBlock.x, withBlock.y, max_range, min_range, ref arguparty1);
                                }

                                GUI.Center(withBlock.x, withBlock.y);
                                GUI.MaskScreen();

                                // 先行入力されていたクリックイベントを解消
                                Application.DoEvents();
                                WaitClickMode = true;
                                GUI.IsFormClicked = false;

                                // クリックされるまで待つ
                                while (!GUI.IsFormClicked)
                                {
                                    GUI.Sleep(25);
                                    Application.DoEvents();
                                    if (GUI.IsRButtonPressed(true))
                                    {
                                        break;
                                    }
                                }

                                GUI.RedrawScreen();
                                if (Information.UBound(list) == 1 & list[i] == "射程範囲")
                                {
                                    break;
                                }
                            }
                            else
                            {
                                // 指定された属性の解説を表示
                                My.MyProject.Forms.frmListBox.Hide();
                                string argatr = GeneralLib.LIndex(ref list[i], 1);
                                Help.AttributeHelp(ref SelectedUnit, ref argatr, w);
                            }
                        }
                    }
                }
            }
        }

        // 「アビリティ一覧」コマンド
        // MOD START MARGE
        // Public Sub AbilityListCommand()
        private static void AbilityListCommand()
        {
            // MOD END MARGE
            string[] list;
            short i;
            string buf;
            short a;
            string alevel, atype, aclass;
            string c;
            GUI.LockGUI();
            short min_range, max_range;
            while (true)
            {
                string argtname = "アビリティ";
                string argcaption_msg = Expression.Term(ref argtname, ref SelectedUnit) + "一覧";
                string arglb_mode = "一覧";
                a = GUI.AbilityListBox(ref SelectedUnit, ref argcaption_msg, ref arglb_mode);
                SelectedAbility = a;
                if (SelectedAbility <= 0)
                {
                    // キャンセル
                    if (SRC.AutoMoveCursor)
                    {
                        GUI.RestoreCursorPos();
                    }

                    My.MyProject.Forms.frmListBox.Hide();
                    GUI.UnlockGUI();
                    CommandState = "ユニット選択";
                    return;
                }

                // 指定されたアビリティの属性一覧を作成
                list = new string[1];
                i = 0;
                {
                    var withBlock = SelectedUnit;
                    aclass = withBlock.Ability(a).Class_Renamed;
                    while (i <= Strings.Len(aclass))
                    {
                        i = (short)(i + 1);
                        buf = GeneralLib.GetClassBundle(ref aclass, ref i);
                        atype = "";
                        alevel = "";

                        // 非表示？
                        if (buf == "|")
                        {
                            break;
                        }

                        // Ｍ属性
                        if (Strings.Mid(aclass, i, 1) == "Ｍ")
                        {
                            i = (short)(i + 1);
                            buf = buf + Strings.Mid(aclass, i, 1);
                        }

                        // レベル指定
                        if (Strings.Mid(aclass, i + 1, 1) == "L")
                        {
                            i = (short)(i + 2);
                            c = Strings.Mid(aclass, i, 1);
                            while (Information.IsNumeric(c) | c == "." | c == "-")
                            {
                                alevel = alevel + c;
                                i = (short)(i + 1);
                                c = Strings.Mid(aclass, i, 1);
                            }

                            i = (short)(i - 1);
                        }

                        // 属性の名称
                        atype = Help.AttributeName(ref SelectedUnit, ref buf, true);
                        if (Strings.Len(atype) > 0)
                        {
                            Array.Resize(ref list, Information.UBound(list) + 1 + 1);
                            if (Strings.Len(alevel) > 0)
                            {
                                string localRightPaddedString() { string argbuf = buf + "L" + alevel; var ret = GeneralLib.RightPaddedString(ref argbuf, 8); return ret; }

                                list[Information.UBound(list)] = localRightPaddedString() + atype + "レベル" + Strings.StrConv(alevel, VbStrConv.Wide);
                            }
                            else
                            {
                                list[Information.UBound(list)] = GeneralLib.RightPaddedString(ref buf, 8) + atype;
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(Map.MapFileName))
                    {
                        Array.Resize(ref list, Information.UBound(list) + 1 + 1);
                        list[Information.UBound(list)] = "射程範囲";
                    }

                    if (Information.UBound(list) > 0)
                    {
                        GUI.TopItem = 1;
                        while (true)
                        {
                            if (Information.UBound(list) == 1 & list[1] == "射程範囲")
                            {
                                i = 1;
                            }
                            else
                            {
                                GUI.ListItemFlag = new bool[Information.UBound(list) + 1];
                                string arglb_caption = "アビリティ属性一覧";
                                string arglb_info = "属性    効果";
                                string arglb_mode1 = "連続表示";
                                i = GUI.ListBox(ref arglb_caption, ref list, ref arglb_info, ref arglb_mode1);
                            }

                            if (i == 0)
                            {
                                // キャンセル
                                break;
                            }
                            else if (list[i] == "射程範囲")
                            {
                                My.MyProject.Forms.frmListBox.Hide();

                                // アビリティの射程を求めておく
                                min_range = withBlock.AbilityMinRange(a);
                                max_range = withBlock.AbilityMaxRange(a);

                                // 射程範囲表示
                                string argattr3 = "Ｐ";
                                string argattr4 = "Ｑ";
                                string argattr5 = "Ｍ直";
                                string argattr6 = "Ｍ拡";
                                string argattr7 = "Ｍ扇";
                                string argattr8 = "Ｍ投";
                                string argattr9 = "Ｍ移";
                                if ((max_range == 1 | withBlock.IsAbilityClassifiedAs(a, ref argattr3)) & !withBlock.IsAbilityClassifiedAs(a, ref argattr4))
                                {
                                    string arguparty = "すべて";
                                    Map.AreaInReachable(ref SelectedUnit, max_range, ref arguparty);
                                }
                                else if (withBlock.IsAbilityClassifiedAs(a, ref argattr5))
                                {
                                    Map.AreaInCross(withBlock.x, withBlock.y, min_range, ref max_range);
                                }
                                else if (withBlock.IsAbilityClassifiedAs(a, ref argattr6))
                                {
                                    Map.AreaInWideCross(withBlock.x, withBlock.y, min_range, ref max_range);
                                }
                                else if (withBlock.IsAbilityClassifiedAs(a, ref argattr7))
                                {
                                    string argattr = "Ｍ扇";
                                    Map.AreaInSectorCross(withBlock.x, withBlock.y, min_range, ref max_range, (short)withBlock.AbilityLevel(a, ref argattr));
                                }
                                else if (withBlock.IsAbilityClassifiedAs(a, ref argattr8))
                                {
                                    string argattr1 = "Ｍ投";
                                    max_range = (short)(max_range + withBlock.AbilityLevel(a, ref argattr1));
                                    string argattr2 = "Ｍ投";
                                    min_range = (short)(min_range - withBlock.AbilityLevel(a, ref argattr2));
                                    min_range = (short)GeneralLib.MaxLng(min_range, 1);
                                    string arguparty2 = "すべて";
                                    Map.AreaInRange(withBlock.x, withBlock.y, max_range, min_range, ref arguparty2);
                                }
                                else if (withBlock.IsAbilityClassifiedAs(a, ref argattr9))
                                {
                                    Map.AreaInMoveAction(ref SelectedUnit, max_range);
                                }
                                else
                                {
                                    string arguparty1 = "すべて";
                                    Map.AreaInRange(withBlock.x, withBlock.y, max_range, min_range, ref arguparty1);
                                }

                                GUI.Center(withBlock.x, withBlock.y);
                                GUI.MaskScreen();

                                // 先行入力されていたクリックイベントを解消
                                Application.DoEvents();
                                WaitClickMode = true;
                                GUI.IsFormClicked = false;

                                // クリックされるまで待つ
                                while (!GUI.IsFormClicked)
                                {
                                    GUI.Sleep(25);
                                    Application.DoEvents();
                                    if (GUI.IsRButtonPressed(true))
                                    {
                                        break;
                                    }
                                }

                                GUI.RedrawScreen();
                                if (Information.UBound(list) == 1 & list[i] == "射程範囲")
                                {
                                    break;
                                }
                            }
                            else
                            {
                                // 指定された属性の解説を表示
                                My.MyProject.Forms.frmListBox.Hide();
                                string argatr = GeneralLib.LIndex(ref list[i], 1);
                                Help.AttributeHelp(ref SelectedUnit, ref argatr, a, true);
                            }
                        }
                    }
                }
            }
        }

        // 「移動範囲」コマンド
        // MOD START MARGE
        // Public Sub ShowAreaInSpeedCommand()
        private static void ShowAreaInSpeedCommand()
        {
            // MOD END MARGE
            SelectedCommand = "移動範囲";
            // MOD START MARGE
            // If MainWidth <> 15 Then
            if (GUI.NewGUIMode)
            {
                // MOD END MARGE
                Status.ClearUnitStatus();
            }

            Map.AreaInSpeed(ref SelectedUnit);
            GUI.Center(SelectedUnit.x, SelectedUnit.y);
            GUI.MaskScreen();
            CommandState = "ターゲット選択";
        }

        // 「射程範囲」コマンド
        // MOD START MARGE
        // Public Sub ShowAreaInRangeCommand()
        private static void ShowAreaInRangeCommand()
        {
            // MOD END MARGE
            short w, i, max_range;
            SelectedCommand = "射程範囲";

            // MOD START MARGE
            // If MainWidth <> 15 Then
            if (GUI.NewGUIMode)
            {
                // MOD END MARGE
                Status.ClearUnitStatus();
            }

            {
                var withBlock = SelectedUnit;
                // 最大の射程を持つ武器を探す
                w = 0;
                max_range = 0;
                var loopTo = withBlock.CountWeapon();
                for (i = 1; i <= loopTo; i++)
                {
                    string argref_mode = "ステータス";
                    string argattr = "Ｍ";
                    if (withBlock.IsWeaponAvailable(i, ref argref_mode) & !withBlock.IsWeaponClassifiedAs(i, ref argattr))
                    {
                        if (withBlock.WeaponMaxRange(i) > max_range)
                        {
                            w = i;
                            max_range = withBlock.WeaponMaxRange(i);
                        }
                    }
                }

                // 見つかった最大の射程を持つ武器の射程範囲を選択
                string arguparty = withBlock.Party + "の敵";
                Map.AreaInRange(withBlock.x, withBlock.y, max_range, 1, ref arguparty);

                // 射程範囲を表示
                GUI.Center(withBlock.x, withBlock.y);
                GUI.MaskScreen();
            }

            CommandState = "ターゲット選択";
        }

        // 「待機」コマンド
        // 他のコマンドの終了処理にも使われる
        // MOD START MARGE
        // Public Sub WaitCommand(Optional ByVal WithoutAction As Boolean)
        // 今後どうしてもPrivateじゃダメな処理が出たら戻してください
        private static void WaitCommand(bool WithoutAction = false)
        {
            // MOD END MARGE
            Pilot p;
            short i;

            // コマンド終了時はターゲットを解除
            // UPGRADE_NOTE: オブジェクト SelectedTarget をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            SelectedTarget = null;

            // ユニットにパイロットが乗っていない？
            if (SelectedUnit.CountPilot() == 0)
            {
                CommandState = "ユニット選択";
                GUI.RedrawScreen();
                Status.ClearUnitStatus();
                return;
            }

            if (!WithoutAction)
            {
                // 残り行動数を減少させる
                SelectedUnit.UseAction();

                // 持続期間が「移動」のスペシャルパワー効果を削除
                if (Strings.InStr(CommandState, "移動後") > 0)
                {
                    string argstype = "移動";
                    SelectedUnit.RemoveSpecialPowerInEffect(ref argstype);
                }
            }

            CommandState = "ユニット選択";

            // アップデート
            SelectedUnit.Update();
            SRC.PList.UpdateSupportMod(SelectedUnit);

            // ユニットが既に出撃していない？
            if (SelectedUnit.Status_Renamed != "出撃")
            {
                GUI.RedrawScreen();
                Status.ClearUnitStatus();
                return;
            }

            GUI.LockGUI();
            GUI.RedrawScreen();
            object argIndex1 = 1;
            p = SelectedUnit.Pilot(ref argIndex1);

            // 接触イベント
            for (i = 1; i <= 4; i++)
            {
                // UPGRADE_NOTE: オブジェクト SelectedTarget をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                SelectedTarget = null;
                {
                    var withBlock = SelectedUnit;
                    switch (i)
                    {
                        case 1:
                            {
                                if (withBlock.x > 1)
                                {
                                    SelectedTarget = Map.MapDataForUnit[withBlock.x - 1, withBlock.y];
                                }

                                break;
                            }

                        case 2:
                            {
                                if (withBlock.x < Map.MapWidth)
                                {
                                    SelectedTarget = Map.MapDataForUnit[withBlock.x + 1, withBlock.y];
                                }

                                break;
                            }

                        case 3:
                            {
                                if (withBlock.y > 1)
                                {
                                    SelectedTarget = Map.MapDataForUnit[withBlock.x, withBlock.y - 1];
                                }

                                break;
                            }

                        case 4:
                            {
                                if (withBlock.y < Map.MapHeight)
                                {
                                    SelectedTarget = Map.MapDataForUnit[withBlock.x, withBlock.y + 1];
                                }

                                break;
                            }
                    }
                }

                if (SelectedTarget is object)
                {
                    Event_Renamed.HandleEvent("接触", SelectedUnit.MainPilot().ID, SelectedTarget.MainPilot().ID);
                    // UPGRADE_NOTE: オブジェクト SelectedTarget をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                    SelectedTarget = null;
                    if (SRC.IsScenarioFinished)
                    {
                        SRC.IsScenarioFinished = false;
                        return;
                    }

                    if (SelectedUnit.Status_Renamed != "出撃")
                    {
                        GUI.RedrawScreen();
                        Status.ClearUnitStatus();
                        GUI.UnlockGUI();
                        return;
                    }
                }
            }

            // 進入イベント
            Event_Renamed.HandleEvent("進入", SelectedUnit.MainPilot().ID, SelectedUnit.x, SelectedUnit.y);
            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
                return;
            }

            if (SelectedUnit.CountPilot() == 0)
            {
                GUI.RedrawScreen();
                Status.ClearUnitStatus();
                GUI.UnlockGUI();
                return;
            }

            // 行動終了イベント
            Event_Renamed.HandleEvent("行動終了", SelectedUnit.MainPilot().ID);
            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
                return;
            }

            if (SelectedUnit.CountPilot() == 0)
            {
                GUI.RedrawScreen();
                Status.ClearUnitStatus();
                GUI.UnlockGUI();
                return;
            }

            if (p.Unit_Renamed is object)
            {
                SelectedUnit = p.Unit_Renamed;
            }

            if (SelectedUnit.Action > 0 & SelectedUnit.CountPilot() > 0)
            {
                // カーソル自動移動
                if (SRC.AutoMoveCursor)
                {
                    string argcursor_mode = "ユニット選択";
                    GUI.MoveCursorPos(ref argcursor_mode, SelectedUnit);
                }
            }

            // ハイパーモード・ノーマルモードの自動発動をチェック
            SelectedUnit.CurrentForm().CheckAutoHyperMode();
            SelectedUnit.CurrentForm().CheckAutoNormalMode();
            if (GUI.IsPictureVisible | GUI.IsCursorVisible)
            {
                GUI.RedrawScreen();
            }

            GUI.UnlockGUI();

            // ステータスウィンドウの表示内容を更新
            if (SelectedUnit.Status_Renamed == "出撃" & GUI.MainWidth == 15)
            {
                Status.DisplayUnitStatus(ref SelectedUnit);
            }
            else
            {
                Status.ClearUnitStatus();
            }
        }


        // マップコマンド実行
        public static void MapCommand(short idx)
        {
            CommandState = "ユニット選択";
            switch (idx)
            {
                case EndTurnCmdID: // ターン終了
                    {
                        if (ViewMode)
                        {
                            ViewMode = false;
                            return;
                        }

                        EndTurnCommand();
                        break;
                    }

                case DumpCmdID: // 中断
                    {
                        DumpCommand();
                        break;
                    }

                case UnitListCmdID: // 部隊表
                    {
                        UnitListCommand();
                        break;
                    }

                case SearchSpecialPowerCmdID: // スペシャルパワー検索
                    {
                        SearchSpecialPowerCommand();
                        break;
                    }

                case GlobalMapCmdID: // 全体マップ
                    {
                        GlobalMapCommand();
                        break;
                    }

                case OperationObjectCmdID: // 作戦目的
                    {
                        GUI.LockGUI();
                        Event_Renamed.HandleEvent("勝利条件");
                        GUI.RedrawScreen();
                        GUI.UnlockGUI();
                        break;
                    }

                case var @case when MapCommand1CmdID <= @case && @case <= MapCommand10CmdID: // マップコマンド
                    {
                        GUI.LockGUI();
                        Event_Renamed.HandleEvent(MapCommandLabelList[idx - MapCommand1CmdID + 1]);
                        GUI.UnlockGUI();
                        break;
                    }

                case AutoDefenseCmdID: // 自動反撃モード
                    {
                        // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                        GUI.MainForm.mnuMapCommandItem(AutoDefenseCmdID).Checked = !GUI.MainForm.mnuMapCommandItem(AutoDefenseCmdID).Checked;
                        // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                        if (GUI.MainForm.mnuMapCommandItem(AutoDefenseCmdID).Checked)
                        {
                            string argini_section = "Option";
                            string argini_entry = "AutoDefense";
                            string argini_data = "On";
                            GeneralLib.WriteIni(ref argini_section, ref argini_entry, ref argini_data);
                        }
                        else
                        {
                            string argini_section1 = "Option";
                            string argini_entry1 = "AutoDefense";
                            string argini_data1 = "Off";
                            GeneralLib.WriteIni(ref argini_section1, ref argini_entry1, ref argini_data1);
                        }

                        break;
                    }

                case ConfigurationCmdID: // 設定変更
                    {
                        // UPGRADE_ISSUE: Load ステートメント はサポートされていません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B530EFF2-3132-48F8-B8BC-D88AF543D321"' をクリックしてください。
                        Load(My.MyProject.Forms.frmConfiguration);
                        My.MyProject.Forms.frmConfiguration.Left = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsX((Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(Screen.PrimaryScreen.Bounds.Width) - Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(My.MyProject.Forms.frmConfiguration.Width)) / 2d);
                        My.MyProject.Forms.frmConfiguration.Top = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsY((Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(Screen.PrimaryScreen.Bounds.Height) - Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(My.MyProject.Forms.frmConfiguration.Height)) / 3d);
                        My.MyProject.Forms.frmConfiguration.ShowDialog();
                        My.MyProject.Forms.frmConfiguration.Close();
                        // UPGRADE_NOTE: オブジェクト frmConfiguration をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                        My.MyProject.Forms.frmConfiguration = null;
                        break;
                    }

                case RestartCmdID: // リスタート
                    {
                        RestartCommand();
                        break;
                    }

                case QuickLoadCmdID: // クイックロード
                    {
                        QuickLoadCommand();
                        break;
                    }

                case QuickSaveCmdID: // クイックセーブ
                    {
                        QuickSaveCommand();
                        break;
                    }
            }

            SRC.IsScenarioFinished = false;
        }

        // 「ターン終了」コマンド
        // MOD START MARGE
        // Public Sub EndTurnCommand()
        private static void EndTurnCommand()
        {
            // MOD END MARGE
            short num;
            short ret;

            // 行動していない味方ユニットの数を数える
            num = 0;
            foreach (Unit u in SRC.UList)
            {
                if (u.Party == "味方" & (u.Status_Renamed == "出撃" | u.Status_Renamed == "格納") & u.Action > 0)
                {
                    num = (short)(num + 1);
                }
            }

            // 行動していないユニットがいれば警告
            if (num > 0)
            {
                ret = (short)Interaction.MsgBox("行動していないユニットが" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(num) + "体あります" + Constants.vbCr + "このターンを終了しますか？", (MsgBoxStyle)((int)MsgBoxStyle.OkCancel + (int)MsgBoxStyle.Question), "終了");
            }
            else
            {
                ret = 0;
            }

            switch (ret)
            {
                case 1:
                    {
                        break;
                    }

                case 2:
                    {
                        return;
                    }
            }

            // 行動終了していないユニットに対して行動終了イベントを実施
            foreach (Unit currentSelectedUnit in SRC.UList)
            {
                SelectedUnit = currentSelectedUnit;
                {
                    var withBlock = SelectedUnit;
                    if (withBlock.Party == "味方" & (withBlock.Status_Renamed == "出撃" | withBlock.Status_Renamed == "格納") & withBlock.Action > 0)
                    {
                        Event_Renamed.HandleEvent("行動終了", withBlock.MainPilot().ID);
                        if (SRC.IsScenarioFinished)
                        {
                            SRC.IsScenarioFinished = false;
                            return;
                        }
                    }
                }
            }

            // 各陣営のフェイズに移行

            string arguparty = "敵";
            SRC.StartTurn(ref arguparty);
            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
                return;
            }

            string arguparty1 = "中立";
            SRC.StartTurn(ref arguparty1);
            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
                return;
            }

            string arguparty2 = "ＮＰＣ";
            SRC.StartTurn(ref arguparty2);
            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
                return;
            }

            // 味方フェイズに戻る
            string arguparty3 = "味方";
            SRC.StartTurn(ref arguparty3);
            SRC.IsScenarioFinished = false;
        }

        // ユニット一覧の表示
        // MOD START MARGE
        // Public Sub UnitListCommand()
        private static void UnitListCommand()
        {
            // MOD END MARGE
            string[] list;
            string[] id_list;
            short j, i, ret;
            string uparty, sort_mode;
            string[] mode_list;
            int[] key_list;
            string[] strkey_list;
            short max_item;
            int max_value;
            string max_str;
            string buf;
            Unit u;
            var pilot_status_mode = default(bool);
            GUI.LockGUI();
            GUI.TopItem = 1;
            GUI.EnlargeListBoxHeight();
            GUI.ReduceListBoxWidth();

            // デフォルトのソート方法
            uparty = "味方";
            sort_mode = "レベル";
            Beginning:
            ;


            // ユニット一覧のリストを作成
            list = new string[2];
            id_list = new string[2];
            list[1] = "▽陣営変更・並べ替え▽";
            foreach (Unit currentU in SRC.UList)
            {
                u = currentU;
                {
                    var withBlock = u;
                    if ((withBlock.Party0 ?? "") == (uparty ?? "") & (withBlock.Status_Renamed == "出撃" | withBlock.Status_Renamed == "格納"))
                    {
                        // 未確認のユニットは表示しない
                        string argoname = "ユニット情報隠蔽";
                        object argIndex1 = "識別済み";
                        object argIndex2 = "ユニット情報隠蔽";
                        if (Expression.IsOptionDefined(ref argoname) & !withBlock.IsConditionSatisfied(ref argIndex1) & (withBlock.Party0 == "敵" | withBlock.Party0 == "中立") | withBlock.IsConditionSatisfied(ref argIndex2))
                        {
                            goto NextUnit;
                        }

                        Array.Resize(ref list, Information.UBound(list) + 1 + 1);
                        Array.Resize(ref id_list, Information.UBound(list) + 1);
                        Array.Resize(ref GUI.ListItemFlag, Information.UBound(list) + 1);
                        string argfname = "ダミーユニット";
                        if (!withBlock.IsFeatureAvailable(ref argfname))
                        {
                            // 通常のユニット表示
                            string argoname1 = "等身大基準";
                            if (Expression.IsOptionDefined(ref argoname1))
                            {
                                // 等身大基準を使った場合のユニット表示
                                string localRightPaddedString() { string argbuf = withBlock.Nickname0; var ret = GeneralLib.RightPaddedString(ref argbuf, 33); withBlock.Nickname0 = argbuf; return ret; }

                                string localLeftPaddedString() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock.MainPilot().Level); var ret = GeneralLib.LeftPaddedString(ref argbuf, 3); return ret; }

                                list[Information.UBound(list)] = localRightPaddedString() + localLeftPaddedString() + " ";
                            }
                            else
                            {
                                string argbuf = withBlock.Nickname0;
                                list[Information.UBound(list)] = GeneralLib.RightPaddedString(ref argbuf, 23);
                                withBlock.Nickname0 = argbuf;
                                if (withBlock.MainPilot().Nickname0 == "パイロット不在")
                                {
                                    // パイロットが乗っていない場合
                                    string argbuf1 = list[Information.UBound(list)] + "";
                                    string argbuf2 = "";
                                    list[Information.UBound(list)] = GeneralLib.RightPaddedString(ref argbuf1, 34) + GeneralLib.LeftPaddedString(ref argbuf2, 2);
                                }
                                else
                                {
                                    string localLeftPaddedString1() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock.MainPilot().Level); var ret = GeneralLib.LeftPaddedString(ref argbuf, 2); return ret; }

                                    string argbuf3 = list[Information.UBound(list)] + withBlock.MainPilot().get_Nickname(false);
                                    list[Information.UBound(list)] = GeneralLib.RightPaddedString(ref argbuf3, 34) + localLeftPaddedString1();
                                }

                                list[Information.UBound(list)] = GeneralLib.RightPaddedString(ref list[Information.UBound(list)], 37);
                            }

                            object argIndex3 = "データ不明";
                            if (withBlock.IsConditionSatisfied(ref argIndex3))
                            {
                                list[Information.UBound(list)] = list[Information.UBound(list)] + "?????/????? ???/???";
                            }
                            else
                            {
                                string localLeftPaddedString2() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock.HP); var ret = GeneralLib.LeftPaddedString(ref argbuf, 5); return ret; }

                                string localLeftPaddedString3() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock.MaxHP); var ret = GeneralLib.LeftPaddedString(ref argbuf, 5); return ret; }

                                string localLeftPaddedString4() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock.EN); var ret = GeneralLib.LeftPaddedString(ref argbuf, 3); return ret; }

                                string localLeftPaddedString5() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock.MaxEN); var ret = GeneralLib.LeftPaddedString(ref argbuf, 3); return ret; }

                                list[Information.UBound(list)] = list[Information.UBound(list)] + localLeftPaddedString2() + "/" + localLeftPaddedString3() + " " + localLeftPaddedString4() + "/" + localLeftPaddedString5();
                            }
                        }
                        else
                        {
                            // パイロットステータス表示時
                            pilot_status_mode = true;
                            {
                                var withBlock1 = withBlock.MainPilot();
                                string localRightPaddedString1() { string argbuf = withBlock1.get_Nickname(false); var ret = GeneralLib.RightPaddedString(ref argbuf, 21); withBlock1.get_Nickname(false) = argbuf; return ret; }

                                string localLeftPaddedString6() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock1.Level); var ret = GeneralLib.LeftPaddedString(ref argbuf, 3); return ret; }

                                string localLeftPaddedString7() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock1.SP) + "/" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock1.MaxSP); var ret = GeneralLib.LeftPaddedString(ref argbuf, 9); return ret; }

                                list[Information.UBound(list)] = localRightPaddedString1() + localLeftPaddedString6() + localLeftPaddedString7() + "  ";
                                // 使用可能なスペシャルパワー一覧
                                var loopTo = withBlock1.CountSpecialPower;
                                for (i = 1; i <= loopTo; i++)
                                {
                                    short localSpecialPowerCost() { string argsname = withBlock1.get_SpecialPower(i); var ret = withBlock1.SpecialPowerCost(ref argsname); withBlock1.get_SpecialPower(i) = argsname; return ret; }

                                    if (withBlock1.SP >= localSpecialPowerCost())
                                    {
                                        SpecialPowerData localItem() { object argIndex1 = withBlock1.get_SpecialPower(i); var ret = SRC.SPDList.Item(ref argIndex1); withBlock1.get_SpecialPower(i) = Conversions.ToString(argIndex1); return ret; }

                                        list[Information.UBound(list)] = list[Information.UBound(list)] + localItem().ShortName;
                                    }
                                }
                            }
                        }

                        if (withBlock.Action == 0)
                        {
                            list[Information.UBound(list)] = list[Information.UBound(list)] + "済";
                        }

                        if (withBlock.Status_Renamed == "格納")
                        {
                            list[Information.UBound(list)] = list[Information.UBound(list)] + "格";
                        }

                        id_list[Information.UBound(list)] = withBlock.ID;
                        GUI.ListItemFlag[Information.UBound(list)] = false;
                    }
                }

                NextUnit:
                ;
            }

            SortList:
            ;


            // ソート
            if (Strings.InStr(sort_mode, "名称") == 0)
            {
                // 数値を使ったソート

                // まず並べ替えに使うキーのリストを作成
                key_list = new int[Information.UBound(list) + 1];
                {
                    var withBlock2 = SRC.UList;
                    switch (sort_mode ?? "")
                    {
                        case "ＨＰ":
                            {
                                var loopTo1 = (short)Information.UBound(list);
                                for (i = 2; i <= loopTo1; i++)
                                {
                                    Unit localItem1() { var tmp = id_list; object argIndex1 = tmp[i]; var ret = withBlock2.Item(ref argIndex1); return ret; }

                                    key_list[i] = localItem1().HP;
                                }

                                break;
                            }

                        case "ＥＮ":
                            {
                                var loopTo2 = (short)Information.UBound(list);
                                for (i = 2; i <= loopTo2; i++)
                                {
                                    Unit localItem2() { var tmp = id_list; object argIndex1 = tmp[i]; var ret = withBlock2.Item(ref argIndex1); return ret; }

                                    key_list[i] = localItem2().EN;
                                }

                                break;
                            }

                        case "レベル":
                        case "パイロットレベル":
                            {
                                var loopTo3 = (short)Information.UBound(list);
                                for (i = 2; i <= loopTo3; i++)
                                {
                                    var tmp = id_list;
                                    object argIndex4 = tmp[i];
                                    {
                                        var withBlock3 = withBlock2.Item(ref argIndex4);
                                        if (withBlock3.CountPilot() > 0)
                                        {
                                            {
                                                var withBlock4 = withBlock3.MainPilot();
                                                key_list[i] = 500 * withBlock4.Level + withBlock4.Exp;
                                            }
                                        }
                                    }
                                }

                                break;
                            }
                    }
                }

                // キーを使って並べ換え
                var loopTo4 = (short)(Information.UBound(list) - 1);
                for (i = 2; i <= loopTo4; i++)
                {
                    max_item = i;
                    max_value = key_list[i];
                    var loopTo5 = (short)Information.UBound(list);
                    for (j = (short)(i + 1); j <= loopTo5; j++)
                    {
                        if (key_list[j] > max_value)
                        {
                            max_item = j;
                            max_value = key_list[j];
                        }
                    }

                    if (max_item != i)
                    {
                        buf = list[i];
                        list[i] = list[max_item];
                        list[max_item] = buf;
                        buf = id_list[i];
                        id_list[i] = id_list[max_item];
                        id_list[max_item] = buf;
                        key_list[max_item] = key_list[i];
                    }
                }
            }
            else
            {
                // 文字列を使ったソート

                // まず並べ替えに使うキーのリストを作成
                strkey_list = new string[Information.UBound(list) + 1];
                {
                    var withBlock5 = SRC.UList;
                    switch (sort_mode ?? "")
                    {
                        case "名称":
                        case "ユニット名称":
                            {
                                var loopTo6 = (short)Information.UBound(list);
                                for (i = 2; i <= loopTo6; i++)
                                {
                                    Unit localItem3() { var tmp = id_list; object argIndex1 = tmp[i]; var ret = withBlock5.Item(ref argIndex1); return ret; }

                                    strkey_list[i] = localItem3().KanaName;
                                }

                                break;
                            }

                        case "パイロット名称":
                            {
                                var loopTo7 = (short)Information.UBound(list);
                                for (i = 2; i <= loopTo7; i++)
                                {
                                    var tmp1 = id_list;
                                    object argIndex5 = tmp1[i];
                                    {
                                        var withBlock6 = withBlock5.Item(ref argIndex5);
                                        if (withBlock6.CountPilot() > 0)
                                        {
                                            strkey_list[i] = withBlock6.MainPilot().KanaName;
                                        }
                                    }
                                }

                                break;
                            }
                    }
                }

                // キーを使って並べ換え
                var loopTo8 = (short)(Information.UBound(strkey_list) - 1);
                for (i = 2; i <= loopTo8; i++)
                {
                    max_item = i;
                    max_str = strkey_list[i];
                    var loopTo9 = (short)Information.UBound(strkey_list);
                    for (j = (short)(i + 1); j <= loopTo9; j++)
                    {
                        if (Strings.StrComp(strkey_list[j], max_str, (CompareMethod)1) == -1)
                        {
                            max_item = j;
                            max_str = strkey_list[j];
                        }
                    }

                    if (max_item != i)
                    {
                        buf = list[i];
                        list[i] = list[max_item];
                        list[max_item] = buf;
                        buf = id_list[i];
                        id_list[i] = id_list[max_item];
                        id_list[max_item] = buf;
                        strkey_list[max_item] = strkey_list[i];
                    }
                }
            }

            GUI.ListItemFlag = new bool[1];
            GUI.ListItemID = new string[Information.UBound(list) + 1];
            var loopTo10 = (short)Information.UBound(list);
            for (i = 1; i <= loopTo10; i++)
                GUI.ListItemID[i] = id_list[i];

            // リストを表示
            string argoname2 = "等身大基準";
            if (pilot_status_mode)
            {
                string arglb_caption = uparty + "パイロット一覧";
                string argtname = "ＳＰ";
                Unit argu = null;
                string argtname1 = "スペシャルパワー";
                Unit argu1 = null;
                string arglb_info = "パイロット名       レベル    " + Expression.Term(ref argtname, ref argu, 4) + "  " + Expression.Term(ref argtname1, u: ref argu1);
                string arglb_mode = "連続表示";
                ret = GUI.ListBox(ref arglb_caption, ref list, ref arglb_info, ref arglb_mode);
            }
            else if (Expression.IsOptionDefined(ref argoname2))
            {
                string arglb_caption2 = uparty + "ユニット一覧";
                string argtname4 = "ＨＰ";
                Unit argu4 = null;
                string argtname5 = "ＥＮ";
                Unit argu5 = null;
                string arglb_info2 = "ユニット名                        Lv     " + Expression.Term(ref argtname4, ref argu4, 8) + Expression.Term(ref argtname5, u: ref argu5);
                string arglb_mode2 = "連続表示";
                ret = GUI.ListBox(ref arglb_caption2, ref list, ref arglb_info2, ref arglb_mode2);
            }
            else
            {
                string arglb_caption1 = uparty + "ユニット一覧";
                string argtname2 = "ＨＰ";
                Unit argu2 = null;
                string argtname3 = "ＥＮ";
                Unit argu3 = null;
                string arglb_info1 = "ユニット               パイロット Lv     " + Expression.Term(ref argtname2, ref argu2, 8) + Expression.Term(ref argtname3, u: ref argu3);
                string arglb_mode1 = "連続表示";
                ret = GUI.ListBox(ref arglb_caption1, ref list, ref arglb_info1, ref arglb_mode1);
            }

            switch (ret)
            {
                case 0:
                    {
                        // キャンセル
                        My.MyProject.Forms.frmListBox.Hide();
                        GUI.ReduceListBoxHeight();
                        GUI.EnlargeListBoxWidth();
                        GUI.ListItemID = new string[1];
                        GUI.UnlockGUI();
                        return;
                    }

                case 1:
                    {
                        // 表示する陣営
                        mode_list = new string[5];
                        mode_list[1] = "味方一覧";
                        mode_list[2] = "ＮＰＣ一覧";
                        mode_list[3] = "敵一覧";
                        mode_list[4] = "中立一覧";

                        // ソート方法を選択
                        string argoname3 = "等身大基準";
                        if (pilot_status_mode)
                        {
                            Array.Resize(ref mode_list, 8);
                            mode_list[5] = "パイロット名称で並べ替え";
                            mode_list[6] = "レベルで並べ替え";
                            string argtname6 = "ＳＰ";
                            Unit argu6 = null;
                            mode_list[7] = Expression.Term(ref argtname6, u: ref argu6) + "で並べ替え";
                        }
                        else if (Expression.IsOptionDefined(ref argoname3))
                        {
                            Array.Resize(ref mode_list, 9);
                            mode_list[5] = "名称で並べ替え";
                            mode_list[6] = "レベルで並べ替え";
                            string argtname9 = "ＨＰ";
                            Unit argu9 = null;
                            mode_list[7] = Expression.Term(ref argtname9, u: ref argu9) + "で並べ替え";
                            string argtname10 = "ＥＮ";
                            Unit argu10 = null;
                            mode_list[8] = Expression.Term(ref argtname10, u: ref argu10) + "で並べ替え";
                        }
                        else
                        {
                            Array.Resize(ref mode_list, 10);
                            string argtname7 = "ＨＰ";
                            Unit argu7 = null;
                            mode_list[5] = Expression.Term(ref argtname7, u: ref argu7) + "で並べ替え";
                            string argtname8 = "ＥＮ";
                            Unit argu8 = null;
                            mode_list[6] = Expression.Term(ref argtname8, u: ref argu8) + "で並べ替え";
                            mode_list[7] = "パイロットレベルで並べ替え";
                            mode_list[8] = "ユニット名称で並べ替え";
                            mode_list[9] = "パイロット名称で並べ替え";
                        }

                        GUI.ListItemID = new string[Information.UBound(mode_list) + 1];
                        GUI.ListItemFlag = new bool[Information.UBound(mode_list) + 1];
                        string arglb_caption3 = "選択";
                        string arglb_info3 = "一覧表示方法";
                        string arglb_mode3 = "連続表示";
                        ret = GUI.ListBox(ref arglb_caption3, ref mode_list, ref arglb_info3, ref arglb_mode3);

                        // 陣営を変更して再表示
                        if (ret > 0)
                        {
                            if (Strings.Right(mode_list[ret], 2) == "一覧")
                            {
                                uparty = Strings.Left(mode_list[ret], Strings.Len(mode_list[ret]) - 2);
                                goto Beginning;
                            }
                            else if (Strings.Right(mode_list[ret], 5) == "で並べ替え")
                            {
                                sort_mode = Strings.Left(mode_list[ret], Strings.Len(mode_list[ret]) - 5);
                                goto SortList;
                            }
                        }

                        goto SortList;
                        break;
                    }
            }

            My.MyProject.Forms.frmListBox.Hide();
            GUI.ReduceListBoxHeight();
            GUI.EnlargeListBoxWidth();

            // 選択されたユニットを画面中央に表示
            var tmp2 = GUI.ListItemID;
            object argIndex6 = tmp2[ret];
            u = SRC.UList.Item(ref argIndex6);
            GUI.Center(u.x, u.y);
            GUI.RefreshScreen();
            Status.DisplayUnitStatus(ref u);

            // カーソル自動移動
            if (SRC.AutoMoveCursor)
            {
                string argcursor_mode = "ユニット選択";
                GUI.MoveCursorPos(ref argcursor_mode, u);
            }

            GUI.ListItemID = new string[1];
            GUI.UnlockGUI();
        }

        // スペシャルパワー検索コマンド
        // MOD START MARGE
        // Public Sub SearchSpecialPowerCommand()
        private static void SearchSpecialPowerCommand()
        {
            // MOD END MARGE
            short j, i, ret;
            string[] list;
            string[] list2;
            bool[] flist;
            string[] pid;
            string buf;
            Pilot p;
            string[] id_list;
            var strkey_list = default(string[]);
            short max_item;
            string max_str;
            bool found;
            GUI.LockGUI();

            // イベント専用のコマンドを除いた全スペシャルパワーのリストを作成
            list = new string[1];
            var loopTo = SRC.SPDList.Count();
            for (i = 1; i <= loopTo; i++)
            {
                object argIndex1 = i;
                {
                    var withBlock = SRC.SPDList.Item(ref argIndex1);
                    if (withBlock.ShortName != "非表示")
                    {
                        Array.Resize(ref list, Information.UBound(list) + 1 + 1);
                        Array.Resize(ref strkey_list, Information.UBound(list) + 1);
                        list[Information.UBound(list)] = withBlock.Name;
                        strkey_list[Information.UBound(list)] = withBlock.KanaName;
                    }
                }
            }

            // ソート
            var loopTo1 = (short)(Information.UBound(strkey_list) - 1);
            for (i = 1; i <= loopTo1; i++)
            {
                max_item = i;
                max_str = strkey_list[i];
                var loopTo2 = (short)Information.UBound(strkey_list);
                for (j = (short)(i + 1); j <= loopTo2; j++)
                {
                    if (Strings.StrComp(strkey_list[j], max_str, (CompareMethod)1) == -1)
                    {
                        max_item = j;
                        max_str = strkey_list[j];
                    }
                }

                if (max_item != i)
                {
                    buf = list[i];
                    list[i] = list[max_item];
                    list[max_item] = buf;
                    buf = strkey_list[i];
                    strkey_list[i] = max_str;
                    strkey_list[max_item] = buf;
                }
            }

            // 個々のスペシャルパワーに対して、そのスペシャルパワーを使用可能なパイロットが
            // いるかどうか判定
            flist = new bool[Information.UBound(list) + 1];
            var loopTo3 = (short)Information.UBound(list);
            for (i = 1; i <= loopTo3; i++)
            {
                flist[i] = true;
                foreach (Pilot currentP in SRC.PList)
                {
                    p = currentP;
                    if (p.Party == "味方")
                    {
                        if (p.Unit_Renamed is object)
                        {
                            object argIndex2 = "憑依";
                            if (p.Unit_Renamed.Status_Renamed == "出撃" & !p.Unit_Renamed.IsConditionSatisfied(ref argIndex2))
                            {
                                // 本当に乗っている？
                                found = false;
                                {
                                    var withBlock1 = p.Unit_Renamed;
                                    if (ReferenceEquals(p, withBlock1.MainPilot()))
                                    {
                                        found = true;
                                    }
                                    else
                                    {
                                        var loopTo4 = withBlock1.CountPilot();
                                        for (j = 2; j <= loopTo4; j++)
                                        {
                                            Pilot localPilot() { object argIndex1 = j; var ret = withBlock1.Pilot(ref argIndex1); return ret; }

                                            if (ReferenceEquals(p, localPilot()))
                                            {
                                                found = true;
                                                break;
                                            }
                                        }

                                        var loopTo5 = withBlock1.CountSupport();
                                        for (j = 1; j <= loopTo5; j++)
                                        {
                                            Pilot localSupport() { object argIndex1 = j; var ret = withBlock1.Support(ref argIndex1); return ret; }

                                            if (ReferenceEquals(p, localSupport()))
                                            {
                                                found = true;
                                                break;
                                            }
                                        }

                                        if (ReferenceEquals(p, withBlock1.AdditionalSupport()))
                                        {
                                            found = true;
                                        }
                                    }
                                }

                                if (found)
                                {
                                    if (p.IsSpecialPowerAvailable(ref list[i]))
                                    {
                                        flist[i] = false;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            while (true)
            {
                GUI.ListItemFlag = new bool[Information.UBound(list) + 1];
                GUI.ListItemComment = new string[Information.UBound(list) + 1];
                id_list = new string[Information.UBound(list) + 1];
                strkey_list = new string[Information.UBound(list) + 1];

                // 選択出来ないスペシャルパワーをマスク
                var loopTo6 = (short)Information.UBound(GUI.ListItemFlag);
                for (i = 1; i <= loopTo6; i++)
                    GUI.ListItemFlag[i] = flist[i];

                // スペシャルパワーの解説を設定
                var loopTo7 = (short)Information.UBound(GUI.ListItemComment);
                for (i = 1; i <= loopTo7; i++)
                {
                    SpecialPowerData localItem() { var tmp = list; object argIndex1 = tmp[i]; var ret = SRC.SPDList.Item(ref argIndex1); return ret; }

                    GUI.ListItemComment[i] = localItem().Comment;
                }

                // 検索するスペシャルパワーを選択
                GUI.TopItem = 1;
                string argtname = "スペシャルパワー";
                Unit argu = null;
                string arglb_caption = Expression.Term(ref argtname, u: ref argu) + "検索";
                ret = GUI.MultiColumnListBox(ref arglb_caption, ref list, true);
                if (ret == 0)
                {
                    CancelCommand();
                    GUI.UnlockGUI();
                    return;
                }

                SelectedSpecialPower = list[ret];

                // 選択されたスペシャルパワーを使用できるパイロットの一覧を作成
                list2 = new string[1];
                GUI.ListItemFlag = new bool[1];
                id_list = new string[1];
                pid = new string[1];
                foreach (Pilot currentP1 in SRC.PList)
                {
                    p = currentP1;
                    // 選択したスペシャルパワーを使用できるパイロットかどうか判定
                    if (p.Party != "味方")
                    {
                        goto NextLoop;
                    }

                    if (p.Unit_Renamed is null)
                    {
                        goto NextLoop;
                    }

                    if (p.Unit_Renamed.Status_Renamed != "出撃")
                    {
                        goto NextLoop;
                    }

                    if (p.Unit_Renamed.CountPilot() > 0)
                    {
                        object argIndex3 = 1;
                        if ((p.ID ?? "") == (p.Unit_Renamed.Pilot(ref argIndex3).ID ?? "") & (p.ID ?? "") != (p.Unit_Renamed.MainPilot().ID ?? ""))
                        {
                            // 追加パイロットのため、使用されていない
                            goto NextLoop;
                        }
                    }

                    if (!p.IsSpecialPowerAvailable(ref SelectedSpecialPower))
                    {
                        goto NextLoop;
                    }

                    // パイロットをリストに追加
                    Array.Resize(ref list2, Information.UBound(list2) + 1 + 1);
                    Array.Resize(ref GUI.ListItemFlag, Information.UBound(list2) + 1);
                    Array.Resize(ref id_list, Information.UBound(list2) + 1);
                    Array.Resize(ref pid, Information.UBound(list2) + 1);
                    GUI.ListItemFlag[Information.UBound(list2)] = false;
                    id_list[Information.UBound(list2)] = p.Unit_Renamed.ID;
                    pid[Information.UBound(list2)] = p.ID;
                    string localRightPaddedString() { string argbuf = p.get_Nickname(false); var ret = GeneralLib.RightPaddedString(ref argbuf, 19); p.get_Nickname(false) = argbuf; return ret; }

                    string localRightPaddedString1() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.SP) + "/" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.MaxSP); var ret = GeneralLib.RightPaddedString(ref argbuf, 10); return ret; }

                    list2[Information.UBound(list2)] = localRightPaddedString() + localRightPaddedString1();
                    buf = "";
                    var loopTo8 = p.CountSpecialPower;
                    for (j = 1; j <= loopTo8; j++)
                    {
                        SpecialPowerData localItem1() { object argIndex1 = p.get_SpecialPower(j); var ret = SRC.SPDList.Item(ref argIndex1); p.get_SpecialPower(j) = Conversions.ToString(argIndex1); return ret; }

                        buf = buf + localItem1().ShortName;
                    }

                    list2[Information.UBound(list2)] = list2[Information.UBound(list2)] + GeneralLib.RightPaddedString(ref buf, 12);
                    if (p.SP < p.SpecialPowerCost(ref SelectedSpecialPower))
                    {
                        string argtname1 = "ＳＰ";
                        list2[Information.UBound(list2)] = list2[Information.UBound(list2)] + " " + Expression.Term(ref argtname1, ref p.Unit_Renamed) + "不足";
                    }

                    if (p.Unit_Renamed.Action == 0)
                    {
                        list2[Information.UBound(list2)] = list2[Information.UBound(list2)] + " 行動済";
                    }

                    NextLoop:
                    ;
                }

                SelectedSpecialPower = "";

                // 検索をかけるパイロットの選択
                GUI.TopItem = 1;
                GUI.EnlargeListBoxHeight();
                string argoname = "等身大基準";
                if (Expression.IsOptionDefined(ref argoname))
                {
                    string arglb_caption1 = "ユニット選択";
                    string argtname2 = "SP";
                    Unit argu1 = null;
                    string argtname3 = "SP";
                    Unit argu2 = null;
                    string argtname4 = "スペシャルパワー";
                    Unit argu3 = null;
                    string arglb_info = "ユニット           " + Expression.Term(ref argtname2, ref argu1, 2) + "/Max" + Expression.Term(ref argtname3, ref argu2, 2) + "  " + Expression.Term(ref argtname4, u: ref argu3);
                    string arglb_mode = "";
                    ret = GUI.ListBox(ref arglb_caption1, ref list2, ref arglb_info, lb_mode: ref arglb_mode);
                }
                else
                {
                    string arglb_caption2 = "パイロット選択";
                    string argtname5 = "SP";
                    Unit argu4 = null;
                    string argtname6 = "SP";
                    Unit argu5 = null;
                    string argtname7 = "スペシャルパワー";
                    Unit argu6 = null;
                    string arglb_info1 = "パイロット         " + Expression.Term(ref argtname5, ref argu4, 2) + "/Max" + Expression.Term(ref argtname6, ref argu5, 2) + "  " + Expression.Term(ref argtname7, u: ref argu6);
                    string arglb_mode1 = "";
                    ret = GUI.ListBox(ref arglb_caption2, ref list2, ref arglb_info1, lb_mode: ref arglb_mode1);
                }

                GUI.ReduceListBoxHeight();

                // パイロットの乗るユニットを画面中央に表示
                if (ret > 0)
                {
                    var tmp = pid;
                    object argIndex4 = tmp[ret];
                    {
                        var withBlock2 = SRC.PList.Item(ref argIndex4);
                        GUI.Center(withBlock2.Unit_Renamed.x, withBlock2.Unit_Renamed.y);
                        GUI.RefreshScreen();
                        Status.DisplayUnitStatus(ref withBlock2.Unit_Renamed);

                        // カーソル自動移動
                        if (SRC.AutoMoveCursor)
                        {
                            string argcursor_mode = "ユニット選択";
                            GUI.MoveCursorPos(ref argcursor_mode, withBlock2.Unit_Renamed);
                        }
                    }

                    id_list = new string[1];
                    GUI.UnlockGUI();
                    return;
                }
            }
        }

        // リスタートコマンド
        // MOD START MARGE
        // Public Sub RestartCommand()
        private static void RestartCommand()
        {
            // MOD END MARGE
            short ret;

            // リスタートを行うか確認
            ret = (short)Interaction.MsgBox("リスタートしますか？", (MsgBoxStyle)((int)MsgBoxStyle.OkCancel + (int)MsgBoxStyle.Question), "リスタート");
            if (ret == (int)MsgBoxResult.Cancel)
            {
                return;
            }

            GUI.LockGUI();
            string argfname = SRC.ScenarioPath + "_リスタート.src";
            bool argquick_load = true;
            SRC.RestoreData(ref argfname, ref argquick_load);
            GUI.UnlockGUI();
        }

        // クイックロードコマンド
        // MOD START MARGE
        // Public Sub QuickLoadCommand()
        private static void QuickLoadCommand()
        {
            // MOD END MARGE
            short ret;

            // ロードを行うか確認
            ret = (short)Interaction.MsgBox("データをロードしますか？", (MsgBoxStyle)((int)MsgBoxStyle.OkCancel + (int)MsgBoxStyle.Question), "クイックロード");
            if (ret == (int)MsgBoxResult.Cancel)
            {
                return;
            }

            GUI.LockGUI();
            string argfname = SRC.ScenarioPath + "_クイックセーブ.src";
            bool argquick_load = true;
            SRC.RestoreData(ref argfname, ref argquick_load);

            // 画面を書き直してステータスを表示
            GUI.RedrawScreen();
            Status.DisplayGlobalStatus();
            GUI.UnlockGUI();
        }

        // クイックセーブコマンド
        // MOD START MARGE
        // Public Sub QuickSaveCommand()
        private static void QuickSaveCommand()
        {
            // MOD END MARGE

            GUI.LockGUI();

            // マウスカーソルを砂時計に
            // UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
            Cursor.Current = Cursors.WaitCursor;

            // 中断データをセーブ
            string argfname = SRC.ScenarioPath + "_クイックセーブ.src";
            SRC.DumpData(ref argfname);
            GUI.UnlockGUI();

            // マウスカーソルを元に戻す
            // UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
            Cursor.Current = Cursors.Default;
        }


        // プレイを中断し、中断用データをセーブする
        // MOD START MARGE
        // Public Sub DumpCommand()
        private static void DumpCommand()
        {
            // MOD END MARGE
            string fname, save_path = default;
            short ret, i;

            // プレイを中断するか確認
            ret = (short)Interaction.MsgBox("プレイを中断しますか？", (MsgBoxStyle)((int)MsgBoxStyle.OkCancel + (int)MsgBoxStyle.Question), "中断");
            if (ret == (int)MsgBoxResult.Cancel)
            {
                return;
            }

            // 中断データをセーブするファイル名を決定
            var loopTo = (short)Strings.Len(SRC.ScenarioFileName);
            for (i = 1; i <= loopTo; i++)
            {
                if (Strings.Mid(SRC.ScenarioFileName, Strings.Len(SRC.ScenarioFileName) - i + 1, 1) == @"\")
                {
                    break;
                }
            }

            fname = Strings.Mid(SRC.ScenarioFileName, Strings.Len(SRC.ScenarioFileName) - i + 2, i - 5);
            fname = fname + "を中断.src";
            string argdtitle = "データセーブ";
            string argftype = "ｾｰﾌﾞﾃﾞｰﾀ";
            string argfsuffix = "src";
            string argftype2 = "";
            string argfsuffix2 = "";
            string argftype3 = "";
            string argfsuffix3 = "";
            fname = FileDialog.SaveFileDialog(ref argdtitle, ref SRC.ScenarioPath, ref fname, 2, ref argftype, ref argfsuffix, ftype2: ref argftype2, fsuffix2: ref argfsuffix2, ftype3: ref argftype3, fsuffix3: ref argfsuffix3);
            if (string.IsNullOrEmpty(fname))
            {
                // キャンセル
                return;
            }

            // セーブ先はシナリオフォルダ？
            if (Strings.InStr(fname, @"\") > 0)
            {
                string argstr2 = @"\";
                save_path = Strings.Left(fname, GeneralLib.InStr2(ref fname, ref argstr2));
            }
            // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            if ((FileSystem.Dir(save_path) ?? "") != (FileSystem.Dir(SRC.ScenarioPath) ?? ""))
            {
                if ((int)Interaction.MsgBox("セーブファイルはシナリオフォルダにないと読み込めません。" + Constants.vbCr + Constants.vbLf + "このままセーブしますか？", (MsgBoxStyle)((int)MsgBoxStyle.OkCancel + (int)MsgBoxStyle.Question)) != 1)
                {
                    return;
                }
            }

            // UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
            Cursor.Current = Cursors.WaitCursor; // マウスカーソルを砂時計に
            GUI.LockGUI();

            // 中断データをセーブ
            SRC.DumpData(ref fname);

            // マウスカーソルを元に戻す
            // UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
            Cursor.Current = Cursors.Default;
            GUI.MainForm.Hide();

            // ゲームを終了
            SRC.ExitGame();
        }


        // 全体マップの表示
        // MOD START MARGE
        // Public Sub GlobalMapCommand()
        private static void GlobalMapCommand()
        {
            // MOD END MARGE
            PictureBox pic;
            short xx, yy;
            short num = default, num2 = default;
            int mwidth, mheight;
            int ret, smode;
            Unit u;
            short i, j;
            bool prev_mode;
            GUI.LockGUI();
            {
                var withBlock = GUI.MainForm;
                // 見やすいように背景を設定
                // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                withBlock.picMain(0).BackColor = 0xC0C0C0;
                // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                withBlock.picMain(0) = Image.FromFile("");

                // マップの縦横の比率を元に縮小マップの大きさを決める
                if (Map.MapWidth > Map.MapHeight)
                {
                    mwidth = 300;
                    mheight = 300 * Map.MapHeight / Map.MapWidth;
                }
                else
                {
                    mheight = 300;
                    mwidth = 300 * Map.MapWidth / Map.MapHeight;
                }

                // マップの全体画像を作成
                // UPGRADE_ISSUE: Control picTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                pic = withBlock.picTmp;
                pic.Image = Image.FromFile("");
                pic.Width = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsX(GUI.MapPWidth);
                pic.Height = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsY(GUI.MapPHeight);
                // UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                // UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                ret = GUI.BitBlt(pic.hDC, 0, 0, GUI.MapPWidth, GUI.MapPHeight, withBlock.picBack.hDC, 0, 0, GUI.SRCCOPY);
                var loopTo = Map.MapWidth;
                for (i = 1; i <= loopTo; i++)
                {
                    xx = (short)(32 * (i - 1));
                    var loopTo1 = Map.MapHeight;
                    for (j = 1; j <= loopTo1; j++)
                    {
                        yy = (short)(32 * (j - 1));
                        u = Map.MapDataForUnit[i, j];
                        if (u is object)
                        {
                            if (u.BitmapID > 0)
                            {
                                string argfname = "地形ユニット";
                                if (u.Action > 0 | u.IsFeatureAvailable(ref argfname))
                                {
                                    // ユニット
                                    // UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    // UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                                    ret = GUI.BitBlt(pic.hDC, xx, yy, 32, 32, withBlock.picUnitBitmap.hDC, 32 * (u.BitmapID % 15), 96 * (u.BitmapID / 15), GUI.SRCCOPY);
                                }
                                else
                                {
                                    // 行動済のユニット
                                    // UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    // UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                                    ret = GUI.BitBlt(pic.hDC, xx, yy, 32, 32, withBlock.picUnitBitmap.hDC, 32 * (u.BitmapID % 15), 96 * (u.BitmapID / 15) + 32, GUI.SRCCOPY);
                                }

                                // ユニットのいる場所に合わせて表示を変更
                                switch (u.Area ?? "")
                                {
                                    case "空中":
                                        {
                                            // UPGRADE_ISSUE: PictureBox メソッド pic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                                            pic.Line(xx, yy + 28); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                                            break;
                                        }

                                    case "水中":
                                        {
                                            // UPGRADE_ISSUE: PictureBox メソッド pic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                                            pic.Line(xx, yy + 3); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                                            break;
                                        }

                                    case "地中":
                                        {
                                            // UPGRADE_ISSUE: PictureBox メソッド pic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                                            pic.Line(xx, yy + 28); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                                            // UPGRADE_ISSUE: PictureBox メソッド pic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                                            pic.Line(xx, yy + 3); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                                            break;
                                        }

                                    case "宇宙":
                                        {
                                            if (Map.TerrainClass(i, j) == "月面")
                                            {
                                                // UPGRADE_ISSUE: PictureBox メソッド pic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                                                pic.Line(xx, yy + 28); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                                            }

                                            break;
                                        }
                                }
                            }
                        }
                    }
                }

                // マップ全体を縮小して描画
                // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                smode = GUI.GetStretchBltMode(withBlock.picMain(0).hDC);
                // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                ret = GUI.SetStretchBltMode(withBlock.picMain(0).hDC, GUI.STRETCH_DELETESCANS);
                // UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                ret = GUI.StretchBlt(withBlock.picMain(0).hDC, (GUI.MainPWidth - mwidth) / 2, (GUI.MainPHeight - mheight) / 2, mwidth, mheight, pic.hDC, 0, 0, GUI.MapPWidth, GUI.MapPHeight, GUI.SRCCOPY);
                // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                ret = GUI.SetStretchBltMode(withBlock.picMain(0).hDC, smode);

                // マップ全体画像を破棄
                pic.Image = Image.FromFile("");
                pic.Width = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsX(32d);
                pic.Height = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsY(32d);

                // 画面を更新
                // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                withBlock.picMain(0).Refresh();
            }

            // 味方ユニット数、敵ユニット数のカウント
            foreach (Unit currentU in SRC.UList)
            {
                u = currentU;
                if (u.Status_Renamed == "出撃" | u.Status_Renamed == "格納")
                {
                    if (u.Party0 == "味方" | u.Party0 == "ＮＰＣ")
                    {
                        num = (short)(num + 1);
                    }
                    else
                    {
                        num2 = (short)(num2 + 1);
                    }
                }
            }

            // 各ユニット数の表示
            prev_mode = GUI.AutoMessageMode;
            GUI.AutoMessageMode = false;
            Unit argu1 = null;
            Unit argu2 = null;
            GUI.OpenMessageForm(u1: ref argu1, u2: ref argu2);
            string argpname = "システム";
            GUI.DisplayMessage(ref argpname, "味方ユニット： " + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(num) + ";" + "敵ユニット  ： " + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(num2));
            GUI.CloseMessageForm();
            GUI.AutoMessageMode = prev_mode;

            // 画面を元に戻す
            // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            GUI.MainForm.picMain(0).BackColor = 0xFFFFFF;
            GUI.RefreshScreen();
            GUI.UnlockGUI();
        }


        // 現在の選択状況を記録
        public static void SaveSelections()
        {
            // スタックのインデックスを増やす
            SelectionStackIndex = (short)(SelectionStackIndex + 1);

            // スタック領域確保
            Array.Resize(ref SavedSelectedUnit, SelectionStackIndex + 1);
            Array.Resize(ref SavedSelectedTarget, SelectionStackIndex + 1);
            Array.Resize(ref SavedSelectedUnitForEvent, SelectionStackIndex + 1);
            Array.Resize(ref SavedSelectedTargetForEvent, SelectionStackIndex + 1);
            Array.Resize(ref SavedSelectedWeapon, SelectionStackIndex + 1);
            Array.Resize(ref SavedSelectedWeaponName, SelectionStackIndex + 1);
            Array.Resize(ref SavedSelectedTWeapon, SelectionStackIndex + 1);
            Array.Resize(ref SavedSelectedTWeaponName, SelectionStackIndex + 1);
            Array.Resize(ref SavedSelectedDefenseOption, SelectionStackIndex + 1);
            Array.Resize(ref SavedSelectedAbility, SelectionStackIndex + 1);
            Array.Resize(ref SavedSelectedAbilityName, SelectionStackIndex + 1);
            Array.Resize(ref SavedSelectedX, SelectionStackIndex + 1);
            Array.Resize(ref SavedSelectedY, SelectionStackIndex + 1);

            // 確保した領域に選択状況を記録
            SavedSelectedUnit[SelectionStackIndex] = SelectedUnit;
            SavedSelectedTarget[SelectionStackIndex] = SelectedTarget;
            SavedSelectedUnitForEvent[SelectionStackIndex] = Event_Renamed.SelectedUnitForEvent;
            SavedSelectedTargetForEvent[SelectionStackIndex] = Event_Renamed.SelectedTargetForEvent;
            SavedSelectedWeapon[SelectionStackIndex] = SelectedWeapon;
            SavedSelectedWeaponName[SelectionStackIndex] = SelectedWeaponName;
            SavedSelectedTWeapon[SelectionStackIndex] = SelectedTWeapon;
            SavedSelectedTWeaponName[SelectionStackIndex] = SelectedTWeaponName;
            SavedSelectedDefenseOption[SelectionStackIndex] = SelectedDefenseOption;
            SavedSelectedAbility[SelectionStackIndex] = SelectedAbility;
            SavedSelectedAbilityName[SelectionStackIndex] = SelectedAbilityName;
            SavedSelectedX[SelectionStackIndex] = SelectedX;
            SavedSelectedY[SelectionStackIndex] = SelectedY;
        }

        // 選択状況を復元
        public static void RestoreSelections()
        {
            // スタックに積まれていない？
            if (SelectionStackIndex == 0)
            {
                return;
            }

            // スタックトップから記録された選択状況を取り出す
            if (SavedSelectedUnit[SelectionStackIndex] is object)
            {
                SelectedUnit = SavedSelectedUnit[SelectionStackIndex].CurrentForm();
            }
            else
            {
                // UPGRADE_NOTE: オブジェクト SelectedUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                SelectedUnit = null;
            }

            if (SavedSelectedTarget[SelectionStackIndex] is object)
            {
                SelectedTarget = SavedSelectedTarget[SelectionStackIndex].CurrentForm();
            }
            else
            {
                // UPGRADE_NOTE: オブジェクト SelectedTarget をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                SelectedTarget = null;
            }

            if (SavedSelectedUnitForEvent[SelectionStackIndex] is object)
            {
                Event_Renamed.SelectedUnitForEvent = SavedSelectedUnitForEvent[SelectionStackIndex].CurrentForm();
            }
            else
            {
                // UPGRADE_NOTE: オブジェクト SelectedUnitForEvent をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                Event_Renamed.SelectedUnitForEvent = null;
            }

            if (SavedSelectedTargetForEvent[SelectionStackIndex] is object)
            {
                Event_Renamed.SelectedTargetForEvent = SavedSelectedTargetForEvent[SelectionStackIndex].CurrentForm();
            }
            else
            {
                // UPGRADE_NOTE: オブジェクト SelectedTargetForEvent をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                Event_Renamed.SelectedTargetForEvent = null;
            }

            SelectedWeapon = SavedSelectedWeapon[SelectionStackIndex];
            SelectedWeaponName = SavedSelectedWeaponName[SelectionStackIndex];
            SelectedTWeapon = SavedSelectedTWeapon[SelectionStackIndex];
            SelectedTWeaponName = SavedSelectedTWeaponName[SelectionStackIndex];
            SelectedDefenseOption = SavedSelectedDefenseOption[SelectionStackIndex];
            SelectedAbility = SavedSelectedAbility[SelectionStackIndex];
            SelectedAbilityName = SavedSelectedAbilityName[SelectionStackIndex];
            SelectedX = SavedSelectedX[SelectionStackIndex];
            SelectedY = SavedSelectedY[SelectionStackIndex];

            // スタックのインデックスを１減らす
            SelectionStackIndex = (short)(SelectionStackIndex - 1);

            // スタックの領域を開放
            Array.Resize(ref SavedSelectedUnit, SelectionStackIndex + 1);
            Array.Resize(ref SavedSelectedTarget, SelectionStackIndex + 1);
            Array.Resize(ref SavedSelectedUnitForEvent, SelectionStackIndex + 1);
            Array.Resize(ref SavedSelectedTargetForEvent, SelectionStackIndex + 1);
            Array.Resize(ref SavedSelectedWeapon, SelectionStackIndex + 1);
            Array.Resize(ref SavedSelectedWeaponName, SelectionStackIndex + 1);
            Array.Resize(ref SavedSelectedTWeapon, SelectionStackIndex + 1);
            Array.Resize(ref SavedSelectedTWeaponName, SelectionStackIndex + 1);
            Array.Resize(ref SavedSelectedDefenseOption, SelectionStackIndex + 1);
            Array.Resize(ref SavedSelectedAbility, SelectionStackIndex + 1);
            Array.Resize(ref SavedSelectedAbilityName, SelectionStackIndex + 1);
            Array.Resize(ref SavedSelectedX, SelectionStackIndex + 1);
            Array.Resize(ref SavedSelectedY, SelectionStackIndex + 1);
        }

        // 選択を入れ替える
        public static void SwapSelections()
        {
            Unit u, t;
            short w, tw;
            string wname, twname;
            u = SelectedUnit;
            t = SelectedTarget;
            SelectedUnit = t;
            SelectedTarget = u;
            w = SelectedWeapon;
            tw = SelectedTWeapon;
            SelectedWeapon = tw;
            SelectedTWeapon = w;
            wname = SelectedWeaponName;
            twname = SelectedTWeaponName;
            SelectedWeaponName = twname;
            SelectedTWeaponName = wname;
        }
    }
}