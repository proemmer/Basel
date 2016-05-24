using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basel.Filters
{
    public class LowPassFilter : IFilter
    {
        /// <summary> 
        ///  This is the smoothing factor used for the 1st order discrete Low-Pass filter 
        ///  The cut-off frequency fc = fs * K/(2*PI*(1-K)) 
        /// </summary> 
        public double LowPassFilterCoef { get; set; } = 0.1; // With a 50Hz sampling rate, this is gives a 1Hz cut-off 


        /// <summary> 
        /// 1st order discrete low-pass filter used to remove noise from raw accelerometer. 
        /// </summary> 
        /// <param name="newInputValue">New input value (latest sample)</param> 
        /// <param name="priorOutputValue">The previous output value (filtered, one sampling period ago)</param> 
        /// <returns>The new output value</returns> 
        public double Apply(double inputValue, double priorOutputValue) 
        {
            return priorOutputValue + LowPassFilterCoef * (inputValue - priorOutputValue);
        }
    }
}
