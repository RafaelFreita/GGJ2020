using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minigame1Controller : GameEndController
{

    public float blowHoldingTime = 5.0f;

    public Text text;

    private float currentBlowingTime = 0f;
    private bool isBlowing = false;
    private bool gameFinished = false;

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

        if(!isBlowing && currentBlowingTime < blowHoldingTime)
        {
            OnLose();
        }
    }

    public override void OnLose()
    {
        gameFinished = true;
        text.text = "YOU LOSE!";
    }

    public override void OnWin()
    {
        gameFinished = true;
        text.text = "YOU WIN!";
    }
}
