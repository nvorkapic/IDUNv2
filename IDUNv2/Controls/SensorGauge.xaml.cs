using IDUNv2.Common;
using IDUNv2.SensorLib;
using Windows.UI.Xaml;

namespace IDUNv2.Controls
{
    public sealed partial class SensorGauge : Windows.UI.Xaml.Controls.UserControl
    {
        #region Dependency Properties

        public static readonly DependencyProperty ButtonCommandProperty =
            DependencyProperty.Register(
                "ButtonCommand",
                typeof(ActionCommand<Sensor>),
                typeof(Gauge),
                new PropertyMetadata(null));

        public static readonly DependencyProperty BUttonCommandParameterProperty =
            DependencyProperty.Register(
                "ButtonCommandParameter",
                typeof(Sensor),
                typeof(Gauge),
                new PropertyMetadata(null));

        #endregion

        public ActionCommand<Sensor> ButtonCommand
        {
            get { return (ActionCommand<Sensor>)GetValue(ButtonCommandProperty); }
            set { SetValue(ButtonCommandProperty, value); }
        }

        public Sensor ButtonCommandParameter
        {
            get { return (Sensor)GetValue(BUttonCommandParameterProperty); }
            set { SetValue(BUttonCommandParameterProperty, value); }
        }

        public SensorGauge()
        {
            this.InitializeComponent();
        }
    }
}
