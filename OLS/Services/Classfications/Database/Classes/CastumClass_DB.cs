using OLS.Services.Classfications.Database.Classes.InterfaceClass;
using OLS.Services.Classfications.Database.Surfaces;

namespace OLS.Services.Classfications.Database.Classes
{
    public class CastumClass_DB : IClass_DB
    {
        private static CastumClass_DB obj;

        public TakeOffAttriputes takeOffAttriputes { get; set; }
        public LanddingAttriputes landdingAttriputes { get; set; }
        public ConicalAttriputes conicalAttriputes { get; set; }
        public TransvareAttriputes transvareAttriputes { get; set; }
        public InnerHorizontalAttriputes innerHorizontalAttriputes { get; set; }

        private CastumClass_DB()
        {

        }

        public static CastumClass_DB getIntstance()
        {
            if (obj == null)
                obj = new CastumClass_DB();

            return obj;
        }
    }
}
