using Kosmos.Prototypes.PCB.Icosahedron.ECS.Components.Tags;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Kosmos.Prototypes.PCB.Icosahedron.ECS.Systems.NodeMarking
{
    [UpdateInGroup(typeof(NodeMarkingSystemGroup), OrderLast = true)]
    [BurstCompile]
    public partial struct NodeSubdivisionMarkerReconciliationSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            /*
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

            new NodeDistanceSubdivisionMarkerReconciliationSystem
            {
                EntityCommandBuffer = ecb
            }.Schedule();
            
            new NodeDistanceUnsubdivisionMarkerReconciliationSystem
            {
                EntityCommandBuffer = ecb
            }.Schedule();
            
            new NodeDistanceConflictingSubdivisionMarkerReconciliationSystem()
            {
                EntityCommandBuffer = ecb
            }.Schedule();
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
            */
        }
    }

    [WithAll(typeof(NodeDistanceShouldSubdivideTagComponent))]
    [WithDisabled(typeof(NodeDistanceShouldUnsubdivideTagComponent))]
    [WithDisabled(typeof(NodeNeighborShouldSubdivideTagComponent))]
    public partial struct NodeDistanceSubdivisionMarkerReconciliationSystem : IJobEntity
    {
        public EntityCommandBuffer EntityCommandBuffer;

        public void Execute(
            in Entity entity
        )
        {
            this.EntityCommandBuffer.SetComponentEnabled<NodeDistanceShouldSubdivideTagComponent>(entity, false);
            this.EntityCommandBuffer.SetComponentEnabled<NodeSubdivideTagComponent>(entity, true);
        }
    }
    
    [WithDisabled(typeof(NodeDistanceShouldSubdivideTagComponent))]
    [WithAll(typeof(NodeDistanceShouldUnsubdivideTagComponent))]
    [WithDisabled(typeof(NodeNeighborShouldSubdivideTagComponent))]
    public partial struct NodeDistanceUnsubdivisionMarkerReconciliationSystem : IJobEntity
    {
        public EntityCommandBuffer EntityCommandBuffer;

        public void Execute(
            in Entity entity
        )
        {
            this.EntityCommandBuffer.SetComponentEnabled<NodeDistanceShouldUnsubdivideTagComponent>(entity, false);
            this.EntityCommandBuffer.SetComponentEnabled<NodeUnsubdivideTagComponent>(entity, true);
        }
    }
    
    [WithAll(typeof(NodeDistanceShouldSubdivideTagComponent))]
    [WithAll(typeof(NodeDistanceShouldUnsubdivideTagComponent))]
    [WithDisabled(typeof(NodeNeighborShouldSubdivideTagComponent))]
    public partial struct NodeDistanceConflictingSubdivisionMarkerReconciliationSystem : IJobEntity
    {
        public EntityCommandBuffer EntityCommandBuffer;

        public void Execute(
            in Entity entity
        )
        {
            this.EntityCommandBuffer.SetComponentEnabled<NodeDistanceShouldSubdivideTagComponent>(entity, false);
            this.EntityCommandBuffer.SetComponentEnabled<NodeDistanceShouldUnsubdivideTagComponent>(entity, false);
        }
    }
}