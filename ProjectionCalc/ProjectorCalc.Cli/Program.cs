using Projection.Cli.CliMods;
using Projection.Core.Models;
using Projection.Core.Services;

namespace Projection.Cli
{
    internal static class Program
    {
        private static int Main(string[] argv)
        {
            try
            {
                // however you're parsing args currently:
                Args args = ArgsParser.Parse(argv);

                SpecFile spec = SpecLoader.LoadFromPath(args.Spec);

                // If no meaningful input was provided, default to listing profiles
                if (!args.List && args.Distance == null)
                {
                    Console.WriteLine("No input provided.");
                    Console.WriteLine();
                    Console.WriteLine("Available profile keys:");
                    foreach (var k in spec.Profiles.Keys)
                    {
                        Profile p = spec.Profiles[k];
                        Console.WriteLine($"  {k}  (TR={p.Lens.ThrowRatio}, AR={p.Projector.NativeAspectRatio})");
                    }

                    Console.WriteLine();
                    Console.WriteLine("Use --key <profile> --distance <value> [options] to compute geometry.");
                    Console.WriteLine("Use --list to show profiles explicitly.");

                    return 0;
                }


                if (args.List)
                {
                    Console.WriteLine("Available profile keys:");
                    foreach (var k in spec.Profiles.Keys)
                    {
                        Profile p = spec.Profiles[k];
                        Console.WriteLine($"  {k}  (TR={p.Lens.ThrowRatio}, AR={p.Projector.NativeAspectRatio})");
                    }
                    return 0;
                }

                if (!spec.Profiles.TryGetValue(args.Key, out var profile))
                {
                    throw new InvalidOperationException($"ERROR: Unknown --key '{args.Key}'. Use --list to see valid keys.");
                }

                if (args.Distance == null)
                {
                    throw new InvalidOperationException("ERROR: --distance is required unless --list is used.");
                }

                double throwRatio = profile.Lens.ThrowRatio;
                if (throwRatio <= 0)
                {
                    throw new InvalidOperationException($"ERROR: Profile '{args.Key}' has invalid throw_ratio.");
                }

                AspectRatio aspect = string.IsNullOrWhiteSpace(args.AspectOverride)
                    ? AspectRatio.Parse(profile.Projector.NativeAspectRatio ?? "16:9")
                    : AspectRatio.Parse(args.AspectOverride);

                double distanceM = Units.DistanceToMeters(args.Distance.Value, args.DistanceUnit);

                GeometryResult geom = Geometry.Compute(distanceM, throwRatio, aspect);

                double widthOut = Units.MetersToUnit(geom.WidthMeters, args.OutUnit);
                double heightOut = Units.MetersToUnit(geom.HeightMeters, args.OutUnit);
                double diagOut = Units.MetersToUnit(geom.DiagonalMeters, args.OutUnit);

                Console.WriteLine($"Profile: {args.Key}");
                Console.WriteLine($"  Projector: {profile.Projector.Manufacturer} {profile.Projector.Model}".Trim());
                Console.WriteLine($"  Lens:      {profile.Lens.Manufacturer} {profile.Lens.Model}  (TR={throwRatio}:1)".Trim());
                Console.WriteLine($"  Aspect:    {aspect}");
                Console.WriteLine();
                Console.WriteLine("Input:");
                Console.WriteLine($"  Throw distance: {args.Distance.Value:g} {args.DistanceUnit}  ({distanceM:F4} m)");
                Console.WriteLine();
                Console.WriteLine($"Output ({args.OutUnit}):");
                Console.WriteLine($"  Image width:    {widthOut:F3} {args.OutUnit}");
                Console.WriteLine($"  Image height:   {heightOut:F3} {args.OutUnit}");
                Console.WriteLine($"  Diagonal:       {diagOut:F3} {args.OutUnit}");

                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return 1;
            }
        }
    }
}
