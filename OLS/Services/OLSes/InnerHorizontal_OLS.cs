using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using OLS.Services.Classfications.Database.Surfaces;
using System;

namespace OLS.Services.OLSes
{
    public class InnerHorizontal_OLS
    {
        public double surfaceLevel { get; set; }
        public Point3d p1 { get; set; }
        public Point3d p2 { get; set; }
        public Point3d p3 { get; set; }
        public Point3d p4 { get; set; }
        public Point3d p5 { get; set; }
        public Point3d p6 { get; set; }
        public Polyline pline { get; set; }
        public TinSurface surface { get; set; }

        public InnerHorizontal_OLS(InnerHorizontalAttriputes innerHorizontalAttriputes, Point3d startPoint, Point3d endPoint,
            Vector3d startAlignmentVector, Vector3d startPrepAlignmentVector, Vector3d endAlignmentVector, Vector3d endPrepAlignmentVector)
        {
            surfaceLevel = startPoint.Z > endPoint.Z ? startPoint.Z : endPoint.Z;
            surfaceLevel += 45;

            p1 = startPoint.Add(startAlignmentVector.MultiplyBy(innerHorizontalAttriputes.radius));
            p1 = new Point3d(p1.X, p1.Y, surfaceLevel);
            p6 = endPoint.Add(endAlignmentVector.MultiplyBy(innerHorizontalAttriputes.radius));
            p6 = new Point3d(p6.X, p6.Y, surfaceLevel);

            p2 = startPoint.Add(startPrepAlignmentVector.MultiplyBy(innerHorizontalAttriputes.radius));
            p2 = new Point3d(p2.X, p2.Y, surfaceLevel);
            p4 = endPoint.Add(endPrepAlignmentVector.Negate().MultiplyBy(innerHorizontalAttriputes.radius));
            p4 = new Point3d(p4.X, p4.Y, surfaceLevel);

            p3 = startPoint.Add(startPrepAlignmentVector.Negate().MultiplyBy(innerHorizontalAttriputes.radius));
            p3 = new Point3d(p3.X, p3.Y, surfaceLevel);
            p5 = endPoint.Add(endPrepAlignmentVector.MultiplyBy(innerHorizontalAttriputes.radius));
            p5 = new Point3d(p5.X, p5.Y, surfaceLevel);
        }

        public void CreatePolylines(Transaction trans, Database db, Editor ed)
        {
            // convert points to 2d points
            var plane = new Plane(Point3d.Origin, Vector3d.ZAxis);
            var p12D = p1.Convert2d(plane);
            var p22D = p2.Convert2d(plane);
            var p32D = p3.Convert2d(plane);
            var p42D = p4.Convert2d(plane);
            var p52D = p5.Convert2d(plane);
            var p62D = p6.Convert2d(plane);

            // compute the bulge of the second segment
            double bulge = -Math.Tan(Math.PI / 8.0);
            var curSpace = (BlockTableRecord)trans.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
            pline = new Polyline();

            //Add verticies to polyline
            pline.AddVertexAt(0, p22D, bulge, 0.0, 0.0);
            pline.AddVertexAt(1, p12D, bulge, 0.0, 0.0);
            pline.AddVertexAt(2, p32D, 0.0, 0.0, 0.0);
            pline.AddVertexAt(3, p52D, bulge, 0.0, 0.0);
            pline.AddVertexAt(4, p62D, bulge, 0.0, 0.0);
            pline.AddVertexAt(5, p42D, 0.0, 0.0, 0.0);

            pline.Elevation = surfaceLevel;

            pline.Closed = true;

            //Append the polyline
            pline.TransformBy(ed.CurrentUserCoordinateSystem);
            curSpace.AppendEntity(pline);
            trans.AddNewlyCreatedDBObject(pline, true);
            trans.Commit();
        }

        public void CreateSurface(CivilDocument _civildoc, Transaction trans)
        {
            // Select a Surface style to use
            ObjectId styleId = _civildoc.Styles.SurfaceStyles[0];

            //Breakline Entities collection
            ObjectIdCollection contourEntitiesIdColl = new ObjectIdCollection();
            contourEntitiesIdColl.Add(pline.ObjectId);

            if (styleId != null && contourEntitiesIdColl != null)
            {
                // Create an empty TIN Surface
                ObjectId surfaceId = TinSurface.Create("InnerHorizontal_OLS", styleId);
                surface = trans.GetObject(surfaceId, OpenMode.ForWrite) as TinSurface;
                surface.BreaklinesDefinition.AddStandardBreaklines(contourEntitiesIdColl, 1.0, 100.00, 15.0, 4.0);
            }
        }
    }
}
/*
 *                              p02                                 p04
 * 
 * 
 * 
 * 
 * 
 * 
 *              P01                                                                        p06
 * 
 * 
 * 
 *  
 * 
 *                              p03                                 p05
 */