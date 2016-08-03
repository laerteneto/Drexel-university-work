using System;
using System.Collections.Generic;

namespace MetricTask.Models
{
    public partial class Metric
    {
        public Metric()
        {
            this.Task_Metric = new List<Task_Metric>();
        }

        public string Metric_Name { get; set; }
        public string Metric_Description { get; set; }
        public virtual ICollection<Task_Metric> Task_Metric { get; set; }
    }
}
