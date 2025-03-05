using UnityEngine;

public class ElementConfig : ScriptableObject
{
    public string id;
    public Sprite icon;
    public string displayName;
    [TextArea] public string description;
    public string[] tags;
}