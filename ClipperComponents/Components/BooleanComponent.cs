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
      this.EvenOdd = this.EvenOdd;
      pManager.AddCurveParameter("A", "A", "The first polyline", GH_ParamAccess.list);
      pManager.AddCurveParameter("B", "B", "The first polyline", GH_ParamAccess.list);
      pManager[1].Optional = true;
      // ctIntersection, ctUnion, ctDifference, ctXor };
      pManager.AddIntegerParameter("BooleanType", "BT", "Type: (0: intersection, 1: union, 2: difference, 3: xor)", GH_ParamAccess.item, 0);

      pManager.AddPlaneParameter("Plane", "Pln", "Plane to project the polylines to", GH_ParamAccess.item, default(Plane));
      pManager.AddNumberParameter("Tolerance", "T", "Tolerance: all floating point data beyond this precision will be discarded.", GH_ParamAccess.item, Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance);
    }

    /// <summary>
    /// Registers all the output parameters for this component.
    /// </summary>
    protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {
      pManager.AddGenericParameter("Result", "R", "Simple result", GH_ParamAccess.item);
    }



    /// <summary>
    /// This is the method that actually does the work.
    /// </summary>
    /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
    protected override void SolveInstance(IGH_DataAccess DA) {
      // SET ALL INPUT PARAMETERS
      List<Curve> A = DA.FetchList<Curve>("A");
      List<Curve> B = DA.FetchList<Curve>("B");
      try {
        ClipType type = (ClipType)DA.Fetch<int>("BooleanType");
        Plane pln = DA.Fetch<Plane>("Plane");
        double tolerance = DA.Fetch<double>("Tolerance");

        // Convert the curves to polylines
        // This is a crude way of doing this.
        // Should we add some parameters for this perhaps?
        IEnumerable<Polyline> APl = Polyline3D.ConvertCurvesToPolyline(A);
        IEnumerable<Polyline> BPl = Polyline3D.ConvertCurvesToPolyline(B);

        // If we don't have a plane, let's try to create a plane from the first curve.
        if (pln.Equals(default(Plane)) || !pln.IsValid) {
          pln = APl.First().FitPlane();
        }

        List<Polyline> result = new List<Polyline>();

        // do the boolean operation
        result = Polyline3D.Boolean(type, APl, BPl, pln, tolerance, EvenOdd);

        // OUTPUT LOGIC
        DA.SetDataList("Result", result);
      } catch (Exception e) {
        this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, e.Message + ": " + e.StackTrace.ToString());
      }
    }
    /// EVEN/ODD SORTING (IMPLICIT SURFACES)
    /// SEE http://www.angusj.com/delphi/clipper/documentation/Docs/Units/ClipperLib/Types/PolyFillType.htm

    public bool m_EvenOdd = true;

    public bool EvenOdd {
      get { return m_EvenOdd; }
      set {
        m_EvenOdd = value;
        if ((m_EvenOdd)) {
          Message = "Even/Odd filling";
        } else {
          Message = "Non-Zero filling";
        }
      }
    }


    public override bool Write(GH_IO.Serialization.GH_IWriter writer) {
      // First add our own field.
      writer.SetBoolean("EvenOdd", EvenOdd);
      // Then call the base class implementation.
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader) {
      // First read our own field.
      EvenOdd = reader.GetBoolean("EvenOdd");
      // Then call the base class implementation.
      return base.Read(reader);
    }

    protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu) {
      // Append the item to the menu, making sure it's always enabled and checked if Absolute is True.
      ToolStripMenuItem item = Menu_AppendItem(menu, "EvenOdd", Menu_AbsoluteClicked, true, EvenOdd);
      // Specifically assign a tooltip text to the menu item.
      item.ToolTipText = "When checked, even/odd sorting rule is used.";
    }

    private void Menu_AbsoluteClicked(object sender, EventArgs e) {
      RecordUndoEvent("EvenOdd");
      EvenOdd = !EvenOdd;
      ExpireSolution(true);
    }

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

