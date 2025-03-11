using System;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

namespace PhEngine.QuickDropdown
{
    public sealed class ScriptableGroup : ScriptableContainer<Object>
    {
        public override string[] GetStringOptions(Type type)
        {
            return GetOptions(type);
        }

        string[] GetOptions(Type type, List<ScriptableGroup> parentList = null)
        {
            parentList ??= new List<ScriptableGroup>();
            return ElementList
                .SelectMany(so=>
                {
                    if (so == this)
                        return new string[] { };
                    
                    if (so is ScriptableGroup nestedGroup)
                    {
                        if (parentList.Contains(nestedGroup))
                            return new string[] { };

                        var newParentList = new List<ScriptableGroup>(parentList);
                        newParentList.Add(this);
                        return nestedGroup.GetOptions(type, newParentList).ToArray();
                    }
                    return so && so.GetType() == type ? new [] {GetOptionString(so)} : new string[]{};
                })
                .ToArray();

            string GetOptionString(Object obj)
            {
                return parentList.Count <= 0 ? 
                    obj.name : 
                    string.Join("/", parentList.Select(p => p.name)) + "/" + obj.name;
            }
        }

        public override Object GetObjectFromFlatTree(Type type, int targetIndex)
        {
            var currentIndex = 0;
            return FlattenAndReturn(new List<ScriptableGroup>(), type, ref currentIndex, targetIndex);
        }

        Object FlattenAndReturn(List<ScriptableGroup> parentList, Type type, ref int currentIndex, int targetIndex)
        {
            foreach (var element in ElementList)
            {
                if (element == this)
                    continue;
                
                if (element is ScriptableGroup nestedGroup)
                {
                    if (parentList.Contains(nestedGroup))
                        continue;
                    
                    var newParentList = new List<ScriptableGroup>(parentList);
                    newParentList.Add(this);
                    var result = nestedGroup.FlattenAndReturn(newParentList, type, ref currentIndex, targetIndex);
                    if (result != null)
                        return result;
                }
                
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
            return targetObject != null && FlattenAndCheckContains(targetObject, new List<ScriptableGroup>());
        }

        bool FlattenAndCheckContains(Object targetToCheck, List<ScriptableGroup> parentList)
        {
            foreach (var element in ElementList)
            {
                if (element == this)
                    continue;
                
                if (element is ScriptableGroup nestedGroup)
                {
                    if (parentList.Contains(nestedGroup))
                        continue;
                
                    var newParentList = new List<ScriptableGroup>(parentList);
                    newParentList.Add(this);
                    if (nestedGroup.FlattenAndCheckContains(targetToCheck, newParentList))
                        return true;
                }

                if (element == targetToCheck)
                    return true;
            }
            return false;
        }
    }
}