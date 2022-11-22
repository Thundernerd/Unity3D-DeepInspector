using System.Reflection;
using TNRD.CustomDrawers;
using UnityEditor;
using UnityEngine;

namespace TNRD.DeepInspector.Drawers
{
    public class PropertyDrawer
    {
        private readonly PropertyInfo property;
        private readonly IDrawer drawer;

        public PropertyDrawer(PropertyInfo property, IDrawer drawer)
        {
            this.property = property;
            this.drawer = drawer;
        }

        public float GetHeight()
        {
            return drawer.GetHeight(true, false);
        }

        public void OnGUI(Rect rect, object instance)
        {
            object value = drawer.OnGUI(rect, property.Name, property.GetValue(instance), false);

            if (property.CanWrite)
            {
                property.SetValue(instance, value);
            }
        }
    }
}
