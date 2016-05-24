using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Basel.Recorder
{
    public class DataRecorder : IDataRecorder
    {
        private readonly ISensorDataProducer _producer;
        private IRecord _record;
        private IBaselConfiguration _configuration;

        public RecorderState RecorderState { get; private set; } = RecorderState.Stopped;

        public IRecord Record
        {
            get
            {
                return _record;
            }
        }

        public DataRecorder(ISensorDataProducer producer, IBaselConfiguration configuration)
        {
            if (producer == null)
                throw new ArgumentNullException("producer");
            if (configuration == null)
                throw new ArgumentNullException("configuration");
            _producer = producer;
            _configuration = configuration;
        }

        public Task<bool> StartAsync()
        {
            if (_record == null)
                _record = new Record();

            ActivateCallbacks();
            RecorderState = RecorderState.Recoring;
            return _producer.StartAsync();
        }

        public async Task<bool> StopAsync()
        {
            var stopped = await _producer.StopAsync();
            RecorderState = RecorderState.Stopped;
            DeactivateCallbacks();
            return stopped;
        }

        public Task<bool> PauseAsync()
        {
            if(RecorderState != RecorderState.Recoring)
                return Task.FromResult(true);

            RecorderState = RecorderState.Pausing;
            return Task.FromResult(true);
        }

        public Task<bool> ResetAsync()
        {
            if (RecorderState == RecorderState.Recoring)
                return Task.FromResult(false);
            _record = new Record();
            return Task.FromResult(true);
        }


        private void ActivateCallbacks()
        {

            if (_configuration.AmbientLight)
            {
                _producer.OnAmbientLightSensorUpdate += _producer_OnAmbientLightSensorUpdate; ;
            }

            if (_configuration.Accelerometer)
            {
                _producer.OnAccelerometerSensorUpdate += _producer_OnAccelerometerSensorUpdate;
            }

            if (_configuration.Altimeter)
            {
                _producer.OnAltimeterSensorUpdate += _producer_OnAltimeterSensorUpdate;
            }

            if (_configuration.Barometer)
            {
                _producer.OnBarometerSensorUpdate += _producer_OnBarometerSensorUpdate;
            }

            if (_configuration.Calories)
            {
                _producer.OnCaloriesSensorUpdate += _producer_OnCaloriesSensorUpdate;
            }

            if (_configuration.Contact)
            {
                _producer.OnContactSensorUpdate += _producer_OnContactSensorUpdate; ;
            }

            if (_configuration.Distance)
            {
                _producer.OnDistanceSensorUpdate += _producer_OnDistanceSensorUpdate;
            }

            if (_configuration.Gsr)
            {
                _producer.OnGrsSensorUpdate += _producer_OnGrsSensorUpdate;
            }

            if (_configuration.Gyroscope)
            {
                _producer.OnGyroscopeSensorUpdate += _producer_OnGyroscopeSensorUpdate;
            }

            if (_configuration.HeartRate)
            {
                _producer.OnHeartRateSensorUpdate += _producer_OnHeartRateSensorUpdate;
            }

            if (_configuration.Pedometer)
            {
                _producer.OnPedometerSensorUpdate += _producer_OnPedometerSensorUpdate;
            }

            if (_configuration.RRInterval)
            {
                _producer.OnRRIntervalSensorUpdate += _producer_OnRRIntervalSensorUpdate;
            }

            if (_configuration.SkinTemperature)
            {
                _producer.OnSkinTemperatureSensorUpdate += _producer_OnSkinTemperatureSensorUpdate;
            }

            if (_configuration.UV)
            {
                _producer.OnUVSensorUpdate += _producer_OnUVSensorUpdate;
            }
        }

        private void DeactivateCallbacks()
        {

            if (_configuration.AmbientLight)
            {
                _producer.OnAmbientLightSensorUpdate -= _producer_OnAmbientLightSensorUpdate; ;
            }

            if (_configuration.Accelerometer)
            {
                _producer.OnAccelerometerSensorUpdate -= _producer_OnAccelerometerSensorUpdate;
            }

            if (_configuration.Altimeter)
            {
                _producer.OnAltimeterSensorUpdate -= _producer_OnAltimeterSensorUpdate;
            }

            if (_configuration.Barometer)
            {
                _producer.OnBarometerSensorUpdate -= _producer_OnBarometerSensorUpdate;
            }

            if (_configuration.Calories)
            {
                _producer.OnCaloriesSensorUpdate -= _producer_OnCaloriesSensorUpdate;
            }

            if (_configuration.Contact)
            {
                _producer.OnContactSensorUpdate -= _producer_OnContactSensorUpdate; ;
            }

            if (_configuration.Distance)
            {
                _producer.OnDistanceSensorUpdate -= _producer_OnDistanceSensorUpdate;
            }

            if (_configuration.Gsr)
            {
                _producer.OnGrsSensorUpdate -= _producer_OnGrsSensorUpdate;
            }

            if (_configuration.Gyroscope)
            {
                _producer.OnGyroscopeSensorUpdate -= _producer_OnGyroscopeSensorUpdate;
            }

            if (_configuration.HeartRate)
            {
                _producer.OnHeartRateSensorUpdate -= _producer_OnHeartRateSensorUpdate;
            }

            if (_configuration.Pedometer)
            {
                _producer.OnPedometerSensorUpdate -= _producer_OnPedometerSensorUpdate;
            }

            if (_configuration.RRInterval)
            {
                _producer.OnRRIntervalSensorUpdate -= _producer_OnRRIntervalSensorUpdate;
            }

            if (_configuration.SkinTemperature)
            {
                _producer.OnSkinTemperatureSensorUpdate -= _producer_OnSkinTemperatureSensorUpdate;
            }

            if (_configuration.UV)
            {
                _producer.OnUVSensorUpdate -= _producer_OnUVSensorUpdate;
            }
        }

        private void _producer_OnUVSensorUpdate(object sender, Microsoft.Band.Sensors.BandSensorReadingEventArgs<Microsoft.Band.Sensors.IBandUVReading> e)
        {
            if(RecorderState == RecorderState.Recoring)
                Record.UV.Add(e.SensorReading);
        }

        private void _producer_OnSkinTemperatureSensorUpdate(object sender, Microsoft.Band.Sensors.BandSensorReadingEventArgs<Microsoft.Band.Sensors.IBandSkinTemperatureReading> e)
        {
            if (RecorderState == RecorderState.Recoring)
                Record.SkinTemperature.Add(e.SensorReading);
        }

        private void _producer_OnRRIntervalSensorUpdate(object sender, Microsoft.Band.Sensors.BandSensorReadingEventArgs<Microsoft.Band.Sensors.IBandRRIntervalReading> e)
        {
            if (RecorderState == RecorderState.Recoring)
                Record.RRInterval.Add(e.SensorReading);
        }

        private void _producer_OnPedometerSensorUpdate(object sender, Microsoft.Band.Sensors.BandSensorReadingEventArgs<Microsoft.Band.Sensors.IBandPedometerReading> e)
        {
            if (RecorderState == RecorderState.Recoring)
                Record.Pedometer.Add(e.SensorReading);
        }

        private void _producer_OnHeartRateSensorUpdate(object sender, Microsoft.Band.Sensors.BandSensorReadingEventArgs<Microsoft.Band.Sensors.IBandHeartRateReading> e)
        {
            if (RecorderState == RecorderState.Recoring)
                Record.HeartRate.Add(e.SensorReading);
        }

        private void _producer_OnGyroscopeSensorUpdate(object sender, Microsoft.Band.Sensors.BandSensorReadingEventArgs<Microsoft.Band.Sensors.IBandGyroscopeReading> e)
        {
            if (RecorderState == RecorderState.Recoring)
                Record.Gyroscope.Add(e.SensorReading);
        }

        private void _producer_OnGrsSensorUpdate(object sender, Microsoft.Band.Sensors.BandSensorReadingEventArgs<Microsoft.Band.Sensors.IBandGsrReading> e)
        {
            if (RecorderState == RecorderState.Recoring)
                Record.Gsr.Add(e.SensorReading);
        }

        private void _producer_OnDistanceSensorUpdate(object sender, Microsoft.Band.Sensors.BandSensorReadingEventArgs<Microsoft.Band.Sensors.IBandDistanceReading> e)
        {
            if (RecorderState == RecorderState.Recoring)
                Record.Distance.Add(e.SensorReading);
        }

        private void _producer_OnContactSensorUpdate(object sender, Microsoft.Band.Sensors.BandSensorReadingEventArgs<Microsoft.Band.Sensors.IBandContactReading> e)
        {
            if (RecorderState == RecorderState.Recoring)
                Record.Contact.Add(e.SensorReading);
        }

        private void _producer_OnCaloriesSensorUpdate(object sender, Microsoft.Band.Sensors.BandSensorReadingEventArgs<Microsoft.Band.Sensors.IBandCaloriesReading> e)
        {
            if (RecorderState == RecorderState.Recoring)
                Record.Calories.Add(e.SensorReading);
        }

        private void _producer_OnBarometerSensorUpdate(object sender, Microsoft.Band.Sensors.BandSensorReadingEventArgs<Microsoft.Band.Sensors.IBandBarometerReading> e)
        {
            if (RecorderState == RecorderState.Recoring)
                Record.Barometer.Add(e.SensorReading);
        }

        private void _producer_OnAltimeterSensorUpdate(object sender, Microsoft.Band.Sensors.BandSensorReadingEventArgs<Microsoft.Band.Sensors.IBandAltimeterReading> e)
        {
            if (RecorderState == RecorderState.Recoring)
                Record.Altimeter.Add(e.SensorReading);
        }

        private void _producer_OnAccelerometerSensorUpdate(object sender, Microsoft.Band.Sensors.BandSensorReadingEventArgs<Microsoft.Band.Sensors.IBandAccelerometerReading> e)
        {
            if (RecorderState == RecorderState.Recoring)
                Record.Accelerometer.Add(e.SensorReading);
        }

        private void _producer_OnAmbientLightSensorUpdate(object sender, Microsoft.Band.Sensors.BandSensorReadingEventArgs<Microsoft.Band.Sensors.IBandAmbientLightReading> e)
        {
            if (RecorderState == RecorderState.Recoring)
                Record.AmbientLight.Add(e.SensorReading);
        }
    }
}
