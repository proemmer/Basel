using Microsoft.Band.Sensors;
using Recognizer.Dollar;
using System.Collections.Generic;
using System.Linq;

namespace Basel.Detection.Recognizer
{
    public abstract class Recognizer : IRecognizer
    {
        protected readonly Dictionary<string, IGesture> _gestures = new Dictionary<string, IGesture>();


        public abstract IGesture Recognize(List<IBandAccelerometerReading> readings, bool protractor);

        #region Gestures

        public int NumOfGestures
        {
            get
            {
                return _gestures.Count;
            }
        }

        public List<IGesture> Gestures
        {
            get
            {
                return _gestures.Values.OrderBy(x => x).ToList();
            }
        }

        public bool AddGesture(string name, IGesture gesture)
        {
            if (!_gestures.ContainsKey(name))
            {
                _gestures.Add(name, gesture);
                return true;
            }
            return false;
        }

        public bool RemoveGesture(string name)
        {
            return _gestures.Remove(name);
        }

        public void ClearGestures()
        {
            _gestures.Clear();
        }



        #endregion
    }
}
