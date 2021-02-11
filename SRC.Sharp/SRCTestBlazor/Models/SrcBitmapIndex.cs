using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SRCTestBlazor.Models
{
    public class SrcBitmapIndex
    {
        public ICollection<BitmapFolder> Folders { get; set; }

        private IDictionary<string, string> index = new Dictionary<string, string>();
        private Uri baseUri;


        public void BuildIndex(string baseUri)
        {
            this.baseUri = new Uri(baseUri);
            index = Folders
                .SelectMany(x => x.Files.Select(y => new
                {
                    Key = $"{x.Name}/{y}".ToLower(),
                    Value = $"{x.Path}/{y}"
                }))
                .ToDictionary(x => x.Key, x => x.Value);
        }
        public string GetBitmapUri(string categoryName, string fileName)
        {
            var key = $"{categoryName}/{fileName}".ToLower().Replace(".bmp", ".png");

            return index.ContainsKey(key)
                ? new Uri(baseUri, index[key]).ToString()
                : "images/-.png";
        }
    }
    public class BitmapFolder
    {
        public string Name { get; set; }
        public string Base { get; set; }
        public string Path { get; set; }
        public ICollection<string> Tags { get; set; }
        public ICollection<string> Files { get; set; }
    }
}
