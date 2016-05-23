using Microsoft.Band.Sensors;
using System;
namespace Basel.SensorReadings
{
	public class BaselBandPedometerReading : BaselBandSensorReadingBase, IBandPedometerReading, IBandSensorReading
	{
		public long TotalSteps
		{
			get;
            set;
		}
		public long StepsToday
		{
			get;
			set;
		}

        public BaselBandPedometerReading()
        {

        }

        public BaselBandPedometerReading(IBandPedometerReading reading) : base(reading.Timestamp)
        {
            TotalSteps = reading.TotalSteps;
            StepsToday = reading.StepsToday;
        }
    }
}
