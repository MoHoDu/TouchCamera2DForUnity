using System.Collections.Generic;
using CameraBehaviour.DataLayer.Input;
using UnityEngine;

namespace CameraBehaviour.DataLayer.Config.Input
{
    [ClassLabel("마우스 휠 업다운")]
    public class MouseWheelConfig : InputConfigBase
    {
        public override void Validate(List<string> warnings)
        {
            base.Validate(warnings);
        }
    }
}