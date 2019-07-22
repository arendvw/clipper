using System.Collections.Generic;
using System.Linq;
using ClipperLib;
using ClipperPlugin.Commands;
using Rhino;
using Rhino.Geometry;
using Rhino.Input.Custom;
using StudioAvw.Geometry;

namespace ClipperPlugin.Options
{
    /// <summary>
    /// Create options for the polyline boolean offset. Settings should be persistent along multiple runs of the class.
    /// </summary>
    class PolylineBooleanOptions : GetEnumPoint
    {
        /// <summary>
        /// The _curves a (First set of polylines)
        /// </summary>
        private List<Polyline> _curvesA = new List<Polyline>();
        
        /// <summary>
        /// The _curves b (Second set of polylines)
        /// </summary>
        private List<Polyline> _curvesB = new List<Polyline>();

        /// <summary>
        /// The result of the boolean operation
        /// </summary>
        private List<Polyline> _result = new List<Polyline>();

        /// <summary>
        /// The tolerance for the boolean operation. 
        /// If a point has an coordinate of 0.052851, and the tolerance is 0.001, 
        /// the point will be truncated to 0.052
        /// </summary>
        private OptionDouble _tolerance;

        /// <summary>
        /// The option that holds the value for the dynamic preview option.
        /// </summary>
        private OptionToggle _dynamicPreviewToggle;

        /// <summary>
        /// The _even odd
        /// </summary>
        private OptionToggle _evenOdd;

        /// <summary>
        /// Tolerance for double to integer conversion for the clipper library
        /// 0.014321 with a tolerance of 0.001 will become 0.014
        /// </summary>
        public double Tolerance => _tolerance.CurrentValue;

        /// <summary>
        /// Should the result be dynamically shown?
        /// </summary>
        public bool HasDynamicPreview => _dynamicPreviewToggle.CurrentValue;

        /// <summary>
        /// The CPlane to which the result should be projected
        /// </summary>
        public ProjectToCplane ProjectToCplane => (ProjectToCplane)GetEnumValue("ProjectTo");

        /// <summary>
        /// Boolean type of the current boolean operation
        /// </summary>
        public BooleanType BooleanType => (BooleanType)GetEnumValue("BooleanType");

        /// <summary>
        /// Filling rule: Even/Odd or Non-Zero?
        /// </summary>
        public bool EvenOdd => _evenOdd.CurrentValue;

        /// <summary>
        /// Initialize the values of the element
        /// </summary>
        /// <param name="absoluteTolerance">Absolute tolerance of the rhino document</param>
        /// <param name="isScripted">Are we running from a non-dynamic environment?</param>
        public void Initialize(double absoluteTolerance, bool isScripted)
        {
            EnableTransparentCommands(true);
            
            _tolerance = new OptionDouble(absoluteTolerance/10, RhinoMath.ZeroTolerance, double.MaxValue);
            _dynamicPreviewToggle = new OptionToggle(!isScripted, "Disabled","Enabled");
            _evenOdd = new OptionToggle(true, "NonZero", "evenOdd");
            AddOptionEnum("BooleanType", BooleanType.Difference);
            AddOptionEnum("ProjectTo", ProjectToCplane.CPlane);
            AddOptionDouble("Tolerance", ref _tolerance, "Tolerance");
            AddOptionToggle("DynamicPreview", ref _dynamicPreviewToggle);
            AddOptionToggle("FillingRule", ref _evenOdd);
            AcceptNothing(true);
        }

        /// <summary>
        /// Re-calculate the boolean operation
        /// </summary>
        public void CalculateBoolean()
        {
            Plane pln = RhinoDoc.ActiveDoc.Views.ActiveView.ActiveViewport.ConstructionPlane();
            // fit the CPlane to the first element
            if (ProjectToCplane.Equals(ProjectToCplane.FitToCurve))
            {
                Plane.FitPlaneToPoints(_curvesA.First(), out pln);
            }

            if (!BooleanType.Equals(BooleanType.MassUnion))
            {
                _result = Polyline3D.Boolean((ClipType) (int) BooleanType, _curvesA, _curvesB, pln, Tolerance, EvenOdd);
            }
            else
            {
                var allCurves = _curvesA.Union(_curvesB);
                var result = new List<Polyline>();
                // ReSharper disable once PossibleMultipleEnumeration
                result.Add(allCurves.First());
                // ReSharper disable once PossibleMultipleEnumeration LoopCanBeConvertedToQuery
                foreach (var curve in allCurves.Skip(1))
                {
                    result = Polyline3D.Boolean(ClipType.ctUnion, result, new List<Polyline> {curve}, pln, Tolerance, false);
                }
                _result = result;
            }
        }


        /// <summary>
        /// Gets the results.
        /// </summary>
        /// <value>
        /// The results.
        /// </value>
        public List<Polyline> Results => _result;

        /// <summary>
        /// Sets the original curves.
        /// </summary>
        /// <param name="polylinesA">The polylines a.</param>
        /// <param name="polylinesB">The polylines b.</param>
        public void SetOriginalCurves(IEnumerable<Polyline> polylinesA, IEnumerable<Polyline> polylinesB )
        {
            //EnableCurveSnapPerpBar(true, true);
            _curvesA = polylinesA.ToList();
            _curvesB = polylinesB.ToList();
            _result = new List<Polyline>();
            CalculateBoolean();
        }

        /// <summary>
        /// Draw the current offset and an arrow from the closest point on any of the polylines to the potential offset.
        /// </summary>
        /// <param name="e">Current argument for the event.</param>
        /// <example>
        ///   <code source="examples\vbnet\ex_getpointdynamicdraw.vb" lang="vbnet" />
        ///   <code source="examples\cs\ex_getpointdynamicdraw.cs" lang="cs" />
        ///   <code source="examples\py\ex_getpointdynamicdraw.py" lang="py" />
        /// </example>
        protected override void OnDynamicDraw(GetPointDrawEventArgs e)
        {
            // calculate closest point.
            if (!HasDynamicPreview)
                return;

            // enable dynamic preview;
            foreach (var polyline in _result)
            {
                e.Display.DrawPolyline(polyline, System.Drawing.Color.Blue, 2);
            }

            // draw the previous default/offset result
            base.OnDynamicDraw(e);
        }


        /// <summary>
        /// Toggle to the next boolean type.
        /// </summary>
        internal void ToggleBoolean()
        {
            int currentItem = (int) BooleanType;
            var newCurrentItem = (currentItem + 1)%4;
            
            RhinoApp.WriteLine("Showing current boolean type: {0}, Old: {1}, New: {2}", (BooleanType) newCurrentItem, currentItem, newCurrentItem);
            RhinoApp.WriteLine("New booleanType {0} {1}", BooleanType, (int) BooleanType);
            SetEnumValue("BooleanType", newCurrentItem);
            CalculateBoolean();
        }
    }
}
