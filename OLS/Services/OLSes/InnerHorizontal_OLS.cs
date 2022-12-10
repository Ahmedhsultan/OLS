using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using OLS.Services.Classfications.Database.Surfaces;

namespace OLS.Services.OLSes
{
    public class InnerHorizontal_OLS
    {
        public Circle Circ { get; set; }
        public double surfaceLevel { get; set; }
        public Point3d center { get; set; }
        public double radius { get; set; }
        public InnerHorizontal_OLS(InnerHorizontalAttriputes innerHorizontalAttriputes, Point3d startPoint, Point3d endPoint)
        {
            surfaceLevel = startPoint.Z > endPoint.Z ? startPoint.Z : endPoint.Z;
            surfaceLevel += 45;

            double centerX = (startPoint.X + endPoint.X) / 2;
            double centerY = (startPoint.Y + endPoint.Y) / 2;
            double centerZ = surfaceLevel;
            center = new Point3d(centerX, centerY, centerZ);

            radius = innerHorizontalAttriputes.radius;
        }

        public void CreatePolylines(BlockTableRecord acBlkTblRec, Transaction trans)
        {
            Circ = new Circle();
            Circ.SetDatabaseDefaults();
            Circ.Center = center;
            Circ.Radius = radius;
            acBlkTblRec.AppendEntity(Circ);
            trans.AddNewlyCreatedDBObject(Circ, true);
        }

        public void CreateSurface(CivilDocument _civildoc, Transaction trans)
        {
            // Select a Surface style to use
            ObjectId styleId = _civildoc.Styles.SurfaceStyles[0];

            //Breakline Entities collection
            ObjectIdCollection contourEntitiesIdColl = new ObjectIdCollection();
            contourEntitiesIdColl.Add(Circ.ObjectId);

            if (styleId != null && contourEntitiesIdColl != null)
            {
                // Create an empty TIN Surface
                ObjectId surfaceId = TinSurface.Create("InnerHorizontal_OLS", styleId);
                TinSurface surface = trans.GetObject(surfaceId, OpenMode.ForWrite) as TinSurface;
                surface.BreaklinesDefinition.AddStandardBreaklines(contourEntitiesIdColl, 1.0, 100.00, 15.0, 4.0);
            }
        }
    }
}
