//
// Procedural Lightning for Unity
// (c) 2015 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

using System;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using Random = System.Random;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace DigitalRuby.ThunderAndLightning
{
    [Serializable]
    public struct RangeOfIntegers
    {
        [Tooltip("Minimum value (inclusive)")] public int Minimum;

        [Tooltip("Maximum value (inclusive)")] public int Maximum;

        public int Random()
        {
            return UnityEngine.Random.Range(Minimum, Maximum + 1);
        }

        public int Random(Random r)
        {
            return r.Next(Minimum, Maximum + 1);
        }
    }

    [Serializable]
    public struct RangeOfFloats
    {
        [Tooltip("Minimum value (inclusive)")] public float Minimum;

        [Tooltip("Maximum value (inclusive)")] public float Maximum;

        public float Random()
        {
            return UnityEngine.Random.Range(Minimum, Maximum);
        }

        public float Random(Random r)
        {
            return Minimum + (float)r.NextDouble() * (Maximum - Minimum);
        }
    }

    [Serializable]
    public class ReorderableList_GameObject : ReorderableList<GameObject>
    {
    }

    [Serializable]
    public class ReorderableList_Transform : ReorderableList<Transform>
    {
    }

    [Serializable]
    public class ReorderableList_Vector3 : ReorderableList<Vector3>
    {
    }

    [Serializable]
    public class ReorderableList_Rect : ReorderableList<Rect>
    {
    }

    [Serializable]
    public class ReorderableList_RectOffset : ReorderableList<RectOffset>
    {
    }

    [Serializable]
    public class ReorderableList_Int : ReorderableList<int>
    {
    }

    [Serializable]
    public class ReorderableList_Float : ReorderableList<float>
    {
    }

    [Serializable]
    public class ReorderableList_String : ReorderableList<string>
    {
    }

    [Serializable]
    public class ReorderableList<T> : ReorderableListBase
    {
        public List<T> List;
    }

    [Serializable]
    public class ReorderableListBase
    {
    }

    public class ReorderableListAttribute : PropertyAttribute
    {
        public ReorderableListAttribute(string tooltip)
        {
            Tooltip = tooltip;
        }

        public string Tooltip { get; }
    }

    public class SingleLineAttribute : PropertyAttribute
    {
        public SingleLineAttribute(string tooltip)
        {
            Tooltip = tooltip;
        }

        public string Tooltip { get; }
    }

    public class SingleLineClampAttribute : SingleLineAttribute
    {
        public SingleLineClampAttribute(string tooltip, double minValue, double maxValue) : base(tooltip)
        {
            MinValue = minValue;
            MaxValue = maxValue;
        }

        public double MinValue { get; }
        public double MaxValue { get; }
    }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(SingleLineAttribute))]
    [CustomPropertyDrawer(typeof(SingleLineClampAttribute))]
    public class SingleLineDrawer : PropertyDrawer
    {
        private void DrawIntTextField(Rect position, string text, string tooltip, SerializedProperty prop)
        {
            EditorGUI.BeginChangeCheck();
            var value = EditorGUI.IntField(position, new GUIContent(text, tooltip), prop.intValue);
            var clamp = attribute as SingleLineClampAttribute;
            if (clamp != null) value = Mathf.Clamp(value, (int)clamp.MinValue, (int)clamp.MaxValue);
            if (EditorGUI.EndChangeCheck()) prop.intValue = value;
        }

        private void DrawFloatTextField(Rect position, string text, string tooltip, SerializedProperty prop)
        {
            EditorGUI.BeginChangeCheck();
            var value = EditorGUI.FloatField(position, new GUIContent(text, tooltip), prop.floatValue);
            var clamp = attribute as SingleLineClampAttribute;
            if (clamp != null) value = Mathf.Clamp(value, (float)clamp.MinValue, (float)clamp.MaxValue);
            if (EditorGUI.EndChangeCheck()) prop.floatValue = value;
        }

        private void DrawRangeField(Rect position, SerializedProperty prop, bool floatingPoint)
        {
            EditorGUIUtility.labelWidth = 30.0f;
            EditorGUIUtility.fieldWidth = 40.0f;
            var width = position.width * 0.49f;
            var spacing = position.width * 0.02f;
            position.width = width;
            if (floatingPoint)
                DrawFloatTextField(position, "Min", "Minimum value", prop.FindPropertyRelative("Minimum"));
            else
                DrawIntTextField(position, "Min", "Minimum value", prop.FindPropertyRelative("Minimum"));
            position.x = position.xMax + spacing;
            position.width = width;
            if (floatingPoint)
                DrawFloatTextField(position, "Max", "Maximum value", prop.FindPropertyRelative("Maximum"));
            else
                DrawIntTextField(position, "Max", "Maximum value", prop.FindPropertyRelative("Maximum"));
        }

        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, prop);
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive),
                new GUIContent(label.text, (attribute as SingleLineAttribute).Tooltip));

            switch (prop.type)
            {
                case "RangeOfIntegers":
                    DrawRangeField(position, prop, false);
                    break;

                case "RangeOfFloats":
                    DrawRangeField(position, prop, true);
                    break;

                default:
                    EditorGUI.HelpBox(position, "[SingleLineDrawer] doesn't work with type '" + prop.type + "'",
                        MessageType.Error);
                    break;
            }

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }

    [CustomPropertyDrawer(typeof(ReorderableListAttribute), true)]
    public class ReorderableListDrawer : PropertyDrawer
    {
        private ReorderableList list;
        private SerializedProperty prevProperty;

        private ReorderableList GetList(SerializedProperty property)
        {
            if (list == null || prevProperty != property)
            {
                prevProperty = property;
                var listProperty = property.FindPropertyRelative("List");
                list = new ReorderableList(listProperty.serializedObject, listProperty, true, false, true, true);
                list.drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    EditorGUIUtility.labelWidth = 100.0f;
                    EditorGUI.PropertyField(rect, listProperty.GetArrayElementAtIndex(index), true);
                };
            }

            return list;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return GetList(property).GetHeight();
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attr = attribute as ReorderableListAttribute;
            var tooltip = attr == null ? string.Empty : attr.Tooltip;
            var list = GetList(property);
            float height;
            if (list.serializedProperty.arraySize == 0)
            {
                height = 20.0f;
            }
            else
            {
                height = 0.0f;
                for (var i = 0; i < list.serializedProperty.arraySize; i++)
                    height = Mathf.Max(height,
                        EditorGUI.GetPropertyHeight(list.serializedProperty.GetArrayElementAtIndex(i)));
            }

            list.drawHeaderCallback = r => { EditorGUI.LabelField(r, new GUIContent(label.text, tooltip)); };
            list.elementHeight = height;
            list.DoList(position);
        }
    }

#endif
}