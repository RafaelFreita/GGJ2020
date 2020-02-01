using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LUT.Events.Primitives;

abstract public class GameEndController : MonoBehaviour
{

    [SerializeField]
    private EventBool  onBlowStatusChanged = null;
    [SerializeField]
    private EventBool onGameEnd = null;
    protected bool gameFinished = false;

    protected void Start()
    {
        onBlowStatusChanged.Register(OnBlowStatusChange);
    }

    private void OnDestroy()
    {
        onBlowStatusChanged.Unregister(OnBlowStatusChange);
    }

    public abstract void OnBlowStatusChange(bool state);

    public virtual void OnWin()
    {
        gameFinished = true;
        onGameEnd.Invoke(true);
    }
    public virtual void OnLose()
    {
        gameFinished = true;
        onGameEnd.Invoke(false);
    }
}
