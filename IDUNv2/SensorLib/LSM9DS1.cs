using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IDUNv2.SensorLib
{
    public enum AccelFullScaleRange
    {
        Range2g = 0,
        Range16g = 1,
        Range4g = 2,
        Range8g = 3,
    }

    public enum AccelLowPassFilter
    {
        Freq408Hz = 0,
        Freq211Hz = 1,
        Freq105Hz = 2,
        Freq50Hz = 3
    }

    public enum AccelSampleRate
    {
        Freq10Hz = 1,
        Freq50Hz = 2,
        Freq119Hz = 3,
        Freq238Hz = 4,
        Freq476Hz = 5,
        Freq952Hz = 6
    }

    public enum CompassSampleRate
    {
        Freq0_625Hz = 0,
        Freq1_25Hz = 1,
        Freq2_5Hz = 2,
        Freq5Hz = 3,
        Freq10Hz = 4,
        Freq20Hz = 5,
        Freq40Hz = 6,
        Freq80Hz = 7
    }

    public enum GyroBandwidthCode
    {
        BandwidthCode0 = 0,
        BandwidthCode1 = 1,
        BandwidthCode2 = 2,
        BandwidthCode3 = 3
    }

    public enum GyroFullScaleRange
    {
        Range245 = 0,
        Range500 = 1,
        Range2000 = 2
    }

    public enum GyroHighPassFilterCode
    {
        FilterCode0 = 0,
        FilterCode1 = 1,
        FilterCode2 = 2,
        FilterCode3 = 3,
        FilterCode4 = 4,
        FilterCode5 = 5,
        FilterCode6 = 6,
        FilterCode7 = 7,
        FilterCode8 = 8,
        FilterCode9 = 9,
    }

    public enum GyroSampleRate
    {
        Freq14_9Hz = 0,
        Freq59_5Hz = 1,
        Freq119Hz = 2,
        Freq238Hz = 3,
        Freq476Hz = 4,
        Freq952Hz = 5
    }

    public enum MagneticFullScaleRange
    {
        Range4Gauss = 0,
        Range8Gauss = 1,
        Range12Gauss = 2,
        Range16Gauss = 3
    }

    //GyroSampleRate = GyroSampleRate.Freq119Hz;
	//GyroBandwidthCode = GyroBandwidthCode.BandwidthCode1;
	//GyroHighPassFilterCode = GyroHighPassFilterCode.FilterCode4;
	//GyroFullScaleRange = GyroFullScaleRange.Range500;

	//AccelSampleRate = AccelSampleRate.Freq119Hz;
	//AccelFullScaleRange = AccelFullScaleRange.Range8g;
	//AccelLowPassFilter = AccelLowPassFilter.Freq50Hz;

	//CompassSampleRate = CompassSampleRate.Freq20Hz;
	//MagneticFullScaleRange = MagneticFullScaleRange.Range4Gauss;

    public class LSM9DS1 : Device
    {
        #region Addresses

        private const byte ADDRESS0 = 0x6a;
        private const byte ADDRESS1 = 0x6b;
        private const byte ID = 0x68;

        private const byte MAG_ADDRESS0 = 0x1c;
        private const byte MAG_ADDRESS1 = 0x1d;
        private const byte MAG_ADDRESS2 = 0x1e;
        private const byte MAG_ADDRESS3 = 0x1f;
        private const byte MAG_ID = 0x3d;

        #endregion

        #region Register Mapping

        private const byte ACT_THS = 0x04;
        private const byte ACT_DUR = 0x05;
        private const byte INT_GEN_CFG_XL = 0x06;
        private const byte INT_GEN_THS_X_XL = 0x07;
        private const byte INT_GEN_THS_Y_XL = 0x08;
        private const byte INT_GEN_THS_Z_XL = 0x09;
        private const byte INT_GEN_DUR_XL = 0x0A;
        private const byte REFERENCE_G = 0x0B;
        private const byte INT1_CTRL = 0x0C;
        private const byte INT2_CTRL = 0x0D;
        private const byte WHO_AM_I = 0x0F;
        private const byte CTRL1 = 0x10;
        private const byte CTRL2 = 0x11;
        private const byte CTRL3 = 0x12;
        private const byte ORIENT_CFG_G = 0x13;
        private const byte INT_GEN_SRC_G = 0x14;
        private const byte OUT_TEMP_L = 0x15;
        private const byte OUT_TEMP_H = 0x16;
        private const byte STATUS = 0x17;
        private const byte OUT_X_L_G = 0x18;
        private const byte OUT_X_H_G = 0x19;
        private const byte OUT_Y_L_G = 0x1A;
        private const byte OUT_Y_H_G = 0x1B;
        private const byte OUT_Z_L_G = 0x1C;
        private const byte OUT_Z_H_G = 0x1D;
        private const byte CTRL4 = 0x1E;
        private const byte CTRL5 = 0x1F;
        private const byte CTRL6 = 0x20;
        private const byte CTRL7 = 0x21;
        private const byte CTRL8 = 0x22;
        private const byte CTRL9 = 0x23;
        private const byte CTRL10 = 0x24;
        private const byte INT_GEN_SRC_XL = 0x26;
        private const byte STATUS2 = 0x27;
        private const byte OUT_X_L_XL = 0x28;
        private const byte OUT_X_H_XL = 0x29;
        private const byte OUT_Y_L_XL = 0x2A;
        private const byte OUT_Y_H_XL = 0x2B;
        private const byte OUT_Z_L_XL = 0x2C;
        private const byte OUT_Z_H_XL = 0x2D;
        private const byte FIFO_CTRL = 0x2E;
        private const byte FIFO_SRC = 0x2F;
        private const byte INT_GEN_CFG_G = 0x30;
        private const byte INT_GEN_THS_XH_G = 0x31;
        private const byte INT_GEN_THS_XL_G = 0x32;
        private const byte INT_GEN_THS_YH_G = 0x33;
        private const byte INT_GEN_THS_YL_G = 0x34;
        private const byte INT_GEN_THS_ZH_G = 0x35;
        private const byte INT_GEN_THS_ZL_G = 0x36;
        private const byte INT_GEN_DUR_G = 0x37;

        private const byte MAG_OFFSET_X_L = 0x05;
        private const byte MAG_OFFSET_X_H = 0x06;
        private const byte MAG_OFFSET_Y_L = 0x07;
        private const byte MAG_OFFSET_Y_H = 0x08;
        private const byte MAG_OFFSET_Z_L = 0x09;
        private const byte MAG_OFFSET_Z_H = 0x0A;
        private const byte MAG_WHO_AM_I = 0x0F;
        private const byte MAG_CTRL1 = 0x20;
        private const byte MAG_CTRL2 = 0x21;
        private const byte MAG_CTRL3 = 0x22;
        private const byte MAG_CTRL4 = 0x23;
        private const byte MAG_CTRL5 = 0x24;
        private const byte MAG_STATUS = 0x27;
        private const byte MAG_OUT_X_L = 0x28;
        private const byte MAG_OUT_X_H = 0x29;
        private const byte MAG_OUT_Y_L = 0x2A;
        private const byte MAG_OUT_Y_H = 0x2B;
        private const byte MAG_OUT_Z_L = 0x2C;
        private const byte MAG_OUT_Z_H = 0x2D;
        private const byte MAG_INT_CFG = 0x30;
        private const byte MAG_INT_SRC = 0x31;
        private const byte MAG_INT_THS_L = 0x32;
        private const byte MAG_INT_THS_H = 0x33;

        #endregion

        public override Task Init()
        {
            byte ctrl1;
            
            // Gyroscope

            // sample rate (119Hz):
            ctrl1 = 0x60;
            // bandwidth code1:
            ctrl1 |= 0x01;
            // full scale range (500 degrees per sec)
            ctrl1 |= 0x08;
            // gyroScale = 0.0175f;
        }

        public override void GetReadings(ref SensorReadings readings)
        {
            throw new NotImplementedException();
        }
    }
}
