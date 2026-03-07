using System;
using System.Drawing;
using System.Linq;
using ClipperTools;
using Grasshopper.Kernel;
using Rhino.Geometry;
using StudioAvw.Clipper.Components.Helpers;

namespace StudioAvw.Clipper.Components.Clipper2;

/// <summary>
/// Simplify polylines by removing vertices within epsilon distance using Clipper2.
/// </summary>
public class ClipperSimplifyComponent : GH_Component
{
    public ClipperSimplifyComponent()
        : base("Polyline Simplify", "PolySimplify", "Simplify a polyline by removing vertices within epsilon distance using Clipper2", "Clipper", "Polyline")
    {
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddCurveParameter("Polyline", "P", "Polyline to simplify", GH_ParamAccess.item);
        pManager.AddNumberParameter("Epsilon", "E", "Distance threshold for vertex removal", GH_ParamAccess.item);
        pManager.AddPlaneParameter("Plane", "Pln", "Plane to project the polyline to", GH_ParamAccess.item, default);
        pManager.AddNumberParameter("Tolerance", "T", "Tolerance: all floating point data beyond this precision will be discarded.", GH_ParamAccess.item, DocHelper.GetModelTolerance());
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddCurveParameter("Result", "R", "Simplified polyline", GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da)
    {
        var curve = da.Fetch<Curve>("Polyline");
        var epsilon = da.Fetch<double>("Epsilon");
        var pln = da.Fetch<Plane>("Plane");
        var tolerance = da.Fetch<double>("Tolerance");

        if (!GenericTools.ConvertCurveToPolyline(curve, out var polyline))
        {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert curve to polyline");
            return;
        }

        if (pln.Equals(default) || !pln.IsValid)
        {
            pln = polyline.FitPlane();
        }

        try
        {
            var result = ClipperTools.Clipper2.Polyline3D.Simplify(polyline, pln, tolerance, epsilon);
            da.SetData("Result", result);
        }
        catch (Exception e)
        {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, e.Message + ": " + e.StackTrace);
        }
    }

    protected override Bitmap Icon => Icons.Icon_CurveBoolean; // TODO: proper icon
    public override Guid ComponentGuid => new Guid("b2c3d4e5-f6a7-8901-bcde-f12345678901"); // TODO: real GUID
}