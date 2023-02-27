using OLS.Services.Classfications.Database.Classes.InterfaceClass;
using OLS.Services.Classfications.Database.Surfaces;

namespace OLS.Services.Classfications.Database.Classes.ExceptTakeOffClass_DB
{
    public class ClassA_4_DB : IApproach_Class
    {
        public LanddingAttriputes landdingAttriputes { get; set; }
        public ConicalAttriputes conicalAttriputes { get; set; }
        public TransvareAttriputes transvareAttriputes { get; set; }
        public InnerHorizontalAttriputes innerHorizontalAttriputes { get; set; }

        public ClassA_4_DB()
        {
            landdingAttriputes = new LanddingAttriputes()
            {
                safeArea = 60,
                totalLength = 3000,
                divargence = 0.1,
                innerEdge = 150,
                s1 = 0.025,
                l1 = 3000,
                s2 = 0,
                l2 = 0,
                s3 = 0,
                l3 = 0
            };
            innerHorizontalAttriputes = new InnerHorizontalAttriputes()
            {
                radius = 4000
            };
            conicalAttriputes = new ConicalAttriputes()
            {
                slope = 0.05,
                height = 100
            };
            transvareAttriputes = new TransvareAttriputes()
            {
                slope = 0.143
            };
        }
    }
}
