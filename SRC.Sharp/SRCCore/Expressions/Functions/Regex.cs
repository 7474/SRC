using System.Text.RegularExpressions;

namespace SRCCore.Expressions.Functions
{
    //RegExp���K�\���ŕ����������
    //RegExpReplace���K�\���Ō��������������u��

    // XXX �S�ʂɖ��a�ʂ��݊����������
    public class RegExp : AFunction
    {
        // XXX �����Ԃ͂����Ɏ����Ă��ē����̖��Ȃ�����������Ȃ�����Ƃ�������
        private MatchCollection lastMatch;
        private int lastMatchIndex;

        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            // RegExp(������, �p�^�[��[,�召��ʂ���|�召��ʂȂ�])
            var result = "";
            if (pcount > 0)
            {
                // ������S�̂�����
                // �啶���������̋�ʁiTrue=��ʂ��Ȃ��j
                var ignoreCase = false;
                if (pcount >= 3)
                {
                    if (SRC.Expression.GetValueAsString(@params[3], is_term[3]) == "�召��ʂȂ�")
                    {
                        ignoreCase = true;
                    }
                }
                // �����p�^�[��
                var regex = new Regex(SRC.Expression.GetValueAsString(@params[2], is_term[2]), ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
                lastMatch = regex.Matches(SRC.Expression.GetValueAsString(@params[1], is_term[1]));
                if (lastMatch.Count == 0)
                {
                    lastMatch = null;
                }
                else
                {
                    lastMatchIndex = 0;
                    result = lastMatch[lastMatchIndex].Value;
                }
            }
            else if (lastMatch != null)
            {
                lastMatchIndex++;
                if (lastMatch.Count > lastMatchIndex)
                {
                    result = lastMatch[lastMatchIndex].Value;
                }
            }

            str_result = result;
            return ValueType.StringType;
        }
    }

    public class RegExpReplace : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            // RegExpReplace(������, �����p�^�[��, �u���p�^�[��[,�召��ʂ���|�召��ʂȂ�])
            var ignoreCase = false;
            if (pcount >= 3)
            {
                if (SRC.Expression.GetValueAsString(@params[4], is_term[4]) == "�召��ʂȂ�")
                {
                    ignoreCase = true;
                }
            }
            // �����p�^�[��
            var regex = new Regex(SRC.Expression.GetValueAsString(@params[2], is_term[2]), ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);

            // �u�����s
            str_result = regex.Replace(SRC.Expression.GetValueAsString(@params[1], is_term[1]), SRC.Expression.GetValueAsString(@params[3], is_term[3]));

            return ValueType.StringType;
        }
    }
}