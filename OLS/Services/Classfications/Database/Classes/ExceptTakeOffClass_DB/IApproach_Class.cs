using OLS.Services.Classfications.Database.Surfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLS.Services.Classfications.Database.Classes.ExceptTakeOffClass_DB
{
    public interface IApproach_Class
    {
        LanddingAttriputes landdingAttriputes { get; set; }
        InnerHorizontalAttriputes innerHorizontalAttriputes { get; set; }
        ConicalAttriputes conicalAttriputes { get; set; }
        TransvareAttriputes transvareAttriputes { get; set; }
    }
}
