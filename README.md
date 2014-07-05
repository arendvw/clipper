Clipper
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
- Only in-plane operations are supported


TODO
----
- Documentation
- Api documentation
- Rhino Plugin to offer functionality on the command line

Warning
-------
Current version is an early release. API's may change, components may change, and things may stop working in the way they do now in the future.

Version
----
2014-07-05: Version 0.1, first beta release

Installation
--------------

Copy the ClipperTools.dll and ClipperComponents.gha into the components folder of your Grasshopper plugin. Make sure they are not blocked.


Source code
-----------
Source code is available on [github]

License
-------
[Boost] licence, free to use for any purpose.

Credits
------
[Angus Johnson] for writing the clipper library.

Thanks goes out to the friends at [APTO] who triggered the requirements for this plugin, and to Maarten Filius for the initial testing. and 

Contact
-------


**Free Software, Hell Yeah!**

[clipper]:http://www.angusj.com/delphi/clipper.php
[Angus Johnson]:http://www.angusj.com
[boost]:http://www.boost.org/LICENSE_1_0.txt
[github]:https://github.com/arendvw/clipper
[apto]:http://www.apto.nl