using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PhEngine.QuickDropdown.Editor
{
    public abstract class ObjectFinder
    {
        protected string ObjectPath { get; }
        protected DropdownField Field { get; }
        protected Type Type { get; }

        protected Object CachedSource { get; private set; }

        bool wasSearchPerformed;
        
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

        public bool CheckAndPrepareSource()
        {
            if (CachedSource)
            {
                if (IsPathMatched())
                    return true;

                Debug.LogWarning($"The old source '{ObjectPath}' was renamed. Either Rename it back, Change the Path in code, or Click 'Fix' to create new source with the correct name.");
                CachedSource = null;
                return false;
            }

            if (wasSearchPerformed)
                return false;

            SearchAndCacheSource();
            wasSearchPerformed = true;
            return CachedSource;
        }

        protected virtual bool IsPathMatched()
        {
            return CachedSource.name == ObjectPath;
        }

        public void SearchAndCacheSource()
        {
#if QDD_DEDUG
            Debug.Log(GetType().Name + " Perform Searching: " + ObjectPath);
#endif
            CachedSource = SearchForSource();
        }

        protected abstract Object SearchForSource();
        public void CreateOrGetSourceFromInspector()
        {
            SearchAndCacheSource();
            if (CachedSource)
            {
                Debug.Log($"[{GetType().Name}] Found an existing source with name '{CachedSource.name}'");
                return;
            }
            
            PrepareSource();
            Debug.Log($"[{GetType().Name}] Created a new source with name '{CachedSource.name}'");
        }

        protected void PrepareSource()
        {
            CachedSource = CreateNewSource();
        }

        protected abstract Object CreateNewSource();
    }
}