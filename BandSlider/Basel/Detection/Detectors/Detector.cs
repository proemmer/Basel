using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Basel.Detection.Detectors
{
    public abstract class Detector : IDetector
    {
        protected readonly ISensorDataProducer _producer;
        protected readonly IBaselConfiguration _configuration;
        protected readonly ConcurrentDictionary<IGesture,Action> _gestures = new ConcurrentDictionary<IGesture, Action>();


        public Detector(ISensorDataProducer producer, IBaselConfiguration configuration)
        {
            if (producer == null)
                throw new ArgumentNullException("producer");
            if (configuration == null)
                throw new ArgumentNullException("configuration");
            _producer = producer;
            _configuration = configuration;
        }

        public abstract void AddRecordAsGesture(string name, IRecord record, Action onDetected);

        public virtual void AddGesture(IGesture gesture, Action onDetected)
        {
            _gestures.TryAdd(gesture, onDetected);
        }

        public void RemoveGesture(IGesture gesture)
        {
            Action onDetected;
            _gestures.TryRemove(gesture,out onDetected);
        }


        public Task<bool> StartAsync()
        {
            ActivateCallbacks();
            return _producer.StartAsync();
        }

        public async Task<bool> StopAsync()
        {
            var stopped = await _producer.StopAsync();
            DeactivateCallbacks();
            return stopped;
        }



        private void ActivateCallbacks()
        {

            if (_configuration.AmbientLight)
            {
                _producer.OnAmbientLightSensorUpdate += _producer_OnAmbientLightSensorUpdate;
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

        protected virtual void _producer_OnUVSensorUpdate(object sender, Microsoft.Band.Sensors.BandSensorReadingEventArgs<Microsoft.Band.Sensors.IBandUVReading> e)
        {
        }

        protected virtual void _producer_OnSkinTemperatureSensorUpdate(object sender, Microsoft.Band.Sensors.BandSensorReadingEventArgs<Microsoft.Band.Sensors.IBandSkinTemperatureReading> e)
        {
        }

        protected virtual void _producer_OnRRIntervalSensorUpdate(object sender, Microsoft.Band.Sensors.BandSensorReadingEventArgs<Microsoft.Band.Sensors.IBandRRIntervalReading> e)
        {
        }

        protected virtual void _producer_OnPedometerSensorUpdate(object sender, Microsoft.Band.Sensors.BandSensorReadingEventArgs<Microsoft.Band.Sensors.IBandPedometerReading> e)
        {

        }

        protected virtual void _producer_OnHeartRateSensorUpdate(object sender, Microsoft.Band.Sensors.BandSensorReadingEventArgs<Microsoft.Band.Sensors.IBandHeartRateReading> e)
        {

        }

        protected virtual void _producer_OnGyroscopeSensorUpdate(object sender, Microsoft.Band.Sensors.BandSensorReadingEventArgs<Microsoft.Band.Sensors.IBandGyroscopeReading> e)
        {

        }

        protected virtual void _producer_OnGrsSensorUpdate(object sender, Microsoft.Band.Sensors.BandSensorReadingEventArgs<Microsoft.Band.Sensors.IBandGsrReading> e)
        {

        }

        protected virtual void _producer_OnDistanceSensorUpdate(object sender, Microsoft.Band.Sensors.BandSensorReadingEventArgs<Microsoft.Band.Sensors.IBandDistanceReading> e)
        {

        }

        protected virtual void _producer_OnContactSensorUpdate(object sender, Microsoft.Band.Sensors.BandSensorReadingEventArgs<Microsoft.Band.Sensors.IBandContactReading> e)
        {

        }

        protected virtual void _producer_OnCaloriesSensorUpdate(object sender, Microsoft.Band.Sensors.BandSensorReadingEventArgs<Microsoft.Band.Sensors.IBandCaloriesReading> e)
        {

        }

        protected virtual void _producer_OnBarometerSensorUpdate(object sender, Microsoft.Band.Sensors.BandSensorReadingEventArgs<Microsoft.Band.Sensors.IBandBarometerReading> e)
        {

        }

        protected virtual void _producer_OnAltimeterSensorUpdate(object sender, Microsoft.Band.Sensors.BandSensorReadingEventArgs<Microsoft.Band.Sensors.IBandAltimeterReading> e)
        {

        }

        protected virtual void _producer_OnAccelerometerSensorUpdate(object sender, Microsoft.Band.Sensors.BandSensorReadingEventArgs<Microsoft.Band.Sensors.IBandAccelerometerReading> e)
        {

        }

        protected virtual void _producer_OnAmbientLightSensorUpdate(object sender, Microsoft.Band.Sensors.BandSensorReadingEventArgs<Microsoft.Band.Sensors.IBandAmbientLightReading> e)
        {
        }
    }
}
