using Basel.Detection.Recognizer.Dollar.Helpers;
using Microsoft.Band.Sensors;
using System;
using System.Collections.Generic;

namespace Basel.Detection.Recognizer.Dollar
{
    public class Unistroke : Gesture
    {
        public List<IBandAccelerometerReading> RawPoints;
        public List<IBandAccelerometerReading> Points;          // pre-processed points (for matching) -- created when loaded
        public List<double> Vector;                             // vector representation -- for Protractor

        /// <summary>
        /// Constructor of a unistroke gesture. A unistroke is comprised of a set of points drawn
        /// out over time in a sequence.
        /// </summary>
        /// <param name="name">The name of the unistroke gesture.</param>
        /// <param name="timepoints">The array of points supplied for this unistroke.</param>
        public Unistroke(string name, List<IBandAccelerometerReading> timepoints)
        {
            Name = name;
            RawPoints = new List<IBandAccelerometerReading>(timepoints); // copy (saved for drawing)
            var interval = timepoints.PathLength() / (DollarRecognizer.NumPoints - 1); // interval distance between points
            Points = timepoints.ResampleInSpace( interval);
            var radians = Points.Centroid().Angle( Points[0], false);
            Points = Points.RotatePoints(-radians);
            Points = Points.ScaleTo(DollarRecognizer.SquareSize);
            Points = Points.TranslateTo( DollarRecognizer.Origin, true);
            Vector = Vectorize(Points); // vectorize resampled points (for Protractor)
        }

        /// <summary>
        /// Vectorize the unistroke according to the algorithm by Yang Li for use in the Protractor extension to $1.
        /// </summary>
        /// <param name="points">The resampled points in the gesture to vectorize.</param>
        /// <returns>A vector of cosine distances.</returns>
        /// <seealso cref="http://yangl.org/protractor/"/>
        public static List<double> Vectorize(List<IBandAccelerometerReading> points)
        {
            var sum = 0.0;
            List<double> vector = new List<double>(points.Count * 2);
            for (int i = 0; i < points.Count; i++)
            {
                vector.Add(points[i].AccelerationX);
                vector.Add(points[i].AccelerationY);
                sum += points[i].AccelerationX * points[i].AccelerationX + points[i].AccelerationY * points[i].AccelerationY;
            }
            var magnitude = Math.Sqrt(sum);
            for (int i = 0; i < vector.Count; i++)
                vector[i] /= magnitude;
            return vector;
        }

        /// <summary>
        /// Gets the duration in milliseconds of this gesture.
        /// </summary>
        public long Duration
        {
            get { return (RawPoints.Count >= 2) ? RawPoints[RawPoints.Count - 1].Timestamp.Ticks - RawPoints[0].Timestamp.Ticks : 0L; }
        }


    }

}
