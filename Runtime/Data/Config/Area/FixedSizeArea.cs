using CameraBehaviour.DataLayer.Input;
using UnityEngine;

namespace CameraBehaviour.DataLayer.Config.Area
{
    public class FixedSizeArea : AreaConfigBase
    {
        [Header("카메라 범위 설정")]
        [FieldLabel("중심점")]
        [SerializeField] Vector2 center;
        [FieldLabel("사이즈")]
        [SerializeField] Vector2 size;

        [Header("적용 범위")]
        [FieldLabel("중심 좌표만 체크")]
        [Tooltip("체크 시에는 카메라 중앙 지점만 확인합니다. 체크하지 않은 경우 카메라 범위 전체에 대해 체크합니다.")]
        [SerializeField] bool checkOnlyCenter;

        public override void Calibrate(InputContext context)
        {
            if (checkOnlyCenter) CalibrateOnlyCenter(context);
            else CalibrateCameraRange(context);
        }

        private void CalibrateOnlyCenter(InputContext context)
        {
            var min = center - size / 2;
            var max = center + size / 2;
            CalibratePosition(context, min, max);
        }

        private void CalibrateCameraRange(InputContext context)
        {
            if (context.RequestState == null) return;

            // 1. 카메라 줌 상태에 따라 유효 범위를 계산합니다.
            float camHeight = context.RequestState.OrthographicSize * 2f;
            float camWidth = camHeight * ((float)Screen.width / Screen.height);

            if (this.size.x < camWidth || this.size.y < camHeight)
            {
                // CamSize 조절
                float aspectRatio = (float)Screen.width / Screen.height;
                float newCamHeight = Mathf.Min(this.size.y, this.size.x / aspectRatio);
                context.RequestState.OrthographicSize = newCamHeight / 2f;
                
                // 보정된 값으로 재계산
                camHeight = context.RequestState.OrthographicSize * 2f;
                camWidth = camHeight * aspectRatio;
            }

            // 2. 카메라 위치를 보정합니다.
            Vector2 effectiveSize = new Vector2(
                Mathf.Max(0, this.size.x - camWidth),
                Mathf.Max(0, this.size.y - camHeight)
            );

            var min = this.center - effectiveSize / 2;
            var max = this.center + effectiveSize / 2;

            // CalibratePosition의 복잡한 로직 대신, 요청된 위치를 유효 범위 내로 직접 제한합니다.
            // 이를 통해 위치에 따른 경계 문제를 해결합니다.
            context.RequestState.Position = new Vector2(
                Mathf.Clamp(context.RequestState.Position.x, min.x, max.x),
                Mathf.Clamp(context.RequestState.Position.y, min.y, max.y)
            );
        }

        private void CalibratePosition(InputContext context, Vector2 min, Vector2 max)
        {
            // CalibrateOnlyCenter를 위해 간단한 Clamp 로직만 남겨둡니다.
            context.RequestState.Position = new Vector2(
                Mathf.Clamp(context.RequestState.Position.x, min.x, max.x),
                Mathf.Clamp(context.RequestState.Position.y, min.y, max.y)
            );
        }
    }
}