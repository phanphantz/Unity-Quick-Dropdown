using System;
using System.Collections.Generic;
using PhEngine.QuickDropdown;
using UnityEngine;

public class QuickDropdownExample : MonoBehaviour
{
    public float health;

    //Let user pick 'ElementConfig' asset from the ScriptableGroup named 'TestGroup' in the asset folder.
    //By default, This also display Inspect button, Create button (Only for ScriptableObjects), and a mini button to jump to the enclosing group.
    [FromGroup("TestGroup"), SerializeField]
    ElementConfig element;

    //Let user pick 'ElementConfig' from a first found ScriptableObject with the type of 'SampleConfig'
    [FromConfig(typeof(SampleConfig)), SerializeField]
    ElementConfig sampleConfigItem;

    public float attack;
    public float stamina;

    //Let user pick Sprite from the folder 'Assets/Sprites' and all the subfolders below.
    //The folder information is hidden from 'isHideInfo' flag
    //These path variations also work: 'Assets/Sprites', 'Assets/Sprites/', 'Sprites/'
    [FromFolder("Sprites"), SerializeField]
    Sprite sprite;
    
    //By default, Inspect button will open the assigned asset as a floating window.
    //You can change the button behaviour using different InpsectModes
    [FromFolder("Prefabs", inspectMode: InspectMode.Select), SerializeField]
    GameObject prefab;
    
    //Quick Dropdown now directly supports List & Array
    [FromGroup("TestGroup"), SerializeField]
    List<ElementConfig> directList = new List<ElementConfig>();
    
    [FromGroup("TestGroup"), SerializeField]
    ElementConfig[] directArray = new ElementConfig[] {};
    
    //Nested List & Array also works
    [SerializeField] List<ElementConfigData> nestedDropdownList = new List<ElementConfigData>();
    [SerializeField] ElementConfigData[] nestedDropdownArray = new ElementConfigData[] {};
    
    [Serializable]
    public class ElementConfigData
    {
        [FromGroup("TestGroup", isHideInfo: true)]
        public ElementConfig config;
    }
}

