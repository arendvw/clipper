using System.Collections.Generic;
using System.Linq;
using Rhino;
using Rhino.Geometry;

namespace ClipperTools;

public static class GenericTools
{
    /// <summary>
    /// Get a plane from a polyline
    /// </summary>
    /// <param name="crv">The CRV.</param>
    /// <returns></returns>
    public static Plane FitPlane(this Polyline crv)
    {
        Plane.FitPlaneToPoints(crv, out var pln);
        return pln;
    }
    public static bool FitPlane(ref Plane pln, params List<Polyline>[] polylineSets)
    {
        if (!pln.Equals(default) && pln.IsValid)
            return true;

        foreach (var set in polylineSets)
        {
            if (set == null || set.Count == 0) continue;
            pln = FitPlane(set.First());
            if (pln.IsValid) return true;
        }

        return false;
    }
    /// <summary>
    /// Converts the curves to polyline.
    /// </summary>
    /// <param name="crvs">The CRVS.</param>
    /// <returns></returns>
    public static IEnumerable<Polyline> ConvertCurvesToPolyline(IEnumerable<Curve> crvs)
    {
        foreach (var c in crvs)
        {
            if (ConvertCurveToPolyline(c, out var pl))
            {
                yield return pl;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum ClosedFilletType
    {
        Round,
        Square,
        Miter
    }

    /// <summary>
    /// 
    /// </summary>
    public enum OpenFilletType
    {
        Round,
        Square,
        Butt
    }

    /// <summary>
    /// Converts the curve to polyline.
    /// </summary>
    /// <param name="c">The c.</param>
    /// <param name="pl">The pl.</param>
    /// <returns></returns>
    public static bool ConvertCurveToPolyline(Curve c, out Polyline pl)
    {
        if (c.TryGetPolyline(out pl))
        {
            return pl.IsValid && !(pl.Length < RhinoMath.ZeroTolerance);
        }
        // default options.. should perhaps do something with the document tolerance.
        var polylineCurve = c.ToPolyline(0, 0, 0.1, 0, 0, 0, 0, 0, true);
        if (polylineCurve == null)
        {
            //Component.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Unable to convert brep edge to polyline");
            return false;
        }
        if (!polylineCurve.TryGetPolyline(out pl))
        {
            //Component.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Unable to convert brep edge to polyline - weird shizzle");
            return false;
        }
        return pl.IsValid && !(pl.Length < RhinoMath.ZeroTolerance);
    }
}