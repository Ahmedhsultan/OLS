using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLS.Services.Classfications.Database.Surfaces
{
    public class InnerHorizontalAttriputes
    {
        public double surfaceHeightAboveRunway { get; set; }
        public double radius { get; set; }
        public InnerHorizontalAttriputes(double radius)
        {
            this.radius = radius;
            this.surfaceHeightAboveRunway = 45;
        }
    }
}
