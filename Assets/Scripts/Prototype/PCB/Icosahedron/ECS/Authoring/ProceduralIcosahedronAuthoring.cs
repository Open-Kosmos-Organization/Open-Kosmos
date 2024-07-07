using PCB.Icosahedron.ECS.Components;
using Unity.Entities;
using UnityEngine;

namespace PCB.Icosahedron.ECS.Authoring
{
    public class ProceduralIcosahedronAuthoring : MonoBehaviour
    {
        public double planetRadius;
            
        public double scale;
            
        [Range(1, 8)]
        public uint chunkSubdivisionCount;

        public double subdivisionDistance;

        public double unsubdivisionDistance;
        
        private class ProceduralIcosahedronAuthoringBaker : Baker<ProceduralIcosahedronAuthoring>
        {
            public override void Bake(ProceduralIcosahedronAuthoring authoring)
            {
                Entity entity = this.GetEntity(TransformUsageFlags.Dynamic);
                
                this.AddComponent(entity, new RootComponent
                {
                    PlanetRadiusMeters = authoring.planetRadius,
                    Scale = authoring.scale,
                    ChunkSubdivisionCount = authoring.chunkSubdivisionCount
                });
                
                this.AddComponent(entity, new NodeDistanceSubdivisionSettingsComponent
                {
                    subdivisionDistance = authoring.subdivisionDistance,
                    unsubdivisionDistance = authoring.subdivisionDistance < authoring.unsubdivisionDistance 
                        ? authoring.subdivisionDistance 
                        : authoring.unsubdivisionDistance
                });
            }
        }
    }
}