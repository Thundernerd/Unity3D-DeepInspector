using System.Reflection;
using TNRD.CustomDrawers;
using UnityEditor;
using UnityEngine;

namespace TNRD.DeepInspector.Drawers
{
    public class FieldDrawer
    {
        private readonly FieldInfo field;
        private readonly IDrawer drawer;

        public FieldDrawer(FieldInfo field, IDrawer drawer)
        {
            this.field = field;
            this.drawer = drawer;
        }

        public float GetHeight()
        {
            return drawer.GetHeight(true, false);
        }

        public void OnGUI(Rect rect, object instance)
        {
            object value = drawer.OnGUI(rect, field.Name, field.GetValue(instance), false);

            if (!field.IsLiteral && !field.IsInitOnly)
            {
                field.SetValue(instance, value);
            }
        }
    }
}
