#region

using System.Collections.Generic;
using ClipperTools.Clipper1;

#endregion


// ReSharper disable once CheckNamespace


namespace StudioAvw.Geometry;

/// <summary>
/// /// This file contains the glue to connect the Clipper library to RhinoCommon
/// It depends only on rhinoCommon and the clipper library (included)
/// </summary>
public static class PolyNodeHelper
{
    // C# Generator sweetness to handle recursion
    /// <summary>
    ///   flatten a polynode tree, return each item.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public static IEnumerable<PolyNode> Iterate(this PolyNode node)
    {
        yield return node;
        foreach (var childNode in node.Childs)
        {
            foreach (var childNodeItem in childNode.Iterate())
            {
                yield return childNodeItem;
            }
        }
    }
}