using Microsoft.Band.Sensors;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Basel.Recorder
{
    public class DataPlayer : SensorDataProducerBase, IDataPlayer
    {
        private IRecord _record;
        private CancellationTokenSource _cts;
        private ManualResetEventSlim _waitingForPlay = new ManualResetEventSlim();
        private bool _pausing;
        private double _speed;
        private IBaselConfiguration _configuration;

        public double Speed
        {
            get { return _speed; }
            set
            {
                if (value > 0)
                    _speed = value;
                else
                    throw new ArgumentOutOfRangeException("speed");
            }
        }

        public bool Loop { get; set; }

        public PlayerState PlayerState { get; private set; } = PlayerState.Stopped;

        public IRecord Record
        {
            get { return _record; }
            set
            {
                if (PlayerState == PlayerState.Stopped)
                    _record = value;
                else
                    throw new InvalidOperationException("Set the record is only available in state stopped!");
            }
        }


        public DataPlayer(IBaselConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException("configuration");
            _configuration = configuration;
        }

        public override Task<bool> StartAsync()
        {
            if(_record == null)
                throw new InvalidOperationException("Set the record before starting!");
            if (PlayerState == PlayerState.Playing)
                Task.FromResult(false);
            _pausing = false;
            _waitingForPlay.Set();
            return Task.FromResult(true);
        }

        public Task<bool> PauseAsync()
        {
            if (PlayerState != PlayerState.Playing)
                Task.FromResult(false);

            _pausing = true;
            return Task.FromResult(true);
        }

        public override Task<bool> StopAsync()
        {
            if (PlayerState != PlayerState.Stopped)
                Task.FromResult(false);

            _pausing = false;
            _cts.Cancel();
            _waitingForPlay.Set();
            return Task.FromResult(true);
        }



        private Task Play()
        {
            if (_cts != null)
                _cts.Dispose();

            _cts = new CancellationTokenSource();

            if (_configuration.AmbientLight)
            {
                PlaySensorAsync(Record.AmbientLight, _ambientLightSensorUpdate);
            }

            if (_configuration.Accelerometer)
            {
                PlaySensorAsync(Record.Accelerometer, _accelerometerSensorUpdate);
            }

            if (_configuration.Altimeter)
            {
                PlaySensorAsync(Record.Altimeter, _altimeterSensorUpdate);
            }

            if (_configuration.Barometer)
            {
                PlaySensorAsync(Record.Barometer, _barometerSensorUpdate);
            }

            if (_configuration.Calories)
            {
                PlaySensorAsync(Record.Calories, _caloriesSensorUpdate);
            }

            if (_configuration.Contact)
            {
                PlaySensorAsync(Record.Contact, _contactSensorUpdate);
            }

            if (_configuration.Distance)
            {
                PlaySensorAsync(Record.Distance, _distanceSensorUpdate);
            }

            if (_configuration.Gsr)
            {
                PlaySensorAsync(Record.Gsr, _grsSensorUpdate);
            }

            if (_configuration.Gyroscope)
            {
                PlaySensorAsync(Record.Gyroscope, _gyroscopeSensorUpdate);
            }

            if (_configuration.HeartRate)
            {
                PlaySensorAsync(Record.HeartRate, _heartRateSensorUpdate);
            }

            if (_configuration.Pedometer)
            {
                PlaySensorAsync(Record.Pedometer, _pedometerSensorUpdate);
            }

            if (_configuration.RRInterval)
            {
                PlaySensorAsync(Record.RRInterval, _rRIntervalSensorUpdate);
            }

            if (_configuration.SkinTemperature)
            {
                PlaySensorAsync(Record.SkinTemperature, _skinTemperatureSensorUpdate);
            }

            if (_configuration.UV)
            {
                PlaySensorAsync(Record.UV, _uVSensorUpdate);
            }

            return Task.FromResult(true);
        }


        private Task PlaySensorAsync<T>(ICollection<T> collection, EventHandler<BandSensorReadingEventArgs<T>> onUpdate) where T : IBandSensorReading
        {
            return Task.Factory.StartNew(() =>
            {
                do
                {
                    var accelerometerEnumerator = collection.GetEnumerator();
                    var startTime = accelerometerEnumerator.Current.Timestamp;

                    while (!_cts.IsCancellationRequested)
                    {
                        if (_pausing)
                            _waitingForPlay.Wait();
                        if (_cts.IsCancellationRequested)
                            return;

                        ProcessSensorReading<T>(accelerometerEnumerator.Current, onUpdate);

                        if (accelerometerEnumerator.MoveNext())
                        {
                            var sleepTime = Convert.ToInt32((accelerometerEnumerator.Current.Timestamp - startTime).TotalMilliseconds * _speed);
                            if (_cts.Token.WaitHandle.WaitOne(sleepTime))
                                break;
                        }
                        else
                            break;
                    }

                    //TODO:  synchronize with other sensors!

                } while (Loop);
            },
            _cts.Token,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Current);

        }

    }
}
