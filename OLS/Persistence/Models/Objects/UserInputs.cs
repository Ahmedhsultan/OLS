using Autodesk.Civil.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLS.Persistence.Models.Objects
{
    public class UserInputs
    {
        public List<Profile> allProfiles { get; set; }
        public double startStation { get; set; }
        public double endStation { get; set; }

        public UserInputs()
        {
            allProfiles = new List<Profile>();
        }
    }
}
