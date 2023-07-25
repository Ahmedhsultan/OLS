using Autodesk.Civil.DatabaseServices;
using OLS.Persistence.Models.Objects;
using OLS.Services.OLSes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLS.Persistence.Models
{
    internal class Runway
    {
        public Alignment alignment { get; set; }
        public Profile profile { get; set; }
        
        public UserInputs userInputs { get; set; }

        public TakeOff_OLS takeOff_OLS_Start { get; set; }
        public TakeOff_OLS takeOff_OLS_End { get; set; }

        public Landding_OLS landding_OLS_Start { get; set; }
        public Landding_OLS landding_OLS_End { get; set; }

        public InnerHorizontal_OLS innerHorizontal_OLS { get; set; }

        public Conical_OLS conical_OLS { get; set; }
        
        public Transvare_OLS transvare_OLS_Start { get; set; }
        public Transvare_OLS transvare_OLS_End { get; set; }


        public Runway()
        {
            userInputs = new UserInputs();
        }
    }
}
