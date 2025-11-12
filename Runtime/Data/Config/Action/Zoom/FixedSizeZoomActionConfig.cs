using System.Collections.Generic;
using CameraBehaviour.Core.Strategy.Zoom;
using UnityEngine;

namespace CameraBehaviour.DataLayer.Config.Action.Zoom
{
    [ClassLabel("고정 사이즈 줌 인아웃")]
    public class FixedSizeZoomActionConfig : ZoomActionConfig
    {
        [Header("간격 설정")]
        [FieldLabel("사이즈")]
        [Tooltip("줌 인/아웃 시 늘거나 작아질 고정된 사이즈를 설정합니다.")]
        [SerializeField][Min(0.1f)] public float fixedSize = 1.0f;

        protected override CameraZoomStrategy CreateZoomStrategy() => new CameraFixedSizeZoomStrategy(this);

        public override void Validate(List<string> warnings)
        {
            base.Validate(warnings);
            if (fixedSize < 0.1f)
                warnings.Add($"{DisplayName} (FixedSizeZoomAction): 사이즈(fixedSize)는 0.1f 이상이어야 합니다.");
        }
    }
}