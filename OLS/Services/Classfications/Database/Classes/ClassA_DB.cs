using OLS.Services.Classfications.Database.Surfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLS.Services.Classfications.Database.Classes
{
    public class ClassA_DB
    {
        public TakeOffAttriputes takeOffAttriputes { get; set; }

        public ClassA_DB()
        {
            takeOffAttriputes = new TakeOffAttriputes(1, 1, 1, 1, 1, 1);
        }
    }
}
