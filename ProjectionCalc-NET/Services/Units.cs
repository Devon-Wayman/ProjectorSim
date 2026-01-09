using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectionCalc_NET.Services
{
    public class Units
    {
        public const double InchesPerMeter = 39.37007874015748;
        public const double FeetPerMeter = 3.280839895013123;

        public static double DistanceToMeters(double value, string unit)
        {
            unit = (unit ?? "").Trim().ToLowerInvariant();

            if (value <= 0) throw new ArgumentException("Distance msut be greater than 0");

            return unit switch
            {
                "m" or "meter" or "meters" => value,
                "ft" or "feet" or "foot" => value / FeetPerMeter,
                "in" or "inch" or "inches" => value / InchesPerMeter,
                _ => throw new ArgumentException($"Unsupported distance unit '{unit}'. Use m, ft or in")
            };
        }

        public static double MetersToUnit(double meters, string unit)
        {
            unit = (unit ?? "").Trim().ToLowerInvariant();

            return unit switch
            {
                "m" or "meter" or "meters" => meters,
                "ft" or "feet" or "foot" => meters * FeetPerMeter,
                "in" or "inch" or "inches" => meters * InchesPerMeter,
                _ => throw new ArgumentException($"Unsupported output unit '{unit}'. Use m, ft or in")
            };
        }
    }
}