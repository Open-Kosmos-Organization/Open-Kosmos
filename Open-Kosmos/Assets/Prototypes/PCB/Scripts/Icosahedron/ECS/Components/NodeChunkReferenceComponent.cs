using Unity.Entities;

namespace Kosmos.Prototypes.PCB.Icosahedron.ECS.Components
{
    public struct NodeChunkReferenceComponent : IComponentData
    {
        public Entity NodeChunkEntity;
    }
}