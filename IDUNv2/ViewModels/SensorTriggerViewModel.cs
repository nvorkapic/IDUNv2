using IDUNv2.Common;
using IDUNv2.Models;
using IDUNv2.SensorLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDUNv2.ViewModels
{
    public class SensorTriggerViewModel : NotifyBase
    {
        #region Fields

        private Sensor sensor;

        #endregion

        #region Notify Fields

        private SensorTriggerComparer _comparer = SensorTriggerComparer.Below;
        private SensorTrigger _model;
        private bool _isEnabled;

        #endregion

        #region Notify Properties

        public SensorTriggerComparer Comparer
        {
            get { return _comparer; }
            set
            {
                _comparer = value;
                Model.Comparer = value;
                Notify();
                Notify("ComparerBelow");
                Notify("ComparerAbove");
            }
        }

        public bool ComparerBelow
        {
            get { return Comparer == SensorTriggerComparer.Below; }
            set { Comparer = SensorTriggerComparer.Below; }
        }

        public bool ComparerAbove
        {
            get { return Comparer == SensorTriggerComparer.Above; }
            set { Comparer = SensorTriggerComparer.Above; }
        }

        public SensorTrigger Model
        {
            get { return _model; }
            set
            {
                _model = value;
                Notify();
                Notify("Id");
                Notify("TemplateId");
                Notify("Value");
            }
        }

        public int Id
        {
            get { return Model.Id; }
            set { Model.Id = value; Notify(); }
        }

        public int TemplateId
        {
            get { return Model.TemplateId; }
            set { Model.TemplateId = value; Notify(); }
        }

        public float Value
        {
            get { return Model.Value; }
            set { Model.Value = value; Notify(); }
        }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { _isEnabled = value; Notify(); ToggleEnabled(value); }
        }

        #endregion

        private void ToggleEnabled(bool value)
        {
            Sensor.Trigger trigger;

            trigger.id = Id;
            trigger.cmp = Comparer == SensorTriggerComparer.Above ? 1 : -1;
            trigger.val = Value;

            if (value)
            {
                sensor.AddTrigger(trigger);
            }
            else
            {
                sensor.RemoveTrigger(trigger);
            }
        }

        #region Constructors

        public SensorTriggerViewModel(Sensor sensor, SensorTrigger model)
        {
            this.sensor = sensor;
            Model = model;
            Comparer = model.Comparer;
            IsEnabled = sensor.Triggers.Any(t => t.id == model.Id);
        }

        #endregion

        public override string ToString()
        {
            return $"Id: {Id}\nComparer: {Comparer}\nValue: {Value}\nTemplate Id: {TemplateId}";
        }
    }
}
