using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace PCB.Icosahedron.ECS.Components
{
    [BurstCompile]
    public struct NodeSubdividedLeftNeighborsComponent : IComponentData
    {
        public NativeList<Entity> SubdividedTopLeftNeighborEntities;
    }
}