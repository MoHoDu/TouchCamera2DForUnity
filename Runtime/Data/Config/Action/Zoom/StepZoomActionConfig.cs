using System.Collections.Generic;
using CameraBehaviour.Core.Strategy.Zoom;
using UnityEngine;

namespace CameraBehaviour.DataLayer.Config.Action.Zoom
{
    [ClassLabel("단계 줌 인아웃")]
    public class StepZoomActionConfig : ZoomActionConfig
    {
        [Header("단계 설정")]
        [FieldLabel("줌 단계")]
        [Tooltip("줌 인/아웃 단계 사이즈를 설정합니다.")]
        [SerializeField] public List<float> stepSizeList = new List<float>();

        protected override CameraZoomStrategy CreateZoomStrategy() => new CameraStepZoomStrategy(this);

        public override void Validate(List<string> warnings)
        {
            base.Validate(warnings);
            if (stepSizeList.Count == 0)
                warnings.Add($"{DisplayName} (StepZoomAction): 줌 단계(stepSizeList) 는 최소 1개 이상이어야 합니다.");
        }
    }
}