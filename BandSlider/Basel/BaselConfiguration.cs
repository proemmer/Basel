namespace Basel
{
    public class BaselConfiguration : IBaselConfiguration
    {
        public bool Accelerometer { get; set; }
        public bool Altimeter { get; set; }
        public bool AmbientLight { get; set; }
        public bool Barometer { get; set; }
        public bool Calories { get; set; }
        public bool Contact { get; set; }
        public bool Distance { get; set; }
        public bool Gsr { get; set; }
        public bool Gyroscope { get; set; }
        public bool HeartRate { get; set; }
        public bool Pedometer { get; set; }
        public bool RRInterval { get; set; }
        public bool SkinTemperature { get; set; }
        public bool UV { get; set; }
    }
}
