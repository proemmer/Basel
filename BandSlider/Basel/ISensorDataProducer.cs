using Microsoft.Band.Sensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basel
{
    public interface ISensorDataProducer
    {
        event EventHandler<BandSensorReadingEventArgs<IBandAccelerometerReading>> OnAccelerometerSensorUpdate;
        event EventHandler<BandSensorReadingEventArgs<IBandAltimeterReading>> OnAltimeterSensorUpdate;
        event EventHandler<BandSensorReadingEventArgs<IBandAmbientLightReading>> OnAmbientLightSensorUpdate;
        event EventHandler<BandSensorReadingEventArgs<IBandBarometerReading>> OnBarometerSensorUpdate;
        event EventHandler<BandSensorReadingEventArgs<IBandCaloriesReading>> OnCaloriesSensorUpdate;
        event EventHandler<BandSensorReadingEventArgs<IBandContactReading>> OnContactSensorUpdate;
        event EventHandler<BandSensorReadingEventArgs<IBandDistanceReading>> OnDistanceSensorUpdate;
        event EventHandler<BandSensorReadingEventArgs<IBandGsrReading>> OnGrsSensorUpdate;
        event EventHandler<BandSensorReadingEventArgs<IBandGyroscopeReading>> OnGyroscopeSensorUpdate;
        event EventHandler<BandSensorReadingEventArgs<IBandHeartRateReading>> OnHeartRateSensorUpdate;
        event EventHandler<BandSensorReadingEventArgs<IBandPedometerReading>> OnPedometerSensorUpdate;
        event EventHandler<BandSensorReadingEventArgs<IBandRRIntervalReading>> OnRRIntervalSensorUpdate;
        event EventHandler<BandSensorReadingEventArgs<IBandSkinTemperatureReading>> OnSkinTemperatureSensorUpdate;
        event EventHandler<BandSensorReadingEventArgs<IBandUVReading>> OnUVSensorUpdate;


        Task<bool> StartAsync();

        Task<bool> StopAsync();
    }
}
