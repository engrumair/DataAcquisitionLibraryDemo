using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAcquisitionLibrary
{
    interface IFrameSearcher
    {
         PositionFrame GetPosition(List<byte> internalMemoryBuffer, int startIndexSearch);
    }
}
