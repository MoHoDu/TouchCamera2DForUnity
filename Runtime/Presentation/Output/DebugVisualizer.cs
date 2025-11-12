using CameraBehaviour.DataLayer.Input;
using CameraBehaviour.DataLayer.State;
using Unity.Mathematics;
using UnityEngine;

namespace CameraBehaviour.PresentationLayer.Output
{
    public static class DebugVisualizer
    {
        public static bool CanLogging { private get; set; } = true;

        public static void LogInput(InputContext context)
        {
            if (!CanLogging || context == null) return;
            Debug.Log($"[CameraBehaviour - 입력 발생]\n{context.GetLogText()}");
        }

        public static void LogOutput(CameraState prev, CameraState cur)
        {
            if (!CanLogging) return;
            string position = $"위치: {cur.Position} (변경 X)";
            if (Vector3.Distance(prev.Position, cur.Position) != 0)
            {
                position = $"위치: {prev.Position} -> {cur.Position}";
            }
            string rotation = $"각도: {cur.Rotation.eulerAngles} (변경 X)";
            if (!quaternion.Equals(prev.Rotation, cur.Rotation))
            {
                rotation = $"각도: {prev.Rotation.eulerAngles} -> {cur.Rotation.eulerAngles}";
            }
            string size = $"카메라 사이즈: {cur.OrthographicSize} (변경 X)";
            if (!Mathf.Approximately(prev.OrthographicSize, cur.OrthographicSize))
            {
                size = $"카메라 사이즈: {prev.OrthographicSize} -> {cur.OrthographicSize}";
            }
            string enabled = $"활성 여부: {cur.IsEnabled} (변경 X)";
            if (prev.IsEnabled != cur.IsEnabled)
            {
                enabled = $"활성 여부: {prev.IsEnabled} -> {cur.IsEnabled}";
            }
            string message = @$"[CameraBehaviour - 동작 알림]
            {position}
            {rotation}
            {size}
            {enabled}";
            Debug.Log(message);
        }

        public static void Log(string message)
        {
            if (!CanLogging) return;
            Debug.Log($"[CameraBehaviour]\n{message}");
        }
    }
}