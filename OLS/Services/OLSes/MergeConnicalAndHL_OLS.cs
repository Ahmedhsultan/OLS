using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using OLS.Services.Util;
using System.Collections.Generic;

namespace OLS.Services.OLSes
{
    internal class MergeConnicalAndHL_OLS
    {
        public List<Polyline> surfacePolylineList { get; set; }
        public List<Polyline> tangentPolylinesList { get; set; }
        public Transaction trans { get; set; }
        public Database db { get; set; }

        public MergeConnicalAndHL_OLS(Transaction trans, Database db, List<Polyline> polylines)
        {
            this.surfacePolylineList = polylines;
            this.trans = trans;
            this.db = db;
            this.tangentPolylinesList = new List<Polyline>();
        }

        public void getTangent()
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
                    {
                        //Get tangent of the curves
                        List<LineSegment3d> tangents = GetTangentsToCircle.run(circularArc3Ds[n], circularArc3Ds[n + 1]);
                        //Draw tangent polyline
                        Polyline tangent = new Polyline();
                        foreach (LineSegment3d lineSegment3d in tangents)
                        {
                            var plane = new Plane(Point3d.Origin, Vector3d.ZAxis);
                            //Add verticies to polyline
                            tangent.AddVertexAt(0, lineSegment3d.StartPoint.Convert2d(plane), 0.0, 0.0, 0.0);
                            tangent.AddVertexAt(1, lineSegment3d.EndPoint.Convert2d(plane), 0.0, 0.0, 0.0);

                            //Add to list of tangent polyline
                            tangentPolylinesList.Add(tangent);

                            //Append the polyline
                            var curSpace = (BlockTableRecord)trans.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
                            curSpace.AppendEntity(tangent);
                            trans.AddNewlyCreatedDBObject(tangent, true);
                        }
                    }
                }
            }
        }
        public void mergePolylinesWithTangents()
        {
            
        }
    }
}
