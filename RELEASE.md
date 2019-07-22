## Building a valid rhino plugin release
This plugin has several projects:
- A Rhino plugin (clipperplugin)
- A Grasshopper plugin (clippercomplonents)
- A base plugin

These projects will be built into 3 versions
1. A generic .zip version containing the plugin, base and gh dll and debug files.
2. A rhino-5 style .ghi installer, created by renaming the package to .zip
3. A rhino-6+ style yak package, using 'yak build'

