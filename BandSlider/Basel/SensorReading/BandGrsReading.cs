using Microsoft.Band.Sensors;
using System;
namespace Basel.SensorReadings
{
    public class BaselBandGsrReading : BaselBandSensorReadingBase, IBandGsrReading, IBandSensorReading
    {
        public int Resistance
        {
            get;
            set;
        }

        public BaselBandGsrReading()
        {
                
        }

        public BaselBandGsrReading(IBandGsrReading reading) : base(reading.Timestamp)
        {
            Resistance = reading.Resistance;
        }
    }


}
