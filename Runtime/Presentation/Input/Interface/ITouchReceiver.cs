using UnityEngine;

namespace CameraBehaviour.PresentationLayer.Inputs.Interface
{
    public interface ITouchReceiver
    {
        void OnTouchStart(Vector2 position, GameObject touchedObject = null);
        void OnTouchEnd(Vector2 position, GameObject touchedObject = null);
        void OnTouchHold(Vector2 position, GameObject touchedObject = null);
    }
}