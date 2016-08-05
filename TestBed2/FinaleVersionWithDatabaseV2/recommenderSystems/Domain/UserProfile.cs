using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace recommenderSystems.Domain
{
    public class UserProfile
    {
        private string userID;

        public string UserID
        {
            get { return userID; }
            set { userID = value; }
        }

        //constructor
        public UserProfile(string userID)//, double userRating)
        {
            this.userID = userID;
        }
        
    }
}
