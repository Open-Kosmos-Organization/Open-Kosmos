using Unity.Entities;
using Unity.Mathematics;

namespace Kosmos.Prototypes.Character.Components
{
    public struct PlayableCharacterData : IComponentData
    {
        public float MoveSpeed;
        public float RotationSpeed;
    }
    
    public struct TargetRotation : IComponentData
    {
        public quaternion Value;
    }
}