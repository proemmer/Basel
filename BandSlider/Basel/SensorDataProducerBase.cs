using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Band.Sensors;

namespace Basel
{
    public abstract class SensorDataProducerBase : ISensorDataProducer
    {
        protected EventHandler<BandSensorReadingEventArgs<IBandAmbientLightReading>> _ambientLightSensorUpdate;
        protected EventHandler<BandSensorReadingEventArgs<IBandAccelerometerReading>> _accelerometerSensorUpdate;
        protected EventHandler<BandSensorReadingEventArgs<IBandAltimeterReading>> _altimeterSensorUpdate;
        protected EventHandler<BandSensorReadingEventArgs<IBandBarometerReading>> _barometerSensorUpdate;
        protected EventHandler<BandSensorReadingEventArgs<IBandCaloriesReading>> _caloriesSensorUpdate;
        protected EventHandler<BandSensorReadingEventArgs<IBandContactReading>> _contactSensorUpdate;
        protected EventHandler<BandSensorReadingEventArgs<IBandDistanceReading>> _distanceSensorUpdate;
        protected EventHandler<BandSensorReadingEventArgs<IBandGsrReading>> _grsSensorUpdate;
        protected EventHandler<BandSensorReadingEventArgs<IBandGyroscopeReading>> _gyroscopeSensorUpdate;
        protected EventHandler<BandSensorReadingEventArgs<IBandHeartRateReading>> _heartRateSensorUpdate;
        protected EventHandler<BandSensorReadingEventArgs<IBandPedometerReading>> _pedometerSensorUpdate;                                                                   
        protected EventHandler<BandSensorReadingEventArgs<IBandRRIntervalReading>> _rRIntervalSensorUpdate;
        protected EventHandler<BandSensorReadingEventArgs<IBandSkinTemperatureReading>> _skinTemperatureSensorUpdate;
        protected EventHandler<BandSensorReadingEventArgs<IBandUVReading>> _uVSensorUpdate;



        public event EventHandler<BandSensorReadingEventArgs<IBandAmbientLightReading>> OnAmbientLightSensorUpdate
        {
            add { _ambientLightSensorUpdate += value; }
            remove { _ambientLightSensorUpdate -= value; }
        }

        public event EventHandler<BandSensorReadingEventArgs<IBandAccelerometerReading>> OnAccelerometerSensorUpdate
        {
            add{ _accelerometerSensorUpdate += value; }
            remove  { _accelerometerSensorUpdate -= value;  }
        }

        public event EventHandler<BandSensorReadingEventArgs<IBandAltimeterReading>> OnAltimeterSensorUpdate
        {
            add { _altimeterSensorUpdate += value; }
            remove { _altimeterSensorUpdate -= value; }
        }

        public event EventHandler<BandSensorReadingEventArgs<IBandBarometerReading>> OnBarometerSensorUpdate
        {
            add { _barometerSensorUpdate += value; }
            remove { _barometerSensorUpdate -= value; }
        }
        public event EventHandler<BandSensorReadingEventArgs<IBandCaloriesReading>> OnCaloriesSensorUpdate
        {
            add { _caloriesSensorUpdate += value; }
            remove { _caloriesSensorUpdate -= value; }
        }
        public event EventHandler<BandSensorReadingEventArgs<IBandContactReading>> OnContactSensorUpdate
        {
            add { _contactSensorUpdate += value; }
            remove { _contactSensorUpdate -= value; }
        }
        public event EventHandler<BandSensorReadingEventArgs<IBandDistanceReading>> OnDistanceSensorUpdate
        {
            add { _distanceSensorUpdate += value; }
            remove { _distanceSensorUpdate -= value; }
        }
        public event EventHandler<BandSensorReadingEventArgs<IBandGsrReading>> OnGrsSensorUpdate
        {
            add { _grsSensorUpdate += value; }
            remove { _grsSensorUpdate -= value; }
        }
        public event EventHandler<BandSensorReadingEventArgs<IBandGyroscopeReading>> OnGyroscopeSensorUpdate
        {
            add { _gyroscopeSensorUpdate += value; }
            remove { _gyroscopeSensorUpdate -= value; }
        }
        public event EventHandler<BandSensorReadingEventArgs<IBandHeartRateReading>> OnHeartRateSensorUpdate
        {
            add { _heartRateSensorUpdate += value; }
            remove { _heartRateSensorUpdate -= value; }
        }
        public event EventHandler<BandSensorReadingEventArgs<IBandPedometerReading>> OnPedometerSensorUpdate
        {
            add { _pedometerSensorUpdate += value; }
            remove { _pedometerSensorUpdate -= value; }
        }
        public event EventHandler<BandSensorReadingEventArgs<IBandRRIntervalReading>> OnRRIntervalSensorUpdate
        {
            add { _rRIntervalSensorUpdate += value; }
            remove { _rRIntervalSensorUpdate -= value; }
        }
        public event EventHandler<BandSensorReadingEventArgs<IBandSkinTemperatureReading>> OnSkinTemperatureSensorUpdate
        {
            add { _skinTemperatureSensorUpdate += value; }
            remove { _skinTemperatureSensorUpdate -= value; }
        }
        public event EventHandler<BandSensorReadingEventArgs<IBandUVReading>> OnUVSensorUpdate
        {
            add { _uVSensorUpdate += value; }
            remove { _uVSensorUpdate -= value; }
        }


        protected void ProcessSensorReading<T>(T sensorUpdate, EventHandler<BandSensorReadingEventArgs<T>> onDataChanged) where T : IBandSensorReading
        {
            if (sensorUpdate == null)
                throw new ArgumentNullException("sensorUpdate");
            
            if (onDataChanged != null)
            {
                try
                {
                    onDataChanged(this, new BandSensorReadingEventArgs<T>(sensorUpdate));
                }
                catch (Exception ex)
                {
                    Environment.FailFast("BandSensorBase.ReadingChanged event handler threw an exception that was not handled by the application.", ex);
                }
            }
        }



        public abstract Task<bool> StartAsync();

        public abstract Task<bool> StopAsync();
    }
}
