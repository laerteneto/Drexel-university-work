using MetricTask.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

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

            //List of objects from the DataBase
            Models.Task[] tasks = new Code().getTasks();
            Models.Metric[] metric = new Code().getMetrics();
            Models.Task_Metric[] tm = new Code().getTaskMetric();

            //String arrays for the metrics
            String[] task_table_task_name = new String[tm.Length];
            String[] task_table_metric_name = new String[tm.Length];
            String[] all_metric_names = new String[metric.Length];

            //From object to String for the Task names and Matric names from the table
            for (int i = 0; i < tm.Length; i++)
            {
                task_table_task_name[i] = tm[i].Task_ID.ToString();
                task_table_metric_name[i] = tm[i].Metric_Name.ToString();
            }

            //Getting all the metrics names
            for (int i = 0; i < metric.Length; i++)
            {
                all_metric_names[i] = metric[i].Metric_Name.ToString();
            }

            //Int arrays for the tasks and metrics from the table
            int[] task_table_task_name_index = new int[tm.Length];
            int[] task_table_metric_value_index = new int[tm.Length];
            int task_num = 1;
            

            //Filling out the Int arrays
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


            //Job names (Task names)
            String[] task_names = new String[task_num];

            for (int i = 0; i < tasks.Length; i++)
            {
                task_names[i] = tasks[i].Task_Name.ToString();
                //If you want the Task ID
                //task_names[i] = tasks[i].Task_ID.ToString();
            }

            //Writing the job_names.txt File
            StreamWriter writeJobNames = new StreamWriter("job_names.txt");
            for (int i = 0; i < task_num; i++)
            {
                writeJobNames.Write(task_names[i]);
                if (i != task_num - 1)
                {
                    writeJobNames.Write("\n");
                }
            }
            writeJobNames.Close();


            //The jobsXfeatures matrix
            int[,] X = new int[task_num, metric.Length];

            //initialzing the X matrix
            for (int i = 0; i < task_num; i++)
            {
                for (int j = 0; j < metric.Length; j++)
                {
                    X[i, j] = 0;
                }
            }

            //filling out the X matrix
            for (int i = 0; i < tm.Length; i++)
            {
                X[task_table_task_name_index[i]-1, task_table_metric_value_index[i]-1] = 1;
            }

            //Writing the X.txt File
            StreamWriter writeX = new StreamWriter("X.txt");
            for (int i = 0; i < task_num; i++)
            {
                for (int j = 0; j < metric.Length; j++)
                {
                    writeX.Write(X[i,j]);
                    if (j != metric.Length - 1)
                    {
                        writeX.Write("\t");
                    }
                }
                if (i != task_num - 1)
                {
                    writeX.Write("\n");
                }
            }
            writeX.Close();


        }
    }
}