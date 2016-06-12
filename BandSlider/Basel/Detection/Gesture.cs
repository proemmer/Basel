using System;

namespace Basel.Detection
{
    public abstract class Gesture : IComparable, IGesture
    {
        public string Name { get; protected set; }


        /// <summary>
        /// Sort comparator in descending order of score.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual int CompareTo(object obj)
        {
            var gesture = obj as IGesture;
            if (gesture != null)
                return Name.CompareTo(gesture.Name);
            throw new ArgumentException("object is not a Gesture");
        }
    }
}
