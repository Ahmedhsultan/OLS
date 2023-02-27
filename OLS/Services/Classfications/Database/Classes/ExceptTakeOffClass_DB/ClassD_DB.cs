using OLS.Services.Classfications.Database.Classes.InterfaceClass;
using OLS.Services.Classfications.Database.Surfaces;

namespace OLS.Services.Classfications.Database.Classes.ExceptTakeOffClass_DB
{
    public class ClassD_DB : IApproach_Class
    {
        public LanddingAttriputes landdingAttriputes { get; set; }
        public ConicalAttriputes conicalAttriputes { get; set; }
        public TransvareAttriputes transvareAttriputes { get; set; }
        public InnerHorizontalAttriputes innerHorizontalAttriputes { get; set; }

        public ClassD_DB()
        {
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