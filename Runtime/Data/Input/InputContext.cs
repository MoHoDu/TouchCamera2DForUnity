using System;
using System.Collections.Generic;
using CameraBehaviour.DataLayer.State;
using UnityEngine;

namespace CameraBehaviour.DataLayer.Input
{
    [Serializable]
    public class InputContext
    {
        // 스크린 입력 정보
        public float Delta;
        public Vector2 Direction;
        public Vector2 StartPosition;
        public Vector2 EndPosition;

        // 확장 슬롯 (Generic Payload)
        public Dictionary<string, object> ExtraData = new();

        // 이전 카메라 상태 정보
        public CameraState PreviousState;

        // 동작 요청 정보
        public CameraState RequestState;
        public GameObject RequestTargetObject;

        public string GetLogText()
        {
            string targetName = RequestTargetObject?.gameObject != null ? RequestTargetObject?.gameObject?.name : "None";
            string message
                = $@"- 입력 시작 지점: {StartPosition}
                - 입력 종료 지점: {EndPosition} 
                - 입력 방향: {Direction}
                - 변화량: {Delta}
                - 타겟: {targetName}
                (모든 위치값은 스크린 포인트입니다.)";
            foreach (var kvp in ExtraData)
            {
                try
                {
                    string value = JsonUtility.ToJson(kvp.Value);
                    message += $"\n- {kvp.Key}: {value}";
                }
                catch
                {
                    continue;
                }
            }
            return message;
        }
    }
}