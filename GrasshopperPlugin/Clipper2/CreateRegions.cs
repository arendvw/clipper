using System;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using StudioAvw.Clipper.Components.Helpers;

namespace StudioAvw.Clipper.Components.Clipper2;

public class CreateRegionsComponent : GH_Component
{
    public CreateRegionsComponent()
        : base("Create Regions", "Regions", "Find closed regions from a set of curves", "Clipper", "Curve")
    {
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddCurveParameter("Curves", "C", "Curves to find regions from (open or closed, coplanar)", GH_ParamAccess.list);
        pManager.AddPlaneParameter("Plane", "Pln", "Plane for region detection", GH_ParamAccess.item, default);
        pManager.AddNumberParameter("Tolerance", "T", "Tolerance", GH_ParamAccess.item, DocHelper.GetModelTolerance());
        pManager.AddBooleanParameter("Combine", "Cb", "Combine nested regions", GH_ParamAccess.item, false);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddCurveParameter("Contours", "C", "Outer boundaries (positive winding)", GH_ParamAccess.tree);
        pManager.AddCurveParameter("Holes", "H", "Holes (negative winding)", GH_ParamAccess.tree);
    }

    protected override void SolveInstance(IGH_DataAccess da)
    {
        var curves = da.FetchList<Curve>("Curves");
        var pln = da.Fetch<Plane>("Plane");
        var tolerance = da.Fetch<double>("Tolerance");
        var combine = da.Fetch<bool>("Combine");

        if (curves.Count == 0) return;

        if (pln.Equals(default) || !pln.IsValid)
        {
            Plane.FitPlaneToPoints(curves[0].GetBoundingBox(true).GetCorners(), out pln);
        }

        var regions = Curve.CreateBooleanRegions(curves, pln, combine, tolerance);
        if (regions == null)
        {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No regions found");
            return;
        }

        var contours = new GH_Structure<GH_Curve>();
        var holes = new GH_Structure<GH_Curve>();

        for (var i = 0; i < regions.RegionCount; i++)
        {
            var path = new GH_Path(i);
            var regionCurves = regions.RegionCurves(i);
            foreach (var crv in regionCurves)
            {
                if (crv.ClosedCurveOrientation(pln) == CurveOrientation.CounterClockwise)
                    contours.Append(new GH_Curve(crv), path);
                else
                    holes.Append(new GH_Curve(crv), path);
            }
        }

        da.SetDataTree(0, contours);
        da.SetDataTree(1, holes);
    }

    protected override Bitmap Icon => Icons.Icon_CurveBoolean; // TODO: proper icon
    public override Guid ComponentGuid => new Guid("d4e5f6a7-b8c9-0123-defa-234567890123"); // TODO: real GUID
}