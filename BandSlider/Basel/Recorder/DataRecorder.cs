using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Basel.Recorder
{
    public class DataRecorder : IDataRecorder, ISensorDataConsumer
    {
        private readonly IBandManager _bandManager;
        private IRecord _record;
 
        private CancellationTokenSource _cts;

        public RecorderState RecorderState { get; private set; } = RecorderState.Stopped;

        public IRecord Record
        {
            get
            {
                if (RecorderState != RecorderState.Stopped)
                    throw new InvalidOperationException("Set the record is only available in state stopped!");
                return _record;
            }
        }



        public DataRecorder(IBandManager manager)
        {
            if (manager == null)
                throw new ArgumentNullException("manager");
            _bandManager = manager;
        }

        public Task<bool> StartAsync()
        {
            if (RecorderState == RecorderState.Pausing)

            if (_record == null)
                _record = new Record();

            return _bandManager.StartAsync();
        }

        public Task<IRecord> StopAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> PauseAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> ResetAsync()
        {
            throw new NotImplementedException();
        }



        public Task<bool> RestartAsync(double speed = 1, bool loop = false)
        {
            if (_record == null)
                throw new InvalidOperationException("Restart is not allowed, you have to call start first!");

            return _bandManager.StartAsync();
        }


        private Task Run(Operation operation)
        {
            _cts = new CancellationTokenSource();
            return Task.Factory.StartNew(() =>
            {

            },
            _cts.Token,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Current);
        }

    }
}
