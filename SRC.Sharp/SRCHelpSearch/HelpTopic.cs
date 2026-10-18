using System.Text.RegularExpressions;

namespace SRCHelpSearch
{
    /// <summary>ヘルプの 1 ページ（SRC.Sharp.Help/src の *.md）</summary>
    public class HelpTopic
    {
        /// <summary>ファイル名から .md を除いたもの。srcsharp help に渡す名前で、ページの中のリンク先とも同じ</summary>
        public string Name { get; }
        /// <summary>見出し。front matter の title（HLP000164 などの番号だけのものは本文の最初の太字）</summary>
        public string Title { get; }
        /// <summary>ファイルの内容（front matter を含む）</summary>
        public string Source { get; }
        /// <summary>front matter を除いた本文</summary>
        public string Body { get; }

        public HelpTopic(string name, string source)
        {
            Name = name;
            Source = source;
            var title = name;
            var match = Regex.Match(source, @"\A---\r?\n(.*?)\r?\n---\r?\n?", RegexOptions.Singleline);
            if (match.Success)
            {
                var titleLine = Regex.Match(match.Groups[1].Value, @"^title:\s*(.+?)\s*$", RegexOptions.Multiline);
                if (titleLine.Success)
                {
                    title = titleLine.Groups[1].Value;
                }
                Body = source.Substring(match.Length);
            }
            else
            {
                Body = source;
            }
            if (Regex.IsMatch(title, @"^HLP\d+$"))
            {
                var heading = Regex.Match(Body, @"^\*\*(?!\s)([^*]+)\*\*\s*$", RegexOptions.Multiline);
                if (heading.Success)
                {
                    title = heading.Groups[1].Value;
                }
            }
            Title = title;
        }
    }
}
