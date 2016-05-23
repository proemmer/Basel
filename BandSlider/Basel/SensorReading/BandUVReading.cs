using Microsoft.Band.Sensors;
using System;
namespace Basel.SensorReadings
{
	internal class BaselBandUVReading : BaselBandSensorReadingBase, IBandUVReading, IBandSensorReading
	{
		public UVIndexLevel IndexLevel
		{
			get;
			private set;
		}
		public long ExposureToday
        {
            get;
            private set;
        }

        public BaselBandUVReading()
        {

        }

        public BaselBandUVReading(IBandUVReading reading) : base(reading.Timestamp)
        {
            IndexLevel = reading.IndexLevel;
            ExposureToday = reading.ExposureToday;
        }
    }
}
