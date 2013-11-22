using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAcquisitionLibrary
{
    interface IFrameDataFormatter
    {
         byte FormatByte(byte val);
    }
}
