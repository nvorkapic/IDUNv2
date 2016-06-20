using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace IDUNv2.Common
{
    /// <summary>
    /// Helper baseclass to update viewmodel bindings
    /// </summary>
    public class NotifyBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void Notify([CallerMemberName] string caller = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
        }
    }
}
