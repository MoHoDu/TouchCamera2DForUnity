using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using CameraBehaviour.DataLayer.Config.Action;
using CameraBehaviour.DataLayer.Config.Action.Move;
using CameraBehaviour.DataLayer.Config.Action.Zoom;
using CameraBehaviour.DataLayer.Input;
using CameraBehaviour.DataLayer.State;
using CameraBehaviour.SystemLayer;
using UnityEngine;
using VContainer;

namespace CameraBehaviour.PresentationLayer.Output
{
    [RequireComponent(typeof(Camera))]
    public class CameraOutputViewer : MonoBehaviour
    {
        private Camera _targetCamera;
        public Camera Cam => _targetCamera;
        private CameraState _currentState;
        public CameraState CurrentState => _currentState;

        // time
        private static int MAXDELAYMILISECONDS = 10000;

        // Components about actions
        [SerializeField] CameraMoveViewer _move;
        [SerializeField] CameraZoomViewer _zoom;

        [Inject] private CameraBehaviourController _controller;

        #region Unity Lifetime Cycle
        private void Awake()
        {
            TryGetComponent<Camera>(out _targetCamera);
            _controller?.SetCameraOutputViewer(this);
            _currentState = new CameraState(_targetCamera);
        }

        private void Update()
        {
            if (_targetCamera != null && _targetCamera.transform.hasChanged)
                _currentState.SetValue(_targetCamera);
        }

        private void OnDestroy()
        {
            _controller?.RemoveOutputViewer(this);
        }
        #endregion

        #region Camera Actions
        public async Task<bool> RequestAction(ActionConfigBase config, InputContext context, CancellationToken token = default)
        {
            try
            {
                // 동작 전 딜레이 대기 
                if (config.actionDelay > 0)
                {
                    int delay = Mathf.Clamp(config.actionDelay, 0, MAXDELAYMILISECONDS);
                    await Task.Delay(delay, token);
                }

                // smooth 시간
                float duration = config.smoothTime;

                // 카메라 이동
                if (config is MoveActionConfig moveInfo)
                {
                    return await _move?.OnRequestMove(moveInfo, context, token);
                }
                // 카메라 줌
                else if (config is ZoomActionConfig zoomInfo)
                {
                    return await _zoom?.OnRequestZoom(zoomInfo, context);
                }
            }
            catch (System.OperationCanceledException)
            {
                // 작업 취소는 드래그/스크롤 시 발생하는 정상적인 동작이므로,
                // 예외를 여기서 잡아서 콘솔에 에러 로그가 표시되지 않도록 합니다.
                return false;
            }
            return true;
        }
        #endregion
    }
}