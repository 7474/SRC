using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.VB;
using System;
using System.Collections.Generic;

namespace SRCCore.CmdDatas.Commands
{
    public enum IfCmdType
    {
        Undefined,
        Then,
        Exit,
        GoTo,
    }

    public abstract class AIfCmd : CmdData
    {
        public IfCmdType IfCmdType { get; protected set; }
        protected string Expr { get; set; }
        // Exprの項の数、0なら単に式として評価するべきExpr。
        protected int ExprTermCount { get; set; }
        protected string GoToLabel { get; set; }

        public AIfCmd(SRC src, CmdType name, EventDataLine eventData) : base(src, name, eventData)
        {
        }

        protected void PrepareArgs()
        {
            // If文の処理の高速化のため、あらかじめ構文解析しておく
            if (ArgNum == 1)
            {
                // 書式エラー
                throw new EventErrorException(this, "Ifコマンドの書式に合っていません");
            }

            var ifCmdType = IfCmdType.Undefined;
            var terms = new List<string>();
            terms.Add(GetArg(2));
            for (var i = 3; i <= ArgNum; i++)
            {
                var buf = GetArg(i);
                switch (Strings.LCase(buf) ?? "")
                {
                    case "then":
                        ifCmdType = IfCmdType.Then;
                        break;
                    case "exit":
                        ifCmdType = IfCmdType.Exit;
                        break;

                    case "goto":
                        ifCmdType = IfCmdType.GoTo;
                        GoToLabel = GetArg(i + 1);
                        break;

                    default:
                        break;
                }
                if (ifCmdType != IfCmdType.Undefined) { break; }

                terms.Add(string.IsNullOrEmpty(buf) ? "\"\"" : buf);
            }

            if (ifCmdType == IfCmdType.Undefined)
            {
                throw new EventErrorException(this, (Name == CmdType.IfCmd ? "If" : "ElseIf") + "に対応する Then または Exit または Goto がありません");
            }

            // 条件式が式であることが確定していれば条件式の項数を0に
            switch (terms.Count)
            {
                case 0:
                    throw new EventErrorException(this, (Name == CmdType.IfCmd ? "If" : "ElseIf") + "コマンドの条件式がありません Then または Exit または Goto がありません");

                case 1:
                    switch (Strings.Asc(terms[0]))
                    {
                        case 36: // $
                            // 変数なら式
                            ExprTermCount = 0;
                            break;

                        case 40: // (
                            // カッコで囲われていたら式
                            // ()を除去
                            terms[0] = Strings.Mid(terms[0], 2, Strings.Len(terms[0]) - 2);
                            ExprTermCount = 0;
                            break;
                        default:
                            // そうでないならパイロット指定
                            break;
                    }

                    break;

                case 2:
                    if (Strings.LCase(terms[0]) == "not")
                    {
                        switch (Strings.Asc(terms[1]))
                        {
                            case 36:
                            case 40: // $, (
                                // 変数かカッコで囲われていたら式
                                ExprTermCount = 0;
                                break;
                            default:
                                // そうでないならパイロット指定
                                break;
                        }
                    }
                    else
                    {
                        // 1つ目の項がNotでないなら式
                        ExprTermCount = 0;
                    }
                    break;

                default:
                    ExprTermCount = 0;
                    break;
            }
            Expr = string.Join(" ", terms);
        }

        internal static int ToEnd(CmdData elseCmd)
        {
            // EndIfを探す
            var depth = 1;
            for (var i = elseCmd.EventData.ID + 1; i <= elseCmd.Event.EventCmd.Count; i++)
            {
                var cmd = elseCmd.Event.EventCmd[i];
                switch (cmd.Name)
                {
                    case CmdType.IfCmd:
                        if ((cmd as AIfCmd).IfCmdType == IfCmdType.Then)
                        {
                            depth = depth + 1;
                        }
                        break;

                    case CmdType.EndIfCmd:
                        depth = depth - 1;
                        if (depth == 0)
                        {
                            return i + 1;
                        }
                        break;
                }
            }

            throw new EventErrorException(elseCmd, "IfとEndIfが対応していません");
        }
    }
}
