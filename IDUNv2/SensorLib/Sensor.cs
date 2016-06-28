using IDUNv2.Common;
using IDUNv2.DataAccess;
using IDUNv2.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IDUNv2.SensorLib
{
    /// <summary>
    /// Implemented sensors, will be used as array indices so should be 0 based and contiguous
    /// </summary>
    public enum SensorId
    {
        Temperature,
        Humidity,
        Pressure
    }

    /// <summary>
    /// Indicates whether sensor is real and online, simulated or offline.
    /// </summary>
    public enum SensorDeviceState
    {
        Offline,
        Simulated,
        Online
    }

    /// <summary>
    /// Indicates if value has dipped into danger range or triggered a Trigger threshold
    /// </summary>
    public enum SensorFaultState
    {
        Normal,
        Faulted
    }

    public enum SensorFaultType
    {
        NoFault,
        FromDangerLo,
        FromDangerHi,
        FromTrigger
    }

    public class SensorFault
    {
        public SensorFaultType Type { get; set; }
        public int Id { get; set; }
    }

    public delegate void SensorFaultHandler(Sensor sensor, SensorFault fault, DateTime timestamp);

    /// <summary>
    /// Represents a Sensor on the SenseHat board (not necessarily 1:1 to a physical device)
    /// </summary>
    public class Sensor : NotifyBase
    {
        public struct Trigger
        {
            public int id;
            public int cmp;
            public float val;

            public override string ToString()
            {
                string s = $"TRIGGER ({id}) ON VAL ";
                s += cmp > 0 ? ">" : "<";
                s += $" {val}";
                return s;
            }

            public override bool Equals(object obj)
            {
                return id == ((Trigger)obj).id;
            }

            public override int GetHashCode()
            {
                return id.GetHashCode();
            }
        }

        #region Settings

        /// <summary>
        /// The fields which will be saved into LocalSettings
        /// </summary>
        private class SensorSettings
        {
            public float RangeMin;
            public float RangeMax;
            public float DangerLo;
            public float DangerHi;
            public string Unit;
            public List<Trigger> Triggers;

            public SensorSettings()
            {
                RangeMin = 0;
                RangeMax = 0;
                DangerLo = 0;
                DangerHi = 0;
                Unit = "";
                Triggers = new List<Trigger>();
            }

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
        public string ValueStringFormat { get; set; }
        public Action<Sensor, SensorFault, DateTime> Faulted { get; set; }
        public List<Trigger> Triggers { get; set; }

        #endregion

        #region Constructors

        public Sensor(SensorId id, string valueStringFormat = "F2")
        {
            settingsKey = "sensor." + (int)id;
            Id = id;
            Triggers = new List<Trigger>();

            ValueStringFormat = valueStringFormat;

            if (HasSettingsValues())
            {
                LoadFromLocalSettings();
            }
            else
            {
                SetDefaults();
            }
        }

        #endregion

        public override string ToString()
        {
            return $"Sensor Id: {Id}\nRange: {RangeMin} to {RangeMax}\nDanger: {DangerLo} and {DangerHi}\nUnit: {Unit}";
        }

        public string FaultString(SensorFault fault)
        {
            return this.ToString() +
                $"\nFaulted State: {FaultState}\nDevice State: {DeviceState}\nSensor Value: {Value}\nFault ID: {fault.Id}\nFault Type: {fault.Type}";
        }

        /// <summary>
        /// Update current sensor Value with the reading.
        /// </summary>
        /// <param name="timestamp">Time when reading was sampled</param>
        /// <param name="val">Value that was read</param>
        /// <param name="bias">Optional bias to add to value</param>
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

                var fault = new SensorFault();

                if (Value > DangerHi)
                {
                    fault.Type = SensorFaultType.FromDangerHi;
                }
                else if (Value < DangerLo)
                {
                    fault.Type = SensorFaultType.FromDangerLo;
                }
                if (Triggers != null)
                {
                    foreach (var t in Triggers)
                    {
                        if ((t.cmp > 0 && Value > t.val) || (t.cmp < 0 && Value < t.val))
                        {
                            fault.Type = SensorFaultType.FromTrigger;
                            fault.Id = t.id;
                        }
                    }
                }

                if (FaultState != SensorFaultState.Faulted && fault.Type != SensorFaultType.NoFault)
                {
                    FaultState = SensorFaultState.Faulted;
                    Faulted?.Invoke(this, fault, timestamp);
                }
            }
        }

        /// <summary>
        /// Determines whether this sensor has been saved to LocalSettings or not.
        /// </summary>
        /// <returns>True or false of settings exists</returns>
        private bool HasSettingsValues()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            return localSettings.Values[settingsKey] != null;
        }

        public void Clear()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values[settingsKey] = new SensorSettings().ToJson();
            RangeMin = 0;
            RangeMax = 0;
            DangerLo = 0;
            DangerHi = 0;
            Unit = "";
            DeviceState = SensorDeviceState.Offline;
            FaultState = SensorFaultState.Normal;
            Triggers?.Clear();
        }

        public void SetDefaults()
        {
            Clear();

            switch (Id)
            {
                case SensorId.Temperature:
                    RangeMin = -100;
                    RangeMax = 100;
                    DangerLo = -100;
                    DangerHi = 100;
                    Unit = "° C";
                    ValueStringFormat = "F2";
                    break;

                case SensorId.Humidity:
                    RangeMin = 0;
                    RangeMax = 100;
                    DangerLo = 0;
                    DangerHi = 100;
                    Unit = "% RH";
                    ValueStringFormat = "F2";
                    break;

                case SensorId.Pressure:
                    RangeMin = 500;
                    RangeMax = 2000;
                    DangerLo = 500;
                    DangerHi = 2000;
                    Unit = "hPa";
                    ValueStringFormat = "N0";
                    break;
            }

            SaveToLocalSettings();
        }

        public void SaveToLocalSettings()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            var ss = new SensorSettings
            {
                RangeMin = RangeMin,
                RangeMax = RangeMax,
                DangerLo = DangerLo,
                DangerHi = DangerHi,
                Unit = Unit,
                Triggers = Triggers
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
                RangeMin = ss.RangeMin;
                RangeMax = ss.RangeMax;
                DangerLo = ss.DangerLo;
                DangerHi = ss.DangerHi;
                Unit = ss.Unit;
                Triggers = ss.Triggers;
            }
        }

        public void AddTrigger(Trigger t)
        {
            if (!Triggers.Any(p => p.id == t.id))
            {
                Triggers.Add(t);
                SaveToLocalSettings();
            }
        }

        public void RemoveTrigger(Trigger t)
        {
            Triggers.Remove(t);
            SaveToLocalSettings();
        }

        public void SetTrigger(int id, float val, int cmp)
        {
            Trigger t;
            t.id = id;
            t.cmp = cmp;
            t.val = val;
            int i = Triggers.IndexOf(t);
            if (i != -1)
            {
                Triggers[i] = t;
                SaveToLocalSettings();
            }
        }
    }
}
