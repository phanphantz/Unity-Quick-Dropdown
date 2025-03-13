using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PhEngine.QuickDropdown.Editor
{
    public abstract class ObjectFinder
    {
        public string ObjectPath { get; }
        public DropdownField Field { get; }
        public Type Type { get; }

        protected ObjectFinder(DropdownField field, Type type)
        {
            ObjectPath = field.Path;
            Field = field;
            Type = type;
        }

        public virtual bool IsTypeSupported(Type type)
        {
            return type.IsSubclassOf(typeof(Object));
        }
        
        public abstract string[] SearchForItems();
        public abstract Object GetResultAtIndex(int index);
        public abstract void SelectAndPingSource();
        public abstract void CreateNewScriptableObject();
        public abstract Texture GetSourceIcon();
        public abstract bool IsBelongToSource(object currentObject);
        public abstract bool CheckAndPrepareSource();
        public abstract void CreateSourceIfNotExists();
    }
}