using System;

namespace PhEngine.QuickDropdown.Editor
{
    public static class ObjectFinderFactory
    {
        public static ObjectFinder CreateFinder(DropdownField field, Type type)
        {
            return field switch
            {
                FromConfig => new ConfigFinder(field, type),
                FromFolder => new FolderObjectFinder(field, type),
                FromGroup => new ScriptableGroupFinder(field, type),
#if ADDRESSABLES_DROPDOWN
                PhEngine.QuickDropdown.Addressables.FromAddressable => new Addressables.AddressableEntryFinder(field,
                    type),
#endif
                _ => throw new NotImplementedException($"Don't know how to get finder for {type}")
            };
        }
    }
}