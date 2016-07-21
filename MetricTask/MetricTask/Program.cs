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
            String[] task_table_metric_name = new String[tm.Length];
            String[] all_metric_names = new String[metric.Length];

            for (int i = 0; i < tm.Length; i++)
            {
                task_table_task_name[i] = tm[i].Task_ID.ToString();
                task_table_metric_name[i] = tm[i].Metric_Name.ToString();
            }

            for (int i = 0; i < metric.Length; i++)
            {
                all_metric_names[i] = metric[i].Metric_Name.ToString();
            }

            int[] task_table_task_name_index = new int[tm.Length];
            int[] task_table_metric_value_index = new int[tm.Length];
            int task_num = 1;

            for (int i = 0; i < tm.Length; i++)
            {
                if (i == 0)
                {
                    task_table_task_name_index[i] = task_num;
                }
                else
                {
                    if (task_table_task_name[i] == task_table_task_name[i - 1])
                    {
                        task_table_task_name_index[i] = task_num;
                    }
                    else
                    {
                        task_num++;
                        task_table_task_name_index[i] = task_num;
                    }
                }

                for (int j = 0; j < metric.Length; j++)
                {
                    if (task_table_metric_name[i] == all_metric_names[j])
                    {
                        task_table_metric_value_index[i] = j + 1;
                        break;
                    }
                }
            }

            int[,] X = new int[task_num, metric.Length];

            for (int i = 0; i < task_num; i++)
            {
                for (int j = 0; j < metric.Length; j++)
                {
                    X[i, j] = 0;
                }
            }

            for (int i = 0; i < tm.Length; i++)
            {
                X[task_table_task_name_index[i], task_table_metric_value_index[i]] = 1;
            }

        }
    }
}