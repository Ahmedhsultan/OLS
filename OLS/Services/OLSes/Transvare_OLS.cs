using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using OLS.Services.Classfications.Database.Surfaces;

namespace OLS.Services.OLSes
{
    public class Transvare_OLS
    {
        public Polyline3d pl1 { get; set; }
        public Polyline3d pl2 { get; set; }
        public Point3d p0 { get; set; }
        public Point3d p00 { get; set; }
        public Point3d p1 { get; set; }
        public Point3d p2 { get; set; }
        public Point3d p3 { get; set; }
        public Point3d p4 { get; set; }
        public Point3d p5 { get; set; }
        public Point3d p6 { get; set; }
        public Point3d p7 { get; set; }
        public Point3d p8 { get; set; }

        public Transvare_OLS(TransvareAttriputes transvareAttriputes, LanddingAttriputes landdingAttriputes, InnerHorizontalAttriputes innerHorizontalAttriputes,
                                InnerHorizontal_OLS innerHorizontal_OLS,
                                Point3d startPoint, Point3d endPoint, Vector3d startVector3D, Vector3d endVector3D, Vector3d prependicularVector)
        {
            //Upper Transational OLS
            p0 = startPoint + (startVector3D * landdingAttriputes.safeArea);
            p1 = p0 + (prependicularVector * (landdingAttriputes.innerEdge / 2));
            p5 = p0 - (prependicularVector * (landdingAttriputes.innerEdge / 2));

            double l = (innerHorizontal_OLS.surfaceLevel- startPoint.Z)/ transvareAttriputes.slope;
            double z1 = p0.Z + (landdingAttriputes.s1 * landdingAttriputes.l1);
            double divWidth = landdingAttriputes.l1 * landdingAttriputes.divargence;

            p4 = p1 + (startVector3D * landdingAttriputes.l1) + (prependicularVector * divWidth);
            p4 = new Point3d(p4.X, p4.Y, z1);
            Vector3d vector3DP4 = new Vector3d(p4.X-p1.X, p4.Y-p1.Y, p4.Z-p1.Z);
            vector3DP4 = vector3DP4.GetNormal();
            p4 = p1 + vector3DP4 * l;

            p8 = p5 + (startVector3D * landdingAttriputes.l1) - (prependicularVector * divWidth);
            p8 = new Point3d(p8.X, p8.Y, z1);
            Vector3d vector3DP8 = new Vector3d(p8.X - p1.X, p8.Y - p1.Y, p8.Z - p1.Z);
            vector3DP8 = vector3DP8.GetNormal();
            p4 = p5 + vector3DP8 * l;

            //Down Transational OLS
            p00 = endPoint + (endVector3D * landdingAttriputes.safeArea);
            p2 = p00 + (prependicularVector * (landdingAttriputes.innerEdge / 2));
            p6 = p00 - (prependicularVector * (landdingAttriputes.innerEdge / 2));

            l = (innerHorizontal_OLS.surfaceLevel - endPoint.Z) / transvareAttriputes.slope;
            z1 = p00.Z + (landdingAttriputes.s1 * landdingAttriputes.l1);
            divWidth = landdingAttriputes.l1 * landdingAttriputes.divargence;

            p3 = p2 + (startVector3D * landdingAttriputes.l1) + (prependicularVector * divWidth);
            p3 = new Point3d(p3.X, p3.Y, z1);
            Vector3d vector3DP3 = new Vector3d(p3.X - p2.X, p3.Y - p2.Y, p3.Z - p2.Z);
            vector3DP3.GetNormal();
            p3 = p2 + vector3DP3 * l;

            p7 = p6 + (startVector3D * landdingAttriputes.l1) - (prependicularVector * divWidth);
            p7 = new Point3d(p7.X, p7.Y, z1);
            Vector3d vector3DP7 = new Vector3d(p7.X - p6.X, p7.Y - p6.Y, p7.Z - p6.Z);
            vector3DP7.GetNormal();
            p7 = p6 + vector3DP7 * l;
        }

        public void CreatePolylines(BlockTableRecord acBlkTblRec, Transaction trans)
        {
            //Upper Transational OLS
            pl1 = new Polyline3d();
            pl1.SetDatabaseDefaults();
            acBlkTblRec.AppendEntity(pl1);
            trans.AddNewlyCreatedDBObject(pl1, true);

            PolylineVertex3d vertexP1 = new PolylineVertex3d(p1);
            PolylineVertex3d vertexP2 = new PolylineVertex3d(p2);
            PolylineVertex3d vertexP3 = new PolylineVertex3d(p3);
            PolylineVertex3d vertexP4 = new PolylineVertex3d(p4);

            pl1.AppendVertex(vertexP1);
            pl1.AppendVertex(vertexP2);
            pl1.AppendVertex(vertexP3);
            pl1.AppendVertex(vertexP4);
            pl1.Closed = true;


            //Down Transational OLS
            pl2 = new Polyline3d();
            pl2.SetDatabaseDefaults();
            acBlkTblRec.AppendEntity(pl2);
            trans.AddNewlyCreatedDBObject(pl2, true);

            PolylineVertex3d vertexP5 = new PolylineVertex3d(p5);
            PolylineVertex3d vertexP6 = new PolylineVertex3d(p6);
            PolylineVertex3d vertexP7 = new PolylineVertex3d(p7);
            PolylineVertex3d vertexP8 = new PolylineVertex3d(p8);

            pl2.AppendVertex(vertexP5);
            pl2.AppendVertex(vertexP6);
            pl2.AppendVertex(vertexP7);
            pl2.AppendVertex(vertexP8);
            pl2.Closed = true;
        }

        public void CreateSurface(CivilDocument _civildoc, Transaction trans)
        {
            //Upper Transational OLS
            // Select a Surface style to use
            ObjectId styleId = _civildoc.Styles.SurfaceStyles[0];

            //Contour Entities collection
            ObjectIdCollection contourEntitiesIdColl1 = new ObjectIdCollection();
            contourEntitiesIdColl1.Add(pl1.ObjectId);

            if (styleId != null && contourEntitiesIdColl1 != null)
            {
                // Create an empty TIN Surface
                ObjectId surfaceId1 = TinSurface.Create("TransationR_OLS", styleId); //Refactor Naming
                TinSurface surface1 = trans.GetObject(surfaceId1, OpenMode.ForWrite) as TinSurface;
                surface1.ContoursDefinition.AddContours(contourEntitiesIdColl1, 1.0, 100.00, 15.0, 4.0);
            }

            //Down Transational OLS
            //Contour Entities collection
            ObjectIdCollection contourEntitiesIdColl2 = new ObjectIdCollection();
            contourEntitiesIdColl2.Add(pl1.ObjectId);

            if (styleId != null && contourEntitiesIdColl2 != null)
            {
                // Create an empty TIN Surface
                ObjectId surfaceId2 = TinSurface.Create("TransationL_OLS", styleId); //Refactor Naming
                TinSurface surface2 = trans.GetObject(surfaceId2, OpenMode.ForWrite) as TinSurface;
                surface2.ContoursDefinition.AddContours(contourEntitiesIdColl2, 1.0, 100.00, 15.0, 4.0);
            }
        }
    }
}

/*
 *  p4                                                                      p3
 * 
 * 
 *                  p1                                  p2
 *  
 *                  p0                                  p00
 *  
 *                  p5                                  p6
 *  
 *  
 *  p8                                                                      p7
 */