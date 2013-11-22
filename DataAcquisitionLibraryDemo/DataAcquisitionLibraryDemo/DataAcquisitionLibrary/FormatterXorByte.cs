using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAcquisitionLibrary
{
    class FormatterXorByte : IFrameDataFormatter
    {
        public byte FormatByte(byte aByte)
        { 
            byte xorByte = 0x22;
                    byte resultByte =(byte) (aByte ^ xorByte);

                    return resultByte;
        }
    }
}
