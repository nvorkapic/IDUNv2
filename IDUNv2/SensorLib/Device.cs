using System;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;

namespace IDUNv2.SensorLib
{
    /// <summary>
    /// Represents a physical sensor device on the SenseHat board
    /// </summary>
    public abstract class Device : IDisposable
    {
        protected I2cDevice device;

        public bool IsValid { get; set; }

        public virtual void Dispose()
        {
            if (device != null)
            {
                device.Dispose();
                device = null;
            }
        }

        public abstract Task Init();
        public abstract void GetReadings(ref SensorReadings readings);

        /// <summary>
        /// Try and connect to a specific device address
        /// </summary>
        /// <param name="addr">Address of device</param>
        /// <param name="speed">Bus speed</param>
        /// <param name="sharing">Sharing mode</param>
        /// <returns></returns>
        protected async Task GetDevice(int addr, I2cBusSpeed speed = I2cBusSpeed.StandardMode, I2cSharingMode sharing = I2cSharingMode.Exclusive)
        {
            var aqs = I2cDevice.GetDeviceSelector();
            var infos = await DeviceInformation.FindAllAsync(aqs);
            var settings = new I2cConnectionSettings(addr)
            {
                BusSpeed = speed,
                SharingMode = sharing
            };
            device = await I2cDevice.FromIdAsync(infos[0].Id, settings);
        }

        /// <summary>
        /// Read bytes from device
        /// </summary>
        /// <param name="addr">Address to read from</param>
        /// <param name="count">Number of bytes to read</param>
        /// <returns></returns>
        protected byte[] ReadBytes(byte addr, int count)
        {
            try
            {
                byte[] wb = { addr };
                byte[] rb = new byte[count];
                device.WriteRead(wb, rb);
                return rb;
            }
            catch (Exception e)
            {
                var msg = String.Format("failed to read {0} bytes from address: {1:X}", count, addr);
                throw new Exception(msg, e);
            }
        }

        /// <summary>
        /// Read a single byte.
        /// </summary>
        /// <param name="addr">Address to read from</param>
        /// <returns></returns>
        protected byte Read8( byte addr)
        {
            var bytes = ReadBytes(addr, 1);
            return bytes[0];
        }

        /// <summary>
        /// Read a single 16 bit integer in Little Endian format
        /// </summary>
        /// <param name="addr">Address to read from</param>
        /// <returns></returns>
        protected UInt16 Read16LE(byte addr)
        {
            var bytes = ReadBytes(addr, 2);
            return (UInt16)(((UInt16)bytes[1] << 8) | (UInt16)bytes[0]);
        }

        /// <summary>
        /// Read a single 24 bit integer in little endian format
        /// </summary>
        /// <param name="addr">Address to read from</param>
        /// <returns></returns>
        protected UInt32 Read24LE(byte addr)
        {
            var bytes = ReadBytes(addr, 3);
            return (UInt32)(((UInt32)bytes[2] << 16) | ((UInt32)bytes[1] << 8) | (UInt32)bytes[0]);
        }

        /// <summary>
        /// Write a single byte.
        /// </summary>
        /// <param name="addr">Address to write at.</param>
        /// <param name="val">Byte value which is written</param>
        protected void WriteByte(byte addr, byte val)
        {
            try
            {
                var buf = new byte[2] { addr, val };
                device.Write(buf);
            }
            catch (Exception e)
            {
                var msg = String.Format("failed to write {0} to address: {1:X}", val, addr);
                throw new Exception(msg, e);
            }
        }
    }
}
