using SRCCore.Lib;
using System;
using System.Collections.Generic;
using System.Text;

namespace SRCCore.Expressions.Functions
{
    public class X : AFunction
    {
        public override ValueType Invoke(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            switch (pcount)
            {
                case 1:
                    {
                        var pname = SRC.Expression.GetValueAsString(@params[1], is_term[1]);
                        switch (pname ?? "")
                        {
                            case "目標地点":
                                {
                                    num_result = (double)SRC.Commands.SelectedX;
                                    break;
                                }

                            case "マウス":
                                {
                                    num_result = (double)SRC.GUI.MouseX;
                                    break;
                                }

                            default:
                                {
                                    if (SRC.UList.IsDefined2(pname))
                                    {
                                        num_result = (double)SRC.UList.Item2(pname).x;
                                    }
                                    else if (SRC.PList.IsDefined(pname))
                                    {
                                        num_result = (double)SRC.PList.Item(pname).Unit.x;
                                    }

                                    break;
                                }
                        }

                        break;
                    }

                case 0:
                    {
                        if (SRC.Event.SelectedUnitForEvent != null)
                        {
                            num_result = (double)SRC.Event.SelectedUnitForEvent.x;
                        }

                        break;
                    }
            }

            if (etype == ValueType.StringType)
            {
                str_result = GeneralLib.FormatNum(num_result);
                return ValueType.StringType;
            }
            else
            {
                return ValueType.NumericType;
            }
        }
    }

    public class Y : AFunction
    {
        public override ValueType Invoke(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            switch (pcount)
            {
                case 1:
                    {
                        var pname = SRC.Expression.GetValueAsString(@params[1], is_term[1]);
                        switch (pname ?? "")
                        {
                            case "目標地点":
                                {
                                    num_result = (double)SRC.Commands.SelectedY;
                                    break;
                                }

                            case "マウス":
                                {
                                    num_result = (double)SRC.GUI.MouseY;
                                    break;
                                }

                            default:
                                {
                                    if (SRC.UList.IsDefined2(pname))
                                    {
                                        num_result = (double)SRC.UList.Item2(pname).y;
                                    }
                                    else if (SRC.PList.IsDefined(pname))
                                    {
                                        num_result = (double)SRC.PList.Item(pname).Unit.y;
                                    }

                                    break;
                                }
                        }

                        break;
                    }

                case 0:
                    {
                        if (SRC.Event.SelectedUnitForEvent != null)
                        {
                            num_result = (double)SRC.Event.SelectedUnitForEvent.y;
                        }

                        break;
                    }
            }

            if (etype == ValueType.StringType)
            {
                str_result = GeneralLib.FormatNum(num_result);
                return ValueType.StringType;
            }
            else
            {
                return ValueType.NumericType;
            }
        }
    }
}
