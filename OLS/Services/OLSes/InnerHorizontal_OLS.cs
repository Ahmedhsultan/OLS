using Autodesk.AutoCAD.DatabaseServices;
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
        public Polyline3d pl1 { get; set; }
        public Polyline3d pl2 { get; set; }
        public Arc arc1 { get; set; }
        public Arc arc2 { get; set; }

        public InnerHorizontal_OLS(InnerHorizontalAttriputes innerHorizontalAttriputes, Point3d startPoint, Point3d endPoint ,
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

        public void CreatePolylines(BlockTableRecord acBlkTblRec, Transaction trans)
        {
            PolylineVertex3d vertexP2 = new PolylineVertex3d(p2);
            PolylineVertex3d vertexP3 = new PolylineVertex3d(p3);
            PolylineVertex3d vertexP4 = new PolylineVertex3d(p4);
            PolylineVertex3d vertexP5 = new PolylineVertex3d(p5);

            pl1 = new Polyline3d();
            pl1.SetDatabaseDefaults();
            acBlkTblRec.AppendEntity(pl1);
            trans.AddNewlyCreatedDBObject(pl1, true);

            pl1.AppendVertex(vertexP2);
            pl1.AppendVertex(vertexP4);

            pl2 = new Polyline3d();
            pl2.SetDatabaseDefaults();
            acBlkTblRec.AppendEntity(pl2);
            trans.AddNewlyCreatedDBObject(pl2, true);

            pl2.AppendVertex(vertexP3);
            pl2.AppendVertex(vertexP5);


            CircularArc3d cArc1 = new CircularArc3d(p2, p1, p3);
            double angle1 = cArc1.ReferenceVector.AngleOnPlane(new Plane(cArc1.Center, cArc1.Normal));

            arc1 =new Arc(cArc1.Center, cArc1.Normal,cArc1.Radius, cArc1.StartAngle + angle1, cArc1.EndAngle + angle1);
            cArc1.Dispose();
            acBlkTblRec.AppendEntity(arc1);
            trans.AddNewlyCreatedDBObject(arc1, true);

            CircularArc3d cArc2 = new CircularArc3d(p5, p6, p4);
            double angle2 = cArc2.ReferenceVector.AngleOnPlane(new Plane(cArc2.Center, cArc2.Normal));

            arc2 = new Arc(cArc2.Center, cArc2.Normal, cArc2.Radius, cArc2.StartAngle + angle2, cArc2.EndAngle + angle2);
            cArc2.Dispose();
            acBlkTblRec.AppendEntity(arc2);
            trans.AddNewlyCreatedDBObject(arc2, true);

            var angle = cArc1.EndAngle - cArc1.StartAngle;
            if (angle < 0)
                angle += Math.PI * 2.0;
            double bulge = Math.Tan(angle / 4.0);
            Polyline pline = new Polyline(4);
            pline.SetDatabaseDefaults();
            acBlkTblRec.AppendEntity(pline);
            trans.AddNewlyCreatedDBObject(pline, true);
            pline.AddVertexAt(0, new Point2d(p2.X, p2.Y), 0.0, 0.0, 0.0);
            pline.AddVertexAt(1, new Point2d(p1.X, p1.Y), bulge, 0.0, 0.0);
            pline.AddVertexAt(2, new Point2d(p3.X, p3.Y), 0.0, 0.0, 0.0);
            pline.Normal = cArc1.Normal;
            pline.TransformBy(Matrix3d.Displacement(cArc1.Center.GetAsVector()));
        }

        public void CreateSurface(CivilDocument _civildoc, Transaction trans)
        {
            // Select a Surface style to use
            ObjectId styleId = _civildoc.Styles.SurfaceStyles[0];

            //Breakline Entities collection
            ObjectIdCollection contourEntitiesIdColl = new ObjectIdCollection();
            contourEntitiesIdColl.Add(pl1.ObjectId);
            contourEntitiesIdColl.Add(pl2.ObjectId);
            
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