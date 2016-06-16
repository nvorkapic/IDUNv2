using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDUNv2.Common
{
    /// <summary>
    /// Helper class for 'async properties' viewmodel bindings
    /// </summary>
    /// <typeparam name="T">Type of the property</typeparam>
    public class TaskBinding<T> : INotifyPropertyChanged
    {
        public Task<T> Task { get; private set; }
        public T Result { get { return Task.Status == TaskStatus.RanToCompletion ? Task.Result : default(T); } }
        public TaskStatus Status { get { return Task.Status; } }
        public bool IsCompleted { get { return Task.IsCompleted; } }
        public bool IsNotCompleted { get { return !Task.IsCompleted; } }
        public bool RanToCompletion { get { return Task.Status == TaskStatus.RanToCompletion; } }
        public bool IsCanceled { get { return Task.IsCanceled; } }
        public bool IsFaulted { get { return Task.IsFaulted; } }
        public AggregateException Exception { get { return Task.Exception; } }
        public Exception InnerException { get { return Exception?.InnerException; } }
        public string ErrorMsg { get { return InnerException?.Message; } }
        public event PropertyChangedEventHandler PropertyChanged;

        public TaskBinding(Task<T> task)
        {
            Task = task;
            if (!task.IsCompleted)
            {
                var _ = WatchTask(task);
            }
        }

        private async Task WatchTask(Task task)
        {
            try
            {
                await task;
            }
            catch { }

            var pc = PropertyChanged;
            if (pc == null) return;

            pc(this, new PropertyChangedEventArgs("Status"));
            pc(this, new PropertyChangedEventArgs("IsCompleted"));
            pc(this, new PropertyChangedEventArgs("IsNotCompleted"));
            if (task.IsCanceled)
            {
                pc(this, new PropertyChangedEventArgs("IsCanceled"));
            }
            else if (task.IsFaulted)
            {
                pc(this, new PropertyChangedEventArgs("IsFaulted"));
                pc(this, new PropertyChangedEventArgs("Exception"));
                pc(this, new PropertyChangedEventArgs("InnerException"));
                pc(this, new PropertyChangedEventArgs("ErrorMsg"));
            }
            else
            {
                pc(this, new PropertyChangedEventArgs("RanToCompletion"));
                pc(this, new PropertyChangedEventArgs("Result"));
            }
        }
    }
}
