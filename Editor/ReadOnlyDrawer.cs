using UnityEditor;
using UnityEngine;
using System.Reflection; // FieldInfo를 위해 추가

namespace EditorScript
{
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        // --- GetPropertyHeight 수정 ---
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // TextArea 속성이 있는지 확인합니다.
            var textAreaAttribute = fieldInfo.GetCustomAttribute<TextAreaAttribute>();
            if (property.propertyType == SerializedPropertyType.String && textAreaAttribute != null)
            {
                // 문자열의 내용에 따라 필요한 높이를 동적으로 계산합니다.
                GUIStyle style = new GUIStyle(EditorStyles.label) { wordWrap = true };
                float height = style.CalcHeight(new GUIContent(property.stringValue), EditorGUIUtility.currentViewWidth - 20);
                
                // 최소 높이를 보장합니다. (TextArea의 minLines * 한 줄 높이)
                float minHeight = textAreaAttribute.minLines * EditorGUIUtility.singleLineHeight;

                // 레이블 높이를 더해줍니다.
                return Mathf.Max(height, minHeight) + EditorGUIUtility.singleLineHeight;
            }

            // 그 외의 경우는 기본 높이를 사용합니다.
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        // --- OnGUI 수정 ---
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // TextArea 속성이 있는지 확인합니다.
            var textAreaAttribute = fieldInfo.GetCustomAttribute<TextAreaAttribute>();
            if (property.propertyType == SerializedPropertyType.String && textAreaAttribute != null)
            {
                // 1. 레이블을 먼저 그립니다.
                Rect labelPosition = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(labelPosition, label);

                // 2. 레이블 아래에 텍스트 영역을 그립니다.
                Rect textPosition = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, position.height - EditorGUIUtility.singleLineHeight);
                
                // 텍스트가 많아도 전체가 보이도록 SelectableLabel을 사용합니다.
                // 이 컨트롤은 편집은 불가능하지만, 텍스트 선택과 복사는 가능합니다.
                // 배경을 그려주어 TextArea처럼 보이게 합니다.
                EditorGUI.DrawRect(textPosition, new Color(0.22f, 0.22f, 0.22f)); // 어두운 배경색
                GUIStyle style = new GUIStyle(EditorStyles.label) { wordWrap = true, padding = new RectOffset(4, 4, 4, 4) };
                EditorGUI.SelectableLabel(textPosition, property.stringValue, style);
            }
            else
            {
                // 기존 로직: TextArea가 아닌 경우, 비활성화된 기본 필드를 그립니다.
                GUI.enabled = false;
                EditorGUI.PropertyField(position, property, label, true);
                GUI.enabled = true;
            }
        }
    }
}