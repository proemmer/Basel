using Microsoft.Band.Sensors;
using System.Collections.Generic;

namespace Basel
{
    public interface IRecord
    {
        ICollection<IBandAccelerometerReading> Accelerometer { get; }
        ICollection<IBandAltimeterReading> Altimeter { get; }
        ICollection<IBandAmbientLightReading> AmbientLight { get; }
        ICollection<IBandBarometerReading> Barometer { get; }
        ICollection<IBandCaloriesReading> Calories { get; }
        ICollection<IBandContactReading> Contact { get; }
        ICollection<IBandDistanceReading> Distance { get; }
        ICollection<IBandGsrReading> Gsr { get; }
        ICollection<IBandGyroscopeReading> Gyroscope { get; }
        ICollection<IBandHeartRateReading> HeartRate { get; }
        ICollection<IBandPedometerReading> Pedometer { get; }
        ICollection<IBandRRIntervalReading> RRInterval { get; }
        ICollection<IBandSkinTemperatureReading> SkinTemperature { get; }
        ICollection<IBandUVReading> UV { get; }
    }
}