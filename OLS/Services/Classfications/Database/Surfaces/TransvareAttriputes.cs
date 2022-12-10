using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLS.Services.Classfications.Database.Surfaces
{
    public class TransvareAttriputes
    {
        public double slope { get; set; }

        public TransvareAttriputes(double slope)
        {
            this.slope = slope;
        }
    }
}
