using MetricTask.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

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
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            
            //Application.Run(new Form1());

            //List of objects from the DataBase
            //Models.Task[] tasks = new Code().getTasks();
            //Models.Metric[] metric = new Code().getMetrics();
            //Models.Task_Metric[] tm = new Code().getTaskMetric();

            //String arrays for the metrics
            //String[] task_table_task_name = new String[tm.Length];
            //String[] task_table_metric_name = new String[tm.Length];
            //String[] all_metric_names = new String[metric.Length];

            //randomly generating the 30 users with ratings for a type of task
            Random rnd = new Random();
            int rand;
            int[] users_with_ratings = new int[30];
            for (int i = 0; i < 30; i++)
            {
                rand = rnd.Next(1, 100); //between 1 and 100
                while (appearedBefore(users_with_ratings, rand))
                {
                    rand = rnd.Next(1, 100);
                }
                users_with_ratings[i] = rand;
            }

            for (int i = 0; i < 30; i++)
            {
                Console.Write(users_with_ratings[i] + " ");
            }

        }

        public static bool appearedBefore(int[] vet, int num)
        {
            for (int i = 0; i < vet.Length; i++)
            {
                if (vet[i] == num)
                    return true;
            }
            return false;
        }
    }
}