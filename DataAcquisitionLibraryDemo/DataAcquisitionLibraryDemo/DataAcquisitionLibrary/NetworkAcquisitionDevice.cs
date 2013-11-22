using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;
using System.Net.Sockets;
using System.IO;

namespace DataAcquisitionLibrary
{
    class NetworkAcquisitionDevice : IAcquisitionDevice
    {
       private TcpClient client;
       private string ipAddress;
       private int portNum;

        private int deviceReadTimeDelay = 30;

     

        public NetworkAcquisitionDevice(string address,int portNumber)
        {
            this.ipAddress = address;
            this.portNum = portNumber;
            
        }

        public NetworkAcquisitionDevice(string address, int portNumber, int subsequentReadTimeDelay)
        {
            this.ipAddress = address;
            this.portNum = portNumber;
            this.deviceReadTimeDelay = subsequentReadTimeDelay;

        }
        public void Open()
        {

            client = new TcpClient(this.ipAddress, portNum);


            client.ReceiveBufferSize = 11000;
                     


        }

        public void Refresh()
        {
            // ftStatus = ftdiDevice.Purge(FTDI.FT_PURGE.FT_PURGE_RX);
        }

        public void Close()
        {
            //ftStatus = ftdiDevice.Close();
        }
               
      
        public byte[] Read()
        {
            try
            {
              //  using (TcpClient client = new TcpClient("127.0.0.1", 51111))
                NetworkStream n = client.GetStream();
          
                BinaryReader br = new BinaryReader(n);
                     byte[] dataFromServer = null;
                    if(client.Available>0)
                     dataFromServer = br.ReadBytes(client.Available);


                    return dataFromServer;
                
                
            }
            catch (Exception excep)
            {

                int aValue = 3;
                String str = excep.Message;
                return null;
            }



        }

    
        public int  GetSubsequentReadTimeDelay()
{
 	return this.deviceReadTimeDelay;
}
}
}
