$ErrorActionPreference = "Stop"

$project = "src/Projection.Cli/Projection.Cli.csproj"
$rids = @(
    "win-x64",
    "osx-x64",
    "osx-arm64"
)

foreach ($rid in $rids) {
    Write-Host "Publishing $rid..."
    dotnet publish $project `
        -c Release `
        -r $rid `
        --self-contained true `
        /p:PublishSingleFile=true `
        /p:IncludeNativeLibrariesForSelfExtract=true
}
