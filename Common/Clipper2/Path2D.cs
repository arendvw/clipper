using System.Collections.Generic;
using Rhino;
using Rhino.Geometry;
using Clipper2Lib;
namespace ClipperTools.Clipper2;

/// <summary>
///   Extension methods for Path2D
/// </summary>
public static class Path2D
{
    /// <summary>
    ///   Convert a path to polyline
    /// </summary>
    /// <param name="path"></param>
    /// <param name="pln"></param>
    /// <param name="closed"></param>
    /// <returns></returns>
    public static Polyline ToPolyline(this List<Point64> path, Plane pln, bool closed)
    {
        return path.ToPolyline(pln, RhinoDoc.ActiveDoc.ModelAbsoluteTolerance, closed);
    }

    /// <summary>
    ///   Convert a 2D Path to a 3D Polyline
    /// </summary>
    /// <param name="path"></param>
    /// <param name="pln"></param>
    /// <param name="tolerance"></param>
    /// <param name="closed"></param>
    /// <returns></returns>
    public static Polyline ToPolyline(this List<Point64> path, Plane pln, double tolerance, bool closed)
    {
        var polylinepts = new List<Point3d>();
        foreach (var pt in path)
        {
            polylinepts.Add(pln.PointAt(pt.X * tolerance, pt.Y * tolerance));
        }

        if (closed && path.Count > 0)
        {
            polylinepts.Add(polylinepts[0]);
        }
        var pl = new Polyline(polylinepts);
        return pl;
    }
}