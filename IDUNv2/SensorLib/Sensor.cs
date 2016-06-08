using IDUNv2.Common;
using System;
using System.Reflection;

namespace IDUNv2.SensorLib
{
    public enum SensorState
    {
        Offline,
        Online,
        Faulted
    }

    public class Sensor : NotifyBase
    {
        #region Value Ring Buffer
        /// <summary>
        /// Size of databuffer. Must be a power of two.
        /// </summary>
        public const int BUFFER_SIZE = 128;

        private float[] valueBuffer = new float[BUFFER_SIZE];
        private int valueBufferIdx;

        #endregion

        #region Notify Fields

        private string _name;
        private float _rangeMin;
        private float _rangeMax;
        private float _dangerLo;
        private float _dangerHi;
        private int? _templateLoId;
        private int? _templateHiId;
        private SensorState _state;
        private float _value;
        private string _valueStringFormat;
        private string _unit;
        private ActionCommand<object> _command;

        #endregion

        #region Notify Properties

        public string Name
        {
            get { return _name; }
            set { _name = value; Notify(); }
        }

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

        public int? TemplateLoId
        {
            get { return _templateLoId; }
            set { _templateLoId = value; Notify(); }
        }

        public int? TemplateHiId
        {
            get { return _templateHiId; }
            set { _templateHiId = value; Notify(); }
        }

        public SensorState State
        {
            get { return _state; }
            set { _state = value; Notify(); }
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

        #endregion

        #region Saved Properties

        private static readonly string[] savedPropNames =
        {
            "RangeMin",
            "RangeMax",
            "DangerHi",
            "DangerLo",
            "TemplateLoId",
            "TemplateHiId",
            "ValueStringFormat",
            "Unit"
        };

        #endregion

        private Func<SensorReadings, float?> readingExtracter;

        public Sensor(Func<SensorReadings, float?> readingExtracter, string name, float rangeMin, float rangeMax, string unit, string valueStringFormat = "F2")
        {
            this.readingExtracter = readingExtracter;
            Name = name;
            RangeMin = rangeMin;
            RangeMax = rangeMax;
            DangerLo = rangeMin;
            DangerHi = rangeMax;
            Unit = unit;
            ValueStringFormat = valueStringFormat;

            if (!HasSettingsValues())
                SaveToLocalSettings();
        }

        public void UpdateValue(DateTime timestamp, SensorReadings readings)
        {
            var val = readingExtracter(readings);
            if (val.HasValue)
            {
                Value = val.Value;
                valueBuffer[valueBufferIdx] = Value;
                valueBufferIdx = (valueBufferIdx + 1) & (BUFFER_SIZE - 1);

                if (Value > DangerHi || Value < DangerLo)
                    State = SensorState.Faulted;
            }
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

        private bool HasSettingsValues()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            return localSettings.Values[_name + "." + savedPropNames[0]] != null;
        }

        private void SavePropertyFromContainer(Windows.Storage.ApplicationDataContainer container, string propName)
        {
            var pi = GetType().GetProperty(propName);
            container.Values[_name + "." + propName] = pi.GetValue(this);
        }

        private void LoadPropertyFromContainer(Windows.Storage.ApplicationDataContainer container, string propName)
        {
            var pi = GetType().GetProperty(propName);
            var val = container.Values[_name + "." + propName];
            pi.SetValue(this, val);
        }

        public void SaveToLocalSettings()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            foreach (var name in savedPropNames)
            {
                SavePropertyFromContainer(localSettings, name);
            }
        }

        public void LoadFromLocalSettings()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            foreach (var name in savedPropNames)
            {
                LoadPropertyFromContainer(localSettings, name);
            }
        }
    }
}
