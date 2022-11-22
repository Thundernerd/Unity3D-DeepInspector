// using System.Reflection;
// using TNRD.DeepInspector.Contracts;
// using Unity.VisualScripting;
// using UnityEditor;
// using UnityEngine;
//
// namespace TNRD.DeepInspector.Caches
// {
//     public static partial class DrawerCache
//     {
//         internal class UnsupportedDrawer : IDrawer, ITypeDrawer
//         {
//             private readonly string reflectedTypeName;
//             private readonly string memberName;
//             private readonly string typeName;
//
//             public UnsupportedDrawer(MemberInfo memberInfo)
//                 : this(memberInfo.ReflectedType.Name, memberInfo.Name, memberInfo.GetAccessorType().Name)
//             {
//             }
//
//             public UnsupportedDrawer(string reflectedTypeName, string memberName, string typeName)
//             {
//                 this.reflectedTypeName = reflectedTypeName;
//                 this.memberName = memberName;
//                 this.typeName = typeName;
//             }
//
//             /// <inheritdoc />
//             void IDrawer.OnGUI(object instance)
//             {
//                 EditorGUILayout.HelpBox($"No drawer availble for '{reflectedTypeName}.{memberName}' ({typeName})",
//                     MessageType.Warning);
//             }
//
//             /// <inheritdoc />
//             object ITypeDrawer.OnGUI(Rect rect, string label, object value)
//             {
//                 EditorGUILayout.HelpBox($"No drawer availble for '{reflectedTypeName}.{memberName}' ({typeName})",
//                     MessageType.Warning);
//                 return null;
//             }
//
//             /// <inheritdoc />
//             object ITypeDrawer.OnLayoutGUI(string label, object value)
//             {
//                 EditorGUILayout.HelpBox($"No drawer availble for '{reflectedTypeName}.{memberName}' ({typeName})",
//                     MessageType.Warning);
//                 return null;
//             }
//         }
//     }
// }
