using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using OLS.Services.Classfications.Database.Surfaces;

namespace OLS.Services.OLSes
{
    public class Landding_OLS
    {
        public Polyline3d pl { get; set; }
        public Point3d p0 { get; set; }
        public Point3d p1 { get; set; }
        public Point3d p2 { get; set; }
        public Point3d p3 { get; set; }
        public Point3d p4 { get; set; }
        public Point3d p5 { get; set; }
        public Point3d p6 { get; set; }
        public Point3d p7 { get; set; }
        public Point3d p8 { get; set; }

        public Landding_OLS(LanddingAttriputes landdingAttriputes, Point3d startPoint, Vector3d vector3D, Vector3d prependicularVector)
        {
            p0 = startPoint.Add(vector3D.MultiplyBy(landdingAttriputes.safeArea));

            p1 = p0.Subtract(prependicularVector.MultiplyBy(landdingAttriputes.innerEdge / 2));
            p2 = p0.Add(prependicularVector.MultiplyBy(landdingAttriputes.innerEdge / 2));

            double z1 = p0.Z + (landdingAttriputes.s1 * landdingAttriputes.l1);
            double divWidth = landdingAttriputes.l1 * landdingAttriputes.divargence;
            p3 = p2.Add(vector3D.MultiplyBy( landdingAttriputes.l1)).Add(prependicularVector.MultiplyBy(divWidth));
            p3 = new Point3d(p3.X, p3.Y, z1);
            p4 = p1 + (vector3D * landdingAttriputes.l1) - (prependicularVector * divWidth);
            p4 = new Point3d(p4.X, p4.Y, z1);

            double z2 = p3.Z + (landdingAttriputes.s2 * landdingAttriputes.l2);
            double divWidth2 = landdingAttriputes.l2 * landdingAttriputes.divargence;
            p5 = p4 + (vector3D * landdingAttriputes.l2) - (prependicularVector * divWidth2);
            p5 = new Point3d(p5.X, p5.Y, z2);
            p6 = p3 + (vector3D * landdingAttriputes.l2) + (prependicularVector * divWidth2);
            p6 = new Point3d(p6.X, p6.Y, z2);

            double divWidth3 = landdingAttriputes.l3 * landdingAttriputes.divargence;
            p8 = p5 + (vector3D * landdingAttriputes.l3) - (prependicularVector * divWidth3);
            p7 = p6 + (vector3D * landdingAttriputes.l3) + (prependicularVector * divWidth3);
        }

        public void CreatePolylines(BlockTableRecord acBlkTblRec, Transaction trans)
        {
            pl = new Polyline3d();
            pl.SetDatabaseDefaults();
            acBlkTblRec.AppendEntity(pl);
            trans.AddNewlyCreatedDBObject(pl, true);

            PolylineVertex3d vertexP1 = new PolylineVertex3d(p1);
            PolylineVertex3d vertexP2 = new PolylineVertex3d(p2);
            PolylineVertex3d vertexP3 = new PolylineVertex3d(p3);
            PolylineVertex3d vertexP4 = new PolylineVertex3d(p4);
            PolylineVertex3d vertexP5 = new PolylineVertex3d(p5);
            PolylineVertex3d vertexP6 = new PolylineVertex3d(p6);
            PolylineVertex3d vertexP7 = new PolylineVertex3d(p7);
            PolylineVertex3d vertexP8 = new PolylineVertex3d(p8);

            pl.AppendVertex(vertexP1);
            pl.AppendVertex(vertexP2);
            pl.AppendVertex(vertexP3);
            pl.AppendVertex(vertexP6);
            pl.AppendVertex(vertexP7);
            pl.AppendVertex(vertexP8);
            pl.AppendVertex(vertexP5);
            pl.AppendVertex(vertexP4);
            pl.Closed = true;
        }

        public void CreateSurface(CivilDocument _civildoc, Transaction trans)
        {
            // Select a Surface style to use
            ObjectId styleId = _civildoc.Styles.SurfaceStyles[0];

            //Contour Entities collection
            ObjectIdCollection contourEntitiesIdColl = new ObjectIdCollection();
            contourEntitiesIdColl.Add(pl.ObjectId);

            if (styleId != null && contourEntitiesIdColl != null)
            {
                // Create an empty TIN Surface
                ObjectId surfaceId = TinSurface.Create("Landing_OLS", styleId);
                TinSurface surface = trans.GetObject(surfaceId, OpenMode.ForWrite) as TinSurface;
                surface.ContoursDefinition.AddContours(contourEntitiesIdColl, 1.0, 100.00, 15.0, 4.0);
            }
        }
    }
}

/*
 *                                  p4                                      p5                          p8
 * 
 * 
 *  p1
 *  
 *  p0
 *  
 *  p2
 *  
 *  
 *                                  p3                                      p6                          p7
 */