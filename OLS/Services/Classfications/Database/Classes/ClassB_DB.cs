using OLS.Services.Classfications.Database.Classes.InterfaceClass;
using OLS.Services.Classfications.Database.Surfaces;

namespace OLS.Services.Classfications.Database.Classes
{
    public class ClassB_DB : IClass_DB
    {
        public TakeOffAttriputes takeOffAttriputes { get; set; }
        public LanddingAttriputes landdingAttriputes { get; set; }
        public ConicalAttriputes conicalAttriputes { get; set; }
        public TransvareAttriputes transvareAttriputes { get; set; }
        public InnerHorizontalAttriputes innerHorizontalAttriputes { get; set; }

        public ClassB_DB()
        {
            takeOffAttriputes = new TakeOffAttriputes()
            {
                safeArea = 60,
                innerEdge = 180,
                totalLength = 15000,
                finalWidth = 1200,
                divargence = 0.125,
                slope = 0.02
            };
            landdingAttriputes = new LanddingAttriputes(60, 280, 15000, 0.15, 0.02, 3000, 0.025, 3600, 0, 8400);
            innerHorizontalAttriputes = new InnerHorizontalAttriputes(4000);
            conicalAttriputes = new ConicalAttriputes(0.05,100);
            transvareAttriputes = new TransvareAttriputes(0.143);
        }
    }
}
