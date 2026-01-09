using System;
using Projection.Core.Models;

namespace Projection.Core.Services
{

    public static class Geometry
    {
        /// <summary>
        /// Returns width/height/diagonal in meters given throw distance in meters, throw ratio (distance/width),
        /// and aspect ratio (w/h).
        /// </summary>
        public static GeometryResult Compute(double distanceMeters, double throwRatio, AspectRatio aspect)
        {
            if (throwRatio <= 0)
            {
                throw new ArgumentException("Throw ratio must be > 0.");
            }

            double widthM = distanceMeters / throwRatio;
            double heightM = widthM / aspect.Value;
            double diagM = Math.Sqrt(widthM * widthM + heightM * heightM);

            return new GeometryResult(widthM, heightM, diagM);
        }
    }
}
