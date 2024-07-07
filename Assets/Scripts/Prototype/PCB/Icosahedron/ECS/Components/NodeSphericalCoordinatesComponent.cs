using PCB.Math;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace PCB.Icosahedron.ECS.Components
{
    [BurstCompile]
    public struct NodeSphericalCoordinatesComponent : IComponentData
    {
        public SphericalCoordinateDegrees Top;
        public SphericalCoordinateDegrees BottomLeft;
        public SphericalCoordinateDegrees BottomRight;

        public SphericalCoordinateDegrees Center
        {
            [BurstCompile] get => SphericalCoordinateDegrees.TriangleInterpolateCenter(this.Top, this.BottomLeft, this.BottomRight);
        }

        public double3 TopCartesian
        {
            [BurstCompile] get => this.Top.ToCartesian();
        }

        public double3 BottomLeftCartesian
        {
            [BurstCompile] get => this.BottomLeft.ToCartesian();
        }

        public double3 BottomRightCartesian
        {
            [BurstCompile] get => this.BottomRight.ToCartesian();
        }
        
        public double3 CenterCartesian
        {
            [BurstCompile] get => this.Center.ToCartesian();
        }

        public SphericalCoordinateRadians TopRadians
        {
            [BurstCompile] get => this.Top.ToRadians();
        }

        public SphericalCoordinateRadians BottomLeftRadians
        {
            [BurstCompile] get => this.BottomLeft.ToRadians();
        }

        public SphericalCoordinateRadians BottomRightRadians
        {
            [BurstCompile] get => this.BottomRight.ToRadians();
        }

        public SphericalCoordinateRadians CenterRadians
        {
            [BurstCompile] get => this.Center.ToRadians();
        }

        public SphericalCoordinateDegrees LeftCenter
        {
            [BurstCompile] get => this.Top.Interpolate(this.BottomLeft, 0.5);
        }

        public SphericalCoordinateDegrees RightCenter
        {
            [BurstCompile] get => this.Top.Interpolate(this.BottomRight, 0.5);
        }

        public SphericalCoordinateDegrees BottomCenter
        {
            [BurstCompile] get => this.BottomLeft.Interpolate(this.BottomRight, 0.5);
        }

        public SphericalCoordinateRadians LeftCenterRadians
        {
            [BurstCompile] get => this.LeftCenter.ToRadians();
        }

        public SphericalCoordinateRadians RightCenterRadians
        {
            [BurstCompile] get => this.RightCenter.ToRadians();
        }

        public SphericalCoordinateRadians BottomCenterRadians
        {
            [BurstCompile] get => this.BottomCenter.ToRadians();
        }

        public double3 LeftCenterCartesian
        {
            [BurstCompile] get => this.LeftCenter.ToCartesian();
        }

        public double3 RightCenterCartesian
        {
            [BurstCompile] get => this.RightCenter.ToCartesian();
        }

        public double3 BottomCenterCartesian
        {
            [BurstCompile] get => this.BottomCenter.ToCartesian();
        }
    }
}