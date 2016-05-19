using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Band.Sensors;

namespace Basel
{
    public class Record : IRecord
    {
        public ICollection<IBandAccelerometerReading> Accelerometer { get; private set; } = new List<IBandAccelerometerReading>();
        public ICollection<IBandAltimeterReading> Altimeter{ get; private set; } = new List<IBandAltimeterReading>();
        public ICollection<IBandAmbientLightReading> AmbientLight{ get; private set; } = new List<IBandAmbientLightReading>();
        public ICollection<IBandBarometerReading> Barometer{ get; private set; } = new List<IBandBarometerReading>();
        public ICollection<IBandCaloriesReading> Calories{ get; private set; } = new List<IBandCaloriesReading>();
        public ICollection<IBandContactReading> Contact{ get; private set; } = new List<IBandContactReading>();
        public ICollection<IBandDistanceReading> Distance{ get; private set; } = new List<IBandDistanceReading>();
        public ICollection<IBandGsrReading> Gsr{ get; private set; } = new List<IBandGsrReading>();
        public ICollection<IBandGyroscopeReading> Gyroscope{ get; private set; } = new List<IBandGyroscopeReading>();
        public ICollection<IBandHeartRateReading> HeartRate{ get; private set; } = new List<IBandHeartRateReading>();
        public ICollection<IBandPedometerReading> Pedometer{ get; private set; } = new List<IBandPedometerReading>();
        public ICollection<IBandRRIntervalReading> RRInterval{ get; private set; } = new List<IBandRRIntervalReading>();
        public ICollection<IBandSkinTemperatureReading> SkinTemperature{ get; private set; } = new List<IBandSkinTemperatureReading>();
        public ICollection<IBandUVReading> UV{ get; private set; } = new List<IBandUVReading>();
    }
}
