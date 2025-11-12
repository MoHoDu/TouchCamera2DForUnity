using UnityEngine;
using CameraBehaviour.DataLayer.Config.Action.Move;
using CameraBehaviour.DataLayer.Input;
using CameraBehaviour.DataLayer.State;
using System.Threading.Tasks;

namespace CameraBehaviour.Core.Strategy.Action
{
    public class CameraFixedDistanceMoveStrategy : CameraMoveStrategy
    {
        FixedDistanceMoveActionConfig _distanceConfig => (FixedDistanceMoveActionConfig)Config;

        public CameraFixedDistanceMoveStrategy(FixedDistanceMoveActionConfig config) : base(config)
        {
            Config = config;
        }

        public override void Calculate(InputContext context, Camera camera, CameraState currentState)
        {
            // 커스텀 값이 있는 경우
            SetCustomValues(context);

            // 화면 시작점과 끝점을 월드 좌표로 변환
            (Vector3 worldStartPos, Vector3 worldEndPos) = GetWorldPoint(context, camera);
            if (!Config.MovableX) worldEndPos.x = worldStartPos.x;
            if (!Config.MovableY) worldEndPos.y = worldStartPos.y;

            // 월드 변위 계산
            Vector3 displacement = (worldEndPos - worldStartPos).normalized;
            displacement *= _distanceConfig.customDistance;

            // 월드 변위를 적용하여 목표 지점을 계산
            // (참고: 일반적으로 카메라 이동(패닝)은 입력 방향과 반대로 움직임
            Vector3 targetPos = currentState.Position - displacement * Config.MoveMultiplier;   // 목표 지점(범위 제한 전) 

            // 이전 정보와 목표 정보 저장
            // 요청 상태는 원래 상태에서 위치만 타겟 지점으로 설정
            CameraState targetState = currentState.DeepCopy();
            targetState.Position = targetPos;
            SetState(context, currentState, targetState);
        }
    }
}