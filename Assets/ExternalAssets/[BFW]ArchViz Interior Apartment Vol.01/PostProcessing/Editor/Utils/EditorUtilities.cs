using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering.PostProcessing;

namespace UnityEditor.Rendering.PostProcessing
{
    public static class EditorUtilities
    {
        private static readonly Dictionary<string, GUIContent> s_GUIContentCache;
        private static readonly Dictionary<Type, AttributeDecorator> s_AttributeDecorators;

        private static PostProcessEffectSettings s_ClipboardContent;

        static EditorUtilities()
        {
            s_GUIContentCache = new Dictionary<string, GUIContent>();
            s_AttributeDecorators = new Dictionary<Type, AttributeDecorator>();
            ReloadDecoratorTypes();
        }

        [DidReloadScripts]
        private static void OnEditorReload()
        {
            ReloadDecoratorTypes();
        }

        private static void ReloadDecoratorTypes()
        {
            s_AttributeDecorators.Clear();

            // Look for all the valid attribute decorators
            var types = RuntimeUtilities.GetAllAssemblyTypes()
                .Where(
                    t => t.IsSubclassOf(typeof(AttributeDecorator))
                         && t.IsDefined(typeof(DecoratorAttribute), false)
                         && !t.IsAbstract
                );

            // Store them
            foreach (var type in types)
            {
                var attr = type.GetAttribute<DecoratorAttribute>();
                var decorator = (AttributeDecorator)Activator.CreateInstance(type);
                s_AttributeDecorators.Add(attr.attributeType, decorator);
            }
        }

        internal static AttributeDecorator GetDecorator(Type attributeType)
        {
            AttributeDecorator decorator;
            return !s_AttributeDecorators.TryGetValue(attributeType, out decorator)
                ? null
                : decorator;
        }

        public static GUIContent GetContent(string textAndTooltip)
        {
            if (string.IsNullOrEmpty(textAndTooltip))
                return GUIContent.none;

            GUIContent content;

            if (!s_GUIContentCache.TryGetValue(textAndTooltip, out content))
            {
                var s = textAndTooltip.Split('|');
                content = new GUIContent(s[0]);

                if (s.Length > 1 && !string.IsNullOrEmpty(s[1]))
                    content.tooltip = s[1];

                s_GUIContentCache.Add(textAndTooltip, content);
            }

            return content;
        }

        public static void DrawFixMeBox(string text, Action action)
        {
            Assert.IsNotNull(action);

            EditorGUILayout.HelpBox(text, MessageType.Warning);

            GUILayout.Space(-32);
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Fix", GUILayout.Width(60)))
                    action();

                GUILayout.Space(8);
            }

            GUILayout.Space(11);
        }

        public static void DrawSplitter()
        {
            var rect = GUILayoutUtility.GetRect(1f, 1f);

            // Splitter rect should be full-width
            rect.xMin = 0f;
            rect.width += 4f;

            if (Event.current.type != EventType.Repaint)
                return;

            EditorGUI.DrawRect(rect, !EditorGUIUtility.isProSkin
                ? new Color(0.6f, 0.6f, 0.6f, 1.333f)
                : new Color(0.12f, 0.12f, 0.12f, 1.333f));
        }

        public static void DrawOverrideCheckbox(Rect rect, SerializedProperty property)
        {
            var oldColor = GUI.color;
            GUI.color = new Color(0.6f, 0.6f, 0.6f, 0.75f);
            property.boolValue = GUI.Toggle(rect, property.boolValue,
                GetContent("|Override this setting for this volume."), Styling.smallTickbox);
            GUI.color = oldColor;
        }

        public static void DrawHeaderLabel(string title)
        {
            EditorGUILayout.LabelField(title, Styling.labelHeader);
        }

        public static bool DrawHeader(string title, bool state)
        {
            var backgroundRect = GUILayoutUtility.GetRect(1f, 17f);

            var labelRect = backgroundRect;
            labelRect.xMin += 16f;
            labelRect.xMax -= 20f;

            var foldoutRect = backgroundRect;
            foldoutRect.y += 1f;
            foldoutRect.width = 13f;
            foldoutRect.height = 13f;

            // Background rect should be full-width
            backgroundRect.xMin = 0f;
            backgroundRect.width += 4f;

            // Background
            var backgroundTint = EditorGUIUtility.isProSkin ? 0.1f : 1f;
            EditorGUI.DrawRect(backgroundRect, new Color(backgroundTint, backgroundTint, backgroundTint, 0.2f));

            // Title
            EditorGUI.LabelField(labelRect, GetContent(title), EditorStyles.boldLabel);

            // Active checkbox
            state = GUI.Toggle(foldoutRect, state, GUIContent.none, EditorStyles.foldout);

            var e = Event.current;
            if (e.type == EventType.MouseDown && backgroundRect.Contains(e.mousePosition) && e.button == 0)
            {
                state = !state;
                e.Use();
            }

            return state;
        }

        public static bool DrawHeader(string title, SerializedProperty group, SerializedProperty activeField,
            PostProcessEffectSettings target, Action resetAction, Action removeAction)
        {
            Assert.IsNotNull(group);
            Assert.IsNotNull(activeField);
            Assert.IsNotNull(target);

            var backgroundRect = GUILayoutUtility.GetRect(1f, 17f);

            var labelRect = backgroundRect;
            labelRect.xMin += 16f;
            labelRect.xMax -= 20f;

            var toggleRect = backgroundRect;
            toggleRect.y += 2f;
            toggleRect.width = 13f;
            toggleRect.height = 13f;

            var menuIcon = EditorGUIUtility.isProSkin
                ? Styling.paneOptionsIconDark
                : Styling.paneOptionsIconLight;

            var menuRect = new Rect(labelRect.xMax + 4f, labelRect.y + 4f, menuIcon.width, menuIcon.height);

            // Background rect should be full-width
            backgroundRect.xMin = 0f;
            backgroundRect.width += 4f;

            // Background
            var backgroundTint = EditorGUIUtility.isProSkin ? 0.1f : 1f;
            EditorGUI.DrawRect(backgroundRect, new Color(backgroundTint, backgroundTint, backgroundTint, 0.2f));

            // Title
            using (new EditorGUI.DisabledScope(!activeField.boolValue))
            {
                EditorGUI.LabelField(labelRect, GetContent(title), EditorStyles.boldLabel);
            }

            // Active checkbox
            activeField.serializedObject.Update();
            activeField.boolValue =
                GUI.Toggle(toggleRect, activeField.boolValue, GUIContent.none, Styling.smallTickbox);
            activeField.serializedObject.ApplyModifiedProperties();

            // Dropdown menu icon
            GUI.DrawTexture(menuRect, menuIcon);

            // Handle events
            var e = Event.current;

            if (e.type == EventType.MouseDown)
            {
                if (menuRect.Contains(e.mousePosition))
                {
                    ShowHeaderContextMenu(new Vector2(menuRect.x, menuRect.yMax), target, resetAction, removeAction);
                    e.Use();
                }
                else if (labelRect.Contains(e.mousePosition))
                {
                    if (e.button == 0)
                        group.isExpanded = !group.isExpanded;
                    else
                        ShowHeaderContextMenu(e.mousePosition, target, resetAction, removeAction);

                    e.Use();
                }
            }

            return group.isExpanded;
        }

        private static void ShowHeaderContextMenu(Vector2 position, PostProcessEffectSettings target,
            Action resetAction, Action removeAction)
        {
            Assert.IsNotNull(resetAction);
            Assert.IsNotNull(removeAction);

            var menu = new GenericMenu();
            menu.AddItem(GetContent("Reset"), false, () => resetAction());
            menu.AddItem(GetContent("Remove"), false, () => removeAction());
            menu.AddSeparator(string.Empty);
            menu.AddItem(GetContent("Copy Settings"), false, () => CopySettings(target));

            if (CanPaste(target))
                menu.AddItem(GetContent("Paste Settings"), false, () => PasteSettings(target));
            else
                menu.AddDisabledItem(GetContent("Paste Settings"));

            menu.DropDown(new Rect(position, Vector2.zero));
        }

        private static void CopySettings(PostProcessEffectSettings target)
        {
            Assert.IsNotNull(target);

            if (s_ClipboardContent != null)
            {
                RuntimeUtilities.Destroy(s_ClipboardContent);
                s_ClipboardContent = null;
            }

            s_ClipboardContent = (PostProcessEffectSettings)ScriptableObject.CreateInstance(target.GetType());
            EditorUtility.CopySerializedIfDifferent(target, s_ClipboardContent);
        }

        private static void PasteSettings(PostProcessEffectSettings target)
        {
            Assert.IsNotNull(target);
            Assert.IsNotNull(s_ClipboardContent);
            Assert.AreEqual(s_ClipboardContent.GetType(), target.GetType());

            Undo.RecordObject(target, "Paste Settings");
            EditorUtility.CopySerializedIfDifferent(s_ClipboardContent, target);
        }

        private static bool CanPaste(PostProcessEffectSettings target)
        {
            return s_ClipboardContent != null
                   && s_ClipboardContent.GetType() == target.GetType();
        }
    }
}