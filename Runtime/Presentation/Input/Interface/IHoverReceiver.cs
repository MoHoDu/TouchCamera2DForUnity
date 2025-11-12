using UnityEngine;

namespace CameraBehaviour.PresentationLayer.Inputs.Interface
{
    public interface IHoverReceiver
    {
        void OnHover(Vector2 position, GameObject hoveredObject);
    }
}