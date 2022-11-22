using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TNRD.CustomDrawers;
using TNRD.DeepInspector.Caches;
using TNRD.DeepInspector.Drawers;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using PropertyDrawer = TNRD.DeepInspector.Drawers.PropertyDrawer;

namespace TNRD.DeepInspector
{
    public class DeepInspectorWindow : EditorWindow, ISerializationCallbackReceiver
    {
        public static void Inspect(object instance)
        {
            DeepInspectorWindow wnd = CreateInstance<DeepInspectorWindow>();
            wnd.titleContent = EditorGUIUtility.IconContent("Search On Icon");
            wnd.titleContent.text = "Deep Inspector";
            // wnd.wantsMouseMove = true;
            wnd.wantsLessLayoutEvents = true;
            wnd.wantsMouseEnterLeaveWindow = true;
            wnd.minSize = new Vector2(MIN_WIDTH, 720);
            wnd.needsInitialization = true;

            if (instance is Object obj)
            {
                wnd.unityInstance = obj;
                wnd.rawInstance = null;
            }
            else
            {
                wnd.rawInstance = instance;
                wnd.unityInstance = null;
            }

            wnd.Initialize();
            wnd.Show();
        }

        private class FoldCache
        {
            public bool Inherited;

            public bool Fields;
            public bool Properties;
            public bool Methods;

            public bool PublicStaticFields;
            public bool NonPublicStaticFields;
            public bool PublicInstanceFields;
            public bool NonPublicSerializedInstanceFields;
            public bool NonPublicNonSerializedInstanceFields;

            public bool PublicStaticProperties;
            public bool NonPublicStaticProperties;
            public bool PublicInstanceProperties;
            public bool NonPublicInstanceProperties;
        }

        private class DropdownMemberData
        {
            public MemberCache.MemberData MemberData;
            public GUIContent DisplayName;
        }

        private class Styles
        {
            private readonly Color dividerColor = new Color(0, 0, 0, .75f);

            private readonly Color toolbarBorderColor = EditorGUIUtility.isProSkin
                ? new Color32(35, 35, 35, 255)
                : new Color32(153, 153, 153, 255);

            private readonly Color rowColor = new Color(0, 0, 0, 0);

            private readonly Color altRowColor = EditorGUIUtility.isProSkin
                ? new Color(0, 0, 0, 0.15f)
                : new Color(1, 1, 1, 0.15f);

            private readonly GUIStyle headerStyle = new GUIStyle("SettingsHeader")
            {
                stretchHeight = true,
                clipping = TextClipping.Overflow
            };

            private readonly GUIStyle memberInfoStyle = new GUIStyle(GUIStyle.none)
            {
                margin = new RectOffset(),
                padding = new RectOffset(0, 0, 0, 8)
            };

            private readonly GUIStyle compactMemberInfoStyle = new GUIStyle(GUIStyle.none)
            {
                margin = new RectOffset(),
                padding = new RectOffset(0, 0, 0, 3)
            };

            public Color DividerColor => dividerColor;
            public Color ToolbarBorderColor => toolbarBorderColor;
            public Color RowColor => rowColor;
            public Color AltRowColor => altRowColor;
            public GUIStyle HeaderStyle => headerStyle;
            public GUIStyle MemberInfoStyle => memberInfoStyle;
            public GUIStyle CompactMemberInfoStyle => compactMemberInfoStyle;
        }

        private const float MIN_WIDTH = 1280;
        private const float MIN_SECTION_WIDTH = MIN_WIDTH / 3;
        private const float TOOLBAR_OFFSET = 21f; // Taken by looking into IMGUI Debugger
        private const float DIVIDER_THICKNESS = 1f;

        private readonly Dictionary<MemberCache.MemberData, FoldCache> memberDataToFoldCache =
            new Dictionary<MemberCache.MemberData, FoldCache>();

        private readonly Dictionary<FieldInfo, FieldDrawer> fieldToDrawer =
            new Dictionary<FieldInfo, FieldDrawer>();

        private readonly Dictionary<PropertyInfo, PropertyDrawer> propertyToDrawer =
            new Dictionary<PropertyInfo, PropertyDrawer>();

        private Rect DividerRect => new Rect(dividerOffset,
            TOOLBAR_OFFSET,
            DIVIDER_THICKNESS,
            position.height - TOOLBAR_OFFSET);

        private Rect DividerResizeRect => isDraggingDivider
            ? new Rect(0, 0, position.width, position.height)
            : new Rect(dividerOffset - 5,
                TOOLBAR_OFFSET,
                DIVIDER_THICKNESS + 10,
                position.height - TOOLBAR_OFFSET);

        [SerializeField] private Object unityInstance;
        [SerializeReference] private object rawInstance;

        private object instance => rawInstance ?? unityInstance;

        private Styles styles;
        
        private float dividerOffset;
        private bool isDraggingDivider;

        private DropdownMemberData selectedDropdownMemberData;
        private DropdownMemberData[] dropdownMemberDatas;

        private Vector2 fieldsScroll;
        private Vector2 propertiesScroll;

        [NonSerialized] private bool needsInitialization;
        [NonSerialized] private bool isInitialized;

        private void OnEnable()
        {
            if (needsInitialization)
            {
                Initialize();
            }
        }

        private void Initialize()
        {
            if (!needsInitialization)
                return;

            if (instance == null)
                return;

            // TODO: Improve this part, it feels nasty
            MemberCache.MemberData memberData = MemberCache.GetMemberData(this.instance.GetType());
            List<DropdownMemberData> allMemberDatas = new List<DropdownMemberData>();

            while (memberData != null)
            {
                string displayName = instance is Object obj
                    ? $"{obj.name} ({memberData.Type.Name})"
                    : memberData.Type.Name;

                allMemberDatas.Add(new DropdownMemberData()
                {
                    DisplayName = new GUIContent(displayName, AssetPreview.GetMiniTypeThumbnail(memberData.Type)),
                    MemberData = memberData
                });

                memberData = memberData.InheritedMemberData;
            }

            selectedDropdownMemberData = allMemberDatas.First();
            dropdownMemberDatas = allMemberDatas.ToArray();

            dividerOffset = MIN_WIDTH / 2f;

            needsInitialization = false;
            isInitialized = true;
        }

        private void OnGUI()
        {
            if (needsInitialization)
                Initialize();
            
            if (!isInitialized)
                return;

            if (styles == null)
                styles = new Styles();

            // return;
            // TODO: Only do this when switching item
            if (!memberDataToFoldCache.ContainsKey(selectedDropdownMemberData.MemberData))
                memberDataToFoldCache.Add(selectedDropdownMemberData.MemberData, new FoldCache());

            DrawToolbar();
            DrawFirstArea();
            DrawSecondArea();
            DrawDivider();
            HandleDividerDragging();
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            if (EditorGUILayout.DropdownButton(selectedDropdownMemberData.DisplayName,
                    FocusType.Passive,
                    EditorStyles.toolbarDropDown))
            {
                GenericMenu gm = new GenericMenu();
                foreach (DropdownMemberData dropdownMemberData in dropdownMemberDatas)
                {
                    gm.AddItem(dropdownMemberData.DisplayName,
                        dropdownMemberData == selectedDropdownMemberData,
                        () => { selectedDropdownMemberData = dropdownMemberData; });
                }

                gm.ShowAsContext();
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawFirstArea()
        {
            GUILayout.BeginArea(new Rect(0, TOOLBAR_OFFSET, dividerOffset, position.height - TOOLBAR_OFFSET));
            DrawFields(selectedDropdownMemberData.MemberData);
            GUILayout.EndArea();
        }

        private void DrawSecondArea()
        {
            GUILayout.BeginArea(new Rect(dividerOffset,
                TOOLBAR_OFFSET,
                position.width - dividerOffset,
                position.height - TOOLBAR_OFFSET));
            DrawProperties(selectedDropdownMemberData.MemberData);
            GUILayout.EndArea();
        }

        private void DrawHeader(string label)
        {
            GUILayout.Space(4);
            EditorGUILayout.LabelField(label, styles.HeaderStyle);
            GUILayout.Space(4);
        }

        private bool DrawFold(bool fold, string label, bool drawRect = false)
        {
            Rect r = EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            if (drawRect)
            {
                r.height = 1;
                EditorGUI.DrawRect(r, styles.ToolbarBorderColor);
            }

            fold = EditorGUILayout.Foldout(fold, label, true);
            EditorGUILayout.EndHorizontal();
            return fold;
        }

        private void DrawFields(MemberCache.MemberData memberData)
        {
            DrawHeader("Fields");

            FoldCache fold = memberDataToFoldCache[memberData];

            fieldsScroll = EditorGUILayout.BeginScrollView(fieldsScroll);
            DrawFields(ref fold.PublicStaticFields,
                $"Public Static ({memberData.PublicStaticFields.Count})",
                memberData.PublicStaticFields,
                null,
                true);

            DrawFields(ref fold.NonPublicStaticFields,
                $"Non-Public Static ({memberData.NonPublicStaticFields.Count})",
                memberData.NonPublicStaticFields,
                null);

            DrawFields(ref fold.PublicInstanceFields,
                $"Public ({memberData.PublicInstanceFields.Count})",
                memberData.PublicInstanceFields,
                instance);

            DrawFields(ref fold.NonPublicSerializedInstanceFields,
                $"Non-Public ({memberData.NonPublicInstanceFields.Count})",
                memberData.NonPublicInstanceFields,
                instance);
            EditorGUILayout.EndScrollView();
        }

        private void DrawFields(
            ref bool fold,
            string label,
            IEnumerable<FieldInfo> fields,
            object instance,
            bool drawRect = false
        )
        {
            fold = DrawFold(fold, label, drawRect);

            if (!fold)
                return;

            bool alt = true;

            foreach (FieldInfo field in fields)
            {
                if (!fieldToDrawer.TryGetValue(field, out FieldDrawer drawer))
                {
                    fieldToDrawer[field] = (drawer = new FieldDrawer(field,
                        DrawerFactory.CreateDrawer(field, field.GetValue(instance))));
                }

                Rect backgroundRect = EditorGUILayout.BeginVertical(styles.MemberInfoStyle);
                EditorGUI.DrawRect(backgroundRect, alt ? styles.AltRowColor : styles.RowColor);
                Rect drawerRect = EditorGUILayout.GetControlRect(true, drawer.GetHeight());
                drawer.OnGUI(drawerRect, instance);
                EditorGUILayout.EndHorizontal();
                alt = !alt;
            }
        }

        private void DrawProperties(MemberCache.MemberData memberData)
        {
            DrawHeader("Properties");

            FoldCache fold = memberDataToFoldCache[memberData];

            propertiesScroll = EditorGUILayout.BeginScrollView(propertiesScroll);
            DrawProperties(ref fold.PublicStaticProperties,
                $"Public Static ({memberData.PublicStaticProperties.Count})",
                memberData.PublicStaticProperties,
                null,
                true);

            DrawProperties(ref fold.NonPublicStaticProperties,
                $"Non-Public Static ({memberData.NonPublicStaticProperties.Count})",
                memberData.NonPublicStaticProperties,
                null);

            DrawProperties(ref fold.PublicInstanceProperties,
                $"Public ({memberData.PublicInstanceProperties.Count})",
                memberData.PublicInstanceProperties,
                instance);

            DrawProperties(ref fold.NonPublicInstanceProperties,
                $"Non-Public ({memberData.NonPublicInstanceProperties.Count})",
                memberData.NonPublicInstanceProperties,
                instance);
            EditorGUILayout.EndScrollView();
        }

        private void DrawProperties(
            ref bool fold,
            string label,
            IEnumerable<PropertyInfo> properties,
            object instance,
            bool drawRect = false
        )
        {
            fold = DrawFold(fold, label, drawRect);

            if (!fold)
                return;

            bool alt = true;

            foreach (PropertyInfo property in properties)
            {
                if (!propertyToDrawer.TryGetValue(property, out PropertyDrawer drawer))
                {
                    propertyToDrawer[property] = (drawer = new PropertyDrawer(property,
                        DrawerFactory.CreateDrawer(property, property.GetValue(instance))));
                }

                Rect backgroundRect = EditorGUILayout.BeginVertical(styles.MemberInfoStyle);
                EditorGUI.DrawRect(backgroundRect, alt ? styles.AltRowColor : styles.RowColor);
                Rect drawerRect = EditorGUILayout.GetControlRect(true, drawer.GetHeight());
                drawer.OnGUI(drawerRect, instance);
                EditorGUILayout.EndHorizontal();
                alt = !alt;
            }
        }

        private void DrawDivider()
        {
            EditorGUI.DrawRect(DividerRect, styles.DividerColor);
        }

        private void HandleDividerDragging()
        {
            Event evt = Event.current;

            EditorGUIUtility.AddCursorRect(DividerResizeRect, MouseCursor.ResizeHorizontal);

            if (evt.type == EventType.MouseDown && evt.button == 0)
            {
                if (DividerResizeRect.Contains(evt.mousePosition))
                {
                    isDraggingDivider = true;
                    Repaint();
                }
            }

            if (evt.type == EventType.MouseDrag && evt.button == 0)
            {
                if (isDraggingDivider)
                {
                    dividerOffset = Mathf.Clamp(dividerOffset + evt.delta.x,
                        MIN_SECTION_WIDTH,
                        position.width - MIN_SECTION_WIDTH);
                    Repaint();
                }
            }

            if (evt.type == EventType.MouseUp)
            {
                isDraggingDivider = false;
                Repaint();
            }
        }

        /// <inheritdoc />
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            // Left empty on purpose
        }

        /// <inheritdoc />
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            needsInitialization = true;
        }
    }
}
