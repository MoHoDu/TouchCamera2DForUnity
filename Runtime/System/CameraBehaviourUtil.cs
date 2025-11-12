using CameraBehaviour.DataLayer.Input;
using VContainer;
using VContainer.Unity;

namespace CameraBehaviour.SystemLayer
{
    public class CameraBehaviourUtil : IStartable
    {
        private CameraBehaviourController _controller;
        private static CameraBehaviourController controller;

        [Inject]
        public CameraBehaviourUtil(
            CameraBehaviourController controller
        )
        {
            _controller = controller;
        }

        public void Start()
        {
            controller ??= _controller;
        }

        public static void CallActionUnit(string callingName, InputContext context)
        {
            controller?.OnCallingAction(callingName, context);
        }
    }
}