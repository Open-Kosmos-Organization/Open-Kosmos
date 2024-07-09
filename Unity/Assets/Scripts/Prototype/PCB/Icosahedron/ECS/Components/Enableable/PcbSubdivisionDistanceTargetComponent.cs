using Unity.Entities;

namespace PCB.Icosahedron.ECS.Components.Tags
{
    public struct PcbSubdivisionDistanceTargetComponent : IComponentData
    {
        public double PreScaleDistanceOffset;

        public double DistanceScale;

        public double PostScaleDistanceOffset;

        public readonly double GetModifiedDistance(double distance)
        {
            return ((distance + this.PreScaleDistanceOffset) * this.DistanceScale) + this.PostScaleDistanceOffset;
        }
    }
}