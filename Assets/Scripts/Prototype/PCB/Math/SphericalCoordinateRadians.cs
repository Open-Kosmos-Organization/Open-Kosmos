using System;
using Unity.Burst;
using Unity.Mathematics;

namespace PCB.Math
{
    [BurstCompile]
    [Serializable]
    public struct SphericalCoordinateRadians
    {
        public double azimuth;
        public double polar;
        public double radial;
        
        public SphericalCoordinateRadians(double radial, double polar, double azimuth)
        {
            this.radial = radial;
            this.polar = polar;
            this.azimuth = azimuth;
            
            this.Normalize();
        }
        
        public SphericalCoordinateRadians Normalized()
        {
            const double twoPi = math.PI_DBL * 2.0;
            
            double newAzimuth = this.azimuth;
            double newPolar = this.polar;
            double newRadial = this.radial;
            
            newAzimuth %= twoPi;
            newPolar %= twoPi;
            
            if (newRadial < 0.0)
            {
                newRadial = -newRadial;

                if (newPolar == 0.0)
                {
                    newPolar = math.PI_DBL;
                }
                else if (FloatUtils.Approximate(newPolar, math.PI_DBL))
                {
                    newPolar = 0.0;
                }
                else
                {
                    newAzimuth += math.PI_DBL;
                }
            }
            
            newAzimuth %= twoPi;
            newPolar %= twoPi;
            
            if (newAzimuth < 0.0)
            {
                newAzimuth = twoPi + newAzimuth;
            }
            
            newAzimuth %= twoPi;
            newPolar %= twoPi;

            if (newPolar < 0.0)
            {
                newPolar = twoPi + newPolar;
            }
            
            if (newPolar > math.PI_DBL)
            {
                newPolar = math.PI_DBL + (math.PI_DBL - newPolar);
                newAzimuth += math.PI_DBL;
            }
            
            newAzimuth %= twoPi;
            newPolar %= twoPi;

            return new SphericalCoordinateRadians
            {
                azimuth = newAzimuth,
                polar = newPolar,
                radial = newRadial
            };
        }

        public void Normalize()
        {
            SphericalCoordinateRadians normalized = this.Normalized();

            this.azimuth = normalized.azimuth;
            this.polar = normalized.polar;
            this.radial = normalized.radial;
        }
        
        public SphericalCoordinateDegrees ToDegrees()
        {
            return new SphericalCoordinateDegrees
            {
                azimuth = math.degrees(this.azimuth),
                polar = math.degrees(this.polar),
                radial = this.radial
            };
        }
        
        public double3 ToCartesian()
        {
            SphericalCoordinateRadians normalized = this.Normalized();
            
            return new double3
            {
                z = this.radial * math.sin(normalized.polar) * math.cos(normalized.azimuth),
                y = this.radial * math.cos(normalized.polar),
                x = this.radial * math.sin(normalized.polar) * math.sin(normalized.azimuth),
            };
        }

        public static SphericalCoordinateRadians FromCartesian(double3 cartesian, double zeroPolar = 0.0, double zeroAzimuth = 0.0)
        {
            if (FloatUtils.Approximate(cartesian.x, 0.0) && FloatUtils.Approximate(cartesian.y, 0.0) && FloatUtils.Approximate(cartesian.z, 0.0))
            {
                return new SphericalCoordinateRadians(0.0, zeroPolar, zeroAzimuth);
            }

            if (FloatUtils.Approximate(cartesian.x, 0.0) && FloatUtils.Approximate(cartesian.z, 0.0))
            {
                return new SphericalCoordinateRadians(cartesian.y, 0.0, zeroAzimuth);
            }
            
            double radial = math.length(cartesian);

            double polar;

            if (FloatUtils.Approximate(cartesian.y, 0.0))
            {
                polar = math.PI_DBL * 0.5;
            }
            else
            {
                double planeLength = math.length(cartesian.xz);
                
                polar = math.atan(planeLength / cartesian.y);

                if (cartesian.y < 0)
                {
                    polar += math.PI_DBL;
                }
            }

            double azimuth;

            if (FloatUtils.Approximate(cartesian.x, 0.0))
            {
                azimuth = math.PI_DBL * 0.5 - math.sign(cartesian.z) * math.PI_DBL * 0.5;
            }
            else if (FloatUtils.Approximate(cartesian.z, 0.0))
            {
                azimuth = math.sign(cartesian.x) * math.PI_DBL * 0.5;
            }
            else
            {
                azimuth = math.atan(cartesian.x / cartesian.z);

                if (cartesian.z < 0.0)
                {
                    azimuth += math.sign(cartesian.x) * math.PI_DBL;
                }
            }

            return new SphericalCoordinateRadians(radial, polar, azimuth);
        }
        
        public SphericalCoordinateRadians Interpolate(SphericalCoordinateRadians other, double x)
        {
            return Interpolate(this, other, x);
        }
        
        public static SphericalCoordinateRadians Interpolate(
            SphericalCoordinateRadians a, 
            SphericalCoordinateRadians b,
            double x)
        {
            double3 x1 = a.ToCartesian();
            double3 x2 = b.ToCartesian();

            double3 crossX1X2 = math.cross(x1, x2);
            
            double3 k = crossX1X2 / math.length(crossX1X2);

            double lengthX1 = math.length(x1);
            double lengthX2 = math.length(x2);

            double dotX1X2 = math.dot(x1, x2);

            double theta = math.acos(dotX1X2 / (lengthX1 * lengthX2));

            double phi = theta * x;

            double3 interpolatedCartesian = x1 * math.cos(phi) +
                                            math.cross(k, x1) * math.sin(phi) +
                                            k * math.dot(k, x1) * (1 - math.cos(phi));

            return SphericalCoordinateRadians.FromCartesian(interpolatedCartesian);
        }

        public static SphericalCoordinateRadians TriangleInterpolateCenter(
            SphericalCoordinateRadians a,
            SphericalCoordinateRadians b, 
            SphericalCoordinateRadians c)
        {
            return a.Interpolate(b, 0.5).Interpolate(c, 0.5);
        }

        public override string ToString()
        {
            return $"SphericalRadians(r: {this.radial}, p: {this.polar}, a: {this.azimuth})";
        }
    }
}