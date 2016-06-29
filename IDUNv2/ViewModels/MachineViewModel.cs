using IDUNv2.Common;
using IDUNv2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace IDUNv2.ViewModels
{
    public class MachineViewModel : NotifyBase
    {
        #region Notify Fields

        private Machine _model;
        private bool _dirty;

        #endregion

        #region Notify Properties

        public Machine Model
        {
            get { return _model; }
            set { _model = value; Notify(); Notify("Text"); }
        }

        public bool Dirty
        {
            get { return _dirty; }
            set { _dirty = value; Notify(); }
        }

        public string Text
        {
            get { return $"{_model.Id}: {MchCode}"; }
        }

        public string MchCode
        {
            get { return _model.MchCode; }
            set { _model.MchCode = value; SetDirty(); }
        }

        public string MchCodeContract
        {
            get { return _model.MchCodeContract; }
            set { _model.MchCodeContract = value; SetDirty(); }
        }

        public string OrgCode
        {
            get { return _model.OrgCode; }
            set { _model.OrgCode = value; SetDirty(); }
        }

        #endregion

        private void SetDirty([CallerMemberName] string caller = "")
        {
            Dirty = true;
            Notify(caller);
        }

        public override string ToString()
        {
            return MchCode;
        }

        #region Properties

        public bool IsValidated
        {
            get
            {
                return MchCode?.Length > 0 && MchCodeContract?.Length > 0 && MchCodeContract?.Length > 0;
            }
        }

        #endregion

        #region Constructors

        public MachineViewModel(Machine model)
        {
            Model = model;
        }

        #endregion
    }
}
