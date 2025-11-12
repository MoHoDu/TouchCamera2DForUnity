using UnityEngine;

namespace CameraBehaviour.PresentationLayer.Inputs.Interface
{
    public interface IPinchReceiver
    {
        void OnPinch(float delta, Vector2 center);
    }
}