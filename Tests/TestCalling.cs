using CameraBehaviour.DataLayer.Input;
using CameraBehaviour.SystemLayer;
using UnityEngine;

namespace CameraBehaviour.Test
{
    public class TestCalling : MonoBehaviour
    {
        [SerializeField] public string callingName = "testCalling";
        [SerializeField] public InputContext TestInput;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                CameraBehaviourUtil.CallActionUnit(callingName, TestInput);
            }
        }
    }
}