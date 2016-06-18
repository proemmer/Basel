using System;
using System.Threading.Tasks;

namespace Basel.Detection.Detectors
{
    public interface IDetector
    {
        void AddRecordAsGesture(string name, IRecord record, Action onDetected);
        void AddGesture(IGesture gesture, Action onDetected);
        void RemoveGesture(IGesture gesture);
        Task<bool> StartAsync();
        Task<bool> StopAsync();
    }
}