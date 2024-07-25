using System.Runtime.InteropServices;
using Kosmos.Prototypes.Character.Components;
using Unity.Entities;
using UnityEngine;

namespace Kosmos.Prototypes.Character.Authoring
{
    public class PlayerCharacterAuthoring : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 20f;
        [SerializeField] private float _rotationSpeed = 10f;
        
        private class PlayerCharacterAuthoringBaker : Baker<PlayerCharacterAuthoring>
        {
            public override void Bake(PlayerCharacterAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                var playerCharacter = new PlayableCharacterData()
                {
                    MoveSpeed = authoring._moveSpeed,
                    RotationSpeed = authoring._rotationSpeed
                };
                
                AddComponent(entity, playerCharacter);

                var targetRotation = new TargetRotation()
                {
                    Value = authoring.transform.rotation
                };
                
                AddComponent(entity, targetRotation);
            }
        }
    }
}