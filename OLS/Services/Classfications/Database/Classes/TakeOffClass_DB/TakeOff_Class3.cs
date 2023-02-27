using OLS.Services.Classfications.Database.Surfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLS.Services.Classfications.Database.Classes.TakeOffClass_DB
{
    public class TakeOff_Class3 : ITakeOff_Class
    {
        public TakeOffAttriputes takeOffAttriputes { get; set; }

        public TakeOff_Class3()
        {
            takeOffAttriputes = new TakeOffAttriputes()
            {
                innerEdge = 180,
                safeArea = 60,
                divargence = 0.125,
                finalWidth = 1200,
                totalLength = 15000,
                slope = 0.02
            };
        }
    }
}
