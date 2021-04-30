using System.IO;
using System.Linq;

namespace Template
{
    class Program
    {
        static void Main(string[] args)
        {
            var type = args.First();
            var template = File.ReadAllText($"{type}Template.cs.txt");
            if (type == "Function")
            {
                File.WriteAllText(
                    Path.Combine("tmp", $"{type}.cs"),
                    @"using SRCCore.Lib;

namespace SRCCore.Expressions.Functions
{
"
                    );
            }
            foreach (var name in args.Skip(1))
            {
                if (type == "Function")
                {
                    File.AppendAllText(
                        Path.Combine("tmp", $"{type}.cs"),
                        template.Replace("{0}", name.Substring(0, 1).ToUpper() + name.Substring(1))
                        );
                }
                else
                {
                    File.WriteAllText(
                        Path.Combine("tmp", $"{name}{type}.cs"),
                        template.Replace("{0}", name)
                        );
                }
            }
            if (type == "Function")
            {
                File.AppendAllText(
                    Path.Combine("tmp", $"{type}.cs"),
                    "}"
                    );
            }
        }
    }
}
