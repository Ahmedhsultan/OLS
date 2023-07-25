using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using OLS.Services.Classfications.Database.Surfaces;

namespace OLS.Services.OLSes
{
    public class Conical_OLS
    {
        public Polyline pline { get; set; }
        public double surfaceLevel { get; set; }
        public double deltaWidth { get; set; }
        public InnerHorizontal_OLS innerHorizontal_OLS { get; set; }
        public Conical_OLS(ConicalAttriputes conicalAttriputes, InnerHorizontal_OLS innerHorizontal_OLS)
        {
            this.innerHorizontal_OLS = innerHorizontal_OLS;
            surfaceLevel = innerHorizontal_OLS.surfaceLevel + conicalAttriputes.height;
            deltaWidth = conicalAttriputes.height / conicalAttriputes.slope;
        }

        public void CreatePolylines(Database db, Transaction trans)
        {
            DBObjectCollection acDbObjColl = innerHorizontal_OLS.pline.GetOffsetCurves(-deltaWidth);
            // Step through the new objects created
            foreach (Autodesk.AutoCAD.DatabaseServices.Entity acEnt in acDbObjColl)
            {
                pline = acEnt as Polyline;
                //Rise pline
                pline.Elevation = surfaceLevel;
                // Add each offset object
                var curSpace = (BlockTableRecord)trans.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                curSpace.AppendEntity(acEnt);
                trans.AddNewlyCreatedDBObject(acEnt, true);

                trans.Commit();
            }
        }

        public void CreateSurface(CivilDocument _civildoc, Transaction trans)
        {
            // Select a Surface style to use
            ObjectId styleId = _civildoc.Styles.SurfaceStyles[0];

            //Breakline Entities collection
            ObjectIdCollection contourEntitiesIdColl = new ObjectIdCollection();
            contourEntitiesIdColl.Add(pline.ObjectId);
            //contourEntitiesIdColl.Add(pline.ObjectId);

            if (styleId != null && contourEntitiesIdColl != null)
            {
                // Create an empty TIN Surface
                ObjectId surfaceId = TinSurface.Create("Conical_OLS", styleId);
                TinSurface surface = trans.GetObject(surfaceId, OpenMode.ForWrite) as TinSurface;
                //Past innerHorizontal surface in conical surface
                surface.PasteSurface(innerHorizontal_OLS.surface.ObjectId);
                //Add breakline to top conical
                surface.BreaklinesDefinition.AddStandardBreaklines(contourEntitiesIdColl, 1.0, 100.00, 15.0, 4.0);
            }
        }
    }
}
