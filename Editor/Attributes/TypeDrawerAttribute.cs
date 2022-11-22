using System;
using JetBrains.Annotations;

namespace TNRD.DeepInspector.Attributes
{
    [MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class TypeDrawerAttribute : Attribute
    {
        public Type Type { get; }
        
        public TypeDrawerAttribute(Type type)
        {
            Type = type;
        }
    }
}
