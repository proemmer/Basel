using Microsoft.Band.Sensors;
using System;
namespace Basel.SensorReadings
{
	public sealed class BaselBandDistanceReading : BaselBandSensorReadingBase, IBandDistanceReading, IBandSensorReading
	{

		public long TotalDistance
		{
			get;
            set;
		}
		public long DistanceToday
		{
			get;
            set;
		}
		public double Speed
		{
			get;
            set;
		}
		public double Pace
		{
			get;
            set;
		}
		public MotionType CurrentMotion
		{
			get;
            set;
		}

        public BaselBandDistanceReading()
        {

        }

        public BaselBandDistanceReading(IBandDistanceReading reading) : base(reading.Timestamp)
        {
            TotalDistance = reading.TotalDistance;
            DistanceToday = reading.DistanceToday;
            Speed = reading.Speed;
            Pace = reading.Pace;
            CurrentMotion = reading.CurrentMotion;
        }
    }
}
