using System.Collections.Generic;
using CameraBehaviour.Core.Strategy.Action;
using CameraBehaviour.Core.Strategy.Interface;
using UnityEngine;

namespace CameraBehaviour.DataLayer.Config.Action.Move
{
    [ClassLabel("패닝")]
    public class MoveActionConfig : ActionConfigBase
    {
        [Header("계수")]
        [FieldLabel("이동 계수")]
        [Tooltip("입력 값 x 해당 계수를 하여 이동 거리를 계산합니다.")]
        [SerializeField][Min(0)] public float MoveMultiplier = 1.0f;

        [Header("이동 방향")]
        [FieldLabel("가로 이동")]
        [Tooltip("카메라의 가로 이동 가능 여부를 설정합니다.")]
        [SerializeField] public bool MovableX = true;
        [FieldLabel("세로 이동")]
        [Tooltip("카메라의 세로 이동 가능 여부를 설정합니다.")]
        [SerializeField] public bool MovableY = true;
        [FieldLabel("정방향 이동")]
        [Tooltip("입력 방향과 동일한 방향으로 움직입니다.")]
        [SerializeField] public bool reversedDirection = false;

        [Header("임의 값 설정 (선택))")]
        [Tooltip("실제 입력 값이 아닌 설정한 임의의 값으로 작동합니다.")]
        [FieldLabel("임의 값 적용")]
        [SerializeField] public bool setCustom;
        [FieldLabel("이동 깂")]
        [SerializeField] public Vector2 customValue;

        public override IActionStrategy CreateStrategy() => CreateMoveStrategy();
        protected virtual CameraMoveStrategy CreateMoveStrategy() => new CameraMoveStrategy(this);

        public override void Validate(List<string> warnings)
        {
            base.Validate(warnings);
            if (MoveMultiplier < 0f)
                warnings.Add($"{DisplayName} (MoveAction): 이동 계수(moveMultiplier)는 0 이상이어야 합니다.");
            if (!MovableX && !MovableY)
                warnings.Add($"{DisplayName} (MoveAction): 가로 이동/세로 이동 모두 false일 수는 없습니다.");
        }
    }
}