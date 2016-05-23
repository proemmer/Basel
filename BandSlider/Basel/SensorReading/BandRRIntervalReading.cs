using Microsoft.Band.Sensors;
using System;
namespace Basel.SensorReadings
{ 
    public class BaselBandRRIntervalReading : BaselBandSensorReadingBase, IBandRRIntervalReading, IBandSensorReading
    {
        public double Interval
        {
            get;
            set;
        }

        public BaselBandRRIntervalReading()
        {

        }

        public BaselBandRRIntervalReading(IBandRRIntervalReading reading) : base(reading.Timestamp)
        {
            Interval = reading.Interval;
        }
    }
}
