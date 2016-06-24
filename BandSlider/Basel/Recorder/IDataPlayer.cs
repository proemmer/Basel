using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basel.Recorder
{
    public enum PlayerState { Stopped, Playing, Pausing }
    public interface IDataPlayer
    {
        IRecord Record { get; set; }

        /// Start playing the given record.
        /// </summary>
        /// <param name="record">given record to play</param>
        /// <param name="speed">multiplication factor of the play speed</param>
        /// <param name="loop">restart the play after end is reached</param>
        /// <returns></returns>
        Task<bool> StartAsync();

        /// <summary>
        /// Pause recording or playing
        /// </summary>
        /// <returns></returns>
        Task<bool> PauseAsync();


        /// <summary>
        /// Stop playing, and set position to begin
        /// </summary>
        /// <returns></returns>
        Task<bool> StopAsync();


        double Speed { get; set; }

        bool Loop { get; set; }


    }
}
