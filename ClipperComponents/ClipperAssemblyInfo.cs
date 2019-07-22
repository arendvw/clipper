using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace StudioAvw.Clipper.Components
{
    public class ClipperAssemblyInfo : GH_AssemblyInfo
    {
        public override string Description => "Clipper offers reliable polygon offsets and boolean operations";

        public override Bitmap Icon => Icons.Icon_Offset;

        public override string Name => "Clipper";

        public override string Version => "0.3.0";

        public override Guid Id => new Guid("{43334b3c-4a74-7648-6ecd-38dfdf4ec67f}");

        public override string AuthorName => "Arend van Waart";

        public override string AuthorContact => "https://www.food4rhino.com/app/clipper-grasshopper-and-rhino";
    }

}
