using recommenderSystems.Domain;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace recommenderSystems.Business
{
    public class DriverFileManager : BusinessManager
    {
        public bool MainRoutine()
        {
            try
            {
                Stopwatch watch = Stopwatch.StartNew();

                //Object to Hold Task Parameters
                TaskDimensions task = new TaskDimensions();

                //User number to start proccessing
                int user_number = 1;

                //Load File System Service
                FileSystemManager fileMgr = new FileSystemManager();


                //Method call to get the number of jobs and users from the file Y
                fileMgr.detectSizeOfJobsColumns(task, Directory.GetCurrentDirectory() + DirectoryPaths.Y);

                //Method call to get the number of features from the file X, and allocating the X matrix
                double[,] X = fileMgr.getNumberOfFeaturesX(Directory.GetCurrentDirectory() + DirectoryPaths.X, task);
                fileMgr.readFeaturesX(X, Directory.GetCurrentDirectory() + DirectoryPaths.X, task);

                //Method call to get the jobs names
                String[] job_list = fileMgr.readJobNames(Directory.GetCurrentDirectory() + DirectoryPaths.EXPRESSIONS, task);

                //method that return the users profile
                UserProfile[] users_profile = fileMgr.readUserProfile(Directory.GetCurrentDirectory() + DirectoryPaths.USER_TABLE, task);

                //Creating a variable to write in a File the job recommendations and comparisons
                //Load File Writer
                StreamWriter writeTextResult = fileMgr.getResultStreamWriter();
                StreamWriter writeTextAverages = fileMgr.getAverageStreamWriter();
                StreamWriter writeText = fileMgr.getIdandAvgStreamWriter();
                //StreamWriter writeTextDiff = fileMgr.getDifficultyStreamWriter();



                double[] users_calculated_raitings = new double[task.num_users_init];

                double total_rating_avg_system = 0;
                double total_similarity_avg_system = 0;
                //double total_inaccuracy_system = 0;

                MLApp.MLApp matlab = new MLApp.MLApp();
                MatlabManager matlabMgr = new MatlabManager();

                while (user_number <= task.num_users_init)
                {
                    // job rating file for a user   
                    double[,] my_ratings = new double[task.num_jobs_init, 1];

                    //Now we read R and Y from theirs files (-1 because I will remove the chosen user from the matrixes)
                    double[,] Y = fileMgr.readTrainingY(Directory.GetCurrentDirectory() + DirectoryPaths.Y, task, my_ratings, user_number);
                    double[,] R = fileMgr.readTrainingR(Directory.GetCurrentDirectory() + DirectoryPaths.R, task, user_number);

                    //Creating a MatLab reference to execute the recommended job script
                  
                    object[] res = matlabMgr.executeFilter(task, job_list, Directory.GetCurrentDirectory() + DirectoryPaths.MATLAB, my_ratings, Y, R, X, matlab);


                    //Each time creates a  to be used to write the recommended jobs in a file
                    List<TopJobData> mylist = fileMgr.writeValuesToFile(writeTextResult, res, job_list, user_number, X);


                    //Calculate Averages for Jobs for a User
                    DataResult avgs = new DataResult(mylist, mylist.Count, users_profile[user_number - 1]);
                    avgs.AverageForEachJob();
                    fileMgr.writeAveragesToFile(avgs, writeTextAverages, users_profile[user_number - 1]);

                    total_rating_avg_system += avgs.Rating_total_avg;
                    total_similarity_avg_system += avgs.Percentage_total_avg;
                    //total_inaccuracy_system += avgs.Self_inaccuracy;
                    //adding the list at the Dictionary for each user

                    //ID and AVGs file
                    writeText.WriteLine(users_profile[user_number - 1].UserID + "\t" + avgs.Rating_total_avg);


                    users_calculated_raitings[user_number - 1] = avgs.Rating_total_avg;

                    //writing in the difficulty file
                    //fileMgr.writeDifficultyToFile(writeTextDiff, avgs);

                    user_number++;

                }

                Console.WriteLine(watch.Elapsed);

                total_rating_avg_system /= task.num_users_init;
                total_similarity_avg_system /= task.num_users_init;
                //total_inaccuracy_system /= task.num_users_init;
                //writing some more global information
                //not used because self innacuracy it's not important anymore
                //fileMgr.writeGlobalAveragesInformation(total_rating_avg_system, total_similarity_avg_system, total_inaccuracy_system,
                //     task, writeTextAverages, users_profile, users_calculated_raitings);

                writeTextAverages.WriteLine("Avgs total (ratings)\t" + total_rating_avg_system);
                writeTextAverages.WriteLine("Avgs total (similarity)\t" + total_similarity_avg_system);

                //closing the three files
                writeText.Close();
                writeTextResult.Close();
                writeTextAverages.Close();
                //writeTextDiff.Close();


                Console.WriteLine("DONE");
                //Wait until fisnih
                Console.ReadLine();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
