using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using CameraBehaviour.DataLayer.Config.Action.Zoom;
using CameraBehaviour.DataLayer.Input;
using UnityEngine;

namespace CameraBehaviour.PresentationLayer.Output
{
    [Serializable]
    public class ZoomInfo
    {
        public ZoomActionConfig Config;
        public float OrthographicSize;
    }

    [RequireComponent(typeof(Camera))]
    public class CameraZoomViewer : MonoBehaviour
    {
        private Camera _targetCamera;
        public bool DuringAction = false;

        // 현재 액션 정보
        [SerializeField, ReadOnly] private ZoomInfo _zoomInfo = null;

        private static float MINGAPORTHOGRAPHICSIZE = 0.02f;
        private float _zoomVelocity = 0f;

        #region Unity Lifetime Cycle
        private void Awake()
        {
            TryGetComponent(out _targetCamera);
        }
        #endregion

        #region Camera Actions
        public async Task<bool> OnRequestZoom(ZoomActionConfig zoomInfo, InputContext context, CancellationToken token = default)
        {
            if (DuringAction && _zoomInfo != null)
            {
                SetInfo(zoomInfo, context);
                return true;
            }
            GenerateInfo(zoomInfo, context);
            return await StartNewAction(zoomInfo, context, token);
        }

        private void SetInfo(ZoomActionConfig zoomInfo, InputContext context)
        {
            _zoomInfo.Config = zoomInfo;
            _zoomInfo.OrthographicSize = context.RequestState.OrthographicSize;
        }

        private void GenerateInfo(ZoomActionConfig zoomInfo, InputContext context)
        {
            CancelAction();
            _zoomInfo = new ZoomInfo
            {
                Config = zoomInfo,
                OrthographicSize = context.RequestState.OrthographicSize
            };
        }

        public async Task<bool> StartNewAction(ZoomActionConfig zoomInfo, InputContext context, CancellationToken token = default)
        {
            float? size = context?.RequestState?.OrthographicSize ?? null;
            float duration = zoomInfo?.smoothTime ?? 0;
            if (size == null) return false;
            // smoothTime이 있는 경우 코루틴으로 점차 변경
            // 0인 경우 즉각적으로 변경
            if (Mathf.Approximately(duration, 0))
                SetOrthographicSizeDirect(size.Value);
            else
                await this.RunCoroutine(SetOrthographicSize(), token);
            return false;
        }

        public void CancelAction()
        {
            if (_zoomInfo != null)
                _targetCamera.orthographicSize = _zoomInfo.OrthographicSize;
            
            DuringAction = false;
            StopAllCoroutines();
        }

        public IEnumerator SetOrthographicSize()
        {
            if (_targetCamera == null) yield break;
            DuringAction = true;

            // 액션이 활성화된 동안 계속 실행
            while (DuringAction)
            {
                // 목표 사이즈와 감속 시간(duration)을 매 프레임 새로 가져옵니다.
                float targetSize = Mathf.Max(0.1f, _zoomInfo.OrthographicSize);
                float smoothTime = _zoomInfo.Config.smoothTime;

                // 목표에 거의 도달했으면 루프를 빠져나갈 수 있도록 처리 (선택적)
                if (Mathf.Abs(_targetCamera.orthographicSize - targetSize) < MINGAPORTHOGRAPHICSIZE)
                {
                    _targetCamera.orthographicSize = targetSize; // 마지막 값 보정
                    break;
                }

                // Mathf.SmoothDamp를 이용한 줌
                _targetCamera.orthographicSize = Mathf.SmoothDamp(
                    _targetCamera.orthographicSize,
                    targetSize,
                    ref _zoomVelocity, // 줌 속도 변수 (ref로 전달)
                    smoothTime
                );

                yield return null;
            }

            // While 루프가 끝나면 최종 정리
            _zoomVelocity = 0f;
            DuringAction = false;
        }

        public void SetOrthographicSizeDirect(float size)
        {
            if (_targetCamera == null) return;
            size = Mathf.Max(size, 0.1f);
            _targetCamera.orthographicSize = size;
        }
        #endregion
    }
}