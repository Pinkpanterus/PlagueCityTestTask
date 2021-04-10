using System.Collections;
using System.Collections.Generic;
using Foundation;
using Game;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class VisitorInFocusScript : AbstractBehaviour,IOnStateActivate
{
    [Inject] IGameManager _gameManager = default;
    [Inject] private ISceneState _sceneState = default;
    
    public Image Portrait;
    public Text NameSurnameText;
    public Text ReplicText;
    
    
    protected override void OnEnable()
    {
        base.OnEnable();
        Observe(_sceneState.OnActivate);
        //Observe(_sceneState.OnUpdate);
    }

    void IOnStateActivate.Do()
    {
        //Debug.Log("On Visitor in focus state activate");
        if (_gameManager.VisitorInFocus != null)
        {
            var visitorInFocus = _gameManager.VisitorInFocus;
            Portrait.sprite = visitorInFocus.Portrait;
            NameSurnameText.text = visitorInFocus.Name + " " + visitorInFocus.SurName;
            ReplicText.text ="\"" + visitorInFocus.Replic + "\"";
        }
        else
        {
            Debug.Log("_gameManager.VisitorInFocus is null");
        }
    }
}
