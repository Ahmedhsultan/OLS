using Autodesk.Civil.DatabaseServices;
using OLS.Persistence.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLS.Persistence
{
    internal class RunwayDB
    {
        public List<Runway> runwaysList { get; set; }
        public static RunwayDB instance { get; private set; }
        private RunwayDB()
        {
            runwaysList = new List<Runway>();
        }

        public static RunwayDB getInstance()
        {
            if(instance == null)
                instance = new RunwayDB();
            return instance;
        }
    }
}
