## Building a valid rhino plugin release
This plugin has several projects:
- A Rhino plugin (clipperplugin)
- A Grasshopper plugin (clippercomplonents)
- A base plugin

These projects will be built into 3 versions
1. A generic .zip version containing the plugin, base and gh dll and debug files.
2. A rhino-5 style .rhi installer, created by renaming the package to .zip
3. A rhino-6+ style yak package, using 'yak build'
4. A rhino mac style .macrhi file.

- The powershell script release.ps1 can be called from powershell, and will update the required version number in all places where it is mentioned
- Build the zip, rhi, macrhi and yak package

TODO: 
- Automatically build the .macrhp version
- Release the files on github
- Automatically push the files to food4rhino.
