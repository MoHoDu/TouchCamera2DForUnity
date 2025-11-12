using System.Collections.Generic;
using CameraBehaviour.Core.Strategy.Action;
using CameraBehaviour.Core.Strategy.Interface;
using UnityEngine;

namespace CameraBehaviour.DataLayer.Config.Action.Move
{
    public enum DragState
    {
        START,
        DOING,
        END
    }

    [ClassLabel("고정 거리 패닝")]
    public class FixedDistanceMoveActionConfig : MoveActionConfig
    {
        [Header("거리 설정")]
        [FieldLabel("이동 거리")]
        [Tooltip("한 번에 이동할 간격을 설정합니다.")]
        [SerializeField][Min(0.1f)] public float customDistance = 1.0f;

        public override IActionStrategy CreateStrategy() => CreateMoveStrategy();
        protected override CameraMoveStrategy CreateMoveStrategy() => new CameraFixedDistanceMoveStrategy(this);

        public override void Validate(List<string> warnings)
        {
            base.Validate(warnings);
        }
    }
}