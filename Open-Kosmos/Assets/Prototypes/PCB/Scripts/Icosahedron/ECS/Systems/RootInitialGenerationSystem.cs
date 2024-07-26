using Kosmos.Prototypes.PCB.Icosahedron.ECS.Components;
using Kosmos.Prototypes.PCB.Icosahedron.ECS.Components.Tags;
using Kosmos.Prototypes.PCB.Math;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Kosmos.Prototypes.PCB.Icosahedron.ECS.Systems
{
    [BurstCompile]
    [UpdateBefore(typeof(TransformSystemGroup))]
    [UpdateBefore(typeof(NodeRootReferenceUpdateSystem))]
    public partial struct RootInitialGenerationSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            /*
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

            new RootInitialGenerationJob
            {
                EntityCommandBuffer = ecb
            }.Schedule();

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
            */
        }
    }

    [BurstCompile]
    [WithNone(typeof(RootGeneratedTagComponent))]
    public partial struct RootInitialGenerationJob : IJobEntity
    {
        public EntityCommandBuffer EntityCommandBuffer;
        
        [BurstCompile]
        public void Execute(
            in Entity entity,
            in RootComponent rootComponent,
            in NodeDistanceSubdivisionSettingsComponent distanceSubdivisionSettings
        )
        {
            double phi = (1 + math.sqrt(5.0)) / 2.0;
            
            NativeList<double3> verticesCartesian = new NativeList<double3>(Allocator.Temp);

            verticesCartesian.Add(new double3(0.0, phi, -1.0));
            verticesCartesian.Add(new double3(0.0, phi, 1.0));
            verticesCartesian.Add(new double3(phi, 1.0, 0.0));
            verticesCartesian.Add(new double3(1.0, 0.0, -phi));
            verticesCartesian.Add(new double3(-1.0, 0.0, -phi));
            verticesCartesian.Add(new double3(-phi, 1.0, 0.0));
            verticesCartesian.Add(new double3(1.0, 0.0, phi));
            verticesCartesian.Add(new double3(phi, -1.0, 0.0));
            verticesCartesian.Add(new double3(0.0, -phi, -1.0));
            verticesCartesian.Add(new double3(-phi, -1.0, 0.0));
            verticesCartesian.Add(new double3(-1.0, 0.0, phi));
            verticesCartesian.Add(new double3(0.0, -phi, 1.0));

            SphericalCoordinateDegrees topRotationDegrees =
                SphericalCoordinateDegrees.FromCartesian(verticesCartesian[0]);

            double4x4 yRotationMatrix = new double4x4(
                    new float4x4(quaternion.EulerXYZ(0.0f, (float) -topRotationDegrees.ToRadians().azimuth, 0.0f), 
                    float3.zero
                    ));

            double4x4 xRotationMatrix = new double4x4(
                    new float4x4(quaternion.EulerXYZ((float) -topRotationDegrees.ToRadians().polar, 0.0f, 0.0f),
                    float3.zero
                    ));

            double4x4 yCounterRotationMatrix = new double4x4(
                    new float4x4(quaternion.EulerXYZ(0.0f, (float) topRotationDegrees.ToRadians().azimuth, 0.0f),
                    float3.zero
                    ));

            for (int i = 0; i < verticesCartesian.Length; i++)
            {
                double4 yRotated = math.mul(yRotationMatrix, new double4(verticesCartesian[i], 0.0));
                double4 xRotated = math.mul(xRotationMatrix, yRotated);
                double4 final = math.mul(yCounterRotationMatrix, xRotated);
                
                verticesCartesian[i] = final.xyz;
            }

            NativeList<SphericalCoordinateRadians> vertices =
                new NativeList<SphericalCoordinateRadians>(verticesCartesian.Length, Allocator.Temp);

            foreach (double3 vertex in verticesCartesian)
            {
                vertices.Add(SphericalCoordinateRadians.FromCartesian(vertex));
            }

            double radial = rootComponent.ScaledRadius;

            for (int i = 0; i < vertices.Length; i++)
            {
                SphericalCoordinateRadians vertex = vertices[i];
                vertex.radial = radial;
                vertices[i] = vertex;
            }

            vertices[0] = new SphericalCoordinateRadians(radial, 0.0, 0.0);
            vertices[11] = new SphericalCoordinateRadians(radial, math.PI_DBL, 0.0);
            
            NativeList<(SphericalCoordinateRadians, SphericalCoordinateRadians, SphericalCoordinateRadians)> nodes 
                = new NativeList<(SphericalCoordinateRadians, SphericalCoordinateRadians, SphericalCoordinateRadians)>(20, Allocator.Temp);
            
            nodes.Add((vertices[0], vertices[2], vertices[1]));
            nodes.Add((vertices[0], vertices[3], vertices[2]));
            nodes.Add((vertices[0], vertices[4], vertices[3]));
            nodes.Add((vertices[0], vertices[5], vertices[4]));
            nodes.Add((vertices[0], vertices[1], vertices[5]));
            
            nodes.Add((vertices[6], vertices[1], vertices[2]));
            nodes.Add((vertices[2], vertices[7], vertices[6]));
            nodes.Add((vertices[7], vertices[2], vertices[3]));
            nodes.Add((vertices[3], vertices[8], vertices[7]));
            nodes.Add((vertices[8], vertices[3], vertices[4]));
            nodes.Add((vertices[4], vertices[9], vertices[8]));
            nodes.Add((vertices[9], vertices[4], vertices[5]));
            nodes.Add((vertices[5], vertices[10], vertices[9]));
            nodes.Add((vertices[10], vertices[5], vertices[1]));
            nodes.Add((vertices[1], vertices[6], vertices[10]));
            
            nodes.Add((vertices[11], vertices[6], vertices[7]));
            nodes.Add((vertices[11], vertices[7], vertices[8]));
            nodes.Add((vertices[11], vertices[8], vertices[9]));
            nodes.Add((vertices[11], vertices[9], vertices[10]));
            nodes.Add((vertices[11], vertices[10], vertices[6]));
            
            NativeList<Entity> childEntities = new NativeList<Entity>(Allocator.Persistent);

            DynamicBuffer<NodeChildComponent> childBuffer = this.EntityCommandBuffer.AddBuffer<NodeChildComponent>(entity);

            for (int i = 0; i < nodes.Length; i++)
            {
                childEntities.Add(this.EntityCommandBuffer.CreateEntity());
                
                childBuffer.Add(new NodeChildComponent
                {
                    ChildNodeEntity = childEntities[i]
                });
                
                this.EntityCommandBuffer.AddComponent(childEntities[i], new Parent
                {
                    Value = entity
                });
                
                this.EntityCommandBuffer.AddComponent(childEntities[i], new LocalTransform
                {
                    Position = default,
                    Scale = 1,
                    Rotation = default
                });
                
                this.EntityCommandBuffer.AddComponent(childEntities[i], new LocalToWorld
                {
                    Value = default
                });
                
                this.EntityCommandBuffer.AddComponent(childEntities[i], new NodeSphericalCoordinatesComponent
                {
                    Top = nodes[i].Item1.ToDegrees(),
                    BottomLeft = nodes[i].Item2.ToDegrees(),
                    BottomRight = nodes[i].Item3.ToDegrees()
                });
                
                this.EntityCommandBuffer.AddComponent(childEntities[i], new NodeRootReferenceComponent
                {
                    Root = entity,
                    RootToWorld = default
                });
                
                this.EntityCommandBuffer.AddComponent(childEntities[i], new NodeParentComponent
                {
                    ParentNodeEntity = entity
                });
                
                this.EntityCommandBuffer.AddComponent(childEntities[i], new NodeComponent
                {
                    NodeLevelOfDetail = rootComponent.ChunkSubdivisionCount
                });
                
                this.EntityCommandBuffer.AddComponent<NodeShowDebugTagComponent>(childEntities[i]);
                
                this.EntityCommandBuffer.AddComponent(childEntities[i], new NodeDistanceSubdivisionSettingsComponent
                {
                    subdivisionDistance = distanceSubdivisionSettings.subdivisionDistance,
                    unsubdivisionDistance = distanceSubdivisionSettings.unsubdivisionDistance
                });
                
                this.EntityCommandBuffer.AddComponent<NodeDistanceShouldSubdivideTagComponent>(childEntities[i]);
                this.EntityCommandBuffer.AddComponent<NodeDistanceShouldUnsubdivideTagComponent>(childEntities[i]);
                this.EntityCommandBuffer.AddComponent<NodeNeighborShouldSubdivideTagComponent>(childEntities[i]);
                this.EntityCommandBuffer.AddComponent<NodeSubdivideTagComponent>(childEntities[i]);
                this.EntityCommandBuffer.AddComponent<NodeUnsubdivideTagComponent>(childEntities[i]);
                
                this.EntityCommandBuffer.SetComponentEnabled<NodeDistanceShouldSubdivideTagComponent>(childEntities[i], false);
                this.EntityCommandBuffer.SetComponentEnabled<NodeDistanceShouldUnsubdivideTagComponent>(childEntities[i], false);
                this.EntityCommandBuffer.SetComponentEnabled<NodeNeighborShouldSubdivideTagComponent>(childEntities[i], false);
                this.EntityCommandBuffer.SetComponentEnabled<NodeSubdivideTagComponent>(childEntities[i], false);
                this.EntityCommandBuffer.SetComponentEnabled<NodeUnsubdivideTagComponent>(childEntities[i], false);
            }
            
            NativeList<Entity> leftNeighborEntities = new NativeList<Entity>(childEntities.Length, Allocator.Temp);
            NativeList<Entity> rightNeighborEntities = new NativeList<Entity>(childEntities.Length, Allocator.Temp);
            NativeList<Entity> bottomNeighborEntities = new NativeList<Entity>(childEntities.Length, Allocator.Temp);
            
            leftNeighborEntities.Add(childEntities[1]);
            rightNeighborEntities.Add(childEntities[4]);
            bottomNeighborEntities.Add(childEntities[5]);
            
            leftNeighborEntities.Add(childEntities[2]);
            rightNeighborEntities.Add(childEntities[0]);
            bottomNeighborEntities.Add(childEntities[7]);
            
            leftNeighborEntities.Add(childEntities[3]);
            rightNeighborEntities.Add(childEntities[1]);
            bottomNeighborEntities.Add(childEntities[9]);
            
            leftNeighborEntities.Add(childEntities[4]);
            rightNeighborEntities.Add(childEntities[3]);
            bottomNeighborEntities.Add(childEntities[11]);
            
            leftNeighborEntities.Add(childEntities[0]);
            rightNeighborEntities.Add(childEntities[3]);
            bottomNeighborEntities.Add(childEntities[13]);
            
            leftNeighborEntities.Add(childEntities[14]);
            rightNeighborEntities.Add(childEntities[6]);
            bottomNeighborEntities.Add(childEntities[0]);
            
            leftNeighborEntities.Add(childEntities[7]);
            rightNeighborEntities.Add(childEntities[5]);
            bottomNeighborEntities.Add(childEntities[15]);
            
            leftNeighborEntities.Add(childEntities[6]);
            rightNeighborEntities.Add(childEntities[8]);
            bottomNeighborEntities.Add(childEntities[1]);
            
            leftNeighborEntities.Add(childEntities[9]);
            rightNeighborEntities.Add(childEntities[7]);
            bottomNeighborEntities.Add(childEntities[16]);
            
            leftNeighborEntities.Add(childEntities[8]);
            rightNeighborEntities.Add(childEntities[10]);
            bottomNeighborEntities.Add(childEntities[2]);
            
            leftNeighborEntities.Add(childEntities[11]);
            rightNeighborEntities.Add(childEntities[9]);
            bottomNeighborEntities.Add(childEntities[17]);
            
            leftNeighborEntities.Add(childEntities[10]);
            rightNeighborEntities.Add(childEntities[12]);
            bottomNeighborEntities.Add(childEntities[3]);
            
            leftNeighborEntities.Add(childEntities[13]);
            rightNeighborEntities.Add(childEntities[11]);
            bottomNeighborEntities.Add(childEntities[18]);
            
            leftNeighborEntities.Add(childEntities[12]);
            rightNeighborEntities.Add(childEntities[14]);
            bottomNeighborEntities.Add(childEntities[4]);
            
            leftNeighborEntities.Add(childEntities[5]);
            rightNeighborEntities.Add(childEntities[13]);
            bottomNeighborEntities.Add(childEntities[19]);
            
            leftNeighborEntities.Add(childEntities[19]);
            rightNeighborEntities.Add(childEntities[16]);
            bottomNeighborEntities.Add(childEntities[6]);
            
            leftNeighborEntities.Add(childEntities[15]);
            rightNeighborEntities.Add(childEntities[17]);
            bottomNeighborEntities.Add(childEntities[8]);
            
            leftNeighborEntities.Add(childEntities[16]);
            rightNeighborEntities.Add(childEntities[18]);
            bottomNeighborEntities.Add(childEntities[10]);
            
            leftNeighborEntities.Add(childEntities[17]);
            rightNeighborEntities.Add(childEntities[19]);
            bottomNeighborEntities.Add(childEntities[12]);
            
            leftNeighborEntities.Add(childEntities[18]);
            rightNeighborEntities.Add(childEntities[15]);
            bottomNeighborEntities.Add(childEntities[14]);

            for (int i = 0; i < childEntities.Length; i++)
            {
                this.EntityCommandBuffer.AddComponent(childEntities[i], new NodeNeighborComponent
                {
                    LeftNeighborEntity = leftNeighborEntities[i],
                    RightNeighborEntity = rightNeighborEntities[i],
                    BottomNeighborEntity = bottomNeighborEntities[i]
                });
            }
            
            this.EntityCommandBuffer.AddComponent<RootGeneratedTagComponent>(entity);
        }
    }
}