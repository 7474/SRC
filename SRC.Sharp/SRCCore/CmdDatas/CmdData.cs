// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Lib;
using SRCCore.Units;
using SRCCore.VB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.CmdDatas
{
    // イベントコマンドのクラス
    public abstract partial class CmdData
    {
        protected SRC SRC { get; }
        protected IGUI GUI => SRC.GUI;
        internal Event Event => SRC.Event;
        protected Expressions.Expression Expression => SRC.Expression;
        protected Maps.Map Map => SRC.Map;
        protected SRCCore.Commands.Command Commands => SRC.Commands;
        internal Sound Sound => SRC.Sound;

        public CmdData(SRC src, CmdType name, EventDataLine eventData)
        {
            SRC = src;
            Name = name;
            EventData = eventData;
            ParseArgs(eventData.Data);
        }

        // コマンドの種類
        public CmdType Name { get; private set; }

        // コマンドのEventData
        public EventDataLine EventData { get; }

        // 引数の数（コマンド名も含む点に留意すること）
        public int ArgNum { get; private set; }

        // 引数
        private IList<CmdArgument> args = new List<CmdArgument>();

        // コマンドを実行し、実行後のEventDataLine#IDを返す
        public int Exec()
        {
            SRC.LogDebug(Name.ToString(), args.Select(x => x.strArg).ToArray());
            try
            {
                return ExecInternal();
            }
            catch (EventErrorException ex)
            {
                Event.DisplayEventErrorMessage(ex?.EventData.ID ?? EventData.ID, ex.Message);
                return -1;
            }
            catch
            {
                // TODO Impl
                if (Strings.LCase(GeneralLib.ListIndex(EventData.Data, 1)) == "talk")
                {
                    Event.DisplayEventErrorMessage(EventData.ID, "Talkコマンド実行中に不正な処理が行われました。" + "MIDIがソフトウェアシンセサイザで演奏されているか、" + "フォントキャッシュが壊れている可能性があります。" + "詳しくはSRC公式ホームページの「よくある質問集」をご覧下さい。");
                }
                else if (Strings.LCase(GeneralLib.ListIndex(EventData.Data, 1)) == "autotalk")
                {
                    Event.DisplayEventErrorMessage(EventData.ID, "AutoTalkコマンド実行中に不正な処理が行われました。" + "MIDIがソフトウェアシンセサイザで演奏されているか、" + "フォントキャッシュが壊れている可能性があります。" + "詳しくはSRC公式ホームページの「よくある質問集」をご覧下さい。");
                }
                else
                {
                    Event.DisplayEventErrorMessage(EventData.ID, "イベントデータが不正です");
                }
                return -1;
            }
        }

        protected abstract int ExecInternal();

        // idx番目の引数を返す
        public CmdArgument GetArgRaw(int idx)
        {
            if (idx - 1 < args.Count)
            {
                // オフセット分引いた値を返す
                return args[idx - 1];
            }
            else
            {
                return CmdArgument.Empty;
            }
        }

        // idx番目の引数を式として評価せずにそのまま返す
        public string GetArg(int idx)
        {
            return GetArgRaw(idx).strArg;
        }

        // idx番目の引数の値を文字列として返す
        public string GetArgAsString(int idx)
        {
            var arg = GetArgRaw(idx);
            switch (arg.argType)
            {
                case Expressions.ValueType.UndefinedType:
                    return Expression.GetValueAsString(arg.strArg, true);

                case Expressions.ValueType.StringType:
                    return arg.strArg;

                case Expressions.ValueType.NumericType:
                    return SrcFormatter.Format(arg.dblArg);
                default:
                    throw new InvalidOperationException();
            }
        }

        // idx番目の引数の値をLongとして返す
        public int GetArgAsLong(int idx)
        {
            var arg = GetArgRaw(idx);
            switch (arg.argType)
            {
                case Expressions.ValueType.UndefinedType:
                    return Expression.GetValueAsLong(arg.strArg, true);

                case Expressions.ValueType.StringType:
                    return 0;

                case Expressions.ValueType.NumericType:
                    return arg.lngArg;
                default:
                    throw new InvalidOperationException();
            }
        }

        // idx番目の引数の値をDoubleとして返す
        public double GetArgAsDouble(int idx)
        {
            var arg = GetArgRaw(idx);
            switch (arg.argType)
            {
                case Expressions.ValueType.UndefinedType:
                    return Expression.GetValueAsDouble(arg.strArg, true);

                case Expressions.ValueType.StringType:
                    return 0d;

                case Expressions.ValueType.NumericType:
                    return arg.dblArg;
                default:
                    throw new InvalidOperationException();
            }
        }

        // idx番目の引数が示すユニットを返す
        public Unit GetArgAsUnit(int idx, bool ignore_error = false)
        {
            string pname = GetArgAsString(idx);
            Unit GetArgAsUnitRet = SRC.UList.Item2(pname);
            if (GetArgAsUnitRet is null)
            {
                if (!SRC.PList.IsDefined(pname))
                {
                    throw new EventErrorException(this, "「" + pname + "」というパイロットが見つかりません");
                }

                GetArgAsUnitRet = SRC.PList.Item(pname).Unit;
                if (!ignore_error)
                {
                    if (GetArgAsUnitRet is null)
                    {
                        throw new EventErrorException(this, "「" + pname + "」はユニットに乗っていません");
                    }
                }
            }

            return GetArgAsUnitRet;
        }

        private void ParseArgs(string list)
        {
            string[] rawArgs;
            ArgNum = GeneralLib.ListSplit(list, out rawArgs);

            // コマンド名も含める
            foreach (var buf in rawArgs)
            {
                var arg = new CmdArgument()
                {
                    strArg = buf,
                    argType = Expressions.ValueType.UndefinedType,
                };
                args.Add(arg);

                // 先頭の一文字からパラメータの属性を判定
                switch (Strings.Asc(buf))
                {
                    case 0: // 空文字列
                        {
                            arg.argType = Expressions.ValueType.StringType;
                            break;
                        }

                    case 34: // "
                        {
                            if (Strings.Right(buf, 1) == "\"")
                            {
                                if (Strings.InStr(buf, "$(") == 0)
                                {
                                    arg.argType = Expressions.ValueType.StringType;
                                    arg.strArg = Strings.Mid(buf, 2, Strings.Len(buf) - 2);
                                }
                            }
                            else
                            {
                                arg.argType = Expressions.ValueType.StringType;
                            }

                            break;
                        }

                    case 40: // (
                        {
                            break;
                        }
                    // 式
                    case 45: // -
                        {
                            if (Information.IsNumeric(buf))
                            {
                                arg.lngArg = GeneralLib.StrToLng(buf);
                                arg.dblArg = Conversions.ToDouble(buf);
                                arg.argType = Expressions.ValueType.NumericType;
                            }
                            else
                            {
                                arg.argType = Expressions.ValueType.StringType;
                            }

                            break;
                        }

                    case var @case when 48 <= @case && @case <= 57: // 0～9
                        {
                            if (Information.IsNumeric(buf))
                            {
                                arg.lngArg = GeneralLib.StrToLng(buf);
                                arg.dblArg = Conversions.ToDouble(buf);
                                arg.argType = Expressions.ValueType.NumericType;
                            }
                            else
                            {
                                arg.argType = Expressions.ValueType.StringType;
                            }

                            break;
                        }

                    case 96: // `
                        {
                            if (Strings.Right(buf, 1) == "`")
                            {
                                arg.strArg = Strings.Mid(buf, 2, Strings.Len(buf) - 2);
                            }

                            arg.argType = Expressions.ValueType.StringType;
                            break;
                        }
                }
            }
        }

        public IEnumerable<int> AfterEventIdRange()
        {
            var start = EventData.ID + 1;
            return Enumerable.Range(start, Event.EventCmd.Count - start);
        }

        public IEnumerable<int> BeforeEventIdRange()
        {
            var start = EventData.ID - 1;
            return Enumerable.Range(0, start).Select(x => start - x);
        }
    }
}
