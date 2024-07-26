using Kosmos.Prototypes.FloatingOrigin.Components;
using Unity.Entities;
using UnityEngine;

namespace Kosmos.Prototypes.FloatingOrigin.Authoring
{
    public class FloatingFocusAuthoring : MonoBehaviour
    {
        private class FloatingFocusAuthoringBaker : Baker<FloatingFocusAuthoring>
        {
            public override void Bake(FloatingFocusAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                var focusData = new FloatingFocusTag();
                AddComponent(entity, focusData);
            }
        }
    }
}