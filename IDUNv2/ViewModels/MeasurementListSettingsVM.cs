using IDUNv2.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDUNv2.ViewModels
{

    public class MeasurementListSettingsItems : BaseViewModel
    {
        public string Title { get; set; }
        public string Icon { get; set; }
        
        public bool Enabled { get; set; }

        public int Interval { get; set; }

        public List<Thersholds> Threshold { get; set; }

    }

    public class Thersholds
    {
        public bool Threshold { get; set; }
        // <, <=, =, >=, >
        public string Operator { get; set; }

        public double Value { get; set; }

        public string ReportTemplate { get; set; }

    }

    public class MeasurementListSettingsVM : BaseViewModel
    {

        public ObservableCollection<MeasurementListSettingsItems> _measurementConfigurationList = new ObservableCollection<MeasurementListSettingsItems>()
        {
            new MeasurementListSettingsItems
            {
                Title="Usage",
                Icon="/Assets/Finger.png",
                Enabled=false,
                Interval=1000,
                Threshold = new List<Thersholds>()
                {
                   new Thersholds {Threshold=false }
                }
                
            },
            new MeasurementListSettingsItems
            {
                Title="Temperature",
                Icon="/Assets/Thermometer.png",
                Enabled=false,
                Interval=1000,
                Threshold = new List<Thersholds>()
                {
                   new Thersholds {Threshold=false }
                }
            },
            new MeasurementListSettingsItems
            {
                Title="Pressure",
                Icon="/Assets/Pressure.png",
                Enabled=false,
                Interval=1000,
                Threshold = new List<Thersholds>()
                {
                   new Thersholds {Threshold=false }
                }
            },
            new MeasurementListSettingsItems
            {
                Title="Humidity",
                Icon="/Assets/Humidity.png",
                Enabled=false,
                Interval=1000,
                Threshold = new List<Thersholds>()
                {
                   new Thersholds {Threshold=false }
                }
            },
            new MeasurementListSettingsItems
            {
                Title="Accelerometer",
                Icon="/Assets/Accelerometer.png",
                Enabled=false,
                Interval=1000,
                Threshold = new List<Thersholds>()
                {
                   new Thersholds {Threshold=false }
                }
            },
            new MeasurementListSettingsItems
            {
                Title="Magnetometer",
                Icon="/Assets/Magnet.png",
                Enabled=false,
                Interval=1000,
                Threshold = new List<Thersholds>()
                {
                   new Thersholds {Threshold=false }
                }
            },
            new MeasurementListSettingsItems
            {
                Title="Gyroscope",
                Icon="/Assets/Gyroscope.png",
                Enabled=false,
                Interval=1000,
                Threshold = new List<Thersholds>()
                {
                   new Thersholds {Threshold=false }
                }
            }         
        };
        public ObservableCollection<MeasurementListSettingsItems> MeasurementConfigurationList { get { return _measurementConfigurationList;}}
    }
}
