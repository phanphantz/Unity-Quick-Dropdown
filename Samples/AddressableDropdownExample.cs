#if ADDRESSABLES_DROPDOWN
using System.Collections.Generic;
using PhEngine.QuickDropdown.Addressables;
using UnityEngine;

public class AddressableDropdownExample : MonoBehaviour
{
    [FromAddressable("PackedAddressableGroup"), SerializeField]
    ElementConfig addressableConfig;
    
    [FromAddressable("PackedAddressableGroup"), SerializeField]
    string addressableAddress;
    
    [FromAddressable("PackedAddressableGroup"), SerializeField]
    List<string> addressList = new List<string>();
}
#endif