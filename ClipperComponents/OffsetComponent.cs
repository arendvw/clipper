using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
    public class ClipperOffsetComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the C# ScriptComponent class.
        /// </summary>
        /// <exclude />
        public ClipperOffsetComponent()
          : base("Polyline Offset", "PolyOffset", "Offset a polyline curve", "Clipper", "Polyline")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        /// <param name="pManager">Use the pManager to register new parameters. pManager is never null.</param>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            // set the message for this component.
            pManager.AddCurveParameter("Polylines", "P", "A list of polylines to offset", GH_ParamAccess.list);
            pManager.AddNumberParameter("Distance", "D", "Offset Distance", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Plane", "Pln", "Plane to project the polylines to", GH_ParamAccess.item, default);


            pManager.AddNumberParameter("Tolerance", "T", "Tolerance: all floating point data beyond this precision will be discarded.", GH_ParamAccess.item, RhinoDoc.ActiveDoc.ModelAbsoluteTolerance);
            //public enum ClosedFilletType { Round, Square, Miter }
            pManager.AddIntegerParameter("ClosedFillet", "CF", "Closed fillet type (0 = Round, 1 = Square, 2 = Miter)", GH_ParamAccess.list, new List<int> { 1 });
            ////public enum OpenFilletType { Round, Square, Butt }
            pManager.AddIntegerParameter("OpenFillet", "OF", "Open fillet type (0 = Round, 1 = Square, 2 = Butt)", GH_ParamAccess.list, new List<int> { 1 });
            pManager.AddNumberParameter("Miter", "M", "If closed fillet type of Miter is selected: the maximum extension of a curve is Distance * Miter", GH_ParamAccess.item, 2);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        /// <param name="pManager">Use the pManager to register new parameters. pManager is never null.</param>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Contour", "C", "Contour polylines", GH_ParamAccess.list);
            pManager.AddCurveParameter("Holes", "H", "Holes polylines", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="da">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess da)
        {
            // SET ALL INPUT PARAMETERS
            var curves = da.FetchList<Curve>("Polylines");
            var dist = da.Fetch<double>("Distance");
            var pln = da.Fetch<Plane>("Plane");
            var tolerance = da.Fetch<double>("Tolerance");
            var openType = da.FetchList<int>("OpenFillet").Cast<Polyline3D.OpenFilletType>().ToList();
            if (openType.Count == 0)
            {
                openType = new List<Polyline3D.OpenFilletType> { Polyline3D.OpenFilletType.Square };
            }
            var miter = da.Fetch<double>("Miter");
            var closedType = da.FetchList<int>("ClosedFillet").Cast<Polyline3D.ClosedFilletType>().ToList();
            var polylines = Polyline3D.ConvertCurvesToPolyline(curves).ToList();

            if (curves.Count == 0)
            {
                return;
            }

            if (pln.Equals(default))
            {
                pln = polylines.First().FitPlane();
            }

            // set default fillet type.
            if (closedType.Count == 0)
            {
                closedType = new List<Polyline3D.ClosedFilletType> { Polyline3D.ClosedFilletType.Square };
            }

            if (curves.Count == 0)
            {
                return;
            }

            Polyline3D.Offset(polylines, openType, closedType, pln, tolerance, new List<double> { dist }, miter, 0.25, out var outside, out var holes);


            // OUTPUT LOGIC
            da.SetDataList("Contour", outside.First());
            da.SetDataList("Holes", holes.First());
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override Bitmap Icon => Icons.Icon_Offset;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("f7e8dd63-a9aa-4ee6-b292-0160ac10755b");
    }
}

