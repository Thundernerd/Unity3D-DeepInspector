// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Reflection;
// using TNRD.DeepInspector.Attributes;
// using TNRD.DeepInspector.Contracts;
// using TNRD.DeepInspector.Drawers;
// using UnityEditor;
// using UnityEngine;
// using PropertyDrawer = TNRD.DeepInspector.Drawers.PropertyDrawer;
//
// namespace TNRD.DeepInspector.Caches
// {
//     public static partial class DrawerCache
//     {
//         private static readonly Dictionary<Type, ITypeDrawer> typeToTypeDrawer =
//             new Dictionary<Type, ITypeDrawer>();
//
//         static DrawerCache()
//         {
//             TypeCache.TypeCollection fieldDrawerTypes = TypeCache.GetTypesWithAttribute<TypeDrawerAttribute>();
//             foreach (Type type in fieldDrawerTypes)
//             {
//                 TypeDrawerAttribute typeDrawerAttribute = type.GetCustomAttribute<TypeDrawerAttribute>();
//                 typeToTypeDrawer.Add(typeDrawerAttribute.Type, (ITypeDrawer)Activator.CreateInstance(type));
//             }
//         }
//
//         public static IDrawer GetDrawer(FieldInfo fieldInfo)
//         {
//             if (TryGetDrawer(fieldInfo.FieldType, out ITypeDrawer typeDrawer))
//                 return new FieldDrawer(fieldInfo, typeDrawer);
//
//             if (fieldInfo.FieldType.IsArray)
//                 return new ArrayDrawer(fieldInfo.FieldType.GetElementType(), fieldInfo);
//
//             if (typeof(IList).IsAssignableFrom(fieldInfo.FieldType))
//                 return new ListDrawer(fieldInfo.FieldType.GetGenericArguments()[0], fieldInfo);
//
//             if (typeof(IDictionary).IsAssignableFrom(fieldInfo.FieldType))
//             {
//                 return new DictionaryDrawer(fieldInfo.FieldType.GetGenericArguments()[0],
//                     fieldInfo.FieldType.GetGenericArguments()[1],
//                     fieldInfo);
//             }
//
//             return new UnsupportedDrawer(fieldInfo);
//         }
//
//         public static IDrawer GetDrawer(PropertyInfo propertyInfo)
//         {
//             if (TryGetDrawer(propertyInfo.PropertyType, out ITypeDrawer typeDrawer))
//                 return new PropertyDrawer(propertyInfo, typeDrawer);
//
//             if (propertyInfo.PropertyType.IsArray)
//                 return new ArrayDrawer(propertyInfo.PropertyType.GetElementType(), propertyInfo);
//
//             if (typeof(IList).IsAssignableFrom(propertyInfo.PropertyType))
//                 return new ListDrawer(propertyInfo.PropertyType.GetGenericArguments()[0], propertyInfo);
//
//             if (typeof(IDictionary).IsAssignableFrom(propertyInfo.PropertyType))
//             {
//                 return new DictionaryDrawer(propertyInfo.PropertyType.GetGenericArguments()[0],
//                     propertyInfo.PropertyType.GetGenericArguments()[1],
//                     propertyInfo);
//             }
//
//             return new UnsupportedDrawer(propertyInfo);
//         }
//
//         internal static bool TryGetDrawer(Type type, out ITypeDrawer drawer)
//         {
//             if (typeToTypeDrawer.TryGetValue(type, out drawer))
//                 return true;
//
//             if (type.IsEnum)
//             {
//                 drawer = typeToTypeDrawer[typeof(Enum)];
//                 return true;
//             }
//
//             if (type.IsSubclassOf(typeof(Component)))
//             {
//                 drawer = typeToTypeDrawer[typeof(Component)];
//                 return true;
//             }
//
//             return false;
//         }
//     }
// }
