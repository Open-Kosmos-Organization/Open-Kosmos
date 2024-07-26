using Kosmos.Prototypes.FloatingOrigin.Systems;
using Unity.Entities;
using UnityEngine;

namespace Kosmos.Prototypes.FloatingOrigin.DebugMover
{
    [UpdateBefore(typeof(FloatingOriginSnapCheckSystem))]
    public class SineMoverAuthoring : MonoBehaviour
    {
        [SerializeField] private float _speed = 10f;
        [SerializeField] private float _amplitude = 1f;
        
        private class SineMoverAuthoringBaker : Baker<SineMoverAuthoring>
        {
            public override void Bake(SineMoverAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new SineMover()
                {
                    Speed = authoring._speed,
                    Amplitude = authoring._amplitude
                });
            }
        }
    }
}