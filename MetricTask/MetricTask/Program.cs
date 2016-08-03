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

            //List of objects from the DataBase
            Models.Task[] tasks = new Code().getTasks();
            Models.Metric[] metric = new Code().getMetrics();
            Models.Task_Metric[] tm = new Code().getTaskMetric();

            //String arrays for the metrics
            String[] task_table_task_name = new String[tm.Length];
            String[] task_table_metric_name = new String[tm.Length];
            String[] all_metrics_names = new String[metric.Length];


            //From object to String for the Task names and Matric names from the table
            for (int i = 0; i < tm.Length; i++)
            {
                task_table_task_name[i] = tm[i].Task_ID.ToString();
                task_table_metric_name[i] = tm[i].Metric_Name.ToString();
            }
            
            //Getting all the metrics names as Strings
            for (int i = 0; i < metric.Length; i++)
            {
                all_metrics_names[i] = metric[i].Metric_Name.ToString();
            }

            //Int arrays for the tasks and metrics from the tables
            int[] tasks_table_task_name_index = new int[tm.Length];
            int[] tasks_table_metric_value_index = new int[tm.Length];


            //Filling out the Int arrays (task_table_task_name_index and task_table_metric_value_index)
            int task_num = filling_out_metric_tasks_arrays(tm.Length, metric.Length,
            task_table_task_name, task_table_metric_name, all_metrics_names, 
            tasks_table_task_name_index, tasks_table_metric_value_index);

            //Job names file (Task names)
            String[] task_names = new String[tasks.Length];
            for (int i = 0; i < tasks.Length; i++)
            {
                task_names[i] = tasks[i].Task_Name.ToString();
                //If you want the Task ID
                //task_names[i] = tasks[i].Task_ID.ToString();
            }
            write_array_to_file("job_names.txt", task_names, tasks.Length);   


            //The X file (jobsXfeatures) 
            int[,] X = new int[tasks.Length, metric.Length];
            //filling out the X matrix
            for (int i = 0; i < tm.Length; i++)
            {
                X[tasks_table_task_name_index[i]-1, tasks_table_metric_value_index[i]-1] = 1;
            }
            write_matrix_to_file("X.txt", X, tasks.Length, metric.Length);


            //The Y file (jobsXusers)
            int[] tasks_for_given_metric;
            int[] users_for_given_metric;
            int users_num = 100;
            int[,] Y = new int[task_num, users_num];
            for (int i = 0; i < metric.Length; i++)
            {
                tasks_for_given_metric = jobs_from_metric(i, tasks_table_task_name_index, tasks_table_metric_value_index, task_num);
                users_for_given_metric = generate_random_Users(i);
                filling_out_Y(Y, tasks_for_given_metric, users_for_given_metric, i);
            }
            write_matrix_to_file("Y.txt", Y, task_num, users_num);

            //The R file 
            int[,] R = new int[task_num, users_num];
            filling_out_R(R, Y, task_num, users_num);
            write_matrix_to_file("R.txt", R, task_num, users_num);

            //The User_table file (user name and self rating) 
            write_user_profile_file(users_num);
        }




        //Filling out the Int arrays (task_table_metric_value_index and task_table_task_name_index)
        public static int filling_out_metric_tasks_arrays(int T_len, int M_len,
            String[] task_table_task_name, String[] task_table_metric_name, String[] all_metric_names, 
            int[] task_table_task_name_index, int[] task_table_metric_value_index)
        {
            int task_num = 1;
            for (int i = 0; i < T_len; i++)
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
                for (int j = 0; j < M_len; j++)
                {
                    if (task_table_metric_name[i].ToUpper() == all_metric_names[j].ToUpper())
                    {
                        task_table_metric_value_index[i] = j + 1;
                        break;
                    }
                }
            }
            return task_num;
        }


        //function that return true if a vector have a value (num)
        public static bool appearedBefore(int[] vet, int num)
        {
            for (int i = 0; i < vet.Length; i++)
            {
                if (vet[i] == num)
                    return true;
            }
            return false;
        }


        //get the index of the jobs that have a specific metric (int metric)
        public static int[] jobs_from_metric(int metric, int[] task_table_task_name_index, int[] task_table_metric_value_index, int task_num)
        {
            int[] jobs_type = new int[task_num];
            int k = 0;
            for (int i = 0; i < task_table_task_name_index.Length; i++)
            {
                if (task_table_metric_value_index[i] - 1 == metric)
                {
                    jobs_type[k] = task_table_task_name_index[i];
                    k++;
                }
                
            }
            jobs_type = jobs_type.Distinct().ToArray(); //remove duplicates
            Array.Resize(ref jobs_type, k); //resize array
            return jobs_type;
        }
        

        //randomly generating the 10 users 
        public static int[] generate_random_Users(int n)
        {
            Random rnd = new Random(n);
            int rand;
            int[] users_with_ratings = new int[10];
            for (int i = 0; i < 10; i++)
            {
                rand = rnd.Next(1, 101); //between 1 and 100
                while (appearedBefore(users_with_ratings, rand))
                {
                    rand = rnd.Next(1, 101);
                }
                users_with_ratings[i] = rand;
            }
            return users_with_ratings;
        }


        //function that ramdomly generate the values for the matrix Y for tasks and users predeterminated 
        public static void filling_out_Y(int[,] Y, int[] tasks_for_given_metric, int[] users_for_given_metric, int n)
        {
            Random rnd = new Random(n);
            int good = rnd.Next(1,11);
            int ok = rnd.Next(1, 11 - good); 
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < tasks_for_given_metric.Length; j++)
                { 
                    if (i < good)
                        Y[tasks_for_given_metric[j]-1, users_for_given_metric[i]-1] = rnd.Next(8, 11);
                    else if (i < ok + good)
                        Y[tasks_for_given_metric[j]-1, users_for_given_metric[i]-1] = rnd.Next(5, 8);
                    else //bad
                        Y[tasks_for_given_metric[j]-1, users_for_given_metric[i]-1] = rnd.Next(0, 5);
                }
            }
        }

        //funcion that fill out the R file
        public static void filling_out_R(int[,] R, int[,] Y, int task_num, int users_num)
        {
            for (int i = 0; i < task_num; i++)
            {
                for (int j = 0; j < users_num; j++)
                {
                    if (Y[i, j] != 0)
                        R[i, j] = 1;
                }
            }
        }

        //function that write an array to a file
        public static void write_array_to_file(String name, String[] array, int len)
        {
            //Writing the job_names.txt File
            StreamWriter writeJobNames = new StreamWriter(name);
            for (int i = 0; i < len; i++)
            {
                writeJobNames.Write(array[i]);
                if (i != len - 1)
                {
                    writeJobNames.Write("\n");
                }
            }
            writeJobNames.Close();
        }


        //function that write a matrix to a file
        public static void write_matrix_to_file(String name, int[,] matrix, int l, int c)
        {
            StreamWriter write = new StreamWriter(name);
            for (int i = 0; i < l; i++)
            {
                for (int j = 0; j < c; j++)
                {
                    write.Write(matrix[i, j]);
                    if (j != c - 1)
                    {
                        write.Write("\t");
                    }
                }
                if (i != l - 1)
                {
                    write.Write("\n");
                }
            }
            write.Close();
        }

        //function that create n users names in a file
        public static void write_user_profile_file(int n)    
        {
            int k = 0; //used to loop through the alphabet
            int index = 1;
            string[] alphabet = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
            Random rnd = new Random();
            StreamWriter writeUserNames = new StreamWriter("user_table.txt");
            for (int i = 0; i < n; i++) //jobs
            {
                if (k == 26)
                {
                    k = 0;
                    index++;
                }

                writeUserNames.Write(alphabet[k] + index + "\t" + (rnd.Next(1, 6)));
                if (i != n - 1)
                {
                    writeUserNames.Write("\n");
                }
                k++;
            }
            writeUserNames.Close();
        }
    }
}