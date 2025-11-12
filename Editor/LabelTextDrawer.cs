using UnityEditor;
using UnityEngine;

namespace EditorScript
{
    /// <summary>
    /// LabelTextAttribute를 처리하는 PropertyDrawer
    /// </summary>
    [CustomPropertyDrawer(typeof(FieldLabelAttribute))]
    public class LabelTextDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            FieldLabelAttribute labelText = attribute as FieldLabelAttribute;

            if (labelText != null)
            {
                label.text = labelText.Label;
            }

            EditorGUI.PropertyField(position, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}
