using Microsoft.Band.Sensors;
using System;
namespace Basel.SensorReadings
{
	public class BaselBandBarometerReading : BaselBandSensorReadingBase, IBandBarometerReading, IBandSensorReading
	{
		public double AirPressure
		{
			get;
            set;
		}
		public double Temperature
		{
			get;
            set;
		}

        public BaselBandBarometerReading()
        {

        }

        public BaselBandBarometerReading(IBandBarometerReading reading) : base(reading.Timestamp)
        {
            AirPressure = reading.AirPressure;
            Temperature = reading.Temperature;
        }
    }
}
