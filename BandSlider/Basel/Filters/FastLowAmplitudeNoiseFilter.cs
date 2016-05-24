using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basel.Filters
{
    public class FastLowAmplitudeNoiseFilter : IFilter
    {
        /// <summary> 
        ///  This is the smoothing factor used for the 1st order discrete Low-Pass filter 
        ///  The cut-off frequency fc = fs * K/(2*PI*(1-K)) 
        /// </summary> 
        public double LowPassFilterCoef { get; set; } = 0.1; // With a 50Hz sampling rate, this is gives a 1Hz cut-off 

        /// <summary> 
        /// Maximum amplitude of noise from sample to sample.  
        /// This is used to remove the noise selectively while allowing fast trending for larger amplitudes 
        /// </summary> 
        public double NoiseMaxAmplitude { get; set; } = 0.05; // up to 0.05g deviation from filtered value is considered noise 


        /// <summary> 
        /// discrete low-magnitude fast low-pass filter used to remove noise from raw accelerometer while allowing fast trending on high amplitude changes 
        /// </summary> 
        /// <param name="newInputValue">New input value (latest sample)</param> 
        /// <param name="priorOutputValue">The previous (n-1) output value (filtered, one sampling period ago)</param> 
        /// <returns>The new output value</returns> 
        public double Apply(double inputValue, double priorOutputValue)
        {
            double result = inputValue;
            if (Math.Abs(inputValue - priorOutputValue) <= NoiseMaxAmplitude)
            {   // Simple low-pass filter 
                result = priorOutputValue + LowPassFilterCoef * (inputValue - priorOutputValue);
            }
            return result;
        }

    }
}
