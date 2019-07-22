using System;
using System.Collections.Generic;
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
    public class ClipperMinkowskiDiffComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the C# ScriptComponent class.
        /// </summary>
        public ClipperMinkowskiDiffComponent()
          : base("Minkowski Difference", "MinkowskiDiff", "Calculate the minkowski difference of two Polylines using Clipper", "Clipper", "Polyline")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("A", "A", "The first polyline", GH_ParamAccess.item);
            pManager.AddCurveParameter("B", "B", "The second polyline", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Plane", "Pln", "Plane to project the polylines to", GH_ParamAccess.item, default);
            pManager.AddNumberParameter("Tolerance", "T", "Tolerance: all floating point data beyond this precision will be discarded.", GH_ParamAccess.item, RhinoDoc.ActiveDoc.ModelAbsoluteTolerance);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {

            pManager.AddCurveParameter("Difference", "D", "Minkowski difference placed relative to A", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // SET ALL INPUT PARAMETERS
            if (!Polyline3D.ConvertCurveToPolyline(DA.Fetch<Curve>("A"), out var plA))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert to polyline from curve");
                return;
            }

            if (!Polyline3D.ConvertCurveToPolyline(DA.Fetch<Curve>("B"), out var plB))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert to polyline from curve");
                return;
            }

            var pln = DA.Fetch<Plane>("Plane");
            var tolerance = DA.Fetch<double>("Tolerance");
            if (pln.Equals(default))
            {
                pln = plA.FitPlane();
            }

            var outCurves = new List<Polyline>();
            foreach (var path in ClipperLib.Clipper.MinkowskiDiff(plA.ToPath2D(pln, tolerance), plB.ToPath2D(pln, tolerance)))
            {
                var plSum = path.ToPolyline(pln, tolerance, true);
                outCurves.Add(plSum);
            }
            
            // OUTPUT LOGIC
            DA.SetDataList("Difference", outCurves);
        }
        /// <summary>
        /// Icon
        /// </summary>
        protected override Bitmap Icon => Icons.Icon_MinkowskiDiff;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("{C7A39F41-798C-43C3-86CD-8E06547BA4CB}");
    }
}

