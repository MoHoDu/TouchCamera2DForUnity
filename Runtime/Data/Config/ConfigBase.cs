using System;
using System.Collections.Generic;
using CameraBehaviour.DataLayer.Config.Validate;

namespace CameraBehaviour.DataLayer.Config
{
    [Serializable]
    public abstract class ConfigBase : IValidatable
    {
        protected string displayName;   // 인스펙터/로그에서 식별용

        public string DisplayName => string.IsNullOrEmpty(displayName) ? displayName : displayName;

        // 외부 서비스 이용 시 필요한 Resolve 등록
        public GameServiceBridge Resolver { get; private set; }
        public virtual void SetResolver(GameServiceBridge resolver) => Resolver = resolver;

        // 기본 유효성 검사 (상속 클래스에서 추가 검사 구현 가능)
        public virtual void Validate(List<string> warnings)
        {
            // 공통 검사 예: (필요 시)
            // if (string.IsNullOrEmpty(DisplayName)) warnings.Add($"{GetType().Name}: 이름이 비어있습니다.");
        }
    }
}