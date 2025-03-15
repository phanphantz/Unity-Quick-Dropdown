using System;
using System.Reflection;
using UnityEditor;

namespace PhEngine.QuickDropdown.Editor
{
    public static class FieldUtils
    {
        public static Type GetFieldType(SerializedProperty property)
        {
            var targetObject = property.serializedObject.targetObject;
            var targetType = targetObject.GetType();
            return GetFieldViaPath(targetType, property.propertyPath)?.FieldType;
        }

        /// <summary>
        /// Taken from: https://discussions.unity.com/t/a-smarter-way-to-get-the-type-of-serializedproperty/186674/6
        /// </summary>
        /// <param name="type"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        static FieldInfo GetFieldViaPath(Type type, string path)
        {
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var parent = type;
            var fi = parent.GetField(path, flags);
            var paths = path.Split('.');
            for (int i = 0; i < paths.Length; i++)
            {
                fi = parent?.GetField(paths[i], flags);
                if (fi != null)
                {
                    // there are only two container field type that can be serialized:
                    // Array and List<T>
                    if (fi.FieldType.IsArray)
                    {
                        parent = fi.FieldType.GetElementType();
                        i += 2;
                        continue;
                    }

                    if (fi.FieldType.IsGenericType)
                    {
                        parent = fi.FieldType.GetGenericArguments()[0];
                        i += 2;
                        continue;
                    }

                    parent = fi.FieldType;
                }
                else
                {
                    break;
                }
            }

            if (fi == null)
                return type.BaseType != null ? GetFieldViaPath(type.BaseType, path) : null;

            return fi;
        }
    }
}