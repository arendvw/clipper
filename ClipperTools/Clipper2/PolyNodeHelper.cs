using System.Collections.Generic;
using System.Linq;
using Clipper2Lib;
using clipper_lib;

namespace ClipperTools.Clipper2;
public static class PolyNodeHelper
{
   
    public static IEnumerable<PolyPath64> FlattenPolyPath(PolyPath64 node)
    {
        foreach (var child in node.OfType<PolyPath64>())
        {
            yield return child;
            foreach (var nested in FlattenPolyPath(child))
                yield return nested;
        }
    }
}