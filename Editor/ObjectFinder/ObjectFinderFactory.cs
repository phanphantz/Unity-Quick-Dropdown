using System;
using System.Collections.Generic;
using UnityEngine;

namespace PhEngine.QuickDropdown.Editor
{
    public static class ObjectFinderFactory
    {
        static Dictionary<Type, Dictionary<string, ObjectFinder>> CachedFinders = new Dictionary<Type, Dictionary<string, ObjectFinder>>();
        public static ObjectFinder GetFinder(DropdownField field, Type type)
        {
            if (!CachedFinders.TryGetValue(type, out var finderDict))
            {
                var newFinder = CreateNewFinder(field, type);
                
                finderDict = new Dictionary<string, ObjectFinder> { { field.Path, newFinder } };
                CachedFinders.TryAdd(type, finderDict);
                
                return newFinder;
            }

            if (!finderDict.TryGetValue(field.Path, out var finder))
            {
                finder = CreateNewFinder(field, type);
                finderDict.TryAdd(field.Path, finder);
            }
            return finder;
        }

        static ObjectFinder CreateNewFinder(DropdownField field, Type type)
        {
#if QDD_DEDUG
            Debug.Log("Create new Finder: " + field.GetType().Name + " for type: " + type.Name);
#endif
            switch (field)
            {
                case FromConfig:
                    return new ConfigFinder(field, type);
                case FromFolder:
                    return new FolderObjectFinder(field, type);
                case FromGroup:
                    return new ScriptableGroupFinder(field, type);
#if ADDRESSABLES_DROPDOWN
                case PhEngine.QuickDropdown.Addressables.FromAddressable:
                    return new Addressables.AddressableEntryFinder(field, type);
#endif
                default:
                    throw new NotImplementedException($"Don't know how to get finder for {type}");
            }
        }

        public static void Dispose()
        {
#if QDD_DEDUG
            Debug.Log("Disposed all cached finders");
#endif
            CachedFinders = new Dictionary<Type, Dictionary<string, ObjectFinder>>();
        }
    }
}