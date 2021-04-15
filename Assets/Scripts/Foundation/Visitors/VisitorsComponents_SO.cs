using System;
using Foundation;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    [CreateAssetMenu(fileName = "VisitorsComponents", menuName = "ScriptableObjects/VisitorsLocalisedComponents_SO", order = 1)]
    public class VisitorsComponents_SO : ScriptableObject
    {
        public NameUnit[] Names;
        public LocalizedString[] Surnames;
        public LocalizedString[] Replics;
        public PortraitUnit[] Portraits;
    }
    
    [Serializable]
    public struct NameUnit
    {
        public LocalizedString Name;
        public bool isFemale;
    }

    [Serializable]
    public struct PortraitUnit
    {
        public Sprite Portrait;
        public bool isFemale;
    }

}
