using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAcquisitionLibrary;

namespace DataAcquisitionLibraryDemo
{
    class Program
    {
        static void Main(string[] args)
        {

            NetworkAcquisitionDevice device  = new NetworkAcquisitionDevice("100.0.0.1",51212,30);// read time delay in seconds.
            FrameSearcherSecond frameSearcher = new FrameSearcherSecond();
            FormatterXorByte formatterCust = new FormatterXorByte();

            DataAcquisition daq = new DataAcquisition(device, frameSearcher, formatterCust);

            daq.EnableDataFormatting(true);
            daq.EnableFrameSearching(true);
            daq.EnableDataWriting(true); // data wil be in your harddisk.

            daq.StartAcquisition();
            bool stop =false;
            while (!stop)
            {

               byte[] dataFromDevice= daq.GetData();

                //process data
            }

            daq.StopDataAcquisition();
            daq.Reset(); // it will reset all parameters. 
        }
    }
}
