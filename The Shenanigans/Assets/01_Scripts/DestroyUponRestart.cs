using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyUponRestart : MonoBehaviour
{
    private void Restart()
    {
        EventManager.RemoveListener(EventType.Restart, Restart);
        Destroy(gameObject);
    }
    private void OnEnable()
    {
        EventManager.AddListener(EventType.Restart, Restart);
    }
}
