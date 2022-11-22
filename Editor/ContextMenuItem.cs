using UnityEditor;
using UnityEngine;

namespace TNRD.DeepInspector
{
    public class ContextMenuItem
    {
        [MenuItem("CONTEXT/Object/Deep Inspect")]
        public static void DeepInspect(MenuCommand menuCommand)
        {
            DeepInspectorWindow.Inspect(menuCommand.context);
        }

        [MenuItem("CONTEXT/Component/Deep Inspect", true)]
        public static bool ValidateDeepInspect(MenuCommand menuCommand)
        {
            return menuCommand.context is Object;
        }
    }
}
