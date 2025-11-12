using System.Collections.Generic;
using CameraBehaviour.Core.Strategy.Zoom;
using CameraBehaviour.Core.Strategy.Interface;
using UnityEngine;

namespace CameraBehaviour.DataLayer.Config.Action.Zoom
{
    [ClassLabel("일반 줌 인아웃")]
    public class ZoomActionConfig : ActionConfigBase
    {
        [Header("계수")]
        [FieldLabel("줌 계수")]
        [Tooltip("입력 값 x 줌 계수를 하여 줌 변화량을 계산합니다.")]
        [SerializeField][Min(0.1f)] public float zoomMultiplier = 0.1f;

        [Header("이동 방향")]
        [FieldLabel("역방향")]
        [Tooltip("기존 방향이 아닌 역방향으로 작동합니다.")]
        [SerializeField] public bool reversedDirection = false;

        [Header("임의 값 설정 (선택))")]
        [Tooltip("실제 입력 값이 아닌 설정한 임의의 값으로 작동합니다.")]
        [FieldLabel("임의 값 적용")]
        [SerializeField] public bool setCustom;
        [FieldLabel("줌인/아웃(true=줌인)")]
        [SerializeField] public bool isZoom;
        [FieldLabel("변화량(0.1 이상)")]
        [SerializeField][Min(0.1f)] public float zoomValue;

        public override IActionStrategy CreateStrategy() => CreateZoomStrategy();
        protected virtual CameraZoomStrategy CreateZoomStrategy() => new CameraZoomStrategy(this);

        public override void Validate(List<string> warnings)
        {
            base.Validate(warnings);
            if (zoomMultiplier < 0.1f)
                warnings.Add($"{DisplayName} (ZoomAction): 줌 계수는 0.1 이상이어야 합니다.");
        }
    }
}