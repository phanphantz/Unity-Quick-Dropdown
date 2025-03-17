# ⚡Unity Quick Dropdown

C# Attributes that allow you to quickly assign Unity assets from a Dropdown on the Inspector. Support any `UnityEngine.Object` types. Help save time and reduce human errors by letting you pick an object from your desired location.

<img src=https://github.com/phanphantz/GameDevSecretSauce/blob/main/Assets/QuickDropdown/QuickDropdownExample_GIF.gif width=100%>

> [!NOTE]
> This Library is not an Official Library from Unity.

# Overview

- **[FromFolder]** - Display a Dropdown of Unity Assets from a specific **Folder**.
- **[FromGroup]** - Display a Dropdown of Unity Assets from the `ScriptableGroup` with a matching name.
  - Using `ScriptableGroups` allows you to move the assets in the project around without losing their group organization.
  - You can nest ScriptableGroup inside each other. Cyclic references are also prevented and filtered out.
- **[FromConfig]** - Display a Dropdown of Unity Assets from a first found `ScriptableContainer` with a specified type.
  - This is useful for looking up objects from a ScriptableObject that is meant to be a singular "Config" or "Setting" (a Singleton if you will)
- **[FromAddressable]** - Display a Dropdown of Addressable Assets from a specific Addressable Group.
- Supported dropdown display for nested elements inside **List & Array**. (Direct usages on List & Array are not supported though)
- **QoL Features**:
  - Select & Jump to the assigned asset or its enclosing location.
  - You get a **Fix** button for creating a new source if it does not exist.
  - An **Inspect** button allows you to quickly open a floating property window of the assigned asset.
  - Warn user if the assigned object does not belong to the specified location.
  - Warn about invalid locations.
  - Supports nested dropdown.
  - Easily customize how the Dropdown look using attribute parameters.
  - Supports Multi-Edit.
  - Undo-Friendly.

- For **ScriptableObjects**:
  - Quickly create new instances of `ScriptableObject` and add them into the specified location from a `+` button. When creating a new asset this way, the enclosing Folder / ScriptableGroup are also **created automatically** if they didn't exist.

## **Installation**
There are 2 options to install the package:
  - A) Download source code and put them into the Unity's Assets folder
  - B) Install from the **Package Manager** using this git URL: https://github.com/phanphantz/Unity-Quick-Dropdown.git
 
# Quick Example

```csharp
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
    [FromFolder("Sprites", isHideInfo: true), SerializeField]
    Sprite sprite;
    
    //By default, Inspect button will open the assigned asset as a floating window.
    //You can change the button behaviour using different InpsectModes
    [FromFolder("Prefabs", inspectMode: InspectMode.Select), SerializeField]
    GameObject prefab;
}
```
### Result:
<img src=https://github.com/phanphantz/GameDevSecretSauce/blob/main/Assets/QuickDropdown/QuickDropdownExample_1.jpeg width=80%>

# List & Array Support
Although direct usages on List & Array are not directly supported, there is a workaround...

```csharp
//This will NOT work. DropdownFields does not directly support List and Array.
[FromGroup("TestGroup"), SerializeField]
List<ElementConfig> notWorkingList = new List<ElementConfig>();

//A Correct way to draw dropdown for List and Array.
[SerializeField] List<ElementConfigData> workingDropdownList = new List<ElementConfigData>();
[SerializeField] ElementConfigData[] workingDropdownArray = new ElementConfigData[] {};
    
[Serializable]
public class ElementConfigData
{
    [FromGroup("TestGroup", isHideInfo: true)]
    public ElementConfig config;
}
```

# Addressables Support
If you have the **Addressables** package installed in the project, you can use **[FromAddressable]** attribute on **Unity Object fields** and **string fields** to display a dropdown of Addressable assets from a desired group.
- When you specify an Addressable group name that does not exist. You also get the **Fix** button on the inspector to quickly create it.
- Creating **ScriptableObjects** using **Create** button from the inspector will also add the created asset to the target Addressable Group.

```csharp
using PhEngine.QuickDropdown.Addressables;
using UnityEngine;

public class AddressableDropdownExample : MonoBehaviour
{
    [FromAddressable("PackedAddressableGroup"), SerializeField]
    ElementConfig addressableConfig;
    
    [FromAddressable("PackedAddressableGroup"), SerializeField]
    string addressableAddress;
}
```
> [!NOTE]
> Unfortunately **[FromAddressable]** does not work with **AssetReference** at the moment. Since it is drawn by its own property drawer.

# Future Plans
- **[FromScene]** attribute.
- **[FromStringList]** attribute.
- A way to bind Create functions for creating non-ScriptableObject assets.
- A Popup UGUI to specify the name of the asset upon creation.

Please feel free to Contribute and send me Pull requests.
You can also [**Buy me a coffee!**](https://buymeacoffee.com/phanphantz)☕

**Phun,**\
phun.peeticharoenthum@gmail.com
