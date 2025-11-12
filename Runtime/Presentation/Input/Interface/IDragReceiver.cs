using CameraBehaviour.DataLayer.Config.Action.Move;
using UnityEngine;

namespace CameraBehaviour.PresentationLayer.Inputs.Interface
{
    public interface IDragReceiver
    {
        void OnDrag(Vector2 start, Vector2 end, DragState state, GameObject clickedObject = null);
    }
}