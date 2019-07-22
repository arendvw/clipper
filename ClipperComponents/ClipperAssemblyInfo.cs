using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace StudioAvw.Clipper.Components
{
    public class CurveComponentsInfo : GH_AssemblyInfo
    {
        public override string Description => "Clipper offers reliable polygon offsets and boolean operations";

        public override Bitmap Icon => Icons.Icon_Offset;

        public override string Name => "Clipper";

        public override string Version => "0.3.0";

        public override Guid Id => new Guid("{5e1ff3af-bdd0-4fc4-9467-90984f94e7cb}");

        public override string AuthorName => "Arend van Waart";

        public override string AuthorContact => "https://www.food4rhino.com/app/clipper-grasshopper-and-rhino";
    }

}
