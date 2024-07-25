using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Kosmos.Prototypes.PCB.Icosahedron.ECS.Components
{
    [BurstCompile]
    public struct NodeSubdividedBottomNeighborsComponent : IComponentData
    {
        public NativeList<Entity> SubdividedBottomNeighborEntities;
    }
}