[CmdletBinding()]
param([string]$ApiPath)

$ErrorActionPreference = "Stop"
$ProjectRoot = Split-Path $PSScriptRoot -Parent
$BuildDirectory = Join-Path $ProjectRoot "build"
$Source = Join-Path $ProjectRoot "scripts/TruthStoryPlus.3.cs"
$Compiler = Join-Path $env:WINDIR "Microsoft.NET/Framework64/v4.0.30319/csc.exe"
if (-not (Test-Path -LiteralPath $Compiler)) {
    $Compiler = Join-Path $env:WINDIR "Microsoft.NET/Framework/v4.0.30319/csc.exe"
}
if (-not (Test-Path -LiteralPath $Compiler)) {
    throw "Install .NET Framework 4.8; its C# compiler was not found."
}
New-Item -ItemType Directory -Force -Path $BuildDirectory | Out-Null

function Invoke-Compile([string[]]$CompilerArguments) {
    & $Compiler @CompilerArguments
    if ($LASTEXITCODE -ne 0) { throw "C# compilation failed." }
}

$Common = @("/nologo", "/langversion:5", "/warn:4", "/warnaserror+", "/platform:x64")
if ($ApiPath) {
    $Api = (Resolve-Path -LiteralPath $ApiPath).Path
    Invoke-Compile ($Common + @(
        "/target:library", "/reference:$Api", "/reference:System.Windows.Forms.dll",
        "/out:$(Join-Path $BuildDirectory 'TruthStoryPlus.dll')", $Source
    ))
    Write-Host "PASS: compiled against the supplied Enhanced API DLL."
} else {
    Write-Host "API compilation skipped. Supply -ApiPath to validate against your installed Enhanced API."
}

# Hash names are extracted from the script; no external native database is shipped.
$NativeNames = [regex]::Matches([IO.File]::ReadAllText($Source), '\bHash\.([A-Z][A-Z0-9_]+)') |
    ForEach-Object { $_.Groups[1].Value } | Sort-Object -Unique
$HashFile = Join-Path $BuildDirectory "MockHashes.cs"
[IO.File]::WriteAllText($HashFile, "namespace GTA.Native { public enum Hash { " + ($NativeNames -join ",") + " } }")

foreach ($Suite in @("TruthTests", "CameraTrafficTests", "UiTests")) {
    $Executable = Join-Path $BuildDirectory ($Suite + ".exe")
    Invoke-Compile ($Common + @(
        "/target:exe", "/nowarn:0067", "/main:$Suite",
        "/reference:System.Drawing.dll", "/reference:System.Windows.Forms.dll",
        "/out:$Executable", $Source, $HashFile,
        (Join-Path $ProjectRoot "tests/MockGta.cs"),
        (Join-Path $ProjectRoot ("tests/" + $Suite + ".cs"))
    ))
    & $Executable
    if ($LASTEXITCODE -ne 0) { throw "$Suite failed." }
}
Write-Host "Offline checks passed. In-game testing is still required."
