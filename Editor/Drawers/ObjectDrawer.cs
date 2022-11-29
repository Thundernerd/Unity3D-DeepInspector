using TNRD.CustomDrawers;
using TNRD.DeepInspector.RectEx;
using UnityEditor;
using UnityEngine;

namespace TNRD.DeepInspector.Drawers
{
    public class ObjectDrawer : IDrawer
    {
        /// <inheritdoc />
        float IDrawer.GetHeight(bool hasLabel, bool compact)
        {
            return EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing;
        }

        /// <inheritdoc />
        object IDrawer.OnGUI(Rect rect, string label, object instance, bool compact)
        {
            Rect[] rects = rect.Column(2);
            EditorGUI.LabelField(rects[0], label);
            
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUI.BeginDisabledGroup(instance == null);
                if (GUI.Button(EditorGUI.IndentedRect(rects[1]), EditorGUIUtility.IconContent("Search On Icon").image))
                {
                    DeepInspectorWindow.Inspect(instance);
                }
                EditorGUI.EndDisabledGroup();
            }
            
            return instance;
        }

        /// <inheritdoc />
        object IDrawer.OnGUI(string label, object instance, bool compact)
        {
            return instance;
        }
    }
}
