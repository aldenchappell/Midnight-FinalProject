using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ColoredHeaderAttribute))]
public class ColoredHeaderDrawer : PropertyDrawer
{
    // public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    // {
    //     ColoredHeaderAttribute coloredHeader = (ColoredHeaderAttribute)attribute;
    //
    //     GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
    //     style.normal.textColor = coloredHeader.Color;
    //
    //     EditorGUI.LabelField(position, coloredHeader.Header, style);
    // }
    //
    // public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    // {
    //     return EditorGUIUtility.singleLineHeight;
    // }
}