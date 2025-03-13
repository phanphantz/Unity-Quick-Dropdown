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

    [FromConfig(typeof(SampleConfig)), SerializeField]
    ElementConfig sampleConfigItem;

    public float attack;
    public float stamina;

    //Let user pick Sprite from the folder 'Assets/Sprites' and all the subfolders below.
    //The folder information is hidden from 'isHideInfo' flag
    [FromFolder("Sprites", isHideInfo: true), SerializeField]
    Sprite sprite;
    
    //By default, Inspect button will open the assigned asset as a floating window.
    //You can change the button behaviour using different InpsectModes
    [FromFolder("Prefabs", inspectMode: InspectMode.Select), SerializeField]
    GameObject prefab;
    
    //This will NOT work. DropdownFields does not directly support List and Array.
    [FromGroup("TestGroup"), SerializeField]
    List<ElementConfig> notWorkingList = new List<ElementConfig>();
    
    //A Correct way to draw dropdown for List and Array.
    [SerializeField] List<ElementConfigData> workingDropdownList = new List<ElementConfigData>();
    
    [Serializable]
    public class ElementConfigData
    {
        [FromGroup("TestGroup", isHideInfo: true)]
        public ElementConfig config;
    }
}

