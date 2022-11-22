using System;
using TNRD.CustomDrawers;
using TNRD.DeepInspector.RectEx;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TNRD.DeepInspector.Drawers
{
    public class GameObjectDrawer : IDrawer
    {
        /// <inheritdoc />
        float IDrawer.GetHeight(bool hasLabel, bool compact)
        {
            return EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing;
        }

        object IDrawer.OnGUI(Rect rect, string label, object instance, bool compact)
        {
            GameObject gameObject = (GameObject)instance;
            Type type = typeof(GameObject);
            
            Rect[] rects = rect.Column(2);
            EditorGUI.LabelField(rects[0], label);
            
            using (new EditorGUI.IndentLevelScope())
            {
                Rect[] cutFromRight = rects[1].CutFromRight(25, 4);
            
                Object gObj = EditorGUI.ObjectField(cutFromRight[0], gameObject, type, true);
                EditorGUI.BeginDisabledGroup(gObj == null);
                if (GUI.Button(cutFromRight[1], EditorGUIUtility.IconContent("Search On Icon").image))
                {
                    DeepInspectorWindow.Inspect(gObj);
                }
            
                EditorGUI.EndDisabledGroup();
            
                return gObj;
            }
        }

        /// <inheritdoc />
        object IDrawer.OnGUI(string label, object instance, bool compact)
        {
            throw new NotImplementedException();
        }
    }
}
