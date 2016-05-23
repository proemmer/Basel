using Microsoft.Band.Sensors;
using System;
namespace Basel.SensorReadings
{
	internal class BaselBandHeartRateReading : BaselBandSensorReadingBase, IBandHeartRateReading, IBandSensorReading
	{
		public int HeartRate
		{
			get;
            set;
		}
		public HeartRateQuality Quality
		{
			get;
            set;
		}

        public BaselBandHeartRateReading()
        {

        }

        public BaselBandHeartRateReading(IBandHeartRateReading reading) : base(reading.Timestamp)
        {
            HeartRate = reading.HeartRate;
            Quality = reading.Quality;
        }
    }
}
