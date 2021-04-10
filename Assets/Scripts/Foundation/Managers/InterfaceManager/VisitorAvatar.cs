using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Foundation;

namespace Game
{
    [System.Serializable]
    public sealed class VisitorAvatar : AbstractBehaviour, IPoolable<IMemoryPool>
    {
        public sealed class Factory : PlaceholderFactory<VisitorAvatar>
        {
        }

        Transform originalParent;

        public Visitor VisitorInfo;
        public Button VisitorAvatarButton;

        public Text NameText;
        public Text SurnameText;
        public Image Portrait;
        public IMemoryPool Pool { get; private set; }

        [Inject] IGameManager _gameManager = default;
        [Inject] ISceneState _visitorInFocusState = default;
        [Inject] ISceneStateManager _sceneStateManager = default;

            void Awake()
        {
            originalParent = transform.parent;
            
            VisitorAvatarButton.onClick.AddListener(() =>
            {
                _gameManager.SetVisitorInFocus(VisitorInfo);
                _gameManager.DestroyVisitorAvatar(this);
                
                _sceneStateManager.Push(_visitorInFocusState);
            });
        }

        public void OnSpawned(IMemoryPool pool)
        {
            Pool = pool;
            gameObject.SetActive(true);
            transform.SetParent(originalParent.parent, false);
        }
        
        public void OnDespawned()
        {
            gameObject.SetActive(false);
            transform.SetParent(originalParent, false);
        }

    }
}
