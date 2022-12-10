using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLS.Services.Classfications.Database.Surfaces
{
    public class ConicalAttriputes
    {
        public double slope { get; set; }
        public double height { get; set; }

        public ConicalAttriputes(double slope,double height)
        {
            this.slope = slope;
            this.height = height;
        }
    }
}
