using System;
using System.Collections.Generic;

namespace MetricTask.Models
{
    public partial class Task
    {
        public Task()
        {
            this.Task_Metric = new List<Task_Metric>();
        }

        public System.Guid Task_ID { get; set; }
        public string Task_Name { get; set; }
        public string Description { get; set; }
        public Nullable<decimal> TCI { get; set; }
        public Nullable<decimal> EI { get; set; }
        public Nullable<decimal> EI_Tolerance { get; set; }
        public virtual ICollection<Task_Metric> Task_Metric { get; set; }
    }
}
