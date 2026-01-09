using ProjectionCalc_NET.Cli;
using ProjectionCalc_NET.Json;
using ProjectionCalc_NET.Models;
using ProjectionCalc_NET.Services;

namespace ProjectorCalc;

internal static class Program
{
    private static int Main(string[] argv)
    {
        try
        {
            var args = ArgsParser.Parse(argv);

            var profiles = SpecLoader.LoadProfiles(args.Spec);

            if (args.List)
            {
                Console.WriteLine("Available profile keys:");
                foreach (var k in profiles.Keys.OrderBy(k => k, StringComparer.Ordinal))
                {
                    var p = profiles[k];
                    var proj = JsonUtil.GetObj(p, "projector");
                    var lens = JsonUtil.GetObj(p, "lens");

                    var tr = JsonUtil.GetString(lens, "throw_ratio") ?? "n/a";
                    var ar = JsonUtil.GetString(proj, "native_aspect_ratio") ?? "n/a";
                    Console.WriteLine($"  {k}  (TR={tr}, AR={ar})");
                }

                return 0;
            }

            if (string.IsNullOrWhiteSpace(args.Key))
                throw new InvalidOperationException("ERROR: --key is required unless --list is used.");

            if (!profiles.TryGetValue(args.Key, out var profile))
                throw new InvalidOperationException($"ERROR: Unknown --key '{args.Key}'. Use --list to see valid keys.");

            if (args.Distance is null)
                throw new InvalidOperationException("ERROR: --distance is required unless --list is used.");

            var projObj = JsonUtil.GetObj(profile, "projector");
            var lensObj = JsonUtil.GetObj(profile, "lens");

            var throwRatio = JsonUtil.GetDouble(lensObj, "throw_ratio");
            if (throwRatio <= 0)
                throw new InvalidOperationException($"ERROR: Profile '{args.Key}' has invalid throw_ratio.");

            AspectRatio aspect = !string.IsNullOrWhiteSpace(args.AspectOverride)
                ? AspectRatio.Parse(args.AspectOverride!)
                : AspectRatio.Parse(JsonUtil.GetString(projObj, "native_aspect_ratio") ?? "16:9");

            var distanceM = Units.DistanceToMeters(args.Distance.Value, args.DistanceUnit);

            var geom = Geometry.Compute(distanceM, throwRatio, aspect);

            var outUnit = args.OutUnit;
            var widthOut = Units.MetersToUnit(geom.WidthMeters, outUnit);
            var heightOut = Units.MetersToUnit(geom.HeightMeters, outUnit);
            var diagOut = Units.MetersToUnit(geom.DiagonalMeters, outUnit);

            Console.WriteLine($"Profile: {args.Key}");
            Console.WriteLine($"  Projector: {(JsonUtil.GetString(projObj, "manufacturer") ?? "")} {(JsonUtil.GetString(projObj, "model") ?? "")}".TrimEnd());
            Console.WriteLine($"  Lens:      {(JsonUtil.GetString(lensObj, "manufacturer") ?? "")} {(JsonUtil.GetString(lensObj, "model") ?? "")}  (TR={throwRatio}:1)".TrimEnd());
            Console.WriteLine($"  Aspect:    {aspect.Width:g}:{aspect.Height:g}");
            Console.WriteLine();
            Console.WriteLine("Input:");
            Console.WriteLine($"  Throw distance: {args.Distance.Value:g} {args.DistanceUnit}  ({distanceM:F4} m)");
            Console.WriteLine();
            Console.WriteLine($"Output ({outUnit}):");
            Console.WriteLine($"  Image width:    {widthOut:F3} {outUnit}");
            Console.WriteLine($"  Image height:   {heightOut:F3} {outUnit}");
            Console.WriteLine($"  Diagonal:       {diagOut:F3} {outUnit}");

            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
            return 1;
        }
    }
}
