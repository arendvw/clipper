using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Windows.Forms;
using System.Drawing;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using StudioAvw.Geometry;
using StudioAvw.Tools;
using ClipperLib;


namespace StudioAvw.Clipper.Components {
  /// <summary>
  /// Creates a CScriptComponent
  /// </summary>
  public class ClipperMinkowskiSumComponent : GH_Component {
    /// <summary>
    /// Initializes a new instance of the C# ScriptComponent class.
    /// </summary>
    public ClipperMinkowskiSumComponent()
      : base("Minkowski Sum", "Minkowski", "Calculate the minkowski sum of two polygons", "Studioavw", "Polygon") {
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
      pManager.AddCurveParameter("Sum", "S", "Minkowski sum placed relative to A", GH_ParamAccess.list);
      pManager.AddCurveParameter("DisplacedSum", "D", "Minkowski sum curves with displacements of B", GH_ParamAccess.list);
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
      foreach (List<IntPoint> path in ClipperLib.Clipper.MinkowskiSum(plA.ToPath2D(pln, tolerance), plB.ToPath2D(pln, tolerance), true)) {
        Polyline plSum = path.ToPolyline(pln, tolerance, true);
        //Polyline plDisplacedSum = new Polyline(plSum);
        //plDisplacedSum.Transform(Transform.Translation(-new Vector3d(ptCenter)));
        outCurves.Add(plSum);
        //outDisplacedCurves.Add(plDisplacedSum);
      }

      


      // OUTPUT LOGIC
      DA.SetDataList("Sum", outCurves);
      DA.SetDataList("DisplacedSum", outCurves);
    }

    /// ADDITIONAL CODE



    /// <summary>
    /// Provides an Icon for the component.
    /// </summary>
    /// 
    
    protected override System.Drawing.Bitmap Icon {
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

