using PCB.Icosahedron.ECS.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace PCB.Icosahedron.ECS.Systems
{
    public partial struct NodeChunkGenerationSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

            new NodeChunkGenerationJob
            {
                EntityCommandBuffer = ecb
            }.Schedule();

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }

    [BurstCompile]
    [WithNone(typeof(NodeChunkReferenceComponent))]
    public partial struct NodeChunkGenerationJob : IJobEntity
    {
        public EntityCommandBuffer EntityCommandBuffer;
        
        [BurstCompile]
        public void Execute(
            in Entity entity,
            in NodeComponent node,
            in NodeSphericalCoordinatesComponent coordinates)
        {
        }
    }
}