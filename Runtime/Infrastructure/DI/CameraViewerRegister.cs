using CameraBehaviour.DataLayer.Config;
using CameraBehaviour.PresentationLayer.Output;
using CameraBehaviour.SystemLayer;
using VContainer;
using VContainer.Unity;

namespace CameraBehaviour.Infrastructure.DI
{
    public class CameraViewerRegister : IStartable
    {
        [Inject] CameraBehaviourController _controller;
        [Inject] CameraOutputViewer _viewer;

        public void Start()
        {
            _controller?.SetViewer(_viewer);
        }
    }
}