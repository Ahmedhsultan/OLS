using OLS.Services.Classfications.Database.Classes.InterfaceClass;
using OLS.Services.Classfications.Database.Surfaces;

namespace OLS.Services.Classfications.Database.Classes.ExceptTakeOffClass_DB
{
    public class ClassA_1_DB : IApproach_Class
    {
        public LanddingAttriputes landdingAttriputes { get; set; }
        public ConicalAttriputes conicalAttriputes { get; set; }
        public TransvareAttriputes transvareAttriputes { get; set; }
        public InnerHorizontalAttriputes innerHorizontalAttriputes { get; set; }

        public ClassA_1_DB()
        {
            landdingAttriputes = new LanddingAttriputes()
            {
                safeArea = 30,
                totalLength = 1600,
                divargence = 0.1,
                innerEdge = 60,
                s1 = 0.05,
                l1 = 1600,
                s2 = 0,
                l2 = 0,
                s3 = 0,
                l3 = 0
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
                slope = 0.2
            };
        }
    }
}
