using ExampleModule.System;
using ExampleModule.System.Interface;
using VContainer;
using VContainer.Unity;
using UnityEngine;

public class ExampleSceneLifetimeScope : LifetimeScope
{
    [SerializeField] private GameObject _backgroundObj;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterInstance(_backgroundObj);
        builder.Register<IExampleService, ExampleController>(Lifetime.Scoped);
    }

    protected void Start()
    {
        var serviceBridge = Container?.Resolve<GameServiceBridge>();
        var exampleService = Container?.Resolve<IExampleService>();
        serviceBridge?.RegisterExampleService(exampleService);
    }

    protected override void OnDestroy()
    {
        var serviceBridge = Container?.Resolve<GameServiceBridge>();
        serviceBridge?.UnRegisterExampleService();

        base.OnDestroy();
    }
}
