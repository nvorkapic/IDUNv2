using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SensorLib
{
    public class SensorAlarm<TDevice, TValueFunc>
        where TDevice : Device
    {
        public static async TDevice Create<TValueSelector>(TValueSelector valueSelector)
        {
            var device = new SensorAlarm<TDevice, TValueSelector>();
            await device.Init();

        }
    }

    public static class Testing
    {
        public static void Foo()
        {
            var temp = new SensorAlarm<HTS221>(d => d.Temperature)
        }
    }
}
