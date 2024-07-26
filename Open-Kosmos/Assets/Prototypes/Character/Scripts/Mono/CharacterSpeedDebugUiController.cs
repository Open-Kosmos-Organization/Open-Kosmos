using System;
using Kosmos.Prototypes.Character.Components;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kosmos.Prototypes.Character.Mono
{
    public class CharacterSpeedDebugUiController : MonoBehaviour
    {
        [SerializeField] private UIDocument _uiDocument;
        
        private EntityManager _entityManager;
        private Entity _entity;
        
        private Label _speedLabel;
        
        private void Start()
        {
            _speedLabel = _uiDocument.rootVisualElement.Q<Label>("text_time");
            
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _entity = _entityManager.CreateEntity();
            _entityManager.AddComponentObject(_entity, this);
        }

        private void Update()
        {
            var query = _entityManager.CreateEntityQuery(typeof(PlayableCharacterData));
            var count = query.CalculateEntityCount();
            
            if (count == 0)
            {
                return;
            }
            
            var playerCharacterData = query.GetSingleton<PlayableCharacterData>();
            SetSpeedText(playerCharacterData.MoveSpeed);
        }

        public void SetSpeedText(float speed)
        {
            _speedLabel.text = speed.ToString("N1");
        }
    }
}