using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace PCB.Icosahedron.ECS.Components
{
    [BurstCompile]
    public struct NodeSubdividedRightNeighborsComponent : IComponentData
    {
        public NativeList<Entity> SubdividedTopRightNeighborEntities;
    }
}