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


namespace StudioAvw.Clipper.Components {
  /// <summary>
  /// Creates a CScriptComponent
  /// </summary>
  public class ClipperOffsetComponent : GH_Component {
    /// <summary>
    /// Initializes a new instance of the C# ScriptComponent class.
    /// </summary>
    public ClipperOffsetComponent()
      : base("Polygon Offset", "PolyOffset", "Offset a polygon curve", "Studioavw", "Polygon") {
    }

    /// <summary>
    /// Registers all the input parameters for this component.
    /// </summary>
    protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
      // set the message for this component.
      pManager.AddCurveParameter("Polylines", "P", "A list of polylines to offset", GH_ParamAccess.list);
      pManager.AddNumberParameter("Distance", "D", "Offset Distance", GH_ParamAccess.item);
      pManager.AddPlaneParameter("Plane", "Pln", "Plane to project the polylines to", GH_ParamAccess.item, default(Plane));
    

      pManager.AddNumberParameter("Tolerance", "T", "Tolerance: all floating point data beyond this precision will be discarded.", GH_ParamAccess.item, Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance);
    //public enum ClosedFilletType { Round, Square, Miter }
      pManager.AddIntegerParameter("ClosedFillet", "CF", "Closed fillet type (0 = Round, 1 = Square, 2 = Miter)", GH_ParamAccess.list, new List<int> { 1 });
      ////public enum OpenFilletType { Round, Square, Butt }
      pManager.AddIntegerParameter("OpenFillet", "OF", "Open fillet type (0 = Round, 1 = Square, 2 = Butt)", GH_ParamAccess.list, new List<int> { 1 });
      pManager.AddNumberParameter("Miter", "M", "If closed fillet type of Miter is selected: the maximum extension of a curve is Distance * Miter", GH_ParamAccess.item, 2);
    }

    /// <summary>
    /// Registers all the output parameters for this component.
    /// </summary>
    protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {
      pManager.AddCurveParameter("Contour", "C", "Contour polylines", GH_ParamAccess.list);
      pManager.AddCurveParameter("Holes", "H", "Holes polylines", GH_ParamAccess.list);
    }

    /// <summary>
    /// This is the method that actually does the work.
    /// </summary>
    /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
    protected override void SolveInstance(IGH_DataAccess DA) {
      // SET ALL INPUT PARAMETERS
      List<Curve> curves = DA.FetchList<Curve>("Polylines");
      double dist = DA.Fetch<double>("Distance");
      Plane pln = DA.Fetch<Plane>("Plane");
      double tolerance = DA.Fetch<double>("Tolerance");
      List<Polyline3D.OpenFilletType> openType = DA.FetchList<int>("OpenFillet").Cast<Polyline3D.OpenFilletType>().ToList();
      if (openType == null || openType.Count == 0) {
        openType = new List<Polyline3D.OpenFilletType> { Polyline3D.OpenFilletType.Square };
      }
      double miter = DA.Fetch<double> ("Miter");

      List<Polyline3D.ClosedFilletType> closedType = DA.FetchList<int>("ClosedFillet").Cast<Polyline3D.ClosedFilletType>().ToList();

      IEnumerable<Polyline> polylines = Polyline3D.ConvertCurvesToPolyline(curves);
      
      if (pln.Equals(default(Plane)))
      {
        pln = polylines.First().FitPlane();
      }

      // set default fillet type.
      if (closedType == null || closedType.Count == 0) {
        closedType = new List<Polyline3D.ClosedFilletType> { Polyline3D.ClosedFilletType.Square };
      }

      if (curves.Count == 0) {
        return;
      }

      List<List<Polyline>> outside;
      List<List<Polyline>> holes;
      Polyline3D.Offset(polylines, openType, closedType, pln, tolerance, new List<double> { dist }, miter, 0.25, out outside, out holes);


      // OUTPUT LOGIC
      DA.SetDataList("Contour", outside.First());
      DA.SetDataList("Holes", holes.First());
    }

    /// ADDITIONAL CODE

    /// <summary>
    /// Provides an Icon for the component.
    /// </summary>
    /// 
    
    protected override System.Drawing.Bitmap Icon {
      get {
        return Icons.Icon_Offset;
      }
    }

    /// <summary>
    /// Gets the unique ID for this component. Do not change this ID after release.
    /// </summary>
    public override Guid ComponentGuid {
      // generate guid.
      get {
        return new Guid("f7e8dd63-a9aa-4ee6-b292-0160ac10755b");
      }

    }
  }
}

