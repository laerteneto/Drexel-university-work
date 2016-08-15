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
            int jobs_num = 300, users_num = 1000, features_num = 67;

            //Reading Files
            string path = @"C:\Users\larissaf\Desktop\FileScaling2\inputFiles\";

            //reading the file Y.txt
            string[] Y_array = System.IO.File.ReadAllLines(path + "Y.txt");
            int[,] Y = ConvertStringArrayToIntMatrix(Y_array, jobs_num, users_num);

            //reading the file R.txt
            string[] R_array = System.IO.File.ReadAllLines(path + "R.txt");
            int[,] R = ConvertStringArrayToIntMatrix(R_array, jobs_num, users_num);

            //reading the file X.txt
            string[] X_array = System.IO.File.ReadAllLines(path + "X.txt");
            int[,] X = ConvertStringArrayToIntMatrix(X_array, jobs_num, features_num);

            //reading the file user_table.txt
            string[] user_table = System.IO.File.ReadAllLines(path + "user_table.txt");

            //reading the file job_names.txt
            string[] job_names = System.IO.File.ReadAllLines(path + "job_names.txt");

            //reading the file ranks.txt
            //string[] ranks = System.IO.File.ReadAllLines(path + "ranks.txt");
            //for (int i = 0; i < ranks.Length; i++)
            //{
            //    ranks[i] = ranks[i].Replace("\t", " ");
            //}

            //reading the file averages.txt
            //string[] averages = System.IO.File.ReadAllLines(path + "averages.txt");
            //for (int i = 0; i < averages.Length; i++)
            //{
            //    averages[i] = averages[i].Replace("\t", " ");
            //}
            
            //for each 100 workers
            writeMatrixToFile("outputWorkers/X.txt", X, jobs_num, features_num);
            writeArrayToFile("outputWorkers/job_names.txt", job_names, jobs_num);
            int k = 1;
            for (int i = 100; i <= users_num; i += 100)
            {
                writeMatrixToFile("outputWorkers/Y" + k + ".txt", Y, jobs_num, i);
                writeMatrixToFile("outputWorkers/R" + k + ".txt", R, jobs_num, i);
                writeArrayToFile("outputWorkers/user_table" + k + ".txt", user_table, i);
                k++;
            }

            //for each 30 jobs
            writeArrayToFile("outputJobs/user_table.txt", user_table, users_num);
            k = 1;
            for (int i = 30; i <= jobs_num; i += 30)
            {
                writeMatrixToFile("outputJobs/X" + k + ".txt", X, i, features_num);
                writeArrayToFile("outputJobs/job_names" + k + ".txt", job_names, i);
                writeMatrixToFile("outputJobs/Y" + k + ".txt", Y, i, users_num);
                writeMatrixToFile("outputJobs/R" + k + ".txt", R, i, users_num);
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
    }
}
