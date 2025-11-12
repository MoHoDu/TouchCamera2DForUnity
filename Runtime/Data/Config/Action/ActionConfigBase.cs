using UnityEngine;
using System.Collections.Generic;
using CameraBehaviour.DataLayer.Config.Action.Interface;
using CameraBehaviour.DataLayer.Config.Action.Enum;
using CameraBehaviour.DataLayer.Config.Area;
using CameraBehaviour.Core.Strategy.Interface;

namespace CameraBehaviour.DataLayer.Config.Action
{
    public abstract class ActionConfigBase : ConfigBase, IActionConfig
    {
        [Header("입력 감도 설정")]
        [FieldLabel("동작 딜레이")]
        [Tooltip("기본값은 0으로 0에 가까울수록 즉각적으로 동작합니다. (단위는 Milisecond입니다)")]
        [Range(0, 10000)] public int actionDelay = 0;

        [Header("속도")]
        [FieldLabel("동작 시간")]
        [Tooltip("동작을 완료하는데 걸리는 총 시간입니다. 0인 경우 즉각 적용합니다.")]
        [SerializeField][Min(0)] public float smoothTime = 0;
        [FieldLabel("주의")]
        [ReadOnly, TextArea]
        public string smoothTimeInfo =
@"동작 시간이 0이면, 중간 과정 없이 즉각 적용합니다. SmoothDamp 특성 상 정확한 시간이 아니며 해당 지점에 상당히 근접할 때까지의 시간으로 실제로는 그보다 조금 더 시간이 걸립니다.";

        [Header("영역 설정")]
        [SerializeReference, FieldLabel("동작 영역")] public AreaConfigBase area;
        [SerializeReference, FieldLabel("동작 최소 단위 (선택)")] public SectionConfigBase section;

        public float ActionDelay => actionDelay;

        // 전략 객체 분리 
        public abstract IActionStrategy CreateStrategy();

        public override void Validate(List<string> warnings)
        {
            base.Validate(warnings);
            if (actionDelay < 0f)
                warnings.Add($"{DisplayName} ({GetType().Name}): ActionDelay는 0 이상이어야 합니다.");
        }

        // 외부 서비스 등록
        public override void SetResolver(GameServiceBridge resolver)
        {
            base.SetResolver(resolver);

            // 범위 로직에도 서비스 등록
            area?.SetResolver(resolver);
            section?.SetResolver(resolver);
        }
    }
}