﻿using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDUNv2.Models
{
    public enum SensorType
    {
        Usage,
        Temperature,
        Pressure,
        Humidity,
        Accelerometer,
        Magnetometer,
        Gyroscope
    }

    public enum SensorTriggerComparer
    {
        Above,
        Below
    }

    public class SensorTrigger
    {
        
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public SensorType SensorId { get; set; }
        [NotNull]
        public SensorTriggerComparer Comparer { get; set; }
        [NotNull]
        public float Value { get; set; }

        [Indexed]
        public int TemplateId { get; set; }
    }
}