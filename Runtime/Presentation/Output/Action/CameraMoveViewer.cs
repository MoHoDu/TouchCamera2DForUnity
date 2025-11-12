using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using CameraBehaviour.DataLayer.Config.Action.Move;
using CameraBehaviour.DataLayer.Input;
using CameraBehaviour.SystemLayer;
using UnityEngine;
using VContainer;

namespace CameraBehaviour.PresentationLayer.Output
{
    [Serializable]
    public class MoveInfo
    {
        public MoveActionConfig Config;
        public Vector3 Position;
    }

    [RequireComponent(typeof(Camera))]
    public class CameraMoveViewer : MonoBehaviour
    {
        private Camera _targetCamera;
        public bool DuringAction = false;

        // 현재 액션 정보
        [SerializeField, ReadOnly] private MoveInfo _moveInfo = null;

        private static float MINDISTANCETOMOVE = 0.03f;
        private Vector3 _moveVelocity = Vector3.zero;

        #region Unity Lifetime Cycle
        private void Awake()
        {
            TryGetComponent(out _targetCamera);
        }
        #endregion

        #region Camera Actions
        public async Task<bool> OnRequestMove(MoveActionConfig moveInfo, InputContext context, CancellationToken token = default)
        {
            if (DuringAction && _moveInfo != null)
            {
                SetInfo(moveInfo, context);
                return true;
            }
            GenerateInfo(moveInfo, context);
            return await StartNewAction(moveInfo, context, token);
        }

        private void SetInfo(MoveActionConfig moveInfo, InputContext context)
        {
            _moveInfo.Config = moveInfo;
            _moveInfo.Position = context.RequestState.Position;
            _moveInfo.Position.z = _targetCamera.transform.position.z;
        }

        private void GenerateInfo(MoveActionConfig moveInfo, InputContext context)
        {
            CancelAction();
            _moveInfo = new MoveInfo
            {
                Config = moveInfo,
                Position = context.RequestState.Position
            };
            _moveInfo.Position.z = _targetCamera.transform.position.z;
        }

        private async Task<bool> StartNewAction(MoveActionConfig moveInfo, InputContext context, CancellationToken token = default)
        {
            Vector3? targetPos = context?.RequestState?.Position ?? null;
            float duration = moveInfo?.smoothTime ?? 0;
            if (targetPos == null) return false;
            // smoothTime이 있는 경우 코루틴으로 이동
            // 0인 경우 즉각적으로 이동(텔레포트)
            if (Mathf.Approximately(duration, 0f))
                MoveCameraDirect(targetPos.Value);
            else
                await this.RunCoroutine(MoveCamera(), token);
            return false;
        }

        public void CancelAction()
        {
            if (_moveInfo != null)
                _targetCamera.transform.position = new Vector3(_moveInfo.Position.x, _moveInfo.Position.y, _targetCamera.transform.position.z);

            DuringAction = false;
            StopAllCoroutines();
        }

        public IEnumerator MoveCamera()
        {
            if (_targetCamera == null) yield break;
            DuringAction = true;

            // 액션이 활성화된 동안 계속 실행
            while (DuringAction)
            {
                // 목표 위치와 감속 시간(duration)은 매 프레임 _currentActionInfo에서 새로 가져옵니다.
                Vector3 requestPos = _moveInfo.Position;
                Vector3 targetPosition = new Vector3(requestPos.x, requestPos.y, _targetCamera.transform.position.z);
                float smoothTime = _moveInfo.Config.smoothTime;

                // 목표에 거의 도달했으면 루프를 중단할 수 있습니다. (선택적)
                if (Vector3.Distance(_targetCamera.transform.position, targetPosition) < MINDISTANCETOMOVE)
                {
                    // 마지막 위치 보정
                    _targetCamera.transform.position = targetPosition;
                    break;
                }

                // SmoothDamp를 이용한 이동
                _targetCamera.transform.position = Vector3.SmoothDamp(
                    _targetCamera.transform.position,
                    targetPosition,
                    ref _moveVelocity, // 현재 속도 (ref로 전달되어 함수 내부에서 업데이트됨)
                    smoothTime
                );

                yield return null;
            }

            // While 루프가 끝나면 최종 정리
            _moveVelocity = Vector3.zero;
            DuringAction = false;
        }

        public void MoveCameraDirect(Vector2 pos)
        {
            if (_targetCamera == null) return;
            _targetCamera.transform.position = new Vector3(pos.x, pos.y, _targetCamera.transform.position.z);
        }
        #endregion
    }
}