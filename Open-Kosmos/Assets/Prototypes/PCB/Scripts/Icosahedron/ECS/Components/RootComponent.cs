using Unity.Burst;
using Unity.Entities;

namespace Kosmos.Prototypes.PCB.Icosahedron.ECS.Components
{
    [BurstCompile]
    public struct RootComponent : IComponentData
    {
        public double PlanetRadiusMeters;
        public double Scale;

        public uint ChunkSubdivisionCount;

        public double ScaledRadius
        {
            [BurstCompile]
            get => this.PlanetRadiusMeters / this.Scale;
        }
    }
}