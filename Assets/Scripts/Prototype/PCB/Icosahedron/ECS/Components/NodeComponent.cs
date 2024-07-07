using Unity.Entities;

namespace PCB.Icosahedron.ECS.Components
{
    public struct NodeComponent : IComponentData
    {
        public uint NodeLevelOfDetail;
    }
}