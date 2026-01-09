# List available profiles from the specs.json file
dotnet run -- --list

# Return values for provided key with distance of 12 feet from projection surface; give screen size in inches
dotnet run -- --key EPSON_EB-PU2213B__ELPX02S --distance 12 --distance-unit ft --out-unit in

# Return values for 4.5 meters from projection surface in feet with custom aspect
dotnet run -- --distance 4.5 --distance-unit m --aspect 16:10 --out-unit ft
dotnet run -- --spec specs.json --distance 120 --distance-unit in