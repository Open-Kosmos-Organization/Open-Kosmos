using Unity.Entities;

namespace Kosmos.Prototypes.PCB.Icosahedron.ECS.Components
{
    public struct NodeDistanceSubdivisionSettingsComponent : IComponentData
    {
        public double subdivisionDistance;
        public double unsubdivisionDistance;
    }
}