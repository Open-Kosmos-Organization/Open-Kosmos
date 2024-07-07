using PCB.Icosahedron.ECS.Components;
using PCB.Icosahedron.ECS.Components.Tags;
using PCB.Math;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace PCB.Icosahedron.ECS.Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(NodeRootReferenceUpdateSystem))]
    public partial struct ShowNodeDebugSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new ShowNodeLeftDebugJob().ScheduleParallel();
            new ShowNodeRightDebugJob().ScheduleParallel();
            new ShowNodeBottomDebugJob().ScheduleParallel();
            new ShowNodeSubdividedLeftDebugJob().ScheduleParallel();
            new ShowNodeSubdividedRightDebugJob().ScheduleParallel();
            new ShowNodeSubdividedBottomDebugJob().ScheduleParallel();
        }
    }
    
    [BurstCompile]
    [WithNone(typeof(NodeSubdividedLeftNeighborsComponent))]
    [WithAll(typeof(NodeShowDebugTagComponent))]
    [WithAll( typeof(NodeComponent))]
    [WithAll(typeof(NodeNeighborComponent))]
    public partial struct ShowNodeLeftDebugJob : IJobEntity
    {
        [BurstCompile]
        public void Execute(
            in NodeRootReferenceComponent nodeRootReference,
            in NodeSphericalCoordinatesComponent nodeCoordinates)
        {
            double3 top = nodeRootReference.RootToWorld + nodeCoordinates.TopCartesian;
            double3 bottomLeft = nodeRootReference.RootToWorld + nodeCoordinates.BottomLeftCartesian;
            
            Debug.DrawLine(top.ToVector3(), bottomLeft.ToVector3(), Color.red, 0.0f);
        }
    }
    
    [BurstCompile]
    [WithNone(typeof(NodeSubdividedRightNeighborsComponent))]
    [WithAll(typeof(NodeShowDebugTagComponent))]
    [WithAll( typeof(NodeComponent))]
    [WithAll(typeof(NodeNeighborComponent))]
    public partial struct ShowNodeRightDebugJob : IJobEntity
    {
        public void Execute(
            in NodeRootReferenceComponent nodeRootReference,
            in NodeSphericalCoordinatesComponent nodeCoordinates)
        {
            double3 top = nodeRootReference.RootToWorld + nodeCoordinates.TopCartesian;
            double3 bottomRight = nodeRootReference.RootToWorld + nodeCoordinates.BottomRightCartesian;
            
            Debug.DrawLine(top.ToVector3(), bottomRight.ToVector3(), Color.red, 0.0f);
        }
    }
    
    [BurstCompile]
    [WithNone(typeof(NodeSubdividedBottomNeighborsComponent))]
    [WithAll(typeof(NodeShowDebugTagComponent))]
    [WithAll( typeof(NodeComponent))]
    [WithAll(typeof(NodeNeighborComponent))]
    public partial struct ShowNodeBottomDebugJob : IJobEntity
    {
        [BurstCompile]
        public void Execute(
            in NodeRootReferenceComponent nodeRootReference,
            in NodeSphericalCoordinatesComponent nodeCoordinates)
        {
            double3 bottomLeft = nodeRootReference.RootToWorld + nodeCoordinates.BottomLeftCartesian;
            double3 bottomRight = nodeRootReference.RootToWorld + nodeCoordinates.BottomRightCartesian;
            
            Debug.DrawLine(bottomLeft.ToVector3(), bottomRight.ToVector3(), Color.red, 0.0f);
        }
    }  
    
    [BurstCompile]
    [WithAll(typeof(NodeSubdividedLeftNeighborsComponent))]
    [WithAll(typeof(NodeShowDebugTagComponent))]
    [WithAll( typeof(NodeComponent))]
    [WithAll(typeof(NodeNeighborComponent))]
    public partial struct ShowNodeSubdividedLeftDebugJob : IJobEntity
    {
        [BurstCompile]
        public void Execute(
            in NodeRootReferenceComponent nodeRootReference,
            in NodeSphericalCoordinatesComponent nodeCoordinates)
        {
            double3 top = nodeRootReference.RootToWorld + nodeCoordinates.TopCartesian;
            double3 leftCenter = nodeRootReference.RootToWorld + nodeCoordinates.LeftCenterCartesian;
            double3 bottomLeft = nodeRootReference.RootToWorld + nodeCoordinates.BottomLeftCartesian;
            
            Debug.DrawLine(top.ToVector3(), leftCenter.ToVector3(), Color.red, 0.0f);
            Debug.DrawLine(leftCenter.ToVector3(), bottomLeft.ToVector3(), Color.red, 0.0f);
        }
    }
    
    [BurstCompile]
    [WithAll(typeof(NodeSubdividedRightNeighborsComponent))]
    [WithAll(typeof(NodeShowDebugTagComponent))]
    [WithAll( typeof(NodeComponent))]
    [WithAll(typeof(NodeNeighborComponent))]
    public partial struct ShowNodeSubdividedRightDebugJob : IJobEntity
    {
        [BurstCompile]
        public void Execute(
            in NodeRootReferenceComponent nodeRootReference,
            in NodeSphericalCoordinatesComponent nodeCoordinates)
        {
            double3 top = nodeRootReference.RootToWorld + nodeCoordinates.TopCartesian;
            double3 rightCenter = nodeRootReference.RootToWorld + nodeCoordinates.RightCenterCartesian;
            double3 bottomRight = nodeRootReference.RootToWorld + nodeCoordinates.BottomRightCartesian;
            
            Debug.DrawLine(top.ToVector3(), rightCenter.ToVector3(), Color.red, 0.0f);
            Debug.DrawLine(rightCenter.ToVector3(), bottomRight.ToVector3(), Color.red, 0.0f);
        }
    }
    
    [BurstCompile]
    [WithAll(typeof(NodeSubdividedBottomNeighborsComponent))]
    [WithAll(typeof(NodeShowDebugTagComponent))]
    [WithAll( typeof(NodeComponent))]
    [WithAll(typeof(NodeNeighborComponent))]
    public partial struct ShowNodeSubdividedBottomDebugJob : IJobEntity
    {
        public void Execute(
            in NodeRootReferenceComponent nodeRootReference,
            in NodeSphericalCoordinatesComponent nodeCoordinates)
        {
            double3 bottomLeft = nodeRootReference.RootToWorld + nodeCoordinates.BottomLeftCartesian;
            double3 bottomCenter = nodeRootReference.RootToWorld + nodeCoordinates.BottomCenterCartesian;
            double3 bottomRight = nodeRootReference.RootToWorld + nodeCoordinates.BottomRightCartesian;
            
            Debug.DrawLine(bottomLeft.ToVector3(), bottomCenter.ToVector3(), Color.red, 0.0f);
            Debug.DrawLine(bottomCenter.ToVector3(), bottomRight.ToVector3(), Color.red, 0.0f);
        }
    }
}