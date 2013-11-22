using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAcquisitionLibrary
{
     interface IDisplay
    {
        void UpdateUserInterface(byte[] dataAvailable);
        //void DisplayVlues(StatusParameter param);
    }
}
