using UnityEngine;
using UnityEngine.SceneManagement;

public class TestChangeScene : MonoBehaviour
{
    [SerializeField] protected string _nextSceneName;

    protected virtual void Awake()
    {
        GoNextScene();
    }

    protected void GoNextScene()
    {
        SceneManager.LoadScene(_nextSceneName);
    }
}