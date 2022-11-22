using TNRD.CustomDrawers;
using TNRD.DeepInspector.Drawers;
using UnityEditor;
using UnityEngine;

namespace TNRD.DeepInspector
{
    public class DrawerRegister
    {
        [InitializeOnLoadMethod]
        private static void Init()
        {
            DrawerFactory.OverrideDrawer(typeof(Component), typeof(ComponentDrawer), true);
            DrawerFactory.OverrideDrawer(typeof(GameObject), typeof(GameObjectDrawer), false);
            DrawerFactory.RegisterDrawer<object, ObjectDrawer>(true);
        }
    }
}
