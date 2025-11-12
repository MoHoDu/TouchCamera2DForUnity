using CameraBehaviour.Core.Strategy.Interface;
using CameraBehaviour.DataLayer.Config.Action.Interface;
using CameraBehaviour.DataLayer.Config.Action.Zoom;
using CameraBehaviour.DataLayer.Input;
using CameraBehaviour.DataLayer.State;
using UnityEngine;

namespace CameraBehaviour.Core.Strategy.Zoom
{
    public class CameraZoomStrategy : IActionStrategy
    {
        protected ZoomActionConfig Config;
        IActionConfig IActionStrategy.Config => Config;

        public CameraZoomStrategy(ZoomActionConfig config)
        {
            Config = config;
        }

        public virtual void Calculate(InputContext context, Camera camera, CameraState currentState)
        {
            // 커스텀 값이 있는 경우
            if (Config.setCustom)
            {
                int direction = Config.isZoom ? 1 : -1;
                context.Delta = Config.zoomValue * direction;
            }

            context.PreviousState = currentState;
            context.RequestState = currentState;

            var amount = context.Delta * Config.zoomMultiplier;
            if (Config.reversedDirection) amount *= -1f;

            // amount가 양수인 경우 줌 인 -> 사이즈 축소
            // amount가 음수인 경우 줌 아웃 -> 사이즈 확대
            context.RequestState.OrthographicSize = Mathf.Max(currentState.OrthographicSize - amount, 0.1f);
        }
    }
}