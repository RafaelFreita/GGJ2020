using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LUT.Events.Primitives;

/// <summary>
///  This is actually base minigame controller
/// </summary>
public abstract class GameEndController : MonoBehaviour
{

    [SerializeField]
    private EventBool  onBlowStatusChanged = null;
    [SerializeField]
    private EventBool onGameEnd = null;
    protected bool isGamePaused = true;

    public List<string> tooltips;
    private GameFlowController cachedGameFlow;

    protected void Start()
    {
		cachedGameFlow = GameFlowController.Instance;
        onBlowStatusChanged.Register(OnBlowStatusChange);
        Countdown.Instantiate(3, "GO!", 1.0f, () => { isGamePaused = false; Debug.Log("GO!!!"); });
    }

    private void OnDestroy()
    {
        onBlowStatusChanged.Unregister(OnBlowStatusChange);
    }

    public virtual float GetNormalizedRemainingTime()
    {
        return 1;
    }

    public virtual string GetRandomMinigameTooltip()
    {
        if (tooltips.Count == 0) return "";
        return tooltips[Random.Range(0, tooltips.Count)];
    }

    public abstract void OnBlowStatusChange(bool state);

    protected virtual int GetScore(bool win)
    {
        return win ? Random.Range(80, 100) : 0;
    }

    public virtual void OnWin()
    {
        isGamePaused = true;
        cachedGameFlow.OnEndGame(true, GetScore(true));
        onGameEnd.Invoke(true);
    }
    public virtual void OnLose()
    {
        isGamePaused = true;
        cachedGameFlow.OnEndGame(false, GetScore(false));
        onGameEnd.Invoke(false);
    }
}
