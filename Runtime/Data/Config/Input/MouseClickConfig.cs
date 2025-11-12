using System;
using System.Collections.Generic;
using System.Linq;
using CameraBehaviour.DataLayer.Input;
using UnityEngine;

namespace CameraBehaviour.DataLayer.Config.Input
{
    public enum ClickType
    {
        LEFT = -1,
        WHEEL = 0,
        RIGHT = 1,
    }

    public enum ClickState
    {
        NONE = -1,
        START = 0,
        END = 1,
        HOLD = 2
    }

    [ClassLabel("마우스 클릭")]
    public class MouseClickConfig : InputConfigBase
    {
        [Header("클릭 설정")]
        [FieldLabel("타입")]
        [SerializeField] private ClickType _clickType;
        [FieldLabel("감지 시점")]
        [SerializeField] private ClickState _detectState;

        public override void Validate(List<string> warnings)
        {
            base.Validate(warnings);
        }

        public override bool ValidateInput(InputContext context)
        {
            // Delta => ClickState 감지
            if (Mathf.Abs(context.Delta) != (int)_detectState) return false;
            // Direction => ClickType 감지
            int directionX = (int)context.Direction.x;
            if (directionX != (int)_clickType) return false;
            return true;
        }
    }
}