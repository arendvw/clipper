### 
### VARIABLES
###
$namePrefix = "Clipper"
$projects = ".\clippertools", ".\clippercomponents", ".\clipperplugin"
$copyToOutput = ".\clippercomponents\bin\Release\ClipperComponents.gha",
".\clippercomponents\bin\Release\ClipperComponents.pdb",
".\clipperplugin\bin\ClipperPlugin.rhp",
".\clipperplugin\bin\ClipperPlugin.pdb",
".\clippertools\bin\Release\ClipperTools.dll",
".\clippertools\bin\Release\ClipperTools.pdb"

# Where is the grasshopper assembly file?
$grasshopperAssemblyInfofile = "./clippercomponents/ClipperAssemblyInfo.cs";


## Functions
# Find MS Build
Function Find-MsBuild([int] $MaxVersion = 2019)
{
    $agentPath2019 = "$Env:programfiles (x86)\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin\msbuild.exe"
    $devPath2019 = "$Env:programfiles (x86)\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\msbuild.exe"
    $proPath2019 = "$Env:programfiles (x86)\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\msbuild.exe"
    $communityPath2019 = "$Env:programfiles (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\msbuild.exe"
    $agentPath = "$Env:programfiles (x86)\Microsoft Visual Studio\2017\BuildTools\MSBuild\15.0\Bin\msbuild.exe"
    $devPath = "$Env:programfiles (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\msbuild.exe"
    $proPath = "$Env:programfiles (x86)\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\msbuild.exe"
    $communityPath = "$Env:programfiles (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\msbuild.exe"
    $fallback2015Path = "${Env:ProgramFiles(x86)}\MSBuild\14.0\Bin\MSBuild.exe"
    $fallback2013Path = "${Env:ProgramFiles(x86)}\MSBuild\12.0\Bin\MSBuild.exe"
    $fallbackPath = "C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"

    If ((2019 -le $MaxVersion) -And (Test-Path $agentPath2019)) { return $agentPath2019 } 
    If ((2019 -le $MaxVersion) -And (Test-Path $devPath2019)) { return $devPath2019 } 
    If ((2019 -le $MaxVersion) -And (Test-Path $proPath2019)) { return $proPath2019 } 
    If ((2019 -le $MaxVersion) -And (Test-Path $communityPath2019)) { return $communityPath2019 } 
    If ((2017 -le $MaxVersion) -And (Test-Path $agentPath)) { return $agentPath } 
    If ((2017 -le $MaxVersion) -And (Test-Path $devPath)) { return $devPath } 
    If ((2017 -le $MaxVersion) -And (Test-Path $proPath)) { return $proPath } 
    If ((2017 -le $MaxVersion) -And (Test-Path $communityPath)) { return $communityPath } 
    If ((2015 -le $MaxVersion) -And (Test-Path $fallback2015Path)) { return $fallback2015Path } 
    If ((2013 -le $MaxVersion) -And (Test-Path $fallback2013Path)) { return $fallback2013Path } 
    If (Test-Path $fallbackPath) { return $fallbackPath } 
    throw "Yikes - Unable to find msbuild"
}

function GetCurrentVersion($file)
{
    $contents = [System.IO.File]::ReadAllText($file)
    $versionString = [RegEx]::Match($contents,"(?ms)^\[assembly: AssemblyVersion\(""(.*?)""\)\]\s*$")
    return $versionString.Groups[1];
}

# We ignore the 4th version number, because this is not recognized in 
# Semantic versioning. Let's just stick to the first 3.
function IncrementVersion([string]$version, [int]$major, [int]$minor, [int]$build)
{
    $v = [Version]::new($version);
    $newVersion = [Version]::new(
        $v.Major + $major,
        $v.Minor + $minor,
        $v.Build + $build,
        0
        );
    return $newVersion
}

# Update assemblyinfo.cs
function UpdateVersion($file, $version)
{
    $contents = [System.IO.File]::ReadAllText($file)
    $contents = $contents -replace "(?ms)(?<=^\[assembly: AssemblyVersion\("")(.*?)(?=""\)\])", $version
    $contents = $contents -replace "(?ms)(?<=^\[assembly: AssemblyFileVersion\("")(.*?)(?=""\)\])", $version
    [System.IO.File]::WriteAllText($file, $contents);
}

function EnsureYak($yak)
{
    if (![System.IO.File]::Exists($yak))
    {
        $url = 'http://files.mcneel.com/yak/tools/latest/yak.exe'
        Invoke-WebRequest -Uri $url -OutFile $yak
    }
}



Push-Location $PSScriptRoot;

$file = "$($projects[0])\Properties\AssemblyInfo.cs"
Write-Host $file;
$yak = "$PSScriptRoot\yak.exe"
EnsureYak($yak)
$version = GetCurrentVersion $file
$newVersion = IncrementVersion $version 0 0 1
$confirm = Read-Host -Prompt "Update to version $newversion [Y/n]?"
Write-Host $confirm
if (-Not ([string]::IsNullOrEmpty($confirm)) -and $confirm -ne 'y') {
    $newVersionInput = Read-Host -Prompt "Enter new version number"
    try {
    $newVersion = [Version]::new($newVersionInput);
    } catch {
        Write-Error "Invalid version"
        exit
    }
}

## Update the yak manifest
$manifestFile = "./manifest.yml"
$yakVersion = "$($newVersion.Major).$($newVersion.Minor).$($newVersion.Build)";
(Get-Content $manifestFile) -replace "version:\s*\S+", "version: $yakVersion" | Set-Content $manifestFile


## Update the grasshopper assemblyinfofile
(Get-Content $grasshopperAssemblyInfofile) -replace "(?<=public override string Version => "").*?(?="")", "$yakVersion" | Set-Content $grasshopperAssemblyInfofile

## Update
## *\AssemblyInfo.cs
foreach ($project in $projects) {
     UpdateVersion "$($project)\Properties\AssemblyInfo.cs" $newVersion
}

Write-Host "Building new version.."
## Build a new version
$msbuild = Find-MsBuild;
$a = & $msbuild /p:Configuration=Release

Write-Host "Packaging new version.."
# Create empty temporary folder
$temp = ".\dist"
$output = '.\output';
if (Test-Path $temp)
{
    $a = Remove-Item -path $temp -recurse 
}
$a = New-Item -Path . -Name $temp -ItemType "directory"
if (-Not (Test-Path $output))
{
    $a = New-Item -Path . -Name $output -ItemType "directory"
}

foreach ($item in $copyToOutput) {
    Copy-Item $item $temp
}

Write-Host "Packaging rhi version.."
$outputZip =  ".\$($namePrefix)-$($yakversion).zip";
$outputRhi =  ".\$($namePrefix)-$($yakversion).rhi"

Write-Host "Packaging yak version.."
Copy-Item $manifestFile $temp
Push-Location $temp
$a = & "$PSScriptRoot\yak.exe" build
Pop-Location

# Create RHI Installer (zip package)
Compress-Archive -Path "$($temp)/*" -DestinationPath $outputZip
Move-Item $outputZip $outputRhi
Pop-Location;