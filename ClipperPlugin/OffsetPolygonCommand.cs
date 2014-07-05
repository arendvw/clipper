using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using Rhino.DocObjects;

namespace ClipperPlugin {
  [System.Runtime.InteropServices.Guid("80bd6132-06c8-42d9-9579-fb15f19488ae")]
  public class OffsetPolygonCommand : Command {
    public OffsetPolygonCommand() {
      // Rhino only creates one instance of each command class defined in a
      // plug-in, so it is safe to store a refence in a static property.
      Instance = this;
    }

    ///<summary>The only instance of this command.</summary>
    public static OffsetPolygonCommand Instance {
      get;
      private set;
    }

    ///<returns>The command name as it appears on the Rhino command line.</returns>
    public override string EnglishName {
      get { return "OffsetPolygon"; }
    }

    protected override Result RunCommand(RhinoDoc doc, RunMode mode) {
      var filter = ObjectType.Curve;
      ObjRef[] obj_refs;

      var rc = RhinoGet.GetMultipleObjects("Select objects to offset", false, filter, out obj_refs);
      if (rc != Result.Success) { return rc; }
      return Result.Nothing;
    }
  }
}
