using System.Linq;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

namespace PhEngine.QuickDropdown.Editor.Addressables
{
    public static class AddressableUtils
    {
        static AddressableAssetSettings GetDefaultAddressableAssetSettings()
        {
            if (unsafeSettings == null)
                unsafeSettings = AddressableAssetSettingsDefaultObject.Settings;

            return unsafeSettings;
        }

        static AddressableAssetSettings unsafeSettings;

        public static AddressableAssetEntry[] GetAllEntriesFromGroups(string groupName)
        {
            return GetDefaultAddressableAssetSettings().groups
                .Where(g => g.name == groupName)
                .SelectMany(g => g.entries)
                .ToArray();
        }
    }
}