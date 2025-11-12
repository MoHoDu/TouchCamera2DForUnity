using System.Collections.Generic;
using CameraBehaviour.DataLayer.Config.Input.Interface;
using CameraBehaviour.DataLayer.Input;
using UnityEngine;

namespace CameraBehaviour.DataLayer.Config.Input
{
    public abstract class InputConfigBase : ConfigBase, IInputConfig
    {
        [Header("감지 범위 설정")]
        [FieldLabel("최소 입력 값")]
        [Tooltip("인식하여 동작으로 연결할 최소 입력 변화량(절대값)을 설정합니다. 이보다 작은 변화량은 무시됩니다.\n**주의: 특정 입력 타입에서는 무시될 수 있습니다.**")]
        [Min(0)] public float minDelta = 0f;
        public float MinDelta => minDelta;

        public override void Validate(List<string> warnings)
        {
            base.Validate(warnings);
            if (minDelta < 0f)
                warnings.Add($"{DisplayName} ({GetType().Name}): 최소 변화량(MinDelta)는 0 이상이어야 합니다.");
        }

        public virtual bool ValidateInput(InputContext context)
        {
            if (Mathf.Abs(context.Delta) < minDelta) return false;
            return true;
        }
    }
}