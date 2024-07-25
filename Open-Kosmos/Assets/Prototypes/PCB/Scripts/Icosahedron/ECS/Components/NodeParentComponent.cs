using Unity.Burst;
using Unity.Entities;

namespace Kosmos.Prototypes.PCB.Icosahedron.ECS.Components
{
    [BurstCompile]
    public struct NodeParentComponent : IComponentData
    {
        public Entity ParentNodeEntity;
    }
}