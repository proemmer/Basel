using Microsoft.Band.Sensors;
using System;
namespace Basel.SensorReadings
{
	public abstract class BaselBandSensorReadingBase : IBandSensorReading
	{
		public DateTimeOffset Timestamp
		{
			get;
			set;
		}

        public BaselBandSensorReadingBase()
        {

        }

        public BaselBandSensorReadingBase(DateTimeOffset timestamp)
        {
            Timestamp = timestamp;
        }
	}
}
