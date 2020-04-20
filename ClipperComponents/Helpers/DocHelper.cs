using Rhino;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioAvw.Clipper.Components.Helpers
{
    public static class DocHelper
    {
        /// <summary>
        /// Safely get model tolerance -- use the active RhinoDoc where present, otherwise return a default.
        /// </summary>
        /// <returns></returns>
        public static double GetModelTolerance()
        {
            return RhinoDoc.ActiveDoc?.ModelAbsoluteTolerance ?? 0.01;
        }
    }
}
