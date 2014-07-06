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
  public class ClipperPolygonInsideComponent : GH_Component {
    /// <summary>
    /// Initializes a new instance of the C# ScriptComponent class.
    /// </summary>
    public ClipperPolygonInsideComponent()
      : base("Polyline Containment", "PolyContain", "Tests if a point is inside a Polyline", "StudioAvw", "Polyline") {
    }

    /// <summary>
    /// Registers all the input parameters for this component.
    /// </summary>
    protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
      pManager.AddCurveParameter("Polyline", "Pl", "A list of polylines to offset", GH_ParamAccess.item);
      pManager.AddPointParameter("Point", "P", "Offset Distance", GH_ParamAccess.item);
      pManager.AddPlaneParameter("Plane", "Pln", "Plane to project the polylines to", GH_ParamAccess.item, default(Plane));
      pManager.AddNumberParameter("Tolerance", "T", "Tolerance: all floating point data beyond this precision will be discarded.", GH_ParamAccess.item, Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance);
    }

    /// <summary>
    /// Registers all the output parameters for this component.
    /// </summary>
    protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {
      pManager.AddIntegerParameter("Inside", "I", "-1 if the point is on the boundary, +1 if inside, 0 if outside",  GH_ParamAccess.item);
    }

    /// <summary>
    /// This is the method that actually does the work.
    /// </summary>
    /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
    protected override void SolveInstance(IGH_DataAccess DA) {
      // SET ALL INPUT PARAMETERS
      Polyline plA = default(Polyline);
      if (!Polyline3D.ConvertCurveToPolyline(DA.Fetch<Curve>("Polyline"), out plA)) {
        this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert to polyline from curve");
        return;
      }
      Point3d pt = DA.Fetch<Point3d>("Point");
      Plane pln = DA.Fetch<Plane>("Plane");
      double tolerance = DA.Fetch<double>("Tolerance");
      
      if (pln.Equals(default(Plane)))
      {
        pln = plA.FitPlane();
      }

      DA.SetData("Inside", plA.IsInside(pt,pln, tolerance));
    }

    /// ADDITIONAL CODE



    /// <summary>
    /// Provides an Icon for the component.
    /// </summary>
    /// 
    
    protected override System.Drawing.Bitmap Icon {
      get {
        return Icons.Icon_PointInside;
      }
    }

    /// <summary>
    /// Gets the unique ID for this component. Do not change this ID after release.
    /// </summary>
    public override Guid ComponentGuid {
      // generate guid.
      get {
        return new Guid("{889BB614-57F1-4C3B-A826-F3CA904391EE}");
      }

    }
  }
}

