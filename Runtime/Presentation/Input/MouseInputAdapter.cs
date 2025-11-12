using System;
using System.Collections.Generic;
using CameraBehaviour.PresentationLayer.Inputs.Interface;
using UnityEngine;
using VContainer;
using CameraBehaviour.DataLayer.Config.Action.Move;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CameraBehaviour.PresentationLayer.Inputs
{
    public class MouseInputAdapter : MonoBehaviour
    {
        // Receivers
        private IClickReceiver _clickReceiver;
        private IDragReceiver _dragReceiver;
        private IHoverReceiver _hoverReceiver;
        private IScrollReceiver _scrollReceiver;

        // Drag & click Events
        private Action<Vector2, GameObject>[] _clickStartEvents;
        private Action<Vector2, GameObject>[] _clickEndEvents;
        private Action<Vector2, GameObject>[] _heldEvents;

        // Drag & Click state
        private bool _isDragging;
        private DragState _dragState;
        private Vector2 _dragStartPosition;
        private Vector2 _lastMouseLeftPosition;
        private Vector2 _lastMouseRightPosition;
        private Vector2 _lastMouseWheelPosition;
        private Vector2[] _lastMousePositions;
        private const float DRAG_THRESHOLD_SQR = 25f; // Using squared magnitude for efficiency (5px threshold)

        // Selected Object
        private GameObject _leftClickedObject;
        private GameObject _rightClickedObject;
        private GameObject _wheelClickedObject;
        private GameObject[] _clickedObjects;

        [Inject]
        public void Construct(
            IClickReceiver clickReceiver,
            IScrollReceiver scrollReceiver,
            IDragReceiver dragReceiver,
            IHoverReceiver hoverReceiver
        )
        {
            _clickReceiver = clickReceiver;
            _scrollReceiver = scrollReceiver;
            _dragReceiver = dragReceiver;
            _hoverReceiver = hoverReceiver;
            SetValues();
        }

        private void SetValues()
        {
            _lastMousePositions = new Vector2[] { _lastMouseLeftPosition, _lastMouseRightPosition, _lastMouseWheelPosition };
            _clickedObjects = new GameObject[] { _leftClickedObject, _rightClickedObject, _wheelClickedObject };

            if (_clickReceiver != null)
            {
                _clickStartEvents = new Action<Vector2, GameObject>[] { _clickReceiver.OnClickStartLeft, _clickReceiver.OnClickStartRight, _clickReceiver.OnClickStartWheel };
                _clickEndEvents = new Action<Vector2, GameObject>[] { _clickReceiver.OnClickEndLeft, _clickReceiver.OnClickEndRight, _clickReceiver.OnClickEndWheel };
                _heldEvents = new Action<Vector2, GameObject>[] { _clickReceiver.OnClickHoldLeft, _clickReceiver.OnClickHoldRight, _clickReceiver.OnClickHoldWheel };
            }
        }

        private void Update()
        {
            HandleScrollInput();
            HandleMouseClickAndDrag();
        }

        private void HandleMouseClickAndDrag()
        {
            // MOUSE BUTTON DOWN: Potential start of a click or drag
            OnMouseButtonDown();

            // MOUSE BUTTON HELD: It might be a drag.
            if (OnMouseButton()?.Count == 0)
            {
                // 아무런 클릭이 없는 경우, 마우스 호버 체크
                HandleMouseHover();
            }

            // MOUSE BUTTON UP: Finalize click or drag
            OnMouseButtonUp();
        }

        private void HandleScrollInput()
        {
            float scrollInput = Input.mouseScrollDelta.y;
            if (Mathf.Abs(scrollInput) > 0f)
            {
                _scrollReceiver?.OnScroll(scrollInput, Input.mousePosition);
            }
        }

        private void HandleMouseHover()
        {
            if (_hoverReceiver == null) return;
            Vector2 mousePosition = Input.mousePosition;
            GameObject target = GetTargetObject(mousePosition);
            if (target != null) _hoverReceiver.OnHover(mousePosition, target);
        }

        private List<int> OnMouseButtonDown()
        {
            Vector2? mousePosition = null;
            List<int> buttonIndexes = new List<int>();
            for (int i = 0; i < 3; i++)
            {
                if (Input.GetMouseButtonDown(i))
                {
                    if (!mousePosition.HasValue) mousePosition = Input.mousePosition;
                    if (i == 0) _dragStartPosition = Input.mousePosition;
                    _lastMousePositions[i] = mousePosition.Value;
                    _clickedObjects[i] = GetTargetObject(mousePosition.Value);
                    if (_clickReceiver != null) _clickStartEvents[i].Invoke(_lastMousePositions[i], _clickedObjects[i]);
                    buttonIndexes.Add(i);
                }
            }
            return buttonIndexes;
        }

        private List<int> OnMouseButton()
        {
            Vector2? mousePosition = null;
            List<int> buttonIndexes = new List<int>();
            for (int i = 0; i < 3; i++)
            {
                if (Input.GetMouseButton(i))
                {
                    if (!mousePosition.HasValue) mousePosition = Input.mousePosition;
                    if (i == 0)
                    {
                        if (!_isDragging && (mousePosition.Value - _dragStartPosition).sqrMagnitude > DRAG_THRESHOLD_SQR)
                        {
                            _isDragging = true;
                            _dragState = DragState.START;
                        }
                        else if (_isDragging)
                        {
                            _dragState = DragState.DOING;
                        }

                        if (_isDragging)
                            _dragReceiver?.OnDrag(_dragStartPosition, mousePosition.Value, _dragState, _leftClickedObject);
                    }

                    if (_clickReceiver != null && GetTargetObject(mousePosition.Value) == _clickedObjects[i])
                    {
                        _heldEvents[i]?.Invoke(_lastMousePositions[i], _clickedObjects[i]);
                    }
                    buttonIndexes.Add(i);
                }
            }
            return buttonIndexes;
        }

        private List<int> OnMouseButtonUp()
        {
            Vector2? mousePosition = null;
            List<int> buttonIndexes = new List<int>();
            for (int i = 0; i < 3; i++)
            {
                if (Input.GetMouseButtonUp(i))
                {
                    if (!mousePosition.HasValue) mousePosition = Input.mousePosition;
                    if (i == 0 && _isDragging)
                    {
                        _dragState = DragState.END;
                        _isDragging = false;
                        _dragReceiver?.OnDrag(_dragStartPosition, mousePosition.Value, _dragState, _clickedObjects[0]);
                    }
                    if (_clickReceiver != null) _clickEndEvents[i].Invoke(mousePosition.Value, _clickedObjects[i]);
                    buttonIndexes.Add(i);
                }
            }
            return buttonIndexes;
        }

        private GameObject GetTargetObject(Vector2 pos)
        {
            Ray ray = Camera.main.ScreenPointToRay(pos);
#if UNITY_EDITOR
            if (UnityEditor.Selection.activeGameObject == gameObject)
            {
                Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);
            }
#endif
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray);
            var target = hit.collider != null ? hit.collider.gameObject : null;
            return target;
        }
    }
}
