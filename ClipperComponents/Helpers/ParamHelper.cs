using Grasshopper.Kernel.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioAvw.Clipper.Components.Helpers
{
    public static class ParamHelper
    {
        /// <summary>
        /// Iterates over an Enum type to add the named values to the integer param
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cfParam"></param>
        internal static void AddEnumOptionsToParam<T>(Param_Integer cfParam)
        {
            foreach (int cfType in Enum.GetValues(typeof(T)))
            {
                var name = Enum.GetName(typeof(T), cfType);
                cfParam.AddNamedValue(name, cfType);
            }
        }
    }
}
