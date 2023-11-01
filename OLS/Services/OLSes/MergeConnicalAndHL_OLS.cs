using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using OLS.Services.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OLS.Services.OLSes
{
    internal class MergeConnicalAndHL_OLS
    {
        public List<Polyline> surfacePolylineList { private get; set; }
        public List<Polyline3d> tangentPolylinesList { get; set; }
        public List<Polyline> trimedPolylineList { get; set; }
        public List<LineSegment3d> tangents { private get; set; }
        public Transaction trans { private get; set; }
        public Database db { private get; set; }
        public TinSurface innerHLSurface { get; set; }


        public MergeConnicalAndHL_OLS(Transaction trans, Database db, List<Polyline> polylines)
        {
            this.surfacePolylineList = polylines;
            this.trans = trans;
            this.db = db;
            this.tangentPolylinesList = new List<Polyline3d>();
            this.trimedPolylineList = new List<Polyline>();
            this.tangents = new List<LineSegment3d>();
        }

        public void getTangent()
        {
            using (var curSpace = trans.GetObject(db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord)
            {
                for (int i = 0; i < surfacePolylineList[0].NumberOfVertices; i++)
                {
                    if (surfacePolylineList[0].GetSegmentType(i) == SegmentType.Arc)
                    {
                        //Detect all tangable curves in all polylines
                        List<CircularArc3d> circularArc3Ds = new List<CircularArc3d>();
                        foreach (var polyline in surfacePolylineList)
                        {
                            CircularArc3d circularArc2D = polyline.GetArcSegmentAt(i);
                            circularArc3Ds.Add(circularArc2D);
                        }

                        for (int n = 0; n < circularArc3Ds.Count - 1; n++)
                            //Get tangent of the curves
                            tangents.AddRange(GetTangentsToCircle.run(circularArc3Ds[n], circularArc3Ds[n + 1]));
                    }
                }
                //Draw tangent polyline
                foreach (LineSegment3d lineSegment3d in tangents)
                {
                    Polyline3d tangent = new Polyline3d();
                    curSpace.AppendEntity(tangent);
                    trans.AddNewlyCreatedDBObject(tangent, true);

                    //Add verticies to polyline
                    PolylineVertex3d polylineVertexStart = new PolylineVertex3d(lineSegment3d.StartPoint);
                    tangent.AppendVertex(polylineVertexStart);
                    PolylineVertex3d polylineVertexEnd = new PolylineVertex3d(lineSegment3d.EndPoint);
                    tangent.AppendVertex(polylineVertexEnd);

                    //Add to list of tangent polyline
                    tangentPolylinesList.Add(tangent);
                }
            }
            trans.Commit();
        }
        public void mergePolylinesWithTangents()
        {
            //Get all polyline of surface and trim them at tangent line intersection
            foreach (var polyline in surfacePolylineList)
            {
                //Tangent line intersection points
                List<Point3d> trimPoints = new List<Point3d>();
                foreach (LineSegment3d tangent1 in tangents)
                {
                    //Get intersection point between tangent and polyline
                    bool isStartPointOnPolyline1 = isPointOnPolyline(polyline, tangent1.StartPoint);
                    if (isStartPointOnPolyline1)
                        trimPoints.Add(polyline.GetClosestPointTo(tangent1.StartPoint, false));
                    bool isEndPointOnPolyline = isPointOnPolyline(polyline, tangent1.EndPoint);
                    if (isEndPointOnPolyline)
                        trimPoints.Add(polyline.GetClosestPointTo(tangent1.EndPoint, false));

                    if (trimPoints.Count == 2)
                    {
                        //Trim and add trimmed polylines to db
                        DBObjectCollection dbc = polyline.GetSplitCurves(new Point3dCollection(trimPoints.Cast<Point3d>().Distinct().OrderBy(p => polyline.GetDistAtPoint(p)).ToArray()));
                        foreach (Polyline item in dbc)
                        {
                            using (var curSpace = (BlockTableRecord)trans.GetObject(db.CurrentSpaceId, OpenMode.ForWrite))
                            {
                                curSpace.AppendEntity(item);
                                trans.AddNewlyCreatedDBObject(item, true);
                                trimedPolylineList.Add(item);
                            }
                        }
                        break;
                    }
                }
            }
        }

        public bool isPointOnPolyline(Polyline polyline, Point3d point)
        {
            Point3d closestPoint = polyline.GetClosestPointTo(point, false);
            double dist = closestPoint.DistanceTo(point);
            if (dist < 0.01)
                return true;
            return false;
        }

        public void CreateInnerHLSurface(CivilDocument _civildoc)
        {
            // Select a Surface style to use
            ObjectId styleId = _civildoc.Styles.SurfaceStyles[0];

            //Breakline Entities collection
            ObjectIdCollection contourEntitiesIdColl = new ObjectIdCollection();
            foreach (var item in tangentPolylinesList)
                contourEntitiesIdColl.Add(item.ObjectId);
            foreach (var item in trimedPolylineList)
                contourEntitiesIdColl.Add(item.ObjectId);

            if (styleId != null && contourEntitiesIdColl != null)
            {
                // Create an empty TIN Surface
                trans.Commit();
                ObjectId surfaceId = TinSurface.Create("InnerHorizontal_OLS", styleId);
                innerHLSurface = trans.GetObject(surfaceId, OpenMode.ForWrite) as TinSurface;
                innerHLSurface.BreaklinesDefinition.AddStandardBreaklines(contourEntitiesIdColl, 1.0, 100.00, 15.0, 4.0);
            }
        }
        public void createConicalSurface(CivilDocument _civildoc, TinSurface innerHL)
        {
            // Select a Surface style to use
            ObjectId styleId = _civildoc.Styles.SurfaceStyles[0];

            //Breakline Entities collection
            ObjectIdCollection contourEntitiesIdColl = new ObjectIdCollection();
            foreach (var item in tangentPolylinesList)
                contourEntitiesIdColl.Add(item.ObjectId);

            List<Polyline> filterdTrimedPolylineList = getBoundryOfPolylines(trimedPolylineList);
            foreach (var item in filterdTrimedPolylineList)
                contourEntitiesIdColl.Add(item.ObjectId);

            if (styleId != null && contourEntitiesIdColl != null)
            {
                // Create an empty TIN Surface
                trans.Commit();
                ObjectId surfaceId = TinSurface.Create("Conical_OLS", styleId);
                TinSurface surface = trans.GetObject(surfaceId, OpenMode.ForWrite) as TinSurface;
                //Past innerHorizontal surface in conical surface
                surface.PasteSurface(innerHL.ObjectId);
                //Add breakline to top conical
                surface.BreaklinesDefinition.AddStandardBreaklines(contourEntitiesIdColl, 1.0, 100.00, 15.0, 4.0);
            }
        }

        private List<Polyline> getBoundryOfPolylines(List<Polyline> trimedPolylineList)
        {
            Dictionary<double, List<Polyline>> dic = new Dictionary<double, List<Polyline>>();
            foreach (var polyline in trimedPolylineList)
                foreach (var polyline2 in trimedPolylineList)
                    if (polyline.Id != polyline2.Id)
                    {
                        double distance = 0;
                        for(double factor = 0; factor <=1; factor+=(1d/3d))
                            distance += polyline.GetPointAtDist(polyline.Length * factor).DistanceTo(polyline2.GetPointAtDist(polyline2.Length * factor));

                        if (!dic.ContainsKey(distance))
                            dic.Add(distance, new List<Polyline>() { polyline, polyline2 });
                    }

            double maxArea = dic.Max(x => x.Key);

            List<Polyline> polylines = new List<Polyline>();
            dic.TryGetValue(maxArea, out polylines);

            return polylines;
        }
    }
}