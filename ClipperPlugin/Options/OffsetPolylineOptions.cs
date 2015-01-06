using System.Collections.Generic;
using System.Linq;
using ClipperPlugin.Commands;
using Rhino;
using Rhino.Geometry;
using Rhino.Input.Custom;
using StudioAvw.Geometry;

namespace ClipperPlugin.Options
{
    /// <summary>
    /// A set of persistent offset options
    /// </summary>
    class OffsetPolylineOptions : GetEnumPoint
    {
        private List<Polyline> _originalCurves = new List<Polyline>();
        private List<Polyline> _offset = new List<Polyline>();
        private OptionDouble _offsetDistance;
        private OptionDouble _tolerance;
        private OptionDouble _arcTolerance;
        private OptionDouble _miter;
        private OptionToggle _dynamicPreviewToggle;

        public double OffsetDistance
        {
            get { return _offsetDistance.CurrentValue; }
        }

        public double Tolerance
        {
            get { return _tolerance.CurrentValue; }
        }

        public double ArcTolerance
        {
            get { return _arcTolerance.CurrentValue; }
        }

        public double Miter
        {
            get { return _miter.CurrentValue; }
        }

        public bool HasDynamicPreview
        {
            get { return _dynamicPreviewToggle.CurrentValue; }
        }

        public Polyline3D.OpenFilletType OpenFillet
        {
            get { return (Polyline3D.OpenFilletType) GetEnumValue("OpenFillet");  }
        }

        public Polyline3D.ClosedFilletType ClosedFillet
        {
            get { return (Polyline3D.ClosedFilletType)GetEnumValue("ClosedFillet"); }
        }

        public ProjectToCplane ProjectToCplane
        {
            get { return (ProjectToCplane)GetEnumValue("ProjectTo"); }
        }

        public Side Side
        {
            get { return (Side)GetEnumValue("Side"); }
        }

        /// <summary>
        /// Initializes the specified absolute tolerance.
        /// </summary>
        /// <param name="absoluteTolerance">The absolute tolerance.</param>
        /// <param name="isScripted">if set to <c>true</c> [is scripted].</param>
        public void Initialize(double absoluteTolerance, bool isScripted)
        {
            EnableTransparentCommands(true);
            
            _offsetDistance = new OptionDouble(10, absoluteTolerance, double.MaxValue);
            _tolerance = new OptionDouble(absoluteTolerance/10, RhinoMath.ZeroTolerance, double.MaxValue);
            _arcTolerance = new OptionDouble(0.25, RhinoMath.ZeroTolerance, double.MaxValue);
            _miter = new OptionDouble(1, RhinoMath.ZeroTolerance, double.MaxValue);
            _dynamicPreviewToggle = new OptionToggle(!isScripted, "Disabled","Enabled");
            AddOptionDouble("Distance", ref _offsetDistance, "Distance");
            AddOptionDouble("Tolerance", ref _tolerance, "Tolerance");
            AddOptionEnum("Side", Side.Both);
            AddOptionEnum("OpenFillet", Polyline3D.OpenFilletType.Butt);
            AddOptionEnum("ClosedFillet", Polyline3D.ClosedFilletType.Square);
            AddOptionDouble("Miter", ref _miter, "Miter");
            AddOptionDouble("ArcTolerance", ref _arcTolerance, "ArcTolerance");
            AddOptionEnum("ProjectTo", ProjectToCplane.CPlane);
            AddOptionToggle("DynamicPreview", ref _dynamicPreviewToggle);
            AcceptNothing(true);
        }

        /// <summary>
        /// Calculate the offsed based on it's current settings.
        /// </summary>
        public void CalculateOffset()
        {
            Plane pln = RhinoDoc.ActiveDoc.Views.ActiveView.ActiveViewport.ConstructionPlane();
            if (ProjectToCplane.Equals(ProjectToCplane.FitToCurve))
            {
                Rhino.RhinoApp.WriteLine("Using FitToCurve");
                Plane.FitPlaneToPoints(_originalCurves.First(), out pln);
            }

            var output = new List<Polyline>();
            List<List<Polyline>> contours;
            List<List<Polyline>> holes;
            Polyline3D.Offset(_originalCurves,new List<Polyline3D.OpenFilletType> { OpenFillet }, new List<Polyline3D.ClosedFilletType> { ClosedFillet },
                pln, Tolerance, new List<double> { OffsetDistance }, Miter, ArcTolerance,
                out contours, out holes);

            RhinoApp.WriteLine(Side.ToString());
            var contour = contours.FirstOrDefault();
            if (contour != null && (Side.Equals(Side.Outside) || Side.Equals(Side.Both)))
            {
                output.AddRange(contour);
            }
            var hole = holes.FirstOrDefault();
            if (hole != null && (Side.Equals(Side.Inside) || Side.Equals(Side.Both)))
            {
                output.AddRange(hole);
            }

            _offset = output;
        }


        /// <summary>
        /// Gets the resulting offset.
        /// </summary>
        /// <value>
        /// The resulting offset polylines
        /// </value>
        public List<Polyline> Results {  get { return _offset; } } 

        /// <summary>
        /// Find the closest point in the set of offset curves
        /// </summary>
        /// <param name="pt">Closest point</param>
        /// <returns></returns>
        public Point3d ClosestPoint(Point3d pt)
        {
            var closestPoint = _originalCurves.Select((pl, e) =>
            {
                var cp = pl.ClosestPoint(pt);
                return new {Point = cp, Distance = pt.DistanceTo(cp) };
            })
            .OrderBy(p => p.Distance)
            .FirstOrDefault();

            if (closestPoint == null)
            {
                return Point3d.Unset;
            }
            return closestPoint.Point;
        }

        /// <summary>
        /// Set a list of curves to offset
        /// </summary>
        /// <param name="polylines"></param>
        public void SetOriginalCurves(IEnumerable<Polyline> polylines)
        {
            //EnableCurveSnapPerpBar(true, true);
            _originalCurves = polylines.ToList();
            _offset = new List<Polyline>();
            CalculateOffset();
        }

        /// <summary>
        /// Draw the current offset and an arrow from the closest point on any of the polylines to the potential offset.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDynamicDraw(GetPointDrawEventArgs e)
        {
            // calculate closest point.
            if (!HasDynamicPreview)
                return;

            Point3d cp = ClosestPoint(e.CurrentPoint);
            SetBasePoint(cp, true);
            Vector3d dir = e.CurrentPoint - cp;
            e.Display.DrawArrow(new Line(cp, dir), System.Drawing.Color.Brown);
            e.Display.DrawArrow(new Line(cp, -dir), System.Drawing.Color.Brown);
            // enable dynamic preview;
            foreach (var polyline in _offset)
            {
                e.Display.DrawPolyline(polyline, System.Drawing.Color.Blue, 2);
            }

            // draw the previous default/offset result
            base.OnDynamicDraw(e);
        }

        /// <summary>
        /// Updates the offset distance based on the last clicked offset point.
        /// </summary>
        internal void UpdateDistance()
        {
            _offsetDistance.CurrentValue = Point().DistanceTo(ClosestPoint(Point()));
        }
    }
}
