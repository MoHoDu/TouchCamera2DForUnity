#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using CameraBehaviour.DataLayer.Config;
using CameraBehaviour.DataLayer.Config.Action.Interface;
using CameraBehaviour.DataLayer.Config.Input.Interface;

namespace EditorScript
{
    [CustomEditor(typeof(CameraBehaviourProfile))]
    public class CameraBehaviourProfileEditor : Editor
    {
        private SerializedProperty actionsProp;
        private SerializedProperty debugLoggingProp;

        // 타입 캐싱 (성능 최적화)
        private static Dictionary<Type, List<Type>> implementationCache = new();
        // private static bool cacheInitialized = false;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            debugLoggingProp = serializedObject.FindProperty("debugLogging");
            actionsProp = serializedObject.FindProperty("actions");

            // 상단 공통 옵션
            EditorGUILayout.PropertyField(debugLoggingProp);
            EditorGUILayout.Space();

            // Actions 리스트 렌더링
            EditorGUILayout.LabelField("Camera Action Units", EditorStyles.boldLabel);
            DrawActionList(actionsProp);

            EditorGUILayout.Space(12);
            // Validate 버튼
            if (GUILayout.Button("✅ Validate All"))
            {
                var profile = (CameraBehaviourProfile)target;
                var warnings = profile.ValidateAll();
                if (warnings.Count == 0)
                    Debug.Log($"[CameraBehaviorProfile] ✅ No issues found in {profile.name}.");
                else
                {
                    Debug.LogWarning($"[CameraBehaviorProfile] ⚠ {warnings.Count} issues found in {profile.name}:");
                    foreach (var w in warnings)
                        Debug.LogWarning($"  • {w}");
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawActionList(SerializedProperty listProp)
        {
            if (listProp == null) return;

            for (int i = 0; i < listProp.arraySize; i++)
            {
                var element = listProp.GetArrayElementAtIndex(i);
                if (element == null) continue;
                var nameProp = element.FindPropertyRelative("name");
                var orderProp = element.FindPropertyRelative("order");

                EditorGUILayout.BeginVertical("box");
                // 메타 정보 렌더링
                EditorGUILayout.PropertyField(nameProp, true);
                EditorGUILayout.PropertyField(orderProp, true);
                EditorGUILayout.Space(4);

                // 개별 필드 렌더링
                DrawPolymorphicField(element, "input", "Input Config", typeof(IInputConfig), allowNull: false);
                DrawPolymorphicField(element, "action", "Main Action", typeof(IActionConfig), allowNull: false);

                // 삭제 버튼
                if (GUILayout.Button("🗑 Remove Action Unit"))
                {
                    listProp.DeleteArrayElementAtIndex(i);
                    EditorGUILayout.EndVertical();
                    return;
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(6);
            }

            if (GUILayout.Button("+ Add New Action Unit"))
            {
                listProp.InsertArrayElementAtIndex(listProp.arraySize);
                var newElement = listProp.GetArrayElementAtIndex(listProp.arraySize - 1);

                // 일반 직렬화 타입이므로 FindPropertyRelative로 필드 초기화
                newElement.FindPropertyRelative("name").stringValue = "New Camera Action";
                newElement.FindPropertyRelative("order").intValue = listProp.arraySize - 1;

                // 필수 필드 (allowNull: false)는 기본 타입으로 초기화
                var inputTypes = GetImplementationsOf(typeof(IInputConfig));
                if (inputTypes.Count > 0)
                    newElement.FindPropertyRelative("input").managedReferenceValue = Activator.CreateInstance(inputTypes[0]);
                else
                    newElement.FindPropertyRelative("input").managedReferenceValue = null;

                var actionTypes = GetImplementationsOf(typeof(IActionConfig));
                if (actionTypes.Count > 0)
                    newElement.FindPropertyRelative("action").managedReferenceValue = Activator.CreateInstance(actionTypes[0]);
                else
                    newElement.FindPropertyRelative("action").managedReferenceValue = null;
            }
        }

        /// <summary>
        /// 특정 필드(SerializeReference)에 대해 타입 선택 드롭다운 + 내부 필드 표시
        /// </summary>
        private void DrawPolymorphicField(SerializedProperty parent, string fieldName, string label, Type interfaceType, bool allowNull = false)
        {
            // var fieldProp = parent.FindPropertyRelative(fieldName);
            // var currentValue = fieldProp.managedReferenceValue;
            var fieldProp = parent.FindPropertyRelative(fieldName);
            if (fieldProp == null)
            {
                EditorGUILayout.HelpBox($"[WARN] {fieldName} not found", MessageType.Warning);
                return;
            }

            if (fieldProp.propertyType != SerializedPropertyType.ManagedReference)
            {
                EditorGUILayout.HelpBox($"{fieldName} is not a SerializeReference field (type: {fieldProp.propertyType}).", MessageType.Warning);
                EditorGUILayout.PropertyField(fieldProp, true);
                return;
            }

            var currentValue = fieldProp.managedReferenceValue;
            var currentType = currentValue?.GetType();
            var labelAttr = currentType?
                                .GetCustomAttributes(typeof(ClassLabelAttribute), false)
                                .FirstOrDefault() as ClassLabelAttribute;
            string currentTypeName = labelAttr != null ? labelAttr.DisplayName : (currentType != null ? currentType.Name : "(None)");

            EditorGUILayout.BeginVertical("helpbox");
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);

            // 드롭다운 구성
            var allTypes = GetImplementationsOf(interfaceType);
            // var displayNames = allTypes.Select(t => t.Name).ToList();
            var displayNames = allTypes.Select(t =>
            {
                var labelAttr = t.GetCustomAttributes(typeof(ClassLabelAttribute), false)
                                .FirstOrDefault() as ClassLabelAttribute;
                return labelAttr != null ? labelAttr.DisplayName : t.Name;
            }).ToList();
            if (allowNull) displayNames.Insert(0, "(None)");

            // 현재 인덱스 계산 (안전하게)
            int currentIndex;
            if (allowNull)
            {
                if (currentType == null)
                    currentIndex = 0;
                else
                {
                    int foundIndex = displayNames.IndexOf(currentTypeName);
                    currentIndex = foundIndex >= 0 ? foundIndex : 0;
                }
            }
            else
            {
                int foundIndex = displayNames.IndexOf(currentTypeName);
                currentIndex = foundIndex >= 0 ? foundIndex : 0;
            }

            int newIndex = EditorGUILayout.Popup("Type", currentIndex, displayNames.ToArray());

            // 타입 전환 감지
            if (newIndex != currentIndex)
            {
                if (allowNull && newIndex == 0)
                {
                    fieldProp.managedReferenceValue = null;
                }
                else
                {
                    int typeIndex = allowNull ? newIndex - 1 : newIndex;
                    if (typeIndex >= 0 && typeIndex < allTypes.Count)
                    {
                        var selectedType = allTypes[typeIndex];
                        fieldProp.managedReferenceValue = Activator.CreateInstance(selectedType);
                    }
                }
            }

            // 내부 필드 표시
            if (fieldProp.managedReferenceValue != null)
            {
                // 일반 필드들 먼저 표시
                DrawSerializedFields(fieldProp);

                // 중첩된 SerializeReference 필드들을 재귀적으로 처리
                DrawNestedPolymorphicFields(fieldProp);
            }

            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 일반 직렬화 필드만 표시 (SerializeReference 제외)
        /// </summary>
        private void DrawSerializedFields(SerializedProperty parent)
        {
            var iterator = parent.Copy();
            var endProperty = iterator.GetEndProperty();
            iterator.NextVisible(true); // 자식으로 진입

            while (!SerializedProperty.EqualContents(iterator, endProperty))
            {
                // SerializeReference가 아닌 필드만 표시
                if (iterator.propertyType != SerializedPropertyType.ManagedReference)
                {
                    EditorGUILayout.PropertyField(iterator, true);
                }

                if (!iterator.NextVisible(false))
                    break;
            }
        }

        /// <summary>
        /// 중첩된 SerializeReference 필드들을 재귀적으로 폴리모픽하게 렌더링
        /// </summary>
        private void DrawNestedPolymorphicFields(SerializedProperty parent)
        {
            var iterator = parent.Copy();
            var endProperty = iterator.GetEndProperty();
            iterator.NextVisible(true); // 자식으로 진입

            while (!SerializedProperty.EqualContents(iterator, endProperty))
            {
                // SerializeReference 필드만 처리
                if (iterator.propertyType == SerializedPropertyType.ManagedReference)
                {
                    var fieldType = GetFieldInterfaceType(iterator);
                    if (fieldType != null)
                    {
                        EditorGUILayout.Space(4);
                        DrawPolymorphicField(parent, iterator.name, iterator.displayName, fieldType, allowNull: true);
                    }
                }

                if (!iterator.NextVisible(false))
                    break;
            }
        }

        /// <summary>
        /// SerializeReference 필드의 인터페이스 타입을 리플렉션으로 자동 추론
        /// </summary>
        private Type GetFieldInterfaceType(SerializedProperty prop)
        {
            // 1. 이미 값이 할당된 경우: 해당 타입의 인터페이스 반환
            if (prop.managedReferenceValue != null)
            {
                var valueType = prop.managedReferenceValue.GetType();
                var interfaces = valueType.GetInterfaces();

                // ConfigBase 관련 인터페이스 찾기 (I로 시작하고 Config 포함)
                foreach (var interfaceType in interfaces)
                {
                    if (interfaceType.Name.StartsWith("I") && interfaceType.Name.Contains("Config"))
                        return interfaceType;
                }
            }

            // 2. 값이 없는 경우: 부모 타입의 필드 정보에서 타입 추론
            object parentValue = prop.serializedObject.targetObject;
            if (prop.propertyPath.Contains("."))
            {
                // 중첩된 경우 부모 SerializeReference의 값 가져오기
                var pathParts = prop.propertyPath.Split('.');
                var parentPath = string.Join(".", pathParts.Take(pathParts.Length - 1));
                var parentProp = prop.serializedObject.FindProperty(parentPath);
                if (parentProp != null && parentProp.managedReferenceValue != null)
                {
                    parentValue = parentProp.managedReferenceValue;
                }
            }

            if (parentValue != null)
            {
                var parentType = parentValue.GetType();
                var fieldInfo = parentType.GetField(prop.name,
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance);

                if (fieldInfo != null)
                {
                    var fieldType = fieldInfo.FieldType;

                    // 인터페이스면 직접 반환
                    if (fieldType.IsInterface)
                        return fieldType;

                    // 추상 클래스면 구현하는 인터페이스 찾기
                    if (fieldType.IsAbstract || fieldType.IsClass)
                    {
                        var interfaces = fieldType.GetInterfaces();
                        foreach (var interfaceType in interfaces)
                        {
                            if (interfaceType.Name.StartsWith("I") && interfaceType.Name.Contains("Config"))
                                return interfaceType;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 인터페이스를 구현한 모든 구체 타입 탐색 (캐싱됨)
        /// </summary>
        private List<Type> GetImplementationsOf(Type interfaceType)
        {
            // 캐시에서 먼저 확인
            if (implementationCache.TryGetValue(interfaceType, out var cachedTypes))
            {
                return cachedTypes;
            }

            // 캐시에 없으면 스캔
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a =>
                {
                    Type[] types = Array.Empty<Type>();
                    try { types = a.GetTypes(); } catch { }
                    return types;
                })
                .Where(t =>
                    interfaceType.IsAssignableFrom(t) &&
                    !t.IsAbstract &&
                    !t.IsInterface)
                .OrderBy(t => t.Name)
                .ToList();

            // 캐시에 저장
            implementationCache[interfaceType] = types;
            return types;
        }
    }
}
#endif