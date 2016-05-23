using Microsoft.Band.Sensors;
using System;
namespace Basel.SensorReadings
{
	public class BaselBandSkinTemperatureReading : BaselBandSensorReadingBase, IBandSkinTemperatureReading, IBandSensorReading
	{

		public double Temperature
		{
			get;
			private set;
		}

        public BaselBandSkinTemperatureReading()
        {

        }

        public BaselBandSkinTemperatureReading(IBandSkinTemperatureReading reading) : base(reading.Timestamp)
		{
            Temperature = reading.Temperature;

        }

	}
}
