using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLS.Services.Classfications.Database.Surfaces
{
    public class TakeOffAttriputes
    {
        public double safeArea { get; set; }
        public double innerEdge { get; set; }
        public double totalLength { get; set; }
        public double finalWidth { get; set; }
        public double divargence { get; set; }
        public double slope { get; set; }

        public TakeOffAttriputes(double safeArea, double innerEdge, double totalLength, double finalWidth, double divargence, double slope)
        {
            this.safeArea = safeArea;
            this.innerEdge = innerEdge;
            this.totalLength = totalLength;
            this.finalWidth = finalWidth;
            this.divargence = divargence;
            this.slope = slope;
        }
    }
}
