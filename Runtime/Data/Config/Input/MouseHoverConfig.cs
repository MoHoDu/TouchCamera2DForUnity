using System.Collections.Generic;
using CameraBehaviour.DataLayer.Input;
using UnityEngine;

namespace CameraBehaviour.DataLayer.Config.Input
{
    [ClassLabel("마우스 호버")]
    public class MouseHoverConfig : InputConfigBase
    {
        [Header("타겟 오브젝트 설정")]
        [FieldLabel("감지 레이어")]
        [SerializeField] private LayerMask _detectLayers;

        [Space(10)]

#pragma warning disable 0414    // "필드가 사용되지 않는다"는 경고를 끔
        [FieldLabel("주의 사항")]
        [ReadOnly, TextArea]
        [SerializeField]
        private string _infoText =
        @"특정 레이어의 오브젝트에 마우스를 호버했을 때를 감지합니다.
감지될 오브젝트는 반드시 'Collider'를 가지고 있어야 합니다.";
#pragma warning restore 0414    // 이후 코드엔 다시 경고 적용

        public override void Validate(List<string> warnings)
        {
            base.Validate(warnings);
        }

        public override bool ValidateInput(InputContext context)
        {
            // 모든 레이어가 감지 대상이 아닌 경우 false
            if (_detectLayers.value == 0)
                return false;

            // 호버 대상이 있어야 함 
            if (context.RequestTargetObject == null)
                return false;
            else
            {
                // 레이어 대상이 아니라면 false
                if (((1 << context.RequestTargetObject.layer) & _detectLayers.value) == 0)
                {
                    return false;
                }
            }

            // 모두 부합하는 경우 true
            return true;
        }
    }
}