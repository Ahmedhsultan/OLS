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
            takeOffAttriputes = new TakeOffAttriputes()
            {
                safeArea = 60,
                innerEdge = 180,
                totalLength = 15000,
                finalWidth = 1200,
                divargence = 0.125,
                slope = 0.02
            };
            landdingAttriputes = new LanddingAttriputes()
            {
                safeArea = 60,
                totalLength = 15000,
                divargence = 0.15,
                innerEdge = 280,
                s1 = 0.02,
                l1 = 3000,
                s2 = 0.025,
                l2 = 3600,
                s3 = 0,
                l3 = 8400
            };
            innerHorizontalAttriputes = new InnerHorizontalAttriputes()
            {
                radius = 2000
            };
            conicalAttriputes = new ConicalAttriputes()
            {
                slope = 0.05,
                height = 35
            };
            transvareAttriputes = new TransvareAttriputes()
            {
                slope = 0.143
            };
        }
    }
}
