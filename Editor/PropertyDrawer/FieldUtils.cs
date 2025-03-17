using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace PhEngine.QuickDropdown.Editor
{
    public static class FieldUtils
    {
        public static Type GetFlatFieldType(SerializedProperty property)
        {
            var targetObject = property.serializedObject.targetObject;
            var targetType = targetObject.GetType();
            return ToFlatType(GetFieldViaPath(targetType, property.propertyPath)?.FieldType);
        }

        public static bool IsTypeOrCollectionOfType<T>(Type type)
        {
            return type == typeof(T) || type.IsSubclassOf(typeof(T)) || IsArrayOrListOf<T>(type);
        }

        static Type ToFlatType(Type type)
        {
            if (type.IsArray)
                return type.GetElementType();

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
                return type.GetGenericArguments()[0];

            return type;
        }
        
        static bool IsArrayOrListOf<T>(Type type)
        {
            if (type.IsArray && type.GetElementType()!.IsSubclassOf(typeof(T)))
                return true;

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                var elementType = type.GetGenericArguments()[0];
                return elementType.IsSubclassOf(typeof(T));
            }

            return false;
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