using Unity.Entities;

namespace Kosmos.Prototypes.PCB.Icosahedron.ECS.Components
{
    public struct NodeComponent : IComponentData
    {
        public uint NodeLevelOfDetail;
    }
}