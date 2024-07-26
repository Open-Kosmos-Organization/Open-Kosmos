using Unity.Entities;
using UnityEngine;

namespace Kosmos.Prototypes.Camera.Components
{
    public struct PlayerInput : IComponentData
    {
        public Vector2 TranslationValue;
        public Vector2 OrbitInputValue;
        public bool OrbitActive;
        public Vector2 ZoomInputValue;
    }
}
