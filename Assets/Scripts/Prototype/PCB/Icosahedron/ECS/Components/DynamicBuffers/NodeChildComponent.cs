using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace PCB.Icosahedron.ECS.Components
{
    [BurstCompile]
    [InternalBufferCapacity(20)]
    public struct NodeChildComponent : IBufferElementData
    {
        public Entity ChildNodeEntity;
    }
}