using System.Collections;
using CameraBehaviour.DataLayer.Config.Action.Move;
using CameraBehaviour.PresentationLayer.Inputs.Interface;
using UnityEngine;
using VContainer;

namespace CameraBehaviour.PresentationLayer.Inputs
{
    public class TouchInputAdapter : MonoBehaviour
    {
        private ITouchReceiver _touchReceiver;
        private IDragReceiver _dragReceiver;
        private IPinchReceiver _pinchReceiver;

        // --- State Variables ---
        private const float DRAG_THRESHOLD_SQR = 49f; // 7x7 pixels, Use squared magnitude for efficiency
        private const float PINCH_DEBOUNCE_TIME = 0.1f;

        // State for Tap & Drag (1-finger gestures)
        private bool _isDragging;
        private DragState _dragState;
        private Vector2 _startPosition;
        private Vector2 _lastTouchPosition;

        // State for Pinch (2-finger gestures)
        private Coroutine _pinchDebounceCoroutine;
        private float _accumulatedPinchDelta;

        // Selected Object
        private GameObject _touchedObject;

        [Inject]
        public void Construct(
            ITouchReceiver touchReceiver,
            IPinchReceiver pinchReceiver,
            IDragReceiver dragReceiver
        )
        {
            _touchReceiver = touchReceiver;
            _pinchReceiver = pinchReceiver;
            _dragReceiver = dragReceiver;
        }

        private void Update()
        {
            // Prioritize multi-touch (pinch) over single-touch (drag/tap)
            if (Input.touchCount >= 2)
            {
                HandleMultiTouch();
            }
            else if (Input.touchCount == 1)
            {
                HandleSingleTouch();
            }
            else if (_isDragging)
            {
                _isDragging = false;
                _dragState = DragState.END;
                Touch touch = Input.GetTouch(0);
                _dragReceiver?.OnDrag(_lastTouchPosition, touch.position, _dragState, _touchedObject);
            }
        }

        /// <summary>
        /// Handles single-finger gestures: tap and drag.
        /// </summary>
        private void HandleSingleTouch()
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                // A finger just touched the screen: potential start of a tap or drag.
                case TouchPhase.Began:
                    _isDragging = false;
                    _startPosition = touch.position;
                    _lastTouchPosition = _startPosition; // Initialize last position
                    _touchedObject = GetTouchedObject(_startPosition); // 터치한 지점의 오브젝트 확인
                    _touchReceiver?.OnTouchStart(touch.position, _touchedObject);
                    break;

                // A finger is moving on the screen.
                case TouchPhase.Moved:
                    // If not already dragging, check if movement has exceeded the drag threshold.
                    if (!_isDragging && (touch.position - _startPosition).sqrMagnitude > DRAG_THRESHOLD_SQR)
                    {
                        _isDragging = true;
                        _dragState = DragState.START;
                    }
                    else if (_isDragging)
                    {
                        _dragState = DragState.DOING;
                    }

                    // 드래그
                    if (_isDragging)
                    {
                        _dragReceiver?.OnDrag(_lastTouchPosition, touch.position, _dragState, _touchedObject);
                    }
                    // 홀드
                    if (_touchedObject != null && GetTouchedObject(touch.position) == _touchedObject)
                    {
                        _touchReceiver.OnTouchHold(touch.position, _touchedObject);
                    }
                    _lastTouchPosition = touch.position; // Update last position for the next frame
                    break;

                // A finger was lifted from the screen.
                case TouchPhase.Ended:
                    _touchReceiver?.OnTouchEnd(touch.position, _touchedObject);
                    // Drag events have already been sent. Just reset the state.
                    _isDragging = false;
                    break;

                // The touch was cancelled (e.g., by the system).
                case TouchPhase.Canceled:
                    _isDragging = false;
                    break;
            }
        }

        /// <summary>
        /// Handles two-finger gestures: pinch-to-zoom.
        /// </summary>
        private void HandleMultiTouch()
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            // If this is the first frame of the pinch, cancel any ongoing drag and prepare for pinch.
            if (touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began)
            {
                _isDragging = false;
                _accumulatedPinchDelta = 0;
                return;
            }

            // If fingers are moving, calculate the change in distance and debounce the event.
            if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
            {
                Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;
                Vector2 touch2PrevPos = touch2.position - touch2.deltaPosition;

                float prevDistance = (touch1PrevPos - touch2PrevPos).magnitude;
                float currentDistance = (touch1.position - touch2.position).magnitude;

                float deltaDistance = currentDistance - prevDistance;
                _accumulatedPinchDelta += deltaDistance;

                // Debounce: If a coroutine is already running, stop it and start a new one.
                if (_pinchDebounceCoroutine != null)
                {
                    StopCoroutine(_pinchDebounceCoroutine);
                }
                _pinchDebounceCoroutine = StartCoroutine(PinchDebounceCoroutine());
            }
        }

        /// <summary>
        /// A coroutine that waits for a short period of inactivity during a pinch
        /// before firing the OnPinch event.
        /// </summary>
        private IEnumerator PinchDebounceCoroutine()
        {
            yield return new WaitForSeconds(PINCH_DEBOUNCE_TIME);

            // When the coroutine completes, the pinch has paused. Fire the event.
            if (Input.touchCount >= 2)
            {
                Vector2 center = (Input.GetTouch(0).position + Input.GetTouch(1).position) / 2;
                _pinchReceiver?.OnPinch(_accumulatedPinchDelta, center);
            }

            // Reset for the next pinch sequence.
            _accumulatedPinchDelta = 0f;
            _pinchDebounceCoroutine = null;
        }

        private GameObject GetTouchedObject(Vector2 pos)
        {
            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);
            if (hit.collider != null) return hit.collider.gameObject;
            else return null;
        }
    }
}
