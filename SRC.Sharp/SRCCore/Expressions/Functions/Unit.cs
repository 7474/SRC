namespace SRCCore.Expressions.Functions
{
    public class Unit : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            switch (pcount)
            {
                case 1:
                    var pname = SRC.Expression.GetValueAsString(@params[1], is_term[1]);

                    if (SRC.UList.IsDefined2(pname))
                    {
                        str_result = SRC.UList.Item2(pname).Name;
                    }
                    else if (SRC.PList.IsDefined(pname))
                    {
                        str_result = SRC.PList.Item(pname)?.Unit?.Name ?? "";
                    }

                    break;

                case 0:
                    if (SRC.Event.SelectedUnitForEvent != null)
                    {
                        str_result = SRC.Event.SelectedUnitForEvent.Name;
                    }

                    break;
            }

            return ValueType.StringType;
        }
    }
}
