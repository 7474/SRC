
using SRCCore.Expressions;
using System.Collections.Generic;
using System.Linq;
using System;

namespace SRCCore.Extensions
{
    public static class VarDataExtension
    {
        public static IEnumerable<VarData> ArrayByName(this IEnumerable<VarData> vars, string vname)
        {
            return vars.Where(x => x.Name.StartsWith(vname + "[", StringComparison.Ordinal));
        }

        public static IEnumerable<string> ArrayIndexesByName(this IEnumerable<VarData> vars, string vname)
        {
            return vars.ArrayByName(vname)
                .Select(x => x.Name.ArrayIndexByName())
                .Where(x => !string.IsNullOrEmpty(x));
        }
    }
}
