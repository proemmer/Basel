using Microsoft.Band.Sensors;
using System;
using System.Collections.Generic;
using System.Text;

namespace TileEvents
{
    public class GestureDetector
    {
        /// <summary>
        /// True if band is worn, otherwise false
        /// </summary>
        public bool IsCanDetect { get; set; }

        public void AddAccelerometerData(IBandAccelerometerReading readingData)
        {

        }

        public void AddGyroscopeData(IBandGyroscopeReading readingData)
        {

        }
    }
}
