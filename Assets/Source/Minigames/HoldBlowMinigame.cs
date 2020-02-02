using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class HoldBlowMinigame : GameEndController
{

    public float blowHoldingTime = 5.0f;

    public TextMeshProUGUI text;

    public Animation explosionAnimation;
    public AnimationClip explosionAnimClip;


    public System.Collections.Generic.List<ParticleSystem> sparksSystems = new System.Collections.Generic.List<ParticleSystem>();

    private float currentBlowingTime = 0f;
    private bool isBlowing = false;

    [SerializeField]
    private float sparkSizeChangeSpeed = 1.0f;
    private float sparksSizeMultiplier = 0.0f;

    new private void Start()
    {
        base.Start();
    }

    private void Update()
    {
        text.text = (blowHoldingTime - Mathf.FloorToInt(currentBlowingTime)).ToString();
        if (isBlowing && !isGamePaused)
        {
            UpdateSparksPS();

            currentBlowingTime += Time.deltaTime;
            if (currentBlowingTime >= blowHoldingTime)
            {
                OnWin();
            }
        }
    }

    private void UpdateSparksPS()
    {
        foreach (var p in sparksSystems)
        {
            var psMain = p.main;
            psMain.startSizeMultiplier -= Time.deltaTime * sparkSizeChangeSpeed;
            psMain.startSizeMultiplier = Mathf.Max(psMain.startSizeMultiplier, 1.0f);
        }
    }

    public override void OnBlowStatusChange(bool state)
    {
        isBlowing = state;

        if (!isGamePaused && !isBlowing && currentBlowingTime < blowHoldingTime)
        {
            OnLose();
        }
    }

    public override void OnLose()
    {
        explosionAnimation.Play(explosionAnimClip.name);
        StartCoroutine(WaitForExplosion());
        base.OnLose();
    }

    public override void OnWin()
    {
        base.OnWin();
    }

    private IEnumerator WaitForExplosion()
    {
        yield return new WaitForSeconds(explosionAnimClip.length);
    }
}
