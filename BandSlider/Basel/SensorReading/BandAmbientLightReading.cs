using Basel.SensorReadings;
using Microsoft.Band.Sensors;
using System;
namespace Basel.SensorReadings
{
	public class BaselBandAmbientLightReading : BaselBandSensorReadingBase, IBandAmbientLightReading, IBandSensorReading
	{
		public int Brightness
		{
			get;
            set;
		}

        public BaselBandAmbientLightReading()
        {

        }

        public BaselBandAmbientLightReading(IBandAmbientLightReading reading) : base(reading.Timestamp)
        {
            Brightness = reading.Brightness;
        }
	}
}
