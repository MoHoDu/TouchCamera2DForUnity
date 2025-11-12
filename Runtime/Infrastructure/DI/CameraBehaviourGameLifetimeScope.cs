using CameraBehaviour.SystemLayer;
using VContainer;
using VContainer.Unity;

namespace CameraBehaviour.Infrastructure.DI
{
    public class CameraBehaviourGameLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // 프로필 매니저 등록
            builder.Register<CameraProfileManager>(Lifetime.Singleton);

            // CameraBehaviourController이자 I...Receiver로 등록
            builder.Register<CameraBehaviourController>(Lifetime.Singleton)
                .AsSelf()
                .AsImplementedInterfaces();

            // Call 입력을 처리하기 위한 유틸 클래스
            builder.RegisterEntryPoint<CameraBehaviourUtil>(Lifetime.Singleton);
        }
    }
}