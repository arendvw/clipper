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
  public class ClipperBooleanComponent : GH_Component {
    /// <summary>
    /// Initializes a new instance of the C# ScriptComponent class.
    /// </summary>
    public ClipperBooleanComponent()
      : base("PolylineBoolean", "PolylineBoolean", "Boolean operation between 2 sets of curves", "StudioAvw", "Polygon") {
    }

    /// <summary>
    /// Registers all the input parameters for this component.
    /// </summary>
    protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
      pManager.AddCurveParameter("A", "A", "First Set of polylines", GH_ParamAccess.list);
      pManager.AddCurveParameter("B", "B", "Second Set of polylines", GH_ParamAccess.list);

      // ctIntersection, ctUnion, ctDifference, ctXor };
      pManager.AddIntegerParameter("BooleanType", "BT", "Type: (0: intersection, 1: union, 2: difference, 3: xor)", GH_ParamAccess.item, 0);
      pManager.AddPlaneParameter("Plane", "Pln", "Plane to project polylines to", GH_ParamAccess.item, default(Plane));
      pManager.AddNumberParameter("Tolerance", "T", "Tolerance, points will be truncated to this precision", GH_ParamAccess.item, Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance/10);
    }

    /// <summary>
    /// Registers all the output parameters for this component.
    /// </summary>
    protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {
      pManager.AddGenericParameter("Result", "R", "Boolean result", GH_ParamAccess.item);
    }

    /// <summary>
    /// This is the method that actually does the work.
    /// </summary>
    /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
    protected override void SolveInstance(IGH_DataAccess DA)
        {
            // SET ALL INPUT PARAMETERS
            List<Curve> A = DA.FetchList<Curve>("A");
            List<Curve> B = DA.FetchList<Curve>("B");
            try {
            ClipType type = (ClipType) DA.Fetch<int>("BooleanType");
            Plane pln = DA.Fetch<Plane>("Plane");
            double tolerance = DA.Fetch<double>("Tolerance");

            IEnumerable<Polyline> APl = Polyline3D.ConvertCurvesToPolyline(A);
            IEnumerable<Polyline> BPl = Polyline3D.ConvertCurvesToPolyline(B);
            if (pln.Equals(default(Plane)) || !pln.IsValid)
            {
              pln = APl.First().FitPlane();
            }

            List<Polyline> result = new List<Polyline>();

              // SCRIPTCODE (may need adaptation)
              result = Polyline3D.Boolean(type, APl, BPl, pln, tolerance, false);
              // END OF SCRIPTCODE


            // OUTPUT LOGIC
            DA.SetDataList("Result", result);
            } catch (Exception e) {
              this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, e.Message + ": " + e.StackTrace.ToString());

            }
        }

    /// ADDITIONAL CODE


    protected override Bitmap Icon {
      get {
        return Icons.Icon_CurveBoolean;
      }
    }
    /// <summary>
    /// Gets the unique ID for this component. Do not change this ID after release.
    /// </summary>
    public override Guid ComponentGuid {
      // generate guid.
      get {
        return new Guid("80ab1de3-d9e2-4c5c-8dc8-9edd5ff30fc9");
      }

    }
  }
}

