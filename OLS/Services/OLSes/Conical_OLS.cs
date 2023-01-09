using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using OLS.Services.Classfications.Database.Surfaces;

namespace OLS.Services.OLSes
{
    public class Conical_OLS
    {
        public Circle topCirc { get; set; }
        public Circle buttomCirc { get; set; }
        public double surfaceLevel { get; set; }
        public Point3d topCenter { get; set; }
        public Point3d buttomCenter { get; set; }
        public double buttmRadius { get; set; }
        public double topRadius { get; set; }
        public Conical_OLS(ConicalAttriputes conicalAttriputes, InnerHorizontal_OLS innerHorizontal_OLS)
        {
            /*surfaceLevel = innerHorizontal_OLS.surfaceLevel;

            double topCenter_X = innerHorizontal_OLS.center.X;
            double topCenter_Y = innerHorizontal_OLS.center.Y;
            double topCenter_Z = innerHorizontal_OLS.center.Z + conicalAttriputes.height;
            topCenter = new Point3d(topCenter_X,topCenter_Y,topCenter_Z);
            buttomCenter = innerHorizontal_OLS.center;

            double deltaWidth = conicalAttriputes.slope * conicalAttriputes.height;
            topRadius = innerHorizontal_OLS.radius + deltaWidth;
            buttmRadius = innerHorizontal_OLS.radius;*/
        }

        public void CreatePolylines(BlockTableRecord acBlkTblRec, Transaction trans)
        {
            /*buttomCirc = new Circle();
            buttomCirc.SetDatabaseDefaults();
            buttomCirc.Center = buttomCenter;
            buttomCirc.Radius = buttmRadius;
            acBlkTblRec.AppendEntity(buttomCirc);
            trans.AddNewlyCreatedDBObject(buttomCirc, true);

            topCirc = new Circle();
            topCirc.SetDatabaseDefaults();
            topCirc.Center = topCenter;
            topCirc.Radius = topRadius;
            acBlkTblRec.AppendEntity(topCirc);
            trans.AddNewlyCreatedDBObject(topCirc, true);*/
        }

        public void CreateSurface(CivilDocument _civildoc, Transaction trans)
        {
            /*// Select a Surface style to use
            ObjectId styleId = _civildoc.Styles.SurfaceStyles[0];

            //Breakline Entities collection
            ObjectIdCollection contourEntitiesIdColl = new ObjectIdCollection();
            contourEntitiesIdColl.Add(buttomCirc.ObjectId);
            contourEntitiesIdColl.Add(topCirc.ObjectId);

            if (styleId != null && contourEntitiesIdColl != null)
            {
                // Create an empty TIN Surface
                ObjectId surfaceId = TinSurface.Create("Conical_OLS", styleId);
                TinSurface surface = trans.GetObject(surfaceId, OpenMode.ForWrite) as TinSurface;
                surface.BreaklinesDefinition.AddStandardBreaklines(contourEntitiesIdColl, 1.0, 100.00, 15.0, 4.0);
            }*/
        }
    }
}
