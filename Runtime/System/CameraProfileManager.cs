using System;
using System.Collections.Generic;
using CameraBehaviour.DataLayer.Config;
using CameraBehaviour.DataLayer.Config.Input;
using CameraBehaviour.PresentationLayer.Output;
using VContainer;

namespace CameraBehaviour.SystemLayer
{
    public class ActiveBehaviourInfo
    {
        public LinkedList<CameraActionUnit> SortedActionUnits { get; private set; } = new();
    }

    public class CameraProfileManager
    {
        private CameraBehaviourProfile _activeProfile = null;
        private Dictionary<Type, ActiveBehaviourInfo> _activeUnits = new();
        private Dictionary<string, ActiveBehaviourInfo> _callingActionUnits = new();

        public event Action OnChangedProfile;

        private readonly GameServiceBridge _serviceBridge;

        [Inject]
        public CameraProfileManager(
            GameServiceBridge bridge
        )
        {
            _serviceBridge = bridge;
        }

        // 입력 타입으로 호출하는 액션인 경우 (디폴트)
        public ActiveBehaviourInfo GetBehaviour(Type inputType)
        {
            if (!typeof(InputConfigBase).IsAssignableFrom(inputType))
                throw new ArgumentException("Must inherit from InputConfigBase");
            if (_activeUnits.TryGetValue(inputType, out var info))
            {
                if (info == null || info.SortedActionUnits.Count == 0) return null;
                return info;
            }
            return null;
        }

        // 직접 이름으로 호출하는 액션인 경우
        public ActiveBehaviourInfo GetBehaviour(string callingName)
        {
            if (string.IsNullOrEmpty(callingName))
                throw new ArgumentException("callingName can not be null or empty");
            if (_callingActionUnits.TryGetValue(callingName, out var info))
            {
                if (info == null || info.SortedActionUnits.Count == 0) return null;
                return info;
            }
            return null;
        }

        public void SetBehaviourProfile(CameraBehaviourProfile profile)
        {
            if (profile == null) return;
            // 이전 프로필에 관한 액션 유닛들을 모두 제거
            _activeUnits.Clear();
            _callingActionUnits.Clear();

            // 프로필 변경
            _activeProfile = profile;

            // 새로운 프로필의 액션 유닛을 돌면서 타입 별로 링크드 리스트에 삽입
            // ActionUnit.order 순(작은 값이 먼저) > 등록 순 으로 순서를 배치
            if (_activeProfile.actions != null)
            {
                foreach (var unit in _activeProfile.actions)
                {
                    if (unit == null || unit.input == null) continue;
                    // 서비스 브릿지 등록
                    unit.Initialize(_serviceBridge);
                    // 액션 유닛 정보 등록 시작 
                    ActiveBehaviourInfo behaviourInfo = null;
                    if (unit.input is CallDirectConfig callingConfig)
                    {
                        if (!_callingActionUnits.TryGetValue(callingConfig.callingName, out behaviourInfo))
                        {
                            behaviourInfo = new ActiveBehaviourInfo();
                            _callingActionUnits[callingConfig.callingName] = behaviourInfo;
                        }
                    }
                    else
                    {
                        var inputType = unit.input.GetType();
                        if (!_activeUnits.TryGetValue(inputType, out behaviourInfo))
                        {
                            behaviourInfo = new ActiveBehaviourInfo();
                            _activeUnits[inputType] = behaviourInfo;
                        }
                    }

                    var sortedActionUnits = behaviourInfo.SortedActionUnits;

                    var currentNode = sortedActionUnits.First;
                    while (currentNode != null && currentNode.Value.order <= unit.order)
                    {
                        currentNode = currentNode.Next;
                    }

                    if (currentNode != null)
                    {
                        sortedActionUnits.AddBefore(currentNode, unit);
                    }
                    else
                    {
                        sortedActionUnits.AddLast(unit);
                    }
                }
            }

            // 디버거에 디버깅 가능 여부 세팅
            DebugVisualizer.CanLogging = profile.debugLogging;

            // 동작 시 캔슬 등의 처리를 위해 연결된 이벤트 호출
            OnChangedProfile?.Invoke();
        }
    }
}