using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager current { get; private set; }

    private void Awake()
    {
        current = this;
    }

    public event Action<string> onGroundTriggerChanged;
    public event Action<EnemyController.state> onEnemyStateChanged;

    public void DoGroundTriggerChange(string tag)
    {
        if (onGroundTriggerChanged != null)
        {
            onGroundTriggerChanged(tag);
        }
    }

    public void DoEnemyStateChange(EnemyController.state state)
    {
        if (onEnemyStateChanged != null)
        {
            onEnemyStateChanged(state);
        }
    }
}
