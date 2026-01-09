using System;

namespace Projection.Core.Services
{

    public static class Units
    {
        public const double InchesPerMeter = 39.37007874015748;
        public const double FeetPerMeter = 3.280839895013123;

        public static double DistanceToMeters(double value, string unit)
        {
            if (value <= 0)
                throw new ArgumentException("Distance must be > 0.");

            var u = Normalize(unit);

            return u switch
            {
                "m" => value,
                "ft" => value / FeetPerMeter,
                "in" => value / InchesPerMeter,
                _ => throw new ArgumentException($"Unsupported distance unit '{unit}'. Use m, ft, or in.")
            };
        }

        public static double MetersToUnit(double meters, string unit)
        {
            var u = Normalize(unit);

            return u switch
            {
                "m" => meters,
                "ft" => meters * FeetPerMeter,
                "in" => meters * InchesPerMeter,
                _ => throw new ArgumentException($"Unsupported output unit '{unit}'. Use m, ft, or in.")
            };
        }

        static string Normalize(string unit)
        {
            if (unit == null)
                return "";

            unit = unit.Trim().ToLowerInvariant();

            switch (unit)
            {
                case "m":
                case "meter":
                case "meters":
                    return "m";

                case "ft":
                case "feet":
                case "foot":
                    return "ft";

                case "in":
                case "inch":
                case "inches":
                    return "in";

                default:
                    return unit;
            }
        }
    }
}
