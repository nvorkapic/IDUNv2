using OnScreenKeyboard;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OnScreenKeyboard
{
    public sealed partial class NumericKeyboard : UserControl
    {
            private object _host;

            public Control TargetBox { get; private set; }

            public void SetTarget(Control control)
            {
                TargetBox = control;
            }

            public void RegisterTarget(TextBox control)
            {
                control.GotFocus += delegate { TargetBox = control; };
                control.LostFocus += delegate { TargetBox = null; };
            }
            public void RegisterTarget(PasswordBox control)
            {
                control.GotFocus += delegate { TargetBox = control; };
                control.LostFocus += delegate { TargetBox = null; };
            }

            public void RegisterHost(object host)
            {
                if (host != null)
                {
                    _host = host;
                }
            }

            public object GetHost()
            {
                return _host;
            }


            private void Target_GotFocus(object sender, RoutedEventArgs e)
            {

                var t = sender as TextBox;
                if (t.FocusState == FocusState.Pointer)
                {
                    //this.IsEnabled = true;
                    //turn on the lights
                }
            }


            public NumericKeyboard()
            {
                DataContext = new NumericKeyboardViewModel(this);
                InitializeComponent();
            }

        
    }
}
