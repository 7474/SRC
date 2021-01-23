using System;
using System.Collections.Generic;
using System.Text;

namespace SRC.Core.Exceptions
{
    public class InvalidSrcData
    {
        public string msg { get; }
        public string fname { get; }
        public int line_num { get; }
        public string line_buf { get; }
        public string dname { get; }
        public InvalidSrcData(string msg, string fname, int line_num, string line_buf, string dname)
        {
            this.msg = msg;
            this.fname = fname;
            this.line_num = line_num;
            this.line_buf = line_buf;
            this.dname = dname;
        }
    }
}
