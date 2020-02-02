using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BaloonMinigame : GameEndController
{

    public float minTimeFill = 2.0f;
    public float maxTimeFill = 2.5f;
    public int balloonsToFill = 3;
    public float maxMinigameTime = 10.0f;

    // Ballon Image Refs
    [SerializeField] SpriteRenderer balloonSpriteRenderer;
    [SerializeField] Transform ballonRootBone = null;
    [SerializeField] Transform ballonGrowBone = null;
    [SerializeField] AnimationCurve ballonShakingCurve = null;
    [SerializeField] AnimationCurve ballonGrowingCurve = null;

    public Color balloonNotReadyColor;
    public Color balloonReadyColor;
    public Text textBalloonsFilled;
    public Slider slider;

    [SerializeField] private TextMeshProUGUI balloonsCounter;

    [SerializeField] private GameObject explosionObject;
    [SerializeField] private float explosionDuration;


    private float totalMinigameTime = 0.0f;
    private float currentBlowingTime = 0f;
    private int balloonsFilled = 0;

    private bool isBlowing = false;

    private new void Start()
    {
        base.Start();

        balloonsToFill += GameFlowController.Instance.gameIterations / 2;
        maxMinigameTime += (GameFlowController.Instance.gameIterations / 2) * 3.0f;

        balloonsCounter.text = balloonsFilled.ToString() + "/" + balloonsToFill.ToString();

        ResetBalloon();
    }

    private void Update()
    {
        if (isGamePaused)
        {
            return;
        }

        totalMinigameTime += Time.deltaTime;
        if (totalMinigameTime >= maxMinigameTime)
        {
            OnLose();
        }


        if (isBlowing)
        {
            currentBlowingTime += Time.deltaTime;
        }
        else
        {
            currentBlowingTime -= Time.deltaTime;
        }
        currentBlowingTime = Mathf.Max(currentBlowingTime, 0.0f);

        float blowSlider = currentBlowingTime / maxTimeFill;
        if (blowSlider > 1.0f)
        {
            ResetBalloon();
            StartCoroutine(ExplodeBalloon());
            return;
        }
        ballonRootBone.rotation = Quaternion.AngleAxis(90 + Mathf.Sin(ballonShakingCurve.Evaluate(blowSlider) * 145) * 2, Vector3.forward);
        ballonGrowBone.localScale = Vector3.one * (1.0f + ballonGrowingCurve.Evaluate(blowSlider) * 1.8f);
        if (slider)
        {
            slider.value = blowSlider;
        }

        //sliderImage.color = (IsBalloonReady())
        //    ? balloonReadyColor
        //    : balloonNotReadyColor;
    }

    private bool IsBalloonReady()
    {
        // If blowing time is between min and max
        return currentBlowingTime >= minTimeFill && currentBlowingTime <= maxTimeFill;
    }

    private void ResetBalloon()
    {
        currentBlowingTime = 0.0f;
        balloonSpriteRenderer.color = Random.ColorHSV(0f, 1f, .6f, .8f, .7f, .9f);
    }

    private void CheckCurrentBalloon()
    {
        if (isGamePaused) return;

        if (IsBalloonReady())
        {
            balloonsFilled++;
            balloonsCounter.text = balloonsFilled.ToString() + "/" + balloonsToFill.ToString();

            // Check if all balloons were filled
            if (balloonsFilled >= balloonsToFill)
            {
                OnWin();
            }
            else
            {
                ResetBalloon();
            }
        }
    }

    private IEnumerator ExplodeBalloon()
    {
        balloonSpriteRenderer.gameObject.SetActive(false);
        explosionObject.SetActive(true);

        yield return new WaitForSeconds(explosionDuration);

        explosionObject.SetActive(false);
        balloonSpriteRenderer.gameObject.SetActive(true);
    }

    public override void OnBlowStatusChange(bool state)
    {
        // If is blowing and stops, check if balloon is filled
        if (isBlowing && !state)
        {
            CheckCurrentBalloon();
        }

        isBlowing = state;
    }

    public override void OnLose()
    {
        base.OnLose();
    }

    public override void OnWin()
    {
        base.OnWin();
    }


    public override float GetNormalizedRemainingTime()
    {
        return totalMinigameTime / maxMinigameTime;
    }

}
