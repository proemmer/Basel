using Microsoft.Band.Sensors;
using System;
namespace Basel.SensorReadings
{
	public class BaselBandCaloriesReading : BaselBandSensorReadingBase, IBandCaloriesReading, IBandSensorReading
	{
		public long Calories
		{
			get;
            set;
		}
		public long CaloriesToday
		{
			get;
            set;
		}

        public BaselBandCaloriesReading()
        {

        }

        public BaselBandCaloriesReading(IBandCaloriesReading reading) : base(reading.Timestamp)
        {
            Calories = reading.Calories;
            CaloriesToday = reading.CaloriesToday;
        }
    }
}
