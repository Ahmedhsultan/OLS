using OLS.Services.Classfications.Database.Surfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLS.Services.Classfications.Database.Classes.TakeOffClass_DB
{
    public interface ITakeOff_Class
    {
        TakeOffAttriputes takeOffAttriputes { get; set; }
    }
}
