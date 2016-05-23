using Microsoft.Band.Sensors;
using System;
namespace Basel.SensorReadings
{
	public sealed class BaselBandGyroscopeReading : BaselBandAccelerometerReading, IBandGyroscopeReading, IBandAccelerometerReading, IBandSensorReading
	{
		public double AngularVelocityX
		{
			get;
            set;
		}
		public double AngularVelocityY
		{
			get;
            set;
		}
		public double AngularVelocityZ
		{
			get;
            set;
		}

        public BaselBandGyroscopeReading()
        {

        }

        public BaselBandGyroscopeReading(IBandGyroscopeReading reading) : base(reading)
        {
            AngularVelocityX = reading.AngularVelocityX;
            AngularVelocityY = reading.AngularVelocityY;
            AngularVelocityZ = reading.AngularVelocityZ;
        }

    }
}
