using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PhEngine.QuickDropdown
{
    public abstract class ScriptableContainer : ScriptableObject
    {
        public abstract string[] GetStringOptions(Type type);
        public abstract bool ContainsObject(Object targetObject);
        public abstract void AddObject(Object obj);
        public abstract Object GetObjectFromFlatTree(Type type, int targetIndex);
    }
    
    public abstract class ScriptableContainer<T> : ScriptableContainer where T : Object
    {
        public IReadOnlyList<T> ElementList => elementList.AsReadOnly();
        [SerializeField] List<T> elementList = new List<T>();
        
        public override void AddObject(Object element)
        {
            elementList.Add(element as T);
        }

        public override string[] GetStringOptions(Type type)
        {
            return ElementList
                .Where(so => so && so.GetType() == type)
                .Select(so => so.name)
                .ToArray();
        }
        
        public override Object GetObjectFromFlatTree(Type type, int targetIndex)
        {
            var currentIndex = 0;
            foreach (var element in ElementList)
            {
                var isTypeMatched = element && element.GetType() == type;
                if (currentIndex == targetIndex && isTypeMatched)
                    return element;

                if (isTypeMatched)
                    currentIndex++;
            }
            return null;
        }
        
        public override bool ContainsObject(Object targetObject)
        {
            return ElementList.Contains(targetObject);
        }
    }
}