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
    /// A command that allows boolean operation over two sets of curves
    /// </summary>
    [Guid("A3A0D27D-65A2-4360-AB51-610680EB3DD0")]
    [CommandStyle(Style.ScriptRunner)]
    public class BooleanPolylineCommand : Command
    {
        public BooleanPolylineCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        /// <summary>The only instance of this command.</summary>
        public static BooleanPolylineCommand Instance { get; private set; }

        /// <returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "BooleanPolyline"; }
        }

        /// <summary>
        /// PolylineBoolean options: these should be persistent along multiple runs.
        /// </summary>
        private  PolylineBooleanOptions _options;

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            ObjRef[] objRefsA;
           // RhinoApp.WriteLine("Hi there!!");
            var rc = RhinoGet.GetMultipleObjects("Select set of curves to Boolean (A)", false, ObjectType.Curve, out objRefsA);
            if (rc != Result.Success)
            {
                return rc;
            }

            var getB = new Rhino.Input.Custom.GetObject();
            getB.AcceptNothing(false);
            getB.GeometryFilter = ObjectType.Curve;
            getB.SetCommandPrompt("Select second set of curves to Boolean (B)");
            getB.DisablePreSelect(); //<-- disable pre-selection on second get object
            var result = getB.GetMultiple(1, 0);

            if (result != GetResult.Object)
            {
                return rc;
            }

            // Convert curves to polylines. Perhaps this should have more control?
            var curvesA = Polyline3D.ConvertCurvesToPolyline(objRefsA.Select(r => r.Curve()));
            var curvesB = Polyline3D.ConvertCurvesToPolyline(getB.Objects().Select(r => r.Curve()));

            if (_options == null)
            {
                _options = new PolylineBooleanOptions();
                _options.Initialize(doc.ModelAbsoluteTolerance, mode.Equals(RunMode.Scripted));
                _options.SetCommandPrompt("Select boolean type (click to toggle)");
                _options.AcceptNothing(true);
            }

            _options.EnableTransparentCommands(true);
            _options.SetOriginalCurves(curvesA, curvesB);
            
            while (true)
            {
                var res = _options.Get();
                RhinoApp.WriteLine(res.ToString());

                if (res == GetResult.Point)
                {
                    _options.ToggleBoolean();
                }

                if (res == GetResult.Cancel)
                {
                    return _options.CommandResult();
                }

                if (res == GetResult.Nothing)
                {
                    break;
                }

                if (res == GetResult.Option)
                {
                    // update the enum options
                    _options.UpdateOptions();
                    _options.CalculateBoolean();
                }
            }

            // deleselect all.
            doc.Objects.Select(doc.Objects.GetSelectedObjects(true, true).Select(obj=>obj.Id), false);
            // return the offset
            var guids = _options.Results.Select(pl => doc.Objects.AddPolyline(pl));
            doc.Objects.Select(guids);
            return Result.Success;
        }
    }

    /// <summary>
    /// Boolean options that are available. These options are used for readability, and translate directly to the ClipperLib.ClipType enum
    /// The MassUnion Option is used seperately in this document.
    /// </summary>
    enum BooleanType
    {
        Intersection,
        Union,
        Difference,
        Xor,
        MassUnion
    }
}
