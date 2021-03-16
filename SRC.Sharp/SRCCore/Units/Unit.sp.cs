using System;
using System.Collections.Generic;
using System.Text;

namespace SRCCore.Units
{
    // === スペシャルパワー関連処理 ===
    public partial class Unit
    {
        // TODO Impl
        // 影響下にあるスペシャルパワー一覧
        public string SpecialPowerInEffect()
        {
            string SpecialPowerInEffectRet = default;
            //Condition cnd;
            //foreach (Condition currentCnd in colSpecialPowerInEffect)
            //{
            //    cnd = currentCnd;
            //    {
            //        var withBlock = SRC.SPDList.Item((object)cnd.Name);
            //        if (withBlock.intName == "非表示")
            //        {
            //            // イベント専用
            //            goto NextSpecialPower;
            //        }

            //        if (withBlock.Duration == "みがわり")
            //        {
            //            // みがわりは別表示
            //            goto NextSpecialPower;
            //        }

            //        SpecialPowerInEffectRet = SpecialPowerInEffectRet + withBlock.intName;
            //    }

            //NextSpecialPower:
            //    ;
            //}

            //// みがわりはかばってくれるユニットを表示する
            //foreach (Condition currentCnd1 in colSpecialPowerInEffect)
            //{
            //    cnd = currentCnd1;
            //    {
            //        var withBlock1 = SRC.SPDList.Item((object)cnd.Name);
            //        if (withBlock1.Duration == "みがわり")
            //        {
            //            if (SRC.PList.IsDefined((object)cnd.StrData))
            //            {
            //                SpecialPowerInEffectRet = SpecialPowerInEffectRet + withBlock1.intName + "(" + SRC.PList.Item((object)cnd.StrData).get_Nickname(false) + ")";
            //            }

            //            return SpecialPowerInEffectRet;
            //        }
            //    }
            //}

            return SpecialPowerInEffectRet;
        }

        // ユニットがスペシャルパワー sname の影響下にあるかどうか
        public bool IsSpecialPowerInEffect(string sname)
        {
            return false;
            //            bool IsSpecialPowerInEffectRet = default;
            //            Condition cnd;
            //            if (colSpecialPowerInEffect.Count == 0)
            //            {
            //                return IsSpecialPowerInEffectRet;
            //            };
            //#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            //            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 137362


            //            Input:

            //                    On Error GoTo ErrorHandler

            //             */
            //            cnd = (Condition)colSpecialPowerInEffect[sname];
            //            IsSpecialPowerInEffectRet = true;
            //            return IsSpecialPowerInEffectRet;
            //        ErrorHandler:
            //            ;
        }

        // ユニットがスペシャルパワー効果 sptype の影響下にあるかどうか
        public bool IsUnderSpecialPowerEffect(string sptype)
        {
            bool IsUnderSpecialPowerEffectRet = default;
            //int i;
            //foreach (Condition cnd in colSpecialPowerInEffect)
            //{
            //    {
            //        var withBlock = SRC.SPDList.Item((object)cnd.Name);
            //        var loopTo = withBlock.CountEffect();
            //        for (i = 1; i <= loopTo; i++)
            //        {
            //            if ((withBlock.EffectType(i) ?? "") == (sptype ?? ""))
            //            {
            //                IsUnderSpecialPowerEffectRet = true;
            //                return IsUnderSpecialPowerEffectRet;
            //            }
            //        }
            //    }
            //}

            IsUnderSpecialPowerEffectRet = false;
            return IsUnderSpecialPowerEffectRet;
        }

        //// 影響下にあるスペシャルパワーの総数
        //public int CountSpecialPower()
        //{
        //    int CountSpecialPowerRet = default;
        //    CountSpecialPowerRet = colSpecialPowerInEffect.Count;
        //    return CountSpecialPowerRet;
        //}

        //// 影響下にあるスペシャルパワー
        //public SpecialPowerData SpecialPower(object Index)
        //{
        //    SpecialPowerData SpecialPowerRet = default;
        //    Condition cnd;
        //    ;
        //    #error Cannot convert OnErrorGoToStatementSyntax - see comment for details
        //    /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 138333


        //    Input:

        //            On Error GoTo ErrorHandler

        //     */
        //    cnd = (Condition)colSpecialPowerInEffect[Index];
        //    SpecialPowerRet = SRC.SPDList.Item((object)cnd.Name);
        //    return SpecialPowerRet;
        //ErrorHandler:
        //    ;

        //    // UPGRADE_NOTE: オブジェクト SpecialPower をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
        //    SpecialPowerRet = null;
        //}

        // スペシャルパワー mname の効果レベル
        public double SpecialPowerEffectLevel(string sname)
        {
            double SpecialPowerEffectLevelRet = default;
            //int i;
            //double lv;
            //lv = Constants.DEFAULT_LEVEL;
            //foreach (Condition cnd in colSpecialPowerInEffect)
            //{
            //    {
            //        var withBlock = SRC.SPDList.Item((object)cnd.Name);
            //        var loopTo = withBlock.CountEffect();
            //        for (i = 1; i <= loopTo; i++)
            //        {
            //            if ((withBlock.EffectType(i) ?? "") == (sname ?? ""))
            //            {
            //                if (withBlock.EffectLevel(i) > lv)
            //                {
            //                    lv = withBlock.EffectLevel(i);
            //                }

            //                break;
            //            }
            //        }
            //    }
            //}

            //if (lv != Constants.DEFAULT_LEVEL)
            //{
            //    SpecialPowerEffectLevelRet = lv;
            //}

            return SpecialPowerEffectLevelRet;
        }

        // スペシャルパワーのデータ
        public string SpecialPowerData(object Index)
        {
            string SpecialPowerDataRet = default;
            //Condition cnd;
            //;
            //#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            ///* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 139541


            //Input:

            //        On Error GoTo ErrorHandler

            // */
            //cnd = (Condition)colSpecialPowerInEffect[Index];
            //SpecialPowerDataRet = cnd.StrData;
            return SpecialPowerDataRet;
        }

        //// スペシャルパワー sname の効果を適用
        //public void MakeSpecialPowerInEffect(string sname, [Optional, DefaultParameterValue("")] string sdata)
        //{
        //    var cnd = new Condition();

        //    // すでに使用されていればなにもしない
        //    if (IsSpecialPowerInEffect(sname))
        //    {
        //        return;
        //    }

        //    cnd.Name = sname;
        //    cnd.StrData = sdata;
        //    colSpecialPowerInEffect.Add(cnd, sname);
        //}

        // 持続時間が stype であるスペシャルパワーの効果を発動後、取り除く
        public void RemoveSpecialPowerInEffect(string stype)
        {
            //SpecialPowerData sd;
            //int i;
            //bool is_message_form_visible;
            //string pid;

            //// メッセージウィンドウが表示されているか記録
            //is_message_form_visible = My.MyProject.Forms.frmMessage.Visible;
            //i = 1;
            //while (i <= CurrentForm().CountSpecialPower())
            //{
            //    object argIndex1 = i;
            //    sd = SpecialPower(argIndex1);

            //    // スペシャルパワーの持続期間が指定したものと一致しているかチェック
            //    if ((stype ?? "") != (sd.Duration ?? ""))
            //    {
            //        i = (i + 1);
            //        goto NextSP;
            //    }

            //    // 持続期間が敵ターンの場合、スペシャルパワーをかけてきた敵のフェイズ
            //    // が来るまで効果を削除しない
            //    if (stype == "敵ターン")
            //    {
            //        object argIndex3 = SpecialPowerData((object)sd.Name);
            //        if (SRC.PList.IsDefined(argIndex3))
            //        {
            //            object argIndex2 = SpecialPowerData((object)sd.Name);
            //            {
            //                var withBlock = SRC.PList.Item(argIndex2);
            //                if (withBlock.Unit_Renamed is object)
            //                {
            //                    if ((withBlock.Unit_Renamed.CurrentForm().Party ?? "") != (SRC.Stage ?? ""))
            //                    {
            //                        i = (i + 1);
            //                        goto NextSP;
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    // 消去するスペシャルパワーの効果を発動
            //    if (CurrentForm().Status_Renamed == "出撃")
            //    {
            //        sd.Apply(CurrentForm().MainPilot(), CurrentForm(), false, true);
            //    }

            //    // スペシャルパワーの効果を削除
            //    object argIndex4 = i;
            //    CurrentForm().RemoveSpecialPowerInEffect2(argIndex4);
            //NextSP:
            //    ;
            //}

            //// メッセージウィンドウが元から表示されていなければ閉じておく
            //if (!is_message_form_visible & My.MyProject.Forms.frmMessage.Visible)
            //{
            //    GUI.CloseMessageForm();
            //}
        }

        // スペシャルパワー sname の効果を取り除く
        public void RemoveSpecialPowerInEffect2(object Index)
        {
            //#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            //    /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 141635


            //    Input:
            //            On Error GoTo ErrorHandler

            //     */
            //    colSpecialPowerInEffect.Remove(Index);
            //    return;
            //ErrorHandler:
            //    ;
        }

        // 全てのスペシャルパワーの効果を取り除く
        public void RemoveAllSpecialPowerInEffect()
        {
            //int i;
            //{
            //    var withBlock = colSpecialPowerInEffect;
            //    var loopTo = withBlock.Count;
            //    for (i = 1; i <= loopTo; i++)
            //        withBlock.Remove(1);
            //}
        }

        // スペシャルパワーの効果をユニット u にコピーする
        public void CopySpecialPowerInEffect(Unit u)
        {
            //foreach (Condition cnd in colSpecialPowerInEffect)
            //    u.MakeSpecialPowerInEffect(cnd.Name, cnd.StrData);
        }

    }
}
