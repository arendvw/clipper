#region

using System.Linq;
using System.Runtime.InteropServices;
using ClipperPlugin.Options;
using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Input;
using StudioAvw.Geometry;

#endregion

namespace ClipperPlugin.Commands
{
    /// <summary>
    /// Offset a polyline, provides an interactive method to show the polyline.
    /// </summary>
    [Guid("80bd6132-06c8-42d9-9579-fb15f19488ae")]
    [CommandStyle(Style.ScriptRunner)]
    public class OffsetPolylineCommand : Command
    {
        /// <summary>
        /// Initializes a new singleton instance of the <see cref="OffsetPolylineCommand"/> class.
        /// </summary>
        public OffsetPolylineCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        /// <summary>The only instance of this command.</summary>
        public static OffsetPolylineCommand Instance { get; private set; }

        /// <returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "OffsetPolyline"; }
        }

        /// <summary>
        /// The _polyline options that are kept persistently among commands
        /// </summary>
        private OffsetPolylineOptions _polylineOptions;

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="doc">The current document.</param>
        /// <param name="mode">The command running mode.</param>
        /// <returns>
        /// The command result code.
        /// </returns>
        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            ObjRef[] objRefs;
           // RhinoApp.WriteLine("Hi there!!");
            var rc = RhinoGet.GetMultipleObjects("Select objects to offset", false, ObjectType.Curve, out objRefs);
            if (rc != Result.Success)
            {
                return rc;
            }

            var curves = objRefs.Select(r => r.Curve());
            var polylines = Polyline3D.ConvertCurvesToPolyline(curves).ToList();

            if (_polylineOptions == null)
            {
                _polylineOptions = new OffsetPolylineOptions();
                _polylineOptions.Initialize(doc.ModelAbsoluteTolerance, mode.Equals(RunMode.Scripted));
                _polylineOptions.SetCommandPrompt("Select offset distance");
                _polylineOptions.AcceptNothing(true);
            }
            _polylineOptions.EnableTransparentCommands(true);
            _polylineOptions.SetOriginalCurves(polylines);
            
            while (true)
            {
                var res = _polylineOptions.Get();
                RhinoApp.WriteLine(res.ToString());

                if (res == GetResult.Point)
                {
                    _polylineOptions.UpdateDistance();
                    _polylineOptions.CalculateOffset();
                }

                if (res == GetResult.Cancel)
                {
                    return _polylineOptions.CommandResult();
                }

                if (res == GetResult.Nothing)
                {
                    break;
                }

                if (res == GetResult.Option)
                {
                    // update the enum options
                    _polylineOptions.UpdateOptions();
                    _polylineOptions.CalculateOffset();
                }

                RhinoApp.WriteLine("Got command {0}", res);
            }

            
            // return the offset
            var guids = _polylineOptions.Results.Select(pl => doc.Objects.AddPolyline(pl));
            RhinoApp.RunScript("SelNone", true);
            doc.Objects.Select(guids);
            return Result.Success;
        }

    }


    /// <summary>
    /// Should we use the current CPlane, or project to the first curve?
    /// </summary>
    internal enum ProjectToCplane
    {
        CPlane,
        FitToCurve
    }

    /// <summary>
    /// For which side should we generate the offset?
    /// </summary>
    internal enum Side
    {
        Inside,
        Outside,
        Both
    }
}
