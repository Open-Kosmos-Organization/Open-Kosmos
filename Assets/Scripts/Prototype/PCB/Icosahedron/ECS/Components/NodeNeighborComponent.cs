using Unity.Burst;
using Unity.Entities;

namespace PCB.Icosahedron.ECS.Components
{
    [BurstCompile]
    public struct NodeNeighborComponent : IComponentData
    {
        public Entity LeftNeighborEntity;
        public Entity RightNeighborEntity;
        public Entity BottomNeighborEntity;
    }
}