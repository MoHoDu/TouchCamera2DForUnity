using CameraBehaviour.DataLayer.Config.Action.Interface;
using CameraBehaviour.DataLayer.Input;
using CameraBehaviour.DataLayer.State;
using UnityEngine;

namespace CameraBehaviour.Core.Strategy.Interface
{
    public interface IActionStrategy
    {
        public IActionConfig Config { get; }

        void Calculate(InputContext context, Camera camera, CameraState currentState);
    }
}