using System.Collections.Generic;
using System.Text;
using System.Threading;
using CameraBehaviour.Core.Strategy.Interface;
using CameraBehaviour.DataLayer.Config;
using CameraBehaviour.DataLayer.Config.Action.Move;
using CameraBehaviour.DataLayer.Config.Input;
using CameraBehaviour.DataLayer.Input;
using CameraBehaviour.DataLayer.State;
using CameraBehaviour.PresentationLayer.Inputs.Interface;
using CameraBehaviour.PresentationLayer.Output;
using UnityEngine;

namespace CameraBehaviour.SystemLayer
{
    public class CameraBehaviourController : IClickReceiver, IScrollReceiver, ITouchReceiver, IPinchReceiver, IDragReceiver, IHoverReceiver
    {
        // DI
        private CameraOutputViewer _outputViewer;
        private CameraProfileManager _profileManger;

        // Cancellation
        private CancellationTokenSource _cancelExecuteSource;

        // Debug
        private CameraState _prevState = new();

        public void SetProfileManager(CameraProfileManager profile)
        {
            _profileManger = profile;
            profile.OnChangedProfile += CancelExecute;
            _cancelExecuteSource = new CancellationTokenSource(); // Initialize CancellationTokenSource
        }

        public void SetViewer(CameraOutputViewer viewer)
        {
            _outputViewer = viewer;
        }

        #region OnClickEvent
        public void OnClickEndLeft(Vector2 position, GameObject clickedObject = null)
        {
            var behaviourInfo = _profileManger?.GetBehaviour(typeof(MouseClickConfig));
            if (behaviourInfo == null) return;

            InputContext context = new InputContext();
            context.Delta = (int)ClickState.END;
            context.StartPosition = position;
            context.Direction = Vector2.left;
            context.RequestTargetObject = clickedObject;
            ExecuteActionUnits(context, behaviourInfo, _cancelExecuteSource.Token);
        }

        public void OnClickEndRight(Vector2 position, GameObject clickedObject = null)
        {
            var behaviourInfo = _profileManger?.GetBehaviour(typeof(MouseClickConfig));
            if (behaviourInfo == null) return;

            InputContext context = new InputContext();
            context.Delta = (int)ClickState.END;
            context.StartPosition = position;
            context.Direction = Vector2.right;
            context.RequestTargetObject = clickedObject;
            ExecuteActionUnits(context, behaviourInfo, _cancelExecuteSource.Token);
        }

        public void OnClickEndWheel(Vector2 position, GameObject clickedObject = null)
        {
            var behaviourInfo = _profileManger?.GetBehaviour(typeof(MouseClickConfig));
            if (behaviourInfo == null) return;

            InputContext context = new InputContext();
            context.Delta = (int)ClickState.END;
            context.StartPosition = position;
            context.Direction = Vector2.zero;
            context.RequestTargetObject = clickedObject;
            ExecuteActionUnits(context, behaviourInfo, _cancelExecuteSource.Token);
        }

        public void OnClickStartLeft(Vector2 position, GameObject clickedObject = null)
        {
            var behaviourInfo = _profileManger?.GetBehaviour(typeof(MouseClickConfig));
            if (behaviourInfo == null) return;

            InputContext context = new InputContext();
            context.Delta = (int)ClickState.START;
            context.StartPosition = position;
            context.Direction = Vector2.left;
            context.RequestTargetObject = clickedObject;
            ExecuteActionUnits(context, behaviourInfo, _cancelExecuteSource.Token);
        }

        public void OnClickStartRight(Vector2 position, GameObject clickedObject = null)
        {
            var behaviourInfo = _profileManger?.GetBehaviour(typeof(MouseClickConfig));
            if (behaviourInfo == null) return;

            InputContext context = new InputContext();
            context.Delta = (int)ClickState.START;
            context.StartPosition = position;
            context.Direction = Vector2.right;
            context.RequestTargetObject = clickedObject;
            ExecuteActionUnits(context, behaviourInfo, _cancelExecuteSource.Token);
        }

        public void OnClickStartWheel(Vector2 position, GameObject clickedObject = null)
        {
            var behaviourInfo = _profileManger?.GetBehaviour(typeof(MouseClickConfig));
            if (behaviourInfo == null) return;

            InputContext context = new InputContext();
            context.Delta = (int)ClickState.START;
            context.StartPosition = position;
            context.Direction = Vector2.zero;
            context.RequestTargetObject = clickedObject;
            ExecuteActionUnits(context, behaviourInfo, _cancelExecuteSource.Token);
        }

        public void OnClickHoldLeft(Vector2 position, GameObject clickedObject = null)
        {
            var behaviourInfo = _profileManger?.GetBehaviour(typeof(MouseClickConfig));
            if (behaviourInfo == null) return;

            InputContext context = new InputContext();
            context.Delta = (int)ClickState.HOLD;
            context.StartPosition = position;
            context.Direction = Vector2.left;
            context.RequestTargetObject = clickedObject;
            ExecuteActionUnits(context, behaviourInfo, _cancelExecuteSource.Token);
        }

        public void OnClickHoldRight(Vector2 position, GameObject clickedObject = null)
        {
            var behaviourInfo = _profileManger?.GetBehaviour(typeof(MouseClickConfig));
            if (behaviourInfo == null) return;

            InputContext context = new InputContext();
            context.Delta = (int)ClickState.HOLD;
            context.StartPosition = position;
            context.Direction = Vector2.right;
            context.RequestTargetObject = clickedObject;
            ExecuteActionUnits(context, behaviourInfo, _cancelExecuteSource.Token);
        }

        public void OnClickHoldWheel(Vector2 position, GameObject clickedObject = null)
        {
            var behaviourInfo = _profileManger?.GetBehaviour(typeof(MouseClickConfig));
            if (behaviourInfo == null) return;

            InputContext context = new InputContext();
            context.Delta = (int)ClickState.HOLD;
            context.StartPosition = position;
            context.Direction = Vector2.zero;
            context.RequestTargetObject = clickedObject;
            ExecuteActionUnits(context, behaviourInfo, _cancelExecuteSource.Token);
        }
        #endregion

        #region OnTouchEvent
        public void OnTouchStart(Vector2 position, GameObject touchedObject = null)
        {
            var behaviourInfo = _profileManger?.GetBehaviour(typeof(ScreenTouchConfig));
            if (behaviourInfo == null) return;

            InputContext context = new InputContext();
            context.Delta = (int)ClickState.START;
            context.StartPosition = position;
            context.RequestTargetObject = touchedObject;
            ExecuteActionUnits(context, behaviourInfo, _cancelExecuteSource.Token);
        }

        public void OnTouchEnd(Vector2 position, GameObject touchedObject = null)
        {
            var behaviourInfo = _profileManger?.GetBehaviour(typeof(ScreenTouchConfig));
            if (behaviourInfo == null) return;

            InputContext context = new InputContext();
            context.Delta = (int)ClickState.END;
            context.StartPosition = position;
            context.RequestTargetObject = touchedObject;
            ExecuteActionUnits(context, behaviourInfo, _cancelExecuteSource.Token);
        }

        public void OnTouchHold(Vector2 position, GameObject touchedObject = null)
        {
            var behaviourInfo = _profileManger?.GetBehaviour(typeof(ScreenTouchConfig));
            if (behaviourInfo == null) return;

            InputContext context = new InputContext();
            context.Delta = (int)ClickState.HOLD;
            context.StartPosition = position;
            context.RequestTargetObject = touchedObject;
            ExecuteActionUnits(context, behaviourInfo, _cancelExecuteSource.Token);
        }
        #endregion

        #region OnScrollEvent
        public void OnScroll(float delta, Vector2 position)
        {
            var behaviourInfo = _profileManger?.GetBehaviour(typeof(MouseWheelConfig));
            if (behaviourInfo == null) return;

            InputContext context = new InputContext();
            context.Delta = delta;
            context.StartPosition = position;
            context.EndPosition = position;
            ExecuteActionUnits(context, behaviourInfo, _cancelExecuteSource.Token);
        }
        #endregion

        #region OnPinchEvent
        public void OnPinch(float delta, Vector2 center)
        {
            var behaviourInfo = _profileManger?.GetBehaviour(typeof(ScreenPinchConfig));
            if (behaviourInfo == null) return;

            InputContext context = new InputContext();
            context.Delta = delta;
            context.StartPosition = center;
            context.EndPosition = center;
            ExecuteActionUnits(context, behaviourInfo, _cancelExecuteSource.Token);
        }
        #endregion

        #region OnDragEvent
        public void OnDrag(Vector2 start, Vector2 end, DragState state, GameObject clickedObject = null)
        {
            if (Mathf.Approximately(Vector2.Distance(start, end), 0)) return;

            var behaviourInfo = _profileManger?.GetBehaviour(typeof(MouseDragConfig));
            if (behaviourInfo == null) return;

            InputContext context = new InputContext();
            context.StartPosition = start;
            context.EndPosition = end;
            context.RequestTargetObject = clickedObject;
            context.ExtraData.Add(typeof(DragState).Name, state);

            var gap = end - start;
            context.Delta = Vector2.Distance(start, end);
            context.Direction = (end - start).normalized;
            if (Mathf.Abs(gap.x) > Mathf.Abs(gap.y))
            {
                if (gap.x < 0) context.Delta *= -1f;
            }
            else if (Mathf.Abs(gap.x) < Mathf.Abs(gap.y))
            {
                if (gap.y < 0) context.Delta *= -1f;
            }
            ExecuteActionUnits(context, behaviourInfo, _cancelExecuteSource.Token);
        }
        #endregion

        #region On Hovering
        public void OnHover(Vector2 position, GameObject hoveredObject)
        {
            var behaviourInfo = _profileManger?.GetBehaviour(typeof(MouseHoverConfig));
            if (behaviourInfo == null) return;

            InputContext context = new InputContext();
            context.Delta = (int)ClickState.NONE;
            context.StartPosition = position;
            context.RequestTargetObject = hoveredObject;
            ExecuteActionUnits(context, behaviourInfo, _cancelExecuteSource.Token);
        }
        #endregion

        #region On Calling
        /// <summary>
        /// 직접 호출명으로 동작을 실행
        /// </summary>
        /// <param name="callingName">호출명</param>
        /// <param name="context">입력값</param>
        public void OnCallingAction(string callingName, InputContext context)
        {
            if (string.IsNullOrEmpty(callingName) || context == null) return;

            var behaviourInfo = _profileManger?.GetBehaviour(callingName);
            if (behaviourInfo == null) return;

            ExecuteActionUnits(context, behaviourInfo, _cancelExecuteSource.Token);
        }
        #endregion

        #region Set Output
        public void SetCameraOutputViewer(CameraOutputViewer viewer)
        {
            _outputViewer = viewer;
            if (_outputViewer.Cam != null) _prevState.SetValue(_outputViewer.Cam);
        }

        public void RemoveOutputViewer(CameraOutputViewer viewer)
        {
            if (_outputViewer == viewer)
                _outputViewer = null;
        }
        #endregion

        #region Action Flow
        private async void ExecuteActionUnits(InputContext input, ActiveBehaviourInfo info, CancellationToken token)
        {
            // null이 되어 계산이 중단되지 않도록 빈 상태값을 채움
            input.PreviousState ??= _outputViewer?.CurrentState.DeepCopy();
            input.RequestState ??= _outputViewer?.CurrentState.DeepCopy();
            // 시작 액션 유닛 선택 
            LinkedListNode<CameraActionUnit> current = info.SortedActionUnits.First;
            // 액션 유닛을 순회하면서 동작 
            while (current != null && current.Value != null)
            {
                // 취소 감지
                if (token.IsCancellationRequested) return;

                // 현재 유닛
                var unit = current.Value;

                // 유닛 검증 (설정 자체에 문제가 있는지 확인)
                List<string> warning = new List<string>();
                unit.Validate(warning);
                if (warning.Count == 0)
                {
                    // 입력 상태 확인
                    if (unit.input.ValidateInput(input))
                    {
                        // 입력 알림
                        DebugVisualizer.LogInput(input);

                        // 알맞은 전략 선택
                        IActionStrategy actionStrategy = unit.action.CreateStrategy();
                        // 현재 카메라 상태 복사본
                        CameraState currentState = _outputViewer.CurrentState.DeepCopy();

                        // Section에 맞게 변화량 조정
                        unit.action.section?.Calibrate(input);
                        // 목표 지점 계산
                        actionStrategy.Calculate(input, _outputViewer.Cam, currentState);
                        // Area에 맞게 범위 한정
                        unit.action.area?.Calibrate(input);

                        // 카메라 동작 대기
                        if (await _outputViewer?.RequestAction(unit.action, input, token))
                        {
                            // 중단되지 않고 동작 완료 시 로깅
                            if (_prevState != null)
                                DebugVisualizer.LogOutput(_prevState, _outputViewer.CurrentState);

                            _prevState.SetValue(_outputViewer.Cam);
                        }
                    }
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("액션 유닛 검증 실패");
                    warning.ForEach(w => sb.AppendLine($"- {w}"));
                    DebugVisualizer.Log(sb.ToString());
                }

                // 다음 유닛으로 이동
                current = current.Next;
            }
        }

        private void CancelExecute()
        {
            _cancelExecuteSource?.Cancel();
            _cancelExecuteSource = new CancellationTokenSource();
        }
        #endregion
    }
}