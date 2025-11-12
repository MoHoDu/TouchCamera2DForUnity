using System.Collections.Generic;
using CameraBehaviour.DataLayer.Input;
using UnityEngine;

namespace CameraBehaviour.DataLayer.Config.Input
{
    public enum PinchState
    {
        PINCH_IN = -1,
        NOT_CHANGE = 0,
        PINCH_OUT = 1,
        ALL = 2
    }

    [ClassLabel("핀치 인아웃")]
    public class ScreenPinchConfig : InputConfigBase
    {
        [Header("핀치 설정")]
        [FieldLabel("감지 시점")]
        [SerializeField] private PinchState _detectState;

        public override void Validate(List<string> warnings)
        {
            base.Validate(warnings);
        }

        public override bool ValidateInput(InputContext context)
        {
            // 현재 동작 확인
            PinchState currentState = PinchState.NOT_CHANGE;
            if (context.Delta == 0)
                currentState = PinchState.NOT_CHANGE;
            else if (context.Delta < 0)
                currentState = PinchState.PINCH_IN;
            else
                currentState = PinchState.PINCH_OUT;

            // 감지 상태와 비교
            if (_detectState != PinchState.ALL && _detectState != currentState)
                return false;
            else
                return true;
        }
    }
}