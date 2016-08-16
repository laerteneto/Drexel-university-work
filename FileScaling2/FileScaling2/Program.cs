using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace FileScaling2
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            //Reading input files
            String path = @"C:\Users\larissaf\Desktop\FileScaling2\inputFiles\";
            int jobs_num = 300, users_num = 1000, features_num = 67;
            //reading the file Y.txt
            String[] Y_array = System.IO.File.ReadAllLines(path + "Y.txt");
            int[,] Y = ConvertStringArrayToIntMatrix(Y_array, jobs_num, users_num);
            //reading the file R.txt
            String[] R_array = System.IO.File.ReadAllLines(path + "R.txt");
            int[,] R = ConvertStringArrayToIntMatrix(R_array, jobs_num, users_num);
            //reading the file X.txt
            String[] X_array = System.IO.File.ReadAllLines(path + "X.txt");
            int[,] X = ConvertStringArrayToIntMatrix(X_array, jobs_num, features_num);
            //reading the file user_table.txt
            String[] user_table = System.IO.File.ReadAllLines(path + "user_table.txt");
            //reading the file job_names.txt
            String[] job_names = System.IO.File.ReadAllLines(path + "job_names.txt");
            

            //Writing the output files
            //for each 100 workers
            writeMatrixToFile("outputWorkers/X.txt", X, jobs_num, features_num);
            writeArrayToFile("outputWorkers/job_names.txt", job_names, jobs_num);
            int k = 1;
            for (int i = 100; i <= users_num; i += 100)
            {
                int[,] new_Y = writeMatrixToFileReturningY("outputWorkers/Y" + k + ".txt", Y, jobs_num, i);
                writeMatrixToFile("outputWorkers/R" + k + ".txt", R, jobs_num, i);
                String[] new_user_table = writeArrayToFileReturngUserTable("outputWorkers/user_table" + k + ".txt", user_table, i);
                //users profile with ratings average of the tasks done by them 
                UserProfile[] users = writeUsersAverage(new_user_table, new_Y, job_names, jobs_num, "outputWorkers/averages" + k + ".txt");
                writeUsersRank(users, "outputWorkers/ranks" + k + ".txt");
                k++;
            }
            //for each 30 jobs
            writeArrayToFile("outputJobs/user_table.txt", user_table, users_num);
            k = 1;
            for (int i = 30; i <= jobs_num; i += 30)
            {
                writeMatrixToFile("outputJobs/X" + k + ".txt", X, i, features_num);
                String[] new_job_names = writeArrayToFileReturningJobNames("outputJobs/job_names" + k + ".txt", job_names, i);
                int[,] new_Y = writeMatrixToFileReturningY("outputJobs/Y" + k + ".txt", Y, i, users_num);
                writeMatrixToFile("outputJobs/R" + k + ".txt", R, i, users_num);
                //users profile with ratings average of the tasks done by them 
                UserProfile[] users = writeUsersAverage(user_table, new_Y, new_job_names, new_job_names.Length, "outputJobs/averages" + k + ".txt");
                writeUsersRank(users, "outputJobs/ranks" + k + ".txt");
                k++;
            }


        }



        //Function that convert a string array into an int matrix
        public static int[,] ConvertStringArrayToIntMatrix(String[] array, int lines, int columns)
        {
            int[,] matrix = new int[lines, columns];
            for (int i = 0; i < array.Length; i++)
            {
                string[] temp = array[i].Split('\t');
                int k = 0;
                for (int j = 0; j < columns; j++)
                {
                    matrix[i, k] = Convert.ToInt32(temp[j]);
                    k++;
                }
            }
            return matrix;
        }

        //Function that write an array to a file
        public static void writeArrayToFile(String name, String[] array, int len)
        {
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

        //Function that write an array to a file and return a new user_table
        public static String[] writeArrayToFileReturngUserTable(String name, String[] array, int len)
        {
            String[] new_user_table = new String[len];
            StreamWriter writeJobNames = new StreamWriter(name);
            for (int i = 0; i < len; i++)
            {
                new_user_table[i] = array[i];
                writeJobNames.Write(array[i]);
                if (i != len - 1)
                {
                    writeJobNames.Write("\n");
                }
            }
            writeJobNames.Close();
            return new_user_table;
        }

        //Function that write an array to a file
        public static String[] writeArrayToFileReturningJobNames(String name, String[] array, int len)
        {
            String[] new_job_names = new String[len];
            StreamWriter writeJobNames = new StreamWriter(name);
            for (int i = 0; i < len; i++)
            {
                new_job_names[i] = array[i];
                writeJobNames.Write(array[i]);
                if (i != len - 1)
                {
                    writeJobNames.Write("\n");
                }
            }
            writeJobNames.Close();
            return new_job_names;
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

        
        //Function that write a matrix to a file
        public static int[,] writeMatrixToFileReturningY(String name, int[,] matrix, int l, int c)
        {
            int[,] new_Y = new int[l,c];
            StreamWriter write = new StreamWriter(name);
            for (int i = 0; i < l; i++)
            {
                for (int j = 0; j < c; j++)
                {
                    new_Y[i, j] = matrix[i, j];
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
            return new_Y;
        }

        //get the users averages, write them to a file, and return a sorted users profile (name, rank and average) array
        public static UserProfile[] writeUsersAverage(String[] users_profile, int[,] Y, String[] tasks_names, int task_num, String path)
        {
            StreamWriter writeAvg = new StreamWriter(path);
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
                        writeAvg.WriteLine("Job " + tasks_names[j] + "\t" + Y[j, i]);
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
        public static void writeUsersRank(UserProfile[] users, String path)
        {
            StreamWriter writeRank = new StreamWriter(path);
            int good = 0, bad = 0, ok = 0;
            for (int i = 0; i < users.Length; i++)
            {
                writeRank.Write("User\t" + users[i].getName() + "\t" + users[i].getRank() + "\t" + users[i].getAvg_ratings().ToString("n2"));
                if (users[i].getAvg_ratings() < 6)
                {
                    writeRank.Write("\tBad\n");
                    bad++;
                }
                else if (users[i].getAvg_ratings() < 8)
                {
                    writeRank.Write("\tOk\n");
                    ok++;
                }
                else
                {
                    writeRank.Write("\tGood\n");
                    good++;
                }
            }
            writeRank.WriteLine("\nSummary Report");
            writeRank.WriteLine("Good " + good);
            writeRank.WriteLine("Ok " + ok);
            writeRank.Write("Bad " + bad);
            writeRank.Close();
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
    }
}
