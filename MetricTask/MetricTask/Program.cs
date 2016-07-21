using MetricTask.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MetricTask
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());



            //List of objects to create a txt file
            Models.Task[] tasks = new Code().getTasks();
            Models.Metric[] metric = new Code().getMetrics();
            Models.Task_Metric[] tm = new Code().getTaskMetric();

            String[] task_table_task_name = new String[tm.Length];
            String[] task_table_metric_value = new String[tm.Length];

            for (int i = 0; i < tm.Length; i++)
            {
                task_table_task_name[i] = tm[i].Task_ID.ToString();
                task_table_metric_value[i] = tm[i].Metric_Name.ToString();
            }

        }
    }
}