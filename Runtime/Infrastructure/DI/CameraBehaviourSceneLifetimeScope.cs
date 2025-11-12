using VContainer;
using VContainer.Unity;
using UnityEngine;
using CameraBehaviour.DataLayer.Config;
using CameraBehaviour.PresentationLayer.Output;
using CameraBehaviour.PresentationLayer.Inputs;

namespace CameraBehaviour.Infrastructure.DI
{
    public class CameraBehaviourSceneLifetimeScope : LifetimeScope
    {
        [Header("프로필 설정")]
        [FieldLabel("씬 액션 프로필")]
        [SerializeField] CameraBehaviourProfile _profile;

        [Header("컴포넌트 등록")]
        [FieldLabel("카메라 동작기")]
        [SerializeField] CameraOutputViewer _cameraViewer;
        [FieldLabel("마우스 입력 감지기")]
        [SerializeField] MouseInputAdapter _mouseInput;
        [FieldLabel("터치스크린 입력 감지기")]
        [SerializeField] TouchInputAdapter _touchInput;

        protected override void Configure(IContainerBuilder builder)
        {
            // 프로필 및 컴포넌트 등록 
            builder.RegisterInstance(_profile);
            builder.RegisterInstance(_cameraViewer);
            builder.RegisterInstance(_mouseInput);
            builder.RegisterInstance(_touchInput);

            // 브릿지 객체
            builder.RegisterEntryPoint<CameraAdapterRegister>(Lifetime.Scoped);
            builder.RegisterEntryPoint<CameraViewerRegister>(Lifetime.Scoped);
            builder.RegisterEntryPoint<CameraProfileRegister>(Lifetime.Scoped);
        }
    }
}