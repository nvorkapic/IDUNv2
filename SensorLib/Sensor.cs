namespace SensorLib
{
    public enum SensorState
    {
        Normal,
        Faulted
    }

    public class Sensor
    {
        /// <summary>
        /// Size of databuffer. Must be a power of two.
        /// </summary>
        public const int BUFFER_SIZE = 128;

        private float[] valueBuffer = new float[BUFFER_SIZE];
        private int valueBufferIdx;

        public string Id { get; set; }
        public string DeviceName { get; set; }
        public float RangeMin { get; set; }
        public float RangeMax { get; set; }
        public float DangerLo { get; private set; }
        public float DangerHi { get; private set; }
        public int? TemplateLoId { get; private set; }
        public int? TemplateHiId { get; private set; }
        public SensorState State { get; set; }

        public float Value { get; set; }
        public string ValueStringFormat { get; set; }
        public string Unit { get; set; }

        public Sensor(string id, string deviceName, string unit, string valueStringFormat = "F2")
        {
            Id = id;
            DeviceName = deviceName;
        }

        public void UpdateValue(float value)
        {
            Value = value;
            valueBuffer[valueBufferIdx] = value;
            valueBufferIdx = (valueBufferIdx + 1) & (BUFFER_SIZE - 1);
        }

        public void SetDangerLo(float val, int templateId)
        {
            DangerLo = val;
            TemplateLoId = templateId;
        }

        public void SetDangerHi(float val, int templateId)
        {
            DangerHi = val;
            TemplateHiId = templateId;
        }
    }
}
