using Kosmos.Prototypes.FloatingOrigin.Utility;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Kosmos.Prototypes.FloatingOrigin.Authoring
{
    public class FloatingPositionAuthoring : MonoBehaviour
    {
        private class FloatingPositionAuthoringBaker : Baker<FloatingPositionAuthoring>
        {
            public override void Bake(FloatingPositionAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                
                var floatingPosition = FloatingOriginMath.InitializeLocal(
                    new double3(authoring.transform.position),
                    authoring.transform.localScale.x);
                
                AddComponent(entity, floatingPosition);
            }
        }
    }
}