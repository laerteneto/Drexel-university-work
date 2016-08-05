using MetricTask.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetricTask
{
    public class Code
    {
        public MetricTask.Models.Task[] getTasks()
        {
            TestBedContext db = new TestBedContext();
            IQueryable<MetricTask.Models.Task> tasks = from t in db.Tasks
                                                      select t;

            return tasks.ToArray();
        }

        public MetricTask.Models.Metric[] getMetrics()
        {
            //Code You need
            TestBedContext db = new TestBedContext();
            IQueryable<MetricTask.Models.Metric> metrics = from m in db.Metrics
                                                       select m;

            return metrics.ToArray();
        }

        public MetricTask.Models.Task_Metric[] getTaskMetric()
        {
            TestBedContext db = new TestBedContext();
            IQueryable<MetricTask.Models.Task_Metric> task_metric = from tm in db.Task_Metric
                                                                    select tm;

            return task_metric.ToArray();
        }

        //This is to calculate the weights from each task, but we are not using this for a while
        /*public List<Task_Metric> weightsForTask(String task_id)
        {
            MetricTask.Models.Task[] tasks = this.getTasks();
            List<MetricTask.Models.Task_Metric> task_metrics = new List<Task_Metric>();

            foreach (MetricTask.Models.Task task in tasks)
            {
                foreach (Task_Metric metric in task.Task_Metric)
                {
                    task_metrics.Add(metric);
                }
            }
            return task_metrics;
        }*/
    }
}