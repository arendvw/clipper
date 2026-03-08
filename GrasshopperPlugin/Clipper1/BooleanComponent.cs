using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ClipperTools;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using StudioAvw.Clipper.Components.Helpers;

namespace StudioAvw.Clipper.Components.Clipper1;

public class ClipperBooleanComponent : BaseComponent
{
    public ClipperBooleanComponent()
        : base("Polyline Boolean", "PolyBoolean", "Boolean operation between 2 sets of curves using Clipper")
    {
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        EvenOdd = EvenOdd;
        pManager.AddCurveParameter("A", "A", "The first polyline", GH_ParamAccess.list);
        pManager.AddCurveParameter("B", "B", "The first polyline", GH_ParamAccess.list);
        pManager[1].Optional = true;
        var btParamIndex = pManager.AddIntegerParameter("BooleanType", "BT", "Type: (0: intersection, 1: union, 2: difference, 3: xor)", GH_ParamAccess.item, 0);
        var btParam = pManager[btParamIndex] as Param_Integer;
        ParamHelper.AddEnumOptionsToParam<GenericTools.BooleanClipType>(btParam);

        pManager.AddPlaneParameter("Plane", "Pln", "Plane to project the polylines to", GH_ParamAccess.item, default);
        pManager.AddNumberParameter("Tolerance", "T", "Tolerance: all floating point data beyond this precision will be discarded.", GH_ParamAccess.item, DocHelper.GetModelTolerance());
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddGenericParameter("Result", "R", "Simple result", GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da)
    {
        var curvesA = da.FetchList<Curve>("A");
        var curvesB = da.FetchList<Curve>("B");
        try
        {
            var boolType = (GenericTools.BooleanClipType)da.Fetch<int>("BooleanType");
            var pln = da.Fetch<Plane>("Plane");
            var tolerance = da.Fetch<double>("Tolerance");

            var polylinesA = GenericTools.ConvertCurvesToPolyline(curvesA).ToList();
            var polylinesB = GenericTools.ConvertCurvesToPolyline(curvesB).ToList();

            if (pln.Equals(default) || !pln.IsValid)
            {
                if (polylinesA.Count != 0)
                    pln = polylinesA.First().FitPlane();
                else if (polylinesB.Count != 0)
                    pln = polylinesB.First().FitPlane();
                else
                {
                    da.SetDataList("Result", new List<Polyline>());
                    return;
                }
            }

            List<Polyline> result;
            switch (Version)
            {
                case ClipperVersion.Clipper1:
                    result = ClipperTools.Clipper1.Polyline3D.Boolean(boolType, polylinesA, polylinesB, pln, tolerance, EvenOdd);
                    break;
                case ClipperVersion.Clipper2:
                    result = ClipperTools.Clipper2.Polyline3D.Boolean(boolType, polylinesA, polylinesB, pln, tolerance, EvenOdd);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            da.SetDataList("Result", result);
        }
        catch (Exception e)
        {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, e.Message + ": " + e.StackTrace);
        }
    }

    protected override Bitmap Icon => Icons.Icon_CurveBoolean;
    public override Guid ComponentGuid => new Guid("80ab1de3-d9e2-4c5c-8dc8-9edd5ff30fc9");
}