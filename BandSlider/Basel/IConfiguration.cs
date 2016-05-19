namespace Basel
{
    public interface IBaselConfiguration
    {
        bool Accelerometer { get; set; }
        bool Altimeter { get; set; }
        bool AmbientLight { get; set; }
        bool Barometer { get; set; }
        bool Calories { get; set; }
        bool Contact { get; set; }
        bool Distance { get; set; }
        bool Gsr { get; set; }
        bool Gyroscope { get; set; }
        bool HeartRate { get; set; }
        bool Pedometer { get; set; }
        bool RRInterval { get; set; }
        bool SkinTemperature { get; set; }
        bool UV { get; set; }
    }
}