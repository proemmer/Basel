using Microsoft.Band.Sensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basel.Detection.Recognizer.UWave
{
    public class UWaveGesture : Gesture
    {
        public List<IBandAccelerometerReading> Readings { get; private set; }

        public UWaveGesture(string name, List<IBandAccelerometerReading> timepoints)
        {
            Name = name;
            Readings = timepoints;
            Length = timepoints.Count;
        }

        
    }
}
