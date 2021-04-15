using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Foundation;
using UnityEngine.UI;
using Zenject;

namespace Game
{
    public class VisitorInFocusButton : MonoBehaviour
    {
        
        [Inject] IInterfaceManager __interfaceManager = default;
        
        // Start is called before the first frame update
        void Start()
        {
            //GetComponent<Button>().onClick.AddListener(() => _interfaceManager.Pop());
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}

