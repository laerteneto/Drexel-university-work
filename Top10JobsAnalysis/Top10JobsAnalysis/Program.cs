using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Top10JobsAnalysis
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            //analysis of the top 10 jobs for 5 users 
            //Reading input files
            String path = @"C:\Users\larissaf\Desktop\Top10JobsAnalysis\inputsFromTestBed\";
            int jobs_num = 300, metric_num = 67;
            
            //getting the jobs IDs, names, and description. And jobx Vs Features
            String[] jobs = System.IO.File.ReadAllLines(path + "jobs.txt");
            String[,] jobs_matrix = ConvertStringArrayIntoStringMatrix(jobs, jobs_num, 3);
            String[] X = System.IO.File.ReadAllLines(path + "X.txt");
            int[,] X_matrix = ConvertStringArrayIntoIntMatrix(X, jobs_num, metric_num);
            //filling out the job data
            JobData[] jobs_data = FillingOutJobData(jobs_matrix, jobs_num, X_matrix);


            //per job
            path = @"C:\Users\larissaf\Desktop\Top10JobsAnalysis\inputsFromTestBed\perJob\";
            for (int i = 1; i <= 10; i++)
            {
                String[] averages = System.IO.File.ReadAllLines(path + "averages" + i.ToString() + ".txt");
                UserPrediction[] users = FillingOutUserPrediction(averages);
                writeJobsSimilarityToFile("job_similarity_perjob" + i.ToString() + ".txt", users, jobs_data);
            }

            //per worker
            path = @"C:\Users\larissaf\Desktop\Top10JobsAnalysis\inputsFromTestBed\perWorker\";
            for (int i = 1; i <= 10; i++)
            {
                String[] averages = System.IO.File.ReadAllLines(path + "averages" + i.ToString() + ".txt");
                UserPrediction[] users = FillingOutUserPrediction(averages);
                writeJobsSimilarityToFile("job_similarity_perworker.txt" + i.ToString() + ".txt", users, jobs_data);
            }
        }

        //Function that convert a String array into an String matrix
        public static String[,] ConvertStringArrayIntoStringMatrix(String[] array, int lines, int columns)
        {
            String[,] matrix = new String[lines, columns];
            for (int i = 0; i < array.Length; i++)
            {
                string[] temp = array[i].Split('\t');
                int k = 0;
                for (int j = 0; j < columns; j++)
                {
                    matrix[i, k] = temp[j];
                    k++;
                }
            }
            return matrix;
        }

        //Function that convert a string array into an int matrix
        public static int[,] ConvertStringArrayIntoIntMatrix(String[] array, int lines, int columns)
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

        //Filling out the jobData matrix with the information of all the jobs
        public static JobData[] FillingOutJobData(String[,] jobs_matrix, int jobs_num, int[,] X_matrix)
        {
            
            JobData[] jobs = new JobData[jobs_num];
            for (int i = 0; i < jobs_num; i++)
            {
                //int[] Row = GetRow(X_matrix, 3);
                jobs[i] = new JobData(jobs_matrix[i, 1], jobs_matrix[i, 0], jobs_matrix[i, 2], GetRow(X_matrix, i));
            }
            return jobs;
        }

        //Filling out the UserPrediction data for the first five users.
        public static UserPrediction[] FillingOutUserPrediction(String[] averages) 
        {
            int number_of_users = 0;
            UserPrediction[] users = new UserPrediction[5];
            int j = 0;
            for(int i = 0; i < averages.Length; i++)
            { 
                if (averages[i].Contains("USER"))
                {
                    number_of_users++;
                    String user_name = averages[i].Substring(6);
                    i++;
                    if (number_of_users > 5) //already got 5 users
                    {
                        break;
                    }
                    String[] jobs = new String[10]; //getting the name of the top 10 jobs
                    int k = 0;
                    while (!averages[i].Contains("RATING"))
                    {
                        String[] temp = averages[i].Split('\t');
                        jobs[k] = temp[0].Substring(4);
                        k++;
                        i++;
                    }
                    Array.Resize(ref jobs, k);
                    users[j] = new UserPrediction(user_name, jobs);
                    j++;
                }
                
            }
            return users; 
        }

        //Manhattan Distance - used to calculate the similarity between two jobs
        public static double manhattanDistance(int[] array1, int[] array2) 
        {
            double sum = 0;
            for (int i = 0; i < array1.Length; i++)
            {
                sum += Math.Abs(array1[i] - array2[i]);
            }
            //number of ones (number of features/metrics) in each vector (job/task)
            int ones_array1 = array1.Sum();
            int ones_array2 = array2.Sum();
            //maximum number of features between the two vectors (jobs)
            int max_number_ones = Math.Max(ones_array1, ones_array2);

            //this is the similarity, not the distance. 
            //We divide the difference between the two vectors (jobs) by the maximum number of features between the two vectors (jobs)
            return 1 - (sum / max_number_ones);
        }

        //Get a row from a matrix
        public static T[] GetRow<T>(T[,] matrix, int row)
        {
            var columns = matrix.GetLength(1);
            var array = new T[columns];
            for (int i = 0; i < columns; ++i)
                array[i] = matrix[row, i];
            return array;
        }

        //find an index in a JobData array given a job ID
        public static int findIndexInAnArray(JobData[] jobs_data, String job_ID)
        {
            for (int i = 0; i < jobs_data.Length; i++)
            {
                if (jobs_data[i].getID() == job_ID)
                    return i;
            }
            return 0;
        }

        //Function that write Jobs similarity to a file
        public static void writeJobsSimilarityToFile(String name, UserPrediction[] users, JobData[] jobs_data)
        {
            StreamWriter write = new StreamWriter(name);
            for (int k = 0; k < users.Length; k++)
            {
                write.WriteLine(users[k].getName());
                String[] jobs = users[k].getJobsIDs();
                for (int i = 0; i < jobs.Length - 1; i++)
                {
                    int index1 = findIndexInAnArray(jobs_data, jobs[i].ToUpper());
                    for (int j = i + 1; j < jobs.Length; j++)
                    {
                        int index2 = findIndexInAnArray(jobs_data, jobs[j].ToUpper());
                        double distance = manhattanDistance(jobs_data[index1].getMetrics(), jobs_data[index2].getMetrics()) * (-100);
                        write.WriteLine(jobs_data[index1].getName() + " compared to " + jobs_data[index2].getName() + " is " + distance.ToString("#.##") + "% similar");
                    }
                }
                write.Write("\n");
            }
            write.Close();
        }
        
    }
}
