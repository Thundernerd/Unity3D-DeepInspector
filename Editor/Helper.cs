using UnityEditor;
using UnityEngine;

namespace TNRD.DeepInspector
{
    public class Helper
    {
        [MenuItem("Window/GetRidOfWindows")]
        private static void Lel()
        {
            var windows = Resources.FindObjectsOfTypeAll<DeepInspectorWindow>();
            for (int i = windows.Length - 1; i >= 0; i--)
            {
                DeepInspectorWindow deepInspectorWindow = windows[i];
                Object.DestroyImmediate(deepInspectorWindow);
            }
        }
    }
}
