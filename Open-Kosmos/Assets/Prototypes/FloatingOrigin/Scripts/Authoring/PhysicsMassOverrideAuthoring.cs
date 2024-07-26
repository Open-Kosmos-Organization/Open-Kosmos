using Unity.Entities;
using Unity.Physics;
using UnityEngine;

namespace Kosmos.Prototypes.FloatingOrigin.Authoring
{
    public class PhysicsMassOverrideAuthoring : MonoBehaviour
    {
        private class PhysicsMassOverrideAuthoringBaker : Baker<PhysicsMassOverrideAuthoring>
        {
            public override void Bake(PhysicsMassOverrideAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new PhysicsMassOverride()
                {
                    IsKinematic = 0
                });
            }
        }
    }
}