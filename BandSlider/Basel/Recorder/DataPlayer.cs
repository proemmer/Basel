using Microsoft.Band.Sensors;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public override Task<bool> StartAsync()
        {
            if(_record == null)
                throw new InvalidOperationException("Set the record before starting!");
            if (PlayerState == PlayerState.Playing)
                Task.FromResult(false);

            return Task.FromResult(true);
        }

        public Task<bool> PauseAsync()
        {
            if (PlayerState != PlayerState.Playing)
                Task.FromResult(false);

            return Task.FromResult(true);
        }

        public override Task<bool> StopAsync()
        {
            if (PlayerState != PlayerState.Stopped)
                Task.FromResult(false);

            _waitingForPlay.Set();
            _cts.Cancel();
            return Task.FromResult(true);
        }



        private Task Play()
        {
            if (_cts != null)
                _cts.Dispose();

            _cts = new CancellationTokenSource();

            //call PlaySensor

            return Task.FromResult(true);
        }


        private Task PlaySensor<T>(ICollection<T> collection, EventHandler<BandSensorReadingEventArgs<T>> onUpdate) where T : IBandSensorReading
        {
            return Task.Factory.StartNew(() =>
            {
                var accelerometerEnumerator = collection.GetEnumerator();
                var startTime = accelerometerEnumerator.Current.Timestamp;

                while (true)
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
            },
            _cts.Token,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Current);

        }

    }
}
