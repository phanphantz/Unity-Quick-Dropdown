# Unity Quick Dropdown

C# Attributes that allow you to quickly assign Unity assets from a Dropdown on the Inspector. Support any `UnityEngine.Object` types. Help save time and reduce human errors by letting you pick an object from your desired location.

<img src=https://github.com/phanphantz/GameDevSecretSauce/blob/main/Assets/QuickDropdown/QuickDropdownExample_GIF.gif width=100%>

> [!NOTE]
> This Library is not associated with Unity.

# Overview

- **[FromFolder]** - Display a Dropdown of Unity Assets from a specific **Folder**.
- **[FromGroup]** - Display a Dropdown of Unity Assets from the `ScriptableGroup` with a matching name.
  - Using `ScriptableGroups` allows you to move the assets in the project around without losing their group organization.
- **QoL Features**:
  - Select & Jump to the assigned asset or its enclosing location.
  - Quickly open a floating property window of the assigned asset.
  - Warn user if the assigned object does not belong to the specified location.
  - Warn about invalid locations.
  - Supports nested dropdown.
  - Easily customize how the Dropdown look using attribute parameters.
- For **ScriptableObjects**:
  - Quickly create new instances of `ScriptableObject` and add them into the specified location from a `+` button. When creating a new asset this way, the enclosing Folder / ScriptableGroup are also **created automatically** if they didn't exist.

## **Installation**
There are 2 ways:
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

    public float attack;
    public float stamina;

    //Let user pick Sprite from the folder 'Assets/Sprite' and all the subfolders below.
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

# Future Plans
- **[FromAddressable]** attribute.
- **[FromScene]** attribute.
- **[FromStringList]** attribute.
- Improve Type Safety of the ScriptableGroup concept.
- A way to bind Create functions for creating non-ScriptableObject assets.
- A Popup UGUI to specify the name of the asset upon creation.

Please feel free to Contribute and send me Pull requests.
You can also [**Buy me a coffee!**](https://buymeacoffee.com/phanphantz)☕

**Phun,**\
phun.peeticharoenthum@gmail.com
