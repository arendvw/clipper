using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Clipper2Lib;
using ClipperTools;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using StudioAvw.Clipper.Components.Helpers;

namespace StudioAvw.Clipper.Components.Clipper2;

/// <summary>
/// Constrained Delaunay Triangulation of polylines using Clipper2.
/// </summary>
public class ClipperTriangulateComponent : GH_Component
{
    public ClipperTriangulateComponent()
        : base("Polyline Triangulate", "PolyTriangulate", "Triangulate polylines using Clipper2 Constrained Delaunay Triangulation", "Clipper", "Polyline")
    {
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddCurveParameter("Polylines", "P", "Polylines to triangulate (outer contours and holes)", GH_ParamAccess.list);
        pManager.AddPlaneParameter("Plane", "Pln", "Plane to project the polylines to", GH_ParamAccess.item, default);
        pManager.AddNumberParameter("Tolerance", "T", "Tolerance: all floating point data beyond this precision will be discarded.", GH_ParamAccess.item, DocHelper.GetModelTolerance());

        var frParamIndex = pManager.AddIntegerParameter("FillRule", "FR", "Fill rule (0 = EvenOdd, 1 = NonZero, 2 = Positive, 3 = Negative)", GH_ParamAccess.item, 0);
        var frParam = pManager[frParamIndex] as Param_Integer;
        ParamHelper.AddEnumOptionsToParam<TriangulateFillRule>(frParam);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddMeshParameter("Mesh", "M", "Triangulated mesh", GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da)
    {
        var curves = da.FetchList<Curve>("Polylines");
        var pln = da.Fetch<Plane>("Plane");
        var tolerance = da.Fetch<double>("Tolerance");
        var fillRule = (TriangulateFillRule)da.Fetch<int>("FillRule");

        var polylines = GenericTools.ConvertCurvesToPolyline(curves).ToList();

        if (polylines.Count == 0)
        {
            return;
        }

        if (!GenericTools.FitPlane(ref pln, polylines))
        {
            return;
        }

        try
        {
            var clipperFillRule = fillRule switch
            {
                TriangulateFillRule.EvenOdd => FillRule.EvenOdd,
                TriangulateFillRule.NonZero => FillRule.NonZero,
                TriangulateFillRule.Positive => FillRule.Positive,
                TriangulateFillRule.Negative => FillRule.Negative,
                _ => throw new ArgumentOutOfRangeException()
            };

            var mesh = ClipperTools.Clipper2.Polyline3D.Triangulate(polylines, pln, tolerance, clipperFillRule);
            da.SetData("Mesh", mesh);
        }
        catch (Exception e)
        {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, e.Message + ": " + e.StackTrace);
        }
    }

    protected override Bitmap Icon => Icons.Icon_CurveBoolean; // TODO: proper icon
    public override Guid ComponentGuid => new Guid("c3d4e5f6-a7b8-9012-cdef-123456789012"); // TODO: real GUID

    public enum TriangulateFillRule
    {
        EvenOdd,
        NonZero,
        Positive,
        Negative
    }
}