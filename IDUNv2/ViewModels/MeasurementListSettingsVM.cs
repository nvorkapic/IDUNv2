using IDUNv2.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDUNv2.ViewModels
{
    public class MeasurementSetting : BaseViewModel
    {
        public bool Enabled { get; set; }

        public int Interval { get; set; }

        public ObservableCollection<Thresholds> Threshold { get; set; }
    }


    public class MeasurementListSettingsItems
    {
        public string Title { get; set; }
        public string Icon { get; set; }
        public string Unit { get; set; }
        public MeasurementSetting Setting { get; set; }
        public List<Operator> ListAvailableOperators { get; set; }

    }

    public class Thresholds
    {
        public bool Threshold { get; set; }
        // <, <=, =, >=, >
        public Operator Operator { get; set; }

        public double Value { get; set; }

        public string ReportTemplate { get; set; }

    }

    public enum Operator
    {
        Less,
        LessOrEqual,
        Greater,
        GreaterOrEqual,
        Equal
 
    }
    public class MeasurementListSettingsVM : BaseViewModel
    {
        public List<MeasurementSetting> _measurementSettingList = new List<MeasurementSetting>();

        public static ObservableCollection<MeasurementListSettingsItems> _measurementConfigurationList = new ObservableCollection<MeasurementListSettingsItems>()
        {
            new MeasurementListSettingsItems { Title="Usage", Icon="/Assets/Finger.png",
                ListAvailableOperators = new List<Operator> {Operator.Equal} },
            new MeasurementListSettingsItems { Title="Temperature", Icon="/Assets/Thermometer.png", Unit="°C",
                ListAvailableOperators = new List<Operator> {Operator.Equal, Operator.Greater, Operator.GreaterOrEqual, Operator.Less, Operator.LessOrEqual} },
            new MeasurementListSettingsItems { Title="Pressure", Icon="/Assets/Pressure.png", Unit="kPa",
                ListAvailableOperators = new List<Operator> {Operator.Equal, Operator.Greater, Operator.GreaterOrEqual, Operator.Less, Operator.LessOrEqual}  },
            new MeasurementListSettingsItems { Title="Humidity", Icon="/Assets/Humidity.png",Unit="%",
                ListAvailableOperators = new List<Operator> {Operator.Equal, Operator.Greater, Operator.GreaterOrEqual, Operator.Less, Operator.LessOrEqual}  },
            new MeasurementListSettingsItems { Title="Accelerometer", Icon="/Assets/Accelerometer.png",Unit="m/s²",
                ListAvailableOperators = new List<Operator> {Operator.Equal, Operator.Greater, Operator.GreaterOrEqual, Operator.Less, Operator.LessOrEqual} },
            new MeasurementListSettingsItems { Title="Magnetometer", Icon="/Assets/Magnet.png",Unit="μT",
                ListAvailableOperators = new List<Operator> {Operator.Equal, Operator.Greater, Operator.GreaterOrEqual, Operator.Less, Operator.LessOrEqual} },
            new MeasurementListSettingsItems { Title="Gyroscope", Icon="/Assets/Gyroscope.png",Unit="rad/s",
                ListAvailableOperators = new List<Operator> {Operator.Equal, Operator.Greater, Operator.GreaterOrEqual, Operator.Less, Operator.LessOrEqual}  }
        };

        public ObservableCollection<MeasurementListSettingsItems> MeasurementConfigurationList { get { return _measurementConfigurationList;}}

        public List<MeasurementSetting> MeasurementSettingList { get { return _measurementSettingList; } }

        private MeasurementListSettingsItems _currentMeasurements;
        public MeasurementListSettingsItems CurrentMeasurements { get { return _currentMeasurements; } set { _currentMeasurements = value; Notify(); } }
    }
}
