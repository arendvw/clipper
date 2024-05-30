using System;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    public class ClipperMinkowskiSumComponent : GH_TaskCapableComponent<ClipperMinkowskiSumComponent.SolveResults>
    {
        /// <summary>
        /// Initializes a new instance of the C# ScriptComponent class.
        /// </summary>
        public ClipperMinkowskiSumComponent()
          : base("Minkowski Sum", "MinkowskiSum", "Calculate the minkowski sum of two polygons using Clipper", "Clipper", "Polyline")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        /// <param name="pManager">Use the pManager to register new parameters. pManager is never null.</param>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("A", "A", "The first polyline", GH_ParamAccess.item);
            pManager.AddCurveParameter("B", "B", "The second polyline", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Plane", "Pln", "Plane to project the polylines to", GH_ParamAccess.item, default);
            pManager.AddNumberParameter("Tolerance", "T", "Tolerance: all floating point data beyond this precision will be discarded.", GH_ParamAccess.item, DocHelper.GetModelTolerance());
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        /// <param name="pManager">Use the pManager to register new parameters. pManager is never null.</param>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Sum", "S", "Minkowski sum placed relative to A", GH_ParamAccess.list);
            pManager.AddCurveParameter("DisplacedSum", "D", "Minkowski sum curves with displacements of B", GH_ParamAccess.list);
        }

        public class SolveResults
        {
            public List<Polyline> Value { get; set; }
        }

        private static SolveResults Compute(Curve aCurve, Curve bCurve, Plane pln, double tolerance)
        {
            // SET ALL INPUT PARAMETERS
            Polyline3D.ConvertCurveToPolyline(aCurve, out var plA);
            Polyline3D.ConvertCurveToPolyline(bCurve, out var plB);

            if (pln.Equals(default))
            {
                pln = plA.FitPlane();
            }

            var outCurves = ClipperLib.Clipper.MinkowskiSum(
              plA.ToPath2D(pln, tolerance),
              plB.ToPath2D(pln, tolerance),
              true)
              .Select(path => path.ToPolyline(pln, tolerance, true))
              .ToList();

            SolveResults result = new SolveResults();
            result.Value = outCurves;
            return result;
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="data">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess data)
        {
            if (InPreSolve)
            {
                // First pass; collect input data
                var aCurve= data.Fetch<Curve>(0);
                var bCurve = data.Fetch<Curve>(1);
                var pln = data.Fetch<Plane>(2);
                var tolerance = data.Fetch<double>(3);

                // Queue up the task
                Task<SolveResults> task = Task.Run(() => Compute(aCurve, bCurve, pln, tolerance), CancelToken);
                TaskList.Add(task);
                return;
            }

            if (!GetSolveResults(data, out SolveResults result))
            {
                // Compute right here; collect input data
                var aCurve = data.Fetch<Curve>(0);
                var bCurve = data.Fetch<Curve>(1);
                var pln = data.Fetch<Plane>(2);
                var tolerance = data.Fetch<double>(3);

                // Compute results on given data
                result = Compute(aCurve, bCurve, pln, tolerance);
            }

            // Set output data
            if (result != null)
            {
                data.SetDataList("Sum", result.Value);
                data.SetDataList("DisplacedSum", result.Value);
            }
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>

        protected override Bitmap Icon => Icons.Icon_Minkowski;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("{9C15D429-8249-4D36-94DF-5552E21B2BA1}");
    }
}

