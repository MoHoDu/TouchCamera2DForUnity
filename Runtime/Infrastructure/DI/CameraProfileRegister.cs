using CameraBehaviour.DataLayer.Config;
using CameraBehaviour.SystemLayer;
using VContainer;
using VContainer.Unity;

namespace CameraBehaviour.Infrastructure.DI
{
    public class CameraProfileRegister : IStartable
    {
        [Inject] CameraProfileManager _manager;
        [Inject] CameraBehaviourController _controller;
        [Inject] CameraBehaviourProfile _profile;

        public void Start()
        {
            _controller?.SetProfileManager(_manager);
            _manager?.SetBehaviourProfile(_profile);
        }
    }
}