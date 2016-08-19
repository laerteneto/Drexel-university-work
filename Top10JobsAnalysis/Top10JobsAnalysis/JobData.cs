using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Top10JobsAnalysis
{
    class JobData
    {
        String job_name;
        String job_ID;
        String job_description;
        private int[] job_metrics;

        public JobData(String name, String ID, String description, int[] metrics)
        {
            job_name = name;
            job_ID = ID;
            job_description = description;
            job_metrics = metrics;
        }

        public int[] getMetrics() { return job_metrics; }
        public String getName() { return job_name;  }
        public String getDescription() { return job_description; }
        public String getID() { return job_ID; }
    }
}
