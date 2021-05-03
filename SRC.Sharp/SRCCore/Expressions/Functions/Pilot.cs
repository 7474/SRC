using SRCCore.Lib;
using System.Linq;

namespace SRCCore.Expressions.Functions
{
    //�p�C���b�g���֐�
    //Level�p�C���b�g�̃��x��
    //Morale�p�C���b�g�̋C��
    //Plana�p�C���b�g�̎c����
    //Relation�p�C���b�g�Ԃ̐M���x
    //Skill�p�C���b�g���w�肵������\�͂������Ă��邩
    //SP�p�C���b�g�̎c��r�o

    public class Level : APilotFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, Pilots.Pilot pilot, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = pilot?.Level ?? 0d;

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

    public class Morale : APilotFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, Pilots.Pilot pilot, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = pilot?.Morale ?? 0d;

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

    public class Plana : APilotFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, Pilots.Pilot pilot, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = pilot?.Plana ?? 0d;

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

    public class Relation : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            // TODO Impl Relation

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

    public class Skill : APilotFunction
    {
        protected override int OptionArgCount => 1;
        protected override ValueType InvokeInternal(SRC SRC, Pilots.Pilot pilot, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;
            var name = pcount == 1
                ? SRC.Expression.GetValueAsString(@params[1], is_term[1])
                : SRC.Expression.GetValueAsString(@params[2], is_term[2]);
            // �G���A�X����`����Ă���H
            if (SRC.ALDList.IsDefined(name))
            {
                var alias = SRC.ALDList.Item(name);
                var aliasElem = alias.Elements.FirstOrDefault(x => GeneralLib.LIndex(x.strAliasData, 1) == name);

                if (aliasElem != null)
                {
                    name = aliasElem.strAliasType;
                }
                else
                {
                    name = alias.Elements.First().strAliasType;
                }
            }
            num_result = pilot?.SkillLevel(name) ?? 0d;
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

    public class SP : APilotFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, Pilots.Pilot pilot, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = pilot?.SP ?? 0d;

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