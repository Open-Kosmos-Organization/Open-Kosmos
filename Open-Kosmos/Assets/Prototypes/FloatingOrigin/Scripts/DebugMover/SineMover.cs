using Unity.Entities;

namespace Kosmos.Prototypes.FloatingOrigin.DebugMover
{
    public struct SineMover : IComponentData
    {
        public float Speed;
        public float Amplitude;
    }
}