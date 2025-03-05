using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PhEngine.QuickDropdown
{
    public class ScriptableGroup : ScriptableGroup<Object>
    {
    }
    
    public abstract class ScriptableGroup<T> : ScriptableObject where T : Object
    {
        public IReadOnlyList<T> ElementList => elementList.AsReadOnly();
        [SerializeField] List<T> elementList = new List<T>();
        
        public void Add(T element)
        {
            elementList.Add(element);
        }
        
        public string[] GetStringOptions(Type type, string prefix = "")
        {
            return elementList
                .SelectMany(so=>
                {
                    if (so is ScriptableGroup nestedGroup)
                        return nestedGroup.GetStringOptions(type, prefix + nestedGroup.name + "/").ToArray();
                    
                    return so && so.GetType() == type ? new[] { prefix + so.name } : new string[]{};
                })
                .ToArray();
        }

        public T GetElementFromFlatTree(Type type, int index)
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
            if (element is ScriptableGroup<T> nestedGroup)
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

        public bool Contains(T targetObject)
        {
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
            if (element is ScriptableGroup<T> nestedGroup)
                return nestedGroup.Contains(targetToCheck);
            
            return element == targetToCheck;
        }
    }
}