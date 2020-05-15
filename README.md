Clipper for Grasshopper
=========

Clipper is a 2D polygon Clipper offering polygon boolean operations (clipping): Intersection, Join, Difference, XOR. Offsets for polygons and Minkowski Sum and Differences. 

*The good stuff:*

 - it accepts all types of polygons including self-intersecting ones
 - it supports multiple polygon filling rules (EvenOdd, NonZero)
 - it's very fast relative to the RhinoCommon counterparts
 - it also performs line and polygon offsetting
 - it's numerically robust
 - it's free to use in both freeware and commercial applications
 - it's open source
 - it's native .NET

*What is does not do*

 - Curves, Splines, Arcs need to be converted to polygons
 - No support for splines, nurbs
 - Only in-plane operations are supported (all geometry will be projected onto it's 2d space)
 - Circular fillets are represented as polygons

#### Installation

*For windows*: Install the clipper-[version].rhi, this will install both the rhino plugin and the grasshopper plugin.

*For Mac OSX*: Install the clipper-[version].macrhi, this will install both the rhino plugin and the grasshopper plugin

*Package manager*: Using rhino 6+'s experimental package manager/yak (from version 0.3.0): using the command TestPackageManager

*Manual installation*: Download the .zip file, and place in rhino's plugin and grasshopper component folder.

Source code is available on github, available under the open source (permissive) Boost licence, free to use for any purpose.

#### Usage in Rhino:

The commands `OffsetPolyline` and `BooleanPolyline` offer interactive commands for the clipper library.

#### Usage in Grasshopper:

Examples can be found on github in the folder [examples]

##### Offsets

- Example 1: Simple single polygon offset (Star offset)
- Example 2: Multiple polygon offsets (Urban plan)

##### Boolean operations

- Example 3: Moire effect

##### Minkowski Sums

- Example 4: Slide an object over another

#### Version

- 2020-04-20: Version 0.3.2: Added offset type selector, thanks to [Andrew Heumann]
- 2020-04-01: Version 0.3.1: Released new version that fixes compatiblity with ShapeDiver
- 2019-10-09: Version 0.3.0: Released new version for yak and macrhi. Improved placement in grasshopper menu's
- 2019-07-22: Version 0.2.3: Updated to clipper version 6.4.2
- 2017-02-15: Version 0.2.2: Update to new clipper version (6 .4.0), release for Rhino WIP
- 2014-07-06: Version 0.1, first beta release

#### Credits

- [Angus Johnson] for doing 99% of all the hard and intelligent work of writing the clipper library.
- [Andrew Heumann] for kindly contributing code improvements
- Thanks goes out to the friends at [APTO] who triggered the requirements for this plugin, and to Maarten Filius for the initial testing.

#### Contact

Arend van Waart arend@studioavw.nl

[Andrew Heumann]:https://github.com/andrewheumann
[clipper]:http://www.angusj.com/delphi/clipper.php
[Angus Johnson]:http://www.angusj.com
[boost]:http://www.boost.org/LICENSE_1_0.txt
[github]:https://github.com/arendvw/clipper
[APTO]:http://www.apto.nl
[examples]:https://github.com/arendvw/clipper/tree/master/examples
