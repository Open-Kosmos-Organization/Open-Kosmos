using PCB.Icosahedron.ECS.Components;
using PCB.Icosahedron.ECS.Components.Tags;
using PCB.Icosahedron.ECS.SystemGroups;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.SocialPlatforms;

namespace PCB.Icosahedron.ECS.Systems
{
    [UpdateInGroup(typeof(NodeMarkingSystemGroup))]
    [BurstCompile]
    public partial struct NodeDistanceSubdivisionMarkingSystem : ISystem
    {
        private EntityQuery _query;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this._query = new EntityQueryBuilder(Allocator.Persistent)
                .WithAll<PcbSubdivisionDistanceTargetComponent>()
                .WithAll<LocalToWorld>()
                .Build(ref state);
        }

        
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);
            
            NativeArray<Entity> targetEntities = this._query.ToEntityArray(Allocator.Temp);
            
            ComponentLookup<PcbSubdivisionDistanceTargetComponent> targetComponentLookup = state.GetComponentLookup<PcbSubdivisionDistanceTargetComponent>(true);
            ComponentLookup<LocalToWorld> localToWorldLookup = state.GetComponentLookup<LocalToWorld>(true);

            new NodeDistanceSubdivisionMarkingJob
            {
                EntityCommandBuffer = ecb,
                TargetEntities = targetEntities,
                TargetComponentLookup = targetComponentLookup,
                LocalToWorldLookup = localToWorldLookup
            }.Schedule();
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }

    [BurstCompile]
    [WithAll(typeof(NodeComponent))]
    [WithPresent(typeof(NodeDistanceShouldSubdivideTagComponent))]
    [WithPresent(typeof(NodeDistanceShouldUnsubdivideTagComponent))]
    public partial struct NodeDistanceSubdivisionMarkingJob : IJobEntity
    {
        public EntityCommandBuffer EntityCommandBuffer;

        public NativeArray<Entity> TargetEntities;

        [ReadOnly]
        public ComponentLookup<PcbSubdivisionDistanceTargetComponent> TargetComponentLookup;
        [ReadOnly]
        public ComponentLookup<LocalToWorld> LocalToWorldLookup;

        [BurstCompile]
        public void Execute(
            in Entity entity,
            in LocalToWorld localToWorld,
            in NodeDistanceSubdivisionSettingsComponent distanceSubdivisionSettings)
        {
            double closestDistance = double.MaxValue;

            foreach (Entity targetEntity in this.TargetEntities)
            {
                if (targetEntity == entity)
                {
                    continue;
                }

                RefRO<PcbSubdivisionDistanceTargetComponent> targetDistanceComponent =
                    this.TargetComponentLookup.GetRefRO(targetEntity);

                RefRO<LocalToWorld> targetLocalToWorld = this.LocalToWorldLookup.GetRefRO(targetEntity);

                float3 offset = targetLocalToWorld.ValueRO.Position - localToWorld.Position;

                double3 doubleOffset = new double3(offset);

                double distance = math.length(doubleOffset);

                double scaledDistance = targetDistanceComponent.ValueRO.GetModifiedDistance(distance);

                if (scaledDistance < closestDistance)
                {
                    closestDistance = scaledDistance;
                }
            }

            if (closestDistance < distanceSubdivisionSettings.subdivisionDistance)
            {
                this.EntityCommandBuffer.SetComponentEnabled<NodeDistanceShouldSubdivideTagComponent>(entity, true);
            }

            if (closestDistance > distanceSubdivisionSettings.unsubdivisionDistance)
            {
                this.EntityCommandBuffer.SetComponentEnabled<NodeDistanceShouldUnsubdivideTagComponent>(entity, true);
            }
        }
    }
}