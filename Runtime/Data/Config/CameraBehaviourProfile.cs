using UnityEngine;
using System.Collections.Generic;

namespace CameraBehaviour.DataLayer.Config
{
    [CreateAssetMenu(fileName = "CameraBehaviorProfile", menuName = "Camera/Behavior Profile")]
    public class CameraBehaviourProfile : ScriptableObject
    {
        [Header("액션 관리")]
        [FieldLabel("액션 리스트")]
        public List<CameraActionUnit> actions = new();

        [Header("공통 설정")]
        [FieldLabel("디버깅 로그 허용")]
        public bool debugLogging = false;

        // 유효성 전체 검사 (에디터 버튼에서 호출)
        public List<string> ValidateAll()
        {
            var warnings = new List<string>();
            foreach (var unit in actions)
            {
                unit?.Validate(warnings);
            }
            return warnings;
        }
    }
}