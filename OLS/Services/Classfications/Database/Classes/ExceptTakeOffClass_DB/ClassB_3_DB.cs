using OLS.Services.Classfications.Database.Classes.InterfaceClass;
using OLS.Services.Classfications.Database.Surfaces;

namespace OLS.Services.Classfications.Database.Classes.ExceptTakeOffClass_DB
{
    public class ClassB_3_DB : IApproach_Class
    {
        public LanddingAttriputes landdingAttriputes { get; set; }
        public ConicalAttriputes conicalAttriputes { get; set; }
        public TransvareAttriputes transvareAttriputes { get; set; }
        public InnerHorizontalAttriputes innerHorizontalAttriputes { get; set; }

        public ClassB_3_DB()
        {
            conicalAttriputes = new ConicalAttriputes()
            {
                slope = 0.05,
                height = 100
            };
            innerHorizontalAttriputes = new InnerHorizontalAttriputes()
            {
                radius = 4000
            };
            landdingAttriputes = new LanddingAttriputes()
            {
                innerEdge = 280,
                safeArea = 60,
                divargence = 0.15,
                l1 = 3000,
                s1 = 0.02,
                l2 = 3600,
                s2 = 0.025,
                l3 = 8400,
                s3 = 0,
                totalLength = 15000
            };
            transvareAttriputes = new TransvareAttriputes()
            {
                slope = 0.143
            };
        }
    }
}
