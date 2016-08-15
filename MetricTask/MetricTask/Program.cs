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
            Models.Task[] tasks = new Code().getTasks(); //all the tasks
            Models.Metric[] metrics = new Code().getMetrics(); //all the metrics
            Models.Task_Metric[] tm = new Code().getTaskMetric(); //relationship between tasks and metrics

            //String arrays for all metrics names, and metrics and tasks from tm array
            String[] tm_tasks_names = new String[tm.Length];
            String[] tm_metrics_names = new String[tm.Length];
            String[] metrics_names = new String[metrics.Length];


            //Filling out the string arrays
            for (int i = 0; i < tm.Length; i++)
            {
                tm_tasks_names[i] = tm[i].Task_ID.ToString();
                tm_metrics_names[i] = tm[i].Metric_Name.ToString();
            }
            
            //filling out the metrics names array
            for (int i = 0; i < metrics.Length; i++)
            {
                metrics_names[i] = metrics[i].Metric_Name.ToString();
            }

            //Int arrays for the metrics and tasks from tm array
            int[] tm_task_name_index = new int[tm.Length];
            int[] tm_metric_value_index = new int[tm.Length];


            //Filling out the Int arrays (tm_task_name_index and tm_metric_value_index)
            fillingOutMetricTaskArrays(tm.Length, metrics.Length,
            tm_tasks_names, tm_metrics_names, metrics_names, 
            tm_task_name_index, tm_metric_value_index);


            //Job_names file (or 'Task_names')
            int task_num = tasks.Length; //task_num is the amount os tasks
            String[] tasks_names = new String[tasks.Length];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks_names[i] = tasks[i].Task_Name.ToString();
                //If you want the Task ID
                //task_names[i] = tasks[i].Task_ID.ToString();
            }
            writeArrayToFile("job_names.txt", tasks_names, task_num);   


            //The X file ('jobs Vs features' or 'tasks Vs metrics') 
            int metrics_num = metrics.Length; //the amount of metrics
            int[,] X = new int[task_num, metrics_num];
            //filling out the X matrix
            for (int i = 0; i < tm.Length; i++) //for each relationship between a task and a metric
            {
                X[tm_task_name_index[i]-1, tm_metric_value_index[i]-1] = 1; //uses the indexes of the relationship to fill out the X matrix
            }
            writeMatrixToFile("X.txt", X, tasks.Length, metrics.Length);


            //The Y file ('jobs Vs users' or 'taks Vs users')
            int users_num = 50;
            int[] users_for_each_task; //the amount of users for each task
            int[,] Y = new int[task_num, users_num];
            for (int i = 0; i < task_num; i++) //for each task, a ramdom number os users have ratings for it
            {
                users_for_each_task = generateRandomUsers(i, users_num); //between 10 and (users_num/4)
                fillingOutY(Y, users_for_each_task, i);
            }
            writeMatrixToFile("Y.txt", Y, task_num, users_num);

            //The R file ('jobs Vs users' or 'taks Vs users')
            int[,] R = new int[task_num, users_num];
            fillingOutR(R, Y, task_num, users_num);
            writeMatrixToFile("R.txt", R, task_num, users_num);

            //The User_table file (user name and self rating) //the self rating does not matter, but it's needed for the recommender system
            String[] users_profile = writeUserProfileFile(users_num);

            //users profile with ratings average of the tasks done by them 
            UserProfile[] users = writeUsersAverage(users_profile, Y, tasks_names, task_num);
            writeUsersRank(users);
        }


        //Filling out the Int arrays (task_table_metric_value_index and task_table_task_name_index)
        public static void fillingOutMetricTaskArrays(int tasks_num, int metric_num,
            String[] tm_tasks_names, String[] tm_metric_names, String[] all_metric_names,
            int[] tm_tasks_names_index, int[] tm_metrics_values_index)
        {
            int task_num = 1;
            for (int i = 0; i < tasks_num; i++)
            {
                if (i == 0)
                {
                    tm_tasks_names_index[i] = task_num;
                }
                else
                {
                    if (tm_tasks_names[i] == tm_tasks_names[i - 1])
                    {
                        tm_tasks_names_index[i] = task_num;
                    }
                    else
                    {
                        task_num++;
                        tm_tasks_names_index[i] = task_num;
                    }
                }
                for (int j = 0; j < metric_num; j++)
                {
                    if (tm_metric_names[i].ToUpper() == all_metric_names[j].ToUpper())
                    {
                        tm_metrics_values_index[i] = j + 1;
                        break;
                    }
                }
            }
        }


        //Function that return true if an array has a value (num)
        //Used when generating an array with ramdom users that did a job. This function is used to garantee that there are no repetitive users.
        public static bool appearedBefore(int[] vet, int num)
        {
            for (int i = 0; i < vet.Length; i++)
            {
                if (vet[i] == num)
                    return true;
            }
            return false;
        }


        //Randomly generating the n users (between 10 and (total_user_num/4)) 
        public static int[] generateRandomUsers(int n, int total_user_num)
        {
            Random rnd = new Random((int)DateTime.Now.Ticks);
            int num_users = rnd.Next(10, (total_user_num/4)+1); //this is how many users are going to have ratings for a job
            int rand;
            int[] users_with_ratings = new int[num_users];
            for (int i = 0; i < num_users; i++)
            {
                rand = new Random().Next(1, total_user_num+1); //between 1 and 100
                while (appearedBefore(users_with_ratings, rand))
                {
                    rand = new Random().Next(1, total_user_num + 1);
                }
                users_with_ratings[i] = rand;
            }
            Array.Resize(ref users_with_ratings, num_users); //resize array
            return users_with_ratings;
        }


        //Function that ramdomly generate the values for the matrix Y for a n task with users predeterminated 
        public static void fillingOutY(int[,] Y, int[] users_for_each_task, int n)
        {
            Random rnd = new Random();
            int num_users = users_for_each_task.Length;
            int good = rnd.Next(1,num_users+1);
            int ok = rnd.Next(1, num_users + 1 - good); 

            for (int i = 0; i < num_users; i++)
            {
                if (i < good)
                    Y[n, users_for_each_task[i]-1] = rnd.Next(8, 11);
                else if (i < ok + good)
                    Y[n, users_for_each_task[i]-1] = rnd.Next(5, 8);
                else //bad
                    Y[n, users_for_each_task[i]-1] = rnd.Next(0, 5);
             }
        }

        //Funcion that fill out the R file
        public static void fillingOutR(int[,] R, int[,] Y, int task_num, int users_num)
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

        //Function that write an array to a file
        public static void writeArrayToFile(String name, String[] array, int len)
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


        //Function that write a matrix to a file
        public static void writeMatrixToFile(String name, int[,] matrix, int l, int c)
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

        //Function that create n users names in a file
        public static String[] writeUserProfileFile(int n)    
        {
            String[] users_profile = new String[n];
            int k = 0; //used to loop through the alphabet
            int index = 1;
            string[] alphabet = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
            StreamWriter writeUserNames = new StreamWriter("user_table.txt");
            for (int i = 0; i < n; i++) //jobs
            {
                if (k == 26)
                {
                    k = 0;
                    index++;
                }
                writeUserNames.Write(alphabet[k] + index);
                users_profile[i] = alphabet[k] + index;
                if (i != n - 1)
                {
                    writeUserNames.Write("\n");
                }
                k++;
            }
            writeUserNames.Close();
            return users_profile;
        }

        //get the rank of a user using his name
        public static int getRank(UserProfile[] users, String name)
        {
            for (int i = 0; i < users.Length; i++)
            {
                if (name == users[i].getName())
                {
                    return i;
                }
            }
            return 0;
        }

        //get the users averages, write them to a file, and return a sorted users profile (name, rank and average) array
        public static UserProfile[] writeUsersAverage(String[] users_profile, int[,] Y, String[] tasks_names, int task_num)
        {
            StreamWriter writeAvg = new StreamWriter("averages.txt");
            UserProfile[] users = new UserProfile[users_profile.Length];
            int amount;
            int sum_rating;
            String name;
            int rank;

            //filling out the UserProfile array
            for (int i = 0; i < users_profile.Length; i++)
            {
                amount = 0;
                sum_rating = 0;
                for (int j = 0; j < task_num; j++)
                {
                    if (Y[j, i] != 0) //means that the user i did the task
                    {
                        sum_rating += Y[j, i];
                        amount++;
                    }
                }
                users[i] = new UserProfile(users_profile[i], ((double)sum_rating / amount));
            }

            UserProfile[] users_sorted = users.OrderBy(c => c.getAvg_ratings()).ToArray();
            Array.Reverse(users_sorted);

            int jobs_per_user;

            //writing the average file
            for (int i = 0; i < users_profile.Length; i++)
            {
                writeAvg.WriteLine("User: " + users_profile[i]);
                jobs_per_user = 0;
                for (int j = 0; j < task_num; j++)
                {
                    if (Y[j, i] != 0) //means that the user i did the task
                    {
                        writeAvg.WriteLine("Job " + tasks_names[j] + " " + Y[j, i]);
                        jobs_per_user++;
                    }
                }
                writeAvg.WriteLine("AVG rating " + users[i].getAvg_ratings().ToString("n2"));
                writeAvg.WriteLine("Amount of jobs " + jobs_per_user);
                //getting the rank
                name = users[i].getName();
                rank = getRank(users_sorted, name);
                users[i].setRank(rank + 1);
                writeAvg.WriteLine("Rank " + users[i].getRank());
                writeAvg.WriteLine();
            }
            writeAvg.Close();
            return users_sorted;
        }

        //write the users ranks to a file
        public static void writeUsersRank(UserProfile[] users)
        {
            StreamWriter writeRank = new StreamWriter("ranks.txt");
            int good = 0, bad = 0, ok = 0;
            for (int i = 0; i < users.Length; i++)
            {
                writeRank.Write("User " + users[i].getName() + " " + users[i].getRank() + " " + users[i].getAvg_ratings().ToString("n2"));
                if (users[i].getAvg_ratings() < 6)
                {
                    writeRank.Write(" Bad\n");
                    bad++;
                }
                else if (users[i].getAvg_ratings() < 8)
                {
                    writeRank.Write(" Ok\n");
                    ok++;
                }
                else
                {
                    writeRank.Write(" Good\n");
                    good++;
                }
            }
            writeRank.WriteLine("\nSummary Report");
            writeRank.WriteLine("Good "+ good);
            writeRank.WriteLine("Ok " + ok);
            writeRank.Write("Bad " + bad);
            writeRank.Close();
        }
        
    }
}