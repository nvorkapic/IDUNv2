using IDUNv2.Common;
using Newtonsoft.Json;
using System;
using System.Reflection;

namespace IDUNv2.SensorLib
{
    public enum SensorId
    {
        Temperature,
        Humidity,
        Pressure
    }

    public enum SensorDeviceState
    {
        Offline,
        Simulated,
        Online
    }

    public enum SensorFaultState
    {
        Normal,
        Faulted
    }

    public class Sensor : NotifyBase
    {
        #region Settings

        private class SensorSettings
        {
            public SensorDeviceState DeviceState;
            public float RangeMin;
            public float RangeMax;
            public float DangerLo;
            public float DangerHi;
            public string ValueStringFormat;
            public string Unit;

            public string ToJson()
            {
                return JsonConvert.SerializeObject(this);
            }

            public static SensorSettings CreateFromJson(string json)
            {
                return JsonConvert.DeserializeObject<SensorSettings>(json);
            }
        }

        #endregion

        #region Notify Fields

        private float _rangeMin;
        private float _rangeMax;
        private float _dangerLo;
        private float _dangerHi;
        private SensorDeviceState _deviceState;
        private SensorFaultState _faultState;
        private float _value;
        private string _valueStringFormat;
        private string _unit;
        private ActionCommand<object> _command;
        private bool _hasHardware;

        #endregion

        #region Notify Properties

        public float RangeMin
        {
            get { return _rangeMin; }
            set { _rangeMin = value; Notify(); }
        }

        public float RangeMax
        {
            get { return _rangeMax; }
            set { _rangeMax = value; Notify(); }
        }

        public float DangerLo
        {
            get { return _dangerLo; }
            set { _dangerLo = value; Notify(); }
        }

        public float DangerHi
        {
            get { return _dangerHi; }
            set { _dangerHi = value; Notify(); }
        }

        public SensorDeviceState DeviceState
        {
            get { return _deviceState; }
            set { _deviceState = value; Notify(); }
        }

        public SensorFaultState FaultState
        {
            get { return _faultState; }
            set { _faultState = value; Notify(); }
        }

        public float Value
        {
            get { return _value; }
            set { _value = value; Notify(); }
        }

        public string ValueStringFormat
        {
            get { return _valueStringFormat; }
            set { _valueStringFormat = value; Notify(); }
        }

        public string Unit
        {
            get { return _unit; }
            set { _unit = value; Notify(); }
        }

        public ActionCommand<object> Command
        {
            get { return _command; }
            set { _command = value; Notify(); }
        }

        public bool HasHardware
        {
            get { return _hasHardware; }
            set { _hasHardware = value; Notify(); }
        }

        #endregion

        #region Fields

        private readonly string settingsKey;

        #endregion

        #region Properties

        public SensorId Id { get; private set; }
        public Func<float> GetSimValue { get; set; }

        #endregion

        public Sensor(SensorId id, float rangeMin, float rangeMax, string unit, string valueStringFormat = "F2")
        {
            settingsKey = "sensor." + (int)id;
            Id = id;
            RangeMin = rangeMin;
            RangeMax = rangeMax;
            DangerLo = rangeMin;
            DangerHi = rangeMax;
            Unit = unit;
            ValueStringFormat = valueStringFormat;

            if (!HasSettingsValues())
                SaveToLocalSettings();
        }

        public void UpdateValue(DateTime timestamp, float? val, float? bias)
        {
            if (DeviceState == SensorDeviceState.Simulated && GetSimValue != null)
            {
                val = GetSimValue();
            }
            if (DeviceState != SensorDeviceState.Offline && val.HasValue)
            {
                Value = val.Value;
                if (bias.HasValue)
                    Value += bias.Value;

                if ((Value > DangerHi || Value < DangerLo) && (FaultState != SensorFaultState.Faulted))
                {
                    FaultState = SensorFaultState.Faulted;
                }
            }
        }

        private bool HasSettingsValues()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            return localSettings.Values[settingsKey] != null;
        }

        public void SaveToLocalSettings()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            var ss = new SensorSettings
            {
                DeviceState = DeviceState,
                RangeMin = RangeMin,
                RangeMax = RangeMax,
                DangerLo = DangerLo,
                DangerHi = DangerHi,
                ValueStringFormat = ValueStringFormat,
                Unit = Unit
            };

            localSettings.Values[settingsKey] = ss.ToJson();
        }

        public void LoadFromLocalSettings()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            var json = localSettings.Values[settingsKey] as string;
            var ss = SensorSettings.CreateFromJson(json);
            if (ss != null)
            {
                DeviceState = ss.DeviceState;
                RangeMin = ss.RangeMin;
                RangeMax = ss.RangeMax;
                DangerLo = ss.DangerLo;
                DangerHi = ss.DangerHi;
                ValueStringFormat = ss.ValueStringFormat;
                Unit = ss.Unit;
            }
        }
    }
}
