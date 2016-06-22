using Basel.SensorReadings;
using Microsoft.Band.Sensors;
using System;
using System.Collections.Generic;

namespace Basel.Detection.Recognizer.Dollar.Helpers
{
    public static class DollarDetectionExtensions
    {
        public static List<IBandAccelerometerReading> ResampleInSpace(this List<IBandAccelerometerReading> points, double px)
        {
            List<IBandAccelerometerReading> srcPts = new List<IBandAccelerometerReading>(points);
            List<IBandAccelerometerReading> dstPts = new List<IBandAccelerometerReading>();
            dstPts.Add(srcPts[0]);
            double D = 0.0;
            for (int i = 1; i < srcPts.Count; i++)
            {
                IBandAccelerometerReading pt = srcPts[i - 1];
                IBandAccelerometerReading pt2 = srcPts[i];
                double d = pt.Distance( pt2);
                if (D + d >= px)
                {
                    double qx = (double)pt.AccelerationX + (px - D) / d * (double)(pt2.AccelerationX - pt.AccelerationX);
                    double qy = (double)pt.AccelerationY + (px - D) / d * (double)(pt2.AccelerationY - pt.AccelerationY);
                    var q = new BaselBandAccelerometerReading
                    {
                        AccelerationX = qx,
                        AccelerationY = qy
                    } as IBandAccelerometerReading;
                    dstPts.Add(q);
                    srcPts.Insert(i, q);
                    D = 0.0;
                }
                else
                {
                    D += d;
                }
            }
            if (D > 0.0)
                dstPts.Add(srcPts[srcPts.Count - 1]);
            return dstPts;
        }

        public static double Distance(this IBandAccelerometerReading p1, IBandAccelerometerReading p2)
        {
            double dx = p2.AccelerationX - p1.AccelerationX;
            double dy = p2.AccelerationY - p1.AccelerationY;
            double dz = p2.AccelerationZ - p1.AccelerationZ;
            return Math.Sqrt(dx * dx + dy * dy /*+ dz * dz*/);  //3d
        }

        public static double PathLength(this List<IBandAccelerometerReading> points)
        {
            double dx = 0.0;
            for (int i = 1; i < points.Count; i++)
                dx += Distance(points[i - 1], points[i]);
            return dx;
        }

        public static double Angle(this IBandAccelerometerReading start, IBandAccelerometerReading end, bool positiveOnly)
        {
            double radians = 0.0;
            if (start.AccelerationX != end.AccelerationX)
            {
                radians = Math.Atan2(end.AccelerationY - start.AccelerationY, end.AccelerationX - start.AccelerationX);
            }
            else
            {
                if (end.AccelerationY < start.AccelerationY)
                {
                    radians = -1.5707963267948966;
                }
                else
                {
                    if (end.AccelerationY > start.AccelerationY)
                    {
                        radians = 1.5707963267948966;
                    }
                }
            }
            if (positiveOnly && radians < 0.0)
                radians += 6.2831853071795862;
            return radians;
        }

        public static IBandAccelerometerReading Centroid(this List<IBandAccelerometerReading> points)
        {
            double xsum = 0f;
            double ysum = 0f;
            double zsum = 0f;
            foreach (IBandAccelerometerReading p in points)
            {
                xsum += p.AccelerationX;
                ysum += p.AccelerationY;
                zsum += p.AccelerationZ;
            }
            return new BaselBandAccelerometerReading
            {
                AccelerationX = xsum / points.Count,
                AccelerationY = ysum / points.Count,
                AccelerationZ = zsum / points.Count
            };
        }

        public static List<IBandAccelerometerReading> RotatePoints(this List<IBandAccelerometerReading> points, double radians)
        {
            List<IBandAccelerometerReading> newPoints = new List<IBandAccelerometerReading>(points.Count);
            IBandAccelerometerReading c = Centroid(points);
            var cos = Math.Cos(radians);
            var sin = Math.Sin(radians);
            var cx = c.AccelerationX;
            var cy = c.AccelerationY;
            for (int i = 0; i < points.Count; i++)
            {
                IBandAccelerometerReading p = points[i];
                var dx = p.AccelerationX - cx;
                var dy = p.AccelerationY - cy;
                IBandAccelerometerReading q = new BaselBandAccelerometerReading
                {
                    AccelerationX = dx * cos - dy * sin + cx,
                    AccelerationY = dx * sin + dy * cos + cy
                };
                newPoints.Add(q);
            }
            return newPoints;
        }

        public static List<IBandAccelerometerReading> ScaleTo(this List<IBandAccelerometerReading> points, SizeF size)
        {
            List<IBandAccelerometerReading> newPoints = new List<IBandAccelerometerReading>(points.Count);
            RectangleF r = FindBoundingBox(points);
            for (int i = 0; i < points.Count; i++)
            {
                BaselBandAccelerometerReading p = points[i] as BaselBandAccelerometerReading;
                if (r.Width != 0.0)
                    p.AccelerationX *= size.Width / r.Width;
                if (r.Height != 0.0)
                    p.AccelerationY *= size.Height / r.Height;
                newPoints.Add(p);
            }
            return newPoints;
        }

        public static RectangleF FindBoundingBox(this List<IBandAccelerometerReading> points)
        {
            float minX = 3.40282347E+38f;
            float maxX = -3.40282347E+38f;
            float minY = 3.40282347E+38f;
            float maxY = -3.40282347E+38f;
            foreach (IBandAccelerometerReading p in points)
            {
                if (p.AccelerationX < minX)
                {
                    minX = (float)p.AccelerationX;
                }
                if (p.AccelerationX > maxX)
                {
                    maxX = (float)p.AccelerationX;
                }
                if (p.AccelerationY < minY)
                {
                    minY = (float)p.AccelerationY;
                }
                if (p.AccelerationY > maxY)
                {
                    maxY = (float)p.AccelerationY;
                }
            }
            return new RectangleF(minX, minY, maxX - minX, maxY - minY);
        }

        public static List<IBandAccelerometerReading> TranslateTo(this List<IBandAccelerometerReading> points, IBandAccelerometerReading toPt, bool centroid)
        {
            List<IBandAccelerometerReading> newPoints = new List<IBandAccelerometerReading>(points.Count);
            if (centroid)
            {
                IBandAccelerometerReading ctr = Centroid(points);
                for (int i = 0; i < points.Count; i++)
                {
                    BaselBandAccelerometerReading p = points[i] as BaselBandAccelerometerReading;
                    p.AccelerationX += toPt.AccelerationX - ctr.AccelerationX;
                    p.AccelerationY += toPt.AccelerationY - ctr.AccelerationY;
                    newPoints.Add(p);
                }
            }
            else
            {
                RectangleF r = FindBoundingBox(points);
                for (int j = 0; j < points.Count; j++)
                {
                    BaselBandAccelerometerReading p2 = points[j] as BaselBandAccelerometerReading;
                    p2.AccelerationX += toPt.AccelerationX - r.X;
                    p2.AccelerationY += toPt.AccelerationY - r.Y;
                    newPoints.Add(p2);
                }
            }
            return newPoints;
        }

        public static double Radians2Degrees(double radians)
        {
            return radians * 180.0 / 3.1415926535897931;
        }
        public static double Degrees2Radians(double degrees)
        {
            return degrees * 3.1415926535897931 / 180.0;
        }

        public static List<IBandAccelerometerReading> RotateByRadians(List<IBandAccelerometerReading> points, double radians)
        {
            var newPoints = new List<IBandAccelerometerReading>(points.Count);
            IBandAccelerometerReading c = Centroid(points);

            double cos = Math.Cos(radians);
            double sin = Math.Sin(radians);

            double cx = c.AccelerationX;
            double cy = c.AccelerationY;

            for (int i = 0; i < points.Count; i++)
            {
                var p = points[i];

                double dx = p.AccelerationX - cx;
                double dy = p.AccelerationY - cy;

                var q = new BaselBandAccelerometerReading
                {
                    AccelerationX = dx * cos - dy * sin + cx,
                    AccelerationY = dx * sin + dy * cos + cy
                };
                newPoints.Add(q);
            }
            return newPoints;
        }

    }
}
