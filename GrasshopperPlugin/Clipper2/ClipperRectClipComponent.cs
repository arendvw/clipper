
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ClipperTools;
using Grasshopper.Kernel;
using Rhino.Geometry;
using StudioAvw.Clipper.Components.Helpers;

namespace StudioAvw.Clipper.Components.Clipper2;

/// <summary>
/// Fast rectangular clipping using Clipper2's O(n) RectClip algorithm.
/// </summary>
public class ClipperRectClipComponent : GH_Component
{
    public ClipperRectClipComponent()
        : base("Polyline RectClip", "PolyRectClip", "Clip polylines to a rectangle using Clipper2 (O(n) performance)", "Clipper", "Polyline")
    {
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddCurveParameter("Polylines", "P", "Polylines to clip", GH_ParamAccess.list);
        pManager.AddRectangleParameter("Rectangle", "R", "Clipping rectangle", GH_ParamAccess.item);
        pManager.AddPlaneParameter("Plane", "Pln", "Plane to project the polylines to", GH_ParamAccess.item, default);
        pManager.AddNumberParameter("Tolerance", "T", "Tolerance: all floating point data beyond this precision will be discarded.", GH_ParamAccess.item, DocHelper.GetModelTolerance());
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddCurveParameter("Result", "R", "Clipped polylines", GH_ParamAccess.list);
    }

    protected override void SolveInstance(IGH_DataAccess da)
    {
        var curves = da.FetchList<Curve>("Polylines");
        var rect = da.Fetch<Rectangle3d>("Rectangle");
        var pln = da.Fetch<Plane>("Plane");
        var tolerance = da.Fetch<double>("Tolerance");

        var polylines = GenericTools.ConvertCurvesToPolyline(curves).ToList();

        if (polylines.Count == 0)
        {
            da.SetDataList("Result", new List<Polyline>());
            return;
        }

        try
        {
            var result = ClipperTools.Clipper2.Polyline3D.RectClip(polylines, rect, tolerance);
            da.SetDataList("Result", result);
        }
        catch (Exception e)
        {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, e.Message + ": " + e.StackTrace);
        }
    }

    protected override Bitmap Icon => Icons.Icon_CurveBoolean; // TODO: add a RectClip icon
    public override Guid ComponentGuid => new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"); // TODO: generate a real GUID
}