using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace PCB.Icosahedron.ECS.Components
{
    [BurstCompile]
    public struct NodeRootReferenceComponent : IComponentData
    {
        public Entity Root;
        public double3 RootToWorld;
    }
}