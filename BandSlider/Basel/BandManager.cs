using Microsoft.Band;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Basel
{
    public class BandManager : IDisposable
    {
        private readonly IBandClientManager _bandClientManager;
        private IBandClient _bandClient;

        public BandManager(IBandClientManager bandClientManager)
        {
            if (bandClientManager == null)
                throw new ArgumentNullException("bandClientManager");
            _bandClientManager = bandClientManager;
        }

        public async Task<int> StartMonitoring()
        {

            try
            {
                // Get the list of Microsoft Bands paired to the phone.
                IBandInfo[] pairedBands = await _bandClientManager.GetBandsAsync();
                if (pairedBands.Length < 1)
                {
                    return await Task.FromResult(-1);
                }

                _bandClient = await _bandClientManager.ConnectAsync(pairedBands[0]);

                _bandClient.SensorManager.Contact.ReadingChanged += (s, args) =>
                {
                    //args.SensorReading.State
                    //sb.AppendLine($"x:{args.SensorReading.AccelerationX}; y:{args.SensorReading.AccelerationY}; z:{args.SensorReading.AccelerationZ}");
                };

                _bandClient.SensorManager.Accelerometer.ReadingChanged += (s, args) =>
                {
                    //sb.AppendLine($"x:{args.SensorReading.AccelerationX}; y:{args.SensorReading.AccelerationY}; z:{args.SensorReading.AccelerationZ}");
                };

                _bandClient.SensorManager.Gyroscope.ReadingChanged += (s, args) =>
                {
                    //sb.AppendLine($"x:{args.SensorReading.AccelerationX}; y:{args.SensorReading.AccelerationY}; z:{args.SensorReading.AccelerationZ}");
                };

                await _bandClient.SensorManager.Accelerometer.StartReadingsAsync();
            }
            catch (Exception ex)
            {
                return await Task.FromResult(-1);
            }
            return await Task.FromResult(0);
        }

        public async Task<int> StopMonitoring()
        {
            try
            {
                if(_bandClient != null)
                    await _bandClient.SensorManager.Accelerometer.StopReadingsAsync();
            }
            catch (Exception ex)
            {
                return await Task.FromResult(-1);
            }
            return await Task.FromResult(0);
        }

        public void Dispose()
        {
            if (_bandClient != null)
                _bandClient.Dispose();
        }
    }
}
