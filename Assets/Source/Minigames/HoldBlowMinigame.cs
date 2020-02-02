using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HoldBlowMinigame : GameEndController
{

    public float blowHoldingTime = 5.0f;

    public TextMeshProUGUI text;

    private float currentBlowingTime = 0f;
    private bool isBlowing = false;

    new private void Start()
    {
        base.Start();

        text.text = $"Blow for {blowHoldingTime} seconds!";
    }

    private void Update()
    {
        if (isBlowing && !gameFinished)
        {
            currentBlowingTime += Time.deltaTime;
            text.text = currentBlowingTime.ToString("F2");
            if (currentBlowingTime >= blowHoldingTime)
            {
                OnWin();
            }
        }
    }

    public override void OnBlowStatusChange(bool state)
    {
        isBlowing = state;

        if (!gameFinished && !isBlowing && currentBlowingTime < blowHoldingTime)
        {
            OnLose();
        }
    }

    public override void OnLose()
    {
        base.OnLose();
    }

    public override void OnWin()
    {
        base.OnWin();
    }
}
