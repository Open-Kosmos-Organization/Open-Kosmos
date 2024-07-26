using Unity.Entities;

namespace Kosmos.Prototypes.FloatingOrigin.Components
{
    public struct ParentFloatingPositionData : IComponentData
    {
        public double LocalX;
        public double LocalY;
        public double LocalZ;
        public long GlobalX;
        public long GlobalY;
        public long GlobalZ;
    }
}
