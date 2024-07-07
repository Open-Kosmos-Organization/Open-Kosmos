using System;
using Unity.Burst;
using Unity.Mathematics;

namespace PCB.Math
{
    [BurstCompile]
    [Serializable]
    public struct SphericalCoordinateDegrees
    {
        public double azimuth;
        public double polar;
        public double radial;

        public SphericalCoordinateDegrees(double radial, double polar, double azimuth)
        {
            this.radial = radial;
            this.polar = polar;
            this.azimuth = azimuth;

            this.Normalize();
        }
        
        public SphericalCoordinateDegrees Normalized()
        {
            return this
                .ToRadians()
                .Normalized()
                .ToDegrees();
        }

        public void Normalize()
        {
            SphericalCoordinateDegrees normalized = this.Normalized();

            this.azimuth = normalized.azimuth;
            this.polar = normalized.polar;
            this.radial = normalized.radial;
        }
        
        public SphericalCoordinateRadians ToRadians()
        {
            return new SphericalCoordinateRadians
            {
                azimuth = math.radians(this.azimuth),
                polar = math.radians(this.polar),
                radial = this.radial
            };
        }
        
        public double3 ToCartesian()
        {
            return this
                .ToRadians()
                .ToCartesian();
        }

        public static SphericalCoordinateDegrees FromCartesian(double3 cartesian)
        {
            return SphericalCoordinateRadians.FromCartesian(cartesian).ToDegrees();
        }

        public SphericalCoordinateDegrees Interpolate(SphericalCoordinateDegrees other, double x)
        {
            return Interpolate(this, other, x);
        }
        
        public static SphericalCoordinateDegrees Interpolate(
            SphericalCoordinateDegrees a, 
            SphericalCoordinateDegrees b,
            double x)
        {
            return SphericalCoordinateRadians.Interpolate(a.ToRadians(), b.ToRadians(), x).ToDegrees();
        }

        public static SphericalCoordinateDegrees TriangleInterpolateCenter(
            SphericalCoordinateDegrees a,
            SphericalCoordinateDegrees b,
            SphericalCoordinateDegrees c)
        {
            return a.Interpolate(b, 0.5).Interpolate(c, 0.5);
        }

        public override string ToString()
        {
            return $"SphericalDegrees(r: {this.radial}, p: {this.polar}, a: {this.azimuth})";
        }
    }
}