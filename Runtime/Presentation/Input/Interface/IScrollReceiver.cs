using UnityEngine;

namespace CameraBehaviour.PresentationLayer.Inputs.Interface
{
    public interface IScrollReceiver
    {
        void OnScroll(float delta, Vector2 position);
    }
}