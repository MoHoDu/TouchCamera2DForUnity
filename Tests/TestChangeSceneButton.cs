using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class TestChangeSceneButton : TestChangeScene
{
    private Button _button;

    protected override void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(GoNextScene);
    }
}