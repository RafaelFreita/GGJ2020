using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LUT.Events.Primitives;

abstract public class GameEndController : MonoBehaviour
{

    [SerializeField]
    private EventBool onBlowStatusChanged;

    protected void Start()
    {
        onBlowStatusChanged.Register(OnBlowStatusChange);
    }

    private void OnDestroy()
    {
        onBlowStatusChanged.Unregister(OnBlowStatusChange);
    }

    public abstract void OnBlowStatusChange(bool state);

    public abstract void OnWin();
    public abstract void OnLose();
}
