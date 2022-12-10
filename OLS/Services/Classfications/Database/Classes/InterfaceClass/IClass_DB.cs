using OLS.Services.Classfications.Database.Surfaces;

namespace OLS.Services.Classfications.Database.Classes.InterfaceClass
{
    public interface IClass_DB
    {
        TakeOffAttriputes takeOffAttriputes { get; set; }
        LanddingAttriputes landdingAttriputes { get; set; }
        InnerHorizontalAttriputes innerHorizontalAttriputes { get; set; }
        ConicalAttriputes conicalAttriputes { get; set; }
        TransvareAttriputes transvareAttriputes { get; set; }
    }
}
