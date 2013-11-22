using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.Concurrent;

namespace DataAcquisitionLibrary
{
    class DataWriter
    {
        FileStream threadWriter;
        BinaryWriter binaryThreadWriter;

        private BlockingCollection<List<byte>> transferDataBuffer = new BlockingCollection<List<byte>>();

        bool stopWriting= false;

        public void WriteData()
        {

            while (!stopWriting)
            {
                // thread safe thick call from the generating thread..
                //   DateTime atime = DateTime.Now;
                
                List<byte> transferredData = transferDataBuffer.Take();
                

                foreach (byte b in transferredData)
                {
                    binaryThreadWriter.Write(b);
                }

             
            }

        }

        private void StopWriting()
        {
            this.stopWriting = true;
        }
        
        internal void Add(List<byte> aTestBuffer12)
        {
            transferDataBuffer.Add(aTestBuffer12);
            
        }

        public void openDataStorageConnection()
        {

            this.stopWriting = false;

            DateTime dt = DateTime.Now;

            string dtString = Configuration.getAddressOfStorage() +"mydata"+dt.Hour.ToString() + dt.Minute.ToString() + dt.Second.ToString();



            threadWriter = new FileStream(dtString, FileMode.Create);
            binaryThreadWriter = new BinaryWriter(threadWriter);


        }

        public void closeDataStorageConnection(List<byte> remainingBytes)
        {
            
            this.stopWriting = true;

            for (int i = 0; i < remainingBytes.Count; i++)
            {

                binaryThreadWriter.Write(remainingBytes[i]);
            }

            binaryThreadWriter.Close();
            threadWriter.Close();


        }
            


    }
}
