using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAcquisitionLibrary
{
    interface IAcquisitionDevice
    {

         void Open();
         void Refresh();
         void Close();
         int GetSubsequentReadTimeDelay();
         byte[] Read();

    }

}
