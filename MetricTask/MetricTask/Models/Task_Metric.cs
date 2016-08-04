using System;
using System.Collections.Generic;

namespace MetricTask.Models
{
    public partial class Task_Metric
    {
        public System.Guid Task_ID { get; set; }
        public string Metric_Name { get; set; }
        public decimal Metric_Value { get; set; }
        public virtual Metric Metric { get; set; }
        public virtual Task Task { get; set; }
    }
}
