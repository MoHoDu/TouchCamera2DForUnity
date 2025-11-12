using System;
using UnityEngine;

/// <summary>
/// Inspector에서 필드의 표시 이름을 변경하는 어트리뷰트
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class FieldLabelAttribute : PropertyAttribute
{
    public string Label { get; private set; }

    public FieldLabelAttribute(string label)
    {
        Label = label;
    }
}

