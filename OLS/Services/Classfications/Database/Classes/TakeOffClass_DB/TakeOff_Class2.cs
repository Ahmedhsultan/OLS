using OLS.Services.Classfications.Database.Surfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLS.Services.Classfications.Database.Classes.TakeOffClass_DB
{
    public class TakeOff_Class2 : ITakeOff_Class
    {
        public TakeOffAttriputes takeOffAttriputes { get; set; }

        public TakeOff_Class2()
        {
            takeOffAttriputes = new TakeOffAttriputes()
            {
                innerEdge = 80,
                safeArea = 60,
                divargence = 0.1,
                finalWidth = 580,
                totalLength = 2500,
                slope = 0.04
            };
        }
    }
}
