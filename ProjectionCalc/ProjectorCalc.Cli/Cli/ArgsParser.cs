using System.Globalization;

namespace Projection.Cli.CliMods
{
    public class ArgsParser
    {
        public static Args Parse(string[] argv)
        {
            Args a = new Args();

            for (int i = 0; i < argv.Length; i++)
            {
                string tok = argv[i];

                string Next()
                {
                    if (i + 1 >= argv.Length)
                        throw new ArgumentException($"Missing value for {tok}");
                    return argv[++i];
                }

                switch (tok)
                {
                    case "--spec":
                        a.Spec = Next();
                        break;

                    case "--key":
                        a.Key = Next();
                        break;

                    case "--list":
                        a.List = true;
                        break;

                    case "--distance":
                    case "-d":
                        a.Distance = ParseDoubleInvariant(Next(), "--distance");
                        break;

                    case "--distance-unit":
                    case "--distanceunit":
                    case "--distance_unit":
                    case "-u":
                        a.DistanceUnit = Next();
                        break;

                    case "--out-unit":
                    case "--outunit":
                    case "--out_unit":
                    case "-o":
                        a.OutUnit = Next();
                        break;

                    case "--aspect":
                        a.AspectOverride = Next();
                        break;

                    default:
                        throw new ArgumentException($"Unknown argument: {tok}");
                }
            }

            return a;
        }

        private static double ParseDoubleInvariant(string s, string argName)
        {
            if (!double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var v))
            {
                throw new ArgumentException($"Invalid numeric value for {argName}: '{s}'");
            }

            return v;
        }
    }
}