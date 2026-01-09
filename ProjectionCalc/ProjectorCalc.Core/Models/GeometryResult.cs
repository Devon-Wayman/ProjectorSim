namespace Projection.Core.Models
{

    public readonly struct GeometryResult
    {
        public double WidthMeters { get; }
        public double HeightMeters { get; }
        public double DiagonalMeters { get; }

        public GeometryResult(double widthMeters, double heightMeters, double diagonalMeters)
        {
            WidthMeters = widthMeters;
            HeightMeters = heightMeters;
            DiagonalMeters = diagonalMeters;
        }
    }
}