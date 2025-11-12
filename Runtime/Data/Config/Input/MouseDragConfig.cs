using System.Collections.Generic;
using CameraBehaviour.DataLayer.Config.Action.Move;
using CameraBehaviour.DataLayer.Input;
using UnityEngine;

namespace CameraBehaviour.DataLayer.Config.Input
{
    [ClassLabel("마우스 드래그")]
    public class MouseDragConfig : InputConfigBase
    {
        [Header("감지할 상태 선택")]
        [Tooltip("감지할 상태를 모두 고르세요. 고르지 않은 상태의 입력은 무시됩니다.")]

        [FieldLabel("드래그 시작")]
        [SerializeField] public bool onStartValidate;
        [FieldLabel("드래그 중")]
        [SerializeField] public bool onDoingValidate;
        [FieldLabel("드래그 종료")]
        [SerializeField] public bool onEndValidate;

        public override void Validate(List<string> warnings)
        {
            base.Validate(warnings);
        }

        public override bool ValidateInput(InputContext context)
        {
            bool baseValidate = base.ValidateInput(context);
            if (!baseValidate) return false;

            // 단일 동작이므로 연속된 동작에서는 동작하지 않습니다.
            if (context.ExtraData.TryGetValue(typeof(DragState).Name, out var result))
            {
                if (result is DragState state)
                {
                    return CheckStateValidate(state);
                }
            }
            return false;
        }

        private bool CheckStateValidate(DragState state)
        {
            if (state == DragState.START && onStartValidate) return true;
            if (state == DragState.DOING && onDoingValidate) return true;
            if (state == DragState.END && onEndValidate) return true;
            return false;
        }
    }
}