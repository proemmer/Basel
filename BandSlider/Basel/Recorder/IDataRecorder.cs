using System.Threading.Tasks;

namespace Basel
{

    public enum RecorderState{ None, Playing, Recording, PausePlaying, PauseRecording}
    public interface IDataRecorder
    {
        /// <summary>
        /// Start playing the given record.
        /// </summary>
        /// <param name="record">given record to play</param>
        /// <param name="speed">multiplication factor of the play speed</param>
        /// <param name="loop">restart the play after end is reached</param>
        /// <returns></returns>
        Task<bool> StartPlayingAsync(IRecord record, double speed = 1.0 ,bool loop = false);

        /// <summary>
        /// Start playing the last given record if not stopped.
        /// </summary>
        /// <param name="speed">multiplication factor of the play speed</param>
        /// <param name="loop">restart the play after end is reached</param>
        /// <returns></returns>
        Task<bool> RestartPlayingAsync(double speed = 1.0, bool loop = false);

        /// <summary>
        /// Stop playing, and set position to begin
        /// </summary>
        /// <returns></returns>
        Task<bool> StopPlayingAsync();

        /// <summary>
        /// Create a new IRecord (if not in state paused) and collect data
        /// </summary>
        /// <returns></returns>
        Task<bool> StartRecordingAsync();

        /// <summary>
        /// Stop the recording and return the recorded IRecord
        /// </summary>
        /// <returns></returns>
        Task<IRecord> StopRecordingAsync();

        /// <summary>
        /// Pause recording or playing
        /// </summary>
        /// <returns></returns>
        Task<bool> PauseAsync();
        
        /// <summary>
        /// Holds the current state of the recorder
        /// </summary>
        RecorderState RecorderState { get; }

        /// <summary>
        /// Reset the current record, clear the record
        /// </summary>
        /// <returns></returns>
        Task<bool> ResetAsync();
    }
}