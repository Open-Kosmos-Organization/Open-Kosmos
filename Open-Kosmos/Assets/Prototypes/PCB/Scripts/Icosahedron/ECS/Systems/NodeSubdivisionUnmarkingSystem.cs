using Kosmos.Prototypes.PCB.Icosahedron.ECS.Components.Tags;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Kosmos.Prototypes.PCB.Icosahedron.ECS.Systems
{
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderLast = true)]
    [UpdateBefore(typeof(EndInitializationEntityCommandBufferSystem))]
    public partial struct NodeSubdivisionUnmarkingSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            return;
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

            new NodeDistanceSubdivisionUnmarkingJob
            {
                ecb = ecb
            }.Schedule();
            
            new NodeDistanceUnsubdivisionUnmarkingJob
            {
                ecb = ecb
            }.Schedule();
            
            new NodeNeighborSubdivisionUnmarkingJob
            {
                ecb = ecb
            }.Schedule();
            
            new NodeSubdivisionUnmarkingJob
            {
                ecb = ecb
            }.Schedule();
            
            new NodeUnsubdivisionUnmarkingJob
            {
                ecb = ecb
            }.Schedule();
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }

    [WithAll(typeof(NodeDistanceShouldSubdivideTagComponent))]
    public partial struct NodeDistanceSubdivisionUnmarkingJob : IJobEntity
    {
        public EntityCommandBuffer ecb;
        
        public void Execute(
            in Entity entity)
        {
            this.ecb.SetComponentEnabled<NodeDistanceShouldSubdivideTagComponent>(entity, false);
        }
    }
    
    [WithAll(typeof(NodeDistanceShouldUnsubdivideTagComponent))]
    public partial struct NodeDistanceUnsubdivisionUnmarkingJob : IJobEntity
    {
        public EntityCommandBuffer ecb;
        
        public void Execute(
            in Entity entity)
        {
            this.ecb.SetComponentEnabled<NodeDistanceShouldUnsubdivideTagComponent>(entity, false);
        }
    }
    
    [WithAll(typeof(NodeNeighborShouldSubdivideTagComponent))]
    public partial struct NodeNeighborSubdivisionUnmarkingJob : IJobEntity
    {
        public EntityCommandBuffer ecb;
        
        public void Execute(
            in Entity entity)
        {
            this.ecb.SetComponentEnabled<NodeNeighborShouldSubdivideTagComponent>(entity, false);
        }
    }
    
    [WithAll(typeof(NodeSubdivideTagComponent))]
    public partial struct NodeSubdivisionUnmarkingJob : IJobEntity
    {
        public EntityCommandBuffer ecb;
        
        public void Execute(
            in Entity entity)
        {
            this.ecb.SetComponentEnabled<NodeSubdivideTagComponent>(entity, false);
        }
    }
    
    [WithAll(typeof(NodeUnsubdivideTagComponent))]
    public partial struct NodeUnsubdivisionUnmarkingJob : IJobEntity
    {
        public EntityCommandBuffer ecb;
        
        public void Execute(
            in Entity entity)
        {
            this.ecb.SetComponentEnabled<NodeUnsubdivideTagComponent>(entity, false);
        }
    }
}