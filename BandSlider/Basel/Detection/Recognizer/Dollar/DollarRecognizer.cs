using System;
using System.Collections.Generic;
using Microsoft.Band.Sensors;
using Basel.Detection.Helpers;
using Basel.SensorReadings;
using Recognizer.Dollar;
using System.Linq;

namespace Basel.Detection.Recognizer.Dollar
{
    /// <summary>
    /// https://depts.washington.edu/aimgroup/proj/dollar/
    /// </summary>
    public class DollarRecognizer : Recognizer
    {
        #region Members

        public const int NumPoints = 64;
        private const float DX = 250f;
        public static readonly SizeF SquareSize = new SizeF(DX, DX);
        public static readonly double Diagonal = Math.Sqrt(DX * DX + DX * DX);
        public static readonly double HalfDiagonal = 0.5 * Diagonal;
        public static readonly IBandAccelerometerReading Origin = new BaselBandAccelerometerReading();
        private static readonly double Phi = 0.5 * (-1.0 + Math.Sqrt(5.0)); // Golden Ratio
        

        #endregion

        #region Constructor

        public DollarRecognizer()
        {
        }

        #endregion

        #region Recognition

        /// <summary>
        /// 
        /// </summary>
        /// <param name="readings"></param>
        /// <param name="protractor"></param>
        /// <returns></returns>
        public override IGesture Recognize(List<IBandAccelerometerReading> readings) // candidate points
        {
            bool protractor = false;
            double intervall = readings.PathLength() / (NumPoints - 1); // interval distance between points
            List<IBandAccelerometerReading> points = readings.ResampleInSpace(intervall);
            double radians = points.Centroid().Angle(points[0], false);
            points = points.RotatePoints(-radians);
            points = points.ScaleTo(SquareSize);
            points = points.TranslateTo(Origin, true);
            List<double> vector = Unistroke.Vectorize(points); // candidate's vector representation

            var nbest = new NBestList();
            foreach (var u in _gestures.Values.OfType<Unistroke>())
            {
                if (protractor) // Protractor extension by Yang Li (CHI 2010)
                {
                    double[] best = OptimalCosineDistance(u.Vector, vector);
                    double score = 1.0 / best[0];
                    nbest.AddResult(u.Name, score, best[0], best[1]); // name, score, distance, angle
                }
                else // original $1 angular invariance search -- Golden Section Search (GSS)
                {
                    double[] best = GoldenSectionSearch(
                            points,                             // to rotate
                            u.Points,                           // to match
                            DetectionExtensions.Degrees2Radians(-45.0),   // lbound
                            DetectionExtensions.Degrees2Radians(+45.0),   // ubound
                            DetectionExtensions.Degrees2Radians(2.0)      // threshold
                        );

                    double score = 1.0 - best[0] / HalfDiagonal;
                    nbest.AddResult(u.Name, score, best[0], best[1]); // name, score, distance, angle
                }
            }
            nbest.SortDescending(); // sort descending by score so that nbest[0] is best result
            var res = _gestures.FirstOrDefault(x => x.Key == nbest.Name);
            return res.Key != null ? res.Value : null;
        }

        // From http://www.math.uic.edu/~jan/mcs471/Lec9/gss.pdf
        private double[] GoldenSectionSearch(List<IBandAccelerometerReading> pts1, List<IBandAccelerometerReading> pts2, double a, double b, double threshold)
        {
            double x1 = Phi * a + (1 - Phi) * b;
            List<IBandAccelerometerReading> newPoints = pts1.RotatePoints( x1);
            double fx1 = PathDistance(newPoints, pts2);

            double x2 = (1 - Phi) * a + Phi * b;
            newPoints = pts1.RotatePoints( x2);
            double fx2 = PathDistance(newPoints, pts2);

            double i = 2.0; // calls to pathdist
            while (Math.Abs(b - a) > threshold)
            {
                if (fx1 < fx2)
                {
                    b = x2;
                    x2 = x1;
                    fx2 = fx1;
                    x1 = Phi * a + (1 - Phi) * b;
                    newPoints = pts1.RotatePoints(x1);
                    fx1 = PathDistance(newPoints, pts2);
                }
                else
                {
                    a = x1;
                    x1 = x2;
                    fx1 = fx2;
                    x2 = (1 - Phi) * a + Phi * b;
                    newPoints = pts1.RotatePoints( x2);
                    fx2 = PathDistance(newPoints, pts2);
                }
                i++;
            }
            return new double[3] { Math.Min(fx1, fx2), DetectionExtensions.Radians2Degrees((b + a) / 2.0), i }; // distance, angle, calls to pathdist
        }

        /// <summary>
        /// From Protractor by Yang Li, published at CHI 2010. See http://yangl.org/protractor/. 
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        private double[] OptimalCosineDistance(List<double> v1, List<double> v2)
        {
            double a = 0.0;
            double b = 0.0;
            for (int i = 0; i < Math.Min(v1.Count, v2.Count); i += 2)
            {
                a += v1[i] * v2[i] + v1[i + 1] * v2[i + 1];
                b += v1[i] * v2[i + 1] - v1[i + 1] * v2[i];
            }
            double angle = Math.Atan(b / a);
            double distance = Math.Acos(a * Math.Cos(angle) + b * Math.Sin(angle));
            return new double[3] { distance, DetectionExtensions.Radians2Degrees(angle), 0.0 }; // distance, angle, calls to pathdist
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path1"></param>
        /// <param name="path2"></param>
        /// <returns></returns>
        private static double PathDistance(List<IBandAccelerometerReading> path1, List<IBandAccelerometerReading> path2)
        {
            double distance = 0;
            for (int i = 0; i < Math.Min(path1.Count, path2.Count); i++)
                distance += path1[i].Distance(path2[i]);
            return distance / path1.Count;
        }

        // continues to rotate 'pts1' by 'step' degrees as long as points become ever-closer 
        // in path-distance to pts2. the initial distance is given by D. the best distance
        // is returned in array[0], while the angle at which it was achieved is in array[1].
        // array[3] contains the number of calls to PathDistance.
        private double[] HillClimbSearch(List<IBandAccelerometerReading> pts1, List<IBandAccelerometerReading> pts2, double D, double step)
        {
            double i = 0.0;
            double theta = 0.0;
            double d = D;
            do
            {
                D = d; // the last angle tried was better still
                theta += step;
                List<IBandAccelerometerReading> newPoints = pts1.RotatePoints( DetectionExtensions.Degrees2Radians(theta));
                d = PathDistance(newPoints, pts2);
                i++;
            }
            while (d <= D);
            return new double[3] { D, theta - step, i }; // distance, angle, calls to pathdist
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pts1"></param>
        /// <param name="pts2"></param>
        /// <param name="writer"></param>
        /// <returns></returns>
        private double[] FullSearch(List<IBandAccelerometerReading> pts1, List<IBandAccelerometerReading> pts2)
        {
            double bestA = 0d;
            double bestD = PathDistance(pts1, pts2);

            for (int i = -180; i <= +180; i++)
            {
                List<IBandAccelerometerReading> newPoints = pts1.RotatePoints(DetectionExtensions.Degrees2Radians(i));
                double d = PathDistance(newPoints, pts2);

                if (d < bestD)
                {
                    bestD = d;
                    bestA = i;
                }
            }
            return new double[3] { bestD, bestA, 360.0 }; // distance, angle, calls to pathdist
        }

        #endregion

       

       
    }
}