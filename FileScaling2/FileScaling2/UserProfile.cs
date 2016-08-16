using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileScaling2
{
    class UserProfile
    {
        private String name;

        public String getName()
        {
            return name;
        }

        private double avg_ratings;

        public double getAvg_ratings()
        {
            return avg_ratings;
        }

        private int rank;

        public int getRank()
        {
            return rank;
        }

        public void setRank(int r)
        {
            rank = r;
        }

        // Constructor
        public UserProfile(String nm, double rating)
        {
            name = nm;
            avg_ratings = rating;
        }
    }
}
