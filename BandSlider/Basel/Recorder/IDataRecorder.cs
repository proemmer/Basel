using System.Threading.Tasks;

namespace Basel.Recorder
{

    public enum RecorderState{ Stopped, Recoring, Pausing }
    public interface IDataRecorder
    {

        IRecord Record { get; }
        /// <summary>
        /// Create a new IRecord (if not in state paused) and collect data
        /// </summary>
        /// <returns></returns>
        Task<bool> StartAsync();

        /// <summary>
        /// Stop the recording and return the recorded IRecord
        /// </summary>
        /// <returns></returns>
        Task<bool> StopAsync();

        /// <summary>
        /// Pause recording or playing
        /// </summary>
        /// <returns></returns>
        Task<bool> PauseAsync();

        /// <summary>
        /// Reset the current record, clear the record
        /// </summary>
        /// <returns></returns>
        Task<bool> ResetAsync();
    }

}