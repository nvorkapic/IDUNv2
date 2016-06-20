using System;
using System.Threading.Tasks;
using Windows.Devices.I2c;

namespace IDUNv2.SensorLib
{
    /// <summary>
    /// Pressure and Temperature device
    /// </summary>
    public class LPS25H : Device
    {
        private const byte C_Addr = 0x5C;

        #region Regiser Mapping

        private const byte C_ResConf = 0x10;
        private const byte C_Ctrl1 = 0x20;
        private const byte C_Ctrl2 = 0x21;
        private const byte C_Status = 0x27;
        private const byte C_PressOutXL = 0x28;
        private const byte C_FifoCtrl = 0x2E;

        #endregion

        /// <summary>
        /// Conversion factor from raw data to hecto Pascal
        /// </summary>
        private const float pressureFactor = 1.0f / 4096.0f;

        public float? Pressure { get; set; }

        /// <summary>
        /// Initialize and send initial configuration settings to device
        /// </summary>
        public override async Task Init()
        {
            await GetDevice(C_Addr, I2cBusSpeed.FastMode);

            if (device != null)
            {
                IsValid = true;
            }

            // PD ODR2-0 DIFF BDU RESET SIM
            //  1    100    0   1     0   0
            // Active mode, 25Hz, default, non-continous, disable, default
            WriteByte(C_Ctrl1, 0xC4);

            // AVGP1-0 AVGT1-0
            //      01      01
            // N. internal average: 32, 16 (pressure, temperature)
            WriteByte(C_ResConf, 0x05);

            // F_MODE2 F_MODE1 F_MODE0
            //       1       1       0
            // FIFO_MEAN_MODE: running average filtered pressure
            WriteByte(C_FifoCtrl, 0xC0);

            // BOOT FIFO_EN WTM_EN FIFO_MEAN I2C_EN SWRESET AUTOZERO ONESHOT
            //    0    1         0         0      0       0        0       0
            // normal, enable, disable, disable, i2c enable, normal, normal, waiting
            WriteByte(C_Ctrl2, 0x40);
        }

        /// <summary>
        /// Try and get new readings.
        /// </summary>
        /// <param name="readings">Optionally filled out readings.Pressure</param>
        public override void GetReadings(ref SensorReadings readings)
        {
            var status = Read8(C_Status);
            if ((status & 2) == 2)
            {
                var raw = (Int32)Read24LE(C_PressOutXL + 0x80);
                readings.Pressure = raw * pressureFactor;
            }
        }
    }
}
