using System.Collections.Generic;
using CameraBehaviour.DataLayer.Input;
using UnityEngine;

namespace CameraBehaviour.DataLayer.Config.Input
{
    [ClassLabel("터치")]
    public class ScreenTouchConfig : InputConfigBase
    {
        [Header("터치 설정")]
        [FieldLabel("감지 시점")]
        [SerializeField] private ClickState _detectState;

        public override void Validate(List<string> warnings)
        {
            base.Validate(warnings);
        }

        public override bool ValidateInput(InputContext context)
        {
            // Delta => ClickState 감지
            if (Mathf.Abs(context.Delta) < 0) return false;
            return true;
        }
    }
}