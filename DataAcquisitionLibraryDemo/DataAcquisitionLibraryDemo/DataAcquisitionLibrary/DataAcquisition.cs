using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;

namespace DataAcquisitionLibrary
{
    class DataAcquisition
    {
               
        //Reference variables
        DataWriter dataWriter;
        IAcquisitionDevice device;
        IFrameSearcher frameSearcher;
        IFrameDataFormatter frameFormatter;
        IDisplay publisher;


        // 
        Task taskDataAcquistion; // data acquisition thread

        Task taskDataWriting;// data writing thread.

        System.Threading.Timer threadTimer;

        
    
        //Working memory buffer
        private List<byte> internalMemoryBuffer = new List<byte>();
        int startIndexSearch = 0;

        // For Data Hand-OFF to outside libraries..
        private BlockingCollection<byte[]> transferBuffer = new BlockingCollection<byte[]>(30);

        // for closing the connection
        private bool stopDataAcquisition = false;

        // control parameters.
        private bool isDataWritingRequired = false;
         
        private bool isFrameSearchingRequired = false;

        private bool isDataFormattingRequired = false;
                
       

        
        public void EnableDataWriting(bool aval)
        {
            //check the user if data acquisition open do not set the control variables.
            if (!stopDataAcquisition)
            {

                isDataWritingRequired = aval;

                if (isDataWritingRequired)
                    this.dataWriter = new DataWriter();
            }
        }
        public void EnableFrameSearching(bool aval)
        {
            isFrameSearchingRequired = aval;
        }
        public void EnableDataFormatting(bool aval)
        {
            isDataFormattingRequired = aval;
        }
        /// <summary>
        /// only reset the control parameters.
        /// </summary>
        public void Reset()
        {
            this.isDataFormattingRequired = false;
            this.isDataWritingRequired = false;
            this.isFrameSearchingRequired = false;
            this.stopDataAcquisition = false; 
        }


        public DataAcquisition(IAcquisitionDevice aDevice, IFrameSearcher aFrameSearcher,IFrameDataFormatter formatFrame ,IDisplay publisher)
        {
            this.device = aDevice;
            this.frameFormatter = formatFrame;
            this.frameSearcher = aFrameSearcher;
            this.publisher = publisher;
          
        }
        public DataAcquisition(IAcquisitionDevice aDevice, IFrameSearcher aFrameSearcher, IFrameDataFormatter formatFrame)
        {
            this.device = aDevice;
            this.frameFormatter = formatFrame;
            this.frameSearcher = aFrameSearcher;            

        }
        public DataAcquisition(IAcquisitionDevice aDevice, IFrameSearcher aFrameSearcher)
        {
            this.device = aDevice;
            this.frameSearcher = aFrameSearcher;

        }
        public DataAcquisition(IAcquisitionDevice aDevice)
        {
            this.device = aDevice;
            

        }
        
      
        public void StartAcquisition()
        {
          
            if(isDataWritingRequired)
            dataWriter.openDataStorageConnection();

            stopDataAcquisition = false;
            taskDataAcquistion = new Task(() => this.BeginProducingData(), TaskCreationOptions.LongRunning);

            taskDataAcquistion.Start();

            if (isDataWritingRequired)
            {
                taskDataWriting = new Task(() => dataWriter.WriteData(), TaskCreationOptions.LongRunning);
                taskDataWriting.Start();
            }

        }
        
     
        public void StopDataAcquisition()
        {
            this.stopDataAcquisition = true;

            if(isDataWritingRequired)

            dataWriter.closeDataStorageConnection(internalMemoryBuffer);
        }


        private void BeginProducingData()
        {
        //    this.setIsStopDataAcquisition(false);

            AutoResetEvent autoEvent = new AutoResetEvent(false);

            TimerCallback continuousTimerCallback = new TimerCallback(acquireData);
            device.Refresh();

         
            threadTimer = new System.Threading.Timer(continuousTimerCallback, autoEvent, 0, device.GetSubsequentReadTimeDelay() );


            autoEvent.WaitOne(-1);

            //important: threadTimer must be disposed herere.. otherwise it will be called continuously.

            threadTimer.Dispose();
            if(stopDataAcquisition)
            stopDataAcquisition = false;
            device.Refresh();
            device.Close();

        }
        
        private void acquireData(object info)
        {
            AutoResetEvent anEvent = (AutoResetEvent)info;
            try
            {
               int sync = Interlocked.CompareExchange(ref syncPoint, 1, 0); // put 1 in syncPoint 
                
                if (sync == 0)
                {                   
                    
                   byte[] tempDataBuffer =  device.Read();
                              
                   if (tempDataBuffer != null)
                    {
                                                
                        // copy the data to the main buffer

                        internalMemoryBuffer.AddRange(tempDataBuffer);
                                              
                           // this will search and extract frames in the internal memory buffer and /or format them and Handoff them for external use.
                        SearchFormatHandoff();

                        
                        EmptyInternalBuffer();// Empty the internal buffer and /or write the data chunk to hard disk..
                        
                    }
              
                    if (this.stopDataAcquisition)
                        anEvent.Set();

                        syncPoint = 0;

                }
                else
                {
                    if (this.stopDataAcquisition)
                    {
                        syncPoint = 0;
                        anEvent.Set();
                    }
                    

                }
            }
            catch (Exception excep)
            {
                syncPoint = 0;
                anEvent.Set();
                
            }

        }

        private void EmptyInternalBuffer()
        {
            if (internalMemoryBuffer.Count >= (noOfBytesToTransfer + bytesToRemainInBuffer))
            {


                startIndexSearch = startIndexSearch - noOfBytesToTransfer;
                // Write data to Disk Storage

                if (isDataWritingRequired)
                {
                    List<byte> aTestBuffer12 = internalMemoryBuffer.GetRange(0, noOfBytesToTransfer);

                    dataWriter.Add(aTestBuffer12);

                }
                // Remove data from internal Memory..
                internalMemoryBuffer.RemoveRange(0, noOfBytesToTransfer);
                if (startIndexSearch < 0) startIndexSearch = 0;

            }
        }

        private void SearchFormatHandoff()
        {
         
                         
            
            while(startIndexSearch < internalMemoryBuffer.Count)
            {
                byte[] locData = null;
                if (isFrameSearchingRequired)
                {

                    PositionFrame position = frameSearcher.GetPosition(internalMemoryBuffer, startIndexSearch);

                    if (position == null)
                    {
                        //Debugger.Break(); 
                    }
                    // frame not Found...
                    if (position.IsNullPosition())
                    {
                        startIndexSearch = position.GetNextSearchStartingPosition();

                        return;
                    }

                    // else if frame found.
                    startIndexSearch = position.GetNextSearchStartingPosition();

                    locData = new byte[position.GetFrameLength()];
                    int counter = 0;
                    for (int i = position.GetFrameStartPosition(); i < position.GetFrameStartPosition() + position.GetFrameLength(); i++)
                    {

                        // now first XOR ith Element of Internal Memory Buffer with 55 here
                        if (isDataFormattingRequired)
                        {
                            locData[counter++] = frameFormatter.FormatByte(internalMemoryBuffer[i]);

                        }
                        else
                        {
                            locData[counter++] = internalMemoryBuffer[i];
                        }



                    }
                    
                }
                else
                { 
                    // No frame search required now just copy the data to output buffer...
                                       
                    locData = new byte[internalMemoryBuffer.Count - startIndexSearch];

                    int dataCounter =0;

                    for (int i = startIndexSearch; i < internalMemoryBuffer.Count; i++)
                    {
                        if (isDataFormattingRequired)
                        {

                            locData[dataCounter++] = frameFormatter.FormatByte(internalMemoryBuffer[i]);
                        }
                        else
                        {
                            locData[dataCounter++] = internalMemoryBuffer[i];
                        }
                    
                    }
                    startIndexSearch = internalMemoryBuffer.Count;

                
                }
                // now Hand off data..
             transferBuffer.TryAdd(locData);

             //publisher.UpdateUserInterface(locData);
                                
            }
          
                       
        }

               
        public byte[] GetData()
        {
                        
            byte[] dataResultWithTry = null;
                     

            bool isDataAvailable = this.transferBuffer.TryTake(out dataResultWithTry);
                       


            return dataResultWithTry;
           
                        
        }
          
           
        private static int syncPoint = 0; // use to prevent ree-entrancy

        private const int bytesToRemainInBuffer = 1024; // 

        private const int noOfBytesToTransfer = 1048576 * 5; // 1 MByte = 1048576.. transfer from working memory..
    }
}
