using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Basel
{
    public class DataRecorder : IDataRecorder, ISensorDataConsumer
    {
        private readonly IBandManager _bandManager;
        private IRecord _record;
        private CancellationTokenSource _cts;
        private enum OperationType { Playing, Recoring };

        private class Operation
        {
            public OperationType OperationType { get; set; }
            public IRecord Record { get; set; }
        }

        public DataRecorder(IBandManager manager)
        {
            if (manager == null)
                throw new ArgumentNullException("manager");
            _bandManager = manager;
        }

        public RecorderState RecorderState { get; private set; } = RecorderState.None;

        public Task<bool> PauseAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> ResetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> StartPlayingAsync(IRecord record, double speed = 1, bool loop = false)
        {
            if (_record == null)
                throw new InvalidOperationException("Start is not allowed, there is already an active record!");

            if (record == null)
                throw new ArgumentNullException("record");

            return _bandManager.StartAsync();
        }

        public Task<bool> RestartPlayingAsync(double speed = 1, bool loop = false)
        {
            if (_record == null)
                throw new InvalidOperationException("Restart is not allowed, you have to call start first!");

            return _bandManager.StartAsync();
        }

        public Task<bool> StopPlayingAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> StartRecordingAsync()
        {
            return _bandManager.StartAsync();
        }

        public async Task<IRecord> StopRecordingAsync()
        {
            if(await _bandManager.StopAsync())
            {
                //TODO
            }
            return null;
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

        public Task<bool> StartAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> StopAsync()
        {
            throw new NotImplementedException();
        }
    }
}
