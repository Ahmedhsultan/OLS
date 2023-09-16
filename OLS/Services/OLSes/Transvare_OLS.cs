using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using OLS.Services.Classfications.Database.Surfaces;
using System;
using System.Collections.Generic;
using OLS.Persistence.Models;

namespace OLS.Services.OLSes
{
    public class Transvare_OLS
    {
        public Polyline3d pl { get; set; }
        public Point3d p0 { get; set; }
        public Point3d p00 { get; set; }
        public Point3d p1 { get; set; }
        public Point3d p2 { get; set; }
        public Point3d p3 { get; set; }
        public Point3d p4 { get; set; }
        public Point3d p5 { get; set; }
        public Point3d p6 { get; set; }
        public List<PolylineVertex3d> polylineVertex3ds { get; set; } = new List<PolylineVertex3d>();

        public Transvare_OLS(Runway runway, TransvareAttriputes transvareAttriputes, LanddingAttriputes landdingAttriputes, InnerHorizontal_OLS innerHorizontal_OLS,
                                Point3d startPoint, Point3d endPoint, Vector3d startVector3D, Vector3d endVector3D, Vector3d prependicularVector)
        {
            //Upper Transational OLS
            p0 = startPoint.Add(startVector3D.MultiplyBy(landdingAttriputes.safeArea));
            p1 = p0.Add(prependicularVector.MultiplyBy(landdingAttriputes.innerEdge/2));

            p00 = endPoint.Add(endVector3D.MultiplyBy(landdingAttriputes.safeArea));
            p2 = p00.Add(prependicularVector.MultiplyBy(landdingAttriputes.innerEdge / 2));

            double z1 = p0.Z + (landdingAttriputes.s1 * landdingAttriputes.l1);
            double divWidth = landdingAttriputes.l1 * landdingAttriputes.divargence;

            p4 = p1.Add(startVector3D.MultiplyBy(landdingAttriputes.l1)).Add(prependicularVector.MultiplyBy(divWidth));
            p4 = new Point3d(p4.X, p4.Y, z1);
            Vector3d vector3DP4 = new Vector3d(p4.X - p1.X, p4.Y - p1.Y, p4.Z - p1.Z);
            vector3DP4 = vector3DP4.GetNormal();
            double l = (innerHorizontal_OLS.surfaceLevel - p1.Z) / vector3DP4.Z;
            p4 = p1 + vector3DP4 * l;

            z1 = p00.Z +(landdingAttriputes.s1 * landdingAttriputes.l1);
            divWidth = landdingAttriputes.l1 * landdingAttriputes.divargence;

            p3 = p2.Add(endVector3D * landdingAttriputes.l1).Add(prependicularVector * divWidth);
            p3 = new Point3d(p3.X, p3.Y, z1);
            Vector3d vector3DP3 = new Vector3d(p3.X - p2.X, p3.Y - p2.Y, p3.Z - p2.Z);
            vector3DP3 = vector3DP3.GetNormal();
            l = (innerHorizontal_OLS.surfaceLevel - p2.Z ) / vector3DP3.Z;
            p3 = p2 + vector3DP3 * l;

            double y5 = innerHorizontal_OLS.surfaceLevel - p1.Z;
            double slope5 = transvareAttriputes.slope;
            double x5 = y5 / slope5;
            p5 = p1.Add(prependicularVector.MultiplyBy(x5));
            p5 = new Point3d(p5.X,p5.Y, innerHorizontal_OLS.surfaceLevel);

            double y6 = innerHorizontal_OLS.surfaceLevel - p2.Z;
            double slope6 = transvareAttriputes.slope;
            double x6 = y6 / slope6;
            p6 = p2.Add(prependicularVector.MultiplyBy(x6));
            p6 = new Point3d(p6.X, p6.Y, innerHorizontal_OLS.surfaceLevel);
        }

        public void CreatePolylines(BlockTableRecord acBlkTblRec, Transaction trans)
        {
            //Upper Transational OLS
            pl = new Polyline3d();
            pl.SetDatabaseDefaults();
            acBlkTblRec.AppendEntity(pl);
            trans.AddNewlyCreatedDBObject(pl, true);

            PolylineVertex3d vertexP1 = new PolylineVertex3d(p1);

            List<PolylineVertex3d> polylineVertex3ds = new List<PolylineVertex3d>();
            
            
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
            //Upper Transational OLS
            // Select a Surface style to use
            ObjectId styleId = _civildoc.Styles.SurfaceStyles[0];

            //Contour Entities collection
            ObjectIdCollection contourEntitiesIdColl1 = new ObjectIdCollection();
            contourEntitiesIdColl1.Add(pl.ObjectId);

            if (styleId != null && contourEntitiesIdColl1 != null)
            {
                // Create an empty TIN Surface
                ObjectId surfaceId1 = TinSurface.Create("Transation_OLS", styleId); //Refactor Naming
                TinSurface surface1 = trans.GetObject(surfaceId1, OpenMode.ForWrite) as TinSurface;
                surface1.ContoursDefinition.AddContours(contourEntitiesIdColl1, 1.0, 100.00, 15.0, 4.0);
            }
        }

        private List<Point3d> getIntermediatePoints(Point3d startPoint, Point3d endPoint, double interval)
        {
            //calc the distance between start and end point and calc the normal of start vector from start point to end point
            double dist = startPoint.DistanceTo(endPoint);
            Vector3d startNormalVector = startPoint.GetVectorTo(endPoint).GetNormal();

            //Result list
            List<Point3d> intermediatePoints = new List<Point3d>();

            //increment the interval and calc the new point and add it to the list
            Vector3d newVector;
            Point3d endPointOfNewVector;
            for (double i = 0; i < dist; i =+interval )
            {
                newVector = startNormalVector.MultiplyBy(i);
                endPointOfNewVector = startPoint.Add(newVector);
                intermediatePoints.Add(endPointOfNewVector);
            }
            
            return intermediatePoints;
        }

        private List<Point3d> addElevationToListOfPointFromAlignmentProfile(List<Point3d> points, Alignment alignment,
            Profile profile)
        {
            List<Point3d> pointsWithElevation = new List<Point3d>();
            
            foreach (Point3d point in points)
            {
                try
                {
                    //Get elevation of this point on profile by cala the point location on alignment first
                    Point3d pointOnAlignment = alignment.GetClosestPointTo(point, false);
                    double stationOfPoint = alignment.GetDistAtPoint(pointOnAlignment);
                    double elevation = profile.ElevationAt(stationOfPoint);

                    //Get the old coordinate then add new elevation to the new point
                    Point3d newPointWithElevation = new Point3d(point.X, point.Y, elevation);
                    
                    //Add the new point to the result list
                    pointsWithElevation.Add(newPointWithElevation);
                }
                catch (Exception e) { }
            }

            return pointsWithElevation;
        }
    }
}

/*
 *  p4              p5                                  p6                  p3
 * 
 * 
 *                  p1                                  p2
 *  
 *                  p0                                  p00
 *  
 *                  p9                                  p8
 *  
 *  
 *  p8                                                                      p7
 */