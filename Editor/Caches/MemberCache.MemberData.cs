using System;
using System.Collections.Generic;
using System.Reflection;

namespace TNRD.DeepInspector.Caches
{
    public partial class MemberCache
    {
        public class MemberData
        {
            public MemberData(
                Type type,
                IReadOnlyList<FieldInfo> publicStaticFields,
                IReadOnlyList<FieldInfo> nonPublicStaticFields,
                IReadOnlyList<FieldInfo> publicInstanceFields,
                IReadOnlyList<FieldInfo> nonPublicInstanceFields,
                IReadOnlyList<PropertyInfo> publicStaticProperties,
                IReadOnlyList<PropertyInfo> nonPublicStaticProperties,
                IReadOnlyList<PropertyInfo> publicInstanceProperties,
                IReadOnlyList<PropertyInfo> nonPublicInstanceProperties,
                MemberData inheritedMemberData
            )
            {
                Type = type;
                PublicStaticFields = publicStaticFields;
                NonPublicStaticFields = nonPublicStaticFields;
                PublicInstanceFields = publicInstanceFields;
                NonPublicInstanceFields = nonPublicInstanceFields;
                PublicStaticProperties = publicStaticProperties;
                NonPublicStaticProperties = nonPublicStaticProperties;
                PublicInstanceProperties = publicInstanceProperties;
                NonPublicInstanceProperties = nonPublicInstanceProperties;
                InheritedMemberData = inheritedMemberData;
            }

            public Type Type { get; }
            public IReadOnlyList<FieldInfo> PublicStaticFields { get; }
            public IReadOnlyList<FieldInfo> NonPublicStaticFields { get; }
            public IReadOnlyList<FieldInfo> PublicInstanceFields { get; }
            public IReadOnlyList<FieldInfo> NonPublicInstanceFields { get; }
            public IReadOnlyList<PropertyInfo> PublicStaticProperties { get; }
            public IReadOnlyList<PropertyInfo> NonPublicStaticProperties { get; }
            public IReadOnlyList<PropertyInfo> PublicInstanceProperties { get; }
            public IReadOnlyList<PropertyInfo> NonPublicInstanceProperties { get; }
            public MemberData InheritedMemberData { get; }
        }
    }
}
