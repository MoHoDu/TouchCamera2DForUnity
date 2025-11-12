using CameraBehaviour.PresentationLayer.Inputs;
using CameraBehaviour.PresentationLayer.Inputs.Interface;
using VContainer;
using VContainer.Unity;

namespace CameraBehaviour.Infrastructure.DI
{
    public class CameraAdapterRegister : IStartable
    {
        // Adapters
        [Inject] MouseInputAdapter _mouseAdapter;
        [Inject] TouchInputAdapter _touchAdapter;
        // Receivers
        [Inject] IClickReceiver _clickReceiver;
        [Inject] IScrollReceiver _scrollReceiver;
        [Inject] ITouchReceiver _touchReceiver;
        [Inject] IPinchReceiver _pinchReceiver;
        [Inject] IDragReceiver _drapReceiver;
        [Inject] IHoverReceiver _hoverReceiver;

        public void Start()
        {
            _mouseAdapter?.Construct(_clickReceiver, _scrollReceiver, _drapReceiver, _hoverReceiver);
            _touchAdapter?.Construct(_touchReceiver, _pinchReceiver, _drapReceiver);
        }
    }
}