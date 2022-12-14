using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using OLS.Services.Classfications.Database.Surfaces;
using System;

namespace OLS.Services.OLSes
{
    public class TakeOff_OLS
    {
        public Polyline3d pl { get; set; }
        public Point3d p0 { get; set; }
        public Point3d p1 { get; set; }
        public Point3d p2 { get; set; }
        public Point3d p3 { get; set; }
        public Point3d p4 { get; set; }
        public Point3d p5 { get; set; }
        public Point3d p6 { get; set; }

        public TakeOff_OLS(TakeOffAttriputes takeOffAttriputes, Point3d startPoint, Vector3d vector3D, Vector3d prependicularVector)
        {
            p0 = startPoint + (vector3D * takeOffAttriputes.safeArea);

            p1 = p0 - (prependicularVector * (takeOffAttriputes.innerEdge / 2));
            p2 = p0 + (prependicularVector * (takeOffAttriputes.innerEdge / 2));

            double divWidth = (takeOffAttriputes.finalWidth - takeOffAttriputes.innerEdge) / 2;
            double firstLength = Math.Abs(divWidth / takeOffAttriputes.divargence);
            double z2 = p0.Z + (takeOffAttriputes.slope * firstLength);
            p3 = p2 + (vector3D * firstLength) + (prependicularVector * divWidth);
            p3 = new Point3d(p3.X, p3.Y, z2);
            p4 = p1 + (vector3D * firstLength) - (prependicularVector * divWidth);
            p4 = new Point3d(p4.X, p4.Y, z2);

            double z3 = p0.Z + (takeOffAttriputes.slope * takeOffAttriputes.totalLength);
            p5 = p4 + (vector3D * (takeOffAttriputes.totalLength - firstLength));
            p5 = new Point3d(p5.X, p5.Y, z3);
            p6 = p3 + (vector3D * (takeOffAttriputes.totalLength - firstLength));
            p6 = new Point3d(p6.X, p6.Y, z3);
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

            pl.AppendVertex(vertexP1);
            pl.AppendVertex(vertexP2);
            pl.AppendVertex(vertexP3);
            pl.AppendVertex(vertexP6);
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
                ObjectId surfaceId = TinSurface.Create("TakeOff_OLS", styleId); //Refactor Naming
                TinSurface surface = trans.GetObject(surfaceId, OpenMode.ForWrite) as TinSurface;
                surface.ContoursDefinition.AddContours(contourEntitiesIdColl, 1.0, 100.00, 15.0, 4.0);
            }
        }
    }
}

/*
 *                                  p4                                      p5 
 * 
 * 
 *  p1
 *  
 *  p0
 *  
 *  p2
 *  
 *  
 *                                  p3                                      p6
 */