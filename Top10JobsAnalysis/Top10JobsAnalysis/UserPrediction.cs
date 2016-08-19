using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Top10JobsAnalysis
{
    class UserPrediction
    {
        String user_name;
        String[] jobs_IDs;

        public UserPrediction(String name, String[] IDs)
        {
            user_name = name;
            jobs_IDs = IDs;
        }

        public String getName() { return user_name; }
        public String[] getJobsIDs() { return jobs_IDs; }
    }
}
