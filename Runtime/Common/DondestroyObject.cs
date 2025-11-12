using UnityEngine;

public class DontDestoryObject : MonoBehaviour
{
    protected void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}