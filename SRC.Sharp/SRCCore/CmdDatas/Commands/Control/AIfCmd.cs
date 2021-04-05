using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Lib;
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
        public string Expr { get; protected set; }
        // Exprの項の数、0なら単に式として評価するべきExpr。
        public int ExprTermCount { get; protected set; }
        public string GoToLabel { get; protected set; }

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
            IfCmdType = ifCmdType;

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

        public bool Evaluate()
        {
            // Ifコマンドはあらかじめ構文解析されていて、条件式の項数が入っている
            bool flag;
            switch (ExprTermCount)
            {
                case 1:
                    if (SRC.PList.IsDefined(Expr))
                    {
                        var p = SRC.PList.Item(Expr);
                        if (p.Unit is null)
                        {
                            flag = false;
                        }
                        else
                        {
                            {
                                var withBlock1 = p.Unit;
                                if (withBlock1.Status == "出撃" | withBlock1.Status == "格納")
                                {
                                    flag = true;
                                }
                                else
                                {
                                    flag = false;
                                }
                            }
                        }
                    }
                    else if (Expression.GetValueAsLong(Expr, true) != 0)
                    {
                        flag = true;
                    }
                    else
                    {
                        flag = false;
                    }
                    break;

                case 2:
                    var pname = GeneralLib.ListIndex(Expr, 2);
                    if (SRC.PList.IsDefined(pname))
                    {
                        {
                            var p = SRC.PList.Item(pname);
                            if (p.Unit is null)
                            {
                                flag = true;
                            }
                            else
                            {
                                if (p.Unit.Status == "出撃" | p.Unit.Status == "格納")
                                {
                                    flag = false;
                                }
                                else
                                {
                                    flag = true;
                                }
                            }
                        }
                    }
                    else if (Expression.GetValueAsLong(pname, true) == 0)
                    {
                        flag = true;
                    }
                    else
                    {
                        flag = false;
                    }
                    break;

                default:
                    if (Expression.GetValueAsLong(Expr) != 0)
                    {
                        flag = true;
                    }
                    else
                    {
                        flag = false;
                    }
                    break;
            }

            return flag;
        }

        internal static int ToEnd(CmdData elseCmd)
        {
            // EndIfを探す
            var depth = 1;
            foreach (var i in elseCmd.AfterEventIdRange())
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
