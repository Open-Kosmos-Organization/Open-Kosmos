using Kosmos.Prototypes.PCB.Icosahedron.ECS.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Kosmos.Prototypes.PCB.Icosahedron.ECS.Systems
{
    [UpdateAfter(typeof(TransformSystemGroup))]
    public partial struct NodeRootReferenceUpdateSystem : ISystem
    {
        private ComponentLookup<LocalToWorld> _localToWorldLookup;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this._localToWorldLookup = state.GetComponentLookup<LocalToWorld>(true);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            this._localToWorldLookup.Update(ref state);

            new NodeRootReferenceUpdateJob
            {
                LocalToWorldLookup = this._localToWorldLookup
            }.ScheduleParallel();
        }
    }

    [BurstCompile]
    public partial struct NodeRootReferenceUpdateJob : IJobEntity
    {
        [ReadOnly]
        public ComponentLookup<LocalToWorld> LocalToWorldLookup;

        [BurstCompile]
        public void Execute(ref NodeRootReferenceComponent rootReferenceComponent)
        {
            if (!this.LocalToWorldLookup.HasComponent(rootReferenceComponent.Root))
            {
                Debug.LogWarning("Node Entity with root reference to Root Entity that has no LocalToWorld component");
            }

            rootReferenceComponent.RootToWorld = this.LocalToWorldLookup[rootReferenceComponent.Root].Position;
        }
    }
}