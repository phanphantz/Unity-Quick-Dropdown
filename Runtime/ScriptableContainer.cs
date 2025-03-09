using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PhEngine.QuickDropdown
{
    public abstract class ScriptableContainer : ScriptableObject
    {
        public abstract string[] GetStringOptions(Type type, string prefix = "");
        public abstract bool ContainsObject(Object obj);
        public abstract void AddObject(Object obj);
        public abstract Object GetObjectFromFlatTree(Type type, int index);
    }
    
    public abstract class ScriptableContainer<T> : ScriptableContainer where T : Object
    {
        public IReadOnlyList<T> ElementList => elementList.AsReadOnly();
        [SerializeField] List<T> elementList = new List<T>();
        
        public override void AddObject(Object element)
        {
            elementList.Add(element as T);
        }
        
        public override string[] GetStringOptions(Type type, string prefix = "")
        {
            return elementList
                .SelectMany(so=>
                {
                    if (so is ScriptableContainer nestedGroup)
                        return nestedGroup.GetStringOptions(type, prefix + nestedGroup.name + "/").ToArray();
                    
                    return so && so.GetType() == type ? new[] { prefix + so.name } : new string[]{};
                })
                .ToArray();
        }

        public override Object GetObjectFromFlatTree(Type type, int index)
        {
            var currentIndex = 0;
            foreach (var e in elementList)
            {
                var result = FlattenAndReturn(type, index, e, ref currentIndex);
                if (result) 
                    return result;
            }
            return null;
        }

        static T FlattenAndReturn(Type type, int targetIndex, T element, ref int currentIndex)
        {
            var isTypeMatched = element && element.GetType() == type;
            if (element is ScriptableContainer<T> nestedGroup)
            {
                foreach (var nestedElement in nestedGroup.elementList)
                {
                    if (currentIndex == targetIndex)
                        return FlattenAndReturn(type, targetIndex, nestedElement, ref currentIndex);
                  
                    if (nestedElement && nestedElement.GetType() == type)
                        currentIndex++;
                }
            }
            else if (currentIndex == targetIndex && isTypeMatched)
            {
                return element;
            }
            else if (isTypeMatched)
                currentIndex++;
            
            return null;
        }

        public override bool ContainsObject(Object obj)
        {
            var targetObject = obj as T;
            if (targetObject == null)
                return false;
            
            foreach (var e in elementList)
            {
                var result = FlattenAndCheckContains(e, targetObject);
                if (result)
                    return true;
            }
            return false;
        }

        bool FlattenAndCheckContains(T element, T targetToCheck)
        {
            if (element is ScriptableContainer<T> nestedGroup)
                return nestedGroup.ContainsObject(targetToCheck);
            
            return element == targetToCheck;
        }
    }
}