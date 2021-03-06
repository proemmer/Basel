﻿using Microsoft.Band.Sensors;
using System.Collections.Generic;
using System.Linq;
using Basel.SensorReadings;

namespace Basel.Detection.Recognizer.UWave
{
    /// <summary>
    /// http://zhen-wang.appspot.com/rice/projects_uWave.html
    /// </summary>
    public class UWaveRecognizer : Recognizer
    {
        //(8,4) applies to 100Hz; if 50Hz, change to (4,2)...

        public int QuanWinSize { get; set; } = 4;
        public int QuanMoveStep { get; set; } = 2;
        public double MaxDistance { get; set; } = 1.0;

        public override IGesture Recognize(List<IBandAccelerometerReading> readings)
        {
            var accIndex = 0;
            var readingsCopy = readings.ToList();
            accIndex = QuantizeAcc(readingsCopy);

            var gestures = _gestures.Values.OfType<UWaveGesture>().ToList();
            foreach (var gesture in gestures)
                gesture.Length = QuantizeAcc(gesture.Readings);

            var ret = DetectGesture(readingsCopy, accIndex, gestures, MaxDistance);
            return ret >= 0 ?  gestures[ret] : null;
        }



        private int DetectGesture(List<IBandAccelerometerReading> readings, int length, List<UWaveGesture> gestures, double maxDistance)
        {
            if (length <= 0)
                return -1;
            int ret = 0;
            var distances = new List<double>();
            foreach (var gesture in gestures)
            {
                var table = new Dictionary<int, double>();
                for (var j = 0; j < length * gesture.Length; j++)
                    table[j] = -1;

                var distance = DTWdistance(readings, length, gesture.Readings, gesture.Length, length - 1, gesture.Length - 1, table);
                distance /= (length + gesture.Length);
                distances.Add(distance);
            }

            for (int i = 0; i < gestures.Count; i++)
            {
                if (distances[i] < distances[ret])
                    ret = i;
            }

            return distances[ret] <= maxDistance ? ret : -1;
        }

        private int QuantizeAcc(List<IBandAccelerometerReading> readings)
        {
            var i = 0;
            var k = 0;
            var window = QuanWinSize;
            var temp = new BaselBandAccelerometerReading[readings.Count / QuanMoveStep + 1];
            //take moving window average
            while (i < readings.Count)
            {
                if (i + window > readings.Count)
                    window = readings.Count - i;
                var sumX = 0.0;
                var sumY = 0.0;
                var sumZ = 0.0;
                for (var j = i; j < window + i; j++)
                {
                    sumX += readings[j].AccelerationX;
                    sumY += readings[j].AccelerationY;
                    sumZ += readings[j].AccelerationZ;
                }

                temp[k] = new BaselBandAccelerometerReading
                {
                    AccelerationX = sumX * 1.0 / window,
                    AccelerationY = sumY * 1.0 / window,
                    AccelerationZ = sumZ * 1.0 / window
                };
                k++;
                i += QuanMoveStep;
            }//while


            //nonlinear quantization and copy quantized value to original buffer 	
            for (i = 0; i < k; i++)
            {
                if (temp[i].AccelerationX > 10)
                {
                    if (temp[i].AccelerationX > 20)
                        temp[i].AccelerationX = 16;
                    else
                        temp[i].AccelerationX = 10 + (temp[i].AccelerationX - 10) / 10 * 5;
                }
                else if (temp[i].AccelerationX < -10)
                {
                    if (temp[i].AccelerationX < -20)
                        temp[i].AccelerationX = -16;
                    else
                        temp[i].AccelerationX = -10 + (temp[i].AccelerationX + 10) / 10 * 5;
                }

                if (temp[i].AccelerationY > 10)
                {
                    if (temp[i].AccelerationY > 20)
                        temp[i].AccelerationY = 16;
                    else
                        temp[i].AccelerationY = 10 + (temp[i].AccelerationY - 10) / 10 * 5;
                }
                else if (temp[i].AccelerationY < -10)
                {
                    if (temp[i].AccelerationY < -20)
                        temp[i].AccelerationY = -16;
                    else
                        temp[i].AccelerationY = -10 + (temp[i].AccelerationY + 10) / 10 * 5;
                }

                if (temp[i].AccelerationZ > 10)
                {
                    if (temp[i].AccelerationZ > 20)
                        temp[i].AccelerationZ = 16;
                    else
                        temp[i].AccelerationZ = 10 + (temp[i].AccelerationZ - 10) / 10 * 5;
                }
                else if (temp[i].AccelerationZ < -10)
                {
                    if (temp[i].AccelerationZ < -20)
                        temp[i].AccelerationZ = -16;
                    else
                        temp[i].AccelerationZ = -10 + (temp[i].AccelerationZ + 10) / 10 * 5;
                }

                readings[i] = new BaselBandAccelerometerReading
                {
                    AccelerationX = temp[i].AccelerationX,
                    AccelerationY = temp[i].AccelerationY,
                    AccelerationZ = temp[i].AccelerationZ

                };
            }

            return k;
        }

        private double DTWdistance(List<IBandAccelerometerReading> sample1, int length1 , List<IBandAccelerometerReading> sample2, int length2, int i, int j, Dictionary<int, double> table)
        {
            if (i < 0 || j < 0)
                return 100000000;
            int tableWidth = length2;
            double localDistance = 0.0;

            localDistance += ((sample1[i].AccelerationX - sample2[j].AccelerationX) * (sample1[i].AccelerationX - sample2[j].AccelerationX));
            localDistance += ((sample1[i].AccelerationY - sample2[j].AccelerationY) * (sample1[i].AccelerationY - sample2[j].AccelerationY));
            localDistance += ((sample1[i].AccelerationZ - sample2[j].AccelerationZ) * (sample1[i].AccelerationZ - sample2[j].AccelerationZ));

            double sdistance, s1, s2, s3;

            if(i == 0 && j == 0)
            {
                if (table[i * tableWidth + j] < 0)
                    table[i * tableWidth + j] = localDistance;
                return localDistance;
            }
            else if( i == 0)
            {
                if (table[i * tableWidth + (j - 1)] < 0)
                    sdistance = DTWdistance(sample1, length1, sample2, length2, i, j - 1, table);
                else
                    sdistance = table[i * tableWidth + j - 1];
            }
            else if (j == 0)
            {
                if (table[(i - 1) * tableWidth + j] < 0)
                    sdistance = DTWdistance(sample1, length1, sample2, length2, i - 1, j, table);
                else
                    sdistance = table[(i - 1) * tableWidth + j];
            }
            else
            {
                if (table[i * tableWidth + (j - 1)] < 0)
                    s1 = DTWdistance(sample1, length1, sample2, length2, i, j - 1, table);
                else
                    s1 = table[i * tableWidth + (j - 1)];
                if (table[(i - 1) * tableWidth + j] < 0)
                    s2 = DTWdistance(sample1, length1, sample2, length2, i - 1, j, table);
                else
                    s2 = table[(i - 1) * tableWidth + j];
                if (table[(i - 1) * tableWidth + j - 1] < 0)
                    s3 = DTWdistance(sample1, length1, sample2, length2, i - 1, j - 1, table);
                else
                    s3 = table[(i - 1) * tableWidth + j - 1];
                sdistance = s1 < s2 ? s1 : s2;
                sdistance = sdistance < s3 ? sdistance : s3;
            }
            table[i * tableWidth + j] = localDistance + sdistance;
            return table[i * tableWidth + j];
        }
    }
}
