using Kosmos.Prototypes.FloatingOrigin.Components;
using Unity.Entities;
using UnityEngine;

namespace Kosmos.Prototypes.FloatingOrigin.Authoring
{
    public class FloatingOriginAuthoring : MonoBehaviour
    {
        private class FloatingOriginAuthoringBaker : Baker<FloatingOriginAuthoring>
        {
            public override void Bake(FloatingOriginAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new FloatingOriginData()
                {
                    Scale = 1f
                });
            }
        }
    }
}