using UnityEngine;

namespace CameraBehaviour.PresentationLayer.Inputs.Interface
{
    public interface IClickReceiver
    {
        void OnClickStartLeft(Vector2 position, GameObject clickedObject = null);
        void OnClickStartRight(Vector2 position, GameObject clickedObject = null);
        void OnClickStartWheel(Vector2 position, GameObject clickedObject = null);
        void OnClickHoldLeft(Vector2 position, GameObject clickedObject = null);
        void OnClickHoldRight(Vector2 position, GameObject clickedObject = null);
        void OnClickHoldWheel(Vector2 position, GameObject clickedObject = null);
        void OnClickEndLeft(Vector2 position, GameObject clickedObject = null);
        void OnClickEndRight(Vector2 position, GameObject clickedObject = null);
        void OnClickEndWheel(Vector2 position, GameObject clickedObject = null);
    }
}