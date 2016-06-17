using Basel;
using Basel.Detection.Recognizer;
using Basel.Detection.Recognizer.UWave;
using Microsoft.Band;
using Microsoft.Band.Sensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace TileEvents
{
    public class GestureDetector
    {
        private const double GForceThreshold = 1.5;
        private IRecognizer _recognizer;
        private ISensorDataProducer _bandManager;
        private IBaselConfiguration _config = new BaselConfiguration() { Accelerometer = true };
        private List<IBandAccelerometerReading> _readings = new List<IBandAccelerometerReading>();
        private int _minDataForDetection = 0;
        private Action<string> _onDetected;


        public double Threshold { get; set; } = GForceThreshold;

        public GestureDetector(Action<string> onDetected)
        {
            _onDetected = onDetected;
        }

        private async Task ReadGestures()
        {
            var folder = KnownFolders.PicturesLibrary;
            _recognizer = new UWaveRecognizer();
            foreach (var file in (await folder.GetFilesAsync()).Where(x => x.Name.EndsWith(".bsd")))
            {
                string jsonText = await FileIO.ReadTextAsync(file);
                var record = JsonRecordPersistor.Deserialize(jsonText);
                var name = file.Name.Substring(0, file.Name.IndexOf("."));
                var gesture = new UWaveGesture(name, record.Accelerometer.SkipWhile(reading => !InRange(reading)).TakeWhile(reading => InRange(reading)).ToList());  //TODO!!! ->  
                _recognizer.AddGesture(name, gesture);

                if (_minDataForDetection < gesture.Length)
                    _minDataForDetection = gesture.Length;
            }

            if (_recognizer.Gestures.Any())
                _bandManager = new BandManager(BandClientManager.Instance, _config);
            
        }


        public async Task StartDetectionAsync()
        {
            await ReadGestures();

            _bandManager.OnAccelerometerSensorUpdate += _bandManager_OnAccelerometerSensorUpdate;
            await _bandManager.StartAsync();
        }

       
        public async Task StopDetectionAsync()
        {
            await _bandManager.StopAsync();
        }


        private void _bandManager_OnAccelerometerSensorUpdate(object sender, Microsoft.Band.Sensors.BandSensorReadingEventArgs<IBandAccelerometerReading> e)
        {
            var test = -Threshold;
            if (InRange(e.SensorReading))
            {
                _readings.Add(e.SensorReading);
                if (_readings.Count >= _minDataForDetection)
                {
                    var gesture = _recognizer.Recognize(_readings, false);
                    if (gesture != null)
                    {
                        _readings.Clear();
                        OnGestureDetected(gesture.Name);
                    }
                    else
                        _readings.RemoveAt(0);
                }
            }
            else if(_readings.Any())
                _readings.Clear();
        }


        private bool InRange(IBandAccelerometerReading reading)
        {
            return (reading.AccelerationX >= Threshold || reading.AccelerationY >= Threshold || reading.AccelerationZ >= Threshold ||
                reading.AccelerationX <= -Threshold || reading.AccelerationY <= -Threshold || reading.AccelerationZ <= -Threshold);
        }

        private async void OnGestureDetected(string command)
        {
            await Task.Run(() =>
            {
                _onDetected?.Invoke(command);
            });
        }

    }
}
