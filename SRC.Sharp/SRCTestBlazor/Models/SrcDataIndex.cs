using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SRCTestBlazor.Models
{
    public class SrcDataIndex
    {
        public ICollection<TitleData> Titles { get; set; }
    }
    public class TitleData
    {
        public string Title { get; set; }
        public string Base { get; set; }
        public string Path { get; set; }
        public ICollection<string> Tags { get; set; }
        public ICollection<string> Files { get; set; }
    }
}
