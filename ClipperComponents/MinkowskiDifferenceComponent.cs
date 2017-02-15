using System;
using System.Collections.Generic;
using ClipperLib;
using Grasshopper.Kernel;
using Rhino;
using Rhino.Geometry;
using StudioAvw.Clipper.Components.Helpers;
using StudioAvw.Geometry;

namespace StudioAvw.Clipper.Components {
  /// <summary>
  /// Creates a CScriptComponent
  /// </summary>
  public class ClipperMinkowskiDiffComponent : GH_Component {
    /// <summary>
    /// Initializes a new instance of the C# ScriptComponent class.
    /// </summary>
    public ClipperMinkowskiDiffComponent()
      : base("Minkowski Difference", "MinkowskiDiff", "Calculate the minkowski difference of two Polylines", "Studioavw", "Polyline") {
    }

    /// <summary>
    /// Registers all the input parameters for this component.
    /// </summary>
    protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
      pManager.AddCurveParameter("A", "A", "The first polyline", GH_ParamAccess.item);
      pManager.AddCurveParameter("B", "B", "The second polyline", GH_ParamAccess.item);
      pManager.AddPlaneParameter("Plane", "Pln", "Plane to project the polylines to", GH_ParamAccess.item, default(Plane));
      pManager.AddNumberParameter("Tolerance", "T", "Tolerance: all floating point data beyond this precision will be discarded.", GH_ParamAccess.item, Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance);
    }

    /// <summary>
    /// Registers all the output parameters for this component.
    /// </summary>
    
      protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {
        
          pManager.AddCurveParameter("Difference", "D", "Minkowski difference placed relative to A", GH_ParamAccess.list);
    }

    /// <summary>
    /// This is the method that actually does the work.
    /// </summary>
    /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
    protected override void SolveInstance(IGH_DataAccess DA) {
      // SET ALL INPUT PARAMETERS
      Polyline plA = default(Polyline);
      if (!Polyline3D.ConvertCurveToPolyline(DA.Fetch<Curve>("A"), out plA))
      {
        this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert to polyline from curve");
        return;
      }
      Polyline plB = default(Polyline);
      if (!Polyline3D.ConvertCurveToPolyline(DA.Fetch<Curve>("B"), out plB)) {
        this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert to polyline from curve");
        return;
      }

      Plane pln = DA.Fetch<Plane>("Plane");
      double tolerance = DA.Fetch<double>("Tolerance");
      if (pln.Equals(default(Plane)))
      {
        pln = plA.FitPlane();
      }

      Point3d ptCenter = new Box(pln, plB).Center;

      List<Polyline> outCurves = new List<Polyline>();
      List<Polyline> outDisplacedCurves = new List<Polyline>();
      foreach (List<IntPoint> path in ClipperLib.Clipper.MinkowskiDiff(plA.ToPath2D(pln, tolerance), plB.ToPath2D(pln, tolerance))) {
        Polyline plSum = path.ToPolyline(pln, tolerance, true);
        //Polyline plDisplacedSum = new Polyline(plSum);
        //plDisplacedSum.Transform(Transform.Translation(-new Vector3d(ptCenter)));
        outCurves.Add(plSum);
        //outDisplacedCurves.Add(plDisplacedSum);
      }

      


      // OUTPUT LOGIC
      DA.SetDataList("Difference", outCurves);
    }

    /// ADDITIONAL CODE



    /// <summary>
    /// Provides an Icon for the component.
    /// </summary>
    /// 
    
    protected override System.Drawing.Bitmap Icon {
      get {
        return Icons.Icon_MinkowskiDiff;
      }
    }

    /// <summary>
    /// Gets the unique ID for this component. Do not change this ID after release.
    /// </summary>
    public override Guid ComponentGuid {
      // generate guid.
      get {
        return new Guid("{C7A39F41-798C-43C3-86CD-8E06547BA4CB}");
      }

    }
  }
}

