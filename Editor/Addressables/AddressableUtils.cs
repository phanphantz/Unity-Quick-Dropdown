#if ADDRESSABLES_DROPDOWN
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;

namespace PhEngine.QuickDropdown.Editor.Addressables
{
    public static class AddressableUtils
    {
        static AddressableAssetSettings GetDefaultAddressableAssetSettings()
        {
            if (unsafeSettings == null)
                unsafeSettings = AddressableAssetSettingsDefaultObject.GetSettings(true);
            
            return unsafeSettings;
        }

        static AddressableAssetSettings unsafeSettings;

        public static AddressableAssetGroup GetFirstFoundGroup(string groupName)
        {
            return GetDefaultAddressableAssetSettings().FindGroup(groupName);
        }
        
        public static AddressableAssetGroup GetOrCreateGroup(string groupName)
        {
            var existingGroup = GetFirstFoundGroup(groupName);
            if (existingGroup != null)
                return existingGroup;
            
            var settings = GetDefaultAddressableAssetSettings(); 
            Undo.IncrementCurrentGroup();
            var id = Undo.GetCurrentGroup();
            Undo.RegisterCompleteObjectUndo(settings, "Create " + groupName);
            var targetGroup = settings.CreateGroup(groupName, false, false, true, new List<AddressableAssetGroupSchema>());
            Debug.Log($"Created new addressable group: {targetGroup.name}");
            Undo.RegisterCreatedObjectUndo(targetGroup, "Create " + groupName);
            
            targetGroup.AddSchema<BundledAssetGroupSchema>();
            targetGroup.AddSchema<ContentUpdateGroupSchema>();
            EditorUtility.SetDirty(targetGroup);
            EditorUtility.SetDirty(settings);
            
            Undo.CollapseUndoOperations(id);
            return targetGroup;
        }
        
        public static void AddToAddressableGroup(string guid, string groupName, string address)
        {
            var targetGroup = GetOrCreateGroup(groupName);
            var settings = GetDefaultAddressableAssetSettings();
            Undo.RegisterCompleteObjectUndo(settings, "Create Entry");
            settings.CreateOrMoveEntry(guid, targetGroup, false, false);
            if (targetGroup == null)
                throw new InvalidOperationException("Unable to get or create the group: " + groupName);
            
            targetGroup.GetAssetEntry(guid)?.SetAddress(address);
        }
    }
}
#endif