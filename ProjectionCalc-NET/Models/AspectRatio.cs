using System.Globalization;

namespace ProjectionCalc_NET.Models
{
    public readonly record struct AspectRatio(double Width, double Height)
    {
        public double Value => Width / Height;

        public static AspectRatio Parse(string s)
        {
            s = (s ?? "").Trim().Replace("/", ":", StringComparison.Ordinal);

            var parts = s.Split(':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (parts.Length != 2)
            {
                throw new ArgumentException($"Invalud aspect ration '{s}'. Use 16:9 or 16:10.");
            }

            var width = ParseDoubleInvariant(parts[0], "aspect.w");
            var height = ParseDoubleInvariant(parts[1], "aspect.h");

            if (width <= 0 || height <= 0)
            {
                throw new ArgumentException("Aspect ratio values must be over 0");
            }

            return new AspectRatio(width, height);
        }

        static double ParseDoubleInvariant(string s, string name)
        {
            if (!double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var v))
            {
                throw new ArgumentException($"Invalid numerica value for {name}: '{s}'");
            }
            return v;
        }
    }
}