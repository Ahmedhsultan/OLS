using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLS.Services.Util
{
    internal static class GetTangentsToCircle
    {
        public static List<LineSegment3d> run(this CircularArc3d circle, CircularArc3d other)
        {
            // check if circles lies on the same plane
            Vector3d normal = circle.Normal;
            Plane plane = new Plane(Point3d.Origin, normal);
            double elev1 = circle.Center.TransformBy(Matrix3d.WorldToPlane(plane)).Z;
            double elev2 = other.Center.TransformBy(Matrix3d.WorldToPlane(plane)).Z;
            if (!(normal.IsParallelTo(other.Normal) && Math.Abs(elev1 - elev2) < Tolerance.Global.EqualPoint))
                throw new Autodesk.AutoCAD.Runtime.Exception(Autodesk.AutoCAD.Runtime.ErrorStatus.NonCoplanarGeometry);

            List<LineSegment3d> result = new List<LineSegment3d>();

            // check if a circle is not inside the other
            double dist = circle.Center.DistanceTo(other.Center);
            if (dist <= Math.Abs(circle.Radius - other.Radius))
                return result;

            CircularArc3d tmp1, tmp2;
            Point3d center;
            Point3d[] inters;
            Vector3d vec, vec1, vec2;

            // external tangents
            if (Math.Round(circle.Radius,2) == Math.Round(other.Radius, 2))
            {
                center = circle.Center;
                normal = circle.Normal;
                vec = other.Center - center;
                Line3d perp = new Line3d(center, vec.CrossProduct(normal));
                inters = circle.IntersectWith(perp);
                if (inters != null)
                    foreach(Point3d point in inters)
                        result.Add(new LineSegment3d(point, point + vec));
            }
            else
            {
                if (circle.Radius < other.Radius)
                {
                    tmp1 = circle;
                    circle = other;
                    other = tmp1;
                }
                center = circle.Center;
                normal = circle.Normal;
                vec = other.Center - center;
                tmp1 = new CircularArc3d(circle.Center, normal, circle.Radius - other.Radius);
                tmp2 = new CircularArc3d(center + vec / 2.0, normal, dist / 2.0);
                inters = tmp1.IntersectWith(tmp2);
                if (inters != null)
                {
                    vec1 = (inters[0] - center).GetNormal();
                    vec2 = (inters[1] - center).GetNormal();
                    result.Add(new LineSegment3d(center + vec1 * circle.Radius, other.Center + vec1 * other.Radius));
                    result.Add(new LineSegment3d(center + vec2 * circle.Radius, other.Center + vec2 * other.Radius));
                }
            }

            // crossing tangents
            if (circle.Radius + other.Radius < dist)
            {
                double ratio = (circle.Radius / (circle.Radius + other.Radius)) / 2.0;
                tmp1 = new CircularArc3d(center + vec * ratio, normal, dist * ratio);
                inters = circle.IntersectWith(tmp1);
                if (inters != null)
                {
                    vec1 = (inters[0] - center).GetNormal();
                    vec2 = (inters[1] - center).GetNormal();
                    result.Add(new LineSegment3d(center + vec1 * circle.Radius, other.Center + vec1.Negate() * other.Radius));
                    result.Add(new LineSegment3d(center + vec2 * circle.Radius, other.Center + vec2.Negate() * other.Radius));
                }
            }
            return result;
        }
    }
}
