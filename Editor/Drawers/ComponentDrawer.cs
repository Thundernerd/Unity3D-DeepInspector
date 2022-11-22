using System;
using TNRD.CustomDrawers;
using TNRD.DeepInspector.RectEx;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TNRD.DeepInspector.Drawers
{
    public class ComponentDrawer : IDrawer
    {
        /// <inheritdoc />
        float IDrawer.GetHeight(bool hasLabel, bool compact)
        {
            return EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing;
        }

        /// <inheritdoc />
        object IDrawer.OnGUI(Rect rect, string label, object instance, bool compact)
        {
            Component component = (Component)instance;
            Type type = typeof(Component);
            
            Rect[] rects = rect.Column(2);
            EditorGUI.LabelField(rects[0], label);
            
            using (new EditorGUI.IndentLevelScope())
            {
                Rect[] cutFromRight = rects[1].CutFromRight(25, 4);
            
                Object obj = EditorGUI.ObjectField(cutFromRight[0], component, type, true);
                EditorGUI.BeginDisabledGroup(obj == null);
                if (GUI.Button(cutFromRight[1], EditorGUIUtility.IconContent("Search On Icon").image))
                {
                    DeepInspectorWindow.Inspect(obj);
                }
                EditorGUI.EndDisabledGroup();
            
                return obj;
            }
        }

        /// <inheritdoc />
        object IDrawer.OnGUI(string label, object instance, bool compact)
        {
            throw new NotImplementedException();
        }
    }
}
