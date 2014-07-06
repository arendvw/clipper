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

Copy the ClipperTools.dll and ClipperComponents.gha into the components folder of your Grasshopper plugin. Make sure they are not blocked. 

Source code is available on [github], available under the open source (permissive) [Boost] licence, free to use for any purpose.

#### Examples

Examples can be found on github in the folder examples

##### Offsets

 - Example 1: Simple single polygon offset (Star offset)
 - Example 2: Multiple polygon offsets (Urban plan)

##### Boolean operations

- Example 3: Moire effect

##### Minkowski Sums

 - Example 4: Slide an object over another

#### Version

2014-07-06: Version 0.1, first beta release

Current version is an early release. API's may change, components may change, and things may stop working in the way they do now in the future.

#### TODO

- Documentation
- Api documentation
- Rhino Plugin to offer functionality on the command line

#### Credits

[Angus Johnson] for writing the clipper library.

Thanks goes out to the friends at [APTO] who triggered the requirements for this plugin, and to Maarten Filius for the initial testing.

#### Contact
Arend van Waart <Arend@studioavw.nl>


[clipper]:http://www.angusj.com/delphi/clipper.php
[Angus Johnson]:http://www.angusj.com
[boost]:http://www.boost.org/LICENSE_1_0.txt
[github]:https://github.com/arendvw/clipper
[apto]:http://www.apto.nl