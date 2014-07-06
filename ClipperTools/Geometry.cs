using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino;
using Rhino.Geometry;
using ClipperLib;

/// This file contains the glue to connect the Clipper library to RhinoCommon
/// It depends only on rhinoCommon and the clipper library (included)
namespace StudioAvw.Geometry {

  public static class PolyNodeHelper {
    // C# Generator sweetness to handle recursion
    /// <summary>
    /// flatten a polynode tree, return each item.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public static IEnumerable<PolyNode> Iterate(this PolyNode node) {
      yield return node;
      foreach (PolyNode childNode in node.Childs) {
        foreach (PolyNode childNodeItem in childNode.Iterate()) {
          yield return childNodeItem;
        }
      }
    }
  }

  /// <summary>
  /// Extension methods for a 3D polyline
  /// </summary>
  public static class Polyline3D {
    /// <summary>
    /// Get a plane from a polyline
    /// </summary>
    /// <param name="Crv"></param>
    /// <returns></returns>
    public static Plane FitPlane(this Polyline Crv) {
      Plane Pln;
      Plane.FitPlaneToPoints(Crv, out Pln);
      return Pln;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Crvs"></param>
    /// <param name="Pln"></param>
    /// <returns></returns>
    public static IEnumerable<Polyline> ConvertCurvesToPolyline(IEnumerable<Curve> Crvs) {
      foreach (Curve c in Crvs) {
        Polyline pl;
        if (ConvertCurveToPolyline(c, out pl)) {
          yield return pl;
        }
      }
    }

    public static bool ConvertCurveToPolyline(Curve c, out Polyline pl) {
      pl = new Polyline();
      if (!c.TryGetPolyline(out pl)) {
        PolylineCurve polylineCurve = c.ToPolyline(0, 0, 0.1, 0, 0, 0, 0, 0, true);
        if (polylineCurve == null) {
          //Component.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Unable to convert brep edge to polyline");
          return false;
        }
        if (!polylineCurve.TryGetPolyline(out pl)) {
          //Component.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Unable to convert brep edge to polyline - weird shizzle");
          return false;
        }
      }
      if (pl.Length == 0 || !pl.IsValid) { return false; }
      return true;
    }

    public enum OpenFilletType { Round, Square, Butt }
    public enum ClosedFilletType { Round, Square, Miter }

    /// <summary>
    /// Offset a polyline with a distance. 
    /// </summary>
    /// <param name="polyline">polyline input</param>
    /// <param name="distance">offset distance</param>
    /// <param name="outContour">Outer offsets</param>
    /// <param name="outHole">Innter offsets</param>
    public static void Offset (this Polyline polyline, double distance, out List<Polyline> outContour, out List<Polyline> outHole)
    {
      Offset(new List<Polyline> { polyline }, distance, out outContour, out outHole);
    }

    /// <summary>
    /// Offset a list of polylines with a distance
    /// </summary>
    /// <param name="polylines">Input polylines</param>
    /// <param name="distance">Distance to offset</param>
    /// <param name="outContour"></param>
    /// <param name="outHole"></param>
    public static void Offset(IEnumerable<Polyline> polylines, double distance, out List<Polyline> outContour, out List<Polyline> outHole) 
    {

      Polyline pl = polylines.First();
      Plane pln = pl.FitPlane();
      Offset(polylines, OpenFilletType.Butt, ClosedFilletType.Square, distance, pln, RhinoDoc.ActiveDoc.ModelAbsoluteTolerance, out outContour, out outHole);
    }

    public static void Offset(IEnumerable<Polyline> polylines, OpenFilletType openFilletType, ClosedFilletType closedFilletType, double distance, Plane plane, double tolerance, out List<Polyline> outContour, out List<Polyline> outHole)
    {
      List<List<Polyline>> outContours = new List<List<Polyline>> { new List<Polyline> () };
      List<List<Polyline>> outHoles = new List<List<Polyline>> { new List<Polyline>() }; ;
      Offset(polylines, new List<OpenFilletType> { openFilletType }, new List<ClosedFilletType> { closedFilletType }, plane, tolerance, new List<double> { distance }, 2, 0.25, out outContours, out outHoles);
      outContour = outContours[0];
      outHole = outHoles[0];
    }

    public static int IsInside(this Polyline polyline, Point3d Point, Plane pln, Double tolerance) {
      return ClipperLib.Clipper.PointInPolygon(Point.ToIntPoint2D(pln, tolerance), polyline.ToPath2D(pln, tolerance));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="polylines">A list of polylines</param>
    /// <param name="openFilletType">Optional: line endtype (Butt, Square, Round)</param>
    /// <param name="closedFilltetType">Optional: join type: Round, Miter (uses miter parameter) or Square</param>
    /// <param name="plane">Plane to project the polylines to</param>
    /// <param name="tolerance">Tolerance: Cutoff point. Eg. point {1.245; 9.244351; 19.3214} with precision {0.1} will be cut off to {1.2; 9.2; 19.3}. </param>
    /// <param name="distance">Distances to offset set of shapes.</param>
    /// <param name="miter">Miter deterimines how long narrow spikes can become before they are cut off: A miter setting of 2 means not longer than 2 times the offset distance. A miter of 25 will give big spikes.</param>
    /// <param name="arcTolerance"></param>
    /// <param name="outContour"></param>
    /// <param name="outHoles"></param>
    public static void Offset (IEnumerable<Polyline> polylines, List<OpenFilletType> openFilletType, List<ClosedFilletType> closedFilltetType, Plane plane, double tolerance, IEnumerable<double> distance, double miter, double arcTolerance, out List<List<Polyline>> outContour, out List<List<Polyline>> outHoles) {

      outContour = new List<List<Polyline>>();
      outHoles = new List<List<Polyline>>();
      /*
       * iEndType: How to handle open ended polygons.
       * Open				Closed
       * etOpenSquare		etClosedLine    (fill inside & outside)
       * etOpenRound			etClosedPolygon (fill outside only)
       * etOpenButt
       * 
       * See: http://www.angusj.com/delphi/clipper/documentation/Docs/Units/ClipperLib/Types/EndType.htm
       */

      /*
       * jtJoinType
       * How to fill angles of closed polygons
       * jtRound: Round
       * jtMiter: Square with variable distance
       * jtSquare: Square with fixed distance (jtMiter = 1)
       */

       ClipperOffset cOffset = new ClipperOffset(miter, arcTolerance);
       List<List<IntPoint>> paths = new List<List<IntPoint>> ();
       int i = 0;
       foreach (Polyline pl in polylines)
       {
         EndType et = EndType.etOpenButt;
         JoinType jt = JoinType.jtSquare;
         if (pl.IsClosed)
         {
           et = EndType.etClosedLine;
         } else if (openFilletType.Count != 0) {
           OpenFilletType oft = IndexOrLast(openFilletType, i);
           switch (oft)
           {
             case OpenFilletType.Butt:
               et = EndType.etOpenButt;
               break;
             case OpenFilletType.Round:
               et = EndType.etOpenRound;
               break;
             case OpenFilletType.Square:
               et = EndType.etOpenSquare;
               break;
           }
         } else {
           et = EndType.etOpenButt;
         }

         if (closedFilltetType.Count != 0)
         {
           ClosedFilletType cft = IndexOrLast(closedFilltetType, i);
           switch (cft)
           {
             case ClosedFilletType.Miter:
               jt = JoinType.jtMiter;
               break;
             case ClosedFilletType.Round:
               jt = JoinType.jtRound;
               break;
             case ClosedFilletType.Square:
               jt = JoinType.jtSquare;
               break;
           }
         } else {
           jt = JoinType.jtSquare;
         }
         cOffset.AddPath(pl.ToPath2D(plane, tolerance), jt, et);
         i++;
       }

      foreach (double offsetDistance in distance)
      {
        PolyTree tree = new PolyTree();
        cOffset.Execute(ref tree, offsetDistance / tolerance);

        List<Polyline> holes = new List<Polyline>();
        List<Polyline> contours = new List<Polyline>();
        foreach (PolyNode path in tree.Iterate()) {
          if (path.Contour.Count == 0) {
            continue;
          }
          Polyline polyline = path.Contour.ToPolyline(plane, tolerance, !path.IsOpen);
          if (path.IsHole) {
            holes.Add(polyline);
          } else {
            contours.Add(polyline);
          }
        }

        outContour.Add(contours);
        outHoles.Add(holes);
      }
    }


    public static List<Polyline> Boolean (ClipType clipType, IEnumerable<Polyline> polyA, IEnumerable<Polyline> polyB, Plane pln, double tolerance, bool evenOddFilling)
  {

    Clipper clipper = new Clipper(0);
    PolyFillType polyfilltype = PolyFillType.pftEvenOdd;
    if (!evenOddFilling) { polyfilltype = PolyFillType.pftNonZero; }

    List<List<IntPoint>> PathsA = new List<List<IntPoint>>();
    List<List<IntPoint>> PathsB = new List<List<IntPoint>>();
    foreach (Polyline plA in polyA)
    {
      clipper.AddPath(plA.ToPath2D(pln, tolerance), PolyType.ptSubject, plA.IsClosed);
    }

    foreach (Polyline plB in polyB)
    {
      clipper.AddPath(plB.ToPath2D(pln, tolerance), PolyType.ptClip, true);
    }

    PolyTree OutputTree = new PolyTree();

    clipper.Execute(clipType, OutputTree, polyfilltype, polyfilltype);

    List<Polyline> Output = new List<Polyline> ();

    foreach (PolyNode pn in OutputTree.Iterate()) {
      if (pn.Contour.Count > 1) {
        Output.Add(pn.Contour.ToPolyline(pln, tolerance, !pn.IsOpen));
      }
    }

    return Output;
  }

      public static T IndexOrLast<T>(List<T> list, int index)
      {
        if (list.Count - 1 < index) {
          return list.Last();
        }
        return list[index];
      }

    /// <summary>
    /// Convert a Polyline to a Path2D.
    /// </summary>
    /// <param name="pl"></param>
    /// <param name="pln"></param>
    /// <returns></returns>
    public static List<IntPoint> ToPath2D(this Polyline pl, Plane pln) {
      double tolerance = RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;
      return pl.ToPath2D(pln, tolerance);
    }

    /// <summary>
    /// Convert a 3D Polygon to a 2D Path
    /// </summary>
    /// <param name="pl">Polyline to convert</param>
    /// <param name="pln">Plane to project the polyline to</param>
    /// <param name="tolerance">The tolerance at which the plane will be converted. A Path2D consists of integers.</param>
    /// <returns>A 2D Polyline Path</returns>
    public static List<IntPoint> ToPath2D(this Polyline pl, Plane pln, double tolerance) {
      List<IntPoint> path = new List<IntPoint>();
      foreach (Point3d pt in pl) {
        path.Add(pt.ToIntPoint2D(pln, tolerance));
      }
      if (pl.IsClosed) {
        path.RemoveAt(pl.Count - 1);
      }
      return path;
    }

    private static IntPoint ToIntPoint2D(this Point3d pt, Plane pln, double tolerance) {
      double s, t;
      pln.ClosestParameter(pt, out s, out t);
      IntPoint point = new IntPoint(s / tolerance, t / tolerance);
      return point;
    }
  }

  /// <summary>
  /// Extension methods for Path2D
  /// </summary>
  public static class Path2D {
    /// <summary>
    /// Convert a path to polyline
    /// </summary>
    /// <param name="path"></param>
    /// <param name="pln"></param>
    /// <param name="closed"></param>
    /// <returns></returns>
    public static Polyline ToPolyline(this List<IntPoint> path, Plane pln, bool closed) {
      return path.ToPolyline(pln, RhinoDoc.ActiveDoc.ModelAbsoluteTolerance, closed);
    }

    /// <summary>
    /// Convert a 2D Path to a 3D Polyline
    /// </summary>
    /// <param name="path"></param>
    /// <param name="pln"></param>
    /// <param name="tolerance"></param>
    /// <param name="closed"></param>
    /// <returns></returns>
    public static Polyline ToPolyline(this List<IntPoint> path, Plane pln, double tolerance, bool closed) {
      List<Point3d> polylinepts = new List<Point3d>();

      foreach (IntPoint pt in path) {
        polylinepts.Add(pln.PointAt(pt.X * tolerance, pt.Y * tolerance));
      }

      if (closed && path.Count > 0) {
        polylinepts.Add(polylinepts[0]);
      }
      Polyline pl = new Polyline(polylinepts);
      return pl;
    }
  }
}
