using Microsoft.Band.Sensors;
using System;
namespace Basel.SensorReadings
{
	public class BaselBandAccelerometerReading : BaselBandSensorReadingBase, IBandAccelerometerReading, IBandSensorReading
	{
		public double AccelerationX
		{
			get;
            set;
		}
		public double AccelerationY
		{
			get;
            set;
		}
		public double AccelerationZ
		{
			get;
            set;
		}

        public BaselBandAccelerometerReading()
        {

        }

        public BaselBandAccelerometerReading(IBandAccelerometerReading reading) : base(reading.Timestamp)
        {
            AccelerationX = reading.AccelerationX;
            AccelerationY = reading.AccelerationY;
            AccelerationZ = reading.AccelerationZ;
        }

    }
}
