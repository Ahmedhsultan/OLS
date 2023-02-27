using OLS.Services.Classfications.Database.Classes.InterfaceClass;
using OLS.Services.Classfications.Database.Surfaces;

namespace OLS.Services.Classfications.Database.Classes.ExceptTakeOffClass_DB
{
    public class ClassC_1_DB : IApproach_Class
    {
        public LanddingAttriputes landdingAttriputes { get; set; }
        public ConicalAttriputes conicalAttriputes { get; set; }
        public TransvareAttriputes transvareAttriputes { get; set; }
        public InnerHorizontalAttriputes innerHorizontalAttriputes { get; set; }

        public ClassC_1_DB()
        {
            conicalAttriputes = new ConicalAttriputes()
            {
                slope = 0.05,
                height = 60
            };
            innerHorizontalAttriputes = new InnerHorizontalAttriputes()
            {
                radius = 3500
            };
            landdingAttriputes = new LanddingAttriputes()
            {
                innerEdge = 140,
                safeArea = 60,
                divargence = 0.15,
                l1 = 3000,
                s1 = 0.025,
                l2 = 12000,
                s2 = 0.03,
                l3 = 0,
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
