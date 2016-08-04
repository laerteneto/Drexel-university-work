﻿using recommenderSystems.Domain;
using recommenderSystems.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace recommenderSystems.Service.Plugin
{
    public class MatlabSvcImpl : IMatlabSvc
    {
        //Create the MATLAB instance (reference)
        public MatlabSvcImpl()
        {
            //this is used to do not show the matlab window
            //matlab.Visible = 0;
        }

        //Changes to the directory where the functions are located --- always use the desktop to make tests to avoid problems with pathway
        public bool changeDirectory(String path, MLApp.MLApp matlab)
        {
            try
            {
                matlab.Execute(@"cd " + path);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //Excecutes a filter in matlab to calculate the recommeded jobs for an user
        public object[] executeFilter(TaskDimensions task, String[] job_list, String path, double[,] my_ratings, double[,] Y, double[,] R, double[,] X, MLApp.MLApp matlab)
        {
            try
            {
                // Define the output to print the final result
                object result_job_search = null;

                this.changeDirectory(path, matlab);

                // Job recommendations script that will give as result 6 objects described in matlab
                matlab.Feval("scriptGeneration", 6, out result_job_search, my_ratings, job_list, Y, R, X, task.num_features);
                object[] res = result_job_search as object[];

                return res;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
                
    }
}
