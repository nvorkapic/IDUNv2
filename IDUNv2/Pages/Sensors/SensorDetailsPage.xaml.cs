﻿using IDUNv2.Common;
using IDUNv2.DataAccess;
using IDUNv2.Models;
using IDUNv2.SensorLib;
using IDUNv2.ViewModels;
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

namespace IDUNv2.Pages
{
    public class SensorDetailsViewModel : NotifyBase
    {
        private Sensor _sensor;

        public Sensor Sensor
        {
            get { return _sensor; }
            set { _sensor = value; Notify(); }
        }

        public ICollection<CmdBarItem> CmdBarItems { get; private set; }

        #region CmdBar Actions

        private void ShowSettings(object param)
        {

        }

        #endregion

        public SensorDetailsViewModel()
        {

        }
    }

    public sealed partial class SensorDetailsPage : Page
    {
        private DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(10) };
        private Random rnd = new Random();
        private SensorDetailsViewModel viewModel = new SensorDetailsViewModel();

        public SensorDetailsPage()
        {
            this.InitializeComponent();
            this.DataContext = viewModel;

            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            //var v = (rnd.NextDouble() * 20.0) - 10.0;
            float v = viewModel.Sensor.Value;
            SG.AddDataPoint(v);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var sensor = e.Parameter as Sensor;

            SG.SetRange(sensor.RangeMin, sensor.RangeMax);
            SG.SetDanger(sensor.DangerLo, sensor.DangerHi);

            viewModel.Sensor = sensor;
            
            DAL.PushNavLink(new NavLinkItem(viewModel.Sensor.Id.ToString(), GetType(), e.Parameter));

            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            CompositionTarget.Rendering -= CompositionTarget_Rendering;
        }

        private void CompositionTarget_Rendering(object sender, object e)
        {
            SG.Render();
        }
    }
}