using Basel;
using Basel.Detection.Recognizer;
using Basel.Detection.Recognizer.UWave;
using Microsoft.Band;
using Microsoft.Band.Sensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basel.Detection.Detectors
{
    public class AccelerometerGestureDetector : Detector
    {
        private const double GForceThreshold = 0.0;
        private IRecognizer _recognizer = new UWaveRecognizer();
        private readonly ISensorDataProducer _producer;
        private IBaselConfiguration _config = new BaselConfiguration() { Accelerometer = true };
        private List<IBandAccelerometerReading> _readings = new List<IBandAccelerometerReading>();
        private int _minDataForDetection = Int32.MaxValue;



        public double Threshold { get; set; } = GForceThreshold;

        public AccelerometerGestureDetector(ISensorDataProducer producer, IBaselConfiguration configuration) : base(producer, configuration)
        {
            _producer = producer;
            configuration.Accelerometer = true;
            
        }

        public override void AddRecordAsGesture(string name, IRecord record, Action onDetected)
        {
            var gesture = new UWaveGesture(name, record.Accelerometer.SkipWhile(reading => !InRange(reading)).TakeWhile(reading => InRange(reading)).ToList());
            AddGesture(gesture, onDetected);
        }

        public override void AddGesture(IGesture gesture, Action onDetected)
        {
            if (_minDataForDetection > gesture.Length)
                _minDataForDetection = gesture.Length;
            _recognizer.AddGesture(gesture.Name, gesture);
            base.AddGesture(gesture, onDetected);
        }

        public async Task StartDetectionAsync()
        {
            _producer.OnAccelerometerSensorUpdate += Producer_OnAccelerometerSensorUpdate;
            await _producer.StartAsync();
        }

       
        public async Task StopDetectionAsync()
        {
            await _producer.StopAsync();
            _producer.OnAccelerometerSensorUpdate -= Producer_OnAccelerometerSensorUpdate;
        }


        private void Producer_OnAccelerometerSensorUpdate(object sender, BandSensorReadingEventArgs<IBandAccelerometerReading> e)
        {
            var test = -Threshold;
            if (InRange(e.SensorReading))
            {
                _readings.Add(e.SensorReading);
                if (_readings.Count >= _minDataForDetection)
                {
                    var gesture = _recognizer.Recognize(_readings);
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
                var gesture = _gestures.FirstOrDefault(x => x.Key.Name == command);
                if (gesture.Key != null)
                    gesture.Value?.Invoke();
            });
        }


    }
}
