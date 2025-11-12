using System.Collections.Generic;
using UnityEngine;

namespace CameraBehaviour.DataLayer.Config.Input
{
    [ClassLabel("스크립트 내 직접 호출")]
    public class CallDirectConfig : InputConfigBase
    {
        [Header("호출 정보")]
        [FieldLabel("액션 호출명")]
        [SerializeField] public string callingName;

        public override void Validate(List<string> warnings)
        {
            base.Validate(warnings);
            if (string.IsNullOrEmpty(callingName))
                warnings.Add($"{DisplayName} ({GetType().Name}): 액션을 호출하기 위해서는 반드시 호출명을 적어야 합니다.");
        }
    }
}