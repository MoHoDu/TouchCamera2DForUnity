using ExampleModule.System.Interface;
using UnityEngine;
using VContainer;

namespace ExampleModule.System
{
    public class ExampleController : IExampleService
    {
        private readonly GameObject _backgroundObject;

        [Inject]
        public ExampleController(
            GameObject background
        )
        {
            _backgroundObject = background;
        }

        public (Vector2 center, Vector2 size) GetArea()
        {
            (Vector2 center, Vector2 size) result = (new Vector2(), new Vector2());
            var renderers = _backgroundObject?.GetComponentsInChildren<Renderer>();
            if (renderers.Length > 0)
            {
                var bounds = renderers[0].bounds;
                for (var i = 1; i < renderers.Length; i++)
                {
                    bounds.Encapsulate(renderers[i].bounds);
                }
                result.center = bounds.center;
                result.size = bounds.size;
            }
            else
            {
                result.center = _backgroundObject?.transform.position ?? Vector2.zero;
                result.size = Vector2.zero;
            }
            return result;
        }
    }
}