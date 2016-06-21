using Microsoft.Band;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Linq;
using Basel.SensorReadings;

namespace Basel
{
    //https://code.msdn.microsoft.com/windowsapps/Shake-Gesture-Library-04c82d5f/sourcecode?fileId=86871&pathId=1135434873


    public class BandManager : SensorDataProducerBase, IDisposable
    {
        #region Fields
        private readonly IBandClientManager _bandClientManager;
        private IBandClient _bandClient;
        private IBaselConfiguration _configuration;
        #endregion

        public BandManager(IBandClientManager bandClientManager, IBaselConfiguration configuration)
        {
            if (bandClientManager == null)
                throw new ArgumentNullException("bandClientManager");
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            _bandClientManager = bandClientManager;
            _configuration = configuration;
        }

        public override async Task<bool> StartAsync()
        {

            try
            {
                // Get the list of Microsoft Bands paired to the phone.
                IBandInfo[] pairedBands = await _bandClientManager.GetBandsAsync();
                if (pairedBands.Length < 1)
                    return await Task.FromResult(false);

                _bandClient = await _bandClientManager.ConnectAsync(pairedBands[0]);
                if (_bandClient != null)
                {
                    var start = new List<Task>();

                    if (_configuration.AmbientLight)
                    {
                        _bandClient.SensorManager.AmbientLight.ReadingChanged += AmbientLight_ReadingChanged;
                        start.Add(_bandClient.SensorManager.AmbientLight.StartReadingsAsync());
                    }

                    if (_configuration.Accelerometer)
                    {
                        _bandClient.SensorManager.Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
                        start.Add(_bandClient.SensorManager.Accelerometer.StartReadingsAsync());
                    }

                    if (_configuration.Altimeter)
                    {
                        _bandClient.SensorManager.Altimeter.ReadingChanged += Altimeter_ReadingChanged;
                        start.Add(_bandClient.SensorManager.Altimeter.StartReadingsAsync());
                    }

                    if (_configuration.Barometer)
                    {
                        _bandClient.SensorManager.Barometer.ReadingChanged += Barometer_ReadingChanged;
                        start.Add(_bandClient.SensorManager.Barometer.StartReadingsAsync());
                    }

                    if (_configuration.Calories)
                    {
                        _bandClient.SensorManager.Calories.ReadingChanged += Calories_ReadingChanged;
                        start.Add(_bandClient.SensorManager.Calories.StartReadingsAsync());
                    }

                    if (_configuration.Contact)
                    {
                        _bandClient.SensorManager.Contact.ReadingChanged += Contact_ReadingChanged;
                        start.Add(_bandClient.SensorManager.Contact.StartReadingsAsync());
                    }

                    if (_configuration.Distance)
                    {
                        _bandClient.SensorManager.Distance.ReadingChanged += Distance_ReadingChanged;
                        start.Add(_bandClient.SensorManager.Distance.StartReadingsAsync());
                    }

                    if (_configuration.Gsr)
                    {
                        _bandClient.SensorManager.Gsr.ReadingChanged += Gsr_ReadingChanged;
                        start.Add(_bandClient.SensorManager.Gsr.StartReadingsAsync());
                    }

                    if (_configuration.Gyroscope)
                    {
                        _bandClient.SensorManager.Gyroscope.ReadingChanged += Gyroscope_ReadingChanged;
                        start.Add(_bandClient.SensorManager.Gyroscope.StartReadingsAsync());
                    }

                    if (_configuration.HeartRate)
                    {
                        _bandClient.SensorManager.HeartRate.ReadingChanged += HeartRate_ReadingChanged;
                        start.Add(_bandClient.SensorManager.HeartRate.StartReadingsAsync());
                    }

                    if (_configuration.Pedometer)
                    {
                        _bandClient.SensorManager.Pedometer.ReadingChanged += Pedometer_ReadingChanged;
                        start.Add(_bandClient.SensorManager.Pedometer.StartReadingsAsync());
                    }

                    if (_configuration.RRInterval)
                    {
                        _bandClient.SensorManager.RRInterval.ReadingChanged += RRInterval_ReadingChanged;
                        start.Add(_bandClient.SensorManager.RRInterval.StartReadingsAsync());
                    }

                    if (_configuration.SkinTemperature)
                    {
                        _bandClient.SensorManager.SkinTemperature.ReadingChanged += SkinTemperature_ReadingChanged;
                        start.Add(_bandClient.SensorManager.SkinTemperature.StartReadingsAsync());
                    }

                    if (_configuration.UV)
                    {
                        _bandClient.SensorManager.UV.ReadingChanged += UV_ReadingChanged;
                        start.Add(_bandClient.SensorManager.UV.StartReadingsAsync());
                    }

                    await Task.WhenAll(start.ToArray());
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
                if (_bandClient == null)
                    return await Task.FromResult(false);

                var stop = new List<Task>();

                if (_configuration.AmbientLight)
                {
                    _bandClient.SensorManager.AmbientLight.ReadingChanged -= AmbientLight_ReadingChanged;
                    stop.Add(_bandClient.SensorManager.AmbientLight.StopReadingsAsync());
                }

                if (_configuration.Accelerometer)
                {
                    _bandClient.SensorManager.Accelerometer.ReadingChanged -= Accelerometer_ReadingChanged;
                    stop.Add(_bandClient.SensorManager.Accelerometer.StopReadingsAsync());
                }

                if (_configuration.Altimeter)
                {
                    _bandClient.SensorManager.Altimeter.ReadingChanged -= Altimeter_ReadingChanged;
                    stop.Add(_bandClient.SensorManager.Altimeter.StopReadingsAsync());
                }

                if (_configuration.Barometer)
                {
                    _bandClient.SensorManager.Barometer.ReadingChanged -= Barometer_ReadingChanged; 
                    stop.Add(_bandClient.SensorManager.Barometer.StopReadingsAsync());
                }

                if (_configuration.Calories)
                {
                    _bandClient.SensorManager.Calories.ReadingChanged -= Calories_ReadingChanged;
                    stop.Add(_bandClient.SensorManager.Calories.StopReadingsAsync());
                }

                if (_configuration.Contact)
                {
                    _bandClient.SensorManager.Contact.ReadingChanged -= Contact_ReadingChanged;
                    stop.Add(_bandClient.SensorManager.Contact.StopReadingsAsync());
                }

                if (_configuration.Distance)
                {
                    _bandClient.SensorManager.Distance.ReadingChanged -= Distance_ReadingChanged;
                    stop.Add(_bandClient.SensorManager.Distance.StopReadingsAsync());
                }

                if (_configuration.Gsr)
                {
                    _bandClient.SensorManager.Gsr.ReadingChanged -= Gsr_ReadingChanged;
                    stop.Add(_bandClient.SensorManager.Gsr.StopReadingsAsync());
                }

                if (_configuration.Gyroscope)
                {
                    _bandClient.SensorManager.Gyroscope.ReadingChanged -= Gyroscope_ReadingChanged;
                    stop.Add(_bandClient.SensorManager.Gyroscope.StopReadingsAsync());
                }

                if (_configuration.HeartRate)
                {
                    _bandClient.SensorManager.HeartRate.ReadingChanged -= HeartRate_ReadingChanged;
                    stop.Add(_bandClient.SensorManager.HeartRate.StopReadingsAsync());
                }

                if (_configuration.Pedometer)
                {
                    _bandClient.SensorManager.Pedometer.ReadingChanged -= Pedometer_ReadingChanged;
                    stop.Add(_bandClient.SensorManager.Pedometer.StopReadingsAsync());
                }

                if (_configuration.RRInterval)
                {
                    _bandClient.SensorManager.RRInterval.ReadingChanged -= RRInterval_ReadingChanged;
                    stop.Add(_bandClient.SensorManager.RRInterval.StopReadingsAsync());
                }

                if (_configuration.SkinTemperature)
                {
                    _bandClient.SensorManager.SkinTemperature.ReadingChanged -= SkinTemperature_ReadingChanged;
                    stop.Add(_bandClient.SensorManager.SkinTemperature.StopReadingsAsync());
                }

                if (_configuration.UV)
                {
                    _bandClient.SensorManager.UV.ReadingChanged -= UV_ReadingChanged;
                    stop.Add(_bandClient.SensorManager.UV.StopReadingsAsync());
                }

                await Task.WhenAll(stop.ToArray());

                _bandClient.Dispose();
                _bandClient = null;
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

        #region Reading Converter

        private void Barometer_ReadingChanged(object sender, Microsoft.Band.Sensors.BandSensorReadingEventArgs<Microsoft.Band.Sensors.IBandBarometerReading> e)
        {
            ProcessSensorReading(new BaselBandBarometerReading(e.SensorReading), _barometerSensorUpdate);
        }

        private void Calories_ReadingChanged(object sender, Microsoft.Band.Sensors.BandSensorReadingEventArgs<Microsoft.Band.Sensors.IBandCaloriesReading> e)
        {
            ProcessSensorReading(new BaselBandCaloriesReading(e.SensorReading), _caloriesSensorUpdate);
        }

        private void Contact_ReadingChanged(object sender, Microsoft.Band.Sensors.BandSensorReadingEventArgs<Microsoft.Band.Sensors.IBandContactReading> e)
        {
            ProcessSensorReading(new BaselBandContactReading(e.SensorReading), _contactSensorUpdate);
        }

        private void Distance_ReadingChanged(object sender, Microsoft.Band.Sensors.BandSensorReadingEventArgs<Microsoft.Band.Sensors.IBandDistanceReading> e)
        {
            ProcessSensorReading(new BaselBandDistanceReading(e.SensorReading), _distanceSensorUpdate);
        }

        private void Gsr_ReadingChanged(object sender, Microsoft.Band.Sensors.BandSensorReadingEventArgs<Microsoft.Band.Sensors.IBandGsrReading> e)
        {
            ProcessSensorReading(new BaselBandGsrReading(e.SensorReading), _grsSensorUpdate);
        }

        private void Gyroscope_ReadingChanged(object sender, Microsoft.Band.Sensors.BandSensorReadingEventArgs<Microsoft.Band.Sensors.IBandGyroscopeReading> e)
        {
            ProcessSensorReading(new BaselBandGyroscopeReading(e.SensorReading), _gyroscopeSensorUpdate);
        }

        private void HeartRate_ReadingChanged(object sender, Microsoft.Band.Sensors.BandSensorReadingEventArgs<Microsoft.Band.Sensors.IBandHeartRateReading> e)
        {
            ProcessSensorReading(new BaselBandHeartRateReading(e.SensorReading), _heartRateSensorUpdate);
        }

        private void Pedometer_ReadingChanged(object sender, Microsoft.Band.Sensors.BandSensorReadingEventArgs<Microsoft.Band.Sensors.IBandPedometerReading> e)
        {
            ProcessSensorReading(new BaselBandPedometerReading(e.SensorReading), _pedometerSensorUpdate);
        }

        private void RRInterval_ReadingChanged(object sender, Microsoft.Band.Sensors.BandSensorReadingEventArgs<Microsoft.Band.Sensors.IBandRRIntervalReading> e)
        {
            ProcessSensorReading(new BaselBandRRIntervalReading(e.SensorReading), _rRIntervalSensorUpdate);
        }

        private void SkinTemperature_ReadingChanged(object sender, Microsoft.Band.Sensors.BandSensorReadingEventArgs<Microsoft.Band.Sensors.IBandSkinTemperatureReading> e)
        {
            ProcessSensorReading(new BaselBandSkinTemperatureReading(e.SensorReading), _skinTemperatureSensorUpdate);
        }

        private void UV_ReadingChanged(object sender, Microsoft.Band.Sensors.BandSensorReadingEventArgs<Microsoft.Band.Sensors.IBandUVReading> e)
        {
            ProcessSensorReading(new BaselBandUVReading(e.SensorReading), _uVSensorUpdate);
        }

        private void Altimeter_ReadingChanged(object sender, Microsoft.Band.Sensors.BandSensorReadingEventArgs<Microsoft.Band.Sensors.IBandAltimeterReading> e)
        {
            ProcessSensorReading(new BaselBandAltimeterReading(e.SensorReading), _altimeterSensorUpdate);
        }

        private void Accelerometer_ReadingChanged(object sender, Microsoft.Band.Sensors.BandSensorReadingEventArgs<Microsoft.Band.Sensors.IBandAccelerometerReading> e)
        {
            ProcessSensorReading(new BaselBandAccelerometerReading(e.SensorReading), _accelerometerSensorUpdate);
        }

        private void AmbientLight_ReadingChanged(object sender, Microsoft.Band.Sensors.BandSensorReadingEventArgs<Microsoft.Band.Sensors.IBandAmbientLightReading> e)
        {
            ProcessSensorReading(new BaselBandAmbientLightReading(e.SensorReading), _ambientLightSensorUpdate);
        }
        #endregion

    }
}
