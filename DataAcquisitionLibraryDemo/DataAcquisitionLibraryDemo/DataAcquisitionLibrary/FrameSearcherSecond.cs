using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAcquisitionLibrary
{
    class FrameSearcherSecond : IFrameSearcher
    {

        byte[] startPattern = new byte[2];

        byte[] endPattern = new byte[2];

        public FrameSearcherSecond()
        {
            startPattern[0] = 0x70 ;
            startPattern[1] = 0x70 ;
        

            endPattern[0] = 0xf1 ;
            endPattern[1] = 0xD2 ;

        }
     
        /// <summary>
        /// This Function will return the location of the very first frame in the memory buffer..
        /// </summary>
        /// <param name="internalMemoryBuffer">memory buffer reference</param>
        /// <param name="startIndexSearch">starting index inside the memory buffer where it will search</param>
        /// <returns>PositionFrame class which will give the position of the frame inside the memory buffer</returns>
        public PositionFrame GetPosition(List<byte> internalMemoryBuffer, int startIndexSearch)
        {
            if (startIndexSearch > internalMemoryBuffer.Count - startPattern.Length)
            {
               // return null;
                return new PositionFrame(true, internalMemoryBuffer.Count - startPattern.Length);
            }

            // find starting pattern
            // before finding the just invert the logic in here from start to end..

            int frameStartIndex = FindPattern(internalMemoryBuffer, startPattern, startIndexSearch);

            if (frameStartIndex == -1)

            {
                return new PositionFrame(true, internalMemoryBuffer.Count - startPattern.Length);
            }

            int frameEndIndex = FindPattern(internalMemoryBuffer, endPattern, frameStartIndex + startPattern.Length);

            if (frameEndIndex == -1)
            {
                return new PositionFrame(true, frameStartIndex);
                
            }

            int Lenght = frameEndIndex - frameStartIndex + endPattern.Length;
            int nextSearchStartPosition = frameEndIndex + endPattern.Length;

            PositionFrame aPositionFrame = new PositionFrame(frameStartIndex, frameEndIndex,nextSearchStartPosition, Lenght);

            return aPositionFrame;
            
        }
        private int FindPattern(List<byte> data, byte[] pattern, int startPosition)
        {
            int[] failure = ComputeFailure(pattern);
            int j = 0;
            if (data.Count == 0) return -1;
            for (int i = startPosition; i < data.Count; i++)
            {
                while (j > 0 && pattern[j] != data[i])
                {
                    j = failure[j - 1];
                }
                if (pattern[j] == data[i]) { j++; }
                if (j == pattern.Length)
                {
                    return i - pattern.Length + 1;
                }
            }
            return -1;
        }

        private int[] ComputeFailure(byte[] pattern)
        {
            int[] failure = new int[pattern.Length];

            int j = 0;
            for (int i = 1; i < pattern.Length; i++)
            {
                while (j > 0 && pattern[j] != pattern[i])
                {
                    j = failure[j - 1];
                }
                if (pattern[j] == pattern[i])
                {
                    j++;
                }
                failure[i] = j;
            }

            return failure;
        }
    }
}
