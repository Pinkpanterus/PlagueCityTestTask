using System;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "VisitorsComponents", menuName = "ScriptableObjects/VisitorsComponents", order = 1)]
public class VisitorsComponents_SO : ScriptableObject
{   public NameUnit[] Names;
    public string[] Surnames;
    public string[] Replics;
    public PortraitUnit[] Portraits;
}

[Serializable]
public struct NameUnit
{
    public string Name;
    public bool isFemale;
}

[Serializable]
public struct PortraitUnit
{
    public Sprite Portrait;
    public bool isFemale;
}