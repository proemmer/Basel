using Microsoft.Band.Sensors;
using System;
namespace Basel.SensorReadings
{
    public class BaselBandAltimeterReading : BaselBandSensorReadingBase, IBandAltimeterReading, IBandSensorReading
    {
        public long TotalGain
        {
            get;
            set;
        }
        public long TotalLoss
        {
            get;
            set;
        }
        public long SteppingGain
        {
            get;
            set;
        }
        public long SteppingLoss
        {
            get;
            set;
        }
        public long StepsAscended
        {
            get;
            set;
        }
        public long StepsDescended
        {
            get;
            set;
        }
        public float Rate
        {
            get;
            set;
        }
        public long FlightsAscended
        {
            get;
            set;
        }
        public long FlightsDescended
        {
            get;
            set;
        }
        public long FlightsAscendedToday
        {
            get;
            set;
        }
        public long TotalGainToday
        {
            get;
            set;
        }

        public BaselBandAltimeterReading()
        {

        }

        public BaselBandAltimeterReading(IBandAltimeterReading reading) : base(reading.Timestamp)
        {

            TotalGain = reading.TotalGain;
            TotalLoss = reading.TotalLoss;
            SteppingGain = reading.SteppingGain;
            SteppingLoss = reading.SteppingLoss;
            StepsAscended = reading.StepsAscended;
            StepsDescended = reading.StepsDescended;
            Rate = reading.Rate;
            FlightsAscended = reading.FlightsAscended;
            FlightsDescended = reading.FlightsDescended;
            FlightsAscendedToday = reading.FlightsAscendedToday;
            TotalGainToday = reading.TotalGainToday;

        }
    }
}
