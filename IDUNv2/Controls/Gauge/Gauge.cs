﻿using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace IDUNv2.Controls
{
    /// <summary>
    /// A Modern UI Radial Gauge.
    /// </summary>
    [TemplatePart(Name = NeedlePartName, Type = typeof(Path))]
    [TemplatePart(Name = ScalePartName, Type = typeof(Path))]
    [TemplatePart(Name = TrailPartName, Type = typeof(Path))]
    [TemplatePart(Name = DangerLoPartName, Type = typeof(Path))]
    [TemplatePart(Name = DangerHiPartName, Type = typeof(Path))]
    [TemplatePart(Name = ValueTextPartName, Type = typeof(TextBlock))]
    public class Gauge : Control
    {
        #region Constants
        private const string NeedlePartName = "PART_Needle";
        private const string ScalePartName = "PART_Scale";
        private const string TrailPartName = "PART_Trail";
        private const string DangerLoPartName = "PART_DangerLo";
        private const string DangerHiPartName = "PART_DangerHi";
        private const string ValueTextPartName = "PART_ValueText";
        private const double Degrees2Radians = Math.PI / 180;
        #endregion Constants

        #region Dependency Property Registrations
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(
                "Minimum",
                typeof (double),
                typeof (Gauge),
                new PropertyMetadata(0.0));

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(
                "Maximum",
                typeof (double),
                typeof (Gauge),
                new PropertyMetadata(100.0));

        public static readonly DependencyProperty DangerLoProperty =
            DependencyProperty.Register(
                "DangerLo",
                typeof(double),
                typeof(Gauge),
                new PropertyMetadata(0.0));

        public static readonly DependencyProperty DangerHiProperty =
            DependencyProperty.Register(
                "DangerHi",
                typeof(double),
                typeof(Gauge),
                new PropertyMetadata(100.0));

        public static readonly DependencyProperty ScaleWidthProperty =
            DependencyProperty.Register(
                "ScaleWidth",
                typeof (Double),
                typeof (Gauge),
                new PropertyMetadata(26.0));

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value",
                typeof (double),
                typeof (Gauge),
                new PropertyMetadata(0.0));

        public static readonly DependencyProperty UnitProperty =
            DependencyProperty.Register(
                "Unit",
                typeof (string),
                typeof (Gauge),
                new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty NeedleBrushProperty =
            DependencyProperty.Register(
                "NeedleBrush",
                typeof (Brush),
                typeof (Gauge),
                new PropertyMetadata(new SolidColorBrush(Colors.OrangeRed)));

        public static readonly DependencyProperty ScaleBrushProperty =
            DependencyProperty.Register(
                "ScaleBrush",
                typeof (Brush),
                typeof (Gauge),
                new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        public static readonly DependencyProperty TrailBrushProperty =
            DependencyProperty.Register(
                "TrailBrush",
                typeof (Brush),
                typeof (Gauge),
                new PropertyMetadata(new SolidColorBrush(Colors.Orange)));

        public static readonly DependencyProperty DangerLoBrushProperty =
            DependencyProperty.Register(
                "DangerLoBrush",
                typeof(Brush),
                typeof(Gauge),
                new PropertyMetadata(new SolidColorBrush(Colors.Red)));

        public static readonly DependencyProperty DangerHiBrushProperty =
            DependencyProperty.Register(
                "DangerHiBrush",
                typeof(Brush),
                typeof(Gauge),
                new PropertyMetadata(new SolidColorBrush(Colors.Red)));

        public static readonly DependencyProperty ValueBrushProperty =
            DependencyProperty.Register(
                "ValueBrush",
                typeof (Brush),
                typeof (Gauge),
                new PropertyMetadata(new SolidColorBrush(Colors.DimGray)));

        public static readonly DependencyProperty UnitBrushProperty =
            DependencyProperty.Register(
                "UnitBrush",
                typeof (Brush),
                typeof (Gauge),
                new PropertyMetadata(new SolidColorBrush(Colors.DimGray)));

        public static readonly DependencyProperty ValueStringFormatProperty =
            DependencyProperty.Register(
                "ValueStringFormat",
                typeof (string),
                typeof (Gauge),
                new PropertyMetadata("N0"));

        protected static readonly DependencyProperty ValueAngleProperty =
            DependencyProperty.Register(
                "ValueAngle",
                typeof (double),
                typeof (Gauge),
                new PropertyMetadata(null));

        protected static readonly DependencyProperty ValuesProperty =
            DependencyProperty.Register(
                "Values",
                typeof(IEnumerable<Tuple<double, double>>),
                typeof(Gauge),
                new PropertyMetadata(null));

        public static readonly DependencyProperty LabelXProperty =
            DependencyProperty.Register(
                "LabelX",
                typeof(double),
                typeof(Gauge),
                new PropertyMetadata(0.0));

        public static readonly DependencyProperty LabelYProperty =
            DependencyProperty.Register(
                "LabelY",
                typeof(double),
                typeof(Gauge),
                new PropertyMetadata(-100.0));
        #endregion Dependency Property Registrations

        private long valueCallbackToken;

        #region Constructors
        public Gauge()
        {
            this.DefaultStyleKey = typeof(Gauge);
            this.Loaded += (s, e) => valueCallbackToken = RegisterPropertyChangedCallback(ValueProperty, OnValueChanged);
            this.Unloaded += (s, e) => UnregisterPropertyChangedCallback(ValueProperty, valueCallbackToken);
            InitLabels();
        }

        private void OnValueChanged(DependencyObject sender, DependencyProperty dp)
        {
            if (Double.IsNaN(Value))
                return;

            var middleOfScale = 77 - ScaleWidth / 2;
            var needle = GetTemplateChild(NeedlePartName) as Path;
            var valueText = GetTemplateChild(ValueTextPartName) as TextBlock;
            ValueAngle = ValueToAngle(Value);

            // Needle
            if (needle != null && !double.IsNaN(ValueAngle))
            {
                needle.RenderTransform = new RotateTransform() { Angle = ValueAngle };
            }

            // Trail
            var trail = GetTemplateChild(TrailPartName) as Path;

            if (trail != null)
            {
                if (ValueAngle > -146)
                {
                    trail.Visibility = Visibility.Visible;
                    var pg = new PathGeometry();
                    var pf = new PathFigure();
                    pf.IsClosed = false;
                    pf.StartPoint = ScalePoint(-150, middleOfScale);
                    var seg = new ArcSegment();
                    seg.SweepDirection = SweepDirection.Clockwise;
                    // We start from -150, so +30 becomes a large arc.
                    seg.IsLargeArc = ValueAngle > 30;
                    seg.Size = new Size(middleOfScale, middleOfScale);
                    seg.Point = ScalePoint(ValueAngle, middleOfScale);
                    pf.Segments.Add(seg);
                    pg.Figures.Add(pf);
                    trail.Data = pg;
                }
                else
                {
                    trail.Visibility = Visibility.Collapsed;
                }
            }

            // Value Text
            if (valueText != null)
            {
                valueText.Text = Value.ToString(ValueStringFormat);
            }
        }

        #endregion Constructors

        #region Properties
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public double DangerLo
        {
            get { return (double)GetValue(DangerLoProperty); }
            set { SetValue(DangerLoProperty, value); }
        }

        public double DangerHi
        {
            get { return (double)GetValue(DangerHiProperty); }
            set { SetValue(DangerHiProperty, value); }
        }

        public Double ScaleWidth
        {
            get { return (Double)GetValue(ScaleWidthProperty); }
            set { SetValue(ScaleWidthProperty, value); }
        }

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public string Unit
        {
            get { return (string)GetValue(UnitProperty); }
            set { SetValue(UnitProperty, value); }
        }

        public Brush NeedleBrush
        {
            get { return (Brush)GetValue(NeedleBrushProperty); }
            set { SetValue(NeedleBrushProperty, value); }
        }

        public Brush TrailBrush
        {
            get { return (Brush)GetValue(TrailBrushProperty); }
            set { SetValue(TrailBrushProperty, value); }
        }

        public Brush DangerLoBrush
        {
            get { return (Brush)GetValue(DangerLoBrushProperty); }
            set { SetValue(DangerLoBrushProperty, value); }
        }

        public Brush DangerHiBrush
        {
            get { return (Brush)GetValue(DangerHiBrushProperty); }
            set { SetValue(DangerHiBrushProperty, value); }
        }

        public Brush ScaleBrush
        {
            get { return (Brush)GetValue(ScaleBrushProperty); }
            set { SetValue(ScaleBrushProperty, value); }
        }

        public Brush ValueBrush
        {
            get { return (Brush)GetValue(ValueBrushProperty); }
            set { SetValue(ValueBrushProperty, value); }
        }

        public Brush UnitBrush
        {
            get { return (Brush)GetValue(UnitBrushProperty); }
            set { SetValue(UnitBrushProperty, value); }
        }

        public string ValueStringFormat
        {
            get { return (string)GetValue(ValueStringFormatProperty); }
            set { SetValue(ValueStringFormatProperty, value); }
        }

        protected double ValueAngle
        {
            get { return (double)GetValue(ValueAngleProperty); }
            set { SetValue(ValueAngleProperty, value); }
        }

        protected IEnumerable<Tuple<double, double>> Values
        {
            get { return (IEnumerable<Tuple<double, double>>)GetValue(ValuesProperty); }
            set { SetValue(ValuesProperty, value); }
        }

        public double LabelX
        {
            get { return (double)GetValue(LabelXProperty); }
            set { SetValue(LabelXProperty, value); }
        }

        public double LabelY
        {
            get { return (double)GetValue(LabelYProperty); }
            set { SetValue(LabelYProperty, value); }
        }

        #endregion Properties

        protected override void OnApplyTemplate()
        {
            // Draw Scale
            var scale = this.GetTemplateChild(ScalePartName) as Path;

            var middleOfScale = 77 - this.ScaleWidth / 2;

            if (scale != null)
            {
                var pg = new PathGeometry();
                var pf = new PathFigure();
                pf.IsClosed = false;
                pf.StartPoint = ScalePoint(-150, middleOfScale);
                var seg = new ArcSegment();
                seg.SweepDirection = SweepDirection.Clockwise;
                seg.IsLargeArc = true;
                seg.Size = new Size(middleOfScale, middleOfScale);
                seg.Point = ScalePoint(150, middleOfScale);
                pf.Segments.Add(seg);
                pg.Figures.Add(pf);
                scale.Data = pg;
            }

            var dangerLo = GetTemplateChild(DangerLoPartName) as Path;
            if (dangerLo != null)
            {
                dangerLo.Visibility = Visibility.Visible;
                var pg = new PathGeometry();
                var pf = new PathFigure();
                pf.IsClosed = false;
                var ang = ValueToAngle(DangerLo);
                pf.StartPoint = ScalePoint(ang, middleOfScale);
                var seg = new ArcSegment();
                seg.SweepDirection = SweepDirection.Counterclockwise;
                // We start from -150, so +30 becomes a large arc.
                seg.IsLargeArc = ang > 30.0;
                seg.Size = new Size(middleOfScale, middleOfScale);
                seg.Point = ScalePoint(-150, middleOfScale);
                pf.Segments.Add(seg);
                pg.Figures.Add(pf);
                dangerLo.Data = pg;
            }

            var dangerHi = GetTemplateChild(DangerHiPartName) as Path;
            if (dangerHi != null)
            {
                dangerHi.Visibility = Visibility.Visible;
                var pg = new PathGeometry();
                var pf = new PathFigure();
                pf.IsClosed = false;
                var ang = ValueToAngle(DangerHi);
                pf.StartPoint = ScalePoint(ang, middleOfScale);
                var seg = new ArcSegment();
                seg.SweepDirection = SweepDirection.Clockwise;
                // We start from -150, so +30 becomes a large arc.
                seg.IsLargeArc = ang < -30.0;
                seg.Size = new Size(middleOfScale, middleOfScale);
                seg.Point = ScalePoint(150, middleOfScale);
                pf.Segments.Add(seg);
                pg.Figures.Add(pf);
                dangerHi.Data = pg;
            }

            InitLabels();

            OnValueChanged(this, null);
            base.OnApplyTemplate();
        }

        private static Point ScalePoint(double angle, double middleOfScale)
        {
            return new Point(100 + Math.Sin(Degrees2Radians * angle) * middleOfScale, 100 - Math.Cos(Degrees2Radians * angle) * middleOfScale);
        }

        private double ValueToAngle(double value)
        {
            const double minAngle = -150;
            const double maxAngle = 150;

            // Off-scale to the left
            if (value < this.Minimum)
            {
                return minAngle - 7.5;
            }

            // Off-scale to the right
            if (value > this.Maximum)
            {
                return maxAngle + 7.5;
            }

            double angularRange = maxAngle - minAngle;

            return (value - this.Minimum) / (this.Maximum - this.Minimum) * angularRange + minAngle;
        }

        private void InitLabels()
        {
            var middleOfScale = 77 - ScaleWidth / 2;
            var tickSpacing = (this.Maximum - this.Minimum) / 10;
            if (tickSpacing == 0)
            {
                Values = new List<Tuple<double, double>>();
                return;
            }
            var values = new List<Tuple<double, double>>((int)tickSpacing);
            for (double v = this.Minimum; v <= this.Maximum; v += tickSpacing)
            {
                values.Add(new Tuple<double, double>(v, ValueToAngle(v)));
            }
            Values = values;
        }
    }
}