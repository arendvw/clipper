using System;
using System.Drawing;
using Grasshopper.Kernel;
using Rhino;
using Rhino.Geometry;
using StudioAvw.Clipper.Components.Helpers;
using StudioAvw.Geometry;

namespace StudioAvw.Clipper.Components
{
    /// <summary>
    /// Creates a CScriptComponent
    /// </summary>
    public class ClipperPolygonInsideComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the C# ScriptComponent class.
        /// </summary>
        /// <exclude />
        public ClipperPolygonInsideComponent()
          : base("Polyline Containment", "PolyContain", "Tests if a point is inside a Polyline using Clipper", "Clipper", "Polyline")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        /// <param name="pManager">Use the pManager to register new parameters. pManager is never null.</param>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Polyline", "Pl", "A list of polylines to offset", GH_ParamAccess.item);
            pManager.AddPointParameter("Point", "P", "Offset Distance", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Plane", "Pln", "Plane to project the polylines to", GH_ParamAccess.item, default);
            pManager.AddNumberParameter("Tolerance", "T", "Tolerance: all floating point data beyond this precision will be discarded.", GH_ParamAccess.item, RhinoDoc.ActiveDoc.ModelAbsoluteTolerance);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("Inside", "I", "-1 if the point is on the boundary, +1 if inside, 0 if outside", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="da">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess da)
        {
            // SET ALL INPUT PARAMETERS
            if (!Polyline3D.ConvertCurveToPolyline(da.Fetch<Curve>("Polyline"), out var plA))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert to polyline from curve");
                return;
            }
            var pt = da.Fetch<Point3d>("Point");
            var pln = da.Fetch<Plane>("Plane");
            var tolerance = da.Fetch<double>("Tolerance");

            if (pln.Equals(default))
            {
                pln = plA.FitPlane();
            }

            da.SetData("Inside", plA.IsInside(pt, pln, tolerance));
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>

        protected override Bitmap Icon => Icons.Icon_PointInside;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("{889BB614-57F1-4C3B-A826-F3CA904391EE}");
    }
}

