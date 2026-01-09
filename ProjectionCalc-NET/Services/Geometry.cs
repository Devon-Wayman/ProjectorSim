using ProjectionCalc_NET.Models;

namespace ProjectionCalc_NET.Services
{
    public readonly record struct GeometryResult(double WidthMeters, double HeightMeters, double DiagonalMeters);
    public static class Geometry
    {
        public static GeometryResult Compute(double distanceMeters, double throwRatio, AspectRatio aspect)
        {
            if (throwRatio <= 0) throw new ArgumentException("Throw ratio must be > 0");

            double widthMeters = distanceMeters / throwRatio;
            double heightMeters = widthMeters / aspect.Value;
            double diagonalMeters = Math.Sqrt(widthMeters * widthMeters + heightMeters * heightMeters);

            return new GeometryResult(widthMeters, heightMeters, diagonalMeters);
        }
    }
}