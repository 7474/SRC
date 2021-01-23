using System;
using System.Collections.Generic;
using System.Text;

namespace SRC.Core.Exceptions
{
    public class InvalidSrcDataException : ApplicationException
    {
        public IList<InvalidSrcData> InvalidDataList { get; }
        public InvalidSrcDataException(IList<InvalidSrcData> invalidSrcDataList)
        {
            InvalidDataList = new List<InvalidSrcData>(invalidSrcDataList);
        }
    }
}
