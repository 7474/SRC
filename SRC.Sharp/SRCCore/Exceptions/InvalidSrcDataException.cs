using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRC.Core.Exceptions
{
    public class InvalidSrcDataException : ApplicationException
    {
        public IList<InvalidSrcData> InvalidDataList { get; }

        public InvalidSrcDataException(IList<InvalidSrcData> invalidSrcDataList)
            : this(
                  invalidSrcDataList.Any() ? invalidSrcDataList.First().msg : "InvalidSrcData",
                  invalidSrcDataList
            )
        {
        }

        public InvalidSrcDataException(string msg, IList<InvalidSrcData> invalidSrcDataList)
            : base(msg)
        {
            InvalidDataList = new List<InvalidSrcData>(invalidSrcDataList);
        }
    }
}
