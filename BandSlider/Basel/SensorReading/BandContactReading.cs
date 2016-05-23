using Microsoft.Band.Sensors;
using System;
namespace Basel.SensorReadings
{
	public class BaselBandContactReading : BaselBandSensorReadingBase, IBandContactReading, IBandSensorReading
	{
		public BandContactState State
		{
			get;
            set;
		}

        public BaselBandContactReading()
        {

        }

        public BaselBandContactReading(IBandContactReading reading) : base(reading.Timestamp)
        {
            State = reading.State;
        }
    }
}
