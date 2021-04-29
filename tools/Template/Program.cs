using System;
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
            foreach (var name in args.Skip(1))
            {
                File.WriteAllText(
                    Path.Combine("tmp", $"{name}{type}.cs"),
                    template.Replace("{0}", name)
                    );
            }
        }
    }
}
