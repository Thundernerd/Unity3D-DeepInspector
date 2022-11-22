using System.Linq;
using System.Reflection;

namespace TNRD.DeepInspector
{
    public static class Extensions
    {
        public static bool IsStatic(this PropertyInfo propertyInfo)
        {
            return propertyInfo.GetAccessors(true).Any(x => x.IsStatic);
        }
    }
}
