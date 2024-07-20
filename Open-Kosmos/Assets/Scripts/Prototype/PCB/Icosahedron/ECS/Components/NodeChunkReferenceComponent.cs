using Unity.Entities;

namespace PCB.Icosahedron.ECS.Components
{
    public struct NodeChunkReferenceComponent : IComponentData
    {
        public Entity NodeChunkEntity;
    }
}