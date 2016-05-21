using Microsoft.Band;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Linq;

namespace Basel
{
    public class BandManager : SensorDataProducerBase, IDisposable
    {
        private readonly ConcurrentDictionary<ISensorDataConsumer, object> _subscribers = new ConcurrentDictionary<ISensorDataConsumer, object>();
        private readonly IBandClientManager _bandClientManager;
        private IBandClient _bandClient;
        private IBaselConfiguration _configuration;

        public BandManager(IBandClientManager bandClientManager, IBaselConfiguration configuration)
        {
            if (bandClientManager == null)
                throw new ArgumentNullException("bandClientManager");
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            _bandClientManager = bandClientManager;
            _configuration = configuration;
        }


        public bool Subscribe(ISensorDataConsumer consumer)
        {
            return _subscribers.TryAdd(consumer,null);
        }

        public bool Unsubscribe(ISensorDataConsumer consumer)
        {
            object c;
            return _subscribers.TryRemove(consumer, out c);
        }

        public override async Task<bool> StartAsync()
        {

            try
            {
                // Get the list of Microsoft Bands paired to the phone.
                IBandInfo[] pairedBands = await _bandClientManager.GetBandsAsync();
                if (pairedBands.Length < 1)
                {
                    return await Task.FromResult(false);
                }

                _bandClient = await _bandClientManager.ConnectAsync(pairedBands[0]);
                if (_bandClient != null)
                {
                    var start = new List<Task>();

                    if (_configuration.AmbientLight)
                    {
                        _bandClient.SensorManager.AmbientLight.ReadingChanged += (s, args) => ProcessSensorReading(args.SensorReading, _ambientLightSensorUpdate);
                        start.Add(_bandClient.SensorManager.AmbientLight.StartReadingsAsync());
                    }

                    if (_configuration.Accelerometer)
                    {
                        _bandClient.SensorManager.Accelerometer.ReadingChanged += (s, args) => ProcessSensorReading(args.SensorReading, _accelerometerSensorUpdate);
                        start.Add(_bandClient.SensorManager.Accelerometer.StartReadingsAsync());
                    }

                    if (_configuration.Altimeter)
                    {
                        _bandClient.SensorManager.Altimeter.ReadingChanged += (s, args) => ProcessSensorReading(args.SensorReading, _altimeterSensorUpdate);
                        start.Add(_bandClient.SensorManager.Altimeter.StartReadingsAsync());
                    }

                    if (_configuration.Barometer)
                    {
                        _bandClient.SensorManager.Barometer.ReadingChanged += (s, args) => ProcessSensorReading(args.SensorReading, _barometerSensorUpdate);
                        start.Add(_bandClient.SensorManager.Barometer.StartReadingsAsync());
                    }

                    if (_configuration.Calories)
                    {
                        _bandClient.SensorManager.Calories.ReadingChanged += (s, args) => ProcessSensorReading(args.SensorReading, _caloriesSensorUpdate);
                        start.Add(_bandClient.SensorManager.Calories.StartReadingsAsync());
                    }

                    if (_configuration.Contact)
                    {
                        _bandClient.SensorManager.Contact.ReadingChanged += (s, args) => ProcessSensorReading(args.SensorReading, _contactSensorUpdate);
                        start.Add(_bandClient.SensorManager.Contact.StartReadingsAsync());
                    }

                    if (_configuration.Distance)
                    {
                        _bandClient.SensorManager.Distance.ReadingChanged += (s, args) => ProcessSensorReading(args.SensorReading, _distanceSensorUpdate);
                        start.Add(_bandClient.SensorManager.Distance.StartReadingsAsync());
                    }

                    if (_configuration.Gsr)
                    {
                        _bandClient.SensorManager.Gsr.ReadingChanged += (s, args) => ProcessSensorReading(args.SensorReading, _grsSensorUpdate);
                        start.Add(_bandClient.SensorManager.Gsr.StartReadingsAsync());
                    }

                    if (_configuration.Gyroscope)
                    {
                        _bandClient.SensorManager.Gyroscope.ReadingChanged += (s, args) => ProcessSensorReading(args.SensorReading, _gyroscopeSensorUpdate);
                        start.Add(_bandClient.SensorManager.Gyroscope.StartReadingsAsync());
                    }

                    if (_configuration.HeartRate)
                    {
                        _bandClient.SensorManager.HeartRate.ReadingChanged += (s, args) => ProcessSensorReading(args.SensorReading, _heartRateSensorUpdate);
                        start.Add(_bandClient.SensorManager.HeartRate.StartReadingsAsync());
                    }

                    if (_configuration.Pedometer)
                    {
                        _bandClient.SensorManager.Pedometer.ReadingChanged += (s, args) => ProcessSensorReading(args.SensorReading, _pedometerSensorUpdate);
                        start.Add(_bandClient.SensorManager.Pedometer.StartReadingsAsync());
                    }

                    if (_configuration.RRInterval)
                    {
                        _bandClient.SensorManager.RRInterval.ReadingChanged += (s, args) => ProcessSensorReading(args.SensorReading, _rRIntervalSensorUpdate);
                        start.Add(_bandClient.SensorManager.RRInterval.StartReadingsAsync());
                    }

                    if (_configuration.SkinTemperature)
                    {
                        _bandClient.SensorManager.SkinTemperature.ReadingChanged += (s, args) => ProcessSensorReading(args.SensorReading, _skinTemperatureSensorUpdate);
                        start.Add(_bandClient.SensorManager.SkinTemperature.StartReadingsAsync());
                    }

                    if (_configuration.UV)
                    {
                        _bandClient.SensorManager.UV.ReadingChanged += (s, args) => ProcessSensorReading(args.SensorReading, _uVSensorUpdate);
                        start.Add(_bandClient.SensorManager.UV.StartReadingsAsync());
                    }

                    //Task.WaitAll(start.ToArray());
                }
            }
            catch (Exception ex)
            {
                return await Task.FromResult(false);
            }
            return await Task.FromResult(true);
        }

        public override async Task<bool> StopAsync()
        {
            try
            {
                var start = new List<Task>();

                if (_configuration.AmbientLight)
                {
                    _bandClient.SensorManager.AmbientLight.ReadingChanged -= (s, args) => ProcessSensorReading(args.SensorReading, _ambientLightSensorUpdate);
                    start.Add(_bandClient.SensorManager.AmbientLight.StopReadingsAsync());
                }

                if (_configuration.Accelerometer)
                {
                    _bandClient.SensorManager.Accelerometer.ReadingChanged -= (s, args) => ProcessSensorReading(args.SensorReading, _accelerometerSensorUpdate);
                    start.Add(_bandClient.SensorManager.Accelerometer.StopReadingsAsync());
                }

                if (_configuration.Altimeter)
                {
                    _bandClient.SensorManager.Altimeter.ReadingChanged -= (s, args) => ProcessSensorReading(args.SensorReading, _altimeterSensorUpdate);
                    start.Add(_bandClient.SensorManager.Altimeter.StopReadingsAsync());
                }

                if (_configuration.Barometer)
                {
                    _bandClient.SensorManager.Barometer.ReadingChanged -= (s, args) => ProcessSensorReading(args.SensorReading, _barometerSensorUpdate);
                    start.Add(_bandClient.SensorManager.Barometer.StopReadingsAsync());
                }

                if (_configuration.Calories)
                {
                    _bandClient.SensorManager.Calories.ReadingChanged -= (s, args) => ProcessSensorReading(args.SensorReading, _caloriesSensorUpdate);
                    start.Add(_bandClient.SensorManager.Calories.StopReadingsAsync());
                }

                if (_configuration.Contact)
                {
                    _bandClient.SensorManager.Contact.ReadingChanged -= (s, args) => ProcessSensorReading(args.SensorReading, _contactSensorUpdate);
                    start.Add(_bandClient.SensorManager.Contact.StopReadingsAsync());
                }

                if (_configuration.Distance)
                {
                    _bandClient.SensorManager.Distance.ReadingChanged -= (s, args) => ProcessSensorReading(args.SensorReading, _distanceSensorUpdate);
                    start.Add(_bandClient.SensorManager.Distance.StopReadingsAsync());
                }

                if (_configuration.Gsr)
                {
                    _bandClient.SensorManager.Gsr.ReadingChanged -= (s, args) => ProcessSensorReading(args.SensorReading, _grsSensorUpdate);
                    start.Add(_bandClient.SensorManager.Gsr.StopReadingsAsync());
                }

                if (_configuration.Gyroscope)
                {
                    _bandClient.SensorManager.Gyroscope.ReadingChanged -= (s, args) => ProcessSensorReading(args.SensorReading, _gyroscopeSensorUpdate);
                    start.Add(_bandClient.SensorManager.Gyroscope.StopReadingsAsync());
                }

                if (_configuration.HeartRate)
                {
                    _bandClient.SensorManager.HeartRate.ReadingChanged -= (s, args) => ProcessSensorReading(args.SensorReading, _heartRateSensorUpdate);
                    start.Add(_bandClient.SensorManager.HeartRate.StopReadingsAsync());
                }

                if (_configuration.Pedometer)
                {
                    _bandClient.SensorManager.Pedometer.ReadingChanged -= (s, args) => ProcessSensorReading(args.SensorReading, _pedometerSensorUpdate);
                    start.Add(_bandClient.SensorManager.Pedometer.StopReadingsAsync());
                }

                if (_configuration.RRInterval)
                {
                    _bandClient.SensorManager.RRInterval.ReadingChanged -= (s, args) => ProcessSensorReading(args.SensorReading, _rRIntervalSensorUpdate);
                    start.Add(_bandClient.SensorManager.RRInterval.StopReadingsAsync());
                }

                if (_configuration.SkinTemperature)
                {
                    _bandClient.SensorManager.SkinTemperature.ReadingChanged -= (s, args) => ProcessSensorReading(args.SensorReading, _skinTemperatureSensorUpdate);
                    start.Add(_bandClient.SensorManager.SkinTemperature.StopReadingsAsync());
                }

                if (_configuration.UV)
                {
                    _bandClient.SensorManager.UV.ReadingChanged -= (s, args) => ProcessSensorReading(args.SensorReading, _uVSensorUpdate);
                    start.Add(_bandClient.SensorManager.UV.StopReadingsAsync());
                }

                Task.WaitAll(start.ToArray());
            }
            catch (Exception ex)
            {
                return await Task.FromResult(false);
            }
            return await Task.FromResult(true);
        }

        public void Dispose()
        {
            if (_bandClient != null)
                _bandClient.Dispose();
        }
    }
}
