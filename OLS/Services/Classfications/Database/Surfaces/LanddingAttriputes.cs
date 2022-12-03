using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLS.Services.Classfications.Database.Surfaces
{
    public class LanddingAttriputes
    {
        public double safeArea { get; set; }
        public double innerEdge { get; set; }
        public double totalLength { get; set; }
        public double divargence { get; set; }
        public double s1 { get; set; }
        public double s2 { get; set; }
        public double s3 { get; set; }
        public double l1 { get; set; }
        public double l2 { get; set; }
        public double l3 { get; set; }

        public LanddingAttriputes(double safeArea, double innerEdge, double totalLength, double divargence, double s1, double l1, double s2, double l2, double s3, double l3)
        {
            this.safeArea = safeArea;
            this.innerEdge = innerEdge;
            this.totalLength = totalLength;
            this.divargence = divargence;
            this.s1 = s1;
            this.s2 = s2;
            this.s3 = s3;
            this.l1 = l1;
            this.l2 = l2;
            this.l3 = l3;
        }
    }
}
