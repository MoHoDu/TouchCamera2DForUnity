using UnityEngine;
using CameraBehaviour.Core.Strategy.Interface;
using CameraBehaviour.DataLayer.Config.Action.Interface;
using CameraBehaviour.DataLayer.Config.Action.Move;
using CameraBehaviour.DataLayer.Input;
using CameraBehaviour.DataLayer.State;

namespace CameraBehaviour.Core.Strategy.Action
{
    public class CameraMoveStrategy : IActionStrategy
    {
        protected MoveActionConfig Config;
        IActionConfig IActionStrategy.Config => Config;

        public CameraMoveStrategy(MoveActionConfig config)
        {
            Config = config;
        }

        public virtual void Calculate(InputContext context, Camera camera, CameraState currentState)
        {
            // 커스텀 값이 있는 경우
            SetCustomValues(context);

            // 화면 시작점과 끝점을 월드 좌표로 변환
            (Vector3 worldStartPos, Vector3 worldEndPos) = GetWorldPoint(context, camera);

            // 월드 변위 계산
            Vector3 displacement = GetDisplacement(worldStartPos, worldEndPos);

            // 월드 변위를 적용하여 목표 지점을 계산
            // (참고: 일반적으로 카메라 이동(패닝)은 입력 방향과 반대로 움직임
            Vector3 targetPos = currentState.Position - displacement * Config.MoveMultiplier;   // 목표 지점(범위 제한 전)  

            // 이전 정보와 목표 정보 저장
            // 요청 상태는 원래 상태에서 위치만 타겟 지점으로 설정
            CameraState targetState = currentState.DeepCopy();
            targetState.Position = targetPos;
            SetState(context, currentState, targetState);
        }

        protected (Vector3 start, Vector3 end) GetWorldPoint(InputContext context, Camera camera)
        {
            float distance = Mathf.Abs(camera.transform.position.z - 0f);
            Vector3 worldStartPos = camera.ScreenToWorldPoint(new Vector3(context.StartPosition.x, context.StartPosition.y, distance));
            Vector3 worldEndPos = camera.ScreenToWorldPoint(new Vector3(context.EndPosition.x, context.EndPosition.y, distance));
            return (worldStartPos, worldEndPos);
        }

        protected Vector3 GetDisplacement(Vector3 worldStartPos, Vector3 worldEndPos)
        {
            Vector3 displacement = worldEndPos - worldStartPos; // 변위 벡터
            if (Config.reversedDirection) displacement *= -1f;
            if (!Config.MovableX) displacement.x = 0;
            if (!Config.MovableY) displacement.y = 0;
            return displacement;
        }

        protected void SetCustomValues(InputContext context)
        {
            if (Config.setCustom)
            {
                context.EndPosition = context.StartPosition + Config.customValue;
                var gap = context.EndPosition - context.StartPosition;
                context.Delta = Vector2.Distance(context.StartPosition, context.EndPosition);
                if (Mathf.Abs(gap.x) > Mathf.Abs(gap.y))
                {
                    if (gap.x < 0) context.Delta *= -1f;
                }
                else if (Mathf.Abs(gap.x) < Mathf.Abs(gap.y))
                {
                    if (gap.y < 0) context.Delta *= -1f;
                }
            }
        }

        protected void SetState(InputContext context, CameraState prev, CameraState request)
        {
            context.PreviousState = prev;
            context.RequestState = request;
        }
    }
}