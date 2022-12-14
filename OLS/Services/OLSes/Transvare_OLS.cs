﻿using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using OLS.Services.Classfications.Database.Surfaces;
using System.Collections.Generic;
using System.Linq;

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

        public Transvare_OLS(TransvareAttriputes transvareAttriputes, LanddingAttriputes landdingAttriputes,InnerHorizontal_OLS innerHorizontal_OLS,
                                Point3d startPoint, Point3d endPoint, Vector3d startVector3D, Vector3d endVector3D, Vector3d prependicularVector)
        {
            //Upper Transational OLS
            p0 = startPoint + (startVector3D * landdingAttriputes.safeArea);
            p1 = p0 + (prependicularVector * (landdingAttriputes.innerEdge / 2));

            p00 = endPoint + (endVector3D * landdingAttriputes.safeArea);
            p2 = p00 + (prependicularVector * (landdingAttriputes.innerEdge / 2));

            double l = (innerHorizontal_OLS.surfaceLevel- startPoint.Z)/ transvareAttriputes.slope;
            double z1 = p0.Z + (landdingAttriputes.s1 * landdingAttriputes.l1);
            double divWidth = landdingAttriputes.l1 * landdingAttriputes.divargence;

            p4 = p1 + (startVector3D * landdingAttriputes.l1) + (prependicularVector * divWidth);
            p4 = new Point3d(p4.X, p4.Y, z1);
            Vector3d vector3DP4 = new Vector3d(p4.X-p1.X, p4.Y-p1.Y, p4.Z-p1.Z);
            vector3DP4 = vector3DP4.GetNormal();
            p4 = p1 + vector3DP4 * l;

            l = (innerHorizontal_OLS.surfaceLevel - endPoint.Z) / transvareAttriputes.slope;
            z1 = p00.Z + (landdingAttriputes.s1 * landdingAttriputes.l1);
            divWidth = landdingAttriputes.l1 * landdingAttriputes.divargence;

            p3 = p2 + (endVector3D * landdingAttriputes.l1) + (prependicularVector * divWidth);
            p3 = new Point3d(p3.X, p3.Y, z1);
            Vector3d vector3DP3 = new Vector3d(p3.X - p2.X, p3.Y - p2.Y, p3.Z - p2.Z);
            vector3DP3 = vector3DP3.GetNormal();
            p3 = p2 + vector3DP3 * l;
        }

        public void CreatePolylines(BlockTableRecord acBlkTblRec, Transaction trans)
        {
            //Upper Transational OLS
            pl = new Polyline3d();
            pl.SetDatabaseDefaults();
            acBlkTblRec.AppendEntity(pl);
            trans.AddNewlyCreatedDBObject(pl, true);

            PolylineVertex3d vertexP1 = new PolylineVertex3d(p1);
            PolylineVertex3d vertexP2 = new PolylineVertex3d(p2);
            PolylineVertex3d vertexP3 = new PolylineVertex3d(p3);
            PolylineVertex3d vertexP4 = new PolylineVertex3d(p4);

            pl.AppendVertex(vertexP1);
            pl.AppendVertex(vertexP2);
            pl.AppendVertex(vertexP3);
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