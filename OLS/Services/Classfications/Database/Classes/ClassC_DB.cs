using OLS.Services.Classfications.Database.Classes.InterfaceClass;
using OLS.Services.Classfications.Database.Surfaces;

namespace OLS.Services.Classfications.Database.Classes
{
    public class ClassC_DB : IClass_DB
    {
        public TakeOffAttriputes takeOffAttriputes { get; set; }
        public LanddingAttriputes landdingAttriputes { get; set; }
        public ConicalAttriputes conicalAttriputes { get; set; }
        public TransvareAttriputes transvareAttriputes { get; set; }
        public InnerHorizontalAttriputes innerHorizontalAttriputes { get; set; }

        public ClassC_DB()
        {
            takeOffAttriputes = new TakeOffAttriputes(60, 60, 1800, 380, 0.1, 0.1);
            landdingAttriputes = new LanddingAttriputes(60, 60, 15000, 0.15, 0.02, 3000, 0.025, 3600, 0, 8400);
            innerHorizontalAttriputes = new InnerHorizontalAttriputes(2000);
            conicalAttriputes = new ConicalAttriputes(0.05,35);
            transvareAttriputes = new TransvareAttriputes(0.2);
        }
    }
}
