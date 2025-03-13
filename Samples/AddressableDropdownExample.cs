#if ADDRESSABLES_DROPDOWN
using PhEngine.QuickDropdown.Addressables;
using UnityEngine;

public class AddressableDropdownExample : MonoBehaviour
{
    [FromAddressable("PackedAddressableGroup"), SerializeField]
    ElementConfig addressableConfig;
    
    [FromAddressable("PackedAddressableGroup"), SerializeField]
    string addressableAddress;
}
#endif