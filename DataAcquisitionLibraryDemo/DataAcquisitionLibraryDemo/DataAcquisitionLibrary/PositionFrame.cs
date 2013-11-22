using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace DataAcquisitionLibrary
{
    class PositionFrame
    {
        int frameStartPosition;
        int frameEndPosition;
        int nextSearchStartingPosition;
        int frameLength;
        bool isNull = false;

        public PositionFrame(bool isNull, int nextStartPosition)
        {
        
            this.isNull = isNull;
            this.nextSearchStartingPosition = nextStartPosition;
        }
        public bool IsNullPosition() { return this.isNull; }

        public int GetNextSearchStartingPosition()
        {
            return nextSearchStartingPosition;
        
        }
        public PositionFrame(int startPosition, int endPosition, int nextSearchStartPosition, int frameLen)
        {
            this.frameStartPosition = startPosition;

            this.frameEndPosition = endPosition;

            this.nextSearchStartingPosition = nextSearchStartPosition;

            this.frameLength = frameLen;

        
        }
        public int GetFrameStartPosition()
        {
            return this.frameStartPosition;
        }
        public int GetFrameEndPosition()
        {
            return this.frameEndPosition;
        }
        internal int GetFrameLength()
        {
     
            return this.frameLength;
            
        }


    }
}
