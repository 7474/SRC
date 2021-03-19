using SRCCore.Maps;
using System;

namespace SRCCore.Events
{
    public partial class Event
    {
        // イベントデータの消去
        // ただしグローバル変数のデータは残しておく
        public static void ClearEventData()
        {
            short i;

            // UPGRADE_NOTE: オブジェクト SelectedUnitForEvent をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            SelectedUnitForEvent = null;
            // UPGRADE_NOTE: オブジェクト SelectedTargetForEvent をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            SelectedTargetForEvent = null;
            Array.Resize(ref EventData, SysEventDataSize + 1);
            {
                var withBlock = colNormalLabelList;
                var loopTo = (short)withBlock.Count;
                for (i = 1; i <= loopTo; i++)
                    withBlock.Remove(1);
            }

            i = 1;
            {
                var withBlock1 = colEventLabelList;
                while (i <= withBlock1.Count)
                {
                    // UPGRADE_WARNING: オブジェクト colEventLabelList.Item(i).LineNum の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    if (Conversions.ToBoolean(Operators.ConditionalCompareObjectGreater(withBlock1[(int)i].LineNum, SysEventDataSize, false)))
                    {
                        withBlock1.Remove(i);
                    }
                    else
                    {
                        i = (short)(i + 1);
                    }
                }
            }

            EventQue = new string[1];
            CallDepth = 0;
            ArgIndex = 0;
            VarIndex = 0;
            ForIndex = 0;
            UpVarLevel = 0;
            HotPointList = new HotPoint[1];
            ObjColor = ColorTranslator.ToOle(Color.White);
            ObjFillColor = ColorTranslator.ToOle(Color.White);
            // UPGRADE_ISSUE: 定数 vbFSTransparent はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
            ObjFillStyle = vbFSTransparent;
            ObjDrawWidth = 1;
            ObjDrawOption = "";
            GUI.IsPictureVisible = false;
            GUI.IsCursorVisible = false;
            GUI.PaintedAreaX1 = GUI.MainPWidth;
            GUI.PaintedAreaY1 = GUI.MainPHeight;
            GUI.PaintedAreaX2 = -1;
            GUI.PaintedAreaY2 = -1;
            {
                var withBlock2 = LocalVariableList;
                var loopTo1 = (short)withBlock2.Count;
                for (i = 1; i <= loopTo1; i++)
                    withBlock2.Remove(1);
            }
        }

        // グローバル変数を含めたイベントデータの全消去
        public static void ClearAllEventData()
        {
            short i;
            ClearEventData();
            {
                var withBlock = GlobalVariableList;
                var loopTo = (short)withBlock.Count;
                for (i = 1; i <= loopTo; i++)
                    withBlock.Remove(1);
            }

            string argvname = "次ステージ";
            Expression.DefineGlobalVariable(ref argvname);
            string argvname1 = "セーブデータファイル名";
            Expression.DefineGlobalVariable(ref argvname1);
        }

        // 一時中断用データをファイルにセーブする
        public static void DumpEventData()
        {
            short i;

            // グローバル変数
            SaveGlobalVariables();
            // ローカル変数
            SaveLocalVariables();

            // イベント用ラベル
            FileSystem.WriteLine(SRC.SaveDataFileNumber, (object)colEventLabelList.Count);
            foreach (LabelData lab in colEventLabelList)
                FileSystem.WriteLine(SRC.SaveDataFileNumber, (object)lab.Enable);

            // Requireコマンドで追加されたイベントファイル
            FileSystem.WriteLine(SRC.SaveDataFileNumber, (object)Information.UBound(AdditionalEventFileNames));
            var loopTo = (short)Information.UBound(AdditionalEventFileNames);
            for (i = 1; i <= loopTo; i++)
                FileSystem.WriteLine(SRC.SaveDataFileNumber, AdditionalEventFileNames[i]);
        }

        // 一時中断用データをファイルからロードする
        public static void RestoreEventData()
        {
            var num = default(short);
            bool lenable;
            var fname = default(string);
            int file_head;
            int i;
            short j;
            string buf;

            // グローバル変数
            LoadGlobalVariables();
            // ローカル変数
            LoadLocalVariables();

            // イベント用ラベル
            FileSystem.Input(SRC.SaveDataFileNumber, ref num);
            // MOD START MARGE
            // i = 1
            // For Each lab In colEventLabelList
            // If i <= num Then
            // Input #SaveDataFileNumber, lenable
            // lab.Enable = lenable
            // Else
            // lab.Enable = True
            // End If
            // i = i + 1
            // Next
            // Do While i <= num
            // Input #SaveDataFileNumber, buf
            // i = i + 1
            // Loop
            var label_enabled = new object[(num + 1)];
            var loopTo = (int)num;
            for (i = 1; i <= loopTo; i++)
                FileSystem.Input(SRC.SaveDataFileNumber, ref label_enabled[i]);
            // MOD END MARGE

            // Requireコマンドで追加されたイベントファイル
            if (SRC.SaveDataVersion > 20003)
            {
                file_head = Information.UBound(EventData) + 1;

                // MOD START MARGE
                // 'イベントファイルをロード
                // Input #SaveDataFileNumber, num
                // If num = 0 Then
                // Exit Sub
                // End If
                // ReDim AdditionalEventFileNames(num)
                // For i = 1 To num
                // Input #SaveDataFileNumber, fname
                // AdditionalEventFileNames(i) = fname
                // If InStr(fname, ":") = 0 Then
                // fname = ScenarioPath & fname
                // End If
                // 
                // '既に読み込まれている場合はスキップ
                // For j = 1 To UBound(EventFileNames)
                // If fname = EventFileNames(j) Then
                // GoTo NextEventFile
                // End If
                // Next
                // 
                // LoadEventData2 fname, UBound(EventData)
                // NextEventFile:
                // Next
                // 
                // 'エラー表示用にサイズを大きく取っておく
                // ReDim Preserve EventData(UBound(EventData) + 1)
                // ReDim Preserve EventLineNum(UBound(EventData))
                // EventData(UBound(EventData)) = ""
                // EventLineNum(UBound(EventData)) = EventLineNum(UBound(EventData) - 1) + 1
                // 
                // '複数行に分割されたコマンドを結合
                // For i = file_head To UBound(EventData) - 1
                // If Right$(EventData(i), 1) = "_" Then
                // EventData(i + 1) = _
                // '                    Left$(EventData(i), Len(EventData(i)) - 1) & EventData(i + 1)
                // EventData(i) = " "
                // End If
                // Next
                // 
                // 'ラベルを登録
                // For i = file_head To UBound(EventData)
                // buf = EventData(i)
                // If Right$(buf, 1) = ":" Then
                // AddLabel Left$(buf, Len(buf) - 1), i
                // End If
                // Next
                // 
                // 'コマンドデータ配列を設定
                // If UBound(EventData) > UBound(EventCmd) Then
                // ReDim Preserve EventCmd(UBound(EventData))
                // i = UBound(EventData)
                // Do While EventCmd(i) Is Nothing
                // Set EventCmd(i) = New CmdData
                // EventCmd(i).LineNum = i
                // i = i - 1
                // Loop
                // End If
                // For i = file_head To UBound(EventData)
                // EventCmd(i).Name = NullCmd
                // Next
                // End If
                // 追加するイベントファイル数
                FileSystem.Input(SRC.SaveDataFileNumber, ref num);
                if (num > 0)
                {
                    // イベントファイルをロード
                    AdditionalEventFileNames = new string[(num + 1)];
                    var loopTo1 = (int)num;
                    for (i = 1; i <= loopTo1; i++)
                    {
                        FileSystem.Input(SRC.SaveDataFileNumber, ref fname);
                        AdditionalEventFileNames[i] = fname;
                        if (Strings.InStr(fname, ":") == 0)
                        {
                            fname = SRC.ScenarioPath + fname;
                        }

                        // 既に読み込まれている場合はスキップ
                        var loopTo2 = (short)Information.UBound(EventFileNames);
                        for (j = 1; j <= loopTo2; j++)
                        {
                            if ((fname ?? "") == (EventFileNames[j] ?? ""))
                            {
                                goto NextEventFile;
                            }
                        }

                        LoadEventData2(ref fname, Information.UBound(EventData));
                        NextEventFile:
                        ;
                    }

                    // エラー表示用にサイズを大きく取っておく
                    Array.Resize(ref EventData, Information.UBound(EventData) + 1 + 1);
                    Array.Resize(ref EventLineNum, Information.UBound(EventData) + 1);
                    EventData[Information.UBound(EventData)] = "";
                    EventLineNum[Information.UBound(EventData)] = (short)(EventLineNum[Information.UBound(EventData) - 1] + 1);

                    // 複数行に分割されたコマンドを結合
                    var loopTo3 = Information.UBound(EventData) - 1;
                    for (i = file_head; i <= loopTo3; i++)
                    {
                        if (Strings.Right(EventData[i], 1) == "_")
                        {
                            EventData[i + 1] = Strings.Left(EventData[i], Strings.Len(EventData[i]) - 1) + EventData[i + 1];
                            EventData[i] = " ";
                        }
                    }

                    // ラベルを登録
                    var loopTo4 = Information.UBound(EventData);
                    for (i = file_head; i <= loopTo4; i++)
                    {
                        buf = EventData[i];
                        if (Strings.Right(buf, 1) == ":")
                        {
                            string arglname = Strings.Left(buf, Strings.Len(buf) - 1);
                            AddLabel(ref arglname, i);
                        }
                    }

                    // コマンドデータ配列を設定
                    if (Information.UBound(EventData) > Information.UBound(EventCmd))
                    {
                        Array.Resize(ref EventCmd, Information.UBound(EventData) + 1);
                        i = Information.UBound(EventData);
                        while (EventCmd[i] is null)
                        {
                            EventCmd[i] = new CmdData();
                            EventCmd[i].LineNum = i;
                            i = i - 1;
                        }
                    }

                    var loopTo5 = Information.UBound(EventData);
                    for (i = file_head; i <= loopTo5; i++)
                        EventCmd[i].Name = CmdType.NullCmd;
                }
            }

            // イベント用ラベルを設定
            i = 1;
            num = (short)Information.UBound(label_enabled);
            foreach (LabelData lab in colEventLabelList)
            {
                if (i <= num)
                {
                    // UPGRADE_WARNING: オブジェクト label_enabled(i) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    lab.Enable = Conversions.ToBoolean(label_enabled[i]);
                }
                else
                {
                    lab.Enable = true;
                }

                i = i + 1;
            }
            // MOD END MARGE
        }

        // 一時中断用データのイベントデータ部分を読み飛ばす
        public static void SkipEventData()
        {
            short i, num = default;
            string dummy;

            // グローバル変数
            FileSystem.Input(SRC.SaveDataFileNumber, ref num);
            var loopTo = num;
            for (i = 1; i <= loopTo; i++)
                dummy = FileSystem.LineInput(SRC.SaveDataFileNumber);
            // ローカル変数
            FileSystem.Input(SRC.SaveDataFileNumber, ref num);
            var loopTo1 = num;
            for (i = 1; i <= loopTo1; i++)
                dummy = FileSystem.LineInput(SRC.SaveDataFileNumber);

            // ラベル情報
            FileSystem.Input(SRC.SaveDataFileNumber, ref num);
            var loopTo2 = num;
            for (i = 1; i <= loopTo2; i++)
                dummy = FileSystem.LineInput(SRC.SaveDataFileNumber);

            // Requireコマンドで読み込んだイベントデータ
            if (SRC.SaveDataVersion > 20003)
            {
                FileSystem.Input(SRC.SaveDataFileNumber, ref num);
                var loopTo3 = num;
                for (i = 1; i <= loopTo3; i++)
                    dummy = FileSystem.LineInput(SRC.SaveDataFileNumber);
            }
        }

        // グローバル変数をファイルにセーブ
        public static void SaveGlobalVariables()
        {
            FileSystem.WriteLine(SRC.SaveDataFileNumber, (object)GlobalVariableList.Count);
            foreach (VarData var in GlobalVariableList)
            {
                if (var.VariableType == Expression.ValueType.StringType)
                {
                    FileSystem.WriteLine(SRC.SaveDataFileNumber, var.Name, var.StringValue);
                }
                else
                {
                    FileSystem.WriteLine(SRC.SaveDataFileNumber, var.Name, Microsoft.VisualBasic.Compatibility.VB6.Support.Format(var.NumericValue));
                }
            }
        }

        // グローバル変数をファイルからロード
        public static void LoadGlobalVariables()
        {
            short num = default, j, i, k, idx;
            string vvalue, vname = default, buf;
            string aname;
            // ADD START MARGE
            bool is_number;
            // ADD END MARGE
            // グローバル変数を全削除
            {
                var withBlock = GlobalVariableList;
                var loopTo = (short)withBlock.Count;
                for (i = 1; i <= loopTo; i++)
                    withBlock.Remove(1);
            }

            // グローバル変数の総数を読み出し
            FileSystem.Input(SRC.SaveDataFileNumber, ref num);

            // 各変数の値を読み出し
            string vname2;
            var loopTo1 = num;
            for (i = 1; i <= loopTo1; i++)
            {
                FileSystem.Input(SRC.SaveDataFileNumber, ref vname);
                buf = FileSystem.LineInput(SRC.SaveDataFileNumber);
                // MOD START MARGE
                // vvalue = Mid$(buf, 2, Len(buf) - 2)
                // ReplaceString vvalue, """""", """"
                if (Strings.Left(buf, 1) == "\"")
                {
                    is_number = false;
                    vvalue = Strings.Mid(buf, 2, Strings.Len(buf) - 2);
                    string args2 = "\"\"";
                    string args3 = "\"";
                    GeneralLib.ReplaceString(ref vvalue, ref args2, ref args3);
                }
                else
                {
                    is_number = true;
                    vvalue = buf;
                }
                // MOD END MARGE

                if (SRC.SaveDataVersion < 10724)
                {
                    // SetSkillコマンドのセーブデータをエリアスに対応させる
                    if (Strings.Left(vname, 8) == "Ability(")
                    {
                        idx = (short)Strings.InStr(vname, ",");
                        if (idx > 0)
                        {
                            // 個々の能力定義
                            aname = Strings.Mid(vname, idx + 1, Strings.Len(vname) - idx - 1);
                            object argIndex1 = aname;
                            if (SRC.ALDList.IsDefined(ref argIndex1))
                            {
                                AliasDataType localItem() { object argIndex1 = aname; var ret = SRC.ALDList.Item(ref argIndex1); return ret; }

                                vname = Strings.Left(vname, idx) + localItem().get_AliasType(1) + ")";
                                if (GeneralLib.LLength(ref vvalue) == 1)
                                {
                                    vvalue = vvalue + " " + aname;
                                }
                            }
                        }
                        else
                        {
                            // 必要技能用の能力一覧
                            buf = "";
                            var loopTo2 = GeneralLib.LLength(ref vvalue);
                            for (j = 1; j <= loopTo2; j++)
                            {
                                aname = GeneralLib.LIndex(ref vvalue, j);
                                object argIndex2 = aname;
                                if (SRC.ALDList.IsDefined(ref argIndex2))
                                {
                                    AliasDataType localItem1() { object argIndex1 = aname; var ret = SRC.ALDList.Item(ref argIndex1); return ret; }

                                    aname = localItem1().get_AliasType(1);
                                }

                                buf = buf + " " + aname;
                            }

                            vvalue = Strings.Trim(buf);
                        }
                    }
                }

                if (SRC.SaveDataVersion < 10730)
                {
                    // ラーニングした特殊能力が使えないバグに対応
                    if (Strings.Left(vname, 8) == "Ability(")
                    {
                        idx = (short)Strings.InStr(vname, ",");
                        if (idx > 0)
                        {
                            vname2 = Strings.Left(vname, idx - 1) + ")";
                            aname = Strings.Mid(vname, idx + 1, Strings.Len(vname) - idx - 1);
                            if (!Expression.IsGlobalVariableDefined(ref vname2))
                            {
                                Expression.DefineGlobalVariable(ref vname2);
                                // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                GlobalVariableList[vname2].StringValue = aname;
                            }
                        }
                    }
                }

                if (SRC.SaveDataVersion < 10731)
                {
                    // 不必要な非表示能力に対するSetSkillを削除
                    if (Strings.Left(vname, 8) == "Ability(")
                    {
                        if (Strings.Right(vname, 5) == ",非表示)")
                        {
                            goto NextVariable;
                        }
                    }
                }

                if (SRC.SaveDataVersion < 10732)
                {
                    // 不必要な非表示能力に対するSetSkillと能力名のダブりを削除
                    if (Strings.Left(vname, 8) == "Ability(")
                    {
                        if (Strings.InStr(vname, ",") == 0)
                        {
                            buf = "";
                            var loopTo3 = GeneralLib.LLength(ref vvalue);
                            for (j = 1; j <= loopTo3; j++)
                            {
                                aname = GeneralLib.LIndex(ref vvalue, j);
                                if (aname != "非表示")
                                {
                                    var loopTo4 = GeneralLib.LLength(ref buf);
                                    for (k = 1; k <= loopTo4; k++)
                                    {
                                        if ((GeneralLib.LIndex(ref buf, k) ?? "") == (aname ?? ""))
                                        {
                                            break;
                                        }
                                    }

                                    if (k > GeneralLib.LLength(ref buf))
                                    {
                                        buf = buf + " " + aname;
                                    }
                                }
                            }

                            vvalue = Strings.Trim(buf);
                        }
                    }
                }

                if (SRC.SaveDataVersion < 20027)
                {
                    // エリアスされた能力をSetSkillした際にエリアスに含まれる解説が無効になるバグへの対処
                    if (Strings.Left(vname, 8) == "Ability(")
                    {
                        if (GeneralLib.LIndex(ref vvalue, 1) == "0")
                        {
                            if (GeneralLib.LIndex(ref vvalue, 2) == "解説")
                            {
                                vvalue = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(SRC.DEFAULT_LEVEL) + " 解説 " + GeneralLib.ListTail(ref vvalue, 3);
                            }
                        }
                    }
                }

                if (!Expression.IsGlobalVariableDefined(ref vname))
                {
                    Expression.DefineGlobalVariable(ref vname);
                }

                {
                    var withBlock1 = GlobalVariableList[vname];
                    // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    withBlock1.StringValue = vvalue;
                    // MOD START MARGE
                    // If IsNumber(vvalue) Then
                    if (is_number)
                    {
                        // MOD END MARGE
                        // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().VariableType の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        withBlock1.VariableType = Expression.ValueType.NumericType;
                        // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item(vname).NumericValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        withBlock1.NumericValue = Conversions.ToDouble(vvalue);
                    }
                    else
                    {
                        // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().VariableType の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        withBlock1.VariableType = Expression.ValueType.StringType;
                    }
                }

                NextVariable:
                ;
            }
            // ADD START 240a
            // Optionを全て読み込んだら、新ＧＵＩが有効になっているか確認する
            GUI.SetNewGUIMode();
            // ADD  END  240a
        }

        // ローカル変数をファイルにセーブ
        public static void SaveLocalVariables()
        {
            FileSystem.WriteLine(SRC.SaveDataFileNumber, (object)LocalVariableList.Count);
            foreach (VarData var in LocalVariableList)
            {
                if (var.VariableType == Expression.ValueType.StringType)
                {
                    FileSystem.WriteLine(SRC.SaveDataFileNumber, var.Name, var.StringValue);
                }
                else
                {
                    FileSystem.WriteLine(SRC.SaveDataFileNumber, var.Name, Microsoft.VisualBasic.Compatibility.VB6.Support.Format(var.NumericValue));
                }

                if (Strings.InStr(var.Name, "\"") > 0)
                {
                    GUI.ErrorMessage(ref var.Name);
                }
            }
        }

        // ローカル変数をファイルからロード
        public static void LoadLocalVariables()
        {
            short i, num = default;
            // MOD START MARGE
            // Dim vname As String, vvalue As String
            string vvalue, vname = default, buf;
            bool is_number;
            // MOD END MARGE
            // ローカル変数を全削除
            {
                var withBlock = LocalVariableList;
                var loopTo = (short)withBlock.Count;
                for (i = 1; i <= loopTo; i++)
                    withBlock.Remove(1);
            }

            // ローカル変数の総数を読み出し
            FileSystem.Input(SRC.SaveDataFileNumber, ref num);
            var loopTo1 = num;
            for (i = 1; i <= loopTo1; i++)
            {
                // 変数の値を読み出し
                // MOD START MARGE
                // Input #SaveDataFileNumber, vname, vvalue
                FileSystem.Input(SRC.SaveDataFileNumber, ref vname);
                buf = FileSystem.LineInput(SRC.SaveDataFileNumber);
                if (Strings.Left(buf, 1) == "\"")
                {
                    is_number = false;
                    vvalue = Strings.Mid(buf, 2, Strings.Len(buf) - 2);
                    string args2 = "\"\"";
                    string args3 = "\"";
                    GeneralLib.ReplaceString(ref vvalue, ref args2, ref args3);
                }
                else
                {
                    is_number = true;
                    vvalue = buf;
                }
                // MOD END MARGE

                if (SRC.SaveDataVersion < 10731)
                {
                    // ClearSkillのバグで設定された変数を削除
                    if (Strings.Left(vname, 8) == "Ability(")
                    {
                        if ((vname ?? "") == (vvalue ?? ""))
                        {
                            goto NextVariable;
                        }
                    }
                }

                // 変数の値を設定
                if (!Expression.IsLocalVariableDefined(ref vname))
                {
                    Expression.DefineLocalVariable(ref vname);
                }

                {
                    var withBlock1 = LocalVariableList[vname];
                    // UPGRADE_WARNING: オブジェクト LocalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    withBlock1.StringValue = vvalue;
                    // MOD START MARGE
                    // If IsNumber(vvalue) Then
                    if (is_number)
                    {
                        // MOD END MARGE
                        // UPGRADE_WARNING: オブジェクト LocalVariableList.Item().VariableType の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        withBlock1.VariableType = Expression.ValueType.NumericType;
                        // UPGRADE_WARNING: オブジェクト LocalVariableList.Item(vname).NumericValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        withBlock1.NumericValue = Conversions.ToDouble(vvalue);
                    }
                    else
                    {
                        // UPGRADE_WARNING: オブジェクト LocalVariableList.Item().VariableType の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        withBlock1.VariableType = Expression.ValueType.StringType;
                    }
                }

                NextVariable:
                ;
            }
        }
    }
}