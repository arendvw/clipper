using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using Rhino;
using Rhino.Geometry;
using StudioAvw.Clipper.Components.Helpers;
using StudioAvw.Geometry;

namespace StudioAvw.Clipper.Components {
  /// <summary>
  /// Creates a CScriptComponent
  /// </summary>
  public class ClipperMinkowskiSumComponent : GH_Component {
    /// <summary>
    /// Initializes a new instance of the C# ScriptComponent class.
    /// </summary>
    public ClipperMinkowskiSumComponent()
      : base("Minkowski Sum", "MinkowskiSum", "Calculate the minkowski sum of two polygons", "Studioavw", "Polyline") {
    }

    /// <summary>
    /// Registers all the input parameters for this component.
    /// </summary>
    /// <param name="pManager">Use the pManager to register new parameters. pManager is never null.</param>
    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddCurveParameter("A", "A", "The first polyline", GH_ParamAccess.item);
      pManager.AddCurveParameter("B", "B", "The second polyline", GH_ParamAccess.item);
      pManager.AddPlaneParameter("Plane", "Pln", "Plane to project the polylines to", GH_ParamAccess.item, default(Plane));
      pManager.AddNumberParameter("Tolerance", "T", "Tolerance: all floating point data beyond this precision will be discarded.", GH_ParamAccess.item, RhinoDoc.ActiveDoc.ModelAbsoluteTolerance);
    }

    /// <summary>
    /// Registers all the output parameters for this component.
    /// </summary>
    /// <param name="pManager">Use the pManager to register new parameters. pManager is never null.</param>
    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddCurveParameter("Sum", "S", "Minkowski sum placed relative to A", GH_ParamAccess.list);
      pManager.AddCurveParameter("DisplacedSum", "D", "Minkowski sum curves with displacements of B", GH_ParamAccess.list);
    }

    /// <summary>
    /// This is the method that actually does the work.
    /// </summary>
    /// <param name="da">The DA object is used to retrieve from inputs and store in outputs.</param>
    protected override void SolveInstance(IGH_DataAccess da) {
      // SET ALL INPUT PARAMETERS
      Polyline plA;
      if (!Polyline3D.ConvertCurveToPolyline(da.Fetch<Curve>("A"), out plA))
      {
        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert to polyline from curve");
        return;
      }
      Polyline plB;
      if (!Polyline3D.ConvertCurveToPolyline(da.Fetch<Curve>("B"), out plB)) {
        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert to polyline from curve");
        return;
      }

      Plane pln = da.Fetch<Plane>("Plane");
      double tolerance = da.Fetch<double>("Tolerance");
      if (pln.Equals(default(Plane)))
      {
        pln = plA.FitPlane();
      }

      List<Polyline> outCurves = ClipperLib.Clipper.MinkowskiSum(
        plA.ToPath2D(pln, tolerance), 
        plB.ToPath2D(pln, tolerance), 
        true)
        .Select(path => path.ToPolyline(pln, tolerance, true))
        .ToList();


      // OUTPUT LOGIC
      da.SetDataList("Sum", outCurves);
      da.SetDataList("DisplacedSum", outCurves);
    }

    /// <summary>
    /// Provides an Icon for the component.
    /// </summary>
    
    protected override Bitmap Icon {
      get {
        return Icons.Icon_Minkowski;
      }
    }

    /// <summary>
    /// Gets the unique ID for this component. Do not change this ID after release.
    /// </summary>
    public override Guid ComponentGuid {
      // generate guid.
      get {
        return new Guid("{9C15D429-8249-4D36-94DF-5552E21B2BA1}");
      }

    }
  }
}

