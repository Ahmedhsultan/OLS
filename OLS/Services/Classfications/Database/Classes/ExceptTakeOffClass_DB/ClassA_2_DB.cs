using OLS.Services.Classfications.Database.Classes.InterfaceClass;
using OLS.Services.Classfications.Database.Surfaces;

namespace OLS.Services.Classfications.Database.Classes.ExceptTakeOffClass_DB
{
    public class ClassA_2_DB : IApproach_Class
    {
        public LanddingAttriputes landdingAttriputes { get; set; }
        public ConicalAttriputes conicalAttriputes { get; set; }
        public TransvareAttriputes transvareAttriputes { get; set; }
        public InnerHorizontalAttriputes innerHorizontalAttriputes { get; set; }

        public ClassA_2_DB()
        {
            landdingAttriputes = new LanddingAttriputes()
            {
                safeArea = 60,
                totalLength = 2500,
                divargence = 0.1,
                innerEdge = 80,
                s1 = 0.04,
                l1 = 2500,
                s2 = 0,
                l2 = 0,
                s3 = 0,
                l3 = 0
            };
            innerHorizontalAttriputes = new InnerHorizontalAttriputes()
            {
                radius = 2500
            };
            conicalAttriputes = new ConicalAttriputes()
            {
                slope = 0.05,
                height = 55
            };
            transvareAttriputes = new TransvareAttriputes()
            {
                slope = 0.2
            };
        }
    }
}
