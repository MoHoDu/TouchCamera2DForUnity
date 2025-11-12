using CameraBehaviour.DataLayer.Config.Action.Zoom;
using CameraBehaviour.DataLayer.Input;
using CameraBehaviour.DataLayer.State;
using UnityEngine;

namespace CameraBehaviour.Core.Strategy.Zoom
{
    public class CameraStepZoomStrategy : CameraZoomStrategy
    {
        StepZoomActionConfig _stepConfig;

        public CameraStepZoomStrategy(StepZoomActionConfig config) : base(config)
        {
            _stepConfig = config;
            _stepConfig.stepSizeList.Sort();
        }

        public override void Calculate(InputContext context, Camera camera, CameraState currentState)
        {
            // 커스텀 값이 있는 경우
            if (Config.setCustom)
            {
                int direction = Config.isZoom ? 1 : -1;
                context.Delta = Config.zoomValue * direction;
            }

            context.PreviousState = currentState;
            context.RequestState = currentState;
            
            float prevSize = context.PreviousState.OrthographicSize;
            float delta = context.Delta;
            if (Config.reversedDirection) delta *= -1f;
            if (delta < 0)
            {
                for (int i = 0;  i < _stepConfig.stepSizeList.Count; i++)
                {
                    float size = _stepConfig.stepSizeList[i];
                    context.RequestState.OrthographicSize = size;
                    if (prevSize < size)
                    {
                        context.RequestState.OrthographicSize = Mathf.Max(currentState.OrthographicSize, 0.1f);
                        break;
                    }
                }
            }
            else if (delta > 0)
            {
                for (int i = _stepConfig.stepSizeList.Count-1;  i >= 0; i--)
                {
                    float size = _stepConfig.stepSizeList[i];   
                    context.RequestState.OrthographicSize = size;
                    if (prevSize > size)
                    {
                        context.RequestState.OrthographicSize = Mathf.Max(currentState.OrthographicSize, 0.1f);
                        break;
                    }
                }
            }
        }
    }
}