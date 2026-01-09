using System;
using System.Globalization;

namespace Projection.Core.Models
{

    public readonly struct AspectRatio
    {
        public double W { get; }
        public double H { get; }
        public double Value => W / H;

        public AspectRatio(double w, double h)
        {
            if (w <= 0 || h <= 0)
                throw new ArgumentException("Aspect ratio values must be > 0.");

            W = w;
            H = h;
        }

        /// <summary>
        /// Accepts formats like "16:10" or "16/9".
        /// </summary>
        public static AspectRatio Parse(string s)
        {
            s = (s ?? "").Trim().Replace("/", ":", StringComparison.Ordinal);
            string[] parts = s.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2)
                throw new ArgumentException($"Invalid aspect ratio '{s}'. Use like 16:10 or 16:9.");

            double w = double.Parse(parts[0].Trim(), CultureInfo.InvariantCulture);
            double h = double.Parse(parts[1].Trim(), CultureInfo.InvariantCulture);

            return new AspectRatio(w, h);
        }

        public override string ToString() => $"{W:g}:{H:g}";
    }
}