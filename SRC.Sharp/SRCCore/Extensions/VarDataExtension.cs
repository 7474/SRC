
using SRCCore.Expressions;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Extensions
{
    public static class VarDataExtension
    {
        public static IEnumerable<VarData> ArrayByName(this IEnumerable<VarData> vars, string vname)
        {
            return vars.Where(x => x.Name.StartsWith(vname + "["));
        }

        public static IEnumerable<string> ArrayIndexesByName(this IEnumerable<VarData> vars, string vname)
        {
            return vars.ArrayByName(vname)
                .Select(x => x.Name.ArrayIndexByName())
                .Where(x => !string.IsNullOrEmpty(x));
        }
    }
}
