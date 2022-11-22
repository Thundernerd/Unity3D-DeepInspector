using System;
using System.Collections.Generic;
using System.Reflection;

namespace TNRD.DeepInspector.Caches
{
    public partial class MemberCache
    {
        private static readonly Dictionary<Type, MemberData> typeToMemberData = new Dictionary<Type, MemberData>();

        private const BindingFlags FLAGS = BindingFlags.Instance |
                                           BindingFlags.Static |
                                           BindingFlags.Public |
                                           BindingFlags.NonPublic;

        public static MemberData GetMemberData(Type type)
        {
            if (typeToMemberData.TryGetValue(type, out MemberData memberData))
                return memberData;

            MemberData inheritedMemberData = type.BaseType != null ? GetMemberData(type.BaseType) : null;

            GetFields(type,
                out List<FieldInfo> publicStaticFields,
                out List<FieldInfo> nonPublicStaticFields,
                out List<FieldInfo> publicInstanceFields,
                out List<FieldInfo> nonPublicInstanceFields);

            GetProperties(type,
                out List<PropertyInfo> publicStaticProperties,
                out List<PropertyInfo> nonPublicStaticProperties,
                out List<PropertyInfo> publicInstanceProperties,
                out List<PropertyInfo> nonPublicInstanceProperties);

            memberData = new MemberData(type,
                publicStaticFields,
                nonPublicStaticFields,
                publicInstanceFields,
                nonPublicInstanceFields,
                publicStaticProperties,
                nonPublicStaticProperties,
                publicInstanceProperties,
                nonPublicInstanceProperties,
                inheritedMemberData);

            typeToMemberData.Add(type, memberData);
            return memberData;
        }

        private static void GetFields(
            Type type,
            out List<FieldInfo> publicStaticFields,
            out List<FieldInfo> nonPublicStaticFields,
            out List<FieldInfo> publicInstanceFields,
            out List<FieldInfo> nonPublicInstanceFields
        )
        {
            publicStaticFields = new List<FieldInfo>();
            nonPublicStaticFields = new List<FieldInfo>();
            publicInstanceFields = new List<FieldInfo>();
            nonPublicInstanceFields = new List<FieldInfo>();

            FieldInfo[] fields = type.GetFields(FLAGS);

            foreach (FieldInfo field in fields)
            {
                if (field.IsSpecialName)
                    continue;
                
                if (field.DeclaringType != type)
                    continue;

                if (field.GetCustomAttribute<ObsoleteAttribute>() != null)
                    continue;

                if (field.IsStatic && field.IsPublic)
                {
                    publicStaticFields.Add(field);
                }
                else if (field.IsStatic && !field.IsPublic)
                {
                    nonPublicStaticFields.Add(field);
                }
                else if (!field.IsStatic && field.IsPublic)
                {
                    publicInstanceFields.Add(field);
                }
                else if (!field.IsStatic && !field.IsPublic)
                {
                    nonPublicInstanceFields.Add(field);
                }
            }
        }

        private static void GetProperties(
            Type type,
            out List<PropertyInfo> publicStaticProperties,
            out List<PropertyInfo> nonPublicStaticProperties,
            out List<PropertyInfo> publicInstanceProperties,
            out List<PropertyInfo> nonPublicInstanceProperties
        )
        {
            publicStaticProperties = new List<PropertyInfo>();
            nonPublicStaticProperties = new List<PropertyInfo>();
            publicInstanceProperties = new List<PropertyInfo>();
            nonPublicInstanceProperties = new List<PropertyInfo>();

            PropertyInfo[] properties = type.GetProperties(FLAGS);

            foreach (PropertyInfo property in properties)
            {
                if (property.IsSpecialName)
                    continue;

                if (property.DeclaringType != type)
                    continue;

                if (property.GetIndexParameters().Length > 0)
                    continue;

                bool isStatic = property.IsStatic();
                bool publicGetter = property.GetGetMethod()?.IsPublic ?? false;

                if (!property.CanRead)
                    continue;

                if (property.GetCustomAttribute<ObsoleteAttribute>() != null)
                    continue;

                if (isStatic && publicGetter)
                {
                    publicStaticProperties.Add(property);
                }
                else if (isStatic)
                {
                    nonPublicStaticProperties.Add(property);
                }
                else if (publicGetter)
                {
                    publicInstanceProperties.Add(property);
                }
                else
                {
                    nonPublicInstanceProperties.Add(property);
                }
            }
        }
    }
}
