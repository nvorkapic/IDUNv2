using IDUNv2.Common;
using IDUNv2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDUNv2.ViewModels
{
    public class SensorTriggerViewModel : NotifyBase
    {
        #region Notify Fields

        private SensorTriggerComparer _comparer = SensorTriggerComparer.Below;
        private SensorTrigger _model;

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

        #endregion

        public SensorTriggerViewModel(SensorTrigger model)
        {
            Model = model;
            Comparer = model.Comparer;

        }

        public override string ToString()
        {
            if (Model.TemplateId == 0)
                return $"Existing Trigger Not Configured: Enter and Save Changes!";
            else
                return $"TriggerId: {Model.Id} ON value {Model.Comparer.ToString().ToUpper()} {Model.Value} with TemplateID {Model.TemplateId}";
        }
    }
}
