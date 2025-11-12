using ExampleModule.System.Interface;

public class GameServiceBridge
{
    IExampleService _example;
    public void RegisterExampleService(IExampleService service) => _example = service;
    public void UnRegisterExampleService() => _example = null;
    public IExampleService Example => _example;
}