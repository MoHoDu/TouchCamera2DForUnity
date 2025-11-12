using System;
using UnityEngine;
using CameraBehaviour.DataLayer.Config.Action;
using System.Collections.Generic;
using CameraBehaviour.DataLayer.Config.Input;

namespace CameraBehaviour.DataLayer.Config
{
    [Serializable]
    public class CameraActionUnit
    {
        [Header("메타 데이터")]
        [FieldLabel("액션 이름 지정")]
        public string name;         // 가독성을 위한 이름
        [FieldLabel("실행 순서")]
        public int order;           // 실행 순서(또는 우선순위)

        [Header("1) 입력 단계")]
        [SerializeReference] public InputConfigBase input;      // 폴리모픽

        [Header("2) 동작 단계")]
        [SerializeReference, FieldLabel("메인 액션")] public ActionConfigBase action;

        // 외부 서비스 로직 사용 시 필요한 Resolver
        private GameServiceBridge _serviceBridge;

        public void Initialize(GameServiceBridge bridge)
        {
            _serviceBridge = bridge;
            InitializeConfigs();
        }

        public void InitializeConfigs()
        {
            input?.SetResolver(_serviceBridge);
            action?.SetResolver(_serviceBridge);
        }

        public void Validate(List<string> warnings)
        {
            if (input == null)
                warnings.Add($"{name} (ActionUnit): 입력 방식(Input)이 비어있습니다.");

            if (action == null)
                warnings.Add($"{name} (ActionUnit): 메인 동작(Action)이 비어있습니다.");

            input?.Validate(warnings);
            action?.Validate(warnings);
        }
    }
}