using OLS.Services.Classfications.Database.Surfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLS.Services.Classfications.Database.Classes.TakeOffClass_DB
{
    public class TakeOff_Class1 : ITakeOff_Class
    {
        public TakeOffAttriputes takeOffAttriputes { get; set; }

        public TakeOff_Class1()
        {
            takeOffAttriputes = new TakeOffAttriputes()
            {
                innerEdge = 60,
                safeArea = 30,
                divargence = 0.1,
                finalWidth = 380,
                totalLength = 1600,
                slope = 0.05
            };
        }
    }
}
