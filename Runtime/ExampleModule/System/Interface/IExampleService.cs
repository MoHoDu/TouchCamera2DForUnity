using UnityEngine;

namespace ExampleModule.System.Interface
{
    public interface IExampleService
    {
        public (Vector2 center, Vector2 size) GetArea();
    }
}