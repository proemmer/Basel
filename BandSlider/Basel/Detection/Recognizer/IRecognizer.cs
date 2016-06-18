using Microsoft.Band.Sensors;
using Recognizer.Dollar;
using System.Collections.Generic;

namespace Basel.Detection.Recognizer
{
    public interface IRecognizer
    {

        IGesture Recognize(List<IBandAccelerometerReading> readings);

        int NumOfGestures { get; }
        List<IGesture> Gestures { get; }
        bool AddGesture(string name, IGesture gesture);
        bool RemoveGesture(string name);
        void ClearGestures();
    }
}