using Unity.Entities;

namespace PCB.Icosahedron.ECS.Components
{
    public struct NodeDistanceSubdivisionSettingsComponent : IComponentData
    {
        public double subdivisionDistance;
        public double unsubdivisionDistance;
    }
}