/*
 Readme
 * The code is just commented out because in order to use this code you need to download the 
 * FTDI library from www.ftdi.com. This library works only with FTDI devices. I have used FTDI 
 * devices in one of my project.
 *  
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using FTD2XX_NET;
using System.Threading;

namespace DataAcquisitionLibrary
{
    class USBAcquisitionDevice : IAcquisitionDevice
    {

        //FTDI ftdiDevice = new FTDI();
        //FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OK;
        //FTDI.FT_DEVICE_INFO_NODE[] ftdiDeviceList;

        int deviceDelay=20;
        public USBAcquisitionDevice(int subsequentReadTimeDelay)
        { 
          //  this.deviceDelay = subsequentReadTimeDelay;
        }

        public USBAcquisitionDevice() { }
        public void Refresh()
        {
           // ftStatus = ftdiDevice.Purge(FTDI.FT_PURGE.FT_PURGE_RX);
        }

        public void Close()
        {
           // ftStatus = ftdiDevice.Close();
        }

     

        public void Open()
        {
            //uint ftdiDeviceCount = 0;
            //ftStatus = ftdiDevice.GetNumberOfDevices(ref ftdiDeviceCount);

            //if (ftdiDeviceCount == 0 && ftStatus == FTDI.FT_STATUS.FT_OK)
            //{
                
            //    return;
            //}


            //ftdiDeviceList = new FTDI.FT_DEVICE_INFO_NODE[ftdiDeviceCount];

            //// Populate our device list
            //ftStatus = ftdiDevice.GetDeviceList(ftdiDeviceList);

            //ftStatus = ftdiDevice.OpenBySerialNumber(ftdiDeviceList[0].SerialNumber);

            //Thread.Sleep(500);

            //ftdiDevice.Purge(FTDI.FT_PURGE.FT_PURGE_RX);
        
        }
    

        public byte[] Read()
        {
            //uint numBytesAvailable = 0;

            //ftStatus = ftdiDevice.GetRxBytesAvailable(ref numBytesAvailable);

            //if (numBytesAvailable < 0) return null;

            byte[] tempDataBuffer = null;

            //tempDataBuffer = new byte[numBytesAvailable];
            //uint numBytesRead = 0;

            //ftStatus = ftdiDevice.Read(tempDataBuffer, numBytesAvailable, ref numBytesRead);
            //if (ftStatus != FTDI.FT_STATUS.FT_OK) this.Refresh();

            return tempDataBuffer;
             
        }


   

     

        public int GetSubsequentReadTimeDelay()
        {
            return this.deviceDelay;
        }

     
    }
}
